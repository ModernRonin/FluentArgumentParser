using FluentValidation;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Validation;

public class RequiredParameterValidator : AbstractValidator<RequiredParameter>
{
    public RequiredParameterValidator()
    {
        Include(new IndexableParameterValidator());
    }
}