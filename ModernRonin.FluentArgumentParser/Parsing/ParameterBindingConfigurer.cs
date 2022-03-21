using System;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Parsing;

/// <summary>
///     FluentArgumentParser makes an effort to guess your intentions from looking at the properties of the POCOs you
///     supply for verbs, but
///     if you need more control, you can use the methods on <inheritdoc cref="ParameterBindingConfigurer{TProperty}" />.
/// </summary>
/// <typeparam name="TProperty"></typeparam>
public sealed class ParameterBindingConfigurer<TProperty>
{
    readonly Func<AParameter> _getter;
    readonly Action<AParameter> _setter;

    public ParameterBindingConfigurer(Func<AParameter> getter, Action<AParameter> setter)
    {
        _getter = getter;
        _setter = setter;
    }

    AParameter Parameter
    {
        get => _getter();
        set => _setter(value);
    }

    /// <summary>
    ///     Turn a parameter optional. By default, all properties in your verb POCOs that have no specific initialization value
    ///     set, are assumed to be required parameters.
    /// </summary>
    public ParameterBindingConfigurer<TProperty> MakeOptional()
    {
        if (Parameter is RequiredParameter required)
        {
            Parameter = new OptionalParameter
            {
                Index = required.Index,
                LongName = required.LongName,
                ShortName = required.ShortName,
                Type = required.Type,
                Default = required.Type.DefaultValue()
            };
        }

        return this;
    }

    /// <summary>
    ///     <para>
    ///         If you want to change the auto-detected default value of an optional parameter or if you just turned a required
    ///         parameter optional via
    ///         <see cref="MakeOptional" />, then you can use this method.
    ///     </para>
    ///     Auto-detection of default values uses what you set a construction time for a property. If the value you set differs
    ///     from what the property would have
    ///     if you didn't set anything, then the property/parameter is assumed to be optional and your initialization value is
    ///     used as a default value.
    /// </summary>
    public ParameterBindingConfigurer<TProperty> WithDefault(TProperty value)
    {
        if (!(Parameter is OptionalParameter optional))
        {
            throw new InvalidOperationException(
                $"Default are only settable for optional parameters - maybe you forgot a call to {nameof(MakeOptional)}?");
        }

        optional.Default = value;
        return this;
    }

    /// <summary>
    ///     <para>
    ///         Set the index at which this parameter is expected. This is only relevant when users don't use names to specify
    ///         parameters
    ///         and affects only required or optional parameters, but not flags.
    ///     </para>
    ///     By default, indices for parameters are derived from the order of properties in your POCOs, but this can become
    ///     different from
    ///     what you expect when your POCOs have base classes.
    /// </summary>
    public ParameterBindingConfigurer<TProperty> ExpectAt(int index)
    {
        if (!(Parameter is AnIndexableParameter indexable))
        {
            throw new InvalidOperationException(
                "the index is only settable for required and optional parameters");
        }

        indexable.Index = index;
        return this;
    }

    /// <summary>
    ///     Set the long name that can be used to specify a parameter. By default, the property-name is used.
    /// </summary>
    public ParameterBindingConfigurer<TProperty> WithLongName(string longName)
    {
        Parameter.LongName = longName;
        return this;
    }

    /// <summary>
    ///     Set the short name that can be used to specify a parameter. By default, the first letter of the property-name is
    ///     used. If that letter is already in use by another property/parameter, the next one is
    ///     used and so on.
    /// </summary>
    public ParameterBindingConfigurer<TProperty> WithShortName(string shortName)
    {
        Parameter.ShortName = shortName;
        return this;
    }

    /// <summary>
    ///     Set the help description for this parameter/property. By default, the help text is empty.
    /// </summary>
    public ParameterBindingConfigurer<TProperty> WithHelp(string helpText)
    {
        Parameter.HelpText = helpText;
        return this;
    }

    /// <summary>
    ///     <para>
    ///     Set default description for optional parameter.
    ///     </para>
    ///     Default description is required for reference types with a default value of null
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public ParameterBindingConfigurer<TProperty> WithDefaultDescription(string text)
    {
        if (!(Parameter is OptionalParameter optional))
        {
            throw new InvalidOperationException(
                $"Default are only settable for optional parameters - maybe you forgot a call to {nameof(MakeOptional)}?");
        }

        optional.Description = text;
        return this;
    }
}