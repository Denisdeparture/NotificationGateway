using BuisnesLogic.ConstStorage;
using BuisnesLogic.Models;
using BuisnesLogic.Models.Other;
using BuisnesLogic.ServicesInterface.ClientsInterfaces;
using Data.Models;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace BuisnesLogic.Realization.Auth
{
    public class JwtManager : IJwtManager
    {
        private readonly JwtManagerConfig _config;
        public JwtManager(JwtManagerConfig config)
        {
            var p = config.GetType().GetProperties().Where(p => string.IsNullOrWhiteSpace((p.GetValue(config) ?? string.Empty).ToString()) is true).ToArray();
            if (p.Length > 0) throw new ArgumentNullException("values were null: " + string.Join(" ",p.Select(x => x.Name))); 
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        public JwtSecurityToken CreateJwtTokenForUserAsync(ServiceAuthModel user)
        {
            var jwt = new JwtSecurityToken(issuer: _config.Isssuer,
            audience: _config.Audince,
            expires: DateTime.Now.AddMinutes(_config.ExpirationTimeInMinutes),
            claims: GetClaims(user),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecurityKey)), SecurityAlgorithms.HmacSha256)
            );
            return jwt;
        }
        private List<Claim> GetClaims(ServiceAuthModel service)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, service.Login ?? string.Empty),
                new Claim(AppClaimTypes.Password, service.Password)
            };
            return claims;
        }
    }
}
