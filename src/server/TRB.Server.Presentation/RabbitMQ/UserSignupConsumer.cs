using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TRB.Server.Domain.Entities;
using TRB.Server.Domain.Interfaces;
using TRB.Server.Domain.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using TRB.Server.Infrastructure.Interfaces;

namespace TRB.Server.Presentation.Consumers
{
    public class UserSignupConsumer
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserSignupConsumer> _logger;
        private readonly IConfiguration _config;
        private readonly IRabbitMQFactory _rabbitMQFactory;
        private int _roundRobinCounter = 0;
        private readonly string[] _queueNames = { "user.signup.q1", "user.signup.q2", "user.signup.q3" };
        private readonly string _dlqLogPath = "MessageDLQ.log";

        public UserSignupConsumer(IUserRepository userRepository, ILogger<UserSignupConsumer> logger, IConfiguration config, IRabbitMQFactory rabbitMQFactory)
        {
            _userRepository = userRepository;
            _logger = logger;
            _config = config;
            _rabbitMQFactory = rabbitMQFactory;
        }

        public void Start()
        {
            var factory = _rabbitMQFactory.Conn();

            using var connectionForDLX = factory.CreateConnection();
            using var dlxChannel = connectionForDLX.CreateModel();
            dlxChannel.ExchangeDeclare("dlx.user.signup", ExchangeType.Direct);
            dlxChannel.QueueDeclare("user.signup.dlq", durable: true, exclusive: false, autoDelete: false);
            foreach (var q in _queueNames)
            {
                dlxChannel.QueueBind("user.signup.dlq", "dlx.user.signup", q);
            }

            // DLQ Consumer
            Task.Run(() =>
            {
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare("user.signup.retry", durable: true, exclusive: false, autoDelete: false,
                    arguments: new Dictionary<string, object>
                    {
                { "x-dead-letter-exchange", "" },
                { "x-dead-letter-routing-key", "user.signup.q1" },
                { "x-message-ttl", 10000 }
                    });

                channel.QueueDeclare("user.signup.dlq", durable: true, exclusive: false, autoDelete: false);
                channel.BasicQos(0, 1, false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);

                        int deathCount = 0;
                        if (ea.BasicProperties?.Headers != null &&
                            ea.BasicProperties.Headers.TryGetValue("x-death", out var deathHeader) &&
                            deathHeader is List<object> xDeathList &&
                            xDeathList.FirstOrDefault() is Dictionary<string, object> xDeath &&
                            xDeath.TryGetValue("count", out var countObj))
                        {
                            deathCount = Convert.ToInt32(countObj);
                        }

                        if (deathCount >= 3)
                        {
                            try
                            {
                                File.AppendAllText(_dlqLogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] DLQ 재시도 초과 메시지 삭제됨\n{json}\n");
                                _logger.LogWarning("🚫 DLQ 3회 초과 → 삭제됨");
                            }
                            catch (Exception logEx)
                            {
                                _logger.LogError(logEx, "DLQ 로그 쓰기 실패");
                            }

                            try
                            {
                                channel.BasicAck(ea.DeliveryTag, false);
                                _logger.LogInformation("✅ DLQ Ack 완료 → 메시지 삭제됨");
                            }
                            catch (Exception ackEx)
                            {
                                _logger.LogError(ackEx, "DLQ Ack 실패");
                            }

                            return;
                        }

                        channel.BasicPublish("", "user.signup.retry", ea.BasicProperties, body);
                        _logger.LogInformation("🔁 DLQ → retry 전송 완료");
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "DLQ 처리 중 오류");
                    }
                };

                channel.BasicConsume("user.signup.dlq", autoAck: false, consumer: consumer);
                _logger.LogInformation("🐰 DLQ Consumer 시작됨");

                while (true) Thread.Sleep(Timeout.Infinite);
            });

            // 본 큐 Consumer
            foreach (var queueName in _queueNames)
            {
                var currentQueue = queueName;
                Task.Run(() =>
                {
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();

                    channel.QueueDeclare(
                        queue: currentQueue,
                        durable: true,
                        exclusive: false,
                        autoDelete: false,
                        arguments: new Dictionary<string, object>
                        {
                    { "x-dead-letter-exchange", "dlx.user.signup" }
                        });

                    channel.BasicQos(0, 1, false);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += async (model, ea) =>
                    {
                        try
                        {
                            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                            _logger.LogInformation("📩 수신한 메시지: {Json}", json);

                            var message = JsonSerializer.Deserialize<UserSignupMessage>(json);

                            if (message is not null)
                            {
                                var user = new User
                                {
                                    Email = message.Email,
                                    Password = message.Password,
                                    RoleId = message.RoleId,
                                    CreatedAt = DateTime.UtcNow,
                                    Enabled = "Y"
                                };

                                var profile = new UserProfile
                                {
                                    Name = message.Name,
                                    Phone = message.Phone,
                                    BirthDate = message.BirthDate,
                                    Gender = message.Gender,
                                    Address = message.Address,
                                    Nickname = message.Nickname,
                                    ProfileImage = message.ProfileImage
                                };

                                var result = await _userRepository.InsertUserAndProfileAsync(user, profile);

                                if (!result)
                                {
                                    _logger.LogWarning("🚨 가입 트랜잭션 실패, DLQ로 이동 예정: {Email}", message.Email);
                                    throw new Exception("회원가입 실패");
                                }

                                _logger.LogInformation("✅ 가입 처리 완료: {Email}", message.Email);
                            }

                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "❌ {Queue} 처리 중 오류", currentQueue);
                            try
                            {
                                // DLX로 이동 유도
                                channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                            }
                            catch (Exception nackEx)
                            {
                                _logger.LogError(nackEx, "❌ Nack 중 오류 발생");
                            }
                        }
                    };

                    channel.BasicConsume(queue: currentQueue, autoAck: false, consumer: consumer);
                    _logger.LogInformation("🐇 {Queue} Consumer 시작됨", currentQueue);

                    while (true) Thread.Sleep(Timeout.Infinite);
                });
            }

            _logger.LogInformation("🚀 모든 가입 Consumer 스케줄러 시작 완료");
        }
    }
}