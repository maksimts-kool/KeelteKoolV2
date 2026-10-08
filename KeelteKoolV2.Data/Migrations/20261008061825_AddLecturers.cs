using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeelteKoolV2.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLecturers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    ImageID = table.Column<Guid>(type: "TEXT", nullable: false),
                    ImageTitle = table.Column<string>(type: "TEXT", nullable: true),
                    ImageData = table.Column<byte[]>(type: "BLOB", nullable: true),
                    LecturerId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.ImageID);
                });

            migrationBuilder.CreateTable(
                name: "Lecturers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Qualifications = table.Column<string>(type: "TEXT", nullable: true),
                    UserID = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lecturers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Lecturers");
        }
    }
}
