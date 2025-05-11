using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRB.Server.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(string email, int roleId);
    }

}
