using FluentValidation;
using Recruitment.Application.DTOs.Authentication;


namespace Recruitment.Application.Validators.Auth
{

public class ChangePasswordValidator
: AbstractValidator<ChangePasswordDto>
{


public ChangePasswordValidator()
{

RuleFor(x=>x.CurrentPassword)
.NotEmpty();


RuleFor(x=>x.NewPassword)
.NotEmpty()
.MinimumLength(8)
.WithMessage(
"New password must have minimum 8 characters");

}


}

}
