namespace ModernRonin.FluentArgumentParser.Extensibility;

public class NullArgumentPreprocessor : IArgumentPreprocessor
{
    public string Process(string what) => what;
}