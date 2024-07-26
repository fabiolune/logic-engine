using AutoBogus;
using LogicEngine.Compilers;
using LogicEngine.Interfaces.Compilers;
using LogicEngine.Internals;
using LogicEngine.Models;
using System;
using TinyFp;
using TinyFp.Extensions;
using static TinyFp.Prelude;

namespace LogicEngine.Unit.Tests.Compilers;

public class RulesSetCompilerTests
{
    private RulesSetCompiler _sut;
    private IRuleCompiler _mockCompiler;

    [SetUp]
    public void SetUp()
    {
        _mockCompiler = Substitute.For<IRuleCompiler>();
        _sut = new RulesSetCompiler(_mockCompiler);
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
            },
            "ruleset 1"
        );

        var compiledRule = new CompiledRule<TestModel>(_ => false, "whatever");

        _mockCompiler
            .Compile<TestModel>(rule1)
            .Returns(Some(compiledRule));

        _mockCompiler
            .Compile<TestModel>(rule2)
            .Returns(Option<CompiledRule<TestModel>>.None());

        var result = _sut.Compile<TestModel>(set);

        result.IsSome.Should().BeTrue();

        result.OnSome(rs =>
        {
            var item = new AutoFaker<TestModel>().Generate();

            rs.Apply(item).Should().BeFalse();

            rs.DetailedApply(item)
                .Tee(a => a.IsLeft.Should().BeTrue())
                .OnLeft(s => s.Should().BeEquivalentTo("whatever"));

            rs.FirstMatching(item)
                .IsNone.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRulesSetCompilerReturnsSome_ShouldReturnCompiledRulesSetOnlyForSomeAndFirstMatchingShouldBeSome()
    {
        var rule1 = new Rule("x", OperatorType.Equal, "y", "code1");
        var rule2 = new Rule("a", OperatorType.Equal, "b", "code2");

        var set = new RulesSet
        (
            new[]
            {
                rule1,
                rule2
            },
            "ruleset 1"
        );

        var firstExecuted = false;
        var secondExecuted = false;

        var compiledRule1 = new CompiledRule<TestModel>(_ => true.Tee(_ => firstExecuted = true), "code 1");
        var compiledRule2 = new CompiledRule<TestModel>(_ => true.Tee(_ => secondExecuted = true), "code 2");

        _mockCompiler
            .Compile<TestModel>(rule1)
            .Returns(Some(compiledRule1));

        _mockCompiler
            .Compile<TestModel>(rule2)
            .Returns(Some(compiledRule2));

        var result = _sut.Compile<TestModel>(set);

        result.IsSome.Should().BeTrue();

        result.OnSome(rs =>
        {
            var item = new AutoFaker<TestModel>().Generate();

            rs.Apply(item).Should().BeTrue();

            rs.DetailedApply(item)
                .Tee(a => a.IsRight.Should().BeTrue());

            rs.FirstMatching(item)
                .Tee(s => s.IsSome.Should().BeTrue())
                .OnSome(s => s.Should().Be("code 1"));

            firstExecuted.Should().BeTrue();
            secondExecuted.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRulesSetCompilerReturnsSome_ShouldReturnCompiledRulesSetOnlyForSomeAndFirstMatchingShouldBeSomeWithProperExecutions()
    {
        var rule1 = new Rule("x", OperatorType.Equal, "y", "code1");
        var rule2 = new Rule("a", OperatorType.Equal, "b", "code2");
        var rule3 = new Rule("l", OperatorType.Equal, "m", "code3");

        var set = new RulesSet
        (
            new[]
            {
                rule1,
                rule2
            },
            "ruleset 1"
        );

        var firstExecuted = false;
        var secondExecuted = false;
        var thirdExecuted = false;

        var compiledRule1 = new CompiledRule<TestModel>(_ => true.Tee(_ => firstExecuted = true), "code 1");
        var compiledRule2 = new CompiledRule<TestModel>(_ => false.Tee(_ => secondExecuted = true), "code 2");
        var compiledRule3 = new CompiledRule<TestModel>(_ => true.Tee(_ => thirdExecuted = true), "code 3");

        _mockCompiler
            .Compile<TestModel>(rule1)
            .Returns(Some(compiledRule1));

        _mockCompiler
            .Compile<TestModel>(rule2)
            .Returns(Some(compiledRule2));

        var result = _sut.Compile<TestModel>(set);

        result.IsSome.Should().BeTrue();

        result.OnSome(rs =>
        {
            var item = new AutoFaker<TestModel>().Generate();

            rs.Apply(item).Should().BeTrue();

            rs.DetailedApply(item)
                .Tee(a => a.IsRight.Should().BeTrue());

            rs.FirstMatching(item)
                .Tee(s => s.IsSome.Should().BeTrue())
                .OnSome(s => s.Should().Be("code 1"));

            firstExecuted.Should().BeTrue();
            secondExecuted.Should().BeTrue();
            thirdExecuted.Should().BeFalse();
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
            },
            "ruleset 1"
        );

        _mockCompiler
            .Compile<TestModel>(Arg.Any<Rule>())
            .Returns(Option<CompiledRule<TestModel>>.None());

        var result = _sut.Compile<TestModel>(set);

        result.IsNone.Should().BeTrue();
    }

    [Test]
    public void Compile_WhenRulesAreEmpty_ShouldReturnNone()
    {
        var set = new RulesSet([], "ruleset 1");

        var result = _sut.Compile<TestModel>(set);

        result.IsNone.Should().BeTrue();
    }

    [Test]
    public void Compile_WhenRulesIsNull_ShouldReturnNone()
    {
        var set = new RulesSet(null, "ruleset 1");

        var result = _sut.Compile<TestModel>(set);

        result.IsNone.Should().BeTrue();
    }
}