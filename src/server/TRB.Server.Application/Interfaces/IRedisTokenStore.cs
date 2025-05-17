using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRB.Server.Application.Interfaces
{
    public interface IRedisTokenStore
    {
        Task StoreRefreshTokenAsync(int userId, string token, TimeSpan expires);
        Task<string?> GetRefreshTokenAsync(int userId);
        Task<bool> RemoveRefreshTokenAsync(int userId);
    }

}
