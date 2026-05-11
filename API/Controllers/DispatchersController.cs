namespace API.Controllers;

using Asp.Versioning;
using Business.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/dispatchers")]
[ApiVersion(TmsApiVersion.V1)]
[Authorize(Roles = "Admin")]
public class DispatchersController(IDispatcherService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DispatcherDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DispatcherDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] DispatcherCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var id = await service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { v = "1", id }, id);
    }

    [HttpPut]
    public async Task<ActionResult<DispatcherDto>> Update([FromBody] DispatcherCreateUpdateDto dto, CancellationToken cancellationToken)
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