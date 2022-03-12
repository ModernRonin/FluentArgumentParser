using ModernRonin.FluentArgumentParser.Help;

namespace ModernRonin.FluentArgumentParser.Parsing;

/// <summary>
///     <para>
///         The high-level construct to use for argument parsing.
///         A parser can either have a default-verb (if your app doesn't need explicit verbs)
///         or multiple verbs.
///     </para>
///     <para>
///         Verbs are regular POCOs you define and register with the parser.
///         In many scenarios, especially if you don't mind the default naming rules, you
///         can work by just adding the verbs and call <see cref="Parse" />.
///     </para>
///     <para>
///         If you want/need to fine-tune things, you can use the fluent API of the return values
///         of <see cref="AddVerb{T}" /> and <see cref="DefaultVerb{T}" />.
///     </para>
///     <para>
///         You can get an instance of a parser using <see cref="ParserFactory" />. That class also has overloads
///         allowing you to further customize the parser's behavior, for example how it espects arguments to be formatted.
///     </para>
/// </summary>
public interface IBindingCommandLineParser
{
    /// <summary>
    ///     Gets the overview help text, in case you need to display it without the user providing any input yet.
    /// </summary>
    string HelpOverview { get; }

    /// <summary>
    ///     Set this to true to prevent your verb and parameter configuration from being validated. This exists only
    ///     because there seem to be situations when the library used for validation generates MissingMethodExceptions.
    ///     <para>Use with care - this is bound to be removed in the future once the actual problem has been fixed.</para>
    /// </summary>
    bool DoSkipValidation { get; set; }

    /// <summary>
    ///     Use this if your app supports verbs. The type parameter is a regular POCO. Use the return value to further
    ///     customize how FluentArgumentParser treats this verb.
    /// </summary>
    ILeafVerbBinding<T> AddVerb<T>() where T : new();

    /// <summary>
    ///     Use this if your app doesn't support verbs. The type parameter is a regular POCO. Use the return value to further
    ///     customize how FluentArgumentParser treats the parameters discovered from your POCO.
    /// </summary>
    ILeafVerbBinding<T> DefaultVerb<T>() where T : new();

    /// <summary>
    ///     If you have nested verbs, this is what you call to define a container verb. Container verbs can have children, but
    ///     no parameters.
    /// </summary>
    IContainerVerbBinding AddContainerVerb<T>();

    /// <summary>
    ///     Call this to run the parser. The return value will be either one of the POCOs you added with
    ///     <see cref="AddVerb{T}" /> or <see cref="DefaultVerb{T}" />, or <see cref="HelpResult" />.
    ///     The later is returned when the user explicitly requests help or when they supply invalid verbs or arguments.
    /// </summary>
    object Parse(string[] args);
}