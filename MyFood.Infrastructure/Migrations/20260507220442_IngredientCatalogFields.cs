using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IngredientCatalogFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DECLARE @fkName sysname;

                SELECT TOP 1 @fkName = fk.name
                FROM sys.foreign_keys fk
                WHERE fk.parent_object_id = OBJECT_ID(N'[Ingredients]')
                  AND fk.referenced_object_id = OBJECT_ID(N'[FoodItems]');

                IF @fkName IS NOT NULL
                    EXEC(N'ALTER TABLE [Ingredients] DROP CONSTRAINT [' + @fkName + N']');

                IF COL_LENGTH('Ingredients', 'FoodEntityId') IS NOT NULL AND COL_LENGTH('Ingredients', 'FoodId') IS NULL
                    EXEC sp_rename 'Ingredients.FoodEntityId', 'FoodId', 'COLUMN';

                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Ingredients_FoodEntityId' AND object_id = OBJECT_ID(N'[Ingredients]'))
                    EXEC sp_rename 'Ingredients.IX_Ingredients_FoodEntityId', 'IX_Ingredients_FoodId', 'INDEX';

                IF COL_LENGTH('Ingredients', 'FoodId') IS NULL
                    ALTER TABLE [Ingredients] ADD [FoodId] int NULL;
                ELSE
                    ALTER TABLE [Ingredients] ALTER COLUMN [FoodId] int NULL;

                IF COL_LENGTH('Ingredients', 'CaloriesPerUnit') IS NULL
                    ALTER TABLE [Ingredients] ADD [CaloriesPerUnit] decimal(10,2) NOT NULL CONSTRAINT [DF_Ingredients_CaloriesPerUnit] DEFAULT 0;

                IF COL_LENGTH('Ingredients', 'Carbs') IS NULL
                    ALTER TABLE [Ingredients] ADD [Carbs] decimal(10,2) NOT NULL CONSTRAINT [DF_Ingredients_Carbs] DEFAULT 0;

                IF COL_LENGTH('Ingredients', 'Created') IS NULL
                    ALTER TABLE [Ingredients] ADD [Created] datetime2 NOT NULL CONSTRAINT [DF_Ingredients_Created] DEFAULT '0001-01-01T00:00:00.0000000';

                IF COL_LENGTH('Ingredients', 'Fat') IS NULL
                    ALTER TABLE [Ingredients] ADD [Fat] decimal(10,2) NOT NULL CONSTRAINT [DF_Ingredients_Fat] DEFAULT 0;

                IF COL_LENGTH('Ingredients', 'Protein') IS NULL
                    ALTER TABLE [Ingredients] ADD [Protein] decimal(10,2) NOT NULL CONSTRAINT [DF_Ingredients_Protein] DEFAULT 0;

                IF COL_LENGTH('Ingredients', 'Quantity') IS NULL
                    ALTER TABLE [Ingredients] ADD [Quantity] int NOT NULL CONSTRAINT [DF_Ingredients_Quantity] DEFAULT 0;

                IF COL_LENGTH('Ingredients', 'Unit') IS NULL
                    ALTER TABLE [Ingredients] ADD [Unit] nvarchar(50) NOT NULL CONSTRAINT [DF_Ingredients_Unit] DEFAULT '';

                IF COL_LENGTH('AspNetUsers', 'FullName') IS NULL
                    ALTER TABLE [AspNetUsers] ADD [FullName] nvarchar(max) NOT NULL CONSTRAINT [DF_AspNetUsers_FullName] DEFAULT '';

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Ingredients_FoodId' AND object_id = OBJECT_ID(N'[Ingredients]'))
                    CREATE INDEX [IX_Ingredients_FoodId] ON [Ingredients] ([FoodId]);

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = 'FK_Ingredients_FoodItems_FoodId'
                      AND parent_object_id = OBJECT_ID(N'[Ingredients]')
                )
                    ALTER TABLE [Ingredients]
                    ADD CONSTRAINT [FK_Ingredients_FoodItems_FoodId]
                    FOREIGN KEY ([FoodId]) REFERENCES [FoodItems] ([Id]) ON DELETE CASCADE;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_FoodItems_FoodId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "CaloriesPerUnit",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Carbs",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Fat",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Protein",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "FoodId",
                table: "Ingredients",
                newName: "FoodEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Ingredients_FoodId",
                table: "Ingredients",
                newName: "IX_Ingredients_FoodEntityId");

            migrationBuilder.AlterColumn<int>(
                name: "FoodEntityId",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
