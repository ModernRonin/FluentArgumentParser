namespace ModernRonin.FluentArgumentParser.Extensibility;

/// <summary>
///     <para>
///         Implement this interface and set <see cref="Services.ArgumentPreprocessor" /> to your implementation
///         if you want to pre-process arguments before they are fed into verbs.
///     </para>
///     One example use case for this would be if you are not happy with the quoting mechanism used by the combination of
///     shell and CLR runtime and want to customize it.
/// </summary>
public interface IArgumentPreprocessor
{
    string Process(string what);
}