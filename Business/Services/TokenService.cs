namespace Business.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Business.Interfaces;
using Core.Identity;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class TokenService(IConfiguration config, TmsDataContext context) : ITokenService
{
    public async Task<string> CreateToken(AppUser user, string role)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(ClaimTypes.Role, role),
        };

        if (role == "Dispatcher")
        {
            var dispatcher = await context.Dispatchers
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (dispatcher != null)
            {
                claims.Add(new Claim("dispatcher_id", dispatcher.Id.ToString()));
            }
        }
        else if (role == "Driver")
        {
            var driver = await context.Drivers
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (driver != null)
            {
                claims.Add(new Claim("driver_id", driver.Id.ToString()));
            }
        }
        else if (role == "Admin")
        {
            var admin = await context.Admins
                .FirstOrDefaultAsync(d => d.UserId == user.Id);

            if (admin != null)
            {
                claims.Add(new Claim("admin_id", admin.Id.ToString()));
            }
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds,
            Issuer = config["Jwt:Issuer"],
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}