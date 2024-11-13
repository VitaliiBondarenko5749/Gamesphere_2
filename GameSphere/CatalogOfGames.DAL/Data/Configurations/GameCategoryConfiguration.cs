using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogOfGames.DAL.Data.Configurations;

public class GameCategoryConfiguration : IEntityTypeConfiguration<GameCategory>
{
    public void Configure(EntityTypeBuilder<GameCategory> builder)
    {
        builder.HasKey(gc => gc.Id);

        builder.HasOne(gc => gc.Game)
            .WithMany(g => g.GameCategories)
            .HasForeignKey(gc => gc.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gc => gc.Category)
            .WithMany(c => c.GameCategories)
            .HasForeignKey(gc => gc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("GamesGategories");
    }
}