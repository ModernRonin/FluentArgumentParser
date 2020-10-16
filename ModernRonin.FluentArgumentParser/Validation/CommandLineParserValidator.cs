using System.Linq;
using FluentValidation;
using ModernRonin.FluentArgumentParser.Parsing;

namespace ModernRonin.FluentArgumentParser.Validation
{
    public class CommandLineParserValidator : AbstractValidator<CommandLineParser>
    {
        public CommandLineParserValidator()
        {
            Include(new VerbContainerValidator());

            RuleFor(p => p.Configuration).NotNull().SetValidator(new ParserConfigurationValidator());

            When(p => p.DefaultVerb != default, () =>
                {
                    RuleFor(p => p.DefaultVerb)
                        .Must((p, _) => !p.Any())
                        .WithMessage(
                            "If you have multiple verbs, just use regular verbs without a default verb.")
                        .SetValidator(new VerbValidator(false));
                })
                .Otherwise(() =>
                {
                    RuleFor(p => p.Verbs)
                        .Must(p => p.Count() > 1)
                        .WithMessage("If you have just one verb, use the default verb instead.");
                });
        }
    }
}