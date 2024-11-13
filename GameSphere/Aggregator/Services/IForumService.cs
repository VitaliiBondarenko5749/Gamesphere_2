using Helpers.GeneralClasses.Forum.DTOs;
using Helpers;


namespace Aggregator.Services;

public interface IForumService
{
    Task<PageResult<ShortPostInfoDTO>> GetAllPostsAsync(GetTopicsPaginationDTO dto);
    Task<Models.Forum.PostInfoDTO> GetPostByIdAsync(Guid id);
    Task<PageResult<ReplyInfoDTO>> GetAllRepliesAsync(GetRepliesPaginationDTO dto);
    Task<List<ShortPostInfoDTO>> GetSimilarPostsAsync(GetSimilarPostsDTO dto);
    Task DeleteUserDataAsync(string userId);
}

public class ForumService : IForumService 
{
    private readonly HttpClient httpClient;

    public ForumService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<PageResult<ShortPostInfoDTO>> GetAllPostsAsync(GetTopicsPaginationDTO dto)
    {
        string query = $"/api/Post?Page={dto.Page ?? 1}&PageSize={dto.PageSize}&TopicViewer={dto.TopicViewer}&TopicSorter={dto.TopicSorter}";

        if (!string.IsNullOrEmpty(dto.SearchText))
        {
            query += $"&SearchText={dto.SearchText}";
        }

        if (!string.IsNullOrEmpty(dto.UserId))
        {
            query += $"&UserId={dto.UserId}";
        }

        if (dto.GameIds is not null)
        {
                query += $"&GameIds={dto.GameIds}";
        }

        HttpResponseMessage response = await httpClient.GetAsync(query);

        return await response.Content.ReadFromJsonAsync<PageResult<ShortPostInfoDTO>>()
            ?? throw new NullReferenceException("Object is nullable!");  
    }

    public async Task<Models.Forum.PostInfoDTO> GetPostByIdAsync(Guid id)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"/api/Post/{id}");

        return await response.Content.ReadFromJsonAsync<Aggregator.Models.Forum.PostInfoDTO>()
            ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<PageResult<ReplyInfoDTO>> GetAllRepliesAsync(GetRepliesPaginationDTO dto)
    {
        string query = $"/api/Reply?Page={dto.Page}&PageSize={dto.PageSize}&PostId={dto.PostId}";

        HttpResponseMessage response = await httpClient.GetAsync(query);

        return await response.Content.ReadFromJsonAsync<PageResult<ReplyInfoDTO>>()
            ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task<List<ShortPostInfoDTO>> GetSimilarPostsAsync(GetSimilarPostsDTO dto)
    {
        string query = $"/api/Post/GetSimilarPosts?PostId={dto.PostId}&CurrentPostText={dto.CurrentPostText}";

        if(dto.GameId is not null)
        {
            query += $"&GameId={dto.GameId}";
        }

        query += $"&Count={dto.Count}";

        HttpResponseMessage response = await httpClient.GetAsync(query);

        return await response.Content.ReadFromJsonAsync<List<ShortPostInfoDTO>>()
            ?? throw new NullReferenceException("Object is nullable!");
    }

    public async Task DeleteUserDataAsync(string userId)
    {
        await httpClient.DeleteAsync($"/api/Post/DeleteUserForumData/{userId}");
    }
}