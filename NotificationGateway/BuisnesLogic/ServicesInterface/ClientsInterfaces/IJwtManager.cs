using Data.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.ServicesInterface.ClientsInterfaces
{
    public interface IJwtManager
    {
        public JwtSecurityToken CreateJwtTokenForUserAsync(ServiceAuthModel user);
    }
}
