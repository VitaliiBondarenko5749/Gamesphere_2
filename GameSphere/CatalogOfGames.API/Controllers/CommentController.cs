using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.BAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogOfGames.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentController : ControllerBase
{
    private readonly ILogger<CommentController> logger;
    private readonly IGameCommentService gameCommentService;

    public CommentController(ILogger<CommentController> logger, IGameCommentService gameCommentService)
    {
        this.logger = logger;
        this.gameCommentService = gameCommentService;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> AddAsync([FromForm] AddCommentDTO dto) // Done. 
    {
        try
        {
            return await gameCommentService.AddAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");

        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> DeleteAsync(Guid id) // Done.
    {
        try
        {
            return await gameCommentService.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<CommentInfoDTO>>> GetAsync([FromQuery] GetCommentsPaginationDTO dto) // Done. 
    {
        try
        {
            return await gameCommentService.GetByPaginationAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpPost("DoLikeOperation")]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> DoLikeOperationAsync([FromForm] LikeCommentDTO dto) // Done.
    {
        try
        {
            return await gameCommentService.DoLikeOperationAsync(dto);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("CheckExistence")]
    public async Task<ActionResult<ServerResponse>> CheckExistenceAsync([FromQuery] LikeCommentDTO dto) // Done.
    {
        try
        {
            return await gameCommentService.CheckExistenceAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("GetCountOfLikes/{commentId}")]
    public async Task<ActionResult<int>> GetCountOfLikesAsync(Guid commentId) // Done.
    {
        try
        {
            return await gameCommentService.GetCountOfLikesAsync(commentId);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }
}