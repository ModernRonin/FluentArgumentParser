namespace ModernRonin.FluentArgumentParser.Definition;

public abstract class AParameter
{
    public string ShortName { get; set; }
    public string LongName { get; set; }
    public string HelpText { get; set; } = string.Empty;
}