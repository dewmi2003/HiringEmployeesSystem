using Recruitment.Application.DTOs.Interviews;
using Recruitment.Application.Interfaces.Repositories;
using Recruitment.Application.Interfaces.Services;
using Recruitment.Domain.Entities;

namespace Recruitment.Application.Services
{
    public class InterviewService : IInterviewService
    {
        private readonly IInterviewRepository _interviewRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly ICalendarService _calendarService;

        public InterviewService(
            IInterviewRepository interviewRepository,
            IApplicationRepository applicationRepository,
            ICalendarService calendarService)
        {
            _interviewRepository = interviewRepository;
            _applicationRepository = applicationRepository;
            _calendarService = calendarService;
        }

        public async Task<ScheduledInterviewDto> ScheduleInterviewAsync(CreateInterviewDto dto)
        {
            var application = await _applicationRepository.GetByIdAsync(dto.ApplicationId)
                ?? throw new ArgumentException("Application not found.");

            var start = dto.InterviewDate;
            var end = start.AddHours(1);

            var isAvailable = await _calendarService.CheckAvailabilityAsync(start, end);
            if (!isAvailable)
            {
                throw new InvalidOperationException("Calendar is not available for the selected interview time.");
            }

            var candidateName = application.Candidate == null
                ? string.Empty
                : $"{application.Candidate.FirstName} {application.Candidate.LastName}".Trim();
            var candidateEmail = application.Candidate?.User?.Email ?? string.Empty;
            var jobTitle = application.Job?.Title ?? "Interview";

            var calendarResult = await _calendarService.CreateEventAsync(
                $"Interview: {jobTitle}",
                start,
                end,
                candidateEmail);

            var interview = new Interview
            {
                Id = Guid.NewGuid(),
                ApplicationId = application.Id,
                ScheduledDate = start,
                Location = dto.Location ?? string.Empty,
                Status = "Scheduled"
            };

            await _interviewRepository.AddAsync(interview);

            return new ScheduledInterviewDto(
                interview.Id,
                interview.ApplicationId,
                candidateName,
                candidateEmail,
                jobTitle,
                interview.ScheduledDate,
                end,
                interview.Location,
                interview.Status,
                calendarResult);
        }

        public async Task<IEnumerable<InterviewDto>> GetAllAsync()
        {
            var interviews = await _interviewRepository.GetAllAsync();

            return interviews.Select(i => new InterviewDto(
                i.Id,
                i.ApplicationId,
                i.Application?.Candidate == null
                    ? string.Empty
                    : $"{i.Application.Candidate.FirstName} {i.Application.Candidate.LastName}".Trim(),
                i.Application?.Job?.Title ?? string.Empty,
                i.ScheduledDate,
                i.Location,
                i.Status));
        }
    }
}
