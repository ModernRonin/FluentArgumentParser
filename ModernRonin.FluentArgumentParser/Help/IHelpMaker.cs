using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Parsing;

namespace ModernRonin.FluentArgumentParser.Help
{
    public interface IHelpMaker
    {
        string GenerateFor(Verb verb, bool isDefaultVerb, ParserConfiguration configuration);
        string GenerateFor(ICommandLineParser parser);
    }
}