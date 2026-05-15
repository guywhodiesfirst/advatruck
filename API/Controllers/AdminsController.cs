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
[Route("api/v{v:apiVersion}/admins")]
[ApiVersion(TmsApiVersion.V1)]
[Authorize(Roles = "Admin")]
public class AdminsController(IAdminService service) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("me")]
    public async Task<ActionResult<AdminProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email))
        {
            throw new TmsException("User email claim not found in token", HttpStatusCode.Unauthorized);
        }

        var profile = await service.GetProfileByEmailAsync(email, cancellationToken);
        return Ok(profile);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminDto>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AdminDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] AdminCreateUpdateDto dto, CancellationToken cancellationToken)
    {
        var id = await service.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { v = "1", id }, id);
    }

    [HttpPut]
    public async Task<ActionResult<AdminDto>> Update([FromBody] AdminCreateUpdateDto dto, CancellationToken cancellationToken)
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