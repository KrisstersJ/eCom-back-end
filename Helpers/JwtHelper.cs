using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace eCom.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateJwtToken(IConfiguration configuration, string email)
        {
            string jwtSecret = configuration["Jwt:SecretKey"];
            int jwtExpiresInMinutes = configuration.GetValue<int>("Jwt:ExpiresInMinutes");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtExpiresInMinutes),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);



            return tokenString;
        }
    }
}
