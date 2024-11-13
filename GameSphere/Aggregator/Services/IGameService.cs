using Helpers;
using Aggregator.Models.CatalogOfGames;
using Helpers.GeneralClasses.Forum.DTOs;
using Aggregator.Models.Forum;

namespace Aggregator.Services;

public interface IGameService
{
    Task<ServerResponse?> UpdateGameIconAsync(string gameName, string directory);
    Task<ServerResponse?> AddImageToGameAsync(string gameName, string directory);
    Task RemoveImageFromGameAsync(Guid id);
    Task<List<string>> GetImageDirectoriesAsync(Guid gameId);
    Task<ServerResponse> DeleteGameAsync(Guid gameId);
    Task<PageResult<CommentInfoDTO>> GetCommentsAsync(GetCommentsPaginationDTO dto);
    Task<ServerResponse> DeleteUserDataAsync(string userId);
    Task<PostGameInfoDTO> GetGameInfoByIdAsync(Guid gameId);
}

public class GameService : IGameService
{
    private readonly HttpClient httpClient;

    public GameService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ServerResponse?> UpdateGameIconAsync(string gameName, string directory)
    {
        var data = new
        {
            GameName = gameName,
            Directory = directory
        };

        HttpResponseMessage response = await httpClient.PutAsJsonAsync("/api/Game/ChangeGameIcon", data);

        return await response.Content.ReadFromJsonAsync<ServerResponse?>();
    }

    public async Task<ServerResponse?> AddImageToGameAsync(string gameName, string directory)
    {
        var data = new
        {
            GameName = gameName,
            Directory = directory
        };

        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/Game/AddImageToGame", data);

        return await response.Content.ReadFromJsonAsync<ServerResponse?>();
    }

    public async Task RemoveImageFromGameAsync(Guid id)
    {
        await httpClient.DeleteAsync($"/api/Game/DeleteGameImage/{id}");
    }

    public async Task<List<string>> GetImageDirectoriesAsync(Guid gameId)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"/api/Game/Directories/{gameId}");

        return await response.Content.ReadFromJsonAsync<List<string>>() 
            ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<ServerResponse> DeleteGameAsync(Guid gameId)
    {
        HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Game/{gameId}");

        return await response.Content.ReadFromJsonAsync<ServerResponse>()
            ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<PageResult<CommentInfoDTO>> GetCommentsAsync(GetCommentsPaginationDTO dto)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"/api/Comment?GameId={dto.GameId}&Page={dto.Page ?? 1}&PageSize={dto.PageSize}");

        return await response.Content.ReadFromJsonAsync<PageResult<CommentInfoDTO>>()
            ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<ServerResponse> DeleteUserDataAsync(string userId)
    {
        HttpResponseMessage response = await httpClient.DeleteAsync($"/api/Game/DeleteUserData/{userId}");

        return await response.Content.ReadFromJsonAsync<ServerResponse>()
            ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<PostGameInfoDTO> GetGameInfoByIdAsync(Guid gameId)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"/api/Game/GetShortGameInfo/{gameId}");

        return await response.Content.ReadFromJsonAsync<PostGameInfoDTO>()
            ?? throw new NullReferenceException("Object is nullable!");
    }
}