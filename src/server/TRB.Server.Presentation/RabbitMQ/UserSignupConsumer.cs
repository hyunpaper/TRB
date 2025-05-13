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
        private readonly string[] _retryTargets = { "user.signup.q1", "user.signup.q2", "user.signup.q3" };

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

                     // DLQ Consumer를 BasicConsume 방식으로 구현
            Task.Run(() =>
            {
                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare("user.signup.retry", durable: true, exclusive: false, autoDelete: false,
                arguments: new Dictionary<string, object>
                {
                        { "x-dead-letter-exchange", "" },
                        { "x-dead-letter-routing-key", "user.signup.q1" }, 
                        { "x-message-ttl", 10000 } // 10초 후 다시 본 큐로 이동
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

                        var headers = ea.BasicProperties?.Headers;
                        var deathCount = 0;

                        if (headers != null && headers.ContainsKey("x-death"))
                        {
                            var xDeath = headers["x-death"] as List<object>;
                            if (xDeath?.FirstOrDefault() is Dictionary<string, object> deathInfo &&
                                deathInfo.ContainsKey("count"))
                            {
                                deathCount = Convert.ToInt32(deathInfo["count"]);
                            }
                        }

                        if (deathCount >= 2)
                        {
                            var log = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] DLQ 재시도 초과 메시지 삭제됨\n{json}\n";
                            File.AppendAllText(_dlqLogPath, log);
                            _logger.LogWarning(" 재시도 초과 → 삭제됨. 메시지 내용은 로그에 기록됨.");
                            channel.BasicAck(ea.DeliveryTag, false);
                            return;
                        }


                        channel.BasicPublish(
                            exchange: "",
                            routingKey: "user.signup.retry",
                            basicProperties: ea.BasicProperties,
                            body: body
                        );

                        _logger.LogInformation("⏳ 메시지를 DelayQueue로 재전송함");
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "DLQ 처리 중 오류");
                    }
                };

                channel.BasicConsume(queue: "user.signup.dlq", autoAck: false, consumer: consumer);
                _logger.LogInformation(" DLQ Consumer 시작됨");

                while (true) Thread.Sleep(Timeout.Infinite);
            });

            foreach (var queueName in _queueNames)
            {
                var currentQueue = queueName;

                Task.Run(() =>
                {
                    using var connection = factory.CreateConnection();
                    using var channel = connection.CreateModel();

                    channel.QueueDeclare(
                        queue: queueName,
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

                                await _userRepository.CreateAsync(new User
                                {
                                    Email = message.Email,
                                    Password = message.Password,
                                    RoleId = message.RoleId,
                                    CreatedAt = DateTime.UtcNow,
                                    Enabled = "Y"
                                });

                                _logger.LogInformation(" 가입 처리 완료: {Email} (from {Queue})", message.Email, currentQueue);
                            }

                            channel.BasicAck(ea.DeliveryTag, multiple: false);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, " {Queue} 처리 중 오류", currentQueue);
                        }
                    };

                    channel.BasicConsume(queue: currentQueue, autoAck: false, consumer: consumer);
                    _logger.LogInformation(" {Queue} Consumer 시작됨", currentQueue);

                    while (true) Thread.Sleep(Timeout.Infinite);
                });
            }

            _logger.LogInformation(" 모든 가입 Consumer 스케줄러가 시작되었습니다.");
        }
    }
}