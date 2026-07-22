using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace Recruitment.Persistence.Context
{
    public class DesignTimeApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? ReadLocalConnectionString()
                ?? "Server=(localdb)\\mssqllocaldb;Database=RecruitmentDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 6,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null));

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        private static string? ReadLocalConnectionString()
        {
            foreach (var path in GetLocalSettingsPaths())
            {
                if (!File.Exists(path))
                {
                    continue;
                }

                using var document = JsonDocument.Parse(File.ReadAllText(path));
                if (document.RootElement.TryGetProperty("ConnectionStrings", out var connectionStrings)
                    && connectionStrings.TryGetProperty("DefaultConnection", out var defaultConnection))
                {
                    return defaultConnection.GetString();
                }
            }

            return null;
        }

        private static IEnumerable<string> GetLocalSettingsPaths()
        {
            var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            while (currentDirectory is not null)
            {
                yield return Path.Combine(currentDirectory.FullName, "appsettings.Local.json");
                yield return Path.Combine(currentDirectory.FullName, "Recruitment.API", "appsettings.Local.json");
                yield return Path.Combine(currentDirectory.FullName, "Backend", "Recruitment.API", "appsettings.Local.json");
                yield return Path.GetFullPath(Path.Combine(currentDirectory.FullName, "..", "Recruitment.API", "appsettings.Local.json"));

                currentDirectory = currentDirectory.Parent;
            }
        }
    }
}
