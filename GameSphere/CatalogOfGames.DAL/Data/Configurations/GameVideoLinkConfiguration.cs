using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogOfGames.DAL.Data.Configurations;

public class GameVideoLinkConfiguration : IEntityTypeConfiguration<GameVideoLink>
{
    public void Configure(EntityTypeBuilder<GameVideoLink> builder)
    {
        builder.HasKey(gvl => gvl.Id);

        builder.HasOne(gvl => gvl.Game)
            .WithMany(g => g.GameVideoLinks)
            .HasForeignKey(gvl => gvl.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(gvl => gvl.Link)
            .IsRequired()
            .HasMaxLength(100);

        builder.ToTable("GamesVideoLinks");
    }
}