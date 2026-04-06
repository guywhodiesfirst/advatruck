using API.Models;
using Asp.Versioning;
using Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/v{v:apiVersion}/drivers")]
[ApiVersion(TmsApiVersion.V1)]
public class DriversController(IDriverService driverService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var driver = await driverService.LoginAsync(request.Email);

        if (driver == null)
            return Unauthorized();

        return Ok(driver);
    }
}