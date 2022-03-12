using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using MoreLinq;

namespace ModernRonin.FluentArgumentParser.Parsing;

public class LeafVerbBinding<TTarget> : ILeafVerbBinding<TTarget> where TTarget : new()
{
    readonly List<PropertyParameterBinding<TTarget>> _bindings = new();

    readonly INamingStrategy _namingStrategy;
    public LeafVerbBinding(INamingStrategy namingStrategy) => _namingStrategy = namingStrategy;

    public IEnumerable<IPropertyParameterBinding> Bindings => _bindings;

    public Verb Verb { get; } = new();

    public object Create(VerbCall call)
    {
        var result = new TTarget();
        Fill(result, call);
        return result;
    }

    public void Bind()
    {
        var usedShortNames = new HashSet<string>();

        var type = typeof(TTarget);
        Verb.Name = _namingStrategy.GetVerbName(type);
        _bindings.AddRange(type.GetProperties()
            .Where(p => p.CanWrite && p.CanRead)
            .Select(toBinding)
            .OrderBy(b => priorityOf(b.Parameter)));
        //_bindings.Sort(byParameterType);
        _bindings
            .Where(b => b.Parameter is AnIndexableParameter)
            .ForEach((b, i) => ((AnIndexableParameter)b.Parameter).Index = i);
        Verb.Parameters = _bindings.Select(p => p.Parameter).ToArray();

        PropertyParameterBinding<TTarget> toBinding(PropertyInfo propertyInfo)
        {
            var result = new PropertyParameterBinding<TTarget>(_namingStrategy, propertyInfo,
                usedShortNames.ToArray());
            usedShortNames.Add(result.Parameter.ShortName);
            return result;
        }

        int priorityOf(AParameter parameter) =>
            parameter switch
            {
                RequiredParameter _ => 0,
                OptionalParameter _ => 1,
                _ => 2
            };
        //int byParameterType(IPropertyParameterBinding lhs, IPropertyParameterBinding rhs)
        //{

        //}
    }

    public IEnumerable<IVerbBinding> ThisAndChildren
    {
        get { yield return this; }
    }

    public void Fill(TTarget instance, VerbCall call)
    {
        if (Verb != call.Verb) throw new ArgumentException("Verb does not match", nameof(call));

        this.GuardAgainstMissingBindCall();

        call.Arguments.ForEach(set);

        void set(Argument argument)
        {
            var binding = _bindings.Single(b => b.Parameter == argument.Parameter);
            binding.SetIn(instance, argument.Value);
        }
    }
}