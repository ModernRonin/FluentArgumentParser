using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentValidation;

namespace ModernRonin.FluentArgumentParser.Validation;

public static class RuleBuilderInitialExtensions
{
    public static IRuleBuilderOptions<TContainer, IEnumerable<TElement>> MustHaveDistinct<TContainer,
        TElement, TSubProperty>(
        this IRuleBuilderInitial<TContainer, IEnumerable<TElement>> self,
        Expression<Func<TElement, TSubProperty>> propertySelector)
    {
        return self.Must(haveNoDuplicates)
            .WithMessage($"Elements of collection must have distinct values for {propertySelector}");

        bool haveNoDuplicates(TContainer container,
            IEnumerable<TElement> elements,
            ValidationContext<TContainer> context)
        {
            var subject = elements.ToArray();
            if (subject.DistinctBy(propertySelector.Compile()).Count() == subject.Length) return true;
            return false;
        }
    }
}