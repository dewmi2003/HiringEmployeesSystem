using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Recruitment.Domain.Entities;
using Recruitment.Persistence.Context;
using ApplicationEntity = Recruitment.Domain.Entities.Application;

namespace Recruitment.API.Utilities
{
    public static class DatabaseSeeder
    {
        private const int DemoRecordCount = 100;
        private const string DemoPassword = "P@ssw0rd!";

        public static void Seed(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            try
            {
                db.Database.Migrate();

                SeedRoles(db, logger);
                SeedAdmin(db, logger);

                if (configuration.GetValue("SeedDemoData", true))
                {
                    SeedDemoData(db, logger);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while seeding database");
                throw;
            }
        }

        private static void SeedRoles(ApplicationDbContext db, ILogger logger)
        {
            var roleNames = new[] { "Admin", "Recruiter", "Candidate" };

            foreach (var roleName in roleNames)
            {
                if (!db.Roles.Any(r => r.Name == roleName))
                {
                    db.Roles.Add(new Role { Id = Guid.NewGuid(), Name = roleName });
                }
            }

            db.SaveChanges();
            logger.LogInformation("Roles are ready.");
        }

        private static void SeedAdmin(ApplicationDbContext db, ILogger logger)
        {
            if (db.Users.Any(u => u.Email == "admin@local"))
            {
                return;
            }

            var adminRole = db.Roles.First(r => r.Name == "Admin");
            db.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@local",
                FullName = "Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
                CreatedDate = DateTime.UtcNow,
                RoleId = adminRole.Id
            });

            db.SaveChanges();
            logger.LogInformation("Seeded admin user admin@local.");
        }

        private static void SeedDemoData(ApplicationDbContext db, ILogger logger)
        {
            var candidateRole = db.Roles.First(r => r.Name == "Candidate");
            var recruiterRole = db.Roles.First(r => r.Name == "Recruiter");
            var adminUser = db.Users.First(u => u.Email == "admin@local");

            SeedCompanies(db);
            SeedUsersAndProfiles(db, candidateRole.Id, recruiterRole.Id);
            SeedSkills(db);
            SeedCandidateSkills(db);
            SeedJobs(db);
            SeedApplications(db);
            SeedResumes(db);
            SeedInterviews(db);
            SeedEvaluations(db, adminUser.Id);
            SeedStatusHistories(db);
            SeedHiringDecisions(db);
            SeedNotifications(db);
            SeedAuditLogs(db, adminUser.Id);
            SeedReports(db);

            logger.LogInformation("Demo data is ready: {Count} rows for the main workflow tables.", DemoRecordCount);
        }

        private static void SeedCompanies(ApplicationDbContext db)
        {
            var existing = db.Companies
                .Where(c => c.Name.StartsWith("Demo Company "))
                .GroupBy(c => c.Name)
                .ToDictionary(g => g.Key, g => g.First());

            for (var i = 1; i <= DemoRecordCount; i++)
            {
                var name = DemoName("Demo Company", i);
                if (existing.ContainsKey(name))
                {
                    continue;
                }

                db.Companies.Add(new Company
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Description = $"Demo company {i:000} for recruitment workflow testing.",
                    Website = $"https://demo-company-{i:000}.local",
                    Address = $"{i:000} Demo Business Avenue"
                });
            }

            db.SaveChanges();
        }

        private static void SeedUsersAndProfiles(ApplicationDbContext db, Guid candidateRoleId, Guid recruiterRoleId)
        {
            SeedDemoUsers(db, "demo.candidate", "Demo Candidate", candidateRoleId);
            SeedDemoUsers(db, "demo.recruiter", "Demo Recruiter", recruiterRoleId);

            var companies = GetDemoCompanies(db);
            var candidateUsers = GetDemoUsers(db, "demo.candidate");
            var recruiterUsers = GetDemoUsers(db, "demo.recruiter");

            var existingCandidates = db.Candidates
                .Where(c => candidateUsers.Select(u => u.Id).Contains(c.UserId))
                .GroupBy(c => c.UserId)
                .ToDictionary(g => g.Key, g => g.First());

            var existingRecruiters = db.Recruiters
                .Where(r => recruiterUsers.Select(u => u.Id).Contains(r.UserId))
                .GroupBy(r => r.UserId)
                .ToDictionary(g => g.Key, g => g.First());

            for (var i = 1; i <= DemoRecordCount; i++)
            {
                var candidateUser = candidateUsers[i - 1];
                if (!existingCandidates.ContainsKey(candidateUser.Id))
                {
                    db.Candidates.Add(new Candidate
                    {
                        Id = Guid.NewGuid(),
                        UserId = candidateUser.Id,
                        FirstName = "Demo",
                        LastName = $"Candidate {i:000}",
                        Phone = $"070000{i:000}",
                        Address = $"{i:000} Candidate Lane",
                        Bio = $"Candidate profile {i:000} seeded for local API testing.",
                        Experience = $"{1 + i % 8} years in software and business operations.",
                        Education = i % 2 == 0 ? "BSc Computer Science" : "Higher Diploma in Information Technology"
                    });
                }

                var recruiterUser = recruiterUsers[i - 1];
                if (!existingRecruiters.ContainsKey(recruiterUser.Id))
                {
                    db.Recruiters.Add(new Recruiter
                    {
                        Id = Guid.NewGuid(),
                        UserId = recruiterUser.Id,
                        FirstName = "Demo",
                        LastName = $"Recruiter {i:000}",
                        CompanyId = companies[i - 1].Id
                    });
                }
            }

            db.SaveChanges();
        }

        private static void SeedDemoUsers(ApplicationDbContext db, string prefix, string displayName, Guid roleId)
        {
            var existing = db.Users
                .Where(u => u.Email.StartsWith(prefix) && u.Email.EndsWith("@local"))
                .GroupBy(u => u.Email)
                .ToDictionary(g => g.Key, g => g.First());

            for (var i = 1; i <= DemoRecordCount; i++)
            {
                var email = $"{prefix}{i:000}@local";
                if (existing.ContainsKey(email))
                {
                    continue;
                }

                db.Users.Add(new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    FullName = $"{displayName} {i:000}",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(DemoPassword),
                    CreatedDate = DateTime.UtcNow.AddDays(-i),
                    RoleId = roleId
                });
            }

            db.SaveChanges();
        }

        private static void SeedSkills(ApplicationDbContext db)
        {
            var skillNames = new[]
            {
                ".NET", "C#", "ASP.NET Core", "SQL Server", "Entity Framework", "Azure", "React",
                "Angular", "JavaScript", "TypeScript", "REST APIs", "Docker", "CI/CD", "Git",
                "Testing", "Agile", "UI Design", "Business Analysis", "Cloud Storage", "OpenAI APIs"
            };

            var existing = db.Skills
                .Where(s => s.Name.StartsWith("Demo Skill "))
                .GroupBy(s => s.Name)
                .ToDictionary(g => g.Key, g => g.First());

            for (var i = 1; i <= DemoRecordCount; i++)
            {
                var name = $"Demo Skill {i:000} - {skillNames[(i - 1) % skillNames.Length]}";
                if (existing.ContainsKey(name))
                {
                    continue;
                }

                db.Skills.Add(new Skill { Id = Guid.NewGuid(), Name = name });
            }

            db.SaveChanges();
        }

        private static void SeedCandidateSkills(ApplicationDbContext db)
        {
            var candidates = GetDemoCandidates(db);
            var skills = GetDemoSkills(db);
            var existing = db.CandidateSkills
                .Where(cs => candidates.Select(c => c.Id).Contains(cs.CandidateId))
                .Select(cs => new { cs.CandidateId, cs.SkillId })
                .ToHashSet();

            for (var i = 0; i < DemoRecordCount; i++)
            {
                var candidateId = candidates[i].Id;
                var skillId = skills[i].Id;

                if (existing.Contains(new { CandidateId = candidateId, SkillId = skillId }))
                {
                    continue;
                }

                db.CandidateSkills.Add(new CandidateSkill
                {
                    CandidateId = candidateId,
                    SkillId = skillId
                });
            }

            db.SaveChanges();
        }

        private static void SeedJobs(ApplicationDbContext db)
        {
            var companies = GetDemoCompanies(db);
            var recruiters = GetDemoRecruiters(db);
            var existing = db.Jobs
                .Where(j => j.Title.StartsWith("Demo Job "))
                .GroupBy(j => j.Title)
                .ToDictionary(g => g.Key, g => g.First());

            for (var i = 1; i <= DemoRecordCount; i++)
            {
                var title = $"Demo Job {i:000} - {JobTitle(i)}";
                if (existing.ContainsKey(title))
                {
                    continue;
                }

                db.Jobs.Add(new Job
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    Description = $"Seeded job {i:000} used for local Swagger and API workflow testing.",
                    Requirements = "Communication, problem solving, clean implementation, and reliable documentation.",
                    Department = Department(i),
                    PostedDate = DateTime.UtcNow.AddDays(-i),
                    CreatedDate = DateTime.UtcNow.AddDays(-i),
                    Salary = 65000 + (i * 250),
                    Location = i % 3 == 0 ? "Remote" : i % 3 == 1 ? "Colombo" : "Hybrid",
                    Status = i % 10 == 0 ? "Closed" : "Active",
                    CompanyId = companies[i - 1].Id,
                    RecruiterId = recruiters[i - 1].Id
                });
            }

            db.SaveChanges();
        }

        private static void SeedApplications(ApplicationDbContext db)
        {
            var candidates = GetDemoCandidates(db);
            var jobs = GetDemoJobs(db);
            var existing = db.Applications
                .Where(a => candidates.Select(c => c.Id).Contains(a.CandidateId))
                .Select(a => new { a.CandidateId, a.JobId })
                .ToHashSet();

            var statuses = new[] { "Pending", "Shortlisted", "Rejected", "OfferExtended", "Hired", "Withdrawn" };

            for (var i = 0; i < DemoRecordCount; i++)
            {
                var candidateId = candidates[i].Id;
                var jobId = jobs[i].Id;

                if (existing.Contains(new { CandidateId = candidateId, JobId = jobId }))
                {
                    continue;
                }

                db.Applications.Add(new ApplicationEntity
                {
                    Id = Guid.NewGuid(),
                    CandidateId = candidateId,
                    JobId = jobId,
                    AppliedDate = DateTime.UtcNow.AddDays(-(i + 1)),
                    Status = statuses[i % statuses.Length]
                });
            }

            db.SaveChanges();
        }

        private static void SeedResumes(ApplicationDbContext db)
        {
            var candidates = GetDemoCandidates(db);
            var existing = db.Resumes
                .IgnoreQueryFilters()
                .Where(r => r.FileName.StartsWith("demo-resume-"))
                .GroupBy(r => r.CandidateId)
                .ToDictionary(g => g.Key, g => g.First());

            var resumeDirectory = Path.Combine(Directory.GetCurrentDirectory(), "blobs", "demo-resumes");
            Directory.CreateDirectory(resumeDirectory);

            for (var i = 1; i <= DemoRecordCount; i++)
            {
                var candidate = candidates[i - 1];
                if (existing.ContainsKey(candidate.Id))
                {
                    continue;
                }

                var fileName = $"demo-resume-{i:000}.txt";
                var filePath = Path.Combine(resumeDirectory, fileName);
                var parsedText = $"Demo resume {i:000}. Skills include API development, SQL, testing, and hiring workflow support.";

                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, parsedText);
                }

                db.Resumes.Add(new Resume
                {
                    Id = Guid.NewGuid(),
                    CandidateId = candidate.Id,
                    FilePath = filePath,
                    FileName = fileName,
                    FileSize = new FileInfo(filePath).Length,
                    FileType = "text/plain",
                    ParsedText = parsedText,
                    AiScore = 55 + i % 41,
                    Version = 1,
                    IsActive = true,
                    IsDeleted = false,
                    UploadedDate = DateTime.UtcNow.AddDays(-i),
                    CreatedAt = DateTime.UtcNow.AddDays(-i)
                });
            }

            db.SaveChanges();
        }

        private static void SeedInterviews(ApplicationDbContext db)
        {
            var applications = GetDemoApplications(db);
            var existing = db.Interviews
                .Where(i => applications.Select(a => a.Id).Contains(i.ApplicationId))
                .GroupBy(i => i.ApplicationId)
                .ToDictionary(g => g.Key, g => g.First());

            var statuses = new[] { "Scheduled", "Completed", "Pending", "Cancelled" };

            for (var i = 0; i < DemoRecordCount; i++)
            {
                var application = applications[i];
                if (existing.ContainsKey(application.Id))
                {
                    continue;
                }

                db.Interviews.Add(new Interview
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = application.Id,
                    Status = statuses[i % statuses.Length],
                    ScheduledDate = DateTime.UtcNow.AddDays(i + 1).AddHours(9 + i % 6),
                    Location = i % 2 == 0 ? "Microsoft Teams" : "Office Meeting Room"
                });
            }

            db.SaveChanges();
        }

        private static void SeedEvaluations(ApplicationDbContext db, Guid adminUserId)
        {
            var applications = GetDemoApplications(db);
            var interviews = GetDemoInterviews(db);
            var recruiters = GetDemoRecruiters(db);
            var existing = db.Evaluations
                .Where(e => interviews.Select(i => i.Id).Contains(e.InterviewId))
                .GroupBy(e => e.InterviewId)
                .ToDictionary(g => g.Key, g => g.First());

            for (var i = 0; i < DemoRecordCount; i++)
            {
                var interview = interviews[i];
                if (existing.ContainsKey(interview.Id))
                {
                    continue;
                }

                var application = applications[i];
                var technical = 55 + i % 41;
                var communication = 58 + i % 38;
                var experience = 52 + i % 44;
                var culture = 60 + i % 36;
                var overall = Math.Round((technical + communication + experience + culture) / 4.0, 2);

                db.Evaluations.Add(new Evaluation
                {
                    Id = Guid.NewGuid(),
                    InterviewId = interview.Id,
                    InterviewerId = recruiters[i].Id,
                    ApplicationId = application.Id,
                    CandidateId = application.CandidateId,
                    HiringManagerId = adminUserId,
                    TechnicalScore = technical,
                    CommunicationScore = communication,
                    ExperienceScore = experience,
                    CultureFitScore = culture,
                    OverallScore = overall,
                    Recommendation = overall >= 80 ? "StrongHire" : overall >= 65 ? "Hire" : "Review",
                    Score = (int)Math.Round(overall),
                    Notes = $"Seeded evaluation {i + 1:000} for local workflow testing.",
                    CreatedAt = DateTime.UtcNow.AddDays(-i)
                });
            }

            db.SaveChanges();
        }

        private static void SeedStatusHistories(ApplicationDbContext db)
        {
            var applications = GetDemoApplications(db);
            var recruiterUsers = GetDemoUsers(db, "demo.recruiter");
            var existing = db.ApplicationStatusHistories
                .Where(h => applications.Select(a => a.Id).Contains(h.ApplicationId))
                .GroupBy(h => h.ApplicationId)
                .ToDictionary(g => g.Key, g => g.First());

            for (var i = 0; i < DemoRecordCount; i++)
            {
                var application = applications[i];
                if (existing.ContainsKey(application.Id))
                {
                    continue;
                }

                db.ApplicationStatusHistories.Add(new ApplicationStatusHistory
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = application.Id,
                    OldStatus = "Pending",
                    NewStatus = application.Status,
                    ChangedByUserId = recruiterUsers[i].Id,
                    ChangedAt = DateTime.UtcNow.AddDays(-i),
                    Comments = $"Seeded status history {i + 1:000}."
                });
            }

            db.SaveChanges();
        }

        private static void SeedHiringDecisions(ApplicationDbContext db)
        {
            var applications = GetDemoApplications(db);
            var recruiterUsers = GetDemoUsers(db, "demo.recruiter");
            var existing = db.HiringDecisions
                .Where(d => applications.Select(a => a.Id).Contains(d.ApplicationId))
                .GroupBy(d => d.ApplicationId)
                .ToDictionary(g => g.Key, g => g.First());

            for (var i = 0; i < DemoRecordCount; i++)
            {
                var application = applications[i];
                if (existing.ContainsKey(application.Id))
                {
                    continue;
                }

                db.HiringDecisions.Add(new HiringDecision
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = application.Id,
                    DecidedByUserId = recruiterUsers[i].Id,
                    Decision = application.Status switch
                    {
                        "Hired" => "Hired",
                        "Rejected" => "Rejected",
                        "Withdrawn" => "Withdrawn",
                        _ => "PendingReview"
                    },
                    Comments = $"Seeded hiring decision {i + 1:000}.",
                    DecidedAt = DateTime.UtcNow.AddDays(-i)
                });
            }

            db.SaveChanges();
        }

        private static void SeedNotifications(ApplicationDbContext db)
        {
            var candidateUsers = GetDemoUsers(db, "demo.candidate");
            var existing = db.Notifications
                .Where(n => n.Title.StartsWith("Demo Notification "))
                .GroupBy(n => n.Title)
                .ToDictionary(g => g.Key, g => g.First());

            for (var i = 1; i <= DemoRecordCount; i++)
            {
                var title = $"Demo Notification {i:000}";
                if (existing.ContainsKey(title))
                {
                    continue;
                }

                db.Notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = candidateUsers[i - 1].Id,
                    Title = title,
                    Message = $"Your seeded application update {i:000} is ready.",
                    Type = i % 2 == 0 ? "Application" : "Interview",
                    IsRead = i % 3 == 0,
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    ReadAt = i % 3 == 0 ? DateTime.UtcNow.AddDays(-(i - 1)) : null
                });
            }

            db.SaveChanges();
        }

        private static void SeedAuditLogs(ApplicationDbContext db, Guid adminUserId)
        {
            var applications = GetDemoApplications(db);
            var existing = db.AuditLogs
                .Where(a => a.Action.StartsWith("Seeded demo action "))
                .GroupBy(a => a.Action)
                .ToDictionary(g => g.Key, g => g.First());

            for (var i = 1; i <= DemoRecordCount; i++)
            {
                var action = $"Seeded demo action {i:000}";
                if (existing.ContainsKey(action))
                {
                    continue;
                }

                db.AuditLogs.Add(new AuditLog
                {
                    Id = Guid.NewGuid(),
                    UserId = adminUserId,
                    Action = action,
                    EntityName = "Application",
                    EntityId = applications[i - 1].Id,
                    IpAddress = "127.0.0.1",
                    CreatedAt = DateTime.UtcNow.AddDays(-i)
                });
            }

            db.SaveChanges();
        }

        private static void SeedReports(ApplicationDbContext db)
        {
            var existing = db.Reports
                .Where(r => r.Title.StartsWith("Demo Recruitment Report "))
                .GroupBy(r => r.Title)
                .ToDictionary(g => g.Key, g => g.First());

            for (var i = 1; i <= DemoRecordCount; i++)
            {
                var title = $"Demo Recruitment Report {i:000}";
                if (existing.ContainsKey(title))
                {
                    continue;
                }

                db.Reports.Add(new Report
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    Data = JsonSerializer.Serialize(new
                    {
                        month = DateTime.UtcNow.AddMonths(-(i % 12)).ToString("yyyy-MM"),
                        jobs = DemoRecordCount,
                        applications = DemoRecordCount + i,
                        interviews = Math.Max(1, DemoRecordCount - i / 2),
                        hires = i % 17
                    }),
                    CreatedAt = DateTime.UtcNow.AddDays(-i)
                });
            }

            db.SaveChanges();
        }

        private static List<Company> GetDemoCompanies(ApplicationDbContext db)
        {
            return db.Companies
                .Where(c => c.Name.StartsWith("Demo Company "))
                .OrderBy(c => c.Name)
                .Take(DemoRecordCount)
                .ToList();
        }

        private static List<User> GetDemoUsers(ApplicationDbContext db, string prefix)
        {
            return db.Users
                .Where(u => u.Email.StartsWith(prefix) && u.Email.EndsWith("@local"))
                .OrderBy(u => u.Email)
                .Take(DemoRecordCount)
                .ToList();
        }

        private static List<Candidate> GetDemoCandidates(ApplicationDbContext db)
        {
            var userIds = GetDemoUsers(db, "demo.candidate").Select(u => u.Id).ToList();
            return db.Candidates
                .Where(c => userIds.Contains(c.UserId))
                .OrderBy(c => c.User!.Email)
                .Take(DemoRecordCount)
                .ToList();
        }

        private static List<Recruiter> GetDemoRecruiters(ApplicationDbContext db)
        {
            var userIds = GetDemoUsers(db, "demo.recruiter").Select(u => u.Id).ToList();
            return db.Recruiters
                .Where(r => userIds.Contains(r.UserId))
                .OrderBy(r => r.User!.Email)
                .Take(DemoRecordCount)
                .ToList();
        }

        private static List<Skill> GetDemoSkills(ApplicationDbContext db)
        {
            return db.Skills
                .Where(s => s.Name.StartsWith("Demo Skill "))
                .OrderBy(s => s.Name)
                .Take(DemoRecordCount)
                .ToList();
        }

        private static List<Job> GetDemoJobs(ApplicationDbContext db)
        {
            return db.Jobs
                .Where(j => j.Title.StartsWith("Demo Job "))
                .OrderBy(j => j.Title)
                .Take(DemoRecordCount)
                .ToList();
        }

        private static List<ApplicationEntity> GetDemoApplications(ApplicationDbContext db)
        {
            var candidateIds = GetDemoCandidates(db).Select(c => c.Id).ToList();
            return db.Applications
                .Where(a => candidateIds.Contains(a.CandidateId))
                .OrderBy(a => a.AppliedDate)
                .Take(DemoRecordCount)
                .ToList();
        }

        private static List<Interview> GetDemoInterviews(ApplicationDbContext db)
        {
            var applicationIds = GetDemoApplications(db).Select(a => a.Id).ToList();
            return db.Interviews
                .Where(i => applicationIds.Contains(i.ApplicationId))
                .OrderBy(i => i.ScheduledDate)
                .Take(DemoRecordCount)
                .ToList();
        }

        private static string DemoName(string prefix, int number)
        {
            return $"{prefix} {number:000}";
        }

        private static string JobTitle(int number)
        {
            var titles = new[]
            {
                "Backend Developer", "Frontend Developer", "QA Engineer", "DevOps Engineer",
                "Business Analyst", "Product Designer", "Data Analyst", "HR Coordinator"
            };

            return titles[(number - 1) % titles.Length];
        }

        private static string Department(int number)
        {
            var departments = new[] { "Engineering", "Product", "Operations", "People", "Data" };
            return departments[(number - 1) % departments.Length];
        }
    }
}
