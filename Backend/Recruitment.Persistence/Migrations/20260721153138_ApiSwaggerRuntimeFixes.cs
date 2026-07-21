using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recruitment.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ApiSwaggerRuntimeFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UploadedDate",
                table: "Resumes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.Sql(
                "UPDATE [Resumes] SET [UploadedDate] = [CreatedAt] WHERE [UploadedDate] IS NULL OR [UploadedDate] = '0001-01-01T00:00:00.0000000'");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReadAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Notifications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Jobs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PostedDate",
                table: "Jobs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.Sql(
                "UPDATE [Jobs] SET [PostedDate] = [CreatedDate] WHERE [PostedDate] IS NULL OR [PostedDate] = '0001-01-01T00:00:00.0000000'");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Interviews",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Evaluations",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationId",
                table: "Evaluations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CandidateId",
                table: "Evaluations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "CommunicationScore",
                table: "Evaluations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CultureFitScore",
                table: "Evaluations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExperienceScore",
                table: "Evaluations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "HiringManagerId",
                table: "Evaluations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OverallScore",
                table: "Evaluations",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Recommendation",
                table: "Evaluations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TechnicalScore",
                table: "Evaluations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Evaluations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE [e]
SET
    [ApplicationId] = [i].[ApplicationId],
    [CandidateId] = [a].[CandidateId],
    [TechnicalScore] = CASE WHEN [e].[Score] BETWEEN 0 AND 100 THEN [e].[Score] ELSE 0 END,
    [CommunicationScore] = CASE WHEN [e].[Score] BETWEEN 0 AND 100 THEN [e].[Score] ELSE 0 END,
    [ExperienceScore] = CASE WHEN [e].[Score] BETWEEN 0 AND 100 THEN [e].[Score] ELSE 0 END,
    [CultureFitScore] = CASE WHEN [e].[Score] BETWEEN 0 AND 100 THEN [e].[Score] ELSE 0 END,
    [OverallScore] = CASE WHEN [e].[Score] BETWEEN 0 AND 100 THEN CAST([e].[Score] AS float) ELSE 0 END,
    [Recommendation] = CASE
        WHEN [e].[Score] >= 90 THEN N'Hire'
        WHEN [e].[Score] >= 75 THEN N'Strong Candidate'
        WHEN [e].[Score] >= 60 THEN N'Consider'
        ELSE N'Reject'
    END
FROM [Evaluations] AS [e]
INNER JOIN [Interviews] AS [i] ON [e].[InterviewId] = [i].[Id]
INNER JOIN [Applications] AS [a] ON [i].[ApplicationId] = [a].[Id];");

            migrationBuilder.CreateTable(
                name: "ApplicationStatusHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NewStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationStatusHistories_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationStatusHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HiringDecisions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DecidedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DecidedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HiringDecisions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HiringDecisions_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HiringDecisions_Users_DecidedByUserId",
                        column: x => x.DecidedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_ApplicationId",
                table: "Evaluations",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_CandidateId",
                table: "Evaluations",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_HiringManagerId",
                table: "Evaluations",
                column: "HiringManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatusHistories_ApplicationId",
                table: "ApplicationStatusHistories",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationStatusHistories_ChangedByUserId",
                table: "ApplicationStatusHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HiringDecisions_ApplicationId",
                table: "HiringDecisions",
                column: "ApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_HiringDecisions_DecidedByUserId",
                table: "HiringDecisions",
                column: "DecidedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.Sql(
                "ALTER TABLE [Evaluations] WITH NOCHECK ADD CONSTRAINT [FK_Evaluations_Applications_ApplicationId] FOREIGN KEY ([ApplicationId]) REFERENCES [Applications] ([Id]);");

            migrationBuilder.Sql(
                "ALTER TABLE [Evaluations] WITH NOCHECK ADD CONSTRAINT [FK_Evaluations_Candidates_CandidateId] FOREIGN KEY ([CandidateId]) REFERENCES [Candidates] ([Id]);");

            migrationBuilder.Sql(
                "ALTER TABLE [Evaluations] WITH NOCHECK ADD CONSTRAINT [FK_Evaluations_Users_HiringManagerId] FOREIGN KEY ([HiringManagerId]) REFERENCES [Users] ([Id]);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Applications_ApplicationId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Candidates_CandidateId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Users_HiringManagerId",
                table: "Evaluations");

            migrationBuilder.DropTable(
                name: "ApplicationStatusHistories");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "HiringDecisions");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_ApplicationId",
                table: "Evaluations");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_CandidateId",
                table: "Evaluations");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_HiringManagerId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "UploadedDate",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "ReadAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "PostedDate",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Interviews");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "CandidateId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "CommunicationScore",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "CultureFitScore",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "ExperienceScore",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "HiringManagerId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "OverallScore",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "Recommendation",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "TechnicalScore",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Evaluations");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Evaluations",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);
        }
    }
}
