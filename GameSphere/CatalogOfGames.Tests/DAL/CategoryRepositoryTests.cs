using CatalogOfGames.DAL.Data;
using CatalogOfGames.DAL.Entities;
using CatalogOfGames.DAL.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CatalogOfGames.Tests.DAL;

public class CategoryRepositoryTests
{
    private readonly CategoryRepository categoryRepository;
    private readonly ApplicationDbContext dbContext;

    public CategoryRepositoryTests()
    {
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        dbContext = new ApplicationDbContext(options);
        categoryRepository = new CategoryRepository(dbContext);
    }

    [Fact]
    public async Task GetAllAsync_WhenCalled_ReturnsAllCategories()
    {
        // Arrange
        List<Category> categories = new()
        {
            new Category { Id = Guid.NewGuid(), Name = "Category1" },
            new Category { Id = Guid.NewGuid(), Name = "Category2" }
        };

        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        // Act
        Category[]? result = await categoryRepository.GetAllAsync();

        // Assert
        result.Should().NotBeNull().And.HaveCount(categories.Count);
        result.Should().BeEquivalentTo(categories);
    }

    [Fact]
    public async Task GetByNameAsync_WhenCalledWithValidName_ReturnsCategory()
    {
        // Arrange
        string name = "Category1";
        List<Category> categories = new()
        {
            new Category { Id = Guid.NewGuid(), Name = "Category1" },
            new Category { Id = Guid.NewGuid(), Name = "Category2" }
        };

        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        // Act
        Category? result = await categoryRepository.GetByNameAsync(name);

        // Assert
        result.Should().NotBeNull();
        result?.Name.Should().Be(name);
    }

    [Fact]
    public async Task GetTopTenByNameAsync_WhenCalled_ReturnsTopTenCategories()
    {
        // Arrange
        string name = "Category";
        List<Category> categories = Enumerable.Range(1, 15).Select(i => new Category
        {
            Id = Guid.NewGuid(),
            Name = $"Category{i}"
        }).ToList();

        await dbContext.Categories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();

        // Act
        Category[]? result = await categoryRepository.GetTopTenByNameAsync(name);

        // Assert
        result.Should().NotBeNull().And.HaveCount(10);
        result?.All(c => c.Name.Contains(name)).Should().BeTrue();
    }
}