using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskWave_MVC.Services
{
    public class JwtService
    {
        public (string id, string role) CheckJWTToken(string token)
        {
            if (token != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes("TaskWave2023_11092003TaskWave2023_11092003");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var userRole = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;
                return (id: userId, role: userRole);
            }

            return (null, null);
        }
        public string GenerateJwtToken(int? userId, Models.ENUMs.RoleENUM? role)
        {
            string id = userId.ToString() ?? "-1";
            string roleSTR = "";
            switch (role)
            {
                case Models.ENUMs.RoleENUM.USER:
                    {
                        roleSTR = "USER";
                        break;
                    }
                case Models.ENUMs.RoleENUM.ADMIN:
                    {
                        roleSTR = "ADMIN";
                        break;
                    }
                case Models.ENUMs.RoleENUM.SUPERUSER:
                    {
                        roleSTR = "SUPERUSER";
                        break;
                    }
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TaskWave2023_11092003TaskWave2023_11092003")); // Замените на свой секретный ключ

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, id), // ID пользователя
                new Claim(ClaimTypes.Role, roleSTR) // Роль пользователя
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Время действия токена
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
