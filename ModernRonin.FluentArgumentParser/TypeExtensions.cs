using System;

namespace ModernRonin.FluentArgumentParser;

public static class TypeExtensions
{
    public static object DefaultValue(this Type self) =>
        self.IsValueType ? Activator.CreateInstance(self) : null;
}