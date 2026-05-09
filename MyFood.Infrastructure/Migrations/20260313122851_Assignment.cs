using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <inheritdoc />
    /// <remarks>
    /// This migration originally created the Ingredients table with a different schema
    /// (Name nvarchar(260), Quantity, FoodId). After merging, the AuthenticationUser
    /// migration creates the Ingredients table with the canonical schema (Name nvarchar(100),
    /// FoodEntityId), so this migration is now a no-op to avoid a duplicate-table error.
    /// </remarks>
    public partial class Assignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
