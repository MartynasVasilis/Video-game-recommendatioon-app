using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GamePicker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExpandSampleCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Developer", "Dimension", "DurationHours", "Genre", "HasSingleplayer", "MaxPlayers", "Moods", "Platforms", "Price", "ReleaseYear", "Tags", "Title" },
                values: new object[,]
                {
                    { 8, "Baltijos žaidimai", "3D", 60, "Strategija", true, 4, new[] { "Įtemptas" }, new[] { "PC" }, 35m, 2024, new[] { "Atviras pasaulis" }, "G8" },
                    { 9, "Jaukūs žaidimai", "2D", 25, "Simuliacija", true, 1, new[] { "Jaukus" }, new[] { "Switch" }, 20m, 2023, new string[0], "G9" },
                    { 10, "Pixel Forge", "3D", 6, "Veiksmo", false, 4, new[] { "Įtemptas" }, new[] { "PC", "PS5", "Xbox" }, 18m, 2023, new[] { "Kooperatinis" }, "G10" },
                    { 11, "Jaukūs žaidimai", "2D", 5, "Simuliacija", false, 4, new[] { "Jaukus" }, new[] { "PC", "Switch" }, 12m, 2022, new[] { "Kooperatinis" }, "G11" },
                    { 12, "Baltijos žaidimai", "3D", 8, "Lenktynių", false, 8, new[] { "Įtemptas" }, new[] { "PC", "PS5", "Xbox" }, 30m, 2024, new string[0], "G12" },
                    { 13, "Šiaurės studija", "2D", 18, "Veiksmo", true, 1, new[] { "Atmosferiškas" }, new[] { "PS5", "Xbox" }, 35m, 2021, new[] { "Metroidvania" }, "G13" },
                    { 14, "Šiaurės studija", "3D", 80, "RPG", true, 1, new[] { "Atmosferiškas" }, new[] { "PC" }, 60m, 2024, new[] { "Atviras pasaulis" }, "G14" },
                    { 15, "Jaukūs žaidimai", "2D", 6, "Galvosūkių", true, 2, new[] { "Jaukus" }, new[] { "PS5", "Switch" }, 8m, 2020, new string[0], "G15" },
                    { 16, "Baltijos žaidimai", "3D", 25, "Nuotykių", true, 4, new[] { "Atmosferiškas" }, new[] { "PC", "Xbox" }, 28m, 2023, new[] { "Kooperatinis", "Atviras pasaulis" }, "G16" },
                    { 17, "Pixel Forge", "3D", 4, "Veiksmo", false, 10, new[] { "Įtemptas" }, new[] { "PC" }, 0m, 2025, new string[0], "G17" },
                    { 18, "Šiaurės studija", "3D", 7, "Siaubo", true, 1, new[] { "Įtemptas", "Atmosferiškas" }, new[] { "Xbox" }, 22m, 2018, new string[0], "G18" },
                    { 19, "Baltijos žaidimai", "2D", 30, "Strategija", true, 2, new[] { "Įtemptas" }, new[] { "PC", "Switch" }, 16m, 2017, new string[0], "G19" },
                    { 20, "Pixel Forge", "2D", 14, "Veiksmo", true, 1, new[] { "Įtemptas" }, new[] { "Switch" }, 25m, 2025, new[] { "Roguelike", "Metroidvania" }, "G20" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: 20);
        }
    }
}
