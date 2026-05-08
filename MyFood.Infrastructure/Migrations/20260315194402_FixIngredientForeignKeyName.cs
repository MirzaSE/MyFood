using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixIngredientForeignKeyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientItems_FoodItems_FoodId",
                table: "IngredientItems");

            migrationBuilder.RenameColumn(
                name: "FoodId",
                table: "IngredientItems",
                newName: "FoodEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientItems_FoodId",
                table: "IngredientItems",
                newName: "IX_IngredientItems_FoodEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientItems_FoodItems_FoodEntityId",
                table: "IngredientItems",
                column: "FoodEntityId",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientItems_FoodItems_FoodEntityId",
                table: "IngredientItems");

            migrationBuilder.RenameColumn(
                name: "FoodEntityId",
                table: "IngredientItems",
                newName: "FoodId");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientItems_FoodEntityId",
                table: "IngredientItems",
                newName: "IX_IngredientItems_FoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientItems_FoodItems_FoodId",
                table: "IngredientItems",
                column: "FoodId",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
