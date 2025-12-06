using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pylae.Data.Migrations.Visits
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Visits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VisitGuid = table.Column<string>(type: "TEXT", nullable: true),
                    MemberId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    MemberNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    MemberFirstName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MemberLastName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    MemberBusinessRank = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MemberOfficeName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MemberIsPermanentStaff = table.Column<bool>(type: "INTEGER", nullable: false),
                    MemberTypeCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MemberTypeName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MemberPersonalIdNumber = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    MemberBusinessIdNumber = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Direction = table.Column<string>(type: "TEXT", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TimestampLocal = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Method = table.Column<string>(type: "TEXT", nullable: false),
                    SiteCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Username = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    UserDisplayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    WorkstationId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visits", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_MemberNumber_TimestampUtc",
                table: "Visits",
                columns: new[] { "MemberNumber", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_SiteCode_TimestampUtc",
                table: "Visits",
                columns: new[] { "SiteCode", "TimestampUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_TimestampUtc",
                table: "Visits",
                column: "TimestampUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Visits");
        }
    }
}
