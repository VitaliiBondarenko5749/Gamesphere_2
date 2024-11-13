using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CatalogOfGames.Tests.DAL;

public class GameRepositoryTests
{
    private readonly GameRepository gameRepository;
    private readonly ApplicationDbContext dbContext;

    public GameRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        dbContext = new ApplicationDbContext(options);
        gameRepository = new GameRepository(dbContext);
    }

    [Fact]
    public async Task GetAllByCategoryNameAsync_WhenCalledWithValidCategory_ReturnsGames()
    {
        // Arrange
        var category = new Category { Id = Guid.NewGuid(), Name = "Action" };
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Game1",
            Description = "Test",
            GameCategories = new List<GameCategory> { new GameCategory { Category = category } }
        };
        await dbContext.Games.AddAsync(game);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await gameRepository.GetAllByCategoryNameAsync("Action");

        // Assert
        result.Should().NotBeNull();
        result?.Count().Should().Be(1);
        result?.First().Name.Should().Be("Game1");
    }

    [Fact]
    public async Task GetFavoriteByUserIdAsync_WhenCalledWithValidUserId_ReturnsGames()
    {
        // Arrange
        var userId = "user1";
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Game1",
            Description = "Test",
            FavoriteGames = new List<FavoriteGame> { new FavoriteGame { UserId = userId } }
        };
        await dbContext.Games.AddAsync(game);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await gameRepository.GetFavoriteByUserIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result?.Count().Should().Be(1);
        result?.First().Name.Should().Be("Game1");
    }

    [Fact]
    public async Task GetFullGameDataByIdAsync_WhenCalledWithValidId_ReturnsFullGameData()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var game = new Game
        {
            Id = gameId,
            Name = "Game1",
            Description = "Test",
            Publisher = new Publisher { Id = Guid.NewGuid(), Name = "Publisher1" },
            GameCategories = new List<GameCategory> { new GameCategory { Category = new Category { Id = Guid.NewGuid(), Name = "Action" } } },
            GameDevelopers = new List<GameDeveloper> { new GameDeveloper { Developer = new Developer { Id = Guid.NewGuid(), Name = "Dev1" } } },
            GameLanguages = new List<GameLanguage> { new GameLanguage { Language = new Language { Id = Guid.NewGuid(), Name = "English" } } },
            GamePlatforms = new List<GamePlatform> { new GamePlatform { Platform = new Platform { Id = Guid.NewGuid(), Name = "PC" } } },
            GameImages = new List<GameImage> { new GameImage { Id = Guid.NewGuid(), ImageDirectory = "images/game1" } },
            GameVideoLinks = new List<GameVideoLink> { new GameVideoLink { Id = Guid.NewGuid(), Link = "http://video.com/game1" } }
        };
        await dbContext.Games.AddAsync(game);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await gameRepository.GetFullGameDataByIdAsync(gameId);

        // Assert
        result.Should().NotBeNull();
        result?.Name.Should().Be("Game1");
        result?.Publisher.Name.Should().Be("Publisher1");
        result?.GameCategories.Should().HaveCount(1);
        result?.GameDevelopers.Should().HaveCount(1);
        result?.GameLanguages.Should().HaveCount(1);
        result?.GamePlatforms.Should().HaveCount(1);
        result?.GameImages.Should().HaveCount(1);
        result?.GameVideoLinks.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetImageDirectoryNamesByGameIdAsync_WhenCalledWithValidId_ReturnsImageDirectories()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var game = new Game
        {
            Id = gameId,
            Name = "Game1",
            Description = "Test",
            GameImages = new List<GameImage>
            {
                new GameImage { Id = Guid.NewGuid(), ImageDirectory = "images/game1" },
                new GameImage { Id = Guid.NewGuid(), ImageDirectory = "images/game1/2" }
            }
        };
        await dbContext.Games.AddAsync(game);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await gameRepository.GetImageDirectoryNamesByGameIdAsync(gameId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain("images/game1");
        result.Should().Contain("images/game1/2");
    }

    [Fact]
    public async Task GetGamesByCategoriesAsync_WhenCalledWithValidCategories_ReturnsGames()
    {
        // Arrange
        var category1 = new Category { Id = Guid.NewGuid(), Name = "Action" };
        var category2 = new Category { Id = Guid.NewGuid(), Name = "Adventure" };
        var game1 = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Game1",
            Description = "Test",
            GameCategories = new List<GameCategory> { new GameCategory { Category = category1 } }
        };
        var game2 = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Game2",
            Description = "Test",
            GameCategories = new List<GameCategory> { new GameCategory { Category = category2 } }
        };
        await dbContext.Games.AddRangeAsync(game1, game2);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await gameRepository.GetGamesByCategoriesAsync(new List<string> { "Action", "Adventure" });

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(g => g.Name == "Game1");
        result.Should().Contain(g => g.Name == "Game2");
    }
}