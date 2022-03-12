using System;
using System.Linq;
using ModernRonin.FluentArgumentParser.Definition;
using MoreLinq;

namespace ModernRonin.FluentArgumentParser.Parsing;

public class VerbParser : IVerbParser
{
    public void Parse(ParserConfiguration configuration, VerbCall verb, string[] args)
    {
        var arguments = args.Select(match).ToList();
        var identicalNameMultipleValues = arguments.Where(a => a.IsValid)
            .GroupBy(a => a.Parameter.LongName)
            .Where(g => g.Count() > 1);
        identicalNameMultipleValues.SelectMany(g => g)
            .ForEach(a => { a.Error = "The same argument is given multiple times"; });
        arguments.Where(a => a.IsValid).ForEach(set);

        arguments.AddRange(verb.Verb.Parameters
            .Where(p => !arguments.Select(a => a.Parameter).Contains(p))
            .Select(handleUnused)
            .ToArray());

        verb.Arguments = arguments.ToArray();

        Argument handleUnused(AParameter parameter) =>
            parameter switch
            {
                FlagParameter _ => new Argument
                {
                    Parameter = parameter,
                    Value = false
                },
                OptionalParameter opt => new Argument
                {
                    Parameter = parameter,
                    Value = opt.Default
                },
                RequiredParameter _ => new Argument
                {
                    Parameter = parameter,
                    Error = $"'{parameter.LongName}' was not given, but is required"
                },
                _ => throw new NotImplementedException()
            };

        void set(Argument argument)
        {
            argument.Value = argument.Parameter switch
            {
                OptionalParameter o when argument.Value == default => o.Default,
                FlagParameter _ => true,
                AnIndexableParameter i => convert(argument.Value, i.Type),
                _ => default
            };
            if (argument.Value == default)
                argument.Error = $"Value for '{argument.Parameter.LongName}' is not valid";
        }

        object convert(object value, Type type)
        {
            try
            {
                if (type.IsEnum) return Enum.Parse(type, value.ToString());
                return Convert.ChangeType(value, type);
            }
            catch (Exception)
            {
                return null;
            }
        }

        Argument match(string arg, int index)
        {
            AParameter parameter;
            if (arg.StartsWith(configuration.LongNamePrefix))
            {
                var name = arg.After(configuration.LongNamePrefix).Before(configuration.ValueDelimiter);
                parameter = verb.Verb.Parameters.FirstOrDefault(p =>
                    p.LongName.Equals(name,
                        configuration.AreLongParameterNamesCaseSensitive.StringComparison()));
                return new Argument
                {
                    Parameter = parameter,
                    Value = arg.After(configuration.ValueDelimiter),
                    Error = default == parameter ? $"unknown argument '{name}'" : null
                };
            }

            if (arg.StartsWith(configuration.ShortNamePrefix))
            {
                var name = arg.After(configuration.ShortNamePrefix).Before(configuration.ValueDelimiter);
                parameter = verb.Verb.Parameters.FirstOrDefault(p =>
                    p.ShortName.Equals(name,
                        configuration.AreShortParameterNamesCaseSensitive.StringComparison()));
                return new Argument
                {
                    Parameter = parameter,
                    Value = arg.After(configuration.ValueDelimiter),
                    Error = default == parameter ? $"unknown argument '{name}'" : null
                };
            }

            parameter = verb.Verb.Parameters.OfType<AnIndexableParameter>()
                .FirstOrDefault(p => p.Index == index);
            return new Argument
            {
                Parameter = parameter,
                Value = arg,
                WasPositionalMatch = true,
                Error = default == parameter
                    ? $"too many arguments - don't know what to do with '{arg}'"
                    : null
            };
        }
    }
}