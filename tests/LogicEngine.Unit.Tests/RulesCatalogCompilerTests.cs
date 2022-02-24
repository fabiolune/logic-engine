using System;
using FluentAssertions;
using LogicEngine.Interfaces;
using LogicEngine.Internals;
using LogicEngine.Models;
using Moq;
using NUnit.Framework;
using TinyFp;

namespace LogicEngine.Unit.Tests;

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
    public void CompileCatalog_WhenRulesSetCompilerReturnsItems_ShouldReturnThoseItems()
    {
        var set1 = new RulesSet
        {
            Rules = new[]
            {
                new Rule("x", OperatorType.Equal, "y", "code")
            }
        };

        var catalog = new RulesCatalog(new []{ set1 }, "some name");

        var functions = It.Is<Func<TestModel, Either<string, TinyFp.Unit>>[]>(_ => _.Length == 1);

        var compiledRulesSet = new CompiledRulesSet<TestModel>(functions);

        _mockCompiler.Setup(_ => _.Compile<TestModel>(set1)).Returns(compiledRulesSet);

        var result = _sut.CompileCatalog<TestModel>(catalog);

        result.Should().BeEquivalentTo(new CompiledCatalog<TestModel>(new[] { functions }));
    }

    [Test]
    public void CompileCatalog_WhenRulesSetsAreEmpty_ShouldReturnEmpty()
    {
        var catalog = new RulesCatalog(Array.Empty<RulesSet>(), "some name");
        
        var result = _sut.CompileCatalog<TestModel>(catalog);

        result.Executables.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void CompileCatalog_WhenRulesSetsAreNull_ShouldReturnEmpty()
    {
        var catalog = new RulesCatalog(null, "some name");

        var result = _sut.CompileCatalog<TestModel>(catalog);

        result.Executables.Should().NotBeNull().And.BeEmpty();
    }
}