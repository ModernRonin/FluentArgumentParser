using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Parsing;

public interface ICommandLineParser : IVerbContainer
{
    Verb DefaultVerb { get; set; }
    ParserConfiguration Configuration { get; }
    bool DoSkipValidation { get; set; }
    VerbCall Parse(string[] args);
}