using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruitment.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Resumes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Resumes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "Resumes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Resumes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Resumes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Resumes",
                type: "int",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Resumes");
        }
    }
}
