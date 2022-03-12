using System;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Parsing;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Parsing;

[TestFixture]
public class ParameterBindingConfigurerTests
{
    [TestCase(typeof(RequiredParameter))]
    [TestCase(typeof(FlagParameter))]
    public void WithDefault_Throws_If_Parameter_Is_Not_Optional(Type type)
    {
        var underTest =
            new ParameterBindingConfigurer<int>(() => (AParameter)Activator.CreateInstance(type),
                _ => { });
        Action action = () => underTest.WithDefault(13);

        action.Should().Throw<InvalidOperationException>();
    }

    [TestCase(typeof(RequiredParameter))]
    [TestCase(typeof(OptionalParameter))]
    [TestCase(typeof(FlagParameter))]
    public void WithLongName_Sets_LongName(Type type)
    {
        var parameter = (AParameter)Activator.CreateInstance(type);
        var underTest = new ParameterBindingConfigurer<string>(() => parameter, _ => { });

        underTest.WithLongName("alpha");

        parameter.LongName.Should().Be("alpha");
    }

    [TestCase(typeof(RequiredParameter))]
    [TestCase(typeof(OptionalParameter))]
    [TestCase(typeof(FlagParameter))]
    public void WithShortName_Sets_LongName(Type type)
    {
        var parameter = (AParameter)Activator.CreateInstance(type);
        var underTest = new ParameterBindingConfigurer<string>(() => parameter, _ => { });

        underTest.WithShortName("alpha");

        parameter.ShortName.Should().Be("alpha");
    }

    [Test]
    public void ExpectAt_Returns_Self()
    {
        var underTest = new ParameterBindingConfigurer<string>(() => new RequiredParameter(), _ => { });

        underTest.ExpectAt(13).Should().BeSameAs(underTest);
    }

    [Test]
    public void ExpectAt_Sets_Index()
    {
        var parameter = new RequiredParameter();
        var underTest = new ParameterBindingConfigurer<string>(() => parameter, _ => { });

        underTest.ExpectAt(13);

        parameter.Index.Should().Be(13);
    }

    [Test]
    public void ExpectAt_Throws_For_FlagParameters()
    {
        var underTest = new ParameterBindingConfigurer<bool>(() => new FlagParameter(), _ => { });
        Action action = () => underTest.ExpectAt(13);

        action.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void MakeOptional_Does_Nothing_For_FlagParameters()
    {
        var wasSetterCalled = false;
        var underTest =
            new ParameterBindingConfigurer<bool>(() => new FlagParameter(), _ => wasSetterCalled = true);

        underTest.MakeOptional();

        wasSetterCalled.Should().BeFalse();
    }

    [Test]
    public void MakeOptional_Does_Nothing_For_Parameters_Already_Optional()
    {
        var wasSetterCalled = false;
        var underTest =
            new ParameterBindingConfigurer<int>(() => new OptionalParameter(),
                _ => wasSetterCalled = true);

        underTest.MakeOptional();

        wasSetterCalled.Should().BeFalse();
    }

    [Test]
    public void MakeOptional_Returns_Self()
    {
        var underTest = new ParameterBindingConfigurer<bool>(() => new FlagParameter(), _ => { });

        underTest.MakeOptional().Should().BeSameAs(underTest);
    }

    [Test]
    public void MakeOptional_Sets_DefaultValue_For_PrimitiveTypes_To_That_Types_Default()
    {
        AParameter parameter = new RequiredParameter
        {
            Index = 2,
            LongName = "alpha",
            ShortName = "bravo",
            Type = typeof(int)
        };

        var underTest = new ParameterBindingConfigurer<int>(() => parameter, v => parameter = v);

        underTest.MakeOptional();

        ((OptionalParameter)parameter).Default.Should().Be(0);
    }

    [Test]
    public void MakeOptional_Switches_Required_To_Optional_Parameter()
    {
        AParameter parameter = new RequiredParameter
        {
            Index = 2,
            LongName = "alpha",
            ShortName = "bravo",
            Type = typeof(int)
        };

        var underTest = new ParameterBindingConfigurer<int>(() => parameter, v => parameter = v);

        underTest.MakeOptional();

        parameter.Should()
            .BeOfType<OptionalParameter>()
            .Which.Should()
            .BeEquivalentTo(new OptionalParameter
            {
                Index = 2,
                LongName = "alpha",
                ShortName = "bravo",
                Type = typeof(int)
            }, cfg => cfg.Excluding(o => o.Default));
    }

    [Test]
    public void WithDefault_Returns_Self()
    {
        var underTest = new ParameterBindingConfigurer<int>(() => new OptionalParameter(), _ => { });

        underTest.WithDefault(13).Should().BeSameAs(underTest);
    }

    [Test]
    public void WithDefault_Sets_DefaultValue()
    {
        var parameter = new OptionalParameter();
        var underTest = new ParameterBindingConfigurer<int>(() => parameter, _ => { });

        underTest.WithDefault(13);

        parameter.Default.Should().Be(13);
    }

    [Test]
    public void WithHelp_returns_self()
    {
        var underTest = new ParameterBindingConfigurer<string>(() => new RequiredParameter(), _ => { });

        underTest.WithHelp("bla bla").Should().BeSameAs(underTest);
    }

    [Test]
    public void WithHelp_sets_HelpText()
    {
        var parameter = new RequiredParameter();
        var underTest = new ParameterBindingConfigurer<string>(() => parameter, _ => { });

        underTest.WithHelp("bla bla");

        parameter.HelpText.Should().Be("bla bla");
    }

    [Test]
    public void WithLongName_Returns_Self()
    {
        var underTest = new ParameterBindingConfigurer<string>(() => new RequiredParameter(), _ => { });

        underTest.WithLongName("alpha").Should().BeSameAs(underTest);
    }

    [Test]
    public void WithShortName_Returns_Self()
    {
        var underTest = new ParameterBindingConfigurer<string>(() => new RequiredParameter(), _ => { });

        underTest.WithShortName("alpha").Should().BeSameAs(underTest);
    }
}