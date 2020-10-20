using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using ModernRonin.FluentArgumentParser.Validation;

namespace ModernRonin.FluentArgumentParser.Parsing
{
    public class CommandLineParser : AVerbContainer, ICommandLineParser
    {
        readonly IArgumentPreprocessor _argumentPreprocessor;

        readonly string[] _helpVerbs =
        {
            "help",
            "?"
        };

        readonly IVerbParser _verbParser;

        public CommandLineParser() : this(new NullArgumentPreprocessor()) { }

        public CommandLineParser(IArgumentPreprocessor argumentPreprocessor) : this(new VerbParser(),
            argumentPreprocessor) { }

        /// <summary>
        ///     This constructor exists mostly for unit-tests, but obviously also forms an extension point for
        ///     the hopefully unlikely event that you find yourself in need of replacing <see cref="VerbParser" />.
        /// </summary>
        public CommandLineParser(IVerbParser verbParser, IArgumentPreprocessor argumentPreprocessor)
        {
            _verbParser = verbParser;
            _argumentPreprocessor = argumentPreprocessor;
        }

        public ParserConfiguration Configuration { get; set; } = new ParserConfiguration();

        public Verb DefaultVerb { get; set; }

        public VerbCall Parse(string[] args)
        {
            new CommandLineParserValidator().ValidateAndThrow(this);

            if (args.Length == 0) return new VerbCall {IsHelpRequest = true};
            if (args.Length == 1 && _helpVerbs.Contains(args[0].ToLowerInvariant()))
                return new VerbCall {IsHelpRequest = true};
            if (args.Length > 1 && _helpVerbs.Contains(args[0].ToLowerInvariant()))
            {
                var (helpTargetVerb, rest) = findLeafVerb(this, args.Skip(1).ToArray(), true);
                return new VerbCall
                {
                    IsHelpRequest = true,
                    Verb = helpTargetVerb,
                    IsDefaultVerb = helpTargetVerb == DefaultVerb,
                    UnknownVerb = rest.Length == 0 ? default : string.Join(' ', rest)
                };
            }

            var (leafVerb, arguments) = findLeafVerb(this, args);
            leafVerb ??= DefaultVerb;
            if (default == leafVerb)
            {
                return new VerbCall
                {
                    IsHelpRequest = true,
                    UnknownVerb = args[0]
                };
            }

            var result = new VerbCall
            {
                Verb = leafVerb,
                IsDefaultVerb = leafVerb == DefaultVerb
            };
            _verbParser.Parse(Configuration, result,
                arguments.Select(_argumentPreprocessor.Process).ToArray());
            return result;

            (Verb, string[]) findLeafVerb(IEnumerable<Verb> verbs, string[] inputs, bool useLastMatch = false)
            {
                var (name, rest) = uncons(inputs);
                var verb = verbs.FirstOrDefault(withNameMatch);
                if (verb == default) return (default, inputs);
                if (!verb.Any()) return (verb, rest);
                var (verbResult, restResult) = findLeafVerb(verb, rest);

                if (verbResult == default && useLastMatch) return (verb, rest);
                return (verbResult, restResult);

                bool withNameMatch(Verb v) =>
                    v.Name.Equals(name, Configuration.AreVerbNamesCaseSensitive.StringComparison());
            }

            (string head, string[] tail) uncons(string[] what) =>
                what.Length switch
                {
                    0 => (default, what),
                    _ => (what[0], what.Skip(1).ToArray())
                };
        }
    }
}