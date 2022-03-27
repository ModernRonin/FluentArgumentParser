namespace ModernRonin.FluentArgumentParser.Definition;

public class OptionalParameter : AnIndexableParameter
{
    object _default;
    public bool HasDefaultBeenSet { get; private set; }
    public string Description { get; set; }

    public object Default
    {
        get => _default;
        set
        {
            _default = value;
            HasDefaultBeenSet = true;
        }
    }
}