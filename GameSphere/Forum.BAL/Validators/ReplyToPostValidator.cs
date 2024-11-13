using FluentValidation;
using Forum.BAL.DTOs;

namespace Forum.BAL.Validators;

public class ReplyToPostValidator : AbstractValidator<ReplyToPostDTO>
{
    public ReplyToPostValidator()
    {
        RuleFor(r => r.UserId)
           .NotEmpty().WithMessage("UserId required | empty!")
           .NotNull().WithMessage("UserId required | nullable!");

        RuleFor(p => p.PostId)
           .NotEmpty().WithMessage("PostId required | empty!")
           .NotNull().WithMessage("PostId required | nullable!");

        RuleFor(r => r.Content)
            .NotEmpty().WithMessage("Text cannot be empty!")
            .NotNull().WithMessage("Text cannot be nullable!");
    }
}