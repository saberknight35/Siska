using Siska.Admin.Model.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Siska.Admin.Utility
{
    public class TokenGenerator
    {
        private readonly byte[] _accessTokenSecret;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenGenerator(IConfiguration config)
        {
            _accessTokenSecret = Encoding.ASCII.GetBytes(config.GetValue<string>("Jwt:AccessTokenSecret"));
            _issuer = config.GetValue<string>("Jwt:Issuer");
            _audience = config.GetValue<string>("Jwt:Audience");
        }

        public string GenerateAccessToken(Users user, IList<string> roles)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    }
                ),
                Expires = DateTime.UtcNow.AddDays(30),//.AddMinutes(15),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_accessTokenSecret),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = _issuer,
                Audience = _audience,
            };

            tokenDescriptor.Subject.AddClaims(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}
