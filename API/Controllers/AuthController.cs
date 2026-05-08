namespace API.Controllers;

using Asp.Versioning;
using Business.Interfaces;
using Core.Entities;
using Core.Identity;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v{v:apiVersion}/auth")]
[ApiVersion(TmsApiVersion.V1)]
public class AuthController(
    UserManager<AppUser> userManager,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegistrationRequestDto dto)
    {
        if (await userManager.FindByEmailAsync(dto.Email) != null)
        {
            return BadRequest("Email is taken");
        }

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
        };

        if (dto.Role.Equals("Driver", StringComparison.OrdinalIgnoreCase))
        {
            user.Driver = new Driver();
        }
        else if (dto.Role.Equals("Dispatcher", StringComparison.OrdinalIgnoreCase))
        {
            user.Dispatcher = new Dispatcher();
        }
        else
        {
            return BadRequest("Invalid role. Use 'Driver' or 'Dispatcher'");
        }

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        var entityId = user.Driver?.Id ?? user.Dispatcher?.Id ?? user.Id;

        return new AuthResponseDto(
            entityId,
            tokenService.CreateToken(user, dto.Role));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto dto)
    {
        var user = await userManager.Users
            .Include(u => u.Driver)
            .Include(u => u.Dispatcher)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email or password");
        }

        var result = await userManager.CheckPasswordAsync(user, dto.Password);

        if (!result)
        {
            return Unauthorized();
        }

        var role = user.Driver != null ? "Driver" : "Dispatcher";

        var entityId = user.Driver?.Id ?? user.Dispatcher?.Id ?? user.Id;

        return new AuthResponseDto(
            entityId,
            tokenService.CreateToken(user, role));
    }
}