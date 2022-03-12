using System.Collections.Generic;

namespace ModernRonin.FluentArgumentParser.Parsing;

/// <summary>
///     You will never have to directly work this interface, but you will work with extension methods on it.
/// </summary>
public interface ILeafVerbBinding : IVerbBinding
{
    /// <summary>
    ///     Internal use only.
    /// </summary>
    IEnumerable<IPropertyParameterBinding> Bindings { get; }

    /// <summary>
    ///     Internal use only.
    /// </summary>
    bool IsBound { get; }

    /// <summary>
    ///     Internal use only.
    /// </summary>
    object Create(VerbCall call);
}

/// <inheritdoc cref="ILeafVerbBinding" />
// ReSharper disable once UnusedTypeParameter - it's there to give a hint to the compiler so the user doesn't have to supply it (when used with the extension methods)
public interface ILeafVerbBinding<T> : ILeafVerbBinding { }