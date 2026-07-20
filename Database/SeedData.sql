-- ============================================================
-- File: SeedData.sql
-- Purpose: Insert foundational seed data for RecruitmentDB
-- ============================================================

USE [RecruitmentDB];
GO

-- ---- Roles ----
IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Admin')
    INSERT INTO [dbo].[Roles] ([Name]) VALUES ('Admin');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Recruiter')
    INSERT INTO [dbo].[Roles] ([Name]) VALUES ('Recruiter');

IF NOT EXISTS (SELECT 1 FROM [dbo].[Roles] WHERE [Name] = 'Candidate')
    INSERT INTO [dbo].[Roles] ([Name]) VALUES ('Candidate');
GO

-- ---- Skills ----
MERGE [dbo].[Skills] AS target
USING (VALUES
    ('C#'), ('ASP.NET Core'), ('SQL Server'), ('React'), ('TypeScript'),
    ('JavaScript'), ('Python'), ('Azure'), ('Docker'), ('Git'),
    ('Entity Framework Core'), ('Machine Learning'), ('Node.js'), ('REST APIs'),
    ('Communication'), ('Team Leadership'), ('Problem Solving')
) AS source ([Name])
ON target.[Name] = source.[Name]
WHEN NOT MATCHED THEN INSERT ([Name]) VALUES (source.[Name]);
GO

-- ---- Default Admin User (password: Admin@123 hashed via BCrypt) ----
IF NOT EXISTS (SELECT 1 FROM [dbo].[Users] WHERE [Email] = 'admin@recruitment.com')
BEGIN
    DECLARE @AdminRoleId INT = (SELECT [RoleId] FROM [dbo].[Roles] WHERE [Name] = 'Admin');
    INSERT INTO [dbo].[Users] ([FullName], [Email], [PasswordHash], [RoleId])
    VALUES (
        'System Admin',
        'admin@recruitment.com',
        '$2a$11$ExampleHashedPasswordHereReplaceWithActualBCryptHash',
        @AdminRoleId
    );
END
GO

-- ---- Sample Company ----
IF NOT EXISTS (SELECT 1 FROM [dbo].[Companies] WHERE [CompanyName] = 'TechCorp Solutions')
    INSERT INTO [dbo].[Companies] ([CompanyName], [Email], [Phone], [Website])
    VALUES ('TechCorp Solutions', 'hr@techcorp.com', '+1-555-0100', 'https://techcorp.com');
GO

PRINT 'Seed data inserted successfully.';
GO
