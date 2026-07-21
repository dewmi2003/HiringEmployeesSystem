using Microsoft.EntityFrameworkCore;
using Recruitment.Domain.Entities;

namespace Recruitment.Persistence.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Candidate> Candidates { get; set; } = null!;
        public DbSet<Recruiter> Recruiters { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<Job> Jobs { get; set; } = null!;
        public DbSet<Recruitment.Domain.Entities.Application> Applications { get; set; } = null!;
        public DbSet<Resume> Resumes { get; set; } = null!;
        public DbSet<Skill> Skills { get; set; } = null!;
        public DbSet<CandidateSkill> CandidateSkills { get; set; } = null!;
        public DbSet<Interview> Interviews { get; set; } = null!;
        public DbSet<Evaluation> Evaluations { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<HiringDecision> HiringDecisions { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<ApplicationStatusHistory> ApplicationStatusHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
