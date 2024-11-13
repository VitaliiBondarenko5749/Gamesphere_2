using AutoMapper;
using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;

namespace CatalogOfGames.BAL.Services;

public interface IDeveloperService
{
    Task<ServerResponse> AddAsync(AddDeveloperDTO addDeveloperDTO);
    Task<ServerResponse> DeleteAsync(Guid id);
    Task<DeveloperInfoDTO[]?> GetTopTenByNameAsync(string name);
    Task<ServerResponse> AddToGameAsync(DeveloperToGameDTO dto);
    Task<ServerResponse> DeleteFromGameAsync(DeveloperToGameDTO dto);
}

public class DeveloperService : IDeveloperService
{
    private readonly IUnitOfWork unitOfWork;

    public DeveloperService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<ServerResponse> AddAsync(AddDeveloperDTO addDeveloperDTO) // Done. 
    {
        Developer? developer = await unitOfWork.DeveloperRepository.GetByNameAsync(addDeveloperDTO.Name);

        if (developer is not null)
        {
            return new ServerResponse
            {
                Message = $"Developer {addDeveloperDTO.Name} already exists!",
                IsSuccess = false
            };
        }

        developer = new Developer
        {
            Id = Guid.NewGuid(),
            Name = addDeveloperDTO.Name
        };

        await unitOfWork.DeveloperRepository.CreateAsync(developer);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"Developer {developer.Name} has been added!",
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> DeleteAsync(Guid id) // Done. 
    {
        Developer? developer = await unitOfWork.DeveloperRepository.GetByIdAsync(id);

        if(developer is null)
        {
            return new ServerResponse
            {
                Message = "Developer is not found!",
                IsSuccess = false
            };
        }

        await unitOfWork.DeveloperRepository.DeleteAsync(id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"Developer {developer.Name} has been removed successfully!",
            IsSuccess = true
        };
    }

    public async Task<DeveloperInfoDTO[]?> GetTopTenByNameAsync(string name) // Done. 
    {
        Developer[]? developers = await unitOfWork.DeveloperRepository.GetTopTenByNameAsync(name);

        if(developers is null)
        {
            return null;
        }

        MapperConfiguration mapperConfiguration = new(config =>
        {
            config.CreateMap<Developer, DeveloperInfoDTO>();
        });

        IMapper mapper = mapperConfiguration.CreateMapper();

        return mapper.Map<DeveloperInfoDTO[]>(developers);
    }

    public async Task<ServerResponse> AddToGameAsync(DeveloperToGameDTO dto) 
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(dto.GameName);
        Developer? developer = await unitOfWork.DeveloperRepository.GetByIdAsync(dto.DeveloperId);

        if (game is null || developer is null)
        {
            return new ServerResponse { Message = "Invalid data", IsSuccess = false };
        }

        GameDeveloper? gameDeveloper = await unitOfWork.GameDeveloperRepository.CheckExistenceAsync(game.Id, dto.DeveloperId);

        if (gameDeveloper is not null)
        {
            return new ServerResponse { Message = "Data already exists!", IsSuccess = false };
        }

        gameDeveloper = new()
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            DeveloperId = developer.Id
        };

        await unitOfWork.GameDeveloperRepository.CreateAsync(gameDeveloper);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = $"Developer {developer.Name} has been added to game {game.Name}!", IsSuccess = true };
    }

    public async Task<ServerResponse> DeleteFromGameAsync(DeveloperToGameDTO dto)
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(dto.GameName);
        Developer? developer = await unitOfWork.DeveloperRepository.GetByIdAsync(dto.DeveloperId);

        if (game is null || developer is null)
        {
            return new ServerResponse { Message = "Invalid data", IsSuccess = false };
        }

        GameDeveloper? gameDeveloper = await unitOfWork.GameDeveloperRepository.CheckExistenceAsync(game.Id, dto.DeveloperId);

        if (gameDeveloper is null)
        {
            return new ServerResponse { Message = "Data does not exist!", IsSuccess = false };
        }

        await unitOfWork.GameDeveloperRepository.DeleteAsync(gameDeveloper.Id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Data has been removed!", IsSuccess = true };
    }
}