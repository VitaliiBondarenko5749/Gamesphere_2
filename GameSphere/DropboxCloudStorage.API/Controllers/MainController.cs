using Helpers;
using DropboxCloudStorage.API.Services;
using DropboxCloudStorage.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DropboxCloudStorage.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MainController : ControllerBase
{
    private readonly ILogger<MainController> logger;
    private readonly IDropboxService dropboxService;

    public MainController(ILogger<MainController> logger, IDropboxService dropboxService)
    {
        this.logger = logger;
        this.dropboxService = dropboxService;
    }

    [HttpPost("UploadFile")]
    public async Task<ActionResult<ServerResponse>> UploadFileAsync([FromBody] UploadFileViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServerResponse result = await dropboxService.UploadFileAsync(model);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }

    [HttpDelete("DeleteFile/{filePath}")]
    public async Task<IActionResult> DeleteFileAsync(string filePath)
    {
        try
        {
            filePath = UrlEncoder.Decode(filePath);
            ServerResponse result = await dropboxService.DeleteFileAsync(filePath);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return NotFound();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal server error!");
        }
    }
}