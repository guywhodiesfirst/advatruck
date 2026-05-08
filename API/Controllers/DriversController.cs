namespace API.Controllers;

using System.Security.Claims;
using Asp.Versioning;
using Business.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/drivers")]
[ApiVersion(TmsApiVersion.V1)]
public class DriversController(IDriverService driverService) : ControllerBase
{
    [Authorize(Roles = "Driver")]
    [HttpGet("me")]
    public async Task<ActionResult<DriverProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized();
        }

        var profile = await driverService.GetProfileByEmailAsync(email, cancellationToken);

        if (profile == null)
        {
            return NotFound("Driver profile not found");
        }

        return Ok(profile);
    }
}