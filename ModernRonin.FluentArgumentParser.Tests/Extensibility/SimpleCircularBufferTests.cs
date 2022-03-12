using System.Linq;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Extensibility;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Extensibility;

[TestFixture]
public class SimpleCircularBufferTests
{
    [Test]
    public void Enumeration_wraps_around()
    {
        var underTest = new SimpleCircularBuffer<string>("alpha", "bravo", "charlie");

        underTest.Take(10)
            .Should()
            .Equal("alpha", "bravo", "charlie", "alpha", "bravo", "charlie", "alpha", "bravo", "charlie",
                "alpha");
    }
}