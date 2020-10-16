using System.Collections.Generic;

namespace ModernRonin.FluentArgumentParser.Definition
{
    public interface IVerbContainer : IEnumerable<Verb>
    {
        AVerbContainer Parent { get; }
        IEnumerable<Verb> Verbs { get; }
        void Add(Verb verb);
    }
}