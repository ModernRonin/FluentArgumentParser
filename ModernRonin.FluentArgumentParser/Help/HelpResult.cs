using ModernRonin.FluentArgumentParser.Parsing;

namespace ModernRonin.FluentArgumentParser.Help;

/// <summary>
///     Returned by <see cref="IBindingCommandLineParser.Parse" /> if the passed arguments contain an explicit request for
///     help
///     or if they were invalid.
/// </summary>
public class HelpResult
{
    /// <summary>
    ///     The help text to display to the user.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    ///     True if the arguments parsed were invalid, false if they contained an explicit request for help.
    /// </summary>
    public bool IsResultOfInvalidInput { get; set; }
}