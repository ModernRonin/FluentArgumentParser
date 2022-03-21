namespace ModernRonin.FluentArgumentParser.Definition;

public class OptionalParameter : AnIndexableParameter
{
    object _default;
    string _description;
    public bool HasDefaultBeenSet { get; private set; }

    public string Description
    {
        get => string.IsNullOrEmpty(_description) ? $"default: {_default}": _description;
        set
        {
            _description = $"default: {value} ({_default})";
        }
    }

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