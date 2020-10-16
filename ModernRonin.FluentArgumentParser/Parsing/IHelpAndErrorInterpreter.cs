using ModernRonin.FluentArgumentParser.Help;

namespace ModernRonin.FluentArgumentParser.Parsing
{
    public interface IHelpAndErrorInterpreter
    {
        HelpResult Interpret(VerbCall call, ICommandLineParser parser);
    }
}