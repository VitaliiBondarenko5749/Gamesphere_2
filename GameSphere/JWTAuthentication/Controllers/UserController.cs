using JWTAuthentication.ViewModels;
using JWTAuthentication.Helpers;
using JWTAuthentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JWTAuthentication.Data.Models;
using JWTAuthentication.DTOs;
using Helpers;

namespace JWTAuthentication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> logger;
    private readonly IUserService userService;
    private readonly IEmailService emailService;

    public UserController(ILogger<UserController> logger, IUserService userService, IEmailService emailService)
    {
        this.logger = logger;
        this.userService = userService;
        this.emailService = emailService;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<ServerResponse>> RegisterAsync([FromBody] RegisterViewModel model)
    {
        try
        {
            ServerResponse response = await userService.RegisterAsync(model);

            if (response.IsSuccess)
            {
                string code = await emailService.GenerateEmailConfirmationTokenAsync(model.Email);

                response.Message = $"{UrlEncoder.Encode(response.Message)}|{UrlEncoder.Encode(code)}";

                return response;
            }

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpDelete("Delete/{username}")]
    public async Task<ActionResult<ServerResponse>> DeleteAsync(string username) // DONE!
    {
        try
        {
            return await userService.DeleteAsync(username);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("ConfirmEmail")]
    public async Task<ActionResult<ServerResponse>> ConfirmEmailAsync([FromQuery] ConfirmEmailModel model) // DONE. 
    {
        try
        {
            model.UserId = UrlEncoder.Decode(model.UserId);
            model.Code = UrlEncoder.Decode(model.Code);

            return await emailService.ConfirmEmailAsync(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpPost("ForgotPassword")]
    public async Task<ActionResult<ServerResponse>> ForgotPasswordAsync([FromBody] ForgotPasswordViewModel model) // DONE.
    {
        try
        {
            return await userService.FotgotPasswordAsync(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpPost("ResetPassword")]
    public async Task<ActionResult<ServerResponse>> ResetPasswordAsync([FromForm] ResetPasswordViewModel model) // DONE.
    {
        try
        {
            return await userService.ResetPasswordAsync(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpPost("Login")]
    public async Task<ActionResult<ServerResponse>> LoginAsync([FromForm] LoginViewModel model) // DONE.
    {
        try
        {
            return await userService.LoginAsync(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpPost("Revoke/{userId}")]
    public async Task<IActionResult> RevokeTokenByUserIdAsync(string userId) // DONE. 
    {
        try
        {
            ServerResponse result = await userService.RevokeTokenByUserIdAsync(userId);

            return result.IsSuccess ? Ok(result) : NotFound(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpPost("RefreshToken")]
    public async Task<ActionResult<ServerResponse>> RefreshTokenAsync([FromForm] TokenModel model) // DONE.
    {
        try
        {
            return await userService.RefreshTokenAsync(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpGet("GetImageDirectoryAndUsername/{userId}")]
    public async Task<ActionResult<string>> GetImageDirectoryAndUsernameAsync(string userId) // DONE. 
    {
        try
        {
            return await userService.GetImageDirectoryAndUsernameAsync(userId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpPut("UpdateImageDirectory")]
    public async Task<ActionResult<ServerResponse>> UpdateImageDirectoryAsync([FromBody] ChangeImageDirectoryViewModel model) // DONE, 
    {
        try
        {
            model.ImageDirectory = UrlEncoder.Decode(model.ImageDirectory);

            return await userService.UpdateImageDirectoryAsync(model);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpPut("ChangeUsername")]
    [Authorize]
    public async Task<IActionResult> ChangeUsernameAsync([FromForm] ChangeUsernameViewModel model) // DONE. 
    {
        try
        {
            ServerResponse result = await userService.ChangeUsernameAsync(model);

            return (result.IsSuccess) ? Ok(result) : BadRequest(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpGet("GetChangeEmailToken")]
    public async Task<ActionResult<ServerResponse>> GetChangeEmailTokenAsync([FromQuery] ChangeEmailViewModel model) // DONE. 
    {
        try
        {
            return await userService.GenerateChangeEmailTokenAsync(model);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpGet("ChangeEmail")]
    public async Task<ActionResult<ServerResponse>> ChangeEmailAsync([FromQuery] ChangeEmailViewModel model) // DONE.
    {
        try
        {
            return await userService.ChangeEmailAsync(model);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpGet("CheckUsernameExistence/{username}")]
    public async Task<ActionResult<ServerResponse>> CheckUsernameExistenceAsync(string username) // DONE. 
    {
        try
        {
            return await userService.CheckUsernameExistenceAsync(username);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("CheckEmailExistence/{email}")]
    public async Task<ActionResult<ServerResponse>> CheckEmailExistenceAsync(string email) // DONE.
    {
        try
        {
            return await userService.CheckEmailExistenceAsync(email);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("GetInfo/{id}")]
    public async Task<ActionResult<UserInfoDTO>> GetInfoByIdAsync(string id)
    {
        try
        {
            return await userService.GetUserInfoAsync(id);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }
}