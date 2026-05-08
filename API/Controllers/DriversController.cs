namespace API.Controllers;

using System.Net;
using System.Security.Claims;
using Asp.Versioning;
using Business.Interfaces;
using Core.Exceptions;
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
            throw new TmsException("User email claim not found in token", HttpStatusCode.Unauthorized);
        }

        var profile = await driverService.GetProfileByEmailAsync(email, cancellationToken);

        return Ok(profile);
    }
}