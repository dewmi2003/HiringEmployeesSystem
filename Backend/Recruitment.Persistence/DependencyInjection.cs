using Microsoft.Extensions.DependencyInjection;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Persistence.Repositories;
using Recruitment.Persistence.Context;

namespace Recruitment.Persistence
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddPersistence(
            this IServiceCollection services)
        {

            services.AddScoped<
                IApplicationRepository,
                ApplicationRepository>();


            services.AddScoped<
                ICompanyRepository,
                CompanyRepository>();


            services.AddScoped<
                IJobRepository,
                JobRepository>();


            services.AddScoped<
                ICandidateRepository,
                CandidateRepository>();


            services.AddScoped<
                IRecruiterRepository,
                RecruiterRepository>();


            return services;
        }

    }
}