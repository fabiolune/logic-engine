using System;
using System.Collections.Generic;
using FluentAssertions;
using LogicEngine.Interfaces;
using LogicEngine.Internals;
using LogicEngine.Models;
using Moq;
using NUnit.Framework;
using TinyFp;
using static TinyFp.Prelude;

namespace LogicEngine.Unit.Tests;

public class RulesSetCompilerTests
{
    private RulesSetCompiler _sut;
    private Mock<ISingleRuleCompiler> _mockCompiler;

    [SetUp]
    public void SetUp()
    {
        _mockCompiler = new Mock<ISingleRuleCompiler>();
        _sut = new RulesSetCompiler(_mockCompiler.Object);
    }

    [Test]
    public void Compile_WhenRulesSetCompilerReturnsSomeAndNone_ShouldReturnCompiledRulesSetOnlyForSome()
    {
        var rule1 = new Rule("x", OperatorType.Equal, "y");
        var rule2 = new Rule("a", OperatorType.Equal, "b");

        var set = new RulesSet
        {
            Description = "whatever",
            Rules = new []
            {
                rule1,
                rule2
            }
        };

        var func1 = new Func<TestModel, Either<string, TinyFp.Unit>>(_ => Either<string, TinyFp.Unit>.Left("whatever"));

        var compiledRule = new CompiledRule<TestModel>(func1);

        _mockCompiler
            .Setup(_ => _.Compile<TestModel>(rule1))
            .Returns(Some(compiledRule));

        _mockCompiler
            .Setup(_ => _.Compile<TestModel>(rule2))
            .Returns(Option<CompiledRule<TestModel>>.None);

        var result = _sut.Compile<TestModel>(set);

        result.Should().BeEquivalentTo(new CompiledRulesSet<TestModel>(new[] { func1 }));
    }

    [Test]
    public void
        CompileLabeled_WhenRulesSetCompilerReturnsSomeAndNone_ShouldReturnCompiledRulesSetOnlyForSomeWithLabels()
    {
        var rule1 = new Rule("x", OperatorType.Equal, "y")
        {
            Code = "some_code"
        };
        var rule2 = new Rule("a", OperatorType.Equal, "b");

        var set = new RulesSet
        {
            Description = "whatever",
            Rules = new[]
            {
                rule1,
                rule2
            }
        };

        var func1 = new Func<TestModel, Either<string, TinyFp.Unit>>(_ => Either<string, TinyFp.Unit>.Left("whatever"));

        var compiledRule = new CompiledRule<TestModel>(func1);

        _mockCompiler
            .Setup(_ => _.Compile<TestModel>(rule1))
            .Returns(Some(compiledRule));

        _mockCompiler
            .Setup(_ => _.Compile<TestModel>(rule2))
            .Returns(Option<CompiledRule<TestModel>>.None);

        var result = _sut.CompileLabeled<TestModel>(set);

        result
            .Should()
            .NotBeNull();

        result
            .Executables
            .IsSome
            .Should()
            .BeTrue();

        result
            .Executables
            .OnSome(_ => _.Should().BeEquivalentTo(
                new List<KeyValuePair<string, Func<TestModel, Either<string, TinyFp.Unit>>>>
                {
                    new("some_code", func1)
                }));
    }

    [Test]
    public void
        CompileLabeled_WhenRulesSetCompilerReturnsOnlyNone_ShouldReturnCompiledRulesSetWithNone()
    {
        var rule1 = new Rule("x", OperatorType.Equal, "y")
        {
            Code = "some_code"
        };
        var rule2 = new Rule("a", OperatorType.Equal, "b");

        var set = new RulesSet
        {
            Description = "whatever",
            Rules = new[]
            {
                rule1,
                rule2
            }
        };

        _mockCompiler
            .Setup(_ => _.Compile<TestModel>(rule1))
            .Returns(Option<CompiledRule<TestModel>>.None);

        _mockCompiler
            .Setup(_ => _.Compile<TestModel>(rule2))
            .Returns(Option<CompiledRule<TestModel>>.None);

        var result = _sut.CompileLabeled<TestModel>(set);

        result
            .Should()
            .NotBeNull();

        result
            .Executables
            .IsNone
            .Should()
            .BeTrue();
    }

    [Test]
    public void
        CompileLabeled_WhenRulesSetHasNoRules_ShouldReturnCompiledRulesSetWithNone()
    {
        var set = new RulesSet
        {
            Description = "whatever",
            Rules = Array.Empty<Rule>()
        };

        var result = _sut.CompileLabeled<TestModel>(set);

        result
            .Should()
            .NotBeNull();

        result
            .Executables
            .IsNone
            .Should()
            .BeTrue();
    }

    [Test]
    public void
        CompileLabeled_WhenRulesAreNull_ShouldReturnCompiledRulesSetWithNone()
    {
        var set = new RulesSet
        {
            Description = "whatever",
            Rules = null
        };

        var result = _sut.CompileLabeled<TestModel>(set);

        result
            .Should()
            .NotBeNull();

        result
            .Executables
            .IsNone
            .Should()
            .BeTrue();
    }

    [Test]
    public void
        CompileLabeled_WhenRulesAreMissing_ShouldReturnCompiledRulesSetWithNone()
    {
        var set = new RulesSet
        {
            Description = "whatever"
        };

        var result = _sut.CompileLabeled<TestModel>(set);

        result
            .Should()
            .NotBeNull();

        result
            .Executables
            .IsNone
            .Should()
            .BeTrue();
    }

    [Test]
    public void
        CompileLabeled_WhenRulesHasRulesWithNullCode_ShouldReturnListWithEmptyKeys()
    {
        var rule1 = new Rule("x", OperatorType.Equal, "y");
        var rule2 = new Rule("a", OperatorType.Equal, "b");

        var set = new RulesSet
        {
            Description = "set description",
            Rules = new[]
            {
                rule1,
                rule2
            }
        };

        var func1 = new Func<TestModel, Either<string, TinyFp.Unit>>(_ => Either<string, TinyFp.Unit>.Left("whatever"));

        var compiledRule = new CompiledRule<TestModel>(func1);

        _mockCompiler
            .Setup(_ => _.Compile<TestModel>(rule1))
            .Returns(Some(compiledRule));

        _mockCompiler
            .Setup(_ => _.Compile<TestModel>(rule2))
            .Returns(Option<CompiledRule<TestModel>>.None);

        var result = _sut.CompileLabeled<TestModel>(set);

        result
            .Should()
            .NotBeNull();

        result
            .Executables
            .IsSome
            .Should()
            .BeTrue();

        result
            .Executables
            .OnSome(_ => _.Should().BeEquivalentTo(
                new List<KeyValuePair<string, Func<TestModel, Either<string, TinyFp.Unit>>>>
                {
                    new("", func1)
                }));
    }
}