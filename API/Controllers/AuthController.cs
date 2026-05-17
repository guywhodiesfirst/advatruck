namespace API.Controllers;

using Asp.Versioning;
using Business.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/auth")]
[ApiVersion(TmsApiVersion.V1)]
public class AuthController(IAuthService authService) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegistrationRequestDto dto)
    {
        var response = await authService.RegisterAsync(dto);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var response = await authService.LoginAsync(dto);
        return Ok(response);
    }
}