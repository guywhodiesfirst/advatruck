namespace API.Controllers;

using Asp.Versioning;
using Business.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/loads")]
[ApiVersion(TmsApiVersion.V1)]
[Authorize] // Всі методи потребують авторизації
public class LoadsController(ILoadService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoadDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LoadDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Dispatcher,Admin")]
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] LoadCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var id = await service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { v = "1", id }, id);
    }

    [Authorize(Roles = "Dispatcher,Admin")]
    [HttpPut]
    public async Task<ActionResult<LoadDto>> Update([FromBody] LoadCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(dto, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Dispatcher,Admin")]
    [HttpPatch("status")]
    public async Task<ActionResult<LoadDto>> UpdateStatus([FromBody] LoadStatusUpdateDto dto, CancellationToken cancellationToken)
    {
        var result = await service.UpdateStatusAsync(dto, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Dispatcher,Admin")]
    [HttpPatch("assign-driver")]
    public async Task<ActionResult<LoadDto>> AssignDriver([FromBody] LoadAssignDriverDto dto, CancellationToken cancellationToken)
    {
        var result = await service.AssignDriverAsync(dto, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Dispatcher,Admin")]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}