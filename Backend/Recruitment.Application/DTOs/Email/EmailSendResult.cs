namespace Recruitment.Application.DTOs.Email
{
    public record EmailSendResult(
        bool Sent,
        string Message,
        string? Provider = null
    );
}
