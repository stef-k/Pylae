using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pylae.Data.Migrations.Master
{
    /// <inheritdoc />
    public partial class AddMemberTypeNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_MemberTypes_MemberTypeId1",
                table: "Members");

            migrationBuilder.DropIndex(
                name: "IX_Members_MemberTypeId1",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "MemberTypeId1",
                table: "Members");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MemberTypeId1",
                table: "Members",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberTypeId1",
                table: "Members",
                column: "MemberTypeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_MemberTypes_MemberTypeId1",
                table: "Members",
                column: "MemberTypeId1",
                principalTable: "MemberTypes",
                principalColumn: "Id");
        }
    }
}
