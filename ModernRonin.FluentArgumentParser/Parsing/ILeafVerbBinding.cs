using System.Collections.Generic;

namespace ModernRonin.FluentArgumentParser.Parsing
{
    public interface ILeafVerbBinding : IVerbBinding
    {
        IEnumerable<IPropertyParameterBinding> Bindings { get; }
        object Create(VerbCall call);
    }

    // ReSharper disable once UnusedTypeParameter - it's there to give a hint to the compiler so the user doesn't have to supply it (when used with the extension methods)
    public interface ILeafVerbBinding<T> : ILeafVerbBinding { }
}