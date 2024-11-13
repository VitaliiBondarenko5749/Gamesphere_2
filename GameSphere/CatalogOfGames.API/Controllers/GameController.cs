using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.BAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Helpers.GeneralClasses.Forum.DTOs;

namespace CatalogOfGames.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GameController : ControllerBase
{
    private readonly ILogger<GameController> logger;
    private readonly IGameService gameService;
    private readonly IGameImageService gameImageService;
    private readonly IGameVideoLinkService gameVideoLinkService;

    public GameController(ILogger<GameController> logger, IGameService gameService, IGameImageService gameImageService,
        IGameVideoLinkService gameVideoLinkService)
    {
        this.logger = logger;
        this.gameService = gameService;
        this.gameImageService = gameImageService;
        this.gameVideoLinkService = gameVideoLinkService;
    }

    [HttpGet("CheckNameExistence/{name}")]
    public async Task<ActionResult<ServerResponse>> CheckNameExistenceAsync(string name) // DONE.
    {
        try
        {
            return await gameService.CheckNameExistenceAsync(name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR...");
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Creator")]
    public async Task<ActionResult<ServerResponse>> AddAsync([FromForm] AddGameDTO gameDTO) // DONE.
    {
        try
        {
            return await gameService.AddAsync(gameDTO);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR...");
        }
    }

    [HttpPut("ChangeGameIcon")]
    public async Task<ActionResult<ServerResponse>> ChangeGameIconAsync([FromBody] ChangeImageDirectoryDTO dto) // DONE.
    {
        try
        {
            return await gameService.UpdateGameIconAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR...");
        }
    }

    [HttpPost("AddImageToGame")]
    public async Task<ActionResult<ServerResponse>> AddImageToGameAsync([FromBody] AddGameImageDTO dto) // Done. 
    {
        try
        {
            return await gameImageService.AddImageToGameAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR...");
        }
    }

    [HttpDelete("DeleteGameImage/{id}")]
    public async Task<ActionResult<ServerResponse>> DeleteGameImageAsync(Guid id) // Done.
    {
        try
        {
            return await gameImageService.RemoveImageFromGameAsync(id);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR...");
        }
    }

    [HttpPost("AddGameVideoLink")]
    [Authorize(Roles = "Admin, Creator")]
    public async Task<ActionResult<ServerResponse>> AddGameVideoLinkAsync([FromForm] GameVideoLinkDTO dto) // Done. 
    {
        try
        {
            return await gameVideoLinkService.AddAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR...");
        }
    }

    // TODO: Воздух
    [HttpDelete("DeleteGameVideoLink/{id}")]
    [Authorize(Roles = "Admin, Creator")]
    public async Task<ActionResult<ServerResponse>> DeleteVideoLinkAsync(Guid id)
    {
        try
        {
            return await gameVideoLinkService.DeleteAsync(id);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR...");
        }
    }

    [HttpGet("/api/Games/{name}")]
    public async Task<ActionResult<GameListInfoDTO[]>> GetTopTenByNameAsync(string name) // Done. 
    {
        try
        {
            return await gameService.GetTop10ByNameAsync(name) ?? Array.Empty<GameListInfoDTO>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServerResponse>> DeleteAsync(Guid id) // Done. 
    {
        try
        {
            return await gameService.DeleteAsync(id);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpGet("Directories/{id}")]
    public async Task<ActionResult<List<string>>> GetImageDirectoriesByGameIdAsync(Guid id) // Done. 
    {
        try
        {
            return await gameService.GetAllImageDirectoriesByGameIdAsync(id);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpGet("GetAllGames")]
    public async Task<ActionResult<PageResult<ShortGameInfoDTO>>> GetAllGamesAsync([FromQuery] GetGamesPaginationDTO dto) // Done.
    {
        try
        {
            return await gameService.GetAllAsync(dto);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpGet("GetGamesByCategory")]
    public async Task<ActionResult<PageResult<ShortGameInfoDTO>>> GetGamesByCategoryAsync([FromQuery] GetGamesPaginationDTO dto)
    {
        try
        {
            return await gameService.GetAllByCategoryNameAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    } // Done.

    [HttpGet("GetFavoriteGames")]
    [Authorize]
    public async Task<ActionResult<PageResult<ShortGameInfoDTO>>> GetFavoriteGamesAsync([FromQuery] GetGamesPaginationDTO dto)
    {
        try
        {
            return await gameService.GetFavoriteByUserIdAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    } // Done.

    [HttpGet("{id}")]
    public async Task<ActionResult<FullGameInfoDTO>> GetByIdAsync(Guid id)
    {
        try
        {
            return await gameService.GetByIdAsync(id);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }

    [HttpGet("GetShortGameInfo/{id}")]
    public async Task<ActionResult<PostGameInfoDTO>> GetShortGameInfoByIdAsync(Guid id)
    {
        try
        {
            return await gameService.GetShortGameInfoByIdAsync(id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "Internal Server Error");
        }
    }


    [HttpPost("AddToFavoriteList")]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> AddToFavoriteListAsync([FromForm] ToFavoriteListDTO dto) // Done. 
    {
        try
        {
            return await gameService.AddToFavoriteListAsync(dto);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpDelete("RemoveFromFavoriteList")]
    [Authorize]
    public async Task<ActionResult<ServerResponse>> RemoveFromFavoriteListAsync([FromQuery] ToFavoriteListDTO dto) // Done. 
    {
        try
        {
            return await gameService.RemoveFromFavoriteListAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("CheckGameInFavoriteList")]
    public async Task<ActionResult<ServerResponse>> CheckHavingInFavoriteListAsync([FromQuery] ToFavoriteListDTO dto) // Done. 
    {
        try
        {
            return await gameService.CheckHavingInFavoriteListAsync(dto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpGet("GetRecGames/{userId}")]
    [Authorize]
    public async Task<ActionResult<List<ShortGameInfoDTO>>> GetRecGamesAsync(string userId) // Done. 
    {
        try
        {
            return await gameService.GetRecommendedGamesAsync(userId);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }

    [HttpDelete("DeleteUserData/{userId}")]
    public async Task<ActionResult<ServerResponse>> DeleteUserDataAsync(string userId)
    {
        try
        {
            return await gameService.DeleteUserDataAsync(userId);
        }
        catch(Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(501, "INTERNAL SERVER ERROR");
        }
    }
}