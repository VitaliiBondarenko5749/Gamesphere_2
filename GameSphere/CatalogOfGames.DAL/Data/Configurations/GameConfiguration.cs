using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogOfGames.DAL.Data.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(70);

        builder.HasIndex(g => g.Name)
            .IsUnique();

        builder.Property(g => g.ReleaseDate)
            .IsRequired();

        builder.Property(g => g.Description)
            .IsRequired()
            .HasMaxLength(10000);

        builder.Property(g => g.MainImageDirectory)
            .HasMaxLength(100);

        builder.HasOne(g => g.Publisher)
            .WithMany(p => p.Games)
            .HasForeignKey(g => g.PublisherId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Games");
    }
}