using System.Collections.Generic;
using FluentAssertions;
using LogicEngine.Extensions;
using LogicEngine.Internals;
using LogicEngine.Models;
using NUnit.Framework;

namespace LogicEngine.Unit.Tests.Extensions;

[TestFixture]
public class RulesCatalogExtensionsTests
{
    [Test]
    public void NullAndEmptyCatalogShouldBeEquivalent()
    {
        var catalog2 = new RulesCatalog(new List<RulesSet>(), "name 2");

        ((RulesCatalog) null).IsEquivalentTo(catalog2).Should().BeTrue();
    }

    [Test]
    public void WhenCatalogsDiffersForTheRulesOrderShouldNotBeEquivalent()
    {
        var catalog1 = new RulesCatalog(new List<RulesSet>
        {
            new()
            {
                Description = "description 1",
                Rules = new List<Rule>
                {
                    new("prop1", OperatorType.Contains, "value1", "code1"),
                    new("prop1", OperatorType.Contains, "value2", "code2")
                }
            }
        }, "name 1");

        var catalog2 = new RulesCatalog(new List<RulesSet>
        {
            new()
            {
                Description = "description 2",
                Rules = new List<Rule>
                {
                    new("prop1", OperatorType.Contains, "value2", "code2"),
                    new("prop1", OperatorType.Contains, "value1", "code1")
                }
            }
        }, "name 2");

        catalog1.IsEquivalentTo(catalog2).Should().BeFalse();
    }

    [Test]
    public void WhenCatalogsDiffersJustForNameAndDescriptionShouldBeEquivalent()
    {
        var catalog1 = new RulesCatalog(new List<RulesSet>
        {
            new()
            {
                Description = "description 1",
                Rules = new List<Rule>
                {
                    new("prop1", OperatorType.Contains, "value1", "code1")
                }
            }
        }, "name 1");

        var catalog2 = new RulesCatalog(new List<RulesSet>
        {
            new()
            {
                Description = "description 2",
                Rules = new List<Rule>
                {
                    new("prop1", OperatorType.Contains, "value1", "code1")
                }
            }
        }, "name 2");

        catalog1.IsEquivalentTo(catalog2).Should().BeTrue();
    }
}