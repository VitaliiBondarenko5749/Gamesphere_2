using Helpers;
using Aggregator.Models.Authentication;
using Aggregator.Models.CatalogOfGames;
using Aggregator.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aggregator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly ICloudStorageService cloudStorageService;
    private readonly IGameService gameService;
    private readonly ILogger<GameController> logger;
    private readonly IAuthenticationService authenticationService;

    public GameController(ICloudStorageService cloudStorageService, IGameService gameService, ILogger<GameController> logger, 
        IAuthenticationService authenticationService)
    {
        this.cloudStorageService = cloudStorageService;
        this.gameService = gameService;
        this.logger = logger;
        this.authenticationService = authenticationService;
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ServerResponse> DeleteAsync(Guid id) // Done.
    {
        List<string> directories = await gameService.GetImageDirectoriesAsync(id);

        directories.ForEach(async directory => 
        {
            await cloudStorageService.DeleteFileAsync(UrlEncoder.Encode($"/{directory}"));
        });

        return await gameService.DeleteGameAsync(id);
    }

    [HttpGet("GetComments")]
    public async Task<ActionResult<PageResult<FullCommentInfoDTO>>> GetCommentsAsync([FromQuery] GetCommentsPaginationDTO dto) // Done. 
    {
        try
        {
            PageResult<CommentInfoDTO> pageResult = await gameService.GetCommentsAsync(dto);

            PageResult<FullCommentInfoDTO> result = new()
            {
                Count = pageResult.Count,
                PageIndex = pageResult.PageIndex,
                PageSize = pageResult.PageSize
            };

            List<FullCommentInfoDTO> comments = new();

            if(pageResult.Items is not null && pageResult.Items.Count > 0)
            {
                foreach (CommentInfoDTO commentInfo in pageResult.Items)
                {
                    UserInfoDTO userInfo = await authenticationService.GetUserInfoAsync(commentInfo.UserId);

                    FullCommentInfoDTO commentInfoDTO = new() 
                    {
                        Id = commentInfo.Id,  
                        Content = commentInfo.Content,
                        CreatedAt = commentInfo.CreatedAt,
                        UserId = userInfo.Id,
                        Email = userInfo.Email,
                        Role = userInfo.Role,
                        UserName = userInfo.UserName,
                        IconDirectory = userInfo.IconDirectory
                    };

                    comments.Add(commentInfoDTO);
                }
            }

            result.Items = comments;

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }
}