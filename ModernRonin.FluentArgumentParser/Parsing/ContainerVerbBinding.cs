using System.Collections.Generic;
using System.Linq;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;

namespace ModernRonin.FluentArgumentParser.Parsing;

public class ContainerVerbBinding<TTarget> : IContainerVerbBinding
{
    readonly List<IVerbBinding> _children = new();
    readonly IVerbFactory _factory;
    readonly INamingStrategy _namingStrategy;

    public ContainerVerbBinding(IVerbFactory factory, INamingStrategy namingStrategy)
    {
        _factory = factory;
        _namingStrategy = namingStrategy;
    }

    public Verb Verb { get; } = new();

    public void Bind()
    {
        var type = typeof(TTarget);
        Verb.Name = _namingStrategy.GetVerbName(type);
    }

    public IEnumerable<IVerbBinding> ThisAndChildren => _children.Append(this);

    public ILeafVerbBinding<T> AddVerb<T>() where T : new()
    {
        var result = _factory.MakeLeafBinding<T>();
        Add(result);
        return result;
    }

    public IContainerVerbBinding AddContainerVerb<T>()
    {
        var result = _factory.MakeContainerBinding<T>();
        Add(result);
        return result;
    }

    void Add(IVerbBinding result)
    {
        Verb.Add(result.Verb);
        _children.Add(result);
    }
}