using Aggregator.Models.Authentication;
using Aggregator.Models.Forum;
using Aggregator.Services;
using Helpers;
using Helpers.GeneralClasses.Forum.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aggregator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ForumController : ControllerBase
{
    private readonly ILogger<ForumController> logger;
    private readonly IForumService forumService;
    private readonly IAuthenticationService authenticationService;
    private readonly IGameService gameService;

    public ForumController(ILogger<ForumController> logger, IForumService forumService, IAuthenticationService authenticationService,
        IGameService gameService)
    {
        this.logger = logger;
        this.forumService = forumService;
        this.authenticationService = authenticationService;
        this.gameService = gameService;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<Models.Forum.ShortPostInfoDTO>>> GetAllPostsAsync([FromQuery] GetTopicsPaginationDTO dto)
    {
        try
        {
            PageResult<Helpers.GeneralClasses.Forum.DTOs.ShortPostInfoDTO> queryResult = await forumService.GetAllPostsAsync(dto);

            PageResult<Models.Forum.ShortPostInfoDTO> pageResult = new()
            {
                Count = queryResult.Count,
                PageIndex = queryResult.PageIndex,
                PageSize = queryResult.PageSize,
                Items = new List<Models.Forum.ShortPostInfoDTO>()
            };

            if (queryResult.Items is not null)
            {
                foreach (Helpers.GeneralClasses.Forum.DTOs.ShortPostInfoDTO post in queryResult.Items)
                {
                    UserInfoDTO userInfoDTO = await authenticationService.GetUserInfoAsync(post.UserId);

                    Models.Forum.ShortPostInfoDTO shortPostInfoDTO = new()
                    {
                        Id = post.Id,
                        Topic = post.Topic,
                        CreatedAt = post.CreatedAt.ToString("dd.MM.yy"),
                        Views = post.Views,
                        NumberOfReplies = post.NumberOfReplies,
                        UserInfo = new ForumUserInfoDTO()
                        {
                            Id = userInfoDTO.Id,
                            UserName = userInfoDTO.UserName,
                            IconDirectory = userInfoDTO.IconDirectory
                        }
                    };

                    pageResult.Items.Add(shortPostInfoDTO);
                }
            }

            return pageResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FullPostInfoDTO>> GetFullPostInfoAsync(Guid id)
    {
        try
        {
            PostInfoDTO postInfoDTO = await forumService.GetPostByIdAsync(id);
            UserInfoDTO userInfoDTO = await authenticationService.GetUserInfoAsync(postInfoDTO.UserId);
            PostGameInfoDTO? postGameInfoDTO = null;

            if (postInfoDTO.GameId is not null)
            {
                postGameInfoDTO = await gameService.GetGameInfoByIdAsync((Guid)postInfoDTO.GameId);
            }

            FullPostInfoDTO post = new()
            {
                Id = postInfoDTO.Id,
                Topic = postInfoDTO.Topic,
                Content = postInfoDTO.Content,
                CreatedAt = postInfoDTO.CreatedAt.ToString("dd.MM.yyyy | HH:mm"),
                Views = postInfoDTO.Views,
                User = userInfoDTO,
                Game = postGameInfoDTO
            };

            return post;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("GetReplies")]
    public async Task<ActionResult<PageResult<FullReplyInfoDTO>>> GetAllRepliesAsync([FromQuery] GetRepliesPaginationDTO dto)
    {
        try
        {
            PageResult<ReplyInfoDTO> queryResult = await forumService.GetAllRepliesAsync(dto);
            PageResult<FullReplyInfoDTO> pageResult = new()
            {
                Count = queryResult.Count,
                PageIndex = queryResult.PageIndex,
                PageSize = queryResult.PageSize,
                Items = new List<FullReplyInfoDTO>()
            };

            if (queryResult.Items is not null)
            {
                foreach (ReplyInfoDTO replyInfo in queryResult.Items)
                {
                    UserInfoDTO userInfoDTO = await authenticationService.GetUserInfoAsync(replyInfo.UserId);

                    FullReplyInfoDTO fullReplyInfoDTO = new()
                    {
                        Id = replyInfo.Id,
                        PostId = replyInfo.PostId,
                        Content = replyInfo.Content,
                        CreatedAt = replyInfo.CreatedAt.ToString("dd.MM.yyyy | HH:mm"),
                        ReplyToId = replyInfo.ReplyToId,
                        User = userInfoDTO
                    };

                    pageResult.Items.Add(fullReplyInfoDTO);
                }
            }

            return pageResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("GetSimilarPosts")]
    public async Task<ActionResult<List<Models.Forum.ShortPostInfoDTO>>> GetSimilarPostsAsync([FromQuery] GetSimilarPostsDTO dto)
    {
        try
        {
            List<Helpers.GeneralClasses.Forum.DTOs.ShortPostInfoDTO> queryResult = await forumService.GetSimilarPostsAsync(dto);
            List<Models.Forum.ShortPostInfoDTO> posts = new();

            foreach (Helpers.GeneralClasses.Forum.DTOs.ShortPostInfoDTO post in queryResult)
            {
                UserInfoDTO userInfoDTO = await authenticationService.GetUserInfoAsync(post.UserId);

                Models.Forum.ShortPostInfoDTO shortPostInfoDTO = new()
                {
                    Id = post.Id,
                    Topic = post.Topic,
                    CreatedAt = post.CreatedAt.ToString("dd.MM.yy"),
                    Views = post.Views,
                    NumberOfReplies = post.NumberOfReplies,
                    UserInfo = new ForumUserInfoDTO()
                    {
                        Id = userInfoDTO.Id,
                        UserName = userInfoDTO.UserName,
                        IconDirectory = userInfoDTO.IconDirectory
                    }
                };

                posts.Add(shortPostInfoDTO);
            }

            return posts;
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }
}