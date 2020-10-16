using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Extensibility
{
    public interface IExampleValueProvider
    {
        object For(AParameter parameter);
    }
}