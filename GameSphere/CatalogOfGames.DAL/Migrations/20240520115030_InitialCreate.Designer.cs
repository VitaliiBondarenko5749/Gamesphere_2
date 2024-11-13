﻿// <auto-generated />
using System;
using CatalogOfGames.DAL.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CatalogOfGames.DAL.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240520115030_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("gamecatalog")
                .HasAnnotation("ProductVersion", "6.0.28")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Category", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Categories", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Developer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Developers", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.FavoriteGame", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("FavoriteGames", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(10000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MainImageDirectory")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.Property<Guid>("PublisherId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ReleaseDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Views")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("PublisherId");

                    b.ToTable("Games", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CategoryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("GameId");

                    b.ToTable("GamesGategories", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GamesComments", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameDeveloper", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DeveloperId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("DeveloperId");

                    b.HasIndex("GameId");

                    b.ToTable("GamesDevelopers", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameImage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ImageDirectory")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GamesImages", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameLanguage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("LanguageId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("LanguageId");

                    b.ToTable("GamesLanguages", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GamePlatform", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("PlatformId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.HasIndex("PlatformId");

                    b.ToTable("GamesPlatforms", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameVideoLink", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("GameId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GamesVideoLinks", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Language", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Languages", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.LikedComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CommentId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CommentId");

                    b.ToTable("LikedComments", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Platform", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Platforms", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Publisher", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("nvarchar(70)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Publishers", "gamecatalog");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.FavoriteGame", b =>
                {
                    b.HasOne("CatalogOfGames.DAL.Entities.Game", "Game")
                        .WithMany("FavoriteGames")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Game", b =>
                {
                    b.HasOne("CatalogOfGames.DAL.Entities.Publisher", "Publisher")
                        .WithMany("Games")
                        .HasForeignKey("PublisherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Publisher");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameCategory", b =>
                {
                    b.HasOne("CatalogOfGames.DAL.Entities.Category", "Category")
                        .WithMany("GameCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CatalogOfGames.DAL.Entities.Game", "Game")
                        .WithMany("GameCategories")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Game");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameComment", b =>
                {
                    b.HasOne("CatalogOfGames.DAL.Entities.Game", "Game")
                        .WithMany("GameComments")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameDeveloper", b =>
                {
                    b.HasOne("CatalogOfGames.DAL.Entities.Developer", "Developer")
                        .WithMany("GameDevelopers")
                        .HasForeignKey("DeveloperId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CatalogOfGames.DAL.Entities.Game", "Game")
                        .WithMany("GameDevelopers")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Developer");

                    b.Navigation("Game");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameImage", b =>
                {
                    b.HasOne("CatalogOfGames.DAL.Entities.Game", "Game")
                        .WithMany("GameImages")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameLanguage", b =>
                {
                    b.HasOne("CatalogOfGames.DAL.Entities.Game", "Game")
                        .WithMany("GameLanguages")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CatalogOfGames.DAL.Entities.Language", "Language")
                        .WithMany("GameLanguages")
                        .HasForeignKey("LanguageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Language");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GamePlatform", b =>
                {
                    b.HasOne("CatalogOfGames.DAL.Entities.Game", "Game")
                        .WithMany("GamePlatforms")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CatalogOfGames.DAL.Entities.Platform", "Platform")
                        .WithMany("GamePlatforms")
                        .HasForeignKey("PlatformId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");

                    b.Navigation("Platform");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameVideoLink", b =>
                {
                    b.HasOne("CatalogOfGames.DAL.Entities.Game", "Game")
                        .WithMany("GameVideoLinks")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.LikedComment", b =>
                {
                    b.HasOne("CatalogOfGames.DAL.Entities.GameComment", "Comment")
                        .WithMany("LikedComments")
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Comment");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Category", b =>
                {
                    b.Navigation("GameCategories");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Developer", b =>
                {
                    b.Navigation("GameDevelopers");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Game", b =>
                {
                    b.Navigation("FavoriteGames");

                    b.Navigation("GameCategories");

                    b.Navigation("GameComments");

                    b.Navigation("GameDevelopers");

                    b.Navigation("GameImages");

                    b.Navigation("GameLanguages");

                    b.Navigation("GamePlatforms");

                    b.Navigation("GameVideoLinks");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.GameComment", b =>
                {
                    b.Navigation("LikedComments");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Language", b =>
                {
                    b.Navigation("GameLanguages");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Platform", b =>
                {
                    b.Navigation("GamePlatforms");
                });

            modelBuilder.Entity("CatalogOfGames.DAL.Entities.Publisher", b =>
                {
                    b.Navigation("Games");
                });
#pragma warning restore 612, 618
        }
    }
}
