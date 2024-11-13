using AutoMapper;
using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;

namespace CatalogOfGames.BAL.Services;

public interface IPlatformService
{
    Task<ServerResponse> AddAsync(AddPlatformDTO addPlatformDTO);
    Task<ServerResponse> DeleteAsync(Guid id);
    Task<PlatformInfoDTO[]?> GetTopTenByNameAsync(string name);
    Task<ServerResponse> AddToGameAsync(PlatformToGameDTO dto);
    Task<ServerResponse> DeleteFromGameAsync(PlatformToGameDTO dto);
}

public class PlatformService : IPlatformService
{
    private readonly IUnitOfWork unitOfWork;

    public PlatformService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<ServerResponse> AddAsync(AddPlatformDTO addPlatformDTO) // Done. 
    {
        Platform? platform = await unitOfWork.PlatformRepository.GetByNameAsync(addPlatformDTO.Name);

        if(platform is not null)
        {
            return new ServerResponse
            {
                Message = $"Platform {addPlatformDTO.Name} already exists!",
                IsSuccess = false
            };
        }

        platform = new Platform
        {
            Id = Guid.NewGuid(),
            Name = addPlatformDTO.Name
        };

        await unitOfWork.PlatformRepository.CreateAsync(platform);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"Platform {platform.Name} has been added!",
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> DeleteAsync(Guid id) // Done. 
    {
        Platform? platform = await unitOfWork.PlatformRepository.GetByIdAsync(id);

        if (platform is null)
        {
            return new ServerResponse
            {
                Message = "Platform is not found!",
                IsSuccess = false
            };
        }

        await unitOfWork.PlatformRepository.DeleteAsync(id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"Platform {platform.Name} has been removed successfully!",
            IsSuccess = true
        };
    }

    public async Task<PlatformInfoDTO[]?> GetTopTenByNameAsync(string name) // Done. 
    {
        Platform[]? platforms = await unitOfWork.PlatformRepository.GetTopTenByNameAsync(name);

        if(platforms is null)
        {
            return null;
        }

        MapperConfiguration mapperConfiguration = new(config =>
        {
            config.CreateMap<Platform, PlatformInfoDTO>();
        });

        IMapper mapper = mapperConfiguration.CreateMapper();

        return mapper.Map<PlatformInfoDTO[]>(platforms);
    }

    public async Task<ServerResponse> AddToGameAsync(PlatformToGameDTO dto)
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(dto.GameName);
        Platform? platform = await unitOfWork.PlatformRepository.GetByIdAsync(dto.PlatformId);

        if (game is null || platform is null)
        {
            return new ServerResponse { Message = "Invalid data", IsSuccess = false };
        }

        GamePlatform? gamePlatform = await unitOfWork.GamePlatformRepository.CheckExistenceAsync(game.Id, dto.PlatformId);

        if (gamePlatform is not null)
        {
            return new ServerResponse { Message = "Data already exists!", IsSuccess = false };
        }

        gamePlatform = new()
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            PlatformId = platform.Id
        };

        await unitOfWork.GamePlatformRepository.CreateAsync(gamePlatform);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = $"Platform {platform.Name} has been added to game {game.Name}!", IsSuccess = true };
    }

    public async Task<ServerResponse> DeleteFromGameAsync(PlatformToGameDTO dto)
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(dto.GameName);
        Platform? platform = await unitOfWork.PlatformRepository.GetByIdAsync(dto.PlatformId);

        if (game is null || platform is null)
        {
            return new ServerResponse { Message = "Invalid data", IsSuccess = false };
        }

        GamePlatform? gamePlatform = await unitOfWork.GamePlatformRepository.CheckExistenceAsync(game.Id, dto.PlatformId);

        if (gamePlatform is null)
        {
            return new ServerResponse { Message = "Data does not exist!", IsSuccess = false };
        }

        await unitOfWork.GamePlatformRepository.DeleteAsync(gamePlatform.Id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Data has been removed!", IsSuccess = true };
    }
}