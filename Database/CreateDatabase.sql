-- ============================================================
-- File: CreateDatabase.sql
-- Purpose: Create the RecruitmentDB Azure SQL Database
-- ============================================================

USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'RecruitmentDB')
BEGIN
    CREATE DATABASE [RecruitmentDB]
    COLLATE SQL_Latin1_General_CP1_CI_AS;
END
GO

USE [RecruitmentDB];
GO

PRINT 'RecruitmentDB created successfully.';
GO
