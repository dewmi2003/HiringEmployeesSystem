-- ============================================================
-- File: StoredProcedures.sql
-- Purpose: Stored procedures for common queries in RecruitmentDB
-- ============================================================

USE [RecruitmentDB];
GO

-- ---- Get Candidate with Skills ----
CREATE OR ALTER PROCEDURE [dbo].[usp_GetCandidateWithSkills]
    @CandidateId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        c.[CandidateId], c.[FirstName], c.[LastName], c.[Phone], c.[Bio],
        c.[Experience], c.[Education],
        s.[SkillId], s.[Name] AS SkillName
    FROM [dbo].[Candidates] c
    LEFT JOIN [dbo].[CandidateSkills] cs ON c.[CandidateId] = cs.[CandidateId]
    LEFT JOIN [dbo].[Skills] s ON cs.[SkillId] = s.[SkillId]
    WHERE c.[CandidateId] = @CandidateId;
END
GO

-- ---- Get Active Jobs with Company Info ----
CREATE OR ALTER PROCEDURE [dbo].[usp_GetActiveJobs]
    @Location NVARCHAR(200) = NULL,
    @Title    NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        j.[JobId], j.[Title], j.[Description], j.[Requirements],
        j.[Salary], j.[Location], j.[Status], j.[CreatedDate],
        co.[CompanyName], co.[Website]
    FROM [dbo].[Jobs] j
    INNER JOIN [dbo].[Companies] co ON j.[CompanyId] = co.[CompanyId]
    WHERE j.[Status] = 'Active'
        AND (@Location IS NULL OR j.[Location] LIKE '%' + @Location + '%')
        AND (@Title    IS NULL OR j.[Title]    LIKE '%' + @Title    + '%');
END
GO

-- ---- Get Applications For Job ----
CREATE OR ALTER PROCEDURE [dbo].[usp_GetApplicationsByJob]
    @JobId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        a.[ApplicationId], a.[Status], a.[AppliedDate],
        c.[FirstName], c.[LastName],
        u.[Email]
    FROM [dbo].[Applications] a
    INNER JOIN [dbo].[Candidates] c ON a.[CandidateId] = c.[CandidateId]
    INNER JOIN [dbo].[Users]      u ON c.[UserId]       = u.[UserId]
    WHERE a.[JobId] = @JobId
    ORDER BY a.[AppliedDate] DESC;
END
GO

-- ---- Update Application Status ----
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateApplicationStatus]
    @ApplicationId INT,
    @Status        NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Applications]
    SET [Status] = @Status
    WHERE [ApplicationId] = @ApplicationId;
END
GO

-- ---- Get Unread Notifications For User ----
CREATE OR ALTER PROCEDURE [dbo].[usp_GetUnreadNotifications]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [NotificationId], [Message], [CreatedAt]
    FROM [dbo].[Notifications]
    WHERE [UserId] = @UserId AND [IsRead] = 0
    ORDER BY [CreatedAt] DESC;
END
GO

-- ---- Mark All Notifications Read ----
CREATE OR ALTER PROCEDURE [dbo].[usp_MarkNotificationsRead]
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Notifications]
    SET [IsRead] = 1
    WHERE [UserId] = @UserId AND [IsRead] = 0;
END
GO

PRINT 'All stored procedures created successfully.';
GO
