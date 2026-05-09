using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IngredientItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    Quantity = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FoodEntityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IngredientItems_FoodItems_FoodEntityId",
                        column: x => x.FoodEntityId,
                        principalTable: "FoodItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IngredientItems_FoodEntityId",
                table: "IngredientItems",
                column: "FoodEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientItems");
        }
    }
}
