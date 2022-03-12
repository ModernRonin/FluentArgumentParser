using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Parsing;

public static class VerbBindingExtensions
{
    /// <summary>
    ///     Internal use only.
    /// </summary>
    public static void GuardAgainstMissingBindCall(this ILeafVerbBinding self)
    {
        if (!self.Bindings.Any())
        {
            throw new InvalidOperationException(
                $"There are no parameter bindings registered - did you forget to call {nameof(self.Bind)}?");
        }
    }

    /// <summary>
    ///     Rename the verb. (By default, the verb's name is derived from the type name of the POCO you supplied.)
    /// </summary>
    public static T Rename<T>(this T self, string newName) where T : IVerbBinding
    {
        if (self is ILeafVerbBinding leaf) GuardAgainstMissingBindCall(leaf);

        self.Verb.Name = newName;
        return self;
    }

    /// <summary>
    ///     Set the general help text for this verb. By default the help text is empty.
    /// </summary>
    public static T WithHelp<T>(this T self, string helpText) where T : IVerbBinding
    {
        self.Verb.HelpText = helpText;
        return self;
    }

    /// <summary>
    ///     Use this to configure a specific parameter in more detail.
    /// </summary>
    public static ParameterBindingConfigurer<TProperty> Parameter<TProperty, TTarget>(
        this ILeafVerbBinding<TTarget> self,
        Expression<Func<TTarget, TProperty>> accessor) where TTarget : new()
    {
        GuardAgainstMissingBindCall(self);

        var propertyInfo = accessor.PropertyInfo();
        var binding = self.Bindings.FirstOrDefault(b => b.DoesMatch(propertyInfo));
        if (binding == default)
        {
            throw new ArgumentException(
                $"The property pointed at by {accessor} is not bound - is it settable and public?");
        }

        var index = findIndex();

        return new ParameterBindingConfigurer<TProperty>(() => binding.Parameter, v =>
        {
            binding.Parameter = v;
            self.Verb.Parameters[index] = v;
        });

        int findIndex()
        {
            for (var i = 0; i < self.Verb.Parameters.Length; ++i)
            {
                if (self.Verb.Parameters[i] == binding.Parameter) return i;
            }

            throw new InvalidOperationException();
        }
    }

    /// <summary>
    ///     Internal use only.
    /// </summary>
    public static ILeafVerbBinding Find(this IEnumerable<IVerbBinding> self, Verb verb)
    {
        return self.SelectMany(b => b.ThisAndChildren)
            .OfType<ILeafVerbBinding>()
            .FirstOrDefault(b => b.Verb == verb);
    }
}