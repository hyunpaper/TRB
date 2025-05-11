using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TRB.Server.Domain.Messages;

namespace TRB.Server.Application.Interfaces
{
    public interface IUserSignupPublisher
    {
        Task PublishAsync(UserSignupMessage message);
    }

}
