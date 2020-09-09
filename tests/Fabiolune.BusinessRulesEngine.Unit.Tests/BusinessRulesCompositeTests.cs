using System.Collections.Generic;
using Fabiolune.BusinessRulesEngine.Models;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Serilog;

namespace Fabiolune.BusinessRulesEngine.Unit.Tests
{
    [TestFixture]
    public class BusinessRulesCompositeTests
    {
        private readonly BusinessRulesManager<TestModel> _sut;

        public BusinessRulesCompositeTests()
        {
            var logger = new Mock<ILogger>();
            _sut = new BusinessRulesManager<TestModel>(new BusinessRulesCompiler(logger.Object));
        }

        [Test]
        public void WhenItemDoesNotSatisfyTwoCatalogs_SumAndProductShouldGiveFalse()
        {
            TestModel item;

            var catalog1 = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(item.StringProperty), OperatorType.Equal, "wrong value 1")
                        }
                    }
                }
            };

            var catalog2 = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(item.StringProperty), OperatorType.Equal, "wrong value 2")
                        }
                    }
                }
            };

            item = new TestModel
            {
                StringProperty = "test"
            };

            _sut.SetCatalog(catalog1);
            var result1 = _sut.ItemSatisfiesRulesWithMessage(item);
            result1.Success.Should().BeFalse();

            _sut.SetCatalog(catalog2);
            var result2 = _sut.ItemSatisfiesRulesWithMessage(item);
            result2.Success.Should().BeFalse();

            _sut.SetCatalog(catalog1 + catalog2);
            var sumResult = _sut.ItemSatisfiesRulesWithMessage(item);
            sumResult.Success.Should().BeFalse();

            _sut.SetCatalog(catalog1 * catalog2);
            var productResult = _sut.ItemSatisfiesRulesWithMessage(item);
            productResult.Success.Should().BeFalse();
        }

        [Test]
        public void WhenItemSatisfiesOneCatalogButNotTheOther_SumShouldGiveTrueAndProductFalse()
        {
            TestModel item;

            var catalog1 = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(item.IntEnumerableProperty), OperatorType.Contains, "25"),
                            new Rule(nameof(item.IntEnumerableProperty), OperatorType.Contains, "26")
                        }
                    }
                }
            };

            var catalog2 = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(item.IntEnumerableProperty), OperatorType.Contains, "123123123")
                        }
                    }
                }
            };

            item = new TestModel
            {
                IntEnumerableProperty = new[]
                {
                    25,
                    26
                }
            };

            _sut.SetCatalog(catalog1);
            var result1 = _sut.ItemSatisfiesRulesWithMessage(item);
            result1.Success.Should().Be(_sut.ItemSatisfiesRules(item));
            result1.Success.Should().BeTrue();

            _sut.SetCatalog(catalog2);
            var result2 = _sut.ItemSatisfiesRulesWithMessage(item);
            result2.Success.Should().Be(_sut.ItemSatisfiesRules(item));
            result2.Success.Should().BeFalse();

            _sut.SetCatalog(catalog1 + catalog2);
            var sumResult = _sut.ItemSatisfiesRulesWithMessage(item);
            sumResult.Success.Should().Be(_sut.ItemSatisfiesRules(item));
            sumResult.Success.Should().BeTrue();

            _sut.SetCatalog(catalog1 * catalog2);
            var productResult = _sut.ItemSatisfiesRulesWithMessage(item);
            productResult.Success.Should().Be(_sut.ItemSatisfiesRules(item));
            productResult.Success.Should().BeFalse();
        }

        [Test]
        public void WhenItemSatisfiesTwoCatalogs_SumAndProductShouldGiveTrue()
        {
            TestModel item;

            var catalog1 = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(item.StringProperty), OperatorType.Equal, "test")
                        }
                    }
                }
            };

            var catalog2 = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(item.StringProperty2), OperatorType.Equal, "test2")
                        }
                    }
                }
            };

            item = new TestModel
            {
                StringProperty = "test",
                StringProperty2 = "test2"
            };

            _sut.SetCatalog(catalog1);
            var result1 = _sut.ItemSatisfiesRulesWithMessage(item);
            result1.Success.Should().BeTrue();

            _sut.SetCatalog(catalog2);
            var result2 = _sut.ItemSatisfiesRulesWithMessage(item);
            result2.Success.Should().BeTrue();

            _sut.SetCatalog(catalog1 + catalog2);
            var sumResult = _sut.ItemSatisfiesRulesWithMessage(item);
            sumResult.Success.Should().BeTrue();

            _sut.SetCatalog(catalog1 * catalog2);
            var productResult = _sut.ItemSatisfiesRulesWithMessage(item);
            productResult.Success.Should().BeTrue();
        }
    }
}