using FluentValidation;
using JWTAuthentication.ViewModels;

namespace JWTAuthentication.Validators;

public class LoginViewModelValidator : AbstractValidator<LoginViewModel>
{
    public LoginViewModelValidator() 
    {
        RuleFor(model => model.UsernameOrEmailInput)
            .NotEmpty().WithMessage("Username/Email can not be empty!");

        RuleFor(model => model.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password can not be empty!")
            .Length(8, 30).WithMessage("Password's length must be 8-30 symbols!")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$").WithMessage("Password must consist of one uppercase and lowercase Latin letter, a number and a special character!");
    }
}