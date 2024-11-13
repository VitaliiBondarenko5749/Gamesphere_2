using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.BAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogOfGames.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> logger;
    private readonly ICategoryService categoryService;

    public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService)
    {
        this.logger = logger;
        this.categoryService = categoryService;
    }

    [HttpGet("/api/Categories")]
    public async Task<ActionResult<CategoryInfoDTO[]>> GetAllAsync() // Done.
    {
        try
        {
            return await categoryService.GetAllAsync() ?? Array.Empty<CategoryInfoDTO>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> AddAsync([FromForm] AddCategoryDTO categoryInsertDTO) // Done.
    {
        try
        {
            return await categoryService.AddAsync(categoryInsertDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpDelete("{categoryId}")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> DeleteAsync(Guid categoryId) // Done.
    {
        try
        {
            return await categoryService.DeleteAsync(categoryId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpGet("/api/Categories/{name}")]
    public async Task<ActionResult<CategoryInfoDTO[]>> GetTopTenByNameAsync(string name) // Done.
    {
        try
        {
            return await categoryService.GetTopTenByNameAsync(name) ?? Array.Empty<CategoryInfoDTO>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpPost("AddToGame")]
    [Authorize(Roles = "Creator, Admin")]
    public async Task<ActionResult<ServerResponse>> AddToGameAsync([FromForm] CategoryToGameDTO dto) // Done.
    {
        try
        {
            return await categoryService.AddToGameAsync(dto);
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
    public async Task<ActionResult<ServerResponse>> DeleteFromGameAsync([FromQuery] CategoryToGameDTO dto)
    {
        try
        {
            return await categoryService.DeleteFromGameAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }
}