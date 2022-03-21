using FluentValidation;

using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Validation;

public class OptionalParameterValidator : AbstractValidator<OptionalParameter>
{
    public OptionalParameterValidator()
    {
        Include(new IndexableParameterValidator());
        RuleFor(p => p.Default).Must((p, _) => p.HasDefaultBeenSet);
        RuleFor(p => p.Description).Must((p, _) =>
        {
            if (p.Default is null && string.IsNullOrEmpty(p.Description))
            {
                return false;
            }

            return true;
        }).WithMessage("Reference types (with a null default) requires a description to be set.");
    }
}