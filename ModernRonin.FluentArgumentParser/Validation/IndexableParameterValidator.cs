using System;
using System.Linq;
using FluentValidation;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Validation
{
    public class IndexableParameterValidator : AbstractValidator<AnIndexableParameter>
    {
        readonly Type[] _legalTypes =
        {
            typeof(string),
            typeof(byte),
            typeof(sbyte),
            typeof(int),
            typeof(uint),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal)
        };

        public IndexableParameterValidator()
        {
            var legalTypeNames = string.Join(", ", _legalTypes.Select(t => t.Name));
            Include(new ParameterValidator());
            RuleFor(p => p.Index).GreaterThanOrEqualTo(0);
            RuleFor(p => p.Type)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .Must(bePrimitiveOrEnum)
                .WithMessage($"Only {legalTypeNames} and enums are supported for parameters.");

            bool bePrimitiveOrEnum(Type type)
            {
                if (type.IsEnum) return true;
                if (_legalTypes.Contains(type)) return true;
                return false;
            }
        }
    }
}