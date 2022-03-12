using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace ModernRonin.FluentArgumentParser.Validation;

public static class RuleBuilderInitialExtensions
{
    public static IRuleBuilderOptions<TContainer, IEnumerable<TElement>> MustHaveDistinct<TContainer,
        TElement, TSubProperty>(
        this IRuleBuilderInitial<TContainer, IEnumerable<TElement>> self,
        Func<TElement, TSubProperty> propertySelector) =>
        self.Must(vs => vs.Select(propertySelector).Distinct().Count() <= 1);
}