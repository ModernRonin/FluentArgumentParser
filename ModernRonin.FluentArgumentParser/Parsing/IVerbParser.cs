using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Parsing;

public interface IVerbParser
{
    void Parse(ParserConfiguration configuration, VerbCall verb, string[] args);
}