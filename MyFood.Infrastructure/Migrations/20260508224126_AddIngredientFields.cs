using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FoodEntityId",
                table: "Ingredients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<double>(
                name: "CaloriesPerUnit",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Carbs",
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
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriesPerUnit",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Carbs",
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

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "FoodEntityId",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
