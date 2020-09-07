using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Fabiolune.BusinessRulesEngine.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Fabiolune.BusinessRulesEngine.Unit.Tests
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class CatalogCompositionTests
    {
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(2, 3)]
        public void When_AddingTwoCatalogs_ExpectSumOfRules(int rulesets1, int rulesets2)
        {
            // arrange
            var c1 = new RulesCatalog
            {
                Name = "name 1"
            };
            var rules1 = new List<RuleSet>();
            for (var i = 0; i < rulesets1; i++)
                rules1.Add(new RuleSet());
            c1.RuleSets = rules1;

            var c2 = new RulesCatalog
            {
                Name = "name 2"
            };
            var rules2 = new List<RuleSet>();
            for (var i = 0; i < rulesets2; i++)
                rules2.Add(new RuleSet());
            c2.RuleSets = rules2;

            // act
            var c = c1 + c2;

            // assert
            c.RuleSets.Should().HaveCount(rulesets1 + rulesets2);
            c.Name.Should().Be("name 1 OR name 2");
        }

        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 1)]
        [TestCase(2, 3)]
        public void When_MultiplyingTwoCatalogs_ExpectProductOfRules(int rulesets1, int rulesets2)
        {
            // arrange
            var c1 = new RulesCatalog
            {
                Name = "name 1"
            };
            var rules1 = new List<RuleSet>();
            for (var i = 0; i < rulesets1; i++)
                rules1.Add(new RuleSet());
            c1.RuleSets = rules1;

            var c2 = new RulesCatalog
            {
                Name = "name 2"
            };
            var rules2 = new List<RuleSet>();
            for (var i = 0; i < rulesets2; i++)
                rules2.Add(new RuleSet());
            c2.RuleSets = rules2;

            // act
            var c = c1 * c2;

            // assert
            c.RuleSets.Should().HaveCount(rulesets1 * rulesets2);
            c.Name.Should().Be("name 1 AND name 2");
        }
    }
}