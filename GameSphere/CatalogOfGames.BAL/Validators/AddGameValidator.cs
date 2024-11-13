using CatalogOfGames.BAL.DTOs;
using FluentValidation;

namespace CatalogOfGames.BAL.Validators;

public class AddGameValidator : AbstractValidator<AddGameDTO>
{
    public AddGameValidator()
    {
        RuleFor(g => g.Name)
          .NotEmpty().WithMessage("Name cannot be empty!")
          .MaximumLength(70).WithMessage("Name must be less than 70 symbols!");    

        RuleFor(g => g.Description)
            .NotEmpty().WithMessage("Description cannot be empty!")
            .MaximumLength(10000).WithMessage("Description must be less than 70 symbols!");

        RuleFor(g => g.PublisherId)
            .NotNull().WithMessage("PublisherId can not be nullable!")
            .NotEmpty().WithMessage("PublisherId can not be empty!");
    }
}