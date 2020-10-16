using System;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Parsing
{
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

        public ParameterBindingConfigurer<TProperty> WithLongName(string longName)
        {
            Parameter.LongName = longName;
            return this;
        }

        public ParameterBindingConfigurer<TProperty> WithShortName(string shortName)
        {
            Parameter.ShortName = shortName;
            return this;
        }

        public ParameterBindingConfigurer<TProperty> WithHelp(string helpText)
        {
            Parameter.HelpText = helpText;
            return this;
        }
    }
}