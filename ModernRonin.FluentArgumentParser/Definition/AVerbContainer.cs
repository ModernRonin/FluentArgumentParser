using System.Collections;
using System.Collections.Generic;

namespace ModernRonin.FluentArgumentParser.Definition;

public abstract class AVerbContainer : IVerbContainer
{
    readonly IList<Verb> _verbs = new List<Verb>();
    public AVerbContainer Parent { get; private set; }
    public IEnumerable<Verb> Verbs => _verbs;
    public IEnumerator<Verb> GetEnumerator() => _verbs.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_verbs).GetEnumerator();

    public void Add(Verb verb)
    {
        verb.Parent = this;
        _verbs.Add(verb);
    }
}