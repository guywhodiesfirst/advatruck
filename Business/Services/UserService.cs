namespace Business.Services;

using System.Net;
using AutoMapper;
using Business.Interfaces;
using Core.Exceptions;
using Core.Identity;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserService(
    UserManager<AppUser> userManager,
    IMapper mapper) : IUserService
{
    public async Task<IEnumerable<UserProfileDto>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        var usersWithRoles = await userManager.Users
            .AsNoTracking()
            .Select(user => new
            {
                User = user,
                Role = userManager.GetRolesAsync(user).Result.FirstOrDefault(),
            })
            .ToListAsync(cancellationToken);

        var userDtos = new List<UserProfileDto>();

        foreach (var item in usersWithRoles)
        {
            var dto = mapper.Map<UserProfileDto>(item.User);
            dto.Role = item.Role ?? "User";
            userDtos.Add(dto);
        }

        return userDtos;
    }

    public async Task<UserProfileDto> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
        {
            throw new TmsException($"User with ID {id} not found", HttpStatusCode.NotFound);
        }

        var dto = mapper.Map<UserProfileDto>(user);
        var roles = await userManager.GetRolesAsync(user);
        dto.Role = roles.FirstOrDefault() ?? "User";

        return dto;
    }
}