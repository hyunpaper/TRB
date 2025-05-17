using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Application;
using TRB.Server.Application.api.requestbodies;
namespace TRB.Server.Presistance.Repositories.Commands
{
    public class UpdateUserProfileCommand
    {
        public int UserId { get; }
        public UserProfileUpdateRequestDto Dto { get; }

        public UpdateUserProfileCommand(int userId, UserProfileUpdateRequestDto dto)
        {
            UserId = userId;
            Dto = dto;
        }
    }
}
