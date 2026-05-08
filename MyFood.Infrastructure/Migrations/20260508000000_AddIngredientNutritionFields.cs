using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientNutritionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Carbs",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "CaloriesPerUnit",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Fat",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Protein",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Ingredients",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Carbs",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "CaloriesPerUnit",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Fat",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Protein",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Ingredients");
        }
    }
}
