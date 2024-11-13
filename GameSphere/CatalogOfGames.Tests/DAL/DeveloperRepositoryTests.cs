using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CatalogOfGames.Tests.DAL;

public class DeveloperRepositoryTests
{
    private readonly DeveloperRepository developerRepository;
    private readonly ApplicationDbContext dbContext;

    public DeveloperRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        dbContext = new ApplicationDbContext(options);
        developerRepository = new DeveloperRepository(dbContext);
    }

    [Fact]
    public async Task GetByNameAsync_WhenCalledWithValidName_ReturnsDeveloper()
    {
        // Arrange
        string name = "Developer1";
        Developer[]? developers = new[]
        {
            new Developer { Id = Guid.NewGuid(), Name = "Developer1" },
            new Developer { Id = Guid.NewGuid(), Name = "Developer2" }
        };

        await dbContext.Developers.AddRangeAsync(developers);
        await dbContext.SaveChangesAsync();

        // Act
        Developer? result = await developerRepository.GetByNameAsync(name);

        // Assert
        result.Should().NotBeNull();
        result?.Name.Should().Be(name);
    }

    [Fact]
    public async Task GetByNameAsync_WhenCalledWithInvalidName_ReturnsNull()
    {
        // Arrange
        string name = "NonExistentDeveloper";
        Developer[]? developers = new[]
        {
            new Developer { Id = Guid.NewGuid(), Name = "Developer1" },
            new Developer { Id = Guid.NewGuid(), Name = "Developer2" }
        };

        await dbContext.Developers.AddRangeAsync(developers);
        await dbContext.SaveChangesAsync();

        // Act
        Developer? result = await developerRepository.GetByNameAsync(name);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTopTenByNameAsync_WhenCalled_ReturnsTopTenDevelopers()
    {
        // Arrange
        string name = "Developer";
        List<Developer> developers = Enumerable.Range(1, 15).Select(i => new Developer
        {
            Id = Guid.NewGuid(),
            Name = $"Developer{i}"
        }).ToList();

        await dbContext.Developers.AddRangeAsync(developers);
        await dbContext.SaveChangesAsync();

        // Act
        Developer[]? result = await developerRepository.GetTopTenByNameAsync(name);

        // Assert
        result.Should().NotBeNull().And.HaveCount(10);
        result?.All(d => d.Name.Contains(name)).Should().BeTrue();
    }

    [Fact]
    public async Task GetTopTenByNameAsync_WhenCalledWithNoMatchingName_ReturnsEmptyArray()
    {
        // Arrange
        string name = "NonExistent";
        List<Developer> developers = Enumerable.Range(1, 15).Select(i => new Developer
        {
            Id = Guid.NewGuid(),
            Name = $"Developer{i}"
        }).ToList();

        await dbContext.Developers.AddRangeAsync(developers);
        await dbContext.SaveChangesAsync();

        // Act
        Developer[]? result = await developerRepository.GetTopTenByNameAsync(name);

        // Assert
        result.Should().NotBeNull().And.BeEmpty();
    }
}