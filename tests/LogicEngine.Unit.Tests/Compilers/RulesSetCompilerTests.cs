using FluentAssertions;
using LogicEngine.Compilers;
using LogicEngine.Interfaces.Compilers;
using LogicEngine.Internals;
using LogicEngine.Models;
using Moq;
using NUnit.Framework;
using System;
using TinyFp;
using TinyFp.Extensions;
using static TinyFp.Prelude;

namespace LogicEngine.Unit.Tests.Compilers;

public class RulesSetCompilerTests
{
    private RulesSetCompiler _sut;
    private Mock<IRuleCompiler> _mockCompiler;

    [SetUp]
    public void SetUp()
    {
        _mockCompiler = new Mock<IRuleCompiler>();
        _sut = new RulesSetCompiler(_mockCompiler.Object);
    }

    [Test]
    public void Compile_WhenRulesSetCompilerReturnsSomeAndNone_ShouldReturnCompiledRulesSetOnlyForSome()
    {
        var rule1 = new Rule("x", OperatorType.Equal, "y", "code1");
        var rule2 = new Rule("a", OperatorType.Equal, "b", "code2");

        var set = new RulesSet
        (
            new[]
            {
                rule1,
                rule2
            }
        );

        var compiledRule = new CompiledRule<TestModel>(_ => false, "whatever");

        _mockCompiler
            .Setup(_ => _.Compile<TestModel>(rule1))
            .Returns(Some(compiledRule));

        _mockCompiler
            .Setup(_ => _.Compile<TestModel>(rule2))
            .Returns(Option<CompiledRule<TestModel>>.None);

        var result = _sut.Compile<TestModel>(set);

        result.IsSome.Should().BeTrue();

        result.OnSome(rs =>
        {
            rs.Apply(It.IsAny<TestModel>()).Should().BeFalse();

            rs.DetailedApply(It.IsAny<TestModel>())
                .Tee(a => a.IsLeft.Should().BeTrue())
                .OnLeft(s => s.Should().BeEquivalentTo("whatever"));
        });
    }

    [Test]
    public void Compile_WhenRulesSetCompilerReturnsNone_ShouldReturnNone()
    {
        var rule1 = new Rule("x", OperatorType.Equal, "y", "code1");
        var rule2 = new Rule("a", OperatorType.Equal, "b", "code2");

        var set = new RulesSet
        (
            new[]
            {
                rule1,
                rule2
            }
        );

        _mockCompiler
            .Setup(_ => _.Compile<TestModel>(It.IsAny<Rule>()))
            .Returns(Option<CompiledRule<TestModel>>.None);

        var result = _sut.Compile<TestModel>(set);

        result.IsNone.Should().BeTrue();
    }

    [Test]
    public void Compile_WhenRulesAreEmpty_ShouldReturnNone()
    {
        var set = new RulesSet(Array.Empty<Rule>());

        var result = _sut.Compile<TestModel>(set);

        result.IsNone.Should().BeTrue();
    }

    [Test]
    public void Compile_WhenRulesIsNull_ShouldReturnNone()
    {
        var set = new RulesSet(null);

        var result = _sut.Compile<TestModel>(set);

        result.IsNone.Should().BeTrue();
    }
}