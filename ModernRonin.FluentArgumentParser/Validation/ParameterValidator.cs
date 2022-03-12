using FluentValidation;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Validation;

public class ParameterValidator : AbstractValidator<AParameter>
{
    public ParameterValidator()
    {
        RuleFor(p => p.LongName).NotEmpty();
        RuleFor(p => p.ShortName).NotEmpty();
        RuleFor(p => p.HelpText).NotNull();
    }
}