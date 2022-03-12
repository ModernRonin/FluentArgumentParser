using FluentValidation;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Validation;

public class FlagParameterValidator : AbstractValidator<FlagParameter>
{
    public FlagParameterValidator()
    {
        Include(new ParameterValidator());
    }
}