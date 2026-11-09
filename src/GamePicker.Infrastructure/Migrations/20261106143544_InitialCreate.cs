using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GamePicker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Platforms = table.Column<string[]>(type: "text[]", nullable: false),
                    Genre = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string[]>(type: "text[]", nullable: false),
                    Moods = table.Column<string[]>(type: "text[]", nullable: false),
                    DurationHours = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    ReleaseYear = table.Column<int>(type: "integer", nullable: false),
                    HasSingleplayer = table.Column<bool>(type: "boolean", nullable: false),
                    MaxPlayers = table.Column<int>(type: "integer", nullable: false),
                    Developer = table.Column<string>(type: "text", nullable: false),
                    Dimension = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LibraryEntries",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    GameId = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LibraryEntries", x => new { x.UserId, x.GameId });
                    table.ForeignKey(
                        name: "FK_LibraryEntries_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LibraryEntries_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LibraryEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Developer", "Dimension", "DurationHours", "Genre", "HasSingleplayer", "MaxPlayers", "Moods", "Platforms", "Price", "ReleaseYear", "Tags", "Title" },
                values: new object[,]
                {
                    { 1, "Šiaurės studija", "2D", 10, "Nuotykių", true, 1, new[] { "Atmosferiškas" }, new[] { "PC" }, 20m, 2021, new[] { "Roguelike" }, "G1" },
                    { 2, "Pixel Forge", "3D", 15, "Veiksmo", true, 1, new string[0], new[] { "PC" }, 25m, 2022, new[] { "Roguelike" }, "G2" },
                    { 3, "Šiaurės studija", "2D", 8, "Nuotykių", true, 1, new[] { "Atmosferiškas" }, new[] { "PC" }, 15m, 2020, new string[0], "G3" },
                    { 4, "Pixel Forge", "3D", 40, "RPG", true, 1, new string[0], new[] { "PC" }, 40m, 2023, new[] { "Roguelike" }, "G4" },
                    { 5, "Pixel Forge", "3D", 12, "Veiksmo", true, 1, new[] { "Atmosferiškas" }, new[] { "PS5" }, 50m, 2024, new[] { "Roguelike" }, "G5" },
                    { 6, "Šiaurės studija", "2D", 12, "Galvosūkių", true, 1, new string[0], new[] { "PC" }, 10m, 2019, new string[0], "G6" },
                    { 7, "Pixel Forge", "3D", 9, "Veiksmo", true, 1, new string[0], new[] { "PC" }, 30m, 2022, new[] { "Roguelike" }, "G7" }
                });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "alfa", "Žaidimų klubas Alfa" },
                    { "beta", "Žaidimų klubas Beta" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Name", "TenantId" },
                values: new object[,]
                {
                    { 1, "Aistė", "alfa" },
                    { 2, "Mantas", "alfa" },
                    { 3, "Rokas", "beta" }
                });

            migrationBuilder.InsertData(
                table: "LibraryEntries",
                columns: new[] { "GameId", "UserId", "Status", "TenantId" },
                values: new object[,]
                {
                    { 3, 2, "NotInterested", "alfa" },
                    { 1, 3, "Played", "beta" },
                    { 7, 3, "Owned", "beta" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LibraryEntries_GameId",
                table: "LibraryEntries",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryEntries_TenantId",
                table: "LibraryEntries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LibraryEntries");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
