using System.Collections.Generic;
using Fabiolune.BusinessRulesEngine.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Fabiolune.BusinessRulesEngine.Unit.Tests
{
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
            var c1 = new RulesCatalog
            {
                Name = "name 1"
            };
            List<RuleSet> rules1 = null;
            for (var i = 0; i < ruleSets1; i++)
            {
                if(rules1 == null)
                    rules1 = new List<RuleSet>();
                rules1.Add(new RuleSet());
            }
            c1.RuleSets = rules1;

            var c2 = new RulesCatalog
            {
                Name = "name 2"
            };
            List<RuleSet> rules2 = null;
            for (var i = 0; i < ruleSets2; i++)
            {
                if (rules2 == null)
                    rules2 = new List<RuleSet>();
                rules2.Add(new RuleSet());
            }
            c2.RuleSets = rules2;

            var c = c1 + c2;

            c.RuleSets.Should().HaveCount(ruleSets1 + ruleSets2);
            c.Name.Should().Be("name 1 OR name 2");
        }

        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 3)]
        public void When_MultiplyingTwoCatalogs_ExpectProductOfRules(int ruleSets1, int ruleSets2)
        {
            var c1 = new RulesCatalog
            {
                Name = "name 1"
            };
            List<RuleSet> rules1 = null;
            for (var i = 0; i < ruleSets1; i++)
            {
                if (rules1 == null)
                    rules1 = new List<RuleSet>();
                rules1.Add(new RuleSet());
            }
            c1.RuleSets = rules1;

            var c2 = new RulesCatalog
            {
                Name = "name 2"
            };
            List<RuleSet> rules2 = null;
            for (var i = 0; i < ruleSets2; i++)
            {
                if (rules2 == null)
                    rules2 = new List<RuleSet>();
                rules2.Add(new RuleSet());
            }
            c2.RuleSets = rules2;

            var c = c1 * c2;

            c.RuleSets.Should().HaveCount(ruleSets1 * ruleSets2);
            c.Name.Should().Be("name 1 AND name 2");
        }
    }
}