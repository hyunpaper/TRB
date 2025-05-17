using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Domain.Entities;
using TRB.Server.Domain.valueobjects;

namespace TRB.Server.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> FindByEmailAsync(string email); 
        Task<bool> InsertUserAndProfileAsync(User user, UserProfile profile);
        Task<(User user, UserProfile? profile, string roleName)?> GetWithProfileByEmailAsync(string email);
        Task<bool> UpdateUserProfileAsync(int userId, UserProfileValue value);
    }

}
