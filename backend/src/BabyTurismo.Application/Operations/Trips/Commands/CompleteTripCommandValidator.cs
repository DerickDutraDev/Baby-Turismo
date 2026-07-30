using FluentValidation;

namespace BabyTurismo.Application.Operations.Trips.Commands;

public sealed class CompleteTripCommandValidator : AbstractValidator<CompleteTripCommand>
{
    public CompleteTripCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ChecklistNotes).MaximumLength(1000);
    }
}
