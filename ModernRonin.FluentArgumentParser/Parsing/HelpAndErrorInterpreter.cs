using System;
using System.Linq;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Help;

namespace ModernRonin.FluentArgumentParser.Parsing
{
    public class HelpAndErrorInterpreter : IHelpAndErrorInterpreter
    {
        readonly IHelpMaker _helpMaker;

        public HelpAndErrorInterpreter(IHelpMaker helpMaker) => _helpMaker = helpMaker;

        public HelpResult Interpret(VerbCall call, ICommandLineParser parser)
        {
            if (call.IsHelpRequest) return MakeHelpResult(call, parser);
            return call.HasError ? ForBadArguments(call, parser.Configuration) : default;
        }

        public string GetHelpOverview(ICommandLineParser parser) => _helpMaker.GenerateFor(parser);

        HelpResult ForBadArguments(VerbCall call, ParserConfiguration configuration)
        {
            var argumentErrors = string.Join(Environment.NewLine, call.Arguments.Select(a => a.Error));
            return new HelpResult
            {
                IsResultOfInvalidInput = true,
                Text = HelpFor(call, configuration) +
                       Environment.NewLine          +
                       argumentErrors
            };
        }

        string HelpFor(VerbCall call, ParserConfiguration configuration) =>
            _helpMaker.GenerateFor(call.Verb, call.IsDefaultVerb, configuration);

        HelpResult MakeHelpResult(VerbCall call, ICommandLineParser parser)
        {
            if (call.Verb == default)
            {
                if (call.UnknownVerb == default)
                    return new HelpResult {Text = _helpMaker.GenerateFor(parser)};
                return new HelpResult
                {
                    IsResultOfInvalidInput = true,
                    Text = _helpMaker.GenerateFor(parser) + Environment.NewLine +
                           $"Unknown verb '{call.UnknownVerb}'"
                };
            }

            if (call.UnknownVerb != default)
            {
                return new HelpResult
                {
                    IsResultOfInvalidInput = true,
                    Text =
                        HelpFor(call, parser.Configuration) +
                        Environment.NewLine                 +
                        $"Unknown verb '{call.UnknownVerb}'"
                };
            }

            return new HelpResult {Text = HelpFor(call, parser.Configuration)};
        }
    }
}