using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogOfGames.DAL.Data.Configurations;

public class GameDeveloperConfiguration : IEntityTypeConfiguration<GameDeveloper>
{
    public void Configure(EntityTypeBuilder<GameDeveloper> builder)
    {
        builder.HasKey(gd => gd.Id);

        builder.HasOne(gd => gd.Game)
            .WithMany(g => g.GameDevelopers)
            .HasForeignKey(gd => gd.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gd => gd.Developer)
            .WithMany(d => d.GameDevelopers)
            .HasForeignKey(gd => gd.DeveloperId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("GamesDevelopers");
    }
}