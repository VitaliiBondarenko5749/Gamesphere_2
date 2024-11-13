using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;

namespace CatalogOfGames.BAL.Services;

public interface IGameImageService
{
    Task<ServerResponse> AddImageToGameAsync(AddGameImageDTO dto);
    Task<ServerResponse> RemoveImageFromGameAsync(Guid id);
}

public class GameImageService : IGameImageService
{
    private readonly IUnitOfWork unitOfWork;

    public GameImageService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<ServerResponse> AddImageToGameAsync(AddGameImageDTO dto)
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(dto.GameName);

        if (game is null)
        {
            return new ServerResponse { Message = $"Game {dto.GameName} does not exist!", IsSuccess = false };
        }

        GameImage gameImage = new()
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            ImageDirectory = dto.Directory
        };

        await unitOfWork.GameImageRepository.CreateAsync(gameImage);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Image has been added!", IsSuccess = true };
    }

    public async Task<ServerResponse> RemoveImageFromGameAsync(Guid id)
    {
        GameImage? gameImage = await unitOfWork.GameImageRepository.GetByIdAsync(id);

        if (gameImage is null)
        {
            return new ServerResponse { Message = "Data does not exist!", IsSuccess = false };
        }

        await unitOfWork.GameImageRepository.DeleteAsync(id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Data has been removed!", IsSuccess = true };
    }
}