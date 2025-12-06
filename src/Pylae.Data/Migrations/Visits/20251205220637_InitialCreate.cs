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
                name: "AuditLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SiteCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    ActionType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TargetType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TargetId = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DetailsJson = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PasswordSalt = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    PasswordIterations = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    QuickCodeHash = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    QuickCodeSalt = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    QuickCodeIterations = table.Column<int>(type: "INTEGER", nullable: false),
                    IsShared = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsSystem = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastLoginAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    MemberNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    BusinessRank = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Office = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    IsPermanentStaff = table.Column<bool>(type: "INTEGER", nullable: false),
                    MemberTypeId = table.Column<int>(type: "INTEGER", nullable: true),
                    MemberTypeId1 = table.Column<int>(type: "INTEGER", nullable: true),
                    PersonalIdNumber = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    BusinessIdNumber = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    PhotoFileName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    BadgeIssueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BadgeExpiryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_MemberTypes_MemberTypeId",
                        column: x => x.MemberTypeId,
                        principalTable: "MemberTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Members_MemberTypes_MemberTypeId1",
                        column: x => x.MemberTypeId1,
                        principalTable: "MemberTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_ActionType",
                table: "AuditLog",
                column: "ActionType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_TimestampUtc",
                table: "AuditLog",
                column: "TimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Members_LastName",
                table: "Members",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberNumber",
                table: "Members",
                column: "MemberNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberTypeId",
                table: "Members",
                column: "MemberTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_MemberTypeId1",
                table: "Members",
                column: "MemberTypeId1");

            migrationBuilder.CreateIndex(
                name: "IX_MemberTypes_Code",
                table: "MemberTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

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
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Visits");

            migrationBuilder.DropTable(
                name: "MemberTypes");
        }
    }
}
