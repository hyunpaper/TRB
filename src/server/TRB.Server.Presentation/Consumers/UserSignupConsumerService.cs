using TRB.Server.Domain.Interfaces;
using TRB.Server.Domain.Messages;
using TRB.Server.Infrastructure.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TRB.Server.Domain.Entities;
using TRB.Server.Infrastructure.Messaging.Dequeuer;

namespace TRB.Server.Presentation.Consumers
{
    public class UserSignupConsumerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly List<RabbitDequeuer<UserSignupMessage>> _dequeuers = new();
        private readonly ILogger<UserSignupConsumerService> _logger;

        public UserSignupConsumerService(IServiceProvider serviceProvider, ILogger<UserSignupConsumerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _logger.LogWarning("🌀 UserSignupConsumerService 인스턴스 생성됨");

            var queueNames = QueueNaming.GetAllQueueNamesFor<UserSignupMessage>();
            foreach (var queueName in queueNames)
            {
                try
                {
                    var dequeuer = new RabbitDequeuer<UserSignupMessage>(queueName);
                    dequeuer.AsyncMessageReceived += HandleMessageAsync;
                    dequeuer.RetryFailed += message =>
                    {
                        _logger.LogWarning("❌ 최대 재시도 초과로 폐기된 메시지: {Email}", message.Email);
                    };
                    _dequeuers.Add(dequeuer);
                }
                catch (Exception ex)
                {

                }
            }
        }

        private async Task<bool> HandleMessageAsync(UserSignupMessage message)
        {
            using var scope = _serviceProvider.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            try
            {
                var user = new User
                {
                    Email = message.Email,
                    Password = message.Password,
                    RoleId = message.RoleId,
                    Enabled = "Y",
                    CreatedAt = DateTime.UtcNow
                };

                var profile = new UserProfile
                {
                    Name = message.Name,
                    Phone = message.Phone,
                    BirthDate = message.BirthDate.ToString("yyyy-MM-dd"),
                    Gender = message.Gender,
                    Address = message.Address,
                    Nickname = message.Nickname,
                    ProfileImage = message.ProfileImage
                };

                var result = await userRepository.InsertUserAndProfileAsync(user, profile);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ UserSignup 처리 실패");
                return false;
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var dq in _dequeuers)
            {
                dq.Start();
            }

            return Task.CompletedTask;
        }
    }
}
