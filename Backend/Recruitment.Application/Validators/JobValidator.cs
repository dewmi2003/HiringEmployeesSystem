using FluentValidation;
using Recruitment.Application.DTOs.Jobs;


namespace Recruitment.Application.Validators
{

public class JobValidator
: AbstractValidator<CreateJobDto>
{


public JobValidator()
{


RuleFor(x=>x.Title)
.NotEmpty()
.MaximumLength(200);



RuleFor(x=>x.Description)
.NotEmpty();



RuleFor(x=>x.Salary)
.GreaterThan(0)
.When(x=>x.Salary.HasValue)
.WithMessage(
"Salary must be greater than zero");



RuleFor(x=>x.CompanyId)
.NotEmpty();


}

}

}
