using System;

namespace ModernRonin.FluentArgumentParser.Definition
{
    public abstract class AnIndexableParameter : AParameter
    {
        public int Index { get; set; }
        public Type Type { get; set; }
    }
}