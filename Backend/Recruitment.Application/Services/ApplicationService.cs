using ApplicationEntity = Recruitment.Domain.Entities.Application;
using Recruitment.Application.DTOs.Applications;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;

namespace Recruitment.Application.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;


        public ApplicationService(
            IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }



        public async Task<IEnumerable<ApplicationEntity>> GetAllApplicationsAsync()
        {
            return await _applicationRepository.GetAllAsync();
        }



        public async Task<ApplicationEntity?> GetApplicationByIdAsync(Guid id)
        {
            return await _applicationRepository.GetByIdAsync(id);
        }



        public async Task<IEnumerable<ApplicationDetailDto>> GetApplicationsByJobAsync(Guid jobId)
        {
            var applications = await _applicationRepository.GetByJobIdAsync(jobId);

            return applications.Select(a => new ApplicationDetailDto(
                a.Id,
                a.CandidateId,
                a.Candidate == null
                    ? string.Empty
                    : $"{a.Candidate.FirstName} {a.Candidate.LastName}".Trim(),
                a.Candidate?.User?.Email ?? string.Empty,
                a.JobId,
                a.Job?.Title ?? string.Empty,
                a.Status,
                a.AppliedDate));
        }



        public async Task<IEnumerable<ApplicationListDto>> GetApplicationsByCandidateAsync(Guid candidateId)
        {
            var applications = await _applicationRepository.GetByCandidateIdAsync(candidateId);

            return applications.Select(a => new ApplicationListDto(
                a.Id,
                a.JobId,
                a.Job?.Title ?? string.Empty,
                a.Job?.Company?.Name ?? string.Empty,
                a.Status,
                a.AppliedDate));
        }



       public async Task<Guid> ApplyToJobAsync(Guid candidateId, CreateApplicationDto dto)
{
    var application = new ApplicationEntity
    {
        Id = Guid.NewGuid(),
        CandidateId = candidateId,
        JobId = dto.JobId,
        AppliedDate = DateTime.UtcNow,
        Status = "Pending"
    };

    await _applicationRepository.AddAsync(application);

    return application.Id;
}



        public async Task UpdateStatusAsync(Guid applicationId, UpdateApplicationStatusDto dto)
        {
            var application = await _applicationRepository.GetByIdAsync(applicationId);

            if (application == null)
                throw new Exception("Application not found");


            application.Status = dto.Status;

            await _applicationRepository.UpdateAsync(application);
        }



        public async Task UpdateApplicationAsync(ApplicationEntity application)
        {
            await _applicationRepository.UpdateAsync(application);
        }



        public async Task DeleteApplicationAsync(Guid id)
        {
            await _applicationRepository.DeleteAsync(id);
        }
    }
}
