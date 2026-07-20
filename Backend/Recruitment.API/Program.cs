using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Recruitment.Infrastructure.Services;
using Recruitment.Infrastructure.AI;
using Recruitment.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Recruitment.Application.Services;
using Recruitment.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Recruitment.Domain.Entities;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Persistence.Repositories;
var builder = WebApplication.CreateBuilder(args);

// Configuration & Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
    });


    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database
var conn = builder.Configuration.GetConnectionString("DefaultConnection")
           ?? builder.Configuration["ConnectionStrings:DefaultConnection"]
           ?? "Server=(localdb)\\mssqllocaldb;Database=RecruitmentDb;Trusted_Connection=True;MultipleActiveResultSets=true";
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(conn));

// Application services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Register repositories
builder.Services.AddScoped<IUserRepository, Recruitment.Persistence.Repositories.UserRepository>();
builder.Services.AddScoped<IJobRepository, JobRepository>();
builder.Services.AddScoped<IApplicationRepository, ApplicationRepository>();
builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IInterviewRepository, InterviewRepository>();
builder.Services.AddScoped<IResumeRepository, ResumeRepository>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IRecruiterRepository, RecruiterRepository>();
builder.Services.AddScoped<IEvaluationRepository, EvaluationRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();

// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJobService, JobService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IResumeAiService, ResumeAiService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

// Infrastructure services (email/blob)
var useAzure = builder.Configuration["Azure:Enabled"] == "true" || builder.Configuration["UseAzureServices"] == "true";
if (useAzure)
{
    builder.Services.AddSingleton<IEmailService, SendGridEmailService>();
    builder.Services.AddSingleton<IBlobStorage, AzureBlobStorage>();
}
else
{
    builder.Services.AddSingleton<IEmailService, MockEmailService>();
    builder.Services.AddSingleton<IBlobStorage, LocalBlobStorage>();
}

// AI Adapter selection: use Azure adapter when configured, otherwise local mock
var aiProvider = builder.Configuration["AI:Provider"] ?? "local";
if (aiProvider.Equals("azure", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddHttpClient<AzureOpenAiAdapter>();
    builder.Services.AddSingleton<IAiAdapter, AzureOpenAiAdapter>();
}
else
{
    builder.Services.AddSingleton<IAiAdapter, LocalAiAdapter>();
}

// Authentication - JWT
var jwtKey = builder.Configuration["Jwt__Key"] ?? "ChangeThisLocalKeyDontUseInProd0123456789";
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = !string.IsNullOrEmpty(builder.Configuration["Jwt__Issuer"]),
        ValidIssuer = builder.Configuration["Jwt__Issuer"],
        ValidateAudience = !string.IsNullOrEmpty(builder.Configuration["Jwt__Audience"]),
        ValidAudience = builder.Configuration["Jwt__Audience"],
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Remove or comment Serilog setup for now
// try
// {
//     Recruitment.Infrastructure.Logging.SerilogExtensions.ConfigureSerilog(builder.Host, builder.Configuration);
// }
// catch
// {
//     // ignore if Serilog not configured
// }

// Enable Swagger in all environments for development/testing
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<Recruitment.API.Middleware.GlobalExceptionMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapGet("/", () => "Recruitment API");
app.MapControllers();

// Seed database (roles, admin) - safe to run multiple times
try
{
    Recruitment.API.Utilities.DatabaseSeeder.Seed(app.Services);
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Seed");
    logger.LogError(ex, "Database seeding failed.");
}

app.Run();
