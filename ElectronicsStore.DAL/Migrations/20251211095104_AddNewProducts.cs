using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ElectronicsStore.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2025, 12, 11, 9, 51, 4, 200, DateTimeKind.Utc).AddTicks(2496), "Мощный смартфон от Apple" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "ImagePath", "Name", "Price", "UpdatedAt" },
                values: new object[,]
                {
                    { 2, 1, new DateTime(2025, 12, 11, 9, 51, 4, 200, DateTimeKind.Utc).AddTicks(2499), "Флагман на Android с отличным экраном", "/img/samsung_s24.jpg", "Samsung Galaxy S24", 95000m, null },
                    { 3, 1, new DateTime(2025, 12, 11, 9, 51, 4, 200, DateTimeKind.Utc).AddTicks(2501), "Смартфон с лучшей камерой и чистым Android", "/img/pixel8.jpg", "Google Pixel 8", 75000m, null },
                    { 4, 2, new DateTime(2025, 12, 11, 9, 51, 4, 200, DateTimeKind.Utc).AddTicks(2502), "Легкий, тонкий и производительный ультрабук", "/img/mack2.jpg", "MacBook Air M2", 120000m, null },
                    { 5, 2, new DateTime(2025, 12, 11, 9, 51, 4, 200, DateTimeKind.Utc).AddTicks(2504), "Мощный игровой ноутбук для современных игр", "/img/asus_rog.jpg", "ASUS ROG Strix", 150000m, null },
                    { 6, 2, new DateTime(2025, 12, 11, 9, 51, 4, 200, DateTimeKind.Utc).AddTicks(2505), "Надежный ноутбук для бизнеса и работы", "/img/lenovo_thinkpad.jpg", "Lenovo ThinkPad X1", 180000m, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Мощный смартфон" });
        }
    }
}
