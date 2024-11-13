using Helpers;
using JWTAuthentication.Services;
using JWTAuthentication.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JWTAuthentication.DTOs;

namespace JWTAuthentication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrivilegedUserController : ControllerBase
{
    private readonly ILogger<PrivilegedUserController> logger;
    private readonly IPrivilegedUserService privilegedUserService;

    public PrivilegedUserController(ILogger<PrivilegedUserController> logger, IPrivilegedUserService privilegedUserService)
    {
        this.logger = logger;
        this.privilegedUserService = privilegedUserService;
    }

    [HttpPost("BlockUser/{userId}")]
    public async Task<ActionResult<ServerResponse>> BlockUserAsync(string userId) // DONE.
    {
        try
        {
            return await privilegedUserService.BlockAsync(userId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpPost("AddToRole/{userId}")]
    public async Task<ActionResult<ServerResponse>> AddRoleAsync(string userId) 
    {
        try
        {
            return await privilegedUserService.AddToRoleAsync(userId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpPost("RemoveFromRole/{userId}")]
    public async Task<ActionResult<ServerResponse>> RemoveFromRoleAsync(string userId)
    {
        try
        {
           return await privilegedUserService.RemoveFromRoleAsync(userId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpGet("GetUsers")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<PageResult<UserShortInfoDTO>>> GetUsersAsync([FromQuery] GetUsersPaginationViewModel model)
    {
        try
        {
            return await privilegedUserService.GetUsersAsync(model);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }
}