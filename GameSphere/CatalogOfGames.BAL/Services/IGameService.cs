using AutoMapper;
using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.BAL.Validators;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Enums;
using CatalogOfGames.DAL.Repositories;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Helpers.GeneralClasses.Forum.DTOs;

namespace CatalogOfGames.BAL.Services;

#pragma warning disable

public interface IGameService
{
    Task<PostGameInfoDTO> GetShortGameInfoByIdAsync(Guid id);
    Task<PageResult<ShortGameInfoDTO>> GetAllAsync(GetGamesPaginationDTO dto);
    Task<PageResult<ShortGameInfoDTO>> GetAllByCategoryNameAsync(GetGamesPaginationDTO dto);
    Task<List<ShortGameInfoDTO>> GetRecommendedGamesAsync(string userId);
    Task<PageResult<ShortGameInfoDTO>> GetFavoriteByUserIdAsync(GetGamesPaginationDTO dto);
    Task<FullGameInfoDTO> GetByIdAsync(Guid gameId);
    Task<ServerResponse> AddToFavoriteListAsync(ToFavoriteListDTO dto);
    Task<ServerResponse> RemoveFromFavoriteListAsync(ToFavoriteListDTO dto);
    Task<ServerResponse> CheckHavingInFavoriteListAsync(ToFavoriteListDTO dto);
    Task<ServerResponse> AddAsync(AddGameDTO addGameDTO);
    Task<ServerResponse> CheckNameExistenceAsync(string name);
    Task<ServerResponse> UpdateGameIconAsync(ChangeImageDirectoryDTO dto);
    Task<GameListInfoDTO[]?> GetTop10ByNameAsync(string name);
    Task<ServerResponse> DeleteAsync(Guid id);
    Task<List<string>> GetAllImageDirectoriesByGameIdAsync(Guid gameId);
    Task<ServerResponse> DeleteUserDataAsync(string userId);
}

public class GameService : IGameService
{
    private readonly IUnitOfWork unitOfWork;

    public GameService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<PostGameInfoDTO> GetShortGameInfoByIdAsync(Guid id)
    {
        Game game = await unitOfWork.GameRepository.GetByIdAsync(id);

        return new PostGameInfoDTO { Id = game.Id, Name = game.Name, MainImageDirectory = game.MainImageDirectory };
    }

    public async Task<PageResult<ShortGameInfoDTO>> GetAllAsync(GetGamesPaginationDTO dto) // Done. 
    {
        IQueryable<Game>? query = await unitOfWork.GameRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(dto.SearchText))
        {
            dto.SearchText = dto.SearchText.ToLower();
            query = query.Where(q => q.Name.ToLower().Contains(dto.SearchText));
        }

        int totalCount = query.Count();

        if (dto.Page.HasValue)
        {
            query = query.Skip((dto.Page.Value - 1) * dto.PageSize);
        }

        query = query.Take(dto.PageSize);

        query = dto.GameSorter.Equals(GameSorter.DateByDescending) ? query.OrderByDescending(q => q.ReleaseDate)
            : dto.GameSorter.Equals(GameSorter.DateByAscending) ? query.OrderBy(q => q.ReleaseDate)
            : dto.GameSorter.Equals(GameSorter.PopularityByDescending) ? query.OrderByDescending(q => q.Views)
            : query.OrderBy(q => q.Views);

        List<ShortGameInfoDTO> games = await query.Select(g => new ShortGameInfoDTO
        {
            Id = g.Id,
            Name = g.Name,
            MainImageDirectory = g.MainImageDirectory ?? string.Empty
        }).ToListAsync();

        return new PageResult<ShortGameInfoDTO>
        {
            Count = totalCount,
            PageIndex = dto.Page ?? 1,
            PageSize = dto.PageSize,
            Items = games
        };
    }

    public async Task<PageResult<ShortGameInfoDTO>> GetAllByCategoryNameAsync(GetGamesPaginationDTO dto) // Done.
    {
        IQueryable<Game>? query = await unitOfWork.GameRepository.GetAllByCategoryNameAsync(dto.CategoryName 
            ?? throw new NullReferenceException("Category name is nullable!"));

        if (!string.IsNullOrEmpty(dto.SearchText))
        {
            dto.SearchText = dto.SearchText.ToLower();
            query = query.Where(q => q.Name.ToLower().Contains(dto.SearchText));
        }

        int totalCount = query.Count();

        if (dto.Page.HasValue)
        {
            query = query.Skip((dto.Page.Value - 1) * dto.PageSize);
        }

        query = query.Take(dto.PageSize);

        query = dto.GameSorter.Equals(GameSorter.DateByDescending) ? query.OrderByDescending(q => q.ReleaseDate)
            : dto.GameSorter.Equals(GameSorter.DateByAscending) ? query.OrderBy(q => q.ReleaseDate)
            : dto.GameSorter.Equals(GameSorter.PopularityByDescending) ? query.OrderByDescending(q => q.Views)
            : query.OrderBy(q => q.Views);

        List<ShortGameInfoDTO> games = await query.Select(g => new ShortGameInfoDTO
        {
            Id = g.Id,
            Name = g.Name,
            MainImageDirectory = g.MainImageDirectory ?? string.Empty
        }).ToListAsync();

        return new PageResult<ShortGameInfoDTO>
        {
            Count = totalCount,
            PageIndex = dto.Page ?? 1,
            PageSize = dto.PageSize,
            Items = games
        };
    }

    public async Task<List<ShortGameInfoDTO>> GetRecommendedGamesAsync(string userId)
    {
        List<Game> favoriteGames = await unitOfWork.GameRepository.GetFavoriteGamesForRecAsync(userId);
        List<string> favoriteCategories = favoriteGames
            .SelectMany(g => g.GameCategories)
            .Select(gc => gc.Category.Name)
            .Distinct()
            .ToList();

        List<Game> recommendedGames = await unitOfWork.GameRepository.GetGamesByCategoriesAsync(favoriteCategories);

        HashSet<Guid> favoriteGameIds = favoriteGames.Select(g => g.Id).ToHashSet();
        recommendedGames = recommendedGames.Where(g => !favoriteGameIds.Contains(g.Id)).ToList();

        recommendedGames.Shuffle();

        List<ShortGameInfoDTO> games = recommendedGames.Select(g => new ShortGameInfoDTO
        {
            Id = g.Id,
            Name = g.Name,
            MainImageDirectory = g.MainImageDirectory
        }).Take(6).ToList();

        return games;
    }

    public async Task<PageResult<ShortGameInfoDTO>> GetFavoriteByUserIdAsync(GetGamesPaginationDTO dto)
    {
        IQueryable<Game>? query = await unitOfWork.GameRepository.GetFavoriteByUserIdAsync(dto.UserId 
            ?? throw new NullReferenceException("UserId is nullable!"));

        if (!string.IsNullOrEmpty(dto.SearchText))
        {
            dto.SearchText = dto.SearchText.ToLower();
            query = query.Where(q => q.Name.ToLower().Contains(dto.SearchText));
        }

        int totalCount = query.Count();

        if (dto.Page.HasValue)
        {
            query = query.Skip((dto.Page.Value - 1) * dto.PageSize);
        }

        query = query.Take(dto.PageSize);

        query = dto.GameSorter.Equals(GameSorter.DateByDescending) ? query.OrderByDescending(q => q.ReleaseDate)
            : dto.GameSorter.Equals(GameSorter.DateByAscending) ? query.OrderBy(q => q.ReleaseDate)
            : dto.GameSorter.Equals(GameSorter.PopularityByDescending) ? query.OrderByDescending(q => q.Views)
            : query.OrderBy(q => q.Views);

        List<ShortGameInfoDTO> games = await query.Select(g => new ShortGameInfoDTO
        {
            Id = g.Id,
            Name = g.Name,
            MainImageDirectory = g.MainImageDirectory ?? string.Empty
        }).ToListAsync();

        return new PageResult<ShortGameInfoDTO>
        {
            Count = totalCount,
            PageIndex = dto.Page ?? 1,
            PageSize = dto.PageSize,
            Items = games
        };
    }

    public async Task<FullGameInfoDTO> GetByIdAsync(Guid gameId) // Done. 
    {
        Game game = await unitOfWork.GameRepository.GetByIdAsync(gameId);

        game.Views++;

        unitOfWork.GameRepository.UpdateAsync(game);

        await unitOfWork.SaveChangesAsync();

        game = await unitOfWork.GameRepository.GetFullGameDataByIdAsync(gameId) 
            ?? throw new ArgumentNullException("Game is not found!");

        FullGameInfoDTO gameDto = new()
        {
            Id = game.Id,
            Name = game.Name,
            ReleaseDate = $"{game.ReleaseDate.Day}.{game.ReleaseDate.Month}.{game.ReleaseDate.Year}",
            Description = game.Description,
            MainImageDirectory = game.MainImageDirectory ?? string.Empty,
            Views = game.Views,
            PublisherName = game.Publisher.Name ?? string.Empty,
            Categories = game.GameCategories.Select(gc => gc.Category.Name).ToList(),
            Developers = game.GameDevelopers.Select(gd => gd.Developer.Name).ToList(),
            Languages = game.GameLanguages.Select(gl => gl.Language.Name).ToList(),
            Platforms = game.GamePlatforms.Select(gp => gp.Platform.Name).ToList(),
            Images = game.GameImages.Select(gi => gi.ImageDirectory).ToList(),
            VideoLinks = game.GameVideoLinks.Select(gvd => gvd.Link).ToList()
        };

        return gameDto;
    }

    public async Task<ServerResponse> AddToFavoriteListAsync(ToFavoriteListDTO dto) // Done. 
    {
        FavoriteGame? favoriteGame = await unitOfWork.FavoriteGameRepository.GetByUserIdAndGameIdAsync(dto.UserId, dto.GameId);

        if(favoriteGame is not null) 
        {
            return new ServerResponse
            {
                Message = "Logic error!",
                IsSuccess = false
            };
        }

        favoriteGame = new()
        {
            Id = Guid.NewGuid(),
            GameId = dto.GameId,
            UserId = dto.UserId
        };

        await unitOfWork.FavoriteGameRepository.CreateAsync(favoriteGame);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = "Game has been added to favorite list successfully!",
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> RemoveFromFavoriteListAsync(ToFavoriteListDTO dto) // Done. 
    {
        FavoriteGame? favoriteGame = await unitOfWork.FavoriteGameRepository.GetByUserIdAndGameIdAsync(dto.UserId, dto.GameId);

        if (favoriteGame == null)
        {
            return new ServerResponse
            {
                Message = "Not found such favorite game...",
                IsSuccess = false
            };
        }

        await unitOfWork.FavoriteGameRepository.DeleteAsync(favoriteGame.Id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = "User removed game from favorite list successfully!",
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> CheckHavingInFavoriteListAsync(ToFavoriteListDTO dto) // Done. 
    {
        FavoriteGame? favoriteGame = await unitOfWork.FavoriteGameRepository.GetByUserIdAndGameIdAsync(dto.UserId, dto.GameId);

        if(favoriteGame is null)
        {
            return new ServerResponse
            {
                Message = $"User with id {dto.UserId} does't have such game in favorite list!",
                IsSuccess = false
            };
        }

        return new ServerResponse
        {
            Message = $"User with id {dto.UserId} has category in favorite list!",
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> AddAsync(AddGameDTO addGameDTO) // Done. 
    {
        AddGameValidator validator = new();
        ValidationResult validationResult = await validator.ValidateAsync(addGameDTO);

        if (!validationResult.IsValid)
        {
            string errors = validationResult.ToString("~");

            return new ServerResponse { Message = "Data are not valid!", IsSuccess = false, Errors = errors.Split('~') };
        }

        Game? game = await unitOfWork.GameRepository.GetByNameAsync(addGameDTO.Name);
        
        if(game is not null)
        {
            return new ServerResponse { Message = $"Game {addGameDTO.Name} already exists!", IsSuccess = false };
        }

        game = new Game 
        { 
            Id = Guid.NewGuid(),
            Name = addGameDTO.Name, 
            Description = addGameDTO.Description,
            ReleaseDate = addGameDTO.ReleaseDate,
            PublisherId = addGameDTO.PublisherId,
            Views = 0
        };

        await unitOfWork.GameRepository.CreateAsync(game);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = game.Id.ToString(), IsSuccess = true };
    }

    public async Task<ServerResponse> CheckNameExistenceAsync(string name) // Done. 
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(name);

        return new ServerResponse
        {
            Message = (game is not null) ? $"Game {game.Name} is already taken!" : string.Empty,
            IsSuccess = game is null
        };
    }

    public async Task<ServerResponse> UpdateGameIconAsync(ChangeImageDirectoryDTO dto) // Done. 
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(dto.GameName);

        if(game is null)
        {
            return new ServerResponse { Message = $"Game does not exist!", IsSuccess = false };
        }

        game.MainImageDirectory = dto.Directory;

        unitOfWork.GameRepository.UpdateAsync(game);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Directory has been updated!", IsSuccess = true };
    }

    public async Task<GameListInfoDTO[]?> GetTop10ByNameAsync(string name) // Done. 
    {
        Game[]? games = await unitOfWork.GameRepository.GetTopTenByNameAsync(name);

        if(games is null)
        {
            return null;
        }

        MapperConfiguration mapperConfiguration = new(config =>
        {
            config.CreateMap<Game, GameListInfoDTO>();
        });

        IMapper mapper = mapperConfiguration.CreateMapper();

        return mapper.Map<GameListInfoDTO[]>(games);
    }

    public async Task<ServerResponse> DeleteAsync(Guid id) // Done. 
    {
        Game? game = await unitOfWork.GameRepository.GetByIdAsync(id);

        if (game is null)
        {
            return new ServerResponse { Message = "Game is not found!", IsSuccess = false }; 
        }

        IQueryable<GameComment>? gameComments = await unitOfWork.GameCommentRepository.GetByGameIdAsync(id);

        if(gameComments is not null)
        {
            unitOfWork.GameCommentRepository.DeleteRange(await gameComments.ToArrayAsync());
        }

        await unitOfWork.GameRepository.DeleteAsync(id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = $"Game {game.Name} has been removed successfully!", IsSuccess = true };
    }

    public async Task<List<string>> GetAllImageDirectoriesByGameIdAsync(Guid gameId) // Done. 
    {
        Game? game = await unitOfWork.GameRepository.GetByIdAsync(gameId)
            ?? throw new NullReferenceException("Game is nullable!");

        List<string> directories = await unitOfWork.GameImageRepository.GetDirectoriesByGameIdAsync(gameId) ?? new List<string>();

        if (game.MainImageDirectory is not null)
        {
            directories.Add(game.MainImageDirectory);
        }

        return directories;
    }

    public async Task<ServerResponse> DeleteUserDataAsync(string userId)
    {
        GameComment[]? gameComments = await unitOfWork.GameCommentRepository.GetByUserIdAsync(userId);

        if(gameComments is not null && gameComments.Length > 0)
        {
            foreach (GameComment gameComment in gameComments)
            {
                await unitOfWork.GameCommentRepository.DeleteAsync(gameComment.Id);
            }
        }

        FavoriteGame[]? favoriteGames = await unitOfWork.FavoriteGameRepository.GetByUserIdAsync(userId);

        if(favoriteGames is not null && favoriteGames.Length > 0)
        {
            foreach(FavoriteGame game in favoriteGames)
            {
                await unitOfWork.FavoriteGameRepository.DeleteAsync(game.Id);
            }
        }

        LikedComment[]? likedComments = await unitOfWork.LikedCommentRepository.GetByUserIdAsync(userId);

        if(likedComments is not null && likedComments.Length > 0)
        {
            foreach(LikedComment likedComment in likedComments)
            {
                await unitOfWork.LikedCommentRepository.DeleteAsync(likedComment.Id);
            }
        }

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Operation has finished successfully!", IsSuccess = true };
    }
}