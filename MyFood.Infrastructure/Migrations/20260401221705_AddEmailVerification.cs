using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_FoodItems_Food_Id",
                table: "Ingredients");

            migrationBuilder.RenameColumn(
                name: "Food_Id",
                table: "Ingredients",
                newName: "FoodId");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredients_Food_Id",
                table: "Ingredients",
                newName: "IX_Ingredients_FoodId");

            migrationBuilder.AddColumn<string>(
                name: "EmailVerificationToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailVerificationTokenExpiry",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailVerified",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_FoodItems_FoodId",
                table: "Ingredients",
                column: "FoodId",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_FoodItems_FoodId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "EmailVerificationToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailVerificationTokenExpiry",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsEmailVerified",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "FoodId",
                table: "Ingredients",
                newName: "Food_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredients_FoodId",
                table: "Ingredients",
                newName: "IX_Ingredients_Food_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_FoodItems_Food_Id",
                table: "Ingredients",
                column: "Food_Id",
                principalTable: "FoodItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
