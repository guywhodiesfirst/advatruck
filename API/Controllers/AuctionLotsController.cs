namespace API.Controllers;

using Asp.Versioning;
using Business.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/auctionLots")]
[ApiVersion(TmsApiVersion.V1)]
[Authorize]
public class AuctionLotsController(IAuctionLotService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuctionLotDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuctionLotDto>>> GetAllActive(CancellationToken cancellationToken)
    {
        var result = await service.GetAllActiveAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuctionLotDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Dispatcher,Admin")]
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] AuctionLotCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var id = await service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { v = "1", id }, id);
    }

    [Authorize(Roles = "Dispatcher,Admin")]
    [HttpPut]
    public async Task<ActionResult<AuctionLotDto>> Update([FromBody] AuctionLotCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(dto, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

    [Authorize(Roles = "Dispatcher,Admin")]
    [HttpPatch("{id:guid}/status")]
    public async Task<ActionResult<AuctionLotDto>> UpdateStatus(
        Guid id,
        [FromBody] AuctionLotStatusUpdateDto dto,
        CancellationToken cancellationToken)
    {
        return await service.UpdateStatusAsync(id, dto.Status, cancellationToken);
    }
}