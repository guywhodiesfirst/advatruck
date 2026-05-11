namespace API.Controllers;

using Asp.Versioning;
using Business.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/auctionLots")]
[ApiVersion(TmsApiVersion.V1)]
public class AuctionLotsController(IAuctionLotService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuctionLotDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuctionLotDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] AuctionLotCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var id = await service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { v = "1", id }, id);
    }

    [HttpPut]
    public async Task<ActionResult<AuctionLotDto>> Update([FromBody] AuctionLotCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(dto, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}