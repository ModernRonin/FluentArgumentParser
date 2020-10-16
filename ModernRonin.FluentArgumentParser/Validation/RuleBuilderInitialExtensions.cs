using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentValidation;

namespace ModernRonin.FluentArgumentParser.Validation
{
    public static class RuleBuilderInitialExtensions
    {
        public static Rules<T, TElement> Collection<T, TElement>(
            this IRuleBuilderInitial<T, IEnumerable<TElement>> self) =>
            new Rules<T, TElement>(self);

        public class Rules<TOwner, TElement>
        {
            readonly IRuleBuilderInitial<TOwner, IEnumerable<TElement>> _ruleBuilder;

            public Rules(IRuleBuilderInitial<TOwner, IEnumerable<TElement>> ruleBuilder) =>
                _ruleBuilder = ruleBuilder;

            public IRuleBuilderOptions<TOwner, IEnumerable<TElement>> NoDuplicateValuesFor<TSubProperty>(
                Expression<Func<TElement, TSubProperty>> accessor)
            {
                var validator = new NoDuplicateValuesValidator<TElement, TSubProperty>(accessor);
                return _ruleBuilder.SetValidator(validator);
            }
        }
    }
}