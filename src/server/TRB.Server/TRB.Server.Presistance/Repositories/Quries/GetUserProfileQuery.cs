using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using TRB.Server.Application.api.requestbodies;
using TRB.Server.Application.DTOs;

namespace TRB.Server.Presistance.Repositories.Quries
{
    public class GetUserProfileQuery : IRequest<UserProfileResponseDto>
    {
        public int UserId { get; }

        public GetUserProfileQuery(int userId)
        {
            UserId = userId;
        }
    }
}
