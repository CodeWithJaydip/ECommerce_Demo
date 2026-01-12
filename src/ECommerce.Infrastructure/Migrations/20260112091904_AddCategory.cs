using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Devices, gadgets, and accessories", true, "Electronics", null },
                    { 2, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Clothing, footwear, and accessories", true, "Fashion", null },
                    { 3, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Home essentials and kitchenware", true, "Home & Kitchen", null },
                    { 4, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Skincare, haircare, and personal care", true, "Beauty & Personal Care", null },
                    { 5, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Sports gear and outdoor equipment", true, "Sports & Outdoors", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
