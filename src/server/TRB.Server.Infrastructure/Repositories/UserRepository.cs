using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TRB.Server.Domain.Entities;
using TRB.Server.Domain.Interfaces;
using TRB.Server.Infrastructure.Interfaces;


namespace TRB.Server.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.Conn();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT ur.user_id, ur.email, ur.password, ur.role_id, ur.created_at, ur.enabled, ul.role_name
                                    FROM user ur
                                        ,userrole ul
                                    WHERE ur.email = @email
                                      AND ur.role_id = ul.role_id
                                    LIMIT 1";

            var emailParam = command.CreateParameter();
            emailParam.ParameterName = "@email";
            emailParam.Value = email;
            command.Parameters.Add(emailParam);

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new User
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Email = reader["email"].ToString()!,
                Password = reader["password"].ToString()!,
                RoleId = Convert.ToInt32(reader["role_id"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                Enabled = reader["enabled"].ToString()!,
                RoleName = reader["role_name"].ToString(),
            };
        }

        public async Task CreateAsync(User user)
        {
            const string sql = @"INSERT INTO User (email, password, role_id, created_at, enabled)
                                  VALUES (@Email, @Password, @RoleId, @CreatedAt, @Enabled);";

            using var conn = _connectionFactory.Conn();
            await conn.OpenAsync();

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
            cmd.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
            cmd.Parameters.AddWithValue("@Enabled", user.Enabled);

            await cmd.ExecuteNonQueryAsync();
        }

    }
}
