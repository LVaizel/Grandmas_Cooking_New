using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Grandmas_Cooking_API.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Steps",
                table: "Recipe");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Steps",
                table: "Recipe",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
