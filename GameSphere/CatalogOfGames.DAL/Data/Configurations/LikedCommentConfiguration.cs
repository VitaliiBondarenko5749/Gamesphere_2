using CatalogOfGames.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogOfGames.DAL.Data.Configurations;

public class LikedCommentConfiguration : IEntityTypeConfiguration<LikedComment>
{
    public void Configure(EntityTypeBuilder<LikedComment> builder)
    {
        builder.HasKey(lc => lc.Id);

        builder.HasOne(lc => lc.Comment)
            .WithMany(gc => gc.LikedComments)
            .HasForeignKey(lc => lc.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(lc => lc.UserId)
            .IsRequired();

        builder.ToTable("LikedComments");
    }
}