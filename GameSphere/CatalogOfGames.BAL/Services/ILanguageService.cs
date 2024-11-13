using AutoMapper;
using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;

namespace CatalogOfGames.BAL.Services;

public interface ILanguageService
{
    Task<ServerResponse> AddAsync(AddLanguageDTO addLanguageDTO);
    Task<ServerResponse> DeleteAsync(Guid id);
    Task<LanguageInfoDTO[]?> GetTopTenByNameAsync(string name);
    Task<ServerResponse> AddToGameAsync(LanguageToGameDTO dto);
    Task<ServerResponse> DeleteFromGameAsync(LanguageToGameDTO dto);
}

public class LanguageService : ILanguageService
{
    private readonly IUnitOfWork unitOfWork;

    public LanguageService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<ServerResponse> AddAsync(AddLanguageDTO addLanguageDTO) // Done.
    {
        Language? language = await unitOfWork.LanguageRepository.GetByNameAsync(addLanguageDTO.Name);

        if(language is not null)
        {
            return new ServerResponse
            {
                Message = $"Language {addLanguageDTO.Name} already exists!",
                IsSuccess = false
            };
        }

        language = new Language
        {
            Id = Guid.NewGuid(),
            Name = addLanguageDTO.Name
        };
        
        await unitOfWork.LanguageRepository.CreateAsync(language);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"Language {language.Name} has been added!",
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> DeleteAsync(Guid id) // Done. 
    {
        Language? language = await unitOfWork.LanguageRepository.GetByIdAsync(id);

        if (language is null)
        {
            return new ServerResponse
            {
                Message = "Language is not found!",
                IsSuccess = false
            };
        }

        await unitOfWork.LanguageRepository.DeleteAsync(id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"Language {language.Name} has been removed successfully!",
            IsSuccess = true
        };
    }

    public async Task<LanguageInfoDTO[]?> GetTopTenByNameAsync(string name) // Done. 
    {
        Language[]? languages = await unitOfWork.LanguageRepository.GetTopTenByNameAsync(name);

        if(languages is null)
        {
            return null;
        }

        MapperConfiguration mapperConfiguration = new(config =>
        {
            config.CreateMap<Language, LanguageInfoDTO>();
        });

        IMapper mapper = mapperConfiguration.CreateMapper();

        return mapper.Map<LanguageInfoDTO[]>(languages);
    }

    public async Task<ServerResponse> AddToGameAsync(LanguageToGameDTO dto)
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(dto.GameName);
        Language? language = await unitOfWork.LanguageRepository.GetByIdAsync(dto.LanguageId);

        if (game is null || language is null)
        {
            return new ServerResponse { Message = "Invalid data", IsSuccess = false };
        }

        GameLanguage? gameLanguage = await unitOfWork.GameLanguageRepository.CheckExistenceAsync(game.Id, dto.LanguageId);

        if (gameLanguage is not null)
        {
            return new ServerResponse { Message = "Data already exists!", IsSuccess = false };
        }

        gameLanguage = new()
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            LanguageId = language.Id
        };

        await unitOfWork.GameLanguageRepository.CreateAsync(gameLanguage);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = $"Language {language.Name} has been added to game {game.Name}!", IsSuccess = true };
    }

    public async Task<ServerResponse> DeleteFromGameAsync(LanguageToGameDTO dto)
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(dto.GameName);
        Language? language = await unitOfWork.LanguageRepository.GetByIdAsync(dto.LanguageId);

        if (game is null || language is null)
        {
            return new ServerResponse { Message = "Invalid data", IsSuccess = false };
        }

        GameLanguage? gameLanguage = await unitOfWork.GameLanguageRepository.CheckExistenceAsync(game.Id, dto.LanguageId);

        if (gameLanguage is null)
        {
            return new ServerResponse { Message = "Data does not exist!", IsSuccess = false };
        }

        await unitOfWork.GameLanguageRepository.DeleteAsync(gameLanguage.Id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Data has been removed!", IsSuccess = true };
    }
}