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



        public async Task<IEnumerable<ApplicationEntity>> GetApplicationsByJobAsync(Guid jobId)
        {
            return await _applicationRepository.GetByJobIdAsync(jobId);
        }



        public async Task<IEnumerable<ApplicationEntity>> GetApplicationsByCandidateAsync(Guid candidateId)
        {
            return await _applicationRepository.GetByCandidateIdAsync(candidateId);
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