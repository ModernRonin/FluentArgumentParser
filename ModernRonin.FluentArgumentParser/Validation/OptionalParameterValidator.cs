using FluentValidation;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Validation;

public class OptionalParameterValidator : AbstractValidator<OptionalParameter>
{
    public OptionalParameterValidator()
    {
        Include(new IndexableParameterValidator());
        RuleFor(p => p.Default).NotNull();
    }
}