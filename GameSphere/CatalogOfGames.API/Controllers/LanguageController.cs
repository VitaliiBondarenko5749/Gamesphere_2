using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.BAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogOfGames.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LanguageController : ControllerBase
{
    private readonly ILogger<LanguageController> logger;
    private readonly ILanguageService languageService;

    public LanguageController(ILogger<LanguageController> logger, ILanguageService languageService)
    {
        this.logger = logger;
        this.languageService = languageService;
    }

    [HttpPost]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> AddAsync([FromForm] AddLanguageDTO addLanguageDTO) // Done.
    {
        try
        {
            return await languageService.AddAsync(addLanguageDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpDelete("{languageId}")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> DeleteAsync(Guid languageId) // Done.
    {
        try
        {
            return await languageService.DeleteAsync(languageId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpGet("/api/Languages/{name}")]
    public async Task<ActionResult<LanguageInfoDTO[]>> GetTopTenByNameAsync(string name) // Done.
    {
        try
        {
            return await languageService.GetTopTenByNameAsync(name) ?? Array.Empty<LanguageInfoDTO>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpPost("AddToGame")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> AddToGameAsync([FromForm] LanguageToGameDTO dto)
    {
        try
        {
            return await languageService.AddToGameAsync(dto);
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
    public async Task<ActionResult<ServerResponse>> DeleteFromGameAsync([FromQuery] LanguageToGameDTO dto)
    {
        try
        {
            return await languageService.DeleteFromGameAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }
}