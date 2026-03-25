using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientEntities_FoodItems_FoodEntityId",
                table: "IngredientEntities");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientEntities_FoodItems_FoodEntityId",
                table: "IngredientEntities",
                column: "FoodEntityId",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientEntities_FoodItems_FoodEntityId",
                table: "IngredientEntities");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientEntities_FoodItems_FoodEntityId",
                table: "IngredientEntities",
                column: "FoodEntityId",
                principalTable: "FoodItems",
                principalColumn: "Id");
        }
    }
}
