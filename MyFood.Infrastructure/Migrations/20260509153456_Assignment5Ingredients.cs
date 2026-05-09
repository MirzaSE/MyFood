using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Assignment5Ingredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "IngredientItems",
                newName: "Ingredients");

            migrationBuilder.RenameIndex(
                name: "IX_IngredientItems_FoodEntityId",
                table: "Ingredients",
                newName: "IX_Ingredients_FoodEntityId");

            migrationBuilder.DropForeignKey(
                name: "FK_IngredientItems_FoodItems_FoodEntityId",
                table: "Ingredients");

            migrationBuilder.AlterColumn<int>(
                name: "FoodEntityId",
                table: "Ingredients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_FoodItems_FoodEntityId",
                table: "Ingredients",
                column: "FoodEntityId",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_FoodItems_FoodEntityId",
                table: "Ingredients");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredients_FoodEntityId",
                table: "Ingredients",
                newName: "IX_IngredientItems_FoodEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientItems_FoodItems_FoodEntityId",
                table: "Ingredients",
                column: "FoodEntityId",
                principalTable: "FoodItems",
                principalColumn: "Id");

            migrationBuilder.RenameTable(
                name: "Ingredients",
                newName: "IngredientItems");
        }
    }
}
