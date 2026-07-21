using FluentValidation;
using Recruitment.Application.DTOs.Applications;


namespace Recruitment.Application.Validators
{

public class ApplicationValidator
: AbstractValidator<CreateApplicationDto>
{


public ApplicationValidator()
{


RuleFor(x=>x.JobId)
.NotEmpty();


}

}

}
