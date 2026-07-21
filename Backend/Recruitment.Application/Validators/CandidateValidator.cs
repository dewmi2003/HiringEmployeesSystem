using FluentValidation;
using Recruitment.Application.DTOs.Candidates;


namespace Recruitment.Application.Validators
{

public class CandidateValidator
: AbstractValidator<UpdateCandidateProfileDto>
{


public CandidateValidator()
{

RuleFor(x=>x.Phone)
.MaximumLength(20);



RuleFor(x=>x.Address)
.MaximumLength(200);



RuleFor(x=>x.Bio)
.MaximumLength(1000);


}

}

}
