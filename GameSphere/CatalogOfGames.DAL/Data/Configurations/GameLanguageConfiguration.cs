using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogOfGames.DAL.Data.Configurations;

public class GameLanguageConfiguration : IEntityTypeConfiguration<GameLanguage>
{
    public void Configure(EntityTypeBuilder<GameLanguage> builder)
    {
        builder.HasKey(gl => gl.Id);

        builder.HasOne(gl => gl.Game)
            .WithMany(g => g.GameLanguages)
            .HasForeignKey(gl => gl.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gl => gl.Language)
            .WithMany(l => l.GameLanguages)
            .HasForeignKey(gl => gl.LanguageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("GamesLanguages");
    }
}