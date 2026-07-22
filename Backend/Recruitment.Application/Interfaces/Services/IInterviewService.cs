using Recruitment.Application.DTOs.Interviews;

namespace Recruitment.Application.Interfaces.Services
{
    public interface IInterviewService
    {
        Task<ScheduledInterviewDto> ScheduleInterviewAsync(CreateInterviewDto dto);

        Task<IEnumerable<InterviewDto>> GetAllAsync();
    }
}
