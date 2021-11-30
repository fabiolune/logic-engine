using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using RulesEngine.Extensions;
using RulesEngine.Internals;
using RulesEngine.Models;

namespace RulesEngine.Unit.Tests.Extensions
{
    [TestFixture]
    public class RulesCatalogExtensionsTests
    {
        [Test]
        public void NullAndEmptyCatalogShouldBeEquivalent()
        {
            var catalog2 = new RulesCatalog
            {
                Name = "name 2",
                RulesSets = new List<RulesSet>()
            };

            ((RulesCatalog) null).IsEquivalentTo(catalog2).Should().BeTrue();
        }

        [Test]
        public void WhenCatalogsDiffersForTheRulesOrderShouldNotBeEquivalent()
        {
            var catalog1 = new RulesCatalog
            {
                Name = "name 1",
                RulesSets = new List<RulesSet>
                {
                    new()
                    {
                        Description = "description 1",
                        Rules = new List<Rule>
                        {
                            new("prop1", OperatorType.Contains, "value1", "code1", "description 1"),
                            new("prop1", OperatorType.Contains, "value2", "code2", "description 2")
                        }
                    }
                }
            };

            var catalog2 = new RulesCatalog
            {
                Name = "name 2",
                RulesSets = new List<RulesSet>
                {
                    new()
                    {
                        Description = "description 2",
                        Rules = new List<Rule>
                        {
                            new("prop1", OperatorType.Contains, "value2", "code2", "description 2"),
                            new("prop1", OperatorType.Contains, "value1", "code1", "description 1")
                        }
                    }
                }
            };

            catalog1.IsEquivalentTo(catalog2).Should().BeFalse();
        }

        [Test]
        public void WhenCatalogsDiffersJustForNameAndDescriptionShouldBeEquivalent()
        {
            var catalog1 = new RulesCatalog
            {
                Name = "name 1",
                RulesSets = new List<RulesSet>
                {
                    new()
                    {
                        Description = "description 1",
                        Rules = new List<Rule>
                        {
                            new("prop1", OperatorType.Contains, "value1", "code1", "description 1")
                        }
                    }
                }
            };

            var catalog2 = new RulesCatalog
            {
                Name = "name 2",
                RulesSets = new List<RulesSet>
                {
                    new()
                    {
                        Description = "description 2",
                        Rules = new List<Rule>
                        {
                            new("prop1", OperatorType.Contains, "value1", "code1", "description 1")
                        }
                    }
                }
            };

            catalog1.IsEquivalentTo(catalog2).Should().BeTrue();
        }
    }
}