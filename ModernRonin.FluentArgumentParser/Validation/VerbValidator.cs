using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Validation
{
    public class VerbValidator : AbstractValidator<Verb>
    {
        public VerbValidator(bool doRequireName = true)
        {
            if (doRequireName) RuleFor(v => v.Name).NotEmpty();
            RuleFor(v => v.HelpText).NotNull();
            When(v => v.Any(), () =>
                {
                    RuleFor(v => v.Parameters)
                        .Empty()
                        .WithMessage("If a verb contains other verbs, it cannot have parameters.");
                })
                .Otherwise(() =>
                {
                    RuleForEach(v => v.Parameters)
                        .SetInheritanceValidator(v =>
                        {
                            v.Add(new RequiredParameterValidator());
                            v.Add(new OptionalParameterValidator());
                            v.Add(new FlagParameterValidator());
                        });
                    RuleFor(v => v.Parameters).Collection().NoDuplicateValuesFor(p => p.LongName);
                    RuleFor(v => v.Parameters).Collection().NoDuplicateValuesFor(p => p.ShortName);
                    RuleFor(v => v.Parameters)
                        .Must(indexedParametersHaveNoHolesBetweenIndices)
                        .WithMessage("Required and optional parameter indices must be consecutive.")
                        .Must(indexedParametersMustStartWithZero)
                        .WithMessage("The smallest required/optional parameter index must be zero.")
                        .Must(requiredComeBeforeOptional)
                        .WithMessage(
                            "All required parameters must have indices smaller than all optional parameters.");
                });

            bool indexedParametersHaveNoHolesBetweenIndices(AParameter[] parameters)
            {
                var indices = parameters.OfType<AnIndexableParameter>()
                    .OrderBy(index)
                    .Select(index)
                    .ToArray();
                for (var i = 1; i < indices.Length; ++i)
                {
                    var (last, current) = (indices[i - 1], indices[i]);
                    if (last != current - 1) return false;
                }

                return true;
            }

            bool indexedParametersMustStartWithZero(AParameter[] parameters) =>
                minOrDefault(parameters.OfType<AnIndexableParameter>().Select(index), 0) == 0;

            bool requiredComeBeforeOptional(AParameter[] parameters)
            {
                if (!parameters.OfType<RequiredParameter>().Any() ||
                    !parameters.OfType<OptionalParameter>().Any()) return true;
                var maximalRequiredIndex =
                    maxOrDefault(parameters.OfType<RequiredParameter>().Select(index), -1);
                var minimalOptionalIndex =
                    minOrDefault(parameters.OfType<OptionalParameter>().Select(index), 0);
                return minimalOptionalIndex > maximalRequiredIndex;
            }

            int index(AnIndexableParameter indexable) => indexable.Index;

            int minOrDefault(IEnumerable<int> values, int defaultValue)
            {
                var materialized = values.ToArray();
                return materialized.Any() ? materialized.Min() : defaultValue;
            }

            int maxOrDefault(IEnumerable<int> values, int defaultValue)
            {
                var materialized = values.ToArray();
                return materialized.Any() ? materialized.Max() : defaultValue;
            }
        }
    }
}