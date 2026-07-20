namespace Recruitment.Application.DTOs.Jobs
{
    public record CreateJobDto(
        Guid CompanyId,
        string Title,
        string Description,
        string Requirements,
        decimal? Salary,
        string? Location
    );

    public record UpdateJobDto(
        string? Title,
        string? Description,
        string? Requirements,
        decimal? Salary,
        string? Location,
        string? Status
    );

    public record JobListDto(
        Guid JobId,
        string Title,
        string? Location,
        decimal? Salary,
        string Status,
        string CompanyName,
        DateTime CreatedDate
    );

    public record JobDetailDto(
        Guid JobId,
        string Title,
        string Description,
        string Requirements,
        decimal? Salary,
        string? Location,
        string Status,
        string CompanyName,
        string? CompanyWebsite,
        DateTime CreatedDate
    );
}
