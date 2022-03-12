using System;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Parsing;
using NUnit.Framework;

// ReSharper disable TestClassNameSuffixWarning

namespace ModernRonin.FluentArgumentParser.Tests.Regressions;

public class SomeVerb
{
    public string Name { get; set; }
}

public class AnotherVerb
{
    public string Id { get; set; }
}

[TestFixture]
public class Regression001
{
    [Test]
    public void Can_make_nullable_properties_optional_and_have_null_as_default()
    {
        // arrange
        IBindingCommandLineParser setupCommandLineParser()
        {
            var result = ParserFactory.Create("bla", "something");
            result.AddVerb<SomeVerb>().Rename("sv").Parameter(x => x.Name).MakeOptional();
            result.AddVerb<AnotherVerb>().Rename("av");
            return result;
        }

        var parser = setupCommandLineParser();
        // act
        var action = () => parser.Parse(Array.Empty<string>());
        // assert
        action.Should().NotThrow();
    }
}