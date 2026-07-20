# Database Guide — AI Recruitment Platform

## Database: RecruitmentDB (Azure SQL)

---

## Schema Overview

| Table | Description |
|---|---|
| `Roles` | User roles: Admin, Recruiter, Candidate |
| `Users` | System users with hashed passwords |
| `Candidates` | Candidate profiles linked to Users |
| `Recruiters` | Recruiter profiles linked to Users and Companies |
| `Companies` | Company profiles |
| `Jobs` | Job listings posted by Companies via Recruiters |
| `Skills` | Master skill list |
| `CandidateSkills` | Many-to-many: Candidates ↔ Skills |
| `Resumes` | Resume metadata + AI analysis results |
| `Applications` | Job applications (Candidate → Job) |
| `Interviews` | Interviews linked to Applications |
| `Evaluations` | Interview evaluations by Recruiters |
| `Notifications` | User notifications |
| `Reports` | Generated reports |

---

## Running SQL Scripts Manually

1. Open **Azure Data Studio** or **SQL Server Management Studio**
2. Connect to `sql-recruitment-server.database.windows.net`
3. Run scripts in order:

```bash
1. CreateDatabase.sql    # Create RecruitmentDB
2. Tables.sql            # Create all tables
3. Relationships.sql     # Add foreign key constraints
4. Indexes.sql           # Add performance indexes
5. StoredProcedures.sql  # Create stored procedures
6. SeedData.sql          # Insert initial data (first deploy only)
```

---

## EF Core Migrations

### Initial Setup
```bash
cd Backend

dotnet ef migrations add InitialCreate \
  --project Recruitment.Persistence \
  --startup-project Recruitment.API

dotnet ef database update \
  --project Recruitment.Persistence \
  --startup-project Recruitment.API
```

### Adding New Migrations
```bash
dotnet ef migrations add <MigrationName> \
  --project Recruitment.Persistence \
  --startup-project Recruitment.API
```

### Rollback Migration
```bash
dotnet ef database update <PreviousMigrationName> \
  --project Recruitment.Persistence \
  --startup-project Recruitment.API
```

---

## Resume File Strategy

Resume binary files are **not stored in SQL**. They are stored in **Azure Blob Storage**:

| Field | Stored In |
|---|---|
| File binary | Azure Blob (`resumecontainer`) |
| File URL | `Resumes.FilePath` column |
| Extracted text | `Resumes.ExtractedText` column |
| AI analysis output | `Resumes.AIAnalysisResult` column |
| AI match score | `Resumes.AiScore` column |

---

## Performance Indexes

| Index | Table | Purpose |
|---|---|---|
| `IX_Users_Email` | Users | Login lookups |
| `IX_Jobs_Title` | Jobs | Job search |
| `IX_Jobs_Location` | Jobs | Location filtering |
| `IX_Jobs_Status` | Jobs | Active job filtering |
| `IX_Applications_Status` | Applications | Pipeline dashboards |
| `IX_CandidateSkills_SkillId` | CandidateSkills | AI skill matching |
| `IX_Candidates_UserId` | Candidates | Profile lookups |
| `IX_Notifications_UserId_IsRead` | Notifications | Notification feed |
