using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Recruitment.Persistence.Context;
using Recruitment.Domain.Entities;

namespace Recruitment.API.Utilities
{
    public static class DatabaseSeeder
    {
        public static void Seed(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            try
            {
                db.Database.EnsureCreated();

                if (!db.Roles.Any())
                {
                    var adminRole = new Role { Id = Guid.NewGuid(), Name = "Admin" };
                    var recruiterRole = new Role { Id = Guid.NewGuid(), Name = "Recruiter" };
                    var candidateRole = new Role { Id = Guid.NewGuid(), Name = "Candidate" };
                    db.Roles.AddRange(adminRole, recruiterRole, candidateRole);
                    db.SaveChanges();
                    logger.LogInformation("Seeded roles.");
                }

                if (!db.Users.Any(u => u.Email == "admin@local"))
                {
                    var adminRole = db.Roles.FirstOrDefault(r => r.Name == "Admin");
                    var user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = "admin@local",
                        FullName = "Administrator",
                        RoleId = adminRole?.Id ?? Guid.Empty
                    };
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("P@ssw0rd!");
                    db.Users.Add(user);
                    db.SaveChanges();
                    logger.LogInformation("Seeded admin user.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while seeding database");
                throw;
            }
        }
    }
}
