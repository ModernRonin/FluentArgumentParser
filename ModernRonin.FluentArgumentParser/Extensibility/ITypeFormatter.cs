using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Extensibility
{
    public interface ITypeFormatter
    {
        string Format(AParameter parameter);
    }
}