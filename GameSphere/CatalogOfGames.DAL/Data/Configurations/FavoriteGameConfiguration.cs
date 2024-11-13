using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogOfGames.DAL.Data.Configurations;

public class FavoriteGameConfiguration : IEntityTypeConfiguration<FavoriteGame>
{
    public void Configure(EntityTypeBuilder<FavoriteGame> builder)
    {
        builder.HasKey(fg => fg.Id);

        builder.HasOne(fg => fg.Game)
            .WithMany(g => g.FavoriteGames)
            .HasForeignKey(fg => fg.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(fg => fg.UserId)
            .IsRequired();

        builder.ToTable("FavoriteGames");
    }
}