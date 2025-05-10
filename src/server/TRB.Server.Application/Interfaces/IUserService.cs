using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Application.DTOs;



namespace TRB.Server.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByEmailAsync(string email);
        Task CreateAsync(UserDto dto);
    }

}
