using System;
using System.Linq;
using LogicEngine.Internals;
using LogicEngine.Models;

namespace LogicEngine.Unit.Tests;

[TestFixture]
public class CatalogCompositionTests
{
    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(1, 0)]
    [TestCase(1, 1)]
    [TestCase(2, 3)]
    public void When_AddingTwoCatalogs_ExpectSumOfRules(int ruleSets1, int ruleSets2)
    {
        var c1 = new RulesCatalog(Enumerable
            .Range(0, ruleSets1)
            .Select(_ => new RulesSet(Array.Empty<Rule>(), "ruleset 1")), "name 1");
        
        var c2 = new RulesCatalog(Enumerable
            .Range(0, ruleSets2)
            .Select(_ => new RulesSet(Array.Empty<Rule>(), "ruleset 2")), "name 2");

        var sumCatalog = c1 + c2;

        sumCatalog.RulesSets.Should().HaveCount(ruleSets1 + ruleSets2);
        sumCatalog.Name.Should().Be("(name 1 OR name 2)");
    }

    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(1, 0)]
    [TestCase(1, 1)]
    [TestCase(2, 3)]
    public void When_MultiplyingTwoCatalogs_ExpectProductOfRules(int ruleSets1, int ruleSets2)
    {
        var c1 = new RulesCatalog(Enumerable
            .Range(0, ruleSets1)
            .Select(_ => new RulesSet(Array.Empty<Rule>(), "ruleset 1")), "name 1");

        var c2 = new RulesCatalog(Enumerable
            .Range(0, ruleSets2)
            .Select(_ => new RulesSet(Array.Empty<Rule>(), "ruleset 2")), "name 2");

        var sumCatalog = c1 * c2;

        sumCatalog.RulesSets.Should().HaveCount(ruleSets1 * ruleSets2);
        sumCatalog.Name.Should().Be("(name 1 AND name 2)");
    }

    [Test]
    public void CatalogsSum_WhenFirstRulesSetIsNull_ShouldReturnProperSum()
    {
        var c1 = new RulesCatalog(null, "catalog 1");
        var c2 = new RulesCatalog(new[]
        {
            new RulesSet
            (
                new[]
                {
                    new Rule("a", OperatorType.Equal, "b", "code")
                },
                "ruleset 1"
            )
        }, "catalog 2");

        var sumCatalog1 = c1 + c2;

        sumCatalog1.RulesSets.Should().HaveCount(1);
        sumCatalog1.Name.Should().Be("(catalog 1 OR catalog 2)");

        var sumCatalog2 = c2 + c1;

        sumCatalog2.RulesSets.Should().HaveCount(1);
        sumCatalog2.Name.Should().Be("(catalog 2 OR catalog 1)");
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void CatalogsProduct_WhenOneRulesSetIsNull_ShouldReturnProperProduct(int ruleSets2)
    {
        var c1 = new RulesCatalog(null, "catalog 1");
        var c2 = new RulesCatalog(Enumerable
            .Range(0, ruleSets2)
            .Select(_ => new RulesSet(Array.Empty<Rule>(), $"ruleset {_}")), "catalog 2");

        var prodCatalog1 = c1 * c2;

        prodCatalog1.RulesSets.Should().HaveCount(0);
        prodCatalog1.Name.Should().Be("(catalog 1 AND catalog 2)");

        var prodCatalog2 = c2 * c1;

        prodCatalog2.RulesSets.Should().HaveCount(0);
        prodCatalog2.Name.Should().Be("(catalog 2 AND catalog 1)");

    }

    [Test]
    public void CatalogsProduct_WhenBothRulesSetAreNull_ShouldReturnProperProduct()
    {
        var c1 = new RulesCatalog(null, "catalog 1");
        var c2 = new RulesCatalog(null, "catalog 2");

        var prodCatalog1 = c1 * c2;

        prodCatalog1.RulesSets.Should().HaveCount(0);
        prodCatalog1.Name.Should().Be("(catalog 1 AND catalog 2)");
    }
}
