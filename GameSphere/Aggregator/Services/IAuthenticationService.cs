using Helpers;
using Aggregator.Models.Authentication;

namespace Aggregator.Services;

public interface IAuthenticationService
{
    Task<ServerResponse> RegisterAsync(string username, string email, string password, string imageDirectory);
    Task DeleteAsync(string username);
    Task<ServerResponse> ForgotPasswordAsync(string email);
    Task<string> GetImageDirectoryAndUsernameAsync(string userId);
    Task<ServerResponse> UpdateImageDirectoryAsync(string userId, string imageDirectory);
    Task<ServerResponse> GetChangeEmailTokenAsync(ChangeEmailViewModel model);
    Task<ServerResponse> BlockAsync(string userId);
    Task<ServerResponse> UpgradeRoleAsync(string userId);
    Task<ServerResponse> DowngradeRoleAsync(string userId);
    Task<UserInfoDTO> GetUserInfoAsync(string userId);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ServerResponse> RegisterAsync(string username, string email, string password, string imageDirectory) // Done. 
    {
        var requestData = new
        {
            Username = username,
            Email = email,
            Password = password,
            ImageDirectory = imageDirectory
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/User/Register", requestData);

        return await response.Content.ReadFromJsonAsync<ServerResponse>()
            ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task DeleteAsync(string username) // Done. 
    {
        string request = $"/api/User/Delete/{username}";

        await httpClient.DeleteAsync(request);
    }

    public async Task<ServerResponse> ForgotPasswordAsync(string email)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/User/ForgotPassword", new { Email = email });

        return await response.Content.ReadFromJsonAsync<ServerResponse>()
           ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<string> GetImageDirectoryAndUsernameAsync(string userId)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"/api/User/GetImageDirectoryAndUsername/{userId}");

        return await response.Content.ReadAsStringAsync()
            ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<ServerResponse> UpdateImageDirectoryAsync(string username, string imageDirectory)
    {
        var data = new
        {
            Username = username,
            ImageDirectory = imageDirectory
        };

        HttpResponseMessage response = await httpClient.PutAsJsonAsync($"api/User/UpdateImageDirectory", data);

        return await response.Content.ReadFromJsonAsync<ServerResponse>()
            ?? throw new NullReferenceException("Object is nullable");
    }

    public async Task<ServerResponse> GetChangeEmailTokenAsync(ChangeEmailViewModel model)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"/api/User/GetChangeEmailToken?UserId={model.UserId}&NewEmail={model.NewEmail}");

        return await response.Content.ReadFromJsonAsync<ServerResponse>()
           ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<ServerResponse> BlockAsync(string userId)
    {
        HttpResponseMessage response = await httpClient.PostAsync($"/api/PrivilegedUser/BlockUser/{userId}", null);

        return await response.Content.ReadFromJsonAsync<ServerResponse>()
            ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<ServerResponse> UpgradeRoleAsync(string userId)
    {
        HttpResponseMessage response = await httpClient.PostAsync($"/api/PrivilegedUser/AddToRole/{userId}", null);

        return await response.Content.ReadFromJsonAsync<ServerResponse>()
           ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<ServerResponse> DowngradeRoleAsync(string userId)
    {
        HttpResponseMessage response = await httpClient.PostAsync($"/api/PrivilegedUser/RemoveFromRole/{userId}", null);

        return await response.Content.ReadFromJsonAsync<ServerResponse>()
           ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<UserInfoDTO> GetUserInfoAsync(string userId)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"/api/User/GetInfo/{userId}");

        return await response.Content.ReadFromJsonAsync<UserInfoDTO>()
            ?? throw new NullReferenceException("Object is nullable!");
    }
}