using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;

namespace CatalogOfGames.BAL.Services;

public interface IGameVideoLinkService
{
    Task<ServerResponse> AddAsync(GameVideoLinkDTO dto);
    Task<ServerResponse> DeleteAsync(Guid id);
}

public class GameVideoLinkService : IGameVideoLinkService
{
    private readonly IUnitOfWork unitOfWork;

    public GameVideoLinkService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<ServerResponse> AddAsync(GameVideoLinkDTO dto)
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(dto.GameName);

        if (game is null)
        {
            return new ServerResponse { Message = $"Game {dto.GameName} does not exist!", IsSuccess = false };
        }

        GameVideoLink gameVideoLink = new()
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            Link = dto.VideoLink
        };

        await unitOfWork.GameVideoLinkRepository.CreateAsync(gameVideoLink);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Video link has been added!", IsSuccess = true };
    }

    public async Task<ServerResponse> DeleteAsync(Guid id)
    {
        GameVideoLink? gameVideoLink = await unitOfWork.GameVideoLinkRepository.GetByIdAsync(id);

        if(gameVideoLink is null)
        {
            return new ServerResponse { Message = "Data does not exist!", IsSuccess = false };
        }

        await unitOfWork.GameVideoLinkRepository.DeleteAsync(id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Data has been removed!", IsSuccess = true };
    }
}