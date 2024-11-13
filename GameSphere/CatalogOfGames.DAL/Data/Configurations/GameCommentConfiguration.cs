using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogOfGames.DAL.Data.Configurations;

public class GameCommentConfiguration : IEntityTypeConfiguration<GameComment>
{
    public void Configure(EntityTypeBuilder<GameComment> builder)
    {
        builder.HasKey(gc => gc.Id);

        builder.Property(gc => gc.UserId)
            .IsRequired();

        builder.Property(gc => gc.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(gc => gc.CreatedAt)
            .IsRequired();

        builder.HasOne(gc => gc.Game)
            .WithMany(g => g.GameComments)
            .HasForeignKey(gc => gc.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("GamesComments");
    }
}