using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFood.Infrastructure.Migrations
{
    /// <summary>
    /// Originally created the Ingredients table, but a later migration
    /// (AuthenticationUser) recreates it with the correct schema. To avoid
    /// a duplicate-table error on a fresh database, this migration is now
    /// intentionally a no-op.
    /// </summary>
    public partial class Assignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
