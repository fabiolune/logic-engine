using System;
using FluentAssertions;
using LogicEngine.Compilers;
using LogicEngine.Interfaces.Compilers;
using LogicEngine.Internals;
using LogicEngine.Models;
using Moq;
using NUnit.Framework;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine.Unit.Tests.Compilers;

public class RulesCatalogCompilerTests
{
    private Mock<IRulesSetCompiler> _mockCompiler;
    private RulesCatalogCompiler _sut;

    [SetUp]
    public void SetUp()
    {
        _mockCompiler = new Mock<IRulesSetCompiler>();
        _sut = new RulesCatalogCompiler(_mockCompiler.Object);
    }

    [Test]
    public void CompileCatalog_WhenRulesSetCompilerReturnsLeftItems_ShouldReturnThoseItems()
    {
        var set1 = new RulesSet
        (
            new[]
            {
                new Rule("x", OperatorType.Equal, "y", "code")
            }
        );

        var catalog = new RulesCatalog(new[] { set1 }, "some name");

        var compiledRule = new CompiledRule<TestModel>(_ => false, "whatever");

        var compiledRulesSet = new CompiledRulesSet<TestModel>(new[] { compiledRule });

        _mockCompiler.Setup(_ => _.Compile<TestModel>(set1)).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet));

        var result = _sut.Compile<TestModel>(catalog);

        result
            .Tee(r => r.IsSome.Should().BeTrue())
            .OnSome(c => 
            {
                c.Apply(It.IsAny<TestModel>()).Should().BeFalse();

                c.DetailedApply(It.IsAny<TestModel>())
                    .Tee(e => e.IsLeft.Should().BeTrue())
                    .OnLeft(s => s.Should().BeEquivalentTo("whatever"));
            });

        _mockCompiler
            .Verify(c => c.Compile<TestModel>(set1), Times.Once);
    }

    [Test]
    public void CompileCatalog_WhenRulesSetCompilerReturnsRightItems_ShouldReturnThoseItems()
    {
        var set1 = new RulesSet
        (
            new[]
            {
                new Rule("x", OperatorType.Equal, "y", "code")
            }
        );

        var catalog = new RulesCatalog(new[] { set1 }, "some name");

        var compiledRule = new CompiledRule<TestModel>(_ => true, "code");

        var compiledRulesSet = new CompiledRulesSet<TestModel>(new[] { compiledRule });

        _mockCompiler.Setup(_ => _.Compile<TestModel>(set1)).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet));

        var result = _sut.Compile<TestModel>(catalog);

        result
            .Tee(r => r.IsSome.Should().BeTrue())
            .OnSome(c =>
            {
                c.Apply(It.IsAny<TestModel>()).Should().BeTrue();

                c.DetailedApply(It.IsAny<TestModel>()).IsRight.Should().BeTrue();
            });

        _mockCompiler
            .Verify(c => c.Compile<TestModel>(set1), Times.Once);
    }

    [Test]
    public void CompileCatalog_WhenRulesSetsAreEmpty_ShouldReturnNone()
    {
        var catalog = new RulesCatalog(Array.Empty<RulesSet>(), "some name");

        var result = _sut.Compile<TestModel>(catalog);

        result.IsNone.Should().BeTrue();
    }

    [Test]
    public void CompileCatalog_WhenRulesSetsAreNull_ShouldReturnNone()
    {
        var catalog = new RulesCatalog(null, "some name");

        var result = _sut.Compile<TestModel>(catalog);

        result.IsNone.Should().BeTrue();
    }
}