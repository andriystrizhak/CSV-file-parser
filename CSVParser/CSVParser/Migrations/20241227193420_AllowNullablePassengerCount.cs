using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSVParser.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullablePassengerCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Спочатку видаляємо існуючий індекс
            migrationBuilder.DropIndex(
                name: "IX_Trips_Unique",
                table: "Trips");

            // Змінюємо тип колонки на nullable
            migrationBuilder.AlterColumn<int>(
                name: "PassengerCount",
                table: "Trips",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Створюємо новий індекс, який дозволяє null значення
            migrationBuilder.CreateIndex(
                name: "IX_Trips_Unique",
                table: "Trips",
                columns: new[] { "PickupDatetime", "DropoffDatetime", "PassengerCount" },
                unique: true,
                filter: "[PassengerCount] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Видаляємо новий індекс
            migrationBuilder.DropIndex(
                name: "IX_Trips_Unique",
                table: "Trips");

            // Змінюємо тип колонки назад на not null
            migrationBuilder.AlterColumn<int>(
                name: "PassengerCount",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Відновлюємо оригінальний індекс
            migrationBuilder.CreateIndex(
                name: "IX_Trips_Unique",
                table: "Trips",
                columns: new[] { "PickupDatetime", "DropoffDatetime", "PassengerCount" },
                unique: true);
        }
    }
}
