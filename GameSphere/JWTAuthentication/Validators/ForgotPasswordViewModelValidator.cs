using FluentValidation;
using JWTAuthentication.ViewModels;

namespace JWTAuthentication.Validators;

public class ForgotPasswordViewModelValidator : AbstractValidator<ForgotPasswordViewModel>
{
    public ForgotPasswordViewModelValidator()
    {
        RuleFor(model => model.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email can not be empty!")
            .EmailAddress().WithMessage("Email is incorrect!")
            .Matches(@"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$").WithMessage("Email has unavailable symbols!");
    }
}