using LoginDotnet.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginDotnet.Infra.Security
{
    public static class TokenService
    {
        public async static Task<string> CreateToken(User user, UserManager<User> userManager, IConfiguration _config)
        {
            try
            {
                var role = await userManager.GetRolesAsync(user);
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),

            };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddDays(1),
              signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
    }
}
