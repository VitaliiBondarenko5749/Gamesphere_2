using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogOfGames.DAL.Data.Configurations;

public class GameStoreConfiguration : IEntityTypeConfiguration<GameStore>
{
    public void Configure(EntityTypeBuilder<GameStore> builder)
    {
        builder.HasKey(gs => gs.Id);

        builder.HasOne(gs => gs.Store)
            .WithMany(s => s.GameStores)
            .HasForeignKey(gs => gs.StoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gs => gs.Game)
            .WithMany(g => g.GameStores)
            .HasForeignKey(gs => gs.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(gs => gs.Link)
            .IsRequired();

        builder.ToTable("GamesStores");
    }
}