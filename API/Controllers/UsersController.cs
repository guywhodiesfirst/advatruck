namespace API.Controllers;

using Asp.Versioning;
using AutoMapper;
using Core.Identity;
using Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v{v:apiVersion}/users")]
[ApiVersion(TmsApiVersion.V1)]
[Authorize(Roles = "Admin")]
public class UsersController(
    UserManager<AppUser> userManager,
    IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await userManager.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var userDtos = new List<UserProfileDto>();

        foreach (var user in users)
        {
            var dto = mapper.Map<UserProfileDto>(user);
            var roles = await userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault();
            userDtos.Add(dto);
        }

        return Ok(userDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserProfileDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
        {
            return NotFound($"User with ID {id} not found");
        }

        var result = mapper.Map<UserProfileDto>(user);
        var roles = await userManager.GetRolesAsync(user);
        result.Role = roles.FirstOrDefault();

        return Ok(result);
    }
}