using System.Linq;
using FluentAssertions;
using FluentValidation.TestHelper;
using ModernRonin.FluentArgumentParser.Definition;
using ModernRonin.FluentArgumentParser.Validation;
using NUnit.Framework;

namespace ModernRonin.FluentArgumentParser.Tests.Validation;

[TestFixture]
public class VerbContainerValidatorTests
{
    static TestValidationResult<IVerbContainer> Test(IVerbContainer model) =>
        new VerbContainerValidator().TestValidate(model);

    [Test]
    public void Fails_if_nested_child_has_own_duplicate_named_children()
    {
        var grandparent = new Verb("grandpa")
        {
            new("daddy")
            {
                new("kiddo")
                {
                    new Verb("pet"),
                    new Verb("pet")
                }
            }
        };

        Test(grandparent).ShouldHaveAnyValidationError();
    }

    [Test]
    public void Fails_if_nested_child_has_own_validation_error()
    {
        var grandparent = new Verb("grandpa") { new("daddy") { new Verb() } };

        Test(grandparent).ShouldHaveAnyValidationError();
    }

    [Test]
    public void Fails_with_children_if_child_has_own_validation_error()
    {
        var parent = new Verb("parent") { new() { Name = null } };

        Test(parent).Errors.Single().PropertyName.Should().Be("Verbs[0].Name");
    }

    [Test]
    public void Fails_with_multiple_children_with_duplicate_names()
    {
        var parent = new Verb("parent")
        {
            new() { Name = "child1" },
            new() { Name = "child2" },
            new() { Name = "child1" }
        };

        Test(parent).ShouldHaveValidationErrorFor(c => c.Verbs);
    }

    [Test]
    public void Succeeds_with_multiple_children_with_different_names()
    {
        var parent = new Verb("parent")
        {
            new() { Name = "child1" },
            new() { Name = "child2" },
            new() { Name = "child3" }
        };

        Test(parent).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Succeeds_with_nested_children()
    {
        var grandparent = new Verb("grandpa") { new("daddy") { new Verb("kiddo") } };

        Test(grandparent).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Succeeds_with_single_child()
    {
        var parent = new Verb("parent") { new() { Name = "child" } };

        Test(parent).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Succeeds_without_children()
    {
        var verb = new Verb { Name = "verb" };

        Test(verb).ShouldNotHaveAnyValidationErrors();
    }
}