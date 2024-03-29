﻿using System;
using System.Collections.Generic;
using System.Linq;
using ModernRonin.FluentArgumentParser.Help;

namespace ModernRonin.FluentArgumentParser.Parsing;

public class BindingCommandLineParser : IBindingCommandLineParser
{
    readonly IVerbFactory _factory;
    readonly IHelpAndErrorInterpreter _helpAndErrorInterpreter;
    readonly ICommandLineParser _parser;
    readonly List<IVerbBinding> _verbBindings = new();
    ILeafVerbBinding _defaultVerbBinding;

    public BindingCommandLineParser(ICommandLineParser parser,
        IVerbFactory factory,
        IHelpAndErrorInterpreter helpAndErrorInterpreter)
    {
        _parser = parser;
        _factory = factory;
        _helpAndErrorInterpreter = helpAndErrorInterpreter;
    }

    public string HelpOverview => _helpAndErrorInterpreter.GetHelpOverview(_parser);

    public ILeafVerbBinding<T> AddVerb<T>() where T : new()
    {
        var result = _factory.MakeLeafBinding<T>();
        Add(result);
        return result;
    }

    public ILeafVerbBinding<T> DefaultVerb<T>() where T : new()
    {
        var result = _factory.MakeLeafBinding<T>();
        _defaultVerbBinding = result;
        _parser.DefaultVerb = _defaultVerbBinding?.Verb;
        return result;
    }

    public IContainerVerbBinding AddContainerVerb<T>()
    {
        var result = _factory.MakeContainerBinding<T>();
        Add(result);
        return result;
    }

    public bool DoSkipValidation
    {
        get => _parser.DoSkipValidation;
        set => _parser.DoSkipValidation = value;
    }

    public object Parse(string[] args)
    {
        if (_defaultVerbBinding == default && !_verbBindings.Any())
        {
            throw new InvalidOperationException(
                $"At least one verb or the default verb need to be set up before calling {nameof(Parse)}");
        }

        var call = _parser.Parse(args);
        return help() ?? bind();

        object bind()
        {
            var binding = _defaultVerbBinding?.Verb == call.Verb
                ? _defaultVerbBinding
                : _verbBindings.Find(call.Verb);

            // ReSharper disable once PossibleNullReferenceException - at this point binding cannot be null because call.Verb cannot be null
            return binding.Create(call);
        }

        HelpResult help() => _helpAndErrorInterpreter.Interpret(call, _parser);
    }

    void Add(IVerbBinding result)
    {
        _verbBindings.Add(result);
        _parser.Add(result.Verb);
    }
}