using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    public partial class AddAuth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Identity tables are already created in the AuthenticationUser migration.
            // This migration is kept as a no-op to prevent duplicate AspNetRoles/AspNetUsers creation.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No rollback needed because this migration no longer creates tables.
        }
    }
}