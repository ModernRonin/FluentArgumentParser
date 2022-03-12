using System;

namespace ModernRonin.FluentArgumentParser;

static class BooleanExtensions
{
    public static StringComparison StringComparison(this bool self) =>
        self
            ? System.StringComparison.InvariantCulture
            : System.StringComparison.InvariantCultureIgnoreCase;
}