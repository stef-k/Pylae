using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pylae.Data.Migrations.Master
{
    /// <inheritdoc />
    public partial class Master_AddUniqueMemberNumberIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add unique partial index on MemberNumber for active members only
            // This ensures active members cannot have duplicate member numbers
            migrationBuilder.Sql(
                @"CREATE UNIQUE INDEX IX_Members_MemberNumber_Active
                  ON Members(MemberNumber)
                  WHERE IsActive = 1;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Members_MemberNumber_Active;");
        }
    }
}
