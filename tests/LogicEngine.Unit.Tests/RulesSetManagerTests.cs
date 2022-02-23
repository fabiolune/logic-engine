using System;
using System.Collections.Generic;
using FluentAssertions;
using LogicEngine.Interfaces;
using LogicEngine.Models;
using Moq;
using NUnit.Framework;
using TinyFp;

namespace LogicEngine.Unit.Tests;

public class RulesSetManagerTests
{
    private Mock<IRulesSetCompiler> _compiler;
    private RulesSetManager<TestModel> _sut;
    private static readonly RulesSet Set = new();

    [SetUp]
    public void SetUp()
    {
        _compiler = new Mock<IRulesSetCompiler>();
        _sut = new RulesSetManager<TestModel>(_compiler.Object);
    }

    [Test]
    public void FirstMatching_WhenCompilerReturnsNone_ShouldReturnNone()
    {
        _compiler
            .Setup(_ => _.CompileLabeled<TestModel>(Set))
            .Returns(
                new CompiledLabeledRulesSet<TestModel>(
                    Option<KeyValuePair<string, Func<TestModel, Either<string, TinyFp.Unit>>>[]>.None()
                )
            );

        _sut.Set = Set;

        _sut.FirstMatching(new TestModel
            {
                IntProperty = 27,
                IntEnumerableProperty = new[] { 0, 1 }
            })
            .IsSome
            .Should()
            .BeFalse();
    }

    [Test]
    public void FirstMatching_WhenCompilerReturnsSome_ShouldReturnCodeOfFirstMatchedRule()
    {
        _compiler
            .Setup(_ => _.CompileLabeled<TestModel>(Set))
            .Returns(
                new CompiledLabeledRulesSet<TestModel>(
                    Option<KeyValuePair<string, Func<TestModel, Either<string, TinyFp.Unit>>>[]>
                        .Some(new KeyValuePair<string, Func<TestModel, Either<string, TinyFp.Unit>>>[]
                        {
                            new("first", _ => Either<string, TinyFp.Unit>.Left("failed")),
                            new("second", _ => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default)),
                            new("third", _ => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default))
                        })
                )
            );

        _sut.Set = Set;

        var result = _sut
            .FirstMatching(new TestModel
            {
                IntProperty = 27,
                IntEnumerableProperty = new[] { 0, 1 }
            });

        result
            .IsSome
            .Should()
            .BeTrue();

        result
            .OnSome(s => s.Should().Be("second"));
    }

    [Test]
    public void FirstMatching_WhenNoRulesSetIsSet_ShouldReturnNone()
    {
        var result = _sut
            .FirstMatching(new TestModel
            {
                IntProperty = 27,
                IntEnumerableProperty = new[] { 0, 1 }
            });

        result
            .IsNone
            .Should()
            .BeTrue();
    }
}