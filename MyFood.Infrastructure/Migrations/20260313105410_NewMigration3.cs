using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientItems_FoodItems_FoodEntityId",
                table: "IngredientItems");

            migrationBuilder.AlterColumn<int>(
                name: "FoodEntityId",
                table: "IngredientItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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

            migrationBuilder.AlterColumn<int>(
                name: "FoodEntityId",
                table: "IngredientItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientItems_FoodItems_FoodEntityId",
                table: "IngredientItems",
                column: "FoodEntityId",
                principalTable: "FoodItems",
                principalColumn: "Id");
        }
    }
}
