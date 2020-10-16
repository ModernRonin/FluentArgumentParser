using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using ModernRonin.FluentArgumentParser.Definition;
using MoreLinq;

namespace ModernRonin.FluentArgumentParser.Validation
{
    public class VerbContainerValidator : AbstractValidator<IVerbContainer>
    {
        public VerbContainerValidator()
        {
            RuleForEach(c => c.Verbs).SetValidator(new VerbValidator());
            RuleFor(c => c.Verbs).Collection().NoDuplicateValuesFor(v => v.Name);
        }

        public override ValidationResult Validate(ValidationContext<IVerbContainer> context)
        {
            var result = base.Validate(context);
            var container = context.InstanceToValidate;
            if (!container.Any()) return result;
            container.SelectMany(v => Validate(v).Errors).ForEach(result.Errors.Add);
            return result;
        }
    }
}