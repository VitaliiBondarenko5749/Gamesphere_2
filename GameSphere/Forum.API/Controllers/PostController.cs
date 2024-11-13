using Forum.BAL.DTOs;
using Forum.BAL.Services;
using Forum.DAL.Entities;
using Helpers;
using Helpers.GeneralClasses.Forum.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostController : ControllerBase
{
    private readonly ILogger<PostController> logger;
    private readonly IPostService postService;

    public PostController(ILogger<PostController> logger, IPostService postService)
    {
        this.logger = logger;
        this.postService = postService;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<ShortPostInfoDTO>>> GetAllAsync([FromQuery] GetTopicsPaginationDTO dto)
    {
        try
        {
            return await postService.GetAllAsync(dto);
        }
        catch (Exception ex)    
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR...");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> AddPostAsync([FromForm] AddPostDTO dto)
    {
        try
        {
            return await postService.AddAsync(dto);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR...");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> GetByIdAsync(Guid id)
    {
        try
        {
            return await postService.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> DeletePostAsync(Guid id)
    {
        try
        {
            return await postService.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpPost("AddPostToFavorite")]
    [Authorize] 
    public async Task<ActionResult<ServerResponse>> AddToFavoriteAsync([FromForm] SavePostDTO dto)
    {
        try
        {
            return await postService.AddToFavoriteAsync(dto);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpDelete("DeletePostFromFavorite")]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> DeleteFromFavoriteAsync([FromQuery] SavePostDTO dto)
    {
        try
        {
            return await postService.DeleteFromFavoriteAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("CheckExistenceInFavorite")]
    public async Task<ActionResult<ServerResponse>> CheckExistenceInFavoriteAsync([FromQuery] SavePostDTO dto)
    {
        try
        {
            return await postService.CheckExistenceInFavAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("GetSimilarPosts")]
    public async Task<ActionResult<List<ShortPostInfoDTO>>> GetSimilarPostsAsync([FromQuery] GetSimilarPostsDTO dto)
    {
        try
        {
            return await postService.GetSimilarPostsAsync(dto);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpDelete("DeleteUserForumData/{userId}")]
    public async Task<IActionResult> DeleteUserForumDataAsync(string userId)
    {
        try
        {
            await postService.DeleteUserForumDataAsync(userId);
            return StatusCode(200, "User data have been removed successfully");
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }
}