using FluentValidation;
using Forum.BAL.DTOs;

namespace Forum.BAL.Validators;

public class AddPostValidator : AbstractValidator<AddPostDTO>
{
    public AddPostValidator()
    {
        RuleFor(p => p.Subject)
            .NotEmpty().WithMessage("Subject cannot be empty!")
            .NotNull().WithMessage("Subject cannot be nullable!");

        RuleFor(p => p.Text)
            .NotEmpty().WithMessage("Text cannot be empty!")
            .NotNull().WithMessage("Text cannot be nullable!");

        RuleFor(p => p.UserId)
            .NotEmpty().WithMessage("UserId required | empty!")
            .NotNull().WithMessage("UserId required | nullable!");
    }
}