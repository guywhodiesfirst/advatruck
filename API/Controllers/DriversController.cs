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
public class DriversController(
    IDriverService driverService,
    ILoadService loadService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DriverDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await driverService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DriverDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await driverService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

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

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] DriverCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var id = await driverService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { v = "1", id }, id);
    }

    [HttpPut]
    public async Task<ActionResult<DriverDto>> Update([FromBody] DriverCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var result = await driverService.UpdateAsync(dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await driverService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id:guid}/active-load")]
    public async Task<ActionResult<LoadDto>> GetActiveLoad(Guid id, CancellationToken cancellationToken)
    {
        var result = await loadService.GetActiveLoadByIdAsync(id, cancellationToken);

        if (result == null)
        {
            return NotFound($"No active or upcoming loads found for driver with ID {id}");
        }

        return Ok(result);
    }
}