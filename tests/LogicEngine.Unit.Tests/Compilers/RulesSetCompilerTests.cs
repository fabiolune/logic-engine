using AutoBogus;
using LogicEngine.Compilers;
using LogicEngine.Interfaces.Compilers;
using LogicEngine.Internals;
using LogicEngine.Models;
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
            [
                rule1,
                rule2
            ],
            "ruleset 1"
        );

        var compiledRule = new CompiledRule<TestModel>(cr => false, "whatever");

        _mockCompiler
            .Compile<TestModel>(rule1)
            .Returns(Some(compiledRule));

        _mockCompiler
            .Compile<TestModel>(rule2)
            .Returns(Option<CompiledRule<TestModel>>.None());

        var result = _sut.Compile<TestModel>(set);

        result.IsSome.ShouldBeTrue();

        result.OnSome(rs =>
        {
            var item = new AutoFaker<TestModel>().Generate();

            rs.Apply(item).ShouldBeFalse();

            rs.DetailedApply(item)
                .Tee(a => a.IsLeft.ShouldBeTrue())
                .OnLeft(s => s.ShouldBe(["whatever"]));

            rs.FirstMatching(item)
                .IsNone.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRulesSetCompilerReturnsSome_ShouldReturnCompiledRulesSetOnlyForSomeAndFirstMatchingShouldBeSome()
    {
        var rule1 = new Rule("x", OperatorType.Equal, "y", "code1");
        var rule2 = new Rule("a", OperatorType.Equal, "b", "code2");

        var set = new RulesSet
        (
            [
                rule1,
                rule2
            ],
            "ruleset 1"
        );

        var firstExecuted = false;
        var secondExecuted = false;

        var compiledRule1 = new CompiledRule<TestModel>(cr => true.Tee(cr => firstExecuted = true), "code 1");
        var compiledRule2 = new CompiledRule<TestModel>(cr => true.Tee(cr => secondExecuted = true), "code 2");

        _mockCompiler
            .Compile<TestModel>(rule1)
            .Returns(Some(compiledRule1));

        _mockCompiler
            .Compile<TestModel>(rule2)
            .Returns(Some(compiledRule2));

        var result = _sut.Compile<TestModel>(set);

        result.IsSome.ShouldBeTrue();

        result.OnSome(rs =>
        {
            var item = new AutoFaker<TestModel>().Generate();

            rs.Apply(item).ShouldBeTrue();

            rs.DetailedApply(item)
                .Tee(a => a.IsRight.ShouldBeTrue());

            rs.FirstMatching(item)
                .Tee(s => s.IsSome.ShouldBeTrue())
                .OnSome(s => s.ShouldBe("code 1"));

            firstExecuted.ShouldBeTrue();
            secondExecuted.ShouldBeTrue();
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
            [
                rule1,
                rule2
            ],
            "ruleset 1"
        );

        var firstExecuted = false;
        var secondExecuted = false;
        var thirdExecuted = false;

        var compiledRule1 = new CompiledRule<TestModel>(cr => true.Tee(cr => firstExecuted = true), "code 1");
        var compiledRule2 = new CompiledRule<TestModel>(cr => false.Tee(cr => secondExecuted = true), "code 2");
        var compiledRule3 = new CompiledRule<TestModel>(cr => true.Tee(cr => thirdExecuted = true), "code 3");

        _mockCompiler
            .Compile<TestModel>(rule1)
            .Returns(Some(compiledRule1));

        _mockCompiler
            .Compile<TestModel>(rule2)
            .Returns(Some(compiledRule2));

        var result = _sut.Compile<TestModel>(set);

        result.IsSome.ShouldBeTrue();

        result.OnSome(rs =>
        {
            var item = new AutoFaker<TestModel>().Generate();

            rs.Apply(item).ShouldBeTrue();

            rs.DetailedApply(item)
                .Tee(a => a.IsRight.ShouldBeTrue());

            rs.FirstMatching(item)
                .Tee(s => s.IsSome.ShouldBeTrue())
                .OnSome(s => s.ShouldBe("code 1"));

            firstExecuted.ShouldBeTrue();
            secondExecuted.ShouldBeTrue();
            thirdExecuted.ShouldBeFalse();
        });
    }

    [Test]
    public void Compile_WhenRulesSetCompilerReturnsNone_ShouldReturnNone()
    {
        var rule1 = new Rule("x", OperatorType.Equal, "y", "code1");
        var rule2 = new Rule("a", OperatorType.Equal, "b", "code2");

        var set = new RulesSet
        (
            [
                rule1,
                rule2
            ],
            "ruleset 1"
        );

        _mockCompiler
            .Compile<TestModel>(Arg.Any<Rule>())
            .Returns(Option<CompiledRule<TestModel>>.None());

        var result = _sut.Compile<TestModel>(set);

        result.IsNone.ShouldBeTrue();
    }

    [Test]
    public void Compile_WhenRulesAreEmpty_ShouldReturnNone()
    {
        var set = new RulesSet([], "ruleset 1");

        var result = _sut.Compile<TestModel>(set);

        result.IsNone.ShouldBeTrue();
    }

    [Test]
    public void Compile_WhenRulesIsNull_ShouldReturnNone()
    {
        var set = new RulesSet(null, "ruleset 1");

        var result = _sut.Compile<TestModel>(set);

        result.IsNone.ShouldBeTrue();
    }
}