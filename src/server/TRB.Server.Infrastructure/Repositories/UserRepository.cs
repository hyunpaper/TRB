using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TRB.Server.Domain.Entities;
using TRB.Server.Domain.Interfaces;
using TRB.Server.Infrastructure.Interfaces;
using NLog;
using TRB.Server.Application.DTOs;
using TRB.Server.Application.api.requestbodies;
using TRB.Server.Domain.valueobjects;

namespace TRB.Server.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public ILogger _logger = LogManager.GetCurrentClassLogger();
        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }


        // UserRepository.cs (Infrastructure 계층)
        public async Task<(User user, UserProfile? profile, string roleName)?> GetWithProfileByEmailAsync(string email)
        {
            using var conn = _connectionFactory.Conn();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT ur.user_id, ur.email, ur.password, ur.role_id, ur.created_at, ur.enabled,
                       ul.role_name,
                       up.nickname, up.profile_image
                FROM user ur
                JOIN userrole ul ON ur.role_id = ul.role_id
                LEFT JOIN user_profiles up ON ur.user_id = up.user_id
                WHERE ur.email = @email
                LIMIT 1";

            cmd.Parameters.AddWithValue("@email", email);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            var user = new User
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Email = reader["email"].ToString()!,
                Password = reader["password"].ToString()!,
                RoleId = Convert.ToInt32(reader["role_id"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                Enabled = reader["enabled"].ToString()!
            };

            var profile = new UserProfile
            {
                Nickname = reader["nickname"]?.ToString(),
                ProfileImage = reader["profile_image"]?.ToString()
            };

            var roleName = reader["role_name"].ToString()!;
            return (user, profile, roleName);
        }




        public async Task<User?> FindByEmailAsync(string email)
        {
            using var conn = _connectionFactory.Conn();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT user_id, email, password, role_id, created_at, enabled
                        FROM user
                        WHERE email = @email
                        LIMIT 1";

            cmd.Parameters.AddWithValue("@email", email);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) return null;

            return new User
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Email = reader["email"].ToString()!,
                Password = reader["password"].ToString()!,
                RoleId = Convert.ToInt32(reader["role_id"]),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                Enabled = Convert.ToString(reader["enabled"])
            };
        }


        public async Task<UserLoginResponseDto?> GetByEmailAsync(string email)
        {
            using var connection = _connectionFactory.Conn();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT ur.user_id, ur.email, ur.password, ur.role_id, ur.created_at, ur.enabled,
                                           ul.role_name,
                                           up.nickname, up.profile_image
                                    FROM user ur
                                    JOIN userrole ul ON ur.role_id = ul.role_id
                                    LEFT JOIN user_profiles up ON ur.user_id = up.user_id
                                    WHERE ur.email = @email
                                    LIMIT 1";

            var emailParam = command.CreateParameter();
            emailParam.ParameterName = "@email";
            emailParam.Value = email;
            command.Parameters.Add(emailParam);

            using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new UserLoginResponseDto
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Email = reader["email"].ToString()!,
                RoleId = Convert.ToInt32(reader["role_id"]),
                Role_name = reader["role_name"].ToString()!,
                Nickname = reader["nickname"]?.ToString(),
                ProfileImage = reader["profile_image"]?.ToString()
            };
        }


        // UserRepository.cs
        public async Task<bool> InsertUserAndProfileAsync(User user, UserProfile profile)
        {
            using var conn = _connectionFactory.Conn();
            await conn.OpenAsync();

            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                // 1. users insert
                var userCmd = conn.CreateCommand();
                userCmd.Transaction = transaction;
                userCmd.CommandText = @"
                INSERT INTO user (email, password, role_id, enabled, created_at)
                VALUES (@Email, @Password, @RoleId, @Enabled, @CreatedAt);
                SELECT LAST_INSERT_ID();";
                userCmd.Parameters.AddWithValue("@Email", user.Email);
                userCmd.Parameters.AddWithValue("@Password", user.Password);
                userCmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                userCmd.Parameters.AddWithValue("@Enabled", user.Enabled);
                userCmd.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);

                var userId = Convert.ToInt32(await userCmd.ExecuteScalarAsync());

                // 2. user_profiles insert
                var profileCmd = conn.CreateCommand();
                profileCmd.Transaction = transaction;
                profileCmd.CommandText = @"
                INSERT INTO user_profiles 
                (user_id, name, phone, birth_date, gender, address, nickname, profile_image, created_at, updated_at)
                VALUES 
                (@UserId, @Name, @Phone, @BirthDate, @Gender, @Address, @Nickname, @ProfileImage, NOW(), NOW());";
                profileCmd.Parameters.AddWithValue("@UserId", userId);
                profileCmd.Parameters.AddWithValue("@Name", profile.Name);
                profileCmd.Parameters.AddWithValue("@Phone", profile.Phone);
                profileCmd.Parameters.AddWithValue("@BirthDate", profile.BirthDate);
                profileCmd.Parameters.AddWithValue("@Gender", profile.Gender ?? (object)DBNull.Value);
                profileCmd.Parameters.AddWithValue("@Address", profile.Address ?? (object)DBNull.Value);
                profileCmd.Parameters.AddWithValue("@Nickname", profile.Nickname ?? (object)DBNull.Value);
                profileCmd.Parameters.AddWithValue("@ProfileImage", profile.ProfileImage ?? (object)DBNull.Value);

                await profileCmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.Log(LogLevel.Error, ex, "회원가입 트랜잭션 실패");
                return false;
            }
        }
        public async Task<bool> UpdateUserProfileAsync(int userId, UserProfileValue value)
        {
            using var conn = _connectionFactory.Conn();
            await conn.OpenAsync();

            using var command = conn.CreateCommand();
            var updates = new List<string>();

            // 파라미터 추가 도우미
            void AddParam(string name, object? value)
            {
                command.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }

            if (value.BirthDate != null)
            {
                updates.Add("birth_date = @BirthDate");
                AddParam("@BirthDate", value.BirthDate);
            }

            if (!string.IsNullOrEmpty(value.Gender))
            {
                updates.Add("gender = @Gender");
                AddParam("@Gender", value.Gender);
            }

            if (!string.IsNullOrEmpty(value.Address))
            {
                updates.Add("address = @Address");
                AddParam("@Address", value.Address);
            }

            if (!string.IsNullOrEmpty(value.Nickname))
            {
                updates.Add("nickname = @Nickname");
                AddParam("@Nickname", value.Nickname);
            }

            if (!string.IsNullOrEmpty(value.ProfileImage))
            {
                updates.Add("profile_image = @ProfileImage");
                AddParam("@ProfileImage", value.ProfileImage);
            }

            if (!updates.Any())
                return false;

            updates.Add("updated_at = NOW()");

            command.CommandText = $@"
                                    UPDATE user_profiles
                                    SET {string.Join(", ", updates)}
                                    WHERE user_id = @UserId";

            AddParam("@UserId", userId);

            var affected = await command.ExecuteNonQueryAsync();
            return affected > 0;
        }

        public async Task<UserProfile?> GetProfileByUserIdAsync(int userId)
        {
            using var conn = _connectionFactory.Conn();
            await conn.OpenAsync();

            using var command = conn.CreateCommand();
            command.CommandText = @"
                                    SELECT user_id, name, phone, birth_date, gender, address, nickname, profile_image, created_at, updated_at
                                    FROM user_profiles
                                    WHERE user_id = @UserId
                                    LIMIT 1";

            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            return new UserProfile
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Name = reader["name"].ToString() ?? "",
                Phone = reader["phone"].ToString() ?? "",
                BirthDate = reader["birth_date"].ToString() ?? "",
                Gender = reader["gender"] == DBNull.Value ? null : Convert.ToChar(reader["gender"]),
                Address = reader["address"]?.ToString(),
                Nickname = reader["nickname"]?.ToString(),
                ProfileImage = reader["profile_image"]?.ToString(),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                UpdatedAt = Convert.ToDateTime(reader["updated_at"])
            };
        }



    }
}
