using System;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Extensibility;

public abstract class ATypeFormatter
    : ITypeFormatter
{
    public string Format(AParameter parameter) =>
        parameter switch
        {
            FlagParameter _ => Format(typeof(bool)),
            AnIndexableParameter i => Format(i.Type),
            _ => throw new NotImplementedException()
        };

    protected abstract string Format(Type type);
}