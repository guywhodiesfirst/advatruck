using System.Net;
using Core.Exceptions;

namespace API.Controllers;

using API.Extensions;
using Asp.Versioning;
using Business.Interfaces;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v{v:apiVersion}/loads")]
[ApiVersion(TmsApiVersion.V1)]
[Authorize]
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

    [Authorize(Roles = "Dispatcher")]
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] LoadCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var dispatcherId = User.GetDispatcherId();

        if (dispatcherId == Guid.Empty)
        {
            throw new TmsException("Dispatcher profile not found", HttpStatusCode.Unauthorized);
        }

        dto.DispatcherId = dispatcherId;

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