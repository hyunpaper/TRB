using Microsoft.Extensions.Configuration;


namespace TRB.Server.Infrastructure
{
    public static class DbConfig
    {
        public static string ConnectionString { get; private set; } = string.Empty;

        public static void Initialize(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        }
    }
}
