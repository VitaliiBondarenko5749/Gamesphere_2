using CatalogOfGames.API.Controllers;
using CatalogOfGames.BAL.DTOs;
using CatalogOfGames.BAL.Helpers;
using CatalogOfGames.BAL.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

#pragma warning disable

namespace CatalogOfGames.Tests.API;

public class GameControllerTests
{
    private readonly Mock<IGameService> _mockGameService;
    private readonly Mock<ILogger<GameController>> _mockLogger;
    private readonly GameController _controller;

    public GameControllerTests()
    {
        _mockGameService = new Mock<IGameService>();
        _mockLogger = new Mock<ILogger<GameController>>();
        _controller = new GameController(_mockLogger.Object, _mockGameService.Object, null, null);
    }

    [Fact]
    public async Task CheckNameExistenceAsync_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockGameService.Setup(service => service.CheckNameExistenceAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.CheckNameExistenceAsync("Game");

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult.StatusCode.Should().Be(501);
        objectResult.Value.Should().Be("INTERNAL SERVER ERROR...");
    }

    [Fact]
    public async Task GetTopTenByNameAsync_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockGameService.Setup(service => service.GetTop10ByNameAsync(It.IsAny<string>())).ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetTopTenByNameAsync("Game");

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult.StatusCode.Should().Be(501);
        objectResult.Value.Should().Be("Internal Server Error");
    }
}