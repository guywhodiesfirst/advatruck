namespace API.Controllers;

using System.Net;
using Asp.Versioning;
using Business.Interfaces;
using Core.Entities;
using Core.Exceptions;
using Core.Identity;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegistrationRequestDto dto)
    {
        if (await userManager.FindByEmailAsync(dto.Email) != null)
        {
            throw new TmsException("Email is already taken", HttpStatusCode.BadRequest);
        }

        var user = new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            RegistrationDate = DateTime.UtcNow,
        };

        var result = await userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errorDetails = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new TmsException($"Registration failed: {errorDetails}", HttpStatusCode.BadRequest);
        }

        var roleResult = await userManager.AddToRoleAsync(user, dto.Role);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            throw new TmsException("Invalid role or role assignment failed", HttpStatusCode.BadRequest);
        }

        if (dto.Role.Equals("Driver", StringComparison.OrdinalIgnoreCase))
        {
            user.Driver = new Driver { UserId = user.Id };
        }
        else if (dto.Role.Equals("Dispatcher", StringComparison.OrdinalIgnoreCase))
        {
            user.Dispatcher = new Dispatcher { UserId = user.Id };
        }
        else if (dto.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            user.Admin = new Admin { UserId = user.Id };
        }

        await userManager.UpdateAsync(user);

        var entityId = user.Driver?.Id ?? user.Dispatcher?.Id ?? user.Id;

        return Ok(new AuthResponseDto(
            entityId,
            await tokenService.CreateToken(user, dto.Role)));
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
            throw new TmsException("Invalid email or password", HttpStatusCode.Unauthorized);
        }

        var result = await userManager.CheckPasswordAsync(user, dto.Password);

        if (!result)
        {
            throw new TmsException("Invalid email or password", HttpStatusCode.Unauthorized);
        }

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "User";
        var entityId = user.Driver?.Id ?? user.Dispatcher?.Id ?? user.Admin?.Id ?? user.Id;

        return Ok(new AuthResponseDto(
            entityId,
            await tokenService.CreateToken(user, role)));
    }
}