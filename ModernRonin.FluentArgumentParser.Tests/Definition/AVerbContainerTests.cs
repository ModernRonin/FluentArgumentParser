using System.Linq;
using FluentAssertions;
using ModernRonin.FluentArgumentParser.Definition;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Definition
{
    [TestFixture]
    public class AVerbContainerTests
    {
        public class Testable : AVerbContainer { }

        [Test]
        public void Add_adds_verb()
        {
            var underTest = new Testable();

            var kid = new Verb("alpha");
            underTest.Add(kid);

            underTest.Single().Should().BeSameAs(kid);
        }

        [Test]
        public void Add_sets_parent_of_added_Verb_to_self()
        {
            var underTest = new Testable();

            var kid = new Verb("alpha");
            underTest.Add(kid);

            kid.Parent.Should().BeSameAs(underTest);
        }

        [Test]
        public void Is_empty_by_default() => new Testable().Should().BeEmpty();

        [Test]
        public void Parent_is_null_by_default() => new Testable().Parent.Should().BeNull();
    }
}