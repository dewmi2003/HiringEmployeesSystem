using FluentValidation;
using Recruitment.Application.DTOs.Interviews;


namespace Recruitment.Application.Validators
{

public class InterviewValidator
: AbstractValidator<CreateInterviewDto>
{


public InterviewValidator()
{


RuleFor(x=>x.ApplicationId)
.NotEmpty();



RuleFor(x=>x.InterviewDate)
.NotEmpty();



}

}

}
