using JWTAuthentication.Data;
using JWTAuthentication.Data.Models;
using JWTAuthentication.Helpers;
using JWTAuthentication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JWTAuthentication.DTOs;
using Helpers;

namespace JWTAuthentication.Services;

public interface IPrivilegedUserService
{
    Task<ServerResponse> BlockAsync(string userId);
    Task<ServerResponse> AddToRoleAsync(string userId);
    Task<ServerResponse> RemoveFromRoleAsync(string userId);
    Task<PageResult<UserShortInfoDTO>> GetUsersAsync(GetUsersPaginationViewModel model);
}

public class PrivilegedUserService : IPrivilegedUserService
{
    private readonly UserManager<User> userManager;
    private readonly ApplicationDbContext dbContext;

    public PrivilegedUserService(UserManager<User> userManager, ApplicationDbContext dbContext)
    {
        this.userManager = userManager;
        this.dbContext = dbContext;
    }

    public async Task<ServerResponse> BlockAsync(string userId) // DONE. 
    {
        User? user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return new ServerResponse { Message = "User was not found!", IsSuccess = false };
        }

        user.IsLocked = true;
        user.LockDate = DateTime.UtcNow;
        string userName = user.UserName;
        user.UserName = "*locked*";

        string imageDirectory = user.ImageDirectory ?? "/UserIcons/No-icon-image.png";

        user.ImageDirectory = null;

        IList<string> roles = await userManager.GetRolesAsync(user);

        await userManager.RemoveFromRolesAsync(user, roles);
        
        await userManager.UpdateAsync(user);

        await dbContext.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"{imageDirectory}|{userName}|{user.Email}",
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> AddToRoleAsync(string userId)
    {
        User? user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return new ServerResponse { Message = "User was not found!", IsSuccess = false };
        }

        await userManager.RemoveFromRoleAsync(user, UserRoles.User);

        await userManager.AddToRoleAsync(user, UserRoles.Admin);

        await dbContext.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"<p><strong>Dear {user.UserName}, you have got a new role: {UserRoles.Admin}. Congratulations ;)</strong></p>Д{user.Email}",
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> RemoveFromRoleAsync(string userId)
    {
        User? user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return new ServerResponse { Message = "User was not found!", IsSuccess = false };
        }

        await userManager.RemoveFromRoleAsync(user, UserRoles.Admin);

        await userManager.AddToRoleAsync(user, UserRoles.User);

        await dbContext.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"<p>Dear {user.UserName}, you have been demoted from {UserRoles.Admin} to {UserRoles.User}.</p><br>Д{user.Email}",
            IsSuccess = true
        };
    }

    public async Task<PageResult<UserShortInfoDTO>> GetUsersAsync(GetUsersPaginationViewModel model)
    {
        IQueryable<User>? query = userManager.Users.Where(u => !u.IsLocked)
            .AsQueryable();

        if (!string.IsNullOrEmpty(model.SearchText))
        {
            model.SearchText = model.SearchText.ToLower();
            query = query.Where(u => u.UserName.ToLower().Contains(model.SearchText));
        }

        int totalCount = query.Count();

        if (model.Page.HasValue)
        {
            query = query.Skip((model.Page.Value - 1) * model.PageSize);
        }

        query = query.Take(model.PageSize);

        List<UserShortInfoDTO> users = await query.Select(u => new UserShortInfoDTO
        {
            Id = u.Id,
            UserName = u.UserName
        }).ToListAsync();

        foreach (UserShortInfoDTO u in users)
        {
            User? user = query.FirstOrDefault(U => U.UserName.Equals(u.UserName));

            if (user is not null)
            {
                IList<string> roles = await userManager.GetRolesAsync(user);
                u.Role = roles.First();
            }
        }

        return new PageResult<UserShortInfoDTO>
        {
            Count = totalCount,
            PageIndex = model.Page ?? 1,
            PageSize = model.PageSize,
            Items = users
        };
    }
}