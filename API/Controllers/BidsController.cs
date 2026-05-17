namespace API.Controllers;

using API.Extensions;
using Asp.Versioning;
using Business.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/bids")]
[ApiVersion(TmsApiVersion.V1)]
[Authorize]
public class BidsController(IBidService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BidDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BidDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Driver")]
    [HttpPost]
    public async Task<ActionResult<Guid>> PlaceBid([FromBody] BidCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var driverId = User.GetDriverId();

        if (driverId == Guid.Empty)
        {
            return Unauthorized();
        }

        var bidId = await service.UpsertAsync(driverId, dto, cancellationToken);

        return Ok(bidId);
    }

    [Authorize(Roles = "Driver,Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}