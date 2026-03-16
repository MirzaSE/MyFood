using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixIngredientFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_FoodItems_foodItemId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_foodItemId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "foodItemId",
                table: "Ingredients");

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_Food_Id",
                table: "Ingredients",
                column: "Food_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_FoodItems_Food_Id",
                table: "Ingredients",
                column: "Food_Id",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_FoodItems_Food_Id",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_Food_Id",
                table: "Ingredients");

            migrationBuilder.AddColumn<int>(
                name: "foodItemId",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_foodItemId",
                table: "Ingredients",
                column: "foodItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_FoodItems_foodItemId",
                table: "Ingredients",
                column: "foodItemId",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
