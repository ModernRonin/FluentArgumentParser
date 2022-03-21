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
            if (!p.HasDefaultBeenSet)
            {
                return false;
            }

            if (p.Default.GetType().DefaultValue == null)
            {
                return false;
            }

            return true;
        }).WithMessage("Reference types (with a null default) requires a description to be set.");
    }
}