
using FluentValidation;

namespace Badil.Application.Features.Ping;

public class PingQueryValidator : AbstractValidator<PingQuery> {
    public PingQueryValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(20);
    }
}