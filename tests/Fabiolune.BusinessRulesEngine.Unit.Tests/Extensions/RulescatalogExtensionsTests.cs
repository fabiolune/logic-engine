using System.Collections.Generic;
using Fabiolune.BusinessRulesEngine.Extensions;
using Fabiolune.BusinessRulesEngine.Internals;
using Fabiolune.BusinessRulesEngine.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Fabiolune.BusinessRulesEngine.Unit.Tests.Extensions
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
                    new RulesSet
                    {
                        Description = "description 1",
                        Rules = new List<Rule>
                        {
                            new Rule("prop1", OperatorType.Contains, "value1", "code1", "description 1"),
                            new Rule("prop1", OperatorType.Contains, "value2", "code2", "description 2")
                        }
                    }
                }
            };

            var catalog2 = new RulesCatalog
            {
                Name = "name 2",
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "description 2",
                        Rules = new List<Rule>
                        {
                            new Rule("prop1", OperatorType.Contains, "value2", "code2", "description 2"),
                            new Rule("prop1", OperatorType.Contains, "value1", "code1", "description 1")
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
                    new RulesSet
                    {
                        Description = "description 1",
                        Rules = new List<Rule>
                        {
                            new Rule("prop1", OperatorType.Contains, "value1", "code1", "description 1")
                        }
                    }
                }
            };

            var catalog2 = new RulesCatalog
            {
                Name = "name 2",
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "description 2",
                        Rules = new List<Rule>
                        {
                            new Rule("prop1", OperatorType.Contains, "value1", "code1", "description 1")
                        }
                    }
                }
            };

            catalog1.IsEquivalentTo(catalog2).Should().BeTrue();
        }
    }
}