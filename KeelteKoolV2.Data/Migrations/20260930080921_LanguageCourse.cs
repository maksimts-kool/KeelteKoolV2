using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeelteKoolV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class LanguageCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LanguageCourses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nimetus = table.Column<string>(type: "TEXT", nullable: false),
                    Keel = table.Column<string>(type: "TEXT", nullable: false),
                    Tase = table.Column<string>(type: "TEXT", nullable: false),
                    Kirjeldus = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedBy = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageCourses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LanguageCourses");
        }
    }
}
