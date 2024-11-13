using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogOfGames.DAL.Migrations
{
    public partial class StoreRatingsCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameRatings",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameRatings_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "gamecatalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stores",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IconDirectory = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GamesStores",
                schema: "gamecatalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StoreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesStores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesStores_Games_GameId",
                        column: x => x.GameId,
                        principalSchema: "gamecatalog",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamesStores_Stores_StoreId",
                        column: x => x.StoreId,
                        principalSchema: "gamecatalog",
                        principalTable: "Stores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameRatings_GameId",
                schema: "gamecatalog",
                table: "GameRatings",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesStores_GameId",
                schema: "gamecatalog",
                table: "GamesStores",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesStores_StoreId",
                schema: "gamecatalog",
                table: "GamesStores",
                column: "StoreId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameRatings",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "GamesStores",
                schema: "gamecatalog");

            migrationBuilder.DropTable(
                name: "Stores",
                schema: "gamecatalog");
        }
    }
}
