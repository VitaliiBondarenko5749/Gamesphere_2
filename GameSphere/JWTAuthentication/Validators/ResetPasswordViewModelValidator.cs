using FluentValidation;
using JWTAuthentication.ViewModels;

namespace JWTAuthentication.Validators;

public class ResetPasswordViewModelValidator : AbstractValidator<ResetPasswordViewModel>
{
    public ResetPasswordViewModelValidator()
    {
        RuleFor(model => model.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email can not be empty!")
            .EmailAddress().WithMessage("Email is incorrect!")
            .Matches(@"^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,}$").WithMessage("Email has unavailable symbols!");

        RuleFor(model => model.Password)
           .Cascade(CascadeMode.Stop)
           .NotEmpty().WithMessage("Password can not be empty!")
           .Length(8, 30).WithMessage("Password's length must be 8-30 symbols!")
           .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,30}$").WithMessage("Password must consist of one uppercase and lowercase Latin letter, a number and a special character!");

        RuleFor(model => model.ConfirmPassword)
            .Cascade(CascadeMode.Stop)
            .Equal(model => model.Password).WithMessage("Confirm Password and Password are not equal!");

        RuleFor(model => model.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Code can not be empty!");
    }
}