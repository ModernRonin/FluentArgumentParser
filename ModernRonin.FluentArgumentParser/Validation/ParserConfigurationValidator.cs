using FluentValidation;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Validation
{
    public class ParserConfigurationValidator : AbstractValidator<ParserConfiguration>
    {
        public ParserConfigurationValidator()
        {
            RuleFor(c => c.ApplicationName).NotEmpty();
            RuleFor(c => c.ApplicationDescription).NotEmpty();
            RuleFor(c => c.ShortNamePrefix).NotEmpty();
            RuleFor(c => c.LongNamePrefix).NotEmpty();
            RuleFor(c => c.ValueDelimiter).NotEmpty();
        }
    }
}