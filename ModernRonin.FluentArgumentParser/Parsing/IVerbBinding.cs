using System.Collections.Generic;
using ModernRonin.FluentArgumentParser.Definition;

namespace ModernRonin.FluentArgumentParser.Parsing
{
    public interface IVerbBinding
    {
        Verb Verb { get; }
        IEnumerable<IVerbBinding> ThisAndChildren { get; }
        void Bind();
    }
}