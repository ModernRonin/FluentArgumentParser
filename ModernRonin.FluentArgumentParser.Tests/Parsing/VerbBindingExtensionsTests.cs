using System;
using System.Linq;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using ModernRonin.FluentArgumentParser.Parsing;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Parsing;

[TestFixture]
public class VerbBindingExtensionsTests
{
    [SetUp]
    public void Setup()
    {
        // we don't use DefaultNamingStrategy to prove that the strategy is actually being used
        // we don't use nsubstitute because in this case this would actually be more work
        _namingStrategy = new TestNamingStrategy();
        _underTest = new LeafVerbBinding<SimpleTarget>(_namingStrategy);
    }

    INamingStrategy _namingStrategy;
    LeafVerbBinding<SimpleTarget> _underTest;

    [Test]
    public void Parameter_Returns_An_Object_Allowing_To_Change_The_Parameter()
    {
        _underTest.Bind();

        _underTest.Parameter(t => t.Number).MakeOptional();

        _underTest.Verb.Parameters.Single(p => p.LongName == "NUMBER")
            .Should()
            .BeOfType<OptionalParameter>();
        _underTest.Bindings.Single(b => b.Parameter.LongName == "NUMBER")
            .Parameter.Should()
            .BeOfType<OptionalParameter>();
    }

    [Test]
    public void Parameter_With_A_Get_Only_Property_Throws()
    {
        _underTest.Bind();
        Action action = () => _underTest.Parameter(t => t.IsNotOn);

        action.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Parameter_With_An_InternalVisibility_Property_Throws()
    {
        _underTest.Bind();
        Action action = () => _underTest.Parameter(t => t.InternalProperty);

        action.Should().Throw<ArgumentException>();
    }

    [Test]
    public void Parameter_Without_Prior_Call_To_Bind_Throws()
    {
        Action action = () => _underTest.Parameter(t => t.Number);

        action.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void Rename_Returns_Same_Instance()
    {
        _underTest.Bind();

        _underTest.Rename("changed").Should().BeSameAs(_underTest);
    }

    [Test]
    public void Rename_Sets_Verb_Name()
    {
        _underTest.Bind();

        _underTest.Rename("changed");

        _underTest.Verb.Name.Should().Be("changed");
    }

    [Test]
    public void Rename_Throws_If_Not_Bound()
    {
        Action action = () => _underTest.Rename("x");

        action.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void WithHelp_returns_same_instance()
    {
        _underTest.WithHelp("bla bla").Should().BeSameAs(_underTest);
    }

    [Test]
    public void WithHelp_sets_Verb_HelpText()
    {
        _underTest.WithHelp("bla bla");

        _underTest.Verb.HelpText.Should().Be("bla bla");
    }
}