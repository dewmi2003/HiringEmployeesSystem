namespace Recruitment.Application.DTOs.Candidates
{
    public record CandidateProfileDto(
        Guid CandidateId,
        Guid UserId,
        string FullName,
        string Email,
        string? Phone,
        string? Address,
        string? Bio,
        string? Experience,
        string? Education,
        List<string> Skills
    );

    public record UpdateCandidateProfileDto(
        string? Phone,
        string? Address,
        string? Bio,
        string? Experience,
        string? Education
    );
}
