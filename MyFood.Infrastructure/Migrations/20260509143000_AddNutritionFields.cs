using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    [Migration("20260509143000_AddNutritionFields")]
    public partial class AddNutritionFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Carbs",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CaloriesPerUnit",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

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

            migrationBuilder.AddColumn<double>(
                name: "Carbs",
                table: "FoodItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Fat",
                table: "FoodItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Protein",
                table: "FoodItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Carbs", table: "Ingredients");
            migrationBuilder.DropColumn(name: "CaloriesPerUnit", table: "Ingredients");
            migrationBuilder.DropColumn(name: "Fat", table: "Ingredients");
            migrationBuilder.DropColumn(name: "Protein", table: "Ingredients");
            migrationBuilder.DropColumn(name: "Unit", table: "Ingredients");
            migrationBuilder.DropColumn(name: "Carbs", table: "FoodItems");
            migrationBuilder.DropColumn(name: "Fat", table: "FoodItems");
            migrationBuilder.DropColumn(name: "Protein", table: "FoodItems");
        }
    }
}
