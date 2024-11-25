using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnesLogic.Models.Other
{
    public class JwtManagerConfig
    {
        public required string Isssuer { get ; set; }
        public required string Audince { get ; set; }
        public required double ExpirationTimeInMinutes { get; set; }
        public required string SecurityKey { get; set; }
    }
}
