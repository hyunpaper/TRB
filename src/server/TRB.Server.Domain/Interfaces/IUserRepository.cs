using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Domain.Entities;

namespace TRB.Server.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> FindByEmailAsync(string email); // 이름도 의미에 맞게 변경
        Task<bool> InsertUserAndProfileAsync(User user, UserProfile profile);
        Task<(User user, UserProfile? profile, string roleName)?> GetWithProfileByEmailAsync(string email);


    }

}
