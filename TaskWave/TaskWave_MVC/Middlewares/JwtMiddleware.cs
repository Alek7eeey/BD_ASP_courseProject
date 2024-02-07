using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskWave_MVC.Services;

namespace TaskWave_MVC.Middlewares
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtService _jwtService;

        public JwtMiddleware(RequestDelegate next, JwtService jwtService)
        {
            _next = next;
            _jwtService = jwtService;
        }

        public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
        {

            var token = context.Request.Cookies["jwtToken"];

            if (token != null && token != "")
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes("TaskWave2023_11092003TaskWave2023_11092003");

                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var userId = jwtToken.Claims.Last(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    var userRole = jwtToken.Claims.Last(x => x.Type == ClaimTypes.Role).Value;

                    context.Items["UserId"] = userId;
                    context.Items["UserRole"] = userRole;
                }
                catch (Exception ex)
                {
                    context.Items["UserId"] = null;
                    context.Items["UserRole"] = null;
                }

            }
            else
            {
                context.Items["UserId"] = null;
                context.Items["UserRole"] = null;
            }

            await _next.Invoke(context);
        }
    }
}
