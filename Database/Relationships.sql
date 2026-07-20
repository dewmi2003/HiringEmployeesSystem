-- ============================================================
-- File: Relationships.sql
-- Purpose: Add Foreign Key constraints for RecruitmentDB
-- ============================================================

USE [RecruitmentDB];
GO

-- Users -> Roles
ALTER TABLE [dbo].[Users]
    ADD CONSTRAINT FK_Users_Roles FOREIGN KEY ([RoleId])
    REFERENCES [dbo].[Roles] ([RoleId])
    ON DELETE RESTRICT;
GO

-- Candidates -> Users
ALTER TABLE [dbo].[Candidates]
    ADD CONSTRAINT FK_Candidates_Users FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users] ([UserId])
    ON DELETE CASCADE;
GO

-- Recruiters -> Users
ALTER TABLE [dbo].[Recruiters]
    ADD CONSTRAINT FK_Recruiters_Users FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users] ([UserId])
    ON DELETE CASCADE;
GO

-- Recruiters -> Companies
ALTER TABLE [dbo].[Recruiters]
    ADD CONSTRAINT FK_Recruiters_Companies FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Companies] ([CompanyId])
    ON DELETE RESTRICT;
GO

-- Jobs -> Companies
ALTER TABLE [dbo].[Jobs]
    ADD CONSTRAINT FK_Jobs_Companies FOREIGN KEY ([CompanyId])
    REFERENCES [dbo].[Companies] ([CompanyId])
    ON DELETE RESTRICT;
GO

-- Jobs -> Recruiters
ALTER TABLE [dbo].[Jobs]
    ADD CONSTRAINT FK_Jobs_Recruiters FOREIGN KEY ([RecruiterId])
    REFERENCES [dbo].[Recruiters] ([RecruiterId])
    ON DELETE RESTRICT;
GO

-- CandidateSkills -> Candidates
ALTER TABLE [dbo].[CandidateSkills]
    ADD CONSTRAINT FK_CandidateSkills_Candidates FOREIGN KEY ([CandidateId])
    REFERENCES [dbo].[Candidates] ([CandidateId])
    ON DELETE CASCADE;
GO

-- CandidateSkills -> Skills
ALTER TABLE [dbo].[CandidateSkills]
    ADD CONSTRAINT FK_CandidateSkills_Skills FOREIGN KEY ([SkillId])
    REFERENCES [dbo].[Skills] ([SkillId])
    ON DELETE CASCADE;
GO

-- Resumes -> Candidates
ALTER TABLE [dbo].[Resumes]
    ADD CONSTRAINT FK_Resumes_Candidates FOREIGN KEY ([CandidateId])
    REFERENCES [dbo].[Candidates] ([CandidateId])
    ON DELETE CASCADE;
GO

-- Applications -> Candidates
ALTER TABLE [dbo].[Applications]
    ADD CONSTRAINT FK_Applications_Candidates FOREIGN KEY ([CandidateId])
    REFERENCES [dbo].[Candidates] ([CandidateId])
    ON DELETE CASCADE;
GO

-- Applications -> Jobs
ALTER TABLE [dbo].[Applications]
    ADD CONSTRAINT FK_Applications_Jobs FOREIGN KEY ([JobId])
    REFERENCES [dbo].[Jobs] ([JobId])
    ON DELETE CASCADE;
GO

-- Interviews -> Applications
ALTER TABLE [dbo].[Interviews]
    ADD CONSTRAINT FK_Interviews_Applications FOREIGN KEY ([ApplicationId])
    REFERENCES [dbo].[Applications] ([ApplicationId])
    ON DELETE CASCADE;
GO

-- Evaluations -> Interviews
ALTER TABLE [dbo].[Evaluations]
    ADD CONSTRAINT FK_Evaluations_Interviews FOREIGN KEY ([InterviewId])
    REFERENCES [dbo].[Interviews] ([InterviewId])
    ON DELETE CASCADE;
GO

-- Evaluations -> Recruiters
ALTER TABLE [dbo].[Evaluations]
    ADD CONSTRAINT FK_Evaluations_Recruiters FOREIGN KEY ([RecruiterId])
    REFERENCES [dbo].[Recruiters] ([RecruiterId])
    ON DELETE RESTRICT;
GO

-- Notifications -> Users
ALTER TABLE [dbo].[Notifications]
    ADD CONSTRAINT FK_Notifications_Users FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users] ([UserId])
    ON DELETE CASCADE;
GO

PRINT 'All relationships created successfully.';
GO
