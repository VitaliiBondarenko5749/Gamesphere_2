using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogOfGames.DAL.Data.Configurations;

public class GameImageConfiguration : IEntityTypeConfiguration<GameImage>
{
    public void Configure(EntityTypeBuilder<GameImage> builder)
    {
        builder.HasKey(gi => gi.Id);

        builder.HasOne(gi => gi.Game)
            .WithMany(g => g.GameImages)
            .HasForeignKey(gi => gi.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(gi => gi.ImageDirectory)
            .IsRequired()
            .HasMaxLength(100);

        builder.ToTable("GamesImages");
    }
}