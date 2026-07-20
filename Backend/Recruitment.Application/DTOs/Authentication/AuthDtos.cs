namespace Recruitment.Application.DTOs.Authentication
{
    public record LoginRequestDto(string Email, string Password);

    public record RegisterRequestDto(
        string FullName,
        string Email,
        string Password,
        string Role   // "Candidate" or "Recruiter"
    );

    public record AuthResponseDto(
        string Token,
        string Email,
        string FullName,
        string Role,
        DateTime ExpiresAt
    );
}
