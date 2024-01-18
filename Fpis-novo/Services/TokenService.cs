using FpisNovoAPI.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FpisNovoAPI.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _key;

        public TokenService(string key)
        {
            _key = key;
        }

        /*public string GenerateReservationToken(int customerId,int userTokenVersion)
         {
             var tokenHandler = new JwtSecurityTokenHandler();
             var keyBytes = Encoding.UTF8.GetBytes(_key);
             var tokenDescriptor = new SecurityTokenDescriptor
             {
                 Subject = new ClaimsIdentity(new[]
                 {
                 new Claim(ClaimTypes.NameIdentifier, customerId.ToString()),
                 new Claim("TokenVersion", userTokenVersion.ToString())
             }),
                 Expires = DateTime.UtcNow.AddHours(1), 
                 SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
             };

             var token = tokenHandler.CreateToken(tokenDescriptor);
             return tokenHandler.WriteToken(token);
         }
        */
         
        public string GenerateReservationToken(int reservationId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, reservationId.ToString())
                
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
