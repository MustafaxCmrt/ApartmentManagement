using FluentValidation;

namespace ApartmentManagement.Application.Features.Search.Queries.GlobalSearch;

public class GlobalSearchQueryValidator : AbstractValidator<GlobalSearchQuery>
{
    public GlobalSearchQueryValidator()
    {
        RuleFor(x => x.Q)
            .NotEmpty().WithMessage("Arama metni en az 2 karakter olmalıdır.")
            .MinimumLength(2).WithMessage("Arama metni en az 2 karakter olmalıdır.");

        RuleFor(x => x.Limit)
            .LessThanOrEqualTo(20).WithMessage("Limit en fazla 20 olabilir.");
    }
}
