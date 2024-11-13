using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogOfGames.DAL.Data.Configurations;

public class GameRatingConfiguration : IEntityTypeConfiguration<GameRating>
{
    public void Configure(EntityTypeBuilder<GameRating> builder)
    {
        builder.HasKey(gr => gr.Id);

        builder.HasOne(gr => gr.Game)
            .WithMany(g => g.GameRatings)
            .HasForeignKey(gr => gr.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(gr => gr.UserId)
            .IsRequired();

        builder.Property(gr => gr.Rating)
            .IsRequired();

        builder.ToTable("GameRatings");
    }
}