using BuisnesLogic.Models;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using Data.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Realization.Auth
{
    public static class JwtCreator
    {
        public static string CreateToken(ServiceAuthModel model, IJwtManager manager)
        {
            if(model is null) throw new ArgumentNullException(nameof(model)); 
            if(manager is null) throw new ArgumentNullException(nameof(manager));
            var jwt = manager.CreateJwtTokenForUserAsync(model);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;
        }
    }
}
