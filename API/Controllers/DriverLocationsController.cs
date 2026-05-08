namespace API.Controllers;

using Asp.Versioning;
using Business.Interfaces;
using Core.Entities;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/drivers/{driverId:guid}/locations")]
[ApiVersion(TmsApiVersion.V1)]
public class DriverLocationsController(
    IDriverLocationService service)
    : ControllerBase
{
    [HttpGet("last")]
    public async Task<ActionResult<DriverLocation>> GetLastAsync(
        Guid driverId,
        CancellationToken cancellationToken)
    {
        var result = await service.GetLastAsync(driverId, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<DriverLocation>> CreateAsync(
        Guid driverId,
        [FromBody] TrackingUpdateRequestDto request,
        CancellationToken cancellationToken)
    {
        // TODO: OpenStreetMap integration
        var result = await service.AddAsync(
            driverId,
            request,
            cancellationToken);

        return Ok(result);
    }
}