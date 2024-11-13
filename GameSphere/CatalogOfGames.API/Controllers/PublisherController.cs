using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.BAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogOfGames.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PublisherController : ControllerBase
{
    private readonly ILogger<PublisherController> logger;
    private readonly IPublisherService publisherService;

    public PublisherController(ILogger<PublisherController> logger, IPublisherService publisherService)
    {
        this.logger = logger;
        this.publisherService = publisherService;
    }

    [HttpPost]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> AddAsync([FromForm] AddPublisherDTO addPublisherDTO)
    {
        try
        {
            return await publisherService.AddAsync(addPublisherDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpDelete("{publisherId}")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> DeleteAsync(Guid publisherId)
    {
        try
        {
            return await publisherService.DeleteAsync(publisherId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpGet("/api/Publishers/{name}")]
    public async Task<ActionResult<PublisherInfoDTO[]>> GetTopTenByNameAsync(string name)
    {
        try
        {
            return await publisherService.GetTopTenByNameAsync(name) ?? Array.Empty<PublisherInfoDTO>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpPut("UpdateDataInGame")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> UpdateDataInGameAsync(UpdatePublisherInGameDTO dto)
    {
        try
        {
            return await publisherService.UpdateDataInGameAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }
}