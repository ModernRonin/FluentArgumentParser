using System;
using System.Linq.Expressions;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using ModernRonin.FluentArgumentParser.Parsing;
using NSubstitute;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Parsing;

[TestFixture]
public class PropertyParameterBindingTests
{
    readonly string[] _shortNamesToBeExcluded =
    {
        "zulu",
        "yankee"
    };

    PropertyParameterBinding<SimpleTarget> Create<T>(Expression<Func<SimpleTarget, T>> accessor)
    {
        var namingStrategy = Substitute.For<INamingStrategy>();
        namingStrategy.GetShortName(accessor.PropertyInfo(), _shortNamesToBeExcluded).Returns("alpha");
        namingStrategy.GetLongName(accessor.PropertyInfo()).Returns("bravo");
        return new PropertyParameterBinding<SimpleTarget>(namingStrategy,
            accessor.PropertyInfo(), _shortNamesToBeExcluded);
    }

    [Test]
    public void
        Constructor_Initializes_Parameter_As_Flag_Filling_In_Short_And_Long_Name_And_Type_For_Booleans()
    {
        var underTest = Create(t => t.IsOn);

        underTest.Parameter.Should()
            .BeOfType<FlagParameter>()
            .Which.Should()
            .BeEquivalentTo(new FlagParameter
            {
                LongName = "bravo",
                ShortName = "alpha"
            });
    }

    [Test]
    public void
        Constructor_Initializes_Parameter_As_Optional_Filling_In_Short_And_Long_Name_And_Type_For_Properties_With_NonStandard_DefaultValue()
    {
        var underTest = Create(t => t.OptionalReal);

        underTest.Parameter.Should()
            .BeOfType<OptionalParameter>()
            .Which.Should()
            .BeEquivalentTo(new OptionalParameter
            {
                LongName = "bravo",
                ShortName = "alpha",
                Type = typeof(float),
                Default = 3.141f
            });
    }

    [Test]
    public void
        Constructor_Initializes_Parameter_As_Required_Filling_In_Short_And_Long_Name_And_Type_For_Enums()
    {
        var underTest = Create(t => t.Color);

        underTest.Parameter.Should()
            .BeOfType<RequiredParameter>()
            .Which.Should()
            .BeEquivalentTo(new RequiredParameter
            {
                LongName = "bravo",
                ShortName = "alpha",
                Type = typeof(Color)
            });
    }

    [Test]
    public void
        Constructor_Initializes_Parameter_As_Required_Filling_In_Short_And_Long_Name_And_Type_For_Floats()
    {
        var underTest = Create(t => t.Real);

        underTest.Parameter.Should()
            .BeOfType<RequiredParameter>()
            .Which.Should()
            .BeEquivalentTo(new RequiredParameter
            {
                LongName = "bravo",
                ShortName = "alpha",
                Type = typeof(float)
            });
    }

    [Test]
    public void
        Constructor_Initializes_Parameter_As_Required_Filling_In_Short_And_Long_Name_And_Type_For_Ints()
    {
        var underTest = Create(t => t.Number);

        underTest.Parameter.Should()
            .BeOfType<RequiredParameter>()
            .Which.Should()
            .BeEquivalentTo(new RequiredParameter
            {
                LongName = "bravo",
                ShortName = "alpha",
                Type = typeof(int)
            });
    }

    [Test]
    public void
        Constructor_Initializes_Parameter_As_Required_Filling_In_Short_And_Long_Name_And_Type_For_Strings()
    {
        var underTest = Create(t => t.Text);

        underTest.Parameter.Should()
            .BeOfType<RequiredParameter>()
            .Which.Should()
            .BeEquivalentTo(new RequiredParameter
            {
                LongName = "bravo",
                ShortName = "alpha",
                Type = typeof(string)
            });
    }

    [Test]
    public void DoesMatch_Returns_False_For_Other_PropertyInfo()
    {
        var underTest = Create(t => t.Text);
        var propertyInfo = typeof(SimpleTarget).GetProperty(nameof(SimpleTarget.Number));

        underTest.DoesMatch(propertyInfo).Should().BeFalse();
    }

    [Test]
    public void DoesMatch_Returns_True_For_Inherited_PropertyInfo()
    {
        var underTest = Create(t => t.InheritedProperty);
        var propertyInfo = typeof(TargetAncestor).GetProperty(nameof(TargetAncestor.InheritedProperty));

        underTest.DoesMatch(propertyInfo).Should().BeTrue();

        Expression<Func<SimpleTarget, string>> accessor = t => t.InheritedProperty;
        underTest.DoesMatch(accessor.PropertyInfo()).Should().BeTrue();
    }

    [Test]
    public void DoesMatch_Returns_True_For_Right_PropertyInfo()
    {
        var underTest = Create(t => t.Text);
        var propertyInfo = typeof(SimpleTarget).GetProperty(nameof(SimpleTarget.Text));

        underTest.DoesMatch(propertyInfo).Should().BeTrue();
    }

    [Test]
    public void Set_Fills_The_Target_Property_For_Bools()
    {
        var underTest = Create(t => t.IsOn);

        var target = new SimpleTarget();
        underTest.SetIn(target, true);

        target.IsOn.Should().BeTrue();
    }

    [Test]
    public void Set_Fills_The_Target_Property_For_Enums()
    {
        var underTest = Create(t => t.Color);

        var target = new SimpleTarget();
        underTest.SetIn(target, Color.Green);

        target.Color.Should().Be(Color.Green);
    }

    [Test]
    public void Set_Fills_The_Target_Property_For_Floats()
    {
        var underTest = Create(t => t.Real);

        var target = new SimpleTarget();
        underTest.SetIn(target, 3.141f);

        target.Real.Should().Be(3.141f);
    }

    [Test]
    public void Set_Fills_The_Target_Property_For_Ints()
    {
        var underTest = Create(t => t.Number);

        var target = new SimpleTarget();
        underTest.SetIn(target, 13);

        target.Number.Should().Be(13);
    }

    [Test]
    public void Set_Fills_The_Target_Property_For_Strings()
    {
        var underTest = Create(t => t.Text);

        var target = new SimpleTarget();
        underTest.SetIn(target, "charlie");

        target.Text.Should().Be("charlie");
    }
}