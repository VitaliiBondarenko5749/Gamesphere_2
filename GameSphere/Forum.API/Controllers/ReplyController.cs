using Forum.BAL.DTOs;
using Forum.BAL.Services;
using Helpers;
using Helpers.GeneralClasses.Forum.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Forum.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReplyController : ControllerBase
{
    private readonly ILogger<ReplyController> logger;
    private readonly IReplyService replyService;

    public ReplyController(ILogger<ReplyController> logger, IReplyService replyService)
    {
        this.logger = logger;
        this.replyService = replyService;
    }

    [HttpGet]
    public async Task<ActionResult<PageResult<ReplyInfoDTO>>> GetAllAsync([FromQuery] GetRepliesPaginationDTO dto)
    {
        try
        {
            return await replyService.GetAllAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> SendReplyAsync([FromForm] ReplyToPostDTO dto)
    {
        try
        {
            return await replyService.SendReplyAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> DeleteAsync(Guid id)
    {
        try
        {
            return await replyService.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }
}