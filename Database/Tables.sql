-- ============================================================
-- File: Tables.sql
-- Purpose: Create all tables for RecruitmentDB
-- ============================================================

USE [RecruitmentDB];
GO

-- ---- Roles ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
CREATE TABLE [dbo].[Roles] (
    [RoleId]    INT           IDENTITY(1,1) PRIMARY KEY,
    [Name]      NVARCHAR(50)  NOT NULL UNIQUE
);
GO

-- ---- Users ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
CREATE TABLE [dbo].[Users] (
    [UserId]      INT            IDENTITY(1,1) PRIMARY KEY,
    [FullName]    NVARCHAR(100)  NOT NULL,
    [Email]       NVARCHAR(150)  NOT NULL UNIQUE,
    [PasswordHash]NVARCHAR(MAX)  NOT NULL,
    [RoleId]      INT            NOT NULL,
    [CreatedDate] DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ---- Candidates ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Candidates')
CREATE TABLE [dbo].[Candidates] (
    [CandidateId] INT            IDENTITY(1,1) PRIMARY KEY,
    [UserId]      INT            NOT NULL UNIQUE,
    [FirstName]   NVARCHAR(100)  NOT NULL,
    [LastName]    NVARCHAR(100)  NOT NULL,
    [Phone]       NVARCHAR(20)   NULL,
    [Address]     NVARCHAR(300)  NULL,
    [Bio]         NVARCHAR(MAX)  NULL,
    [Experience]  NVARCHAR(MAX)  NULL,
    [Education]   NVARCHAR(MAX)  NULL
);
GO

-- ---- Companies ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Companies')
CREATE TABLE [dbo].[Companies] (
    [CompanyId]   INT            IDENTITY(1,1) PRIMARY KEY,
    [CompanyName] NVARCHAR(200)  NOT NULL,
    [Email]       NVARCHAR(150)  NULL,
    [Phone]       NVARCHAR(20)   NULL,
    [Address]     NVARCHAR(300)  NULL,
    [Website]     NVARCHAR(256)  NULL
);
GO

-- ---- Recruiters ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Recruiters')
CREATE TABLE [dbo].[Recruiters] (
    [RecruiterId] INT            IDENTITY(1,1) PRIMARY KEY,
    [UserId]      INT            NOT NULL UNIQUE,
    [FirstName]   NVARCHAR(100)  NOT NULL,
    [LastName]    NVARCHAR(100)  NOT NULL,
    [CompanyId]   INT            NOT NULL
);
GO

-- ---- Jobs ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Jobs')
CREATE TABLE [dbo].[Jobs] (
    [JobId]       INT            IDENTITY(1,1) PRIMARY KEY,
    [CompanyId]   INT            NOT NULL,
    [RecruiterId] INT            NOT NULL,
    [Title]       NVARCHAR(200)  NOT NULL,
    [Description] NVARCHAR(MAX)  NOT NULL,
    [Requirements]NVARCHAR(MAX)  NOT NULL,
    [Salary]      DECIMAL(18,2)  NULL,
    [Location]    NVARCHAR(200)  NULL,
    [Status]      NVARCHAR(50)   NOT NULL DEFAULT 'Active',
    [CreatedDate] DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ---- Skills ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Skills')
CREATE TABLE [dbo].[Skills] (
    [SkillId]  INT           IDENTITY(1,1) PRIMARY KEY,
    [Name]     NVARCHAR(100) NOT NULL UNIQUE
);
GO

-- ---- CandidateSkills (Many-to-Many) ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'CandidateSkills')
CREATE TABLE [dbo].[CandidateSkills] (
    [CandidateId] INT NOT NULL,
    [SkillId]     INT NOT NULL,
    CONSTRAINT PK_CandidateSkills PRIMARY KEY ([CandidateId], [SkillId])
);
GO

-- ---- Resumes ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Resumes')
CREATE TABLE [dbo].[Resumes] (
    [ResumeId]          INT              IDENTITY(1,1) PRIMARY KEY,
    [CandidateId]       INT              NOT NULL,
    [FileName]          NVARCHAR(300)    NOT NULL,
    [FilePath]          NVARCHAR(500)    NOT NULL,
    [ExtractedText]     NVARCHAR(MAX)    NULL,
    [AIAnalysisResult]  NVARCHAR(MAX)    NULL,
    [AiScore]           INT              NOT NULL DEFAULT 0,
    [UploadedDate]      DATETIME2        NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ---- Applications ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Applications')
CREATE TABLE [dbo].[Applications] (
    [ApplicationId] INT           IDENTITY(1,1) PRIMARY KEY,
    [CandidateId]   INT           NOT NULL,
    [JobId]         INT           NOT NULL,
    [Status]        NVARCHAR(50)  NOT NULL DEFAULT 'Pending',
    [AppliedDate]   DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ---- Interviews ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Interviews')
CREATE TABLE [dbo].[Interviews] (
    [InterviewId]     INT           IDENTITY(1,1) PRIMARY KEY,
    [ApplicationId]   INT           NOT NULL,
    [InterviewDate]   DATETIME2     NOT NULL,
    [Location]        NVARCHAR(200) NULL,
    [InterviewStatus] NVARCHAR(50)  NOT NULL DEFAULT 'Scheduled'
);
GO

-- ---- Evaluations ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Evaluations')
CREATE TABLE [dbo].[Evaluations] (
    [EvaluationId] INT            IDENTITY(1,1) PRIMARY KEY,
    [InterviewId]  INT            NOT NULL,
    [RecruiterId]  INT            NOT NULL,
    [Rating]       INT            NOT NULL,
    [Comments]     NVARCHAR(1000) NULL,
    [CreatedAt]    DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ---- Notifications ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Notifications')
CREATE TABLE [dbo].[Notifications] (
    [NotificationId] INT           IDENTITY(1,1) PRIMARY KEY,
    [UserId]         INT           NOT NULL,
    [Message]        NVARCHAR(500) NOT NULL,
    [IsRead]         BIT           NOT NULL DEFAULT 0,
    [CreatedAt]      DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ---- Reports ----
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reports')
CREATE TABLE [dbo].[Reports] (
    [ReportId]   INT           IDENTITY(1,1) PRIMARY KEY,
    [Title]      NVARCHAR(200) NOT NULL,
    [Data]       NVARCHAR(MAX) NOT NULL,
    [CreatedAt]  DATETIME2     NOT NULL DEFAULT GETUTCDATE()
);
GO

PRINT 'All tables created successfully.';
GO
