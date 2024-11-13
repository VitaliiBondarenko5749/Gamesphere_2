using Aggregator.Models.Authentication;
using Aggregator.Models.CloudStorage;
using Aggregator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Helpers;

#pragma warning disable

namespace Aggregator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ILogger<AuthenticationController> logger;
    private readonly IAuthenticationService authenticationService;
    private readonly ICloudStorageService cloudStorageService;
    private readonly IEmailService emailService;
    private readonly IFileService fileService;
    private readonly IGameService gameService;
    private readonly IForumService forumService;

    public AuthenticationController(ILogger<AuthenticationController> logger, IAuthenticationService authenticationService,
        ICloudStorageService cloudStorageService, IEmailService emailService, IFileService fileService, IGameService gameService,
        IForumService forumService)
    {
        this.logger = logger;
        this.authenticationService = authenticationService;
        this.cloudStorageService = cloudStorageService;
        this.emailService = emailService;
        this.fileService = fileService;
        this.gameService = gameService;
        this.forumService = forumService;
    }

    [HttpPost("Register")]
    public async Task<ActionResult<ServerResponse>> RegisterAsync([FromForm] RegisterViewModel model) // DONE. 
    {
        ServerResponse?[] responses = new ServerResponse[2];

        string imageDirectory = "/UserIcons/";

        if (model.Avatar is not null)
        {
            model.Avatar = await fileService.ChangeNameAsync(model.Avatar, $"{model.Username}-{model.Avatar.FileName}");
            imageDirectory += model.Avatar.FileName;
        }
        else
        {
            imageDirectory += "No-icon-image.png";
        }

        try
        {
            responses[0] = await authenticationService.RegisterAsync(model.Username, model.Email, model.Password, imageDirectory);

            if (responses[0].IsSuccess && responses[0].Message is not null)
            {
                string[] responseParams = responses[0].Message.Split('|');

                if (model.Avatar is not null)
                {
                    UploadFileViewModel uploadFileViewModel = new UploadFileViewModel { File = model.Avatar, Folder = "UserIcons" };

                    responses[1] = await cloudStorageService.UploadFileAsync(uploadFileViewModel) ??
                        throw new Exception("Error during uploading file!");
                }

                string callbackUrl = $"http://localhost:4200/confirm-email?userId={responseParams[0]}&code={responseParams[1]}";

                await emailService.SendAsync(sendTo: model.Email, subject: "Confirm your account", isHtml: true,
                        message: $"<p><strong>Confirm your registration by following link:</strong></p> <a href='{callbackUrl}'>link</a>");

                return new ServerResponse
                {
                    Message = "Check your email and follow the link provided in the letter to confirm the registration ;)",
                    IsSuccess = true
                };
            }

            return new ServerResponse { Message = "Registration error!", IsSuccess = false };
        }
        catch (Exception ex)
        {
            if (responses[0] is not null && responses[0].IsSuccess)
            {
                await authenticationService.DeleteAsync(model.Username);
            }

            if (responses[1] is not null && responses[1].IsSuccess && !imageDirectory.Equals("/UserIcons/No-icon-image.png"))
            {
                await cloudStorageService.DeleteFileAsync(imageDirectory);
            }

            Console.WriteLine(ex.Message);

            return new ServerResponse { Message = "INTERNAL SERVER ERROR", IsSuccess = false };
        }
    }

    [HttpPost("ForgotPassword")]
    public async Task<ActionResult<ServerResponse>> ForgotPasswordAsync([FromForm] ForgotPasswordViewModel model) // DONE. 
    {
        try
        {
            ServerResponse response = await authenticationService.ForgotPasswordAsync(model.Email);

            if (!response.IsSuccess)
            {
                return new ServerResponse { Message = response.Message, IsSuccess = false };
            }

            string callbackUrl = $"http://localhost:4200/reset-password?code={response.Message}";

            await emailService.SendAsync(sendTo: model.Email, subject: "Reset your password", isHtml: true,
                        message: $"<p><strong>Tap the link below to reset your password:</strong></p> <a href='{callbackUrl}'>link</a>");

            return new ServerResponse
            {
                Message = "Check your email and follow the link provided in the letter to reset your password ;)",
                IsSuccess = true
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            return new ServerResponse { Message = "INTERNAL SERVER ERROR", IsSuccess = false };
        }
    }

    [HttpPut("ChangeIcon")]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> ChangeIconAsync([FromForm] ChangeIconViewModel model) // DONE. 
    {
        try
        {
            // Get username and image directory.
            string imageDirectoryAndUsername = await authenticationService.GetImageDirectoryAndUsernameAsync(model.Id);
            string[] arrayData = imageDirectoryAndUsername.Split('|');
            string imageDirectory = arrayData[0];
            string username = arrayData[1];

            // Delete an old icon from the cloud storage.
            if (!imageDirectory.Equals("/UserIcons/No-icon-image.png"))
            {
                await cloudStorageService.DeleteFileAsync(UrlEncoder.Encode(imageDirectory));
            }
           
            model.NewAvatar = await fileService.ChangeNameAsync(model.NewAvatar, $"{username}-{model.NewAvatar.FileName}");
            imageDirectory = "/UserIcons/" + model.NewAvatar.FileName;
            imageDirectory = UrlEncoder.Encode(imageDirectory);
            UploadFileViewModel uploadFileViewModel = new() { File = model.NewAvatar, Folder = "UserIcons" };

            await cloudStorageService.UploadFileAsync(uploadFileViewModel);

            return await authenticationService.UpdateImageDirectoryAsync(username, imageDirectory);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);

            return new ServerResponse { Message = "INTERNAL SERVER ERROR", IsSuccess = false };
        }
    }

    [HttpPost("ChangeEmail")]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> ChangeEmailAsync([FromForm] ChangeEmailViewModel model) // DONE. 
    {
        try
        {
            ServerResponse response = await authenticationService.GetChangeEmailTokenAsync(model);

            if (!response.IsSuccess)
            {
                return response;
            }

            string code = response.Message;
            string callbackUrl = $"http://localhost:4200/change-email?userId={model.UserId}&email={model.NewEmail}&code={code}";

            await emailService.SendAsync(sendTo: model.NewEmail, subject: "Change email", isHtml: true,
                        message: $"<p><strong>Tap the link below to change your email:</strong></p> <a href='{callbackUrl}'>link</a>");

            response.Message = "Check your email and follow the link provided in the letter to change your email ;)";

            return response;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ServerResponse { Message = "INTERNAL SERVER ERROR", IsSuccess = false };
        }
    }

    [HttpPost("Ban")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> BanUserAsync([FromForm] BanUserViewModel model) // DONE. 
    {
        try
        {
            await gameService.DeleteUserDataAsync(model.UserId);

            await forumService.DeleteUserDataAsync(model.UserId);

            ServerResponse response = await authenticationService.BlockAsync(model.UserId);

            if (response.IsSuccess)
            {
                string[] userData = response.Message.Split('|');
                string imageDirectory = userData[0], userName = userData[1], email = userData[2];

                string[] privilegedUserData = model.BannedBy.Split('-');
                string privilegedUserName = privilegedUserData[0], privilegedUserRole = privilegedUserData[1];

                string emailMessage = $"<p>Dear {userName},</p><br>";
                emailMessage += $"<p>You have got banned by <strong>{privilegedUserName}({privilegedUserRole})</strong></p><br>";

                if (!model.Description.IsNullOrEmpty())
                {
                    emailMessage += $"<p>{privilegedUserName}'s message: <i>{model.Description}</i></p>";
                }
                
                await emailService.SendAsync(sendTo: email, subject: "Ban account!", message: emailMessage, isHtml: true);

                if (!imageDirectory.Equals("/UserIcons/No-icon-image.png"))
                {
                    await cloudStorageService.DeleteFileAsync(imageDirectory);
                }

                response.Message = $"User {userName} has been banned!";
            }

            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ServerResponse { Message = "INTERNAL SERVER ERROR", IsSuccess = false };
        }
    }

    [HttpPost("UpgradeRole/{userId}")]
    [Authorize(Roles = "Creator")]
    public async Task<ActionResult<ServerResponse>> UpgradeRoleAsync(string userId)
    {
        try
        {
            ServerResponse response = await authenticationService.UpgradeRoleAsync(userId);

            if (response.IsSuccess)
            {
                string[] userData = response.Message.Split('|');
                string emailMessage = userData[0], email = userData[1];

                await emailService.SendAsync(sendTo: email, subject: "Getting a new role", message: emailMessage, isHtml: true);
            }

            return response;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ServerResponse { Message = "INTERNAL SERVER ERROR", IsSuccess = false };
        }
    } // Done.

    [HttpPost("DowngradeRole")]
    [Authorize(Roles = "Creator")]
    public async Task<ActionResult<ServerResponse>> DowngradeRoleAsync([FromForm] DowngradeRoleViewModel model) // Done. 
    {
        try
        {
            ServerResponse response = await authenticationService.DowngradeRoleAsync(model.UserId);

            if (response.IsSuccess)
            {
                string[] userData = response.Message.Split('|');
                string emailMessage = userData[0], email = userData[1];

                if (!model.Description.IsNullOrEmpty())
                {
                    emailMessage += $"<p>Message: <i>{model.Description}</i></p>";
                }

                await emailService.SendAsync(sendTo: email, subject: "Getting a new role", message: emailMessage, isHtml: true);
            }

            return response;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new ServerResponse { Message = "INTERNAL SERVER ERROR", IsSuccess = false };
        }
    }
}