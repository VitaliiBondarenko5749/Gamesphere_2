using FluentValidation.Results;
using JWTAuthentication.Data;
using JWTAuthentication.Data.Models;
using JWTAuthentication.DTOs;
using JWTAuthentication.Helpers;
using JWTAuthentication.Validators;
using JWTAuthentication.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Helpers;

#pragma warning disable

namespace JWTAuthentication.Services;

public interface IUserService
{
    Task<ServerResponse> CheckUsernameExistenceAsync(string username);
    Task<ServerResponse> CheckEmailExistenceAsync(string email);
    Task<ServerResponse> RegisterAsync(RegisterViewModel model);
    Task<ServerResponse> DeleteAsync(string username);
    Task<ServerResponse> LoginAsync(LoginViewModel model);
    Task<ServerResponse> FotgotPasswordAsync(ForgotPasswordViewModel model);
    Task<ServerResponse> ResetPasswordAsync(ResetPasswordViewModel model);
    Task<ServerResponse> RevokeTokenByUserIdAsync(string userId);
    Task<ServerResponse> RefreshTokenAsync(TokenModel tokenModel);
    Task<string> GetImageDirectoryAndUsernameAsync(string userId);
    Task<ServerResponse> UpdateImageDirectoryAsync(ChangeImageDirectoryViewModel model);
    Task<ServerResponse> ChangeUsernameAsync(ChangeUsernameViewModel model);
    Task<ServerResponse> GenerateChangeEmailTokenAsync(ChangeEmailViewModel model);
    Task<ServerResponse> ChangeEmailAsync(ChangeEmailViewModel model);
    Task<UserInfoDTO> GetUserInfoAsync(string id);
}

public class UserService : IUserService
{
    private readonly UserManager<User> userManager;
    private readonly SignInManager<User> signInManager;
    private readonly ApplicationDbContext dbContext;
    private readonly RoleManager<IdentityRole> roleManager;
    private readonly IConfiguration configuration;

    public UserService(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext dbContext,
        RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        this.userManager = userManager;
        this.signInManager = signInManager;
        this.dbContext = dbContext;
        this.roleManager = roleManager;
        this.configuration = configuration;
    }

    public async Task<ServerResponse> RegisterAsync(RegisterViewModel model) // DONE.
    {
        // Checking data from the form.

        RegisterViewModelValidator validator = new();
        ValidationResult validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            string errors = validationResult.ToString("~");

            return new ServerResponse
            {
                Message = "Something went wrong! All errors is in Errors list!",
                IsSuccess = false,
                Errors = errors.Split('~')
            };
        }

        // Checking existance of user with username from the from.

        User user = await userManager.FindByNameAsync(model.Username);

        if (user is not null)
        {
            return new ServerResponse
            {
                Message = $"User with username \"{model.Username}\" already exists!",
                IsSuccess = false
            };
        }

        user = await userManager.FindByEmailAsync(model.Email);

        if (user is not null)
        {
            return new ServerResponse
            {
                Message = $"User with email {model.Email} already exists!",
                IsSuccess = false
            };
        }

        user = new User()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = model.Username,
            Email = model.Email,
            ImageDirectory = model.ImageDirectory
        };

        // Add user to the database. 

        IdentityResult result = await userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            string errors = string.Join("~", result.Errors.Select(error => error.Description));

            return new ServerResponse
            {
                Message = "Something went wrong during adding user to the database.",
                IsSuccess = false,
                Errors = errors.Split('~')
            };
        }

        await userManager.AddToRoleAsync(user, UserRoles.User);
        await dbContext.SaveChangesAsync();

        return new ServerResponse
        {
            Message = user.Id,
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> DeleteAsync(string username) // DONE. 
    {
        User user = await userManager.FindByNameAsync(username);

        if (user is not null)
        {
            await userManager.DeleteAsync(user);

            await dbContext.SaveChangesAsync();

            return new ServerResponse { Message = "User has been removed!", IsSuccess = true };
        }

        return new ServerResponse { Message = "User is not found!", IsSuccess = false };
    }

    public async Task<ServerResponse> LoginAsync(LoginViewModel model) // DONE. 
    {
        // Checking the data from the form.
        LoginViewModelValidator validator = new();
        ValidationResult validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            string errors = validationResult.ToString("~");

            return new ServerResponse
            {
                Message = "Something went wrong! All errors is in \"Errors\" list!",
                IsSuccess = false,
                Errors = errors.Split(',')
            };
        }

        // Try to login.
        User? user = null;

        if (model.UsernameOrEmailInput.Contains("@"))
        {
            user = await userManager.FindByEmailAsync(model.UsernameOrEmailInput);
        }
        else
        {
            user = await userManager.FindByNameAsync(model.UsernameOrEmailInput);
        }

        if (user is not null && !user.IsLocked && await userManager.CheckPasswordAsync(user, model.Password))
        {
            if (!user.EmailConfirmed)
            {
                return new ServerResponse
                {
                    Message = $"Your email: {user.Email} is not confirmed. Please, confirm it!",
                    IsSuccess = false
                };
            }

            IList<string> userRoles = await userManager.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            if (!user.ImageDirectory.IsNullOrEmpty())
            {
                claims.Add(new Claim("PhotoDirectory", user.ImageDirectory));
            }

            foreach (string userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

            JwtSecurityToken token = new(issuer: configuration["JWT:Issuer"],
            audience: configuration["JWT:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            string tokenAsString = new JwtSecurityTokenHandler().WriteToken(token);
            string refreshTokenAsString = GenerateRefreshToken();

            user.RefreshToken = refreshTokenAsString;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

            await userManager.UpdateAsync(user);

            await dbContext.SaveChangesAsync();

            return new ServerResponse()
            {
                Message = $"Welcome {user.UserName}!",
                AccessToken = tokenAsString,
                RefreshToken = refreshTokenAsString,
                IsSuccess = true
            };
        }

        return new ServerResponse
        {
            Message = (user is null) ? $"User {model.UsernameOrEmailInput} does not exist!" : (user.IsLocked)
            ? $"User {model.UsernameOrEmailInput} is banned. Date: {user.LockDate}!" : "Incorrect password!",
            IsSuccess = false
        };
    }

    public async Task<ServerResponse> CheckUsernameExistenceAsync(string username) // DONE. 
    {
        User? user = await userManager.FindByNameAsync(username);

        return new ServerResponse
        {
            Message = (user is not null) ? $"Name {username} is already used by somebody" : string.Empty,
            IsSuccess = (user is not null) ? false : true
        };
    }

    public async Task<ServerResponse> CheckEmailExistenceAsync(string email) // DONE.
    {
        if (Regex.IsMatch(email, @"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$"))
        {
            User? user = await userManager.FindByEmailAsync(email);

            return new ServerResponse
            {
                Message = (user is not null) ? $"Email {email} is already used by somebody" : string.Empty,
                IsSuccess = (user is not null) ? false : true
            };
        }

        return new ServerResponse { Message = "Email is not valid!", IsSuccess = false };
    }

    public async Task<ServerResponse> FotgotPasswordAsync(ForgotPasswordViewModel model) // DONE. 
    {
        ForgotPasswordViewModelValidator validator = new();
        ValidationResult validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            string errors = validationResult.ToString("~");

            return new ServerResponse
            {
                Message = "Something went wrong! All errors is in \"Errors\" list!",
                IsSuccess = false,
                Errors = errors.Split('~')
            };
        }

        User? user = await userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            return new ServerResponse { Message = "User is not found!", IsSuccess = false };
        }

        string code = await userManager.GeneratePasswordResetTokenAsync(user);
        string encodedToken = UrlEncoder.Encode(code);

        return new ServerResponse { Message = encodedToken, IsSuccess = true };
    }

    public async Task<ServerResponse> ResetPasswordAsync(ResetPasswordViewModel model) // DONE. 
    {
        ResetPasswordViewModelValidator validator = new();
        ValidationResult validationResult = await validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            string errors = validationResult.ToString("~");

            return new ServerResponse
            {
                Message = "Something went wrong! All errors is in Errors list!",
                IsSuccess = false,
                Errors = errors.Split('~')
            };
        }

        model.Code = UrlEncoder.Decode(model.Code);

        User? user = await userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            return new ServerResponse { Message = "User is not found!", IsSuccess = false };
        }

        IdentityResult result = await userManager.ResetPasswordAsync(user, model.Code, model.Password);

        return new ServerResponse
        {
            Message = (result.Succeeded) ? "Password has been changed!" : "Error during password changing!",
            IsSuccess = (result.Succeeded) ? true : false
        };
    }

    public async Task<ServerResponse> RevokeTokenByUserIdAsync(string userId)
    {
        User? user = await userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return new ServerResponse
            {
                Message = "Revoke token error: User not exist!",
                IsSuccess = false
            };
        }

        user.RefreshToken = null;
        await userManager.UpdateAsync(user);

        await dbContext.SaveChangesAsync();

        return new ServerResponse
        {
            Message = "Refresh token revoked successfully!",
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> RefreshTokenAsync(TokenModel model) // DONE. 
    {
        if (model is null)
        {
            return new ServerResponse
            {
                Message = "Wrong token model!",
                IsSuccess = false
            };
        }

        string accessToken = model.AccessToken;
        string refreshToken = model.RefreshToken;

        ClaimsPrincipal? principal = GetPrincipalFromExpiredToken(accessToken);

        List<Claim> claims = principal.Claims.ToList();

        if (principal is null)
        {
            return new ServerResponse
            {
                Message = "Invalid access token or refresh token!",
                IsSuccess = false
            };
        }

        string? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        User? user = await userManager.FindByIdAsync(userId);

        if (user is null || refreshToken != user.RefreshToken || user.IsLocked)
        {
            return new ServerResponse
            {
                Message = (user is null) ? "User not found in the database!" : "Invalid access token or refresh token!",
                IsSuccess = false
            };
        }

        if (!user.ImageDirectory.IsNullOrEmpty())
        {
            Claim? photoDirectory = claims.FirstOrDefault(c => c.Type.Equals("PhotoDirectory"));

            if (photoDirectory is not null && !photoDirectory.Value.Equals(user.ImageDirectory))
            {
                claims.Remove(photoDirectory);
                photoDirectory = new Claim(photoDirectory.Type, user.ImageDirectory);
                claims.Add(photoDirectory);
            }
        }

        Claim? usernameClaim = claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name));

        if (usernameClaim is not null && !usernameClaim.Value.Equals(user.UserName))
        {
            claims.Remove(usernameClaim);
            usernameClaim = new Claim(usernameClaim.Type, user.UserName);
            claims.Add(usernameClaim);
        }

        Claim? emailClaim = claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email));

        if (emailClaim is not null && !emailClaim.Value.Equals(user.Email))
        {
            claims.Remove(emailClaim);
            emailClaim = new Claim(emailClaim.Type, user.Email);
            claims.Add(emailClaim);
        }

        IList<string> roles = await userManager.GetRolesAsync(user);

        string role = roles.First();

        Claim? roleClaim = claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Role));

        if (roleClaim is not null && !roleClaim.Value.Equals(role))
        {
            claims.Remove(roleClaim);
            roleClaim = new Claim(roleClaim.Type, role);
            claims.Add(roleClaim);
        }

        JwtSecurityToken newAccessToken = CreateToken(claims, 1);
        string newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;

        await userManager.UpdateAsync(user);

        await dbContext.SaveChangesAsync();

        return new ServerResponse
        {
            Message = "Token has been refreshed!",
            AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken,
            IsSuccess = true,
            ExpiredDate = newAccessToken.ValidTo
        };
    }

    public async Task<string> GetImageDirectoryAndUsernameAsync(string userId) // DONE. 
    {
        return await userManager.Users.Where(u => u.Id.Equals(userId))
            .Select(u => $"{u.ImageDirectory}|{u.UserName}")
            .FirstAsync();
    }

    public async Task<ServerResponse> UpdateImageDirectoryAsync(ChangeImageDirectoryViewModel model) // DONE. 
    {
        User? user = await userManager.FindByNameAsync(model.Username);

        if (user is null)
        {
            return new ServerResponse { Message = "User is not found!", IsSuccess = false };
        }

        user.ImageDirectory = model.ImageDirectory;

        await userManager.UpdateAsync(user);

        await dbContext.SaveChangesAsync();

        return new ServerResponse { Message = "Image directory has been updated successfully!", IsSuccess = true };
    }

    public async Task<ServerResponse> ChangeUsernameAsync(ChangeUsernameViewModel model) // DONE. 
    {
        User? user = await userManager.FindByIdAsync(model.Id);

        if (user is null)
        {
            return new ServerResponse
            {
                Message = "User doesn't exist!",
                IsSuccess = false
            };
        }

        if (Regex.IsMatch(model.NewUsername, @"^[a-z0-9_.]+$"))
        {
            user.UserName = model.NewUsername;

            await userManager.UpdateAsync(user);

            await dbContext.SaveChangesAsync();

            return new ServerResponse
            {
                Message = $"User with id: {user.Id} has changed his username. New username: {model.NewUsername}",
                IsSuccess = true
            };
        }

        return new ServerResponse
        {
            Message = "New username is not valid!",
            IsSuccess = false
        };
    }

    public async Task<ServerResponse> GenerateChangeEmailTokenAsync(ChangeEmailViewModel model) // DONE. 
    {
        User? user = await userManager.FindByIdAsync(model.UserId);

        if (user is null)
        {
            return new ServerResponse { Message = "User is not found!", IsSuccess = false };
        }

        if (Regex.IsMatch(model.NewEmail, @"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$")
            && await userManager.FindByEmailAsync(model.NewEmail) is null)
        {
            string code = await userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
            code = UrlEncoder.Encode(code);

            return new ServerResponse { Message = code, IsSuccess = true };
        }

        return new ServerResponse { Message = "Email is not valid or already taken!", IsSuccess = false };
    }

    public async Task<ServerResponse> ChangeEmailAsync(ChangeEmailViewModel model) // Done. 
    {
        User? user = await userManager.FindByIdAsync(model.UserId);

        if (user is null)
        {
            return new ServerResponse { Message = "User is not found!", IsSuccess = false };
        }

        if (Regex.IsMatch(model.NewEmail, @"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$") && model.Code is not null)
        {
            model.Code = UrlEncoder.Decode(model.Code);

            IdentityResult identityResult = await userManager.ChangeEmailAsync(user, model.NewEmail, model.Code);

            if (identityResult.Succeeded)
            {
                return new ServerResponse { Message = "Email has been changed successfully!", IsSuccess = true };
            }
        }

        return new ServerResponse { Message = "Email is not valid or already taken!", IsSuccess = false };
    }

    public async Task<UserInfoDTO> GetUserInfoAsync(string id)
    {
        User user = await userManager.FindByIdAsync(id)
            ?? throw new NullReferenceException("User is not found!");

        IList<string> roles = await userManager.GetRolesAsync(user);

        return new UserInfoDTO
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Role = roles.First(),
            IconDirectory = user.ImageDirectory
        };
    }

    private JwtSecurityToken CreateToken(List<Claim> claims, int tokenValidityInHours) // DONE. 
    {
        SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: configuration["JWT:ValidIssuer"],
            audience: configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(tokenValidityInHours),
            claims: claims,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

        return token;
    }

    private static string GenerateRefreshToken() // DONE. 
    {
        byte[] randomNumber = new byte[64];
        using RandomNumberGenerator numberGenerator = RandomNumberGenerator.Create();

        numberGenerator.GetBytes(randomNumber);

        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token) // DONE. 
    {
        TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = configuration["JWT:Audience"],
            ValidIssuer = configuration["JWT:Issuer"],
            RequireExpirationTime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false
        };

        JwtSecurityTokenHandler tokenHandler = new();
        ClaimsPrincipal principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token!");
        }

        return principal;
    }
}