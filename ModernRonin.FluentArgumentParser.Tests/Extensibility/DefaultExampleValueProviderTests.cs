using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Extensibility;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Extensibility;

[TestFixture]
public class DefaultExampleValueProviderTests
{
    static RequiredParameter TypedParameter<T>() => new() { Type = typeof(T) };

    enum MyEnum
    {
        Aleph,
        Bet,
        Gimel
    }

    [Test]
    public void For_byte_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<byte>();

        for (var i = 0; i < 15; ++i) underTest.For(parameter).Should().Be(10 * (1 + i % 10));
    }

    [Test]
    public void For_decimal_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<decimal>();

        underTest.For(parameter).Should().Be(1.1m);
    }

    [Test]
    public void For_double_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<double>();

        ((double)underTest.For(parameter)).Should().BeApproximately(1.1d, 0.001d);
    }

    [Test]
    public void For_enums_takes_the_last_value_stringified()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<MyEnum>();

        underTest.For(parameter).Should().Be("Gimel");
    }

    [Test]
    public void For_float_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<float>();

        underTest.For(parameter).Should().Be(1.1f);
    }

    [Test]
    public void For_int_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<int>();

        for (var i = 0; i < 15; ++i) underTest.For(parameter).Should().Be(10 * (1 + i % 10));
    }

    [Test]
    public void For_long_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<long>();

        for (var i = 0; i < 15; ++i) underTest.For(parameter).Should().Be(10 * (1 + i % 10));
    }

    [Test]
    public void For_sbyte_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<sbyte>();

        for (var i = 0; i < 15; ++i) underTest.For(parameter).Should().Be(10 * (1 + i % 10));
    }

    [Test]
    public void For_short_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<short>();

        for (var i = 0; i < 15; ++i) underTest.For(parameter).Should().Be(10 * (1 + i % 10));
    }

    [Test]
    public void For_string_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<string>();

        underTest.For(parameter).Should().Be("alpha");

        for (var i = 0; i < 27; ++i) underTest.For(parameter);

        underTest.For(parameter).Should().Be("charlie");
    }

    [Test]
    public void For_uint_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<uint>();

        for (var i = 0; i < 15; ++i) underTest.For(parameter).Should().Be(10 * (1 + i % 10));
    }

    [Test]
    public void For_ulong_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<ulong>();

        for (var i = 0; i < 15; ++i) underTest.For(parameter).Should().Be(10 * (1 + i % 10));
    }

    [Test]
    public void For_ushort_parameters()
    {
        var underTest = new DefaultExampleValueProvider();
        var parameter = TypedParameter<ushort>();

        for (var i = 0; i < 15; ++i) underTest.For(parameter).Should().Be(10 * (1 + i % 10));
    }
}