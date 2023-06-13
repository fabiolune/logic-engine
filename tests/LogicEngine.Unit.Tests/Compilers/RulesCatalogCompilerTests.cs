using System;
using LogicEngine.Compilers;
using LogicEngine.Interfaces.Compilers;
using LogicEngine.Internals;
using LogicEngine.Models;
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
            },
            "ruleset 1"
        );

        var catalog = new RulesCatalog(new[] { set1 }, "some name");

        var compiledRule = new CompiledRule<TestModel>(_ => false, "whatever");

        var compiledRulesSet = new CompiledRulesSet<TestModel>(new[] { compiledRule }, "set 1");

        _mockCompiler.Setup(_ => _.Compile<TestModel>(set1)).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet));

        var result = _sut.Compile<TestModel>(catalog);

        result
            .Tee(r => r.IsSome.Should().BeTrue())
            .OnSome(c => 
            {
                c.Name.Should().Be("some name");

                c.Apply(It.IsAny<TestModel>()).Should().BeFalse();

                c.DetailedApply(It.IsAny<TestModel>())
                    .Tee(e => e.IsLeft.Should().BeTrue())
                    .OnLeft(s => s.Should().BeEquivalentTo("whatever"));

                c.FirstMatching(It.IsAny<TestModel>())
                    .IsNone.Should().BeTrue();
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
            },
            "ruleset 1"
        );

        var catalog = new RulesCatalog(new[] { set1 }, "some name");

        var compiledRule = new CompiledRule<TestModel>(_ => true, "code");

        var compiledRulesSet = new CompiledRulesSet<TestModel>(new[] { compiledRule }, "set 1");

        _mockCompiler.Setup(_ => _.Compile<TestModel>(set1)).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet));

        var result = _sut.Compile<TestModel>(catalog);

        result
            .Tee(r => r.IsSome.Should().BeTrue())
            .OnSome(c =>
            {
                c.Name.Should().Be("some name");

                c.Apply(It.IsAny<TestModel>()).Should().BeTrue();

                c.DetailedApply(It.IsAny<TestModel>()).IsRight.Should().BeTrue();

                c.FirstMatching(It.IsAny<TestModel>())
                    .Tee(o => o.IsSome.Should().BeTrue())
                    .OnSome(s => s.Should().Be("set 1"));
            });

        _mockCompiler
            .Verify(c => c.Compile<TestModel>(set1), Times.Once);
    }

    [Test]
    public void CompileCatalog_WhenRulesSetCompilerReturnsRightItems_ShouldReturnThoseItemsAndReturnRightFirst()
    {
        var set1 = new RulesSet
        (
            new[]
            {
                new Rule("x", OperatorType.Equal, "y", "code1")
            },
            "ruleset 1"
        );
        var set2 = new RulesSet
        (
            new[]
            {
                new Rule("a", OperatorType.Equal, "b", "code2")
            },
            "ruleset 1"
        );

        var firstCalled = false;
        var secondCalled = false;

        var catalog = new RulesCatalog(new[] { set1, set2 }, "some name");

        var compiledRule1 = new CompiledRule<TestModel>(_ => true.Tee(_ => firstCalled = true), "code");
        var compiledRulesSet1 = new CompiledRulesSet<TestModel>(new[] { compiledRule1 }, "set 1");

        var compiledRule2 = new CompiledRule<TestModel>(_ => true.Tee(_ => secondCalled = true), "code");
        var compiledRulesSet2 = new CompiledRulesSet<TestModel>(new[] { compiledRule2 }, "set 2");

        _mockCompiler.Setup(_ => _.Compile<TestModel>(set1)).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet1));
        _mockCompiler.Setup(_ => _.Compile<TestModel>(set2)).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet2));

        var result = _sut.Compile<TestModel>(catalog);

        result
            .Tee(r => r.IsSome.Should().BeTrue())
            .OnSome(c =>
            {
                c.Name.Should().Be("some name");

                c.Apply(It.IsAny<TestModel>()).Should().BeTrue();

                c.DetailedApply(It.IsAny<TestModel>()).IsRight.Should().BeTrue();

                c.FirstMatching(It.IsAny<TestModel>())
                    .Tee(o => o.IsSome.Should().BeTrue())
                    .OnSome(s => s.Should().Be("set 1"));
            });

        firstCalled.Should().BeTrue();
        secondCalled.Should().BeTrue();

        _mockCompiler
            .Verify(c => c.Compile<TestModel>(set1), Times.Once);
        _mockCompiler
            .Verify(c => c.Compile<TestModel>(set2), Times.Once);
    }

    [Test]
    public void CompileCatalog_WhenRulesSetCompilerReturnsRightItems_ShouldReturnThoseItemsAndReturnRightFirstWithCircuitBreaking()
    {
        var set1 = new RulesSet
        (
            new[]
            {
                new Rule("x", OperatorType.Equal, "y", "code1")
            },
            "ruleset 1"
        );
        var set2 = new RulesSet
        (
            new[]
            {
                new Rule("a", OperatorType.Equal, "b", "code2")
            },
            "ruleset 1"
        );
        var set3 = new RulesSet
        (
            new[]
            {
                new Rule("l", OperatorType.Equal, "m", "code3")
            },
            "ruleset 3"
        );

        var firstCalled = false;
        var secondCalled = false;
        var thirdCalled = false;

        var catalog = new RulesCatalog(new[] { set1, set2 }, "some name");

        var compiledRule1 = new CompiledRule<TestModel>(_ => true.Tee(_ => firstCalled = true), "code");
        var compiledRulesSet1 = new CompiledRulesSet<TestModel>(new[] { compiledRule1 }, "set 1");

        var compiledRule2 = new CompiledRule<TestModel>(_ => false.Tee(_ => secondCalled = true), "code");
        var compiledRulesSet2 = new CompiledRulesSet<TestModel>(new[] { compiledRule2 }, "set 2");

        var compiledRule3 = new CompiledRule<TestModel>(_ => true.Tee(_ => thirdCalled = true), "code");
        var compiledRulesSet3 = new CompiledRulesSet<TestModel>(new[] { compiledRule3 }, "set 2");

        _mockCompiler.Setup(_ => _.Compile<TestModel>(set1)).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet1));
        _mockCompiler.Setup(_ => _.Compile<TestModel>(set2)).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet2));

        var result = _sut.Compile<TestModel>(catalog);

        result
            .Tee(r => r.IsSome.Should().BeTrue())
            .OnSome(c =>
            {
                c.Name.Should().Be("some name");

                c.Apply(It.IsAny<TestModel>()).Should().BeTrue();

                c.DetailedApply(It.IsAny<TestModel>()).IsRight.Should().BeTrue();

                c.FirstMatching(It.IsAny<TestModel>())
                    .Tee(o => o.IsSome.Should().BeTrue())
                    .OnSome(s => s.Should().Be("set 1"));
            });

        firstCalled.Should().BeTrue();
        secondCalled.Should().BeTrue();
        thirdCalled.Should().BeFalse();

        _mockCompiler
            .Verify(c => c.Compile<TestModel>(set1), Times.Once);
        _mockCompiler
            .Verify(c => c.Compile<TestModel>(set2), Times.Once);
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