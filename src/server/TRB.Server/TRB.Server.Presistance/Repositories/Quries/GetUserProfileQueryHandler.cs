using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TRB.Server.Application.DTOs;
using TRB.Server.Domain.Interfaces;

namespace TRB.Server.Presistance.Repositories.Quries
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileResponseDto>
    {
        private readonly IUserRepository _userRepository;

        public GetUserProfileQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserProfileResponseDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _userRepository.GetProfileByUserIdAsync(request.UserId);
            return new UserProfileResponseDto
            {
                Name = profile.Name,
                Nickname = profile.Nickname,
                BirthDate = DateTime.Parse(profile.BirthDate).ToString("yyyy-MM-dd"),
                Gender = profile.Gender.ToString(),
                Address = profile.Address,
                ProfileImage = profile.ProfileImage,
            };
        }
    }
}
