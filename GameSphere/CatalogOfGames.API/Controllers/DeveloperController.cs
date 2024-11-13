using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.BAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogOfGames.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DeveloperController : ControllerBase
{
    private readonly ILogger<DeveloperController> logger;
    private readonly IDeveloperService developerService;

    public DeveloperController(ILogger<DeveloperController> logger, IDeveloperService developerService)
    {
        this.logger = logger;
        this.developerService = developerService;
    }

    [HttpPost]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> AddAsync([FromForm] AddDeveloperDTO addDeveloperDTO) // Done.
    {
        try
        {
            return await developerService.AddAsync(addDeveloperDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpDelete("{developerId}")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> DeleteAsync(Guid developerId) // Done.
    {
        try
        {
            return await developerService.DeleteAsync(developerId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpGet("/api/Developers/{name}")]
    public async Task<ActionResult<DeveloperInfoDTO[]>> GetTopTenByNameAsync(string name) // Done.
    {
        try
        {
            return await developerService.GetTopTenByNameAsync(name) ?? Array.Empty<DeveloperInfoDTO>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpPost("AddToGame")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> AddToGameAsync([FromForm] DeveloperToGameDTO dto)
    {
        try
        {
            return await developerService.AddToGameAsync(dto);
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
    public async Task<ActionResult<ServerResponse>> DeleteFromGameAsync([FromQuery] DeveloperToGameDTO dto) // Done.
    {
        try
        {
            return await developerService.DeleteFromGameAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }
}