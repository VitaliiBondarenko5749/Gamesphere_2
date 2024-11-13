using AutoMapper;
using CatalogOfGames.BAL.DTOs;
using Helpers;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;


namespace CatalogOfGames.BAL.Services;

public interface ICategoryService
{
    Task<CategoryInfoDTO[]?> GetAllAsync();
    Task<ServerResponse> AddAsync(AddCategoryDTO categoryInsertDTO);
    Task<ServerResponse> DeleteAsync(Guid categoryId);
    Task<CategoryInfoDTO[]?> GetTopTenByNameAsync(string name);
    Task<ServerResponse> AddToGameAsync(CategoryToGameDTO dto);
    Task<ServerResponse> DeleteFromGameAsync(CategoryToGameDTO dto);
}

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<CategoryInfoDTO[]?> GetAllAsync() // Done. 
    {
        Category[]? categories = await unitOfWork.CategoryRepository.GetAllAsync();

        if (categories is null)
        {
            return null;
        }

        MapperConfiguration mapperConfiguration = new(config =>
        {
            config.CreateMap<Category, CategoryInfoDTO>();
        });

        IMapper mapper = mapperConfiguration.CreateMapper();

        CategoryInfoDTO[] categoryDTOs = mapper.Map<CategoryInfoDTO[]>(categories);

        return categoryDTOs;
    }

    public async Task<ServerResponse> AddAsync(AddCategoryDTO categoryInsertDTO) // Done. 
    {
        Category? category = await unitOfWork.CategoryRepository.GetByNameAsync(categoryInsertDTO.Name);

        if (category is not null || categoryInsertDTO.Name.ToLower().Equals("all games") || categoryInsertDTO.Name.ToLower().Equals("favorite games"))
        {
            return new ServerResponse { Message = $"Category {categoryInsertDTO.Name} is already exist!", IsSuccess = false };
        }

        category = new()
        {
            Id = Guid.NewGuid(),
            Name = categoryInsertDTO.Name
        };

        await unitOfWork.CategoryRepository.CreateAsync(category);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"Category {category.Name} has been added!",
            IsSuccess = true
        };
    }

    public async Task<ServerResponse> DeleteAsync(Guid categoryId) // Done. 
    {
        Category? category = await unitOfWork.CategoryRepository.GetByIdAsync(categoryId);

        if (category is null)
        {
            return new ServerResponse
            {
                Message = "Category was not found!",
                IsSuccess = false
            };
        }

        await unitOfWork.CategoryRepository.DeleteAsync(category.Id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse
        {
            Message = $"Category {category.Name} has been removed successfully!",
            IsSuccess = true
        };
    }

    public async Task<CategoryInfoDTO[]?> GetTopTenByNameAsync(string name) // Done. 
    {
        Category[]? categories = await unitOfWork.CategoryRepository.GetTopTenByNameAsync(name);

        if (categories is null)
        {
            return null;
        }

        MapperConfiguration mapperConfiguration = new(config =>
        {
            config.CreateMap<Category, CategoryInfoDTO>();
        });

        IMapper mapper = mapperConfiguration.CreateMapper();

        CategoryInfoDTO[] categoryDTOs = mapper.Map<CategoryInfoDTO[]>(categories);

        return categoryDTOs;
    }

    public async Task<ServerResponse> AddToGameAsync(CategoryToGameDTO dto) // Done. 
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(dto.GameName);
        Category? category = await unitOfWork.CategoryRepository.GetByIdAsync(dto.CategoryId);

        if (game is null || category is null)
        {
            return new ServerResponse { Message = "Invalid data", IsSuccess = false };
        }

        GameCategory? gameCategory = await unitOfWork.GameCategoryRepository.CheckExistenceAsync(game.Id, dto.CategoryId);

        if (gameCategory is not null)
        {
            return new ServerResponse { Message = "Data already exists!", IsSuccess = false };
        }

        gameCategory = new()
        {
            Id = Guid.NewGuid(),
            GameId = game.Id,
            CategoryId = category.Id
        };

        await unitOfWork.GameCategoryRepository.CreateAsync(gameCategory);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = $"Category {category.Name} has been added to game {game.Name}!", IsSuccess = true };
    }

    public async Task<ServerResponse> DeleteFromGameAsync(CategoryToGameDTO dto)
    {
        Game? game = await unitOfWork.GameRepository.GetByNameAsync(dto.GameName);
        Category? category = await unitOfWork.CategoryRepository.GetByIdAsync(dto.CategoryId);

        if (game is null || category is null)
        {
            return new ServerResponse { Message = "Invalid data", IsSuccess = false };
        }

        GameCategory? gameCategory = await unitOfWork.GameCategoryRepository.CheckExistenceAsync(game.Id, dto.CategoryId);

        if (gameCategory is null)
        {
            return new ServerResponse { Message = "Data does not exist!", IsSuccess = false };
        }

        await unitOfWork.GameCategoryRepository.DeleteAsync(gameCategory.Id);

        await unitOfWork.SaveChangesAsync();

        return new ServerResponse { Message = "Data has been removed!", IsSuccess = true };
    }
}