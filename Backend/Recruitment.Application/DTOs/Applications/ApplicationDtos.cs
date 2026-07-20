using System;

namespace Recruitment.Application.DTOs.Applications
{
    public record CreateApplicationDto(
        Guid JobId
    );


    public record ApplicationListDto(
        Guid ApplicationId,
        Guid JobId,
        string JobTitle,
        string CompanyName,
        string Status,
        DateTime AppliedDate
    );


    public record ApplicationDetailDto(
        Guid ApplicationId,
        Guid CandidateId,
        string CandidateFullName,
        string CandidateEmail,
        Guid JobId,
        string JobTitle,
        string Status,
        DateTime AppliedDate
    );


    public record UpdateApplicationStatusDto(
        string Status
    );
}