using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyFoodIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_FoodItems_FoodEntityId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_FoodEntityId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "FoodEntityId",
                table: "Ingredients");

            migrationBuilder.CreateTable(
                name: "FoodIngredients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodEntityId = table.Column<int>(type: "int", nullable: false),
                    IngredientEntityId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodIngredients_FoodItems_FoodEntityId",
                        column: x => x.FoodEntityId,
                        principalTable: "FoodItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoodIngredients_Ingredients_IngredientEntityId",
                        column: x => x.IngredientEntityId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodIngredients_FoodEntityId",
                table: "FoodIngredients",
                column: "FoodEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodIngredients_IngredientEntityId",
                table: "FoodIngredients",
                column: "IngredientEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoodIngredients");

            migrationBuilder.AddColumn<int>(
                name: "FoodEntityId",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
    }
}
