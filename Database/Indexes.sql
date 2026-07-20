-- ============================================================
-- File: Indexes.sql
-- Purpose: Performance indexes for RecruitmentDB
-- ============================================================

USE [RecruitmentDB];
GO

-- Users index on Email (speeds up login lookups)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
    CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
GO

-- Jobs index on Title (job search)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Jobs_Title')
    CREATE NONCLUSTERED INDEX [IX_Jobs_Title] ON [dbo].[Jobs] ([Title]);
GO

-- Jobs index on Location (geo-filtered job browsing)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Jobs_Location')
    CREATE NONCLUSTERED INDEX [IX_Jobs_Location] ON [dbo].[Jobs] ([Location]);
GO

-- Jobs index on Status (Active jobs filter)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Jobs_Status')
    CREATE NONCLUSTERED INDEX [IX_Jobs_Status] ON [dbo].[Jobs] ([Status]);
GO

-- Applications index on Status (pipeline/status dashboards)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Applications_Status')
    CREATE NONCLUSTERED INDEX [IX_Applications_Status] ON [dbo].[Applications] ([Status]);
GO

-- CandidateSkills index on SkillId (skill matching for AI)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CandidateSkills_SkillId')
    CREATE NONCLUSTERED INDEX [IX_CandidateSkills_SkillId] ON [dbo].[CandidateSkills] ([SkillId]);
GO

-- Candidates index on UserId (user-candidate profile lookups)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Candidates_UserId')
    CREATE NONCLUSTERED INDEX [IX_Candidates_UserId] ON [dbo].[Candidates] ([UserId]);
GO

-- Resumes index on CandidateId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Resumes_CandidateId')
    CREATE NONCLUSTERED INDEX [IX_Resumes_CandidateId] ON [dbo].[Resumes] ([CandidateId]);
GO

-- Notifications index on UserId + IsRead (notification feed performance)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_UserId_IsRead')
    CREATE NONCLUSTERED INDEX [IX_Notifications_UserId_IsRead]
    ON [dbo].[Notifications] ([UserId], [IsRead]);
GO

PRINT 'All indexes created successfully.';
GO
