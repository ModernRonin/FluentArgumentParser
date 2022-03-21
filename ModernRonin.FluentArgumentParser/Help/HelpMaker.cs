using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using ModernRonin.FluentArgumentParser.Parsing;
using MoreLinq.Extensions;

namespace ModernRonin.FluentArgumentParser.Help;

public class HelpMaker : IHelpMaker
{
    const int Padding = 2;
    readonly StringBuilder _buffer = new();
    readonly IExampleValueProvider _exampleValues;
    readonly ITypeFormatter _typeFormatter;
    public HelpMaker() : this(new DefaultTypeFormatter(), new DefaultExampleValueProvider()) { }

    public HelpMaker(ITypeFormatter typeFormatter, IExampleValueProvider exampleValues)
    {
        _typeFormatter = typeFormatter;
        _exampleValues = exampleValues;
    }

    public string GenerateFor(Verb verb, bool isDefaultVerb, ParserConfiguration configuration)
    {
        _buffer.Clear();
        if (verb.Any())
        {
            VerbList($"{configuration.ApplicationName} {verb.Name}", verb, () =>
            {
                Line(verb.HelpText);
                LineFeed();
            });
        }
        else Format(verb, configuration, isDefaultVerb);

        return _buffer.ToString();
    }

    public string GenerateFor(ICommandLineParser parser)
    {
        _buffer.Clear();
        var configuration = parser.Configuration;
        var appName = configuration.ApplicationName;

        Line(appName);
        Line(configuration.ApplicationDescription);
        LineFeed();
        Line("Usage:");
        if (parser.DefaultVerb != default) Format(parser.DefaultVerb, configuration, true);
        else VerbList(appName, parser);

        LineFeed();
        Line(
            $"long parameter names are {not(configuration.AreLongParameterNamesCaseSensitive)}case-sensitive");
        Line(
            $"short parameter names are {not(configuration.AreShortParameterNamesCaseSensitive)}case-sensitive");
        Line($"command names are {not(configuration.AreVerbNamesCaseSensitive)}case-sensitive");

        return _buffer.ToString();

        string not(bool flag) => flag ? string.Empty : "not ";
    }

    void Format(Verb verb, ParserConfiguration configuration, bool isDefaultVerb)
    {
        (RequiredParameter[], OptionalParameter[], FlagParameter[]) groupAndSort() =>
            (verb.Parameters.OfType<RequiredParameter>().OrderBy(p => p.Index).ToArray(),
                verb.Parameters.OfType<OptionalParameter>().OrderBy(p => p.Index).ToArray(),
                verb.Parameters.OfType<FlagParameter>().ToArray());

        var (required, optional, flags) = groupAndSort();
        var all = required.Cast<AParameter>().Concat(optional).Concat(flags).ToArray();

        string verbName(AVerbContainer container) =>
            container switch
            {
                CommandLineParser _ => configuration.ApplicationName,
                Verb _ when isDefaultVerb => configuration.ApplicationName,
                Verb v when v.Parent == default => $"{configuration.ApplicationName} {v.Name}",
                Verb v => verbName(v.Parent) + " " + v.Name,
                _ => throw new NotImplementedException()
            };

        Append($"{verbName(verb)} ");

        List(all, " ", grammar);

        LineFeed();
        LineFeed();
        Line(verb.HelpText);

        if (required.Any())
        {
            LineFeed();
            Line("Required arguments:");
            Table(required.Select(requiredToRow).ToArray());
        }

        if (optional.Any())
        {
            LineFeed();
            Line("Optional arguments:");
            Table(optional.Select(optionalToRow).ToArray());
        }

        if (flags.Any())
        {
            LineFeed();
            Line("Flags:");
            Table(flags.Select(flagToRow).ToArray());
        }

        LineFeed();
        Line("Examples:");
        var values = all.ToDictionary(p => p, p => _exampleValues.For(p));
        new[]
            {
                example(Method.ByIndex, true, true),
                example(Method.ByIndex),
                example(Method.ByLongName),
                example(Method.ByShortName),
                example(Method.ByShortName, doRandomize: true)
            }.Select(e => string.Join(" ", e))
            .Select(e => $"{verbName(verb)} {e}")
            .Distinct()
            .ForEach(Line);

        IEnumerable<string> example(Method method,
            bool doExcludeOptional = false,
            bool doExcludeFlags = false,
            bool doRandomize = false)
        {
            IEnumerable<AParameter> included = required;
            if (!doExcludeOptional) included = included.Concat(optional);
            if (!doExcludeFlags) included = included.Concat(flags);
            if (doRandomize) included = Shift(included, 3);

            return included.Select(p => sampleArgument(method, p));
        }

        string sampleArgument(Method method, AParameter parameter)
        {
            var key = method switch
            {
                Method.ByIndex when parameter is AnIndexableParameter => string.Empty,
                Method.ByIndex when parameter is FlagParameter =>
                    $"{configuration.LongNamePrefix}{parameter.LongName}",
                Method.ByLongName => $"{configuration.LongNamePrefix}{parameter.LongName}",
                Method.ByShortName => $"{configuration.ShortNamePrefix}{parameter.ShortName}",
                _ => throw new NotImplementedException()
            };
            return parameter switch
            {
                FlagParameter _ => key,
                AnIndexableParameter _ when method == Method.ByIndex => values[parameter].ToString(),
                AnIndexableParameter _ => $"{key}={values[parameter]}",
                _ => throw new NotImplementedException()
            };
        }

        Row parameterToRow(AParameter parameter)
        {
            var result = new Row
            {
                LeftText =
                    $"{configuration.LongNamePrefix}{parameter.LongName}, {configuration.ShortNamePrefix}{parameter.ShortName}"
            };
            if (!string.IsNullOrWhiteSpace(parameter.HelpText)) result.RightLines.Add(parameter.HelpText);
            return result;
        }

        Row indexableToRow(AnIndexableParameter indexable)
        {
            var result = parameterToRow(indexable);
            result.RightLines.Add(_typeFormatter.Format(indexable));
            return result;
        }

        Row requiredToRow(RequiredParameter req) => indexableToRow(req);

        Row optionalToRow(OptionalParameter opt)
        {
            var result = indexableToRow(opt);
            result.RightLines.Add(opt.Description);
            return result;
        }

        Row flagToRow(FlagParameter flag) => parameterToRow(flag);

        string grammar(AParameter parameter)
        {
            var name = $"{configuration.LongNamePrefix}{parameter.LongName}";
            return parameter switch
            {
                RequiredParameter _ => $"{name}=<value>",
                OptionalParameter _ => $"[{name}=<value>]",
                FlagParameter _ => $"[{name}]",
                _ => throw new NotImplementedException()
            };
        }
    }

    void VerbList(string preamble, IEnumerable<Verb> verbs)
    {
        VerbList(preamble, verbs, () => { });
    }

    void VerbList(string preamble, IEnumerable<Verb> verbs, Action addDescription)
    {
        var materialized = verbs as Verb[] ?? verbs.ToArray();
        Append(preamble);
        Append(" ");
        List(materialized, " | ", v => v.Name);
        Line(" [command specific options]");
        LineFeed();
        addDescription();
        Line("Available commands:");
        Table(materialized.Select(v => new Row
            {
                LeftText = v.Name,
                RightLines = { v.HelpText }
            })
            .ToArray());
        LineFeed();
        Line("use help <command> for more detailed help on a specific command");
    }

    void List<T>(IEnumerable<T> elements, string separator, Func<T, string> formatter) =>
        _buffer.AppendJoin(separator, elements.Select(formatter));

    void Append(string what) => _buffer.Append(what);

    void LineFeed() => _buffer.AppendLine();

    void Line(string what)
    {
        if (!string.IsNullOrWhiteSpace(what)) _buffer.AppendLine(what);
    }

    void Table(Row[] rows)
    {
        var longestLeft = rows.Select(r => r.LeftText.Length).Max();
        var desiredLeftWidth = longestLeft + Padding;
        var lines = rows.SelectMany(format);
        lines.ForEach(Line);

        IEnumerable<string> format(Row row)
        {
            var left = row.LeftText.PadRight(desiredLeftWidth);
            yield return $"{left}{row.RightLines.FirstOrDefault() ?? string.Empty}";
            var filler = new string(' ', desiredLeftWidth);
            foreach (var right in row.RightLines.Skip(1)) yield return $"{filler}{right}";
        }
    }

    static IEnumerable<T> Shift<T>(IEnumerable<T> self, int delta)
    {
        var materialized = self.ToArray();
        return materialized.Skip(delta).Concat(materialized.Take(delta));
    }

    class Row
    {
        public string LeftText { get; set; }
        public List<string> RightLines { get; } = new();
    }

    enum Method
    {
        ByIndex,
        ByShortName,
        ByLongName
    }
}