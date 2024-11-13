using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.BAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogOfGames.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly ILogger<PlatformController> logger;
    private readonly IPlatformService platformService;

    public PlatformController(ILogger<PlatformController> logger, IPlatformService platformService)
    {
        this.logger = logger;
        this.platformService = platformService;
    }

    [HttpPost]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> AddAsync([FromForm] AddPlatformDTO addPlatformDTO)
    {
        try
        {
            return await platformService.AddAsync(addPlatformDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpDelete("{platformId}")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> DeleteAsync(Guid platformId)
    {
        try
        {
            return await platformService.DeleteAsync(platformId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpGet("/api/Platforms/{name}")]
    public async Task<ActionResult<PlatformInfoDTO[]>> GetTopTenByNameAsync(string name)
    {
        try
        {
            return await platformService.GetTopTenByNameAsync(name) ?? Array.Empty<PlatformInfoDTO>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpPost("AddToGame")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> AddToGameAsync([FromForm] PlatformToGameDTO dto)
    {
        try
        {
            return await platformService.AddToGameAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    // TODO: Воздух
    [HttpDelete("DeleteFromGame")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> DeleteFromGameAsync([FromQuery] PlatformToGameDTO dto)
    {
        try
        {
            return await platformService.DeleteFromGameAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }
}