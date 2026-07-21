using FluentValidation;
using Recruitment.Application.DTOs.Authentication;


namespace Recruitment.Application.Validators.Auth
{
    public class RegisterValidator
        : AbstractValidator<RegisterRequestDto>
    {


        public RegisterValidator()
        {

            RuleFor(x=>x.Email)
                .NotEmpty()
                .EmailAddress();



            RuleFor(x=>x.Password)
                .NotEmpty()
                .MinimumLength(8);



            RuleFor(x=>x.FullName)
                .NotEmpty()
                .MaximumLength(100);



            RuleFor(x=>x.Role)
                .NotEmpty();

        }


    }
}
