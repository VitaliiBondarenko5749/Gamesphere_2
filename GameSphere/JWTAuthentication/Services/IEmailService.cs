using Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using JWTAuthentication.Data;
using JWTAuthentication.Data.Models;

namespace JWTAuthentication.Services;

public interface IEmailService
{
    Task<ServerResponse> ConfirmEmailAsync(ConfirmEmailModel model);
    Task<string> GenerateEmailConfirmationTokenAsync(string email);
}

public class EmailService : IEmailService
{
    private readonly UserManager<User> userManager;
    private readonly DbContext dbContext;

    public EmailService(UserManager<User> userManager, ApplicationDbContext dbContext)
    {
        this.userManager = userManager;
        this.dbContext = dbContext;
    }

    public async Task<ServerResponse> ConfirmEmailAsync(ConfirmEmailModel model) // DONE.
    {
        if (model.UserId.IsNullOrEmpty() || model.Code.IsNullOrEmpty())
        {
            return new ServerResponse
            {
                Message = "Error email confirmation: parameters are nullable!",
                IsSuccess = false
            };
        }

        User? user = await userManager.FindByIdAsync(model.UserId);

        if(user is null || user.EmailConfirmed)
        {
            return new ServerResponse
            {
                Message = "Error email confirmation: " + ((user is null) ? "User is not found!" : "Email is already confirmed!"),
                IsSuccess = false
            };
        }

        IdentityResult result = await userManager.ConfirmEmailAsync(user, model.Code);

        if (result.Succeeded)
        {
            await dbContext.SaveChangesAsync();
        }

        return new ServerResponse
        {
            Message = (result.Succeeded) ? "Email is confirmed!" : $"Error during Email confirmation: {result.Errors}",
            IsSuccess = result.Succeeded
        };
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(string email) // DONE.
    {
        User user = await userManager.FindByEmailAsync(email);
       
        return await userManager.GenerateEmailConfirmationTokenAsync(user);
    }
}