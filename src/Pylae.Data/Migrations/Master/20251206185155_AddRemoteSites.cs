using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pylae.Data.Migrations.Master
{
    /// <inheritdoc />
    public partial class AddRemoteSites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RemoteSites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SiteCode = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Host = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    ApiKey = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoteSites", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemoteSites_SiteCode",
                table: "RemoteSites",
                column: "SiteCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RemoteSites");
        }
    }
}
