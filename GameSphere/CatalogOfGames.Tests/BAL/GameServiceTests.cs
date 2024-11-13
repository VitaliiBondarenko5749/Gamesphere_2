using AutoMapper;
using CatalogOfGames.BAL.DTOs;
using CatalogOfGames.BAL.Services;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace CatalogOfGames.Tests.BAL;

public class GameServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IGameService _gameService;
    private readonly IMapper _mapper;

    public GameServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // Setup AutoMapper configuration
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Game, ShortGameInfoDTO>();
            cfg.CreateMap<Game, FullGameInfoDTO>();
            // Other mappings if necessary
        });
        _mapper = mapperConfig.CreateMapper();

        _gameService = new GameService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CheckNameExistenceAsync_GameExists_ShouldReturnGameNameTaken()
    {
        // Arrange
        var game = new Game { Id = Guid.NewGuid(), Name = "ExistingGame", MainImageDirectory = "img1" };
        var mockGameRepository = new Mock<IGameRepository>();
        mockGameRepository.Setup(repo => repo.GetByNameAsync(It.IsAny<string>())).ReturnsAsync(game);

        _unitOfWorkMock.Setup(uow => uow.GameRepository).Returns(mockGameRepository.Object);

        // Act
        var result = await _gameService.CheckNameExistenceAsync("ExistingGame");

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal("Game ExistingGame is already taken!", result.Message);
    }

    [Fact]
    public async Task CheckNameExistenceAsync_GameDoesNotExist_ShouldReturnSuccess()
    {
        // Arrange
        var mockGameRepository = new Mock<IGameRepository>();
        mockGameRepository.Setup(repo => repo.GetByNameAsync(It.IsAny<string>())).ReturnsAsync((Game?)null);

        _unitOfWorkMock.Setup(uow => uow.GameRepository).Returns(mockGameRepository.Object);

        // Act
        var result = await _gameService.CheckNameExistenceAsync("NonExistingGame");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(string.Empty, result.Message);
    }

    [Fact]
    public async Task GetTop10ByNameAsync_GamesExist_ShouldReturnMappedGames()
    {
        // Arrange
        var games = new List<Game>
    {
        new Game { Id = Guid.NewGuid(), Name = "Game1", MainImageDirectory = "img1" },
        new Game { Id = Guid.NewGuid(), Name = "Game2", MainImageDirectory = "img2" }
        // Add more games if necessary
    }.ToArray();

        var mockGameRepository = new Mock<IGameRepository>();
        mockGameRepository.Setup(repo => repo.GetTopTenByNameAsync(It.IsAny<string>())).ReturnsAsync(games);

        _unitOfWorkMock.Setup(uow => uow.GameRepository).Returns(mockGameRepository.Object);

        // Act
        var result = await _gameService.GetTop10ByNameAsync("Game");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(games.Length, result.Length);
        Assert.Equal(games[0].Name, result[0].Name);
        Assert.Equal(games[1].Name, result[1].Name);
    }

    [Fact]
    public async Task GetTop10ByNameAsync_NoGamesExist_ShouldReturnNull()
    {
        // Arrange
        var mockGameRepository = new Mock<IGameRepository>();
        mockGameRepository.Setup(repo => repo.GetTopTenByNameAsync(It.IsAny<string>())).ReturnsAsync((Game[]?)null);

        _unitOfWorkMock.Setup(uow => uow.GameRepository).Returns(mockGameRepository.Object);

        // Act
        var result = await _gameService.GetTop10ByNameAsync("NonExistingGame");

        // Assert
        Assert.Null(result);
    }
}