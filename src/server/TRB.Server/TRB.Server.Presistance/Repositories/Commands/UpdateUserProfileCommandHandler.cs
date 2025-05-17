using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Domain.Interfaces;
using TRB.Server.Domain.valueobjects;

namespace TRB.Server.Presistance.Repositories.Commands
{
    public class UpdateUserProfileCommandHandler
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> HandleAsync(UpdateUserProfileCommand command)
        {
            var dto = command.Dto;
            var value = new UserProfileValue(
                dto.BirthDate,
                dto.Gender,
                dto.Address,
                dto.Nickname,
                dto.ProfileImage
            );

            return await _userRepository.UpdateUserProfileAsync(command.UserId, value);
        }
    }
}
