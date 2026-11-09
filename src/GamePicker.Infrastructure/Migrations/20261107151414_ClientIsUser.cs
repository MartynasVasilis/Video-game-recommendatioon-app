using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GamePicker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClientIsUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LibraryEntries_Tenants_TenantId",
                table: "LibraryEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_LibraryEntries_TenantId",
                table: "LibraryEntries");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "LibraryEntries");

            migrationBuilder.InsertData(
                table: "LibraryEntries",
                columns: new[] { "GameId", "UserId", "Status" },
                values: new object[] { 4, 1, "Played" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LibraryEntries",
                keyColumns: new[] { "GameId", "UserId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TenantId",
                table: "LibraryEntries",
                type: "text",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.UpdateData(
                table: "LibraryEntries",
                keyColumns: new[] { "GameId", "UserId" },
                keyValues: new object[] { 3, 2 },
                column: "TenantId",
                value: "alfa");

            migrationBuilder.UpdateData(
                table: "LibraryEntries",
                keyColumns: new[] { "GameId", "UserId" },
                keyValues: new object[] { 1, 3 },
                column: "TenantId",
                value: "beta");

            migrationBuilder.UpdateData(
                table: "LibraryEntries",
                keyColumns: new[] { "GameId", "UserId" },
                keyValues: new object[] { 7, 3 },
                column: "TenantId",
                value: "beta");

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { "alfa", "Žaidimų klubas Alfa" },
                    { "beta", "Žaidimų klubas Beta" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "TenantId",
                value: "alfa");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "TenantId",
                value: "alfa");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "TenantId",
                value: "beta");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_LibraryEntries_TenantId",
                table: "LibraryEntries",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_LibraryEntries_Tenants_TenantId",
                table: "LibraryEntries",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Tenants_TenantId",
                table: "Users",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
