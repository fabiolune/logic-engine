using AutoBogus;
using LogicEngine.Compilers;
using LogicEngine.Interfaces.Compilers;
using LogicEngine.Internals;
using LogicEngine.Models;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine.Unit.Tests.Compilers;

public class RulesCatalogCompilerTests
{
    private IRulesSetCompiler _mockCompiler;
    private RulesCatalogCompiler _sut;

    [SetUp]
    public void SetUp()
    {
        _mockCompiler = Substitute.For<IRulesSetCompiler>();
        _sut = new RulesCatalogCompiler(_mockCompiler);
    }

    [Test]
    public void CompileCatalog_WhenRulesSetCompilerReturnsLeftItems_ShouldReturnThoseItems()
    {
        var set1 = new RulesSet
        (
            [
                new Rule("x", OperatorType.Equal, "y", "code")
            ],
            "ruleset 1"
        );

        var catalog = new RulesCatalog([set1], "some name");

        var compiledRule = new CompiledRule<TestModel>(cr => false, "whatever");

        var compiledRulesSet = new CompiledRulesSet<TestModel>([compiledRule], "set 1");

        _mockCompiler.Compile<TestModel>(set1).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet));

        var result = _sut.Compile<TestModel>(catalog);

        result
            .Tee(r => r.IsSome.Should().BeTrue())
            .OnSome(c =>
            {
                var item = new AutoFaker<TestModel>().Generate();

                c.Name.Should().Be("some name");

                c.Apply(item).Should().BeFalse();

                c.DetailedApply(item)
                    .Tee(e => e.IsLeft.Should().BeTrue())
                    .OnLeft(s => s.Should().BeEquivalentTo("whatever"));

                c.FirstMatching(item)
                    .IsNone.Should().BeTrue();
            });

        _mockCompiler
            .Received(1)
            .Compile<TestModel>(set1);
    }

    [Test]
    public void CompileCatalog_WhenRulesSetCompilerReturnsRightItems_ShouldReturnThoseItems()
    {
        var set1 = new RulesSet
        (
            [
                new Rule("x", OperatorType.Equal, "y", "code")
            ],
            "ruleset 1"
        );

        var catalog = new RulesCatalog([set1], "some name");

        var compiledRule = new CompiledRule<TestModel>(cr => true, "code");

        var compiledRulesSet = new CompiledRulesSet<TestModel>([compiledRule], "set 1");

        _mockCompiler.Compile<TestModel>(set1).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet));

        var result = _sut.Compile<TestModel>(catalog);

        result
            .Tee(r => r.IsSome.Should().BeTrue())
            .OnSome(c =>
            {
                var item = new AutoFaker<TestModel>().Generate();

                c.Name.Should().Be("some name");

                c.Apply(item).Should().BeTrue();

                c.DetailedApply(item).IsRight.Should().BeTrue();

                c.FirstMatching(item)
                    .Tee(o => o.IsSome.Should().BeTrue())
                    .OnSome(s => s.Should().Be("set 1"));
            });

        _mockCompiler
            .Received(1)
            .Compile<TestModel>(set1);
    }

    [Test]
    public void CompileCatalog_WhenRulesSetCompilerReturnsRightItems_ShouldReturnThoseItemsAndReturnRightFirst()
    {
        var set1 = new RulesSet
        (
            [
                new Rule("x", OperatorType.Equal, "y", "code1")
            ],
            "ruleset 1"
        );
        var set2 = new RulesSet
        (
            [
                new Rule("a", OperatorType.Equal, "b", "code2")
            ],
            "ruleset 1"
        );

        var firstCalled = false;
        var secondCalled = false;

        var catalog = new RulesCatalog([set1, set2], "some name");

        var compiledRule1 = new CompiledRule<TestModel>(cr => true.Tee(cr => firstCalled = true), "code");
        var compiledRulesSet1 = new CompiledRulesSet<TestModel>([compiledRule1], "set 1");

        var compiledRule2 = new CompiledRule<TestModel>(cr => true.Tee(cr => secondCalled = true), "code");
        var compiledRulesSet2 = new CompiledRulesSet<TestModel>([compiledRule2], "set 2");

        _mockCompiler.Compile<TestModel>(set1).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet1));
        _mockCompiler.Compile<TestModel>(set2).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet2));

        var result = _sut.Compile<TestModel>(catalog);

        result
            .Tee(r => r.IsSome.Should().BeTrue())
            .OnSome(c =>
            {
                var item = new AutoFaker<TestModel>().Generate();

                c.Name.Should().Be("some name");

                c.Apply(item).Should().BeTrue();

                c.DetailedApply(item).IsRight.Should().BeTrue();

                c.FirstMatching(item)
                    .Tee(o => o.IsSome.Should().BeTrue())
                    .OnSome(s => s.Should().Be("set 1"));
            });

        firstCalled.Should().BeTrue();
        secondCalled.Should().BeFalse();

        _mockCompiler
            .Received(1)
            .Compile<TestModel>(set1);
        _mockCompiler
            .Received(1)
            .Compile<TestModel>(set2);
    }

    [Test]
    public void CompileCatalog_WhenRulesSetCompilerReturnsRightItems_ShouldReturnThoseItemsAndReturnRightFirstWithCircuitBreaking()
    {
        var set1 = new RulesSet
        (
            [
                new Rule("x", OperatorType.Equal, "y", "code1")
            ],
            "ruleset 1"
        );
        var set2 = new RulesSet
        (
            [
                new Rule("a", OperatorType.Equal, "b", "code2")
            ],
            "ruleset 1"
        );
        var set3 = new RulesSet
        (
            [
                new Rule("l", OperatorType.Equal, "m", "code3")
            ],
            "ruleset 3"
        );

        var firstCalled = false;
        var secondCalled = false;
        var thirdCalled = false;

        var catalog = new RulesCatalog([set1, set2], "some name");

        var compiledRule1 = new CompiledRule<TestModel>(cr => true.Tee(cr => firstCalled = true), "code");
        var compiledRulesSet1 = new CompiledRulesSet<TestModel>([compiledRule1], "set 1");

        var compiledRule2 = new CompiledRule<TestModel>(cr => false.Tee(cr => secondCalled = true), "code");
        var compiledRulesSet2 = new CompiledRulesSet<TestModel>([compiledRule2], "set 2");

        var compiledRule3 = new CompiledRule<TestModel>(cr => true.Tee(cr => thirdCalled = true), "code");
        var compiledRulesSet3 = new CompiledRulesSet<TestModel>([compiledRule3], "set 2");

        _mockCompiler.Compile<TestModel>(set1).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet1));
        _mockCompiler.Compile<TestModel>(set2).Returns(Option<CompiledRulesSet<TestModel>>.Some(compiledRulesSet2));

        var result = _sut.Compile<TestModel>(catalog);

        result
            .Tee(r => r.IsSome.Should().BeTrue())
            .OnSome(c =>
            {
                var item = new AutoFaker<TestModel>().Generate();

                c.Name.Should().Be("some name");

                c.Apply(item).Should().BeTrue();

                c.DetailedApply(item).IsRight.Should().BeTrue();

                c.FirstMatching(item)
                    .Tee(o => o.IsSome.Should().BeTrue())
                    .OnSome(s => s.Should().Be("set 1"));
            });

        firstCalled.Should().BeTrue();
        secondCalled.Should().BeFalse();
        thirdCalled.Should().BeFalse();

        _mockCompiler
            .Received(1)
            .Compile<TestModel>(set1);
        _mockCompiler
            .Received(1)
            .Compile<TestModel>(set2);
    }

    [Test]
    public void CompileCatalog_WhenRulesSetsAreEmpty_ShouldReturnNone()
    {
        var catalog = new RulesCatalog([], "some name");

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