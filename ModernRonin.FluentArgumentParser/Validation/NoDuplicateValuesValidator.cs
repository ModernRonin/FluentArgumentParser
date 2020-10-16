using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentValidation.Validators;
using MoreLinq.Extensions;

namespace ModernRonin.FluentArgumentParser.Validation
{
    public class NoDuplicateValuesValidator<TElement, TSubProperty> : PropertyValidator
    {
        readonly Expression<Func<TElement, TSubProperty>> _accessor;

        public NoDuplicateValuesValidator(Expression<Func<TElement, TSubProperty>> accessor) : base(
            "Each element of {PropertyName} must have a unique value for {SubPropertyName}. For example, the value {ExampleDuplicate} is used more than once.") =>
            _accessor = accessor;

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (!(context.PropertyValue is IEnumerable<TElement> enumerable)) return true;

            var subject = enumerable.ToArray();
            if (subject.DistinctBy(_accessor.Compile()).Count() == subject.Length) return true;
            var exampleDuplicate = subject.GroupBy(_accessor.Compile()).First(g => g.Count() > 1).Key;
            context.MessageFormatter.AppendArgument("SubPropertyName", _accessor.MemberName());
            context.MessageFormatter.AppendArgument("ExampleDuplicate", exampleDuplicate);

            return false;
        }
    }
}