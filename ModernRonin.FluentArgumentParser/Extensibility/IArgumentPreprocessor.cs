namespace ModernRonin.FluentArgumentParser.Extensibility
{
    public interface IArgumentPreprocessor
    {
        string Process(string what);
    }
}