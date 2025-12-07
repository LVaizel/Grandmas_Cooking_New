using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grandmas_Cooking_API.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Recipe",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Recipe",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Recipe");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Recipe");
        }
    }
}
