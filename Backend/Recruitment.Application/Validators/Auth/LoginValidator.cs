using FluentValidation;
using Recruitment.Application.DTOs.Authentication;

namespace Recruitment.Application.Validators.Auth
{
    public class LoginValidator 
        : AbstractValidator<LoginRequestDto>
    {

        public LoginValidator()
        {

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Invalid email format");


            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(8)
                .WithMessage(
                "Password must contain minimum 8 characters");

        }

    }
}
