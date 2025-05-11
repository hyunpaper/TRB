using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TRB.Server.Application.Interfaces;
using TRB.Server.Domain.Entities;
using TRB.Server.Domain.Interfaces;
using TRB.Server.Domain.Messages;
 
namespace TRB.Server.Presentation.Consumers;

public class UserSignupConsumer
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserSignupConsumer> _logger;

    public UserSignupConsumer(IUserRepository userRepository, ILogger<UserSignupConsumer> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public void Start()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest"
        };

        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        var queueName = "trb.user.signup";
        channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

        _ = Task.Run(async () =>
        {
            while (true)
            {
                var result = channel.BasicGet(queue: queueName, autoAck: false);
                if (result != null)
                {
                    var body = result.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

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

                        _logger.LogInformation("✅ 사용자 가입 처리 완료: {Email}", message.Email);
                    }

                    channel.BasicAck(result.DeliveryTag, multiple: false);
                }

                await Task.Delay(5000);
            }
        });

        _logger.LogInformation("📡 사용자 가입 consumer 스케줄러 실행 중...");
    }

}
