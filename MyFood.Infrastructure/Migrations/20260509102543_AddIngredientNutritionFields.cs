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
            // Drop FK and index first
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientItems_FoodItems_FoodEntityId",
                table: "IngredientItems");

            migrationBuilder.DropIndex(
                name: "IX_IngredientItems_FoodEntityId",
                table: "IngredientItems");

            // Drop old column
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "IngredientItems");

            // Add new nutrition columns
            migrationBuilder.AddColumn<double>(
                name: "CaloriesPerUnit",
                table: "IngredientItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Carbs",
                table: "IngredientItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Fat",
                table: "IngredientItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Protein",
                table: "IngredientItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "IngredientItems",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // Make FoodEntityId nullable
            migrationBuilder.AlterColumn<int>(
                name: "FoodEntityId",
                table: "IngredientItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // Rename table (index/FK names stay as-is until we recreate them)
            migrationBuilder.RenameTable(
                name: "IngredientItems",
                newName: "Ingredients");

            // Recreate index and FK with new names on the renamed table
            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_FoodEntityId",
                table: "Ingredients",
                column: "FoodEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_FoodItems_FoodEntityId",
                table: "Ingredients",
                column: "FoodEntityId",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_FoodItems_FoodEntityId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_FoodEntityId",
                table: "Ingredients");

            migrationBuilder.DropColumn(name: "CaloriesPerUnit", table: "Ingredients");
            migrationBuilder.DropColumn(name: "Carbs", table: "Ingredients");
            migrationBuilder.DropColumn(name: "Fat", table: "Ingredients");
            migrationBuilder.DropColumn(name: "Protein", table: "Ingredients");
            migrationBuilder.DropColumn(name: "Unit", table: "Ingredients");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "FoodEntityId",
                table: "Ingredients",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.RenameTable(
                name: "Ingredients",
                newName: "IngredientItems");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientItems_FoodEntityId",
                table: "IngredientItems",
                column: "FoodEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientItems_FoodItems_FoodEntityId",
                table: "IngredientItems",
                column: "FoodEntityId",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
