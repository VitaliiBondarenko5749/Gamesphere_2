using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogOfGames.DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "gamecatalog");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Developers",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Developers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Publishers",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    MainImageDirectory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Views = table.Column<int>(type: "int", nullable: false),
                    PublisherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalSchema: "gamecatalog",
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteGames",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteGames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteGames_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "gamecatalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamesComments",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesComments_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "gamecatalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamesDevelopers",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeveloperId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesDevelopers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesDevelopers_Developers_DeveloperId",
                        column: x => x.DeveloperId,
                        principalSchema: "gamecatalog",
                        principalTable: "Developers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamesDevelopers_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "gamecatalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamesGategories",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesGategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesGategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "gamecatalog",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamesGategories_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "gamecatalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamesImages",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageDirectory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesImages_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "gamecatalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamesLanguages",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesLanguages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesLanguages_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "gamecatalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamesLanguages_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "gamecatalog",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamesPlatforms",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlatformId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesPlatforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesPlatforms_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "gamecatalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamesPlatforms_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalSchema: "gamecatalog",
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamesVideoLinks",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesVideoLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesVideoLinks_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "gamecatalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LikedComments",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikedComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikedComments_GamesComments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "gamecatalog",
                        principalTable: "GamesComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                schema: "gamecatalog",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Developers_Name",
                schema: "gamecatalog",
                table: "Developers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteGames_GameId",
                schema: "gamecatalog",
                table: "FavoriteGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Name",
                schema: "gamecatalog",
                table: "Games",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_PublisherId",
                schema: "gamecatalog",
                table: "Games",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesComments_GameId",
                schema: "gamecatalog",
                table: "GamesComments",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesDevelopers_DeveloperId",
                schema: "gamecatalog",
                table: "GamesDevelopers",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesDevelopers_GameId",
                schema: "gamecatalog",
                table: "GamesDevelopers",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesGategories_CategoryId",
                schema: "gamecatalog",
                table: "GamesGategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesGategories_GameId",
                schema: "gamecatalog",
                table: "GamesGategories",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesImages_GameId",
                schema: "gamecatalog",
                table: "GamesImages",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesLanguages_GameId",
                schema: "gamecatalog",
                table: "GamesLanguages",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesLanguages_LanguageId",
                schema: "gamecatalog",
                table: "GamesLanguages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesPlatforms_GameId",
                schema: "gamecatalog",
                table: "GamesPlatforms",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesPlatforms_PlatformId",
                schema: "gamecatalog",
                table: "GamesPlatforms",
                column: "PlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesVideoLinks_GameId",
                schema: "gamecatalog",
                table: "GamesVideoLinks",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Name",
                schema: "gamecatalog",
                table: "Languages",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LikedComments_CommentId",
                schema: "gamecatalog",
                table: "LikedComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_Name",
                schema: "gamecatalog",
                table: "Platforms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_Name",
                schema: "gamecatalog",
                table: "Publishers",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoriteGames",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "GamesDevelopers",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "GamesGategories",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "GamesImages",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "GamesLanguages",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "GamesPlatforms",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "GamesVideoLinks",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "LikedComments",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "Developers",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "Platforms",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "GamesComments",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "Publishers",
                schema: "gamecatalog");
        }
    }
}
