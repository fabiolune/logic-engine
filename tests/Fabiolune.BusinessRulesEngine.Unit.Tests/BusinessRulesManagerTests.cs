using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Fabiolune.BusinessRulesEngine.Internals;
using Fabiolune.BusinessRulesEngine.Models;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Serilog;

namespace Fabiolune.BusinessRulesEngine.Unit.Tests
{
    [TestFixture]
    public class BusinessRulesManagerTests
    {
        private readonly BusinessRulesManager<TestModel> _sut;

        public BusinessRulesManagerTests()
        {
            var logger = new Mock<ILogger>();
            _sut = new BusinessRulesManager<TestModel>(new BusinessRulesCompiler(logger.Object));
        }

        [TestCase(new[] {1, 2}, new[] {1, 3}, true)]
        [TestCase(new[] {1, 2}, new[] {3, 4}, false)]
        [TestCase(null, new[] {3, 4}, false)]
        [TestCase(new[] {1, 2}, null, false)]
        [TestCase(null, null, false)]
        public void When_ItemSatisfiesRulesWithInnerOverlapsOperator_ShouldReturnExpectedResult(IEnumerable<int> first,
            IEnumerable<int> second, bool expectedResult)
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerOverlaps,
                                nameof(TestModel.IntEnumerableProperty2))
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntEnumerableProperty = first,
                IntEnumerableProperty2 = second
            };


            var result = _sut.ItemSatisfiesRules(item);


            result.Should().Be(expectedResult);
        }

        [TestCase(new[] {1, 2}, new[] {1, 3}, false)]
        [TestCase(new[] {1, 2}, new[] {3, 4}, true)]
        [TestCase(null, new[] {3, 4}, true)]
        [TestCase(new[] {1, 2}, null, true)]
        [TestCase(null, null, true)]
        public void When_ApplyInnerNotOverlapsOperator_ShouldReturnExpectedResult(IEnumerable<int> first,
            IEnumerable<int> second, bool expectedResult)
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerNotOverlaps,
                                nameof(TestModel.IntEnumerableProperty2))
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntEnumerableProperty = first,
                IntEnumerableProperty2 = second
            };


            var result = _sut.ItemSatisfiesRules(item);


            result.Should().Be(expectedResult);
        }

        [TestCase("key_1", "key_1", OperatorType.ContainsKey, true)]
        [TestCase("key_1", "key_2", OperatorType.ContainsKey, false)]
        [TestCase("key_1", "key_1", OperatorType.NotContainsKey, false)]
        [TestCase("key_1", "key_2", OperatorType.NotContainsKey, true)]
        public void When_Apply_Rules_With_KeyValue_Operator_On_Key_Should_Return_Expected_Result(string presentKey,
            string keyToCheck, OperatorType operatorType, bool expectedResult)
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringStringDictionaryProperty), operatorType, keyToCheck)
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {presentKey, "value_1"}
                }
            };


            var result = _sut.ItemSatisfiesRules(item);


            result.Should().Be(expectedResult);
        }

        [TestCase("value_1", "value_1", OperatorType.ContainsValue, true)]
        [TestCase("value_1", "value_2", OperatorType.ContainsValue, false)]
        [TestCase("value_1", "value_1", OperatorType.NotContainsValue, false)]
        [TestCase("value_1", "value_2", OperatorType.NotContainsValue, true)]
        public void When_Apply_Rules_With_KeyValue_Operator_On_Value_Should_Return_Expected_Result(string presentValue,
            string valueToCheck, OperatorType operatorType, bool expectedResult)
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringStringDictionaryProperty), operatorType, valueToCheck)
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"key", presentValue}
                }
            };


            var result = _sut.ItemSatisfiesRules(item);


            result.Should().Be(expectedResult);
        }

        [TestCase("key", "value", "key", "value", OperatorType.KeyContainsValue, true)]
        [TestCase("key", "value", "key", "value", OperatorType.NotKeyContainsValue, false)]
        [TestCase("key", "value", "key2", "value", OperatorType.KeyContainsValue, false)]
        [TestCase("key", "value", "key", "value2", OperatorType.KeyContainsValue, false)]
        [TestCase("key", "value", "key2", "value", OperatorType.NotKeyContainsValue, true)]
        [TestCase("key", "value", "key", "value2", OperatorType.NotKeyContainsValue, true)]
        public void When_Apply_Rules_With_KeyValue_Operator_On_Key_And_Value_Should_Return_Expected_Result(
            string ruleKey,
            string ruleValue,
            string presentKey,
            string presentValue,
            OperatorType operatorType,
            bool expectedResult)
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule($"{nameof(TestModel.StringStringDictionaryProperty)}[{ruleKey}]", operatorType,
                                ruleValue)
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {presentKey, presentValue}
                }
            };

            var result = _sut.ItemSatisfiesRules(item);

            result.Should().Be(expectedResult);
        }

        [Test]
        public void When_ItemDoesNotSatisfiesRulesWithOverlapsOperatorWithValidData_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Overlaps, "1,2,3,4")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntEnumerableProperty = new[]
                {
                    5
                }
            };


            var result = _sut.ItemSatisfiesRules(item);


            result.Should().BeFalse();
        }

        [Test]
        public void When_ItemDoesNotSatisfiesRulesWithOverlapsOperatorWithValidData2_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Overlaps, "1,2,3,4")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntEnumerableProperty = null
            };


            var result = _sut.ItemSatisfiesRules(item);


            result.Should().BeFalse();
        }

        [Test]
        public void When_ItemDoesNotSatisfyRulesWithNotOverlapsOperatorWithValidData_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.NotOverlaps, "1,2,3,4")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntEnumerableProperty = new[]
                {
                    1
                }
            };


            var result = _sut.ItemSatisfiesRules(item);


            result.Should().BeFalse();
        }

        [Test]
        public void When_ItemIntegerElementIsContainedInArrayAndRulesDoesNotRequireIt_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntProperty), OperatorType.IsContained, "1,2")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntProperty = 3
            };


            var result = _sut.ItemSatisfiesRulesWithMessage(item);


            result.Success.Should().BeFalse();
        }

        [Test]
        public void When_ItemIntegerElementIsContainedInArrayAndRulesRequiresIt_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntProperty), OperatorType.IsContained, "1,2")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntProperty = 1
            };


            var result = _sut.ItemSatisfiesRulesWithMessage(item);


            result.Success.Should().BeTrue();
        }

        [Test]
        public void When_ItemIntegerElementIsNotContainedInArrayAndRulesDoesNotRequireIt_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntProperty), OperatorType.IsNotContained, "1,2")
                            {
                                Code = "code_1"
                            }
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntProperty = 1
            };


            var result = _sut.ItemSatisfiesRulesWithMessage(item);


            result.Success.Should().BeFalse();
        }

        [Test]
        public void When_ItemIntegerElementIsNotContainedInArrayAndRulesRequiresIt_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntProperty), OperatorType.IsNotContained, "1,2")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntProperty = 3
            };


            var result = _sut.ItemSatisfiesRulesWithMessage(item);


            result.Success.Should().BeTrue();
        }

        [Test]
        public void When_ItemSatisfiesRulesWithContainsOperatorWith2ValidData_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25"),
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "26")
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntEnumerableProperty = new[]
                {
                    25,
                    26,
                    27
                }
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeTrue("contains a matching item in enumerable property");
            result.Success.Should().Be(result2);
        }

        [Test]
        public void
            When_ItemSatisfiesRulesWithContainsOperatorWithACoupleOfValidAndInvalidDataOrASingleValidDatum_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule for enumerable containing 25 AND 28",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25"),
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "28")
                        }
                    },
                    new RulesSet
                    {
                        Description = "rule for enumerable containing 27",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "27")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntEnumerableProperty = new[]
                {
                    25,
                    26,
                    27
                }
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeTrue("contains a matching item in enumerable property");
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemSatisfiesRulesWithContainsOperatorWithValidData_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule for enumerable containing 25",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25", null,
                                "useless description 1")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntEnumerableProperty = new[]
                {
                    25
                }
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeTrue("contains a matching item in enumerable property");
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemSatisfiesRulesWithInternalContainsAndCorrectParameters_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerContains,
                                nameof(TestModel.IntProperty))
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntProperty = 2,
                IntEnumerableProperty = new[] {1, 2, 3}
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeTrue();
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemSatisfiesRulesWithInternalContainsAndNonCorrectParameters_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerContains,
                                nameof(TestModel.IntProperty))
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntProperty = 5,
                IntEnumerableProperty = new[] {1, 2, 3}
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeFalse();
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemSatisfiesRulesWithInternalEqualAndCorrectParameters_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringProperty), OperatorType.InnerEqual,
                                nameof(TestModel.StringProperty2))
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringProperty = "some value",
                StringProperty2 = "some value"
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeTrue();
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemSatisfiesRulesWithInternalEqualAndNoMatchingParameters_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringProperty), OperatorType.InnerEqual,
                                nameof(TestModel.StringProperty2))
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringProperty = "some value",
                StringProperty2 = "some different value"
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeFalse();
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemSatisfiesRulesWithInternalGreaterThanAndCorrectParameters_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntProperty), OperatorType.InnerGreaterThan,
                                nameof(TestModel.IntProperty2))
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntProperty = 5,
                IntProperty2 = 3
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeTrue();
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemSatisfiesRulesWithInternalNotContainsAndContainedParameters_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerNotContains,
                                nameof(TestModel.IntProperty), null,
                                "rule with inner comparison between 2 properties of the model")
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntProperty = 2,
                IntEnumerableProperty = new[] {1, 2, 3}
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeFalse();
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemSatisfiesRulesWithInternalNotContainsAndNonCorrectParameters_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerNotContains,
                                nameof(TestModel.IntProperty))
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntProperty = 5,
                IntEnumerableProperty = new[] {1, 2, 3}
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeTrue();
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemSatisfiesRulesWithMessageContainsOperatorWith2ValidData_ShouldReturnCorrectCodes()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25", "code_1",
                                null),
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "26", "code_2",
                                null)
                        }
                    },
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25", "code_3",
                                null),
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "27", "code_4",
                                null)
                        }
                    },
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "26", "code_5",
                                null),
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "28", "code_6",
                                null)
                        }
                    },
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "26", "code_2",
                                null)
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntEnumerableProperty = new[]
                {
                    25
                }
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeFalse();
            result.Success.Should().Be(result2);
            result.FailedCodes.Should().HaveCount(4);
            result.FailedCodes.Should().BeEquivalentTo("code_2", "code_4", "code_5", "code_6");
            result.FailedCodes.Should().NotContain("code_1", "code_3");
            result.FailedCodes.FirstOrDefault().Should().NotBeNullOrEmpty();
            result.FailedCodes.FirstOrDefault().Should().Be("code_2");
            result.FailedCodes.LastOrDefault().Should().NotBeNullOrEmpty();
            result.FailedCodes.LastOrDefault().Should().Be("code_6");
        }

        [Test]
        public void When_ItemSatisfiesRulesWithNoRules_ShouldReturnTrue()
        {
            _sut.SetCatalog(new RulesCatalog());
            var item = It.IsAny<TestModel>();


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeTrue("I want to be able to allow full access");
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemSatisfiesRulesWithNotOverlapsOperatorWithValidData_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.NotOverlaps, "1,2,3,4")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntEnumerableProperty = new[]
                {
                    5
                }
            };


            var result = _sut.ItemSatisfiesRules(item);


            result.Should().BeTrue();
        }

        [Test]
        public void When_ItemSatisfiesRulesWithOverlapsOperatorWithValidData_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Overlaps, "1,2,3,4")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                IntEnumerableProperty = new[]
                {
                    1
                }
            };


            var result = _sut.ItemSatisfiesRules(item);


            result.Should().BeTrue();
        }

        [Test]
        public void When_ItemsSatisfiesRulesWithAllInvalidRules_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with wrong property name",
                        Rules = new List<Rule>
                        {
                            new Rule("StringProperty_WRONG_NAME", OperatorType.Equal, "some wrong value")
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = It.IsAny<TestModel>();


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");

            result.Success.Should().BeTrue("any item satisfies an empty set rules catalog");
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemsSatisfiesRulesWithOneRuleWithCorrectParameters_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "valid rule with equality on a field",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringProperty), OperatorType.Equal, "some value")
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringProperty = "some value"
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeTrue();
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemsSatisfiesRulesWithOneRuleWithCorrectParametersAndOneWithWrongParameters_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule for string equals to value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringProperty), OperatorType.Equal, "some wrong value")
                        }
                    },
                    new RulesSet
                    {
                        Description = "rule for string equals to another value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringProperty), OperatorType.Equal, "some value")
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringProperty = "some value"
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeTrue();
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemsSatisfiesRulesWithOneRuleWithWrongParameters_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringProperty), OperatorType.Equal, "some wrong value")
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringProperty = "some value"
            };


            var watch = Stopwatch.StartNew();
            var result = _sut.ItemSatisfiesRulesWithMessage(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

            watch = Stopwatch.StartNew();
            var result2 = _sut.ItemSatisfiesRules(item);
            watch.Stop();
            Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


            result.Success.Should().BeFalse();
            result.Success.Should().Be(result2);
        }

        [Test]
        public void When_ItemStringElementIsContainedInArrayAndRulesDoesNotRequireIt_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringProperty), OperatorType.IsContained, "alpha,beta")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringProperty = "gamma"
            };


            var result = _sut.ItemSatisfiesRulesWithMessage(item);


            result.Success.Should().BeFalse();
        }

        [Test]
        public void When_ItemStringElementIsContainedInArrayAndRulesRequiresIt_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringProperty), OperatorType.IsContained, "alpha,beta")
                        }
                    }
                }
            };
            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringProperty = "alpha"
            };


            var result = _sut.ItemSatisfiesRulesWithMessage(item);


            result.Success.Should().BeTrue();
        }

        [Test]
        public void When_RuleIsContainedWithEmptyArray_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringProperty), OperatorType.IsContained, "")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringProperty = "test"
            };


            var result = _sut.ItemSatisfiesRulesWithMessage(item);


            result.Success.Should().BeFalse();
        }

        [Test]
        public void When_RuleIsDeserializedFromJsonAndOperatorIsWrittenAsAString_RuleIsSatisfied()
        {
            var jsonString = "{\"property\": \"StringProperty\", \"operator\": \"Equal\", \"value\": \"alpha\"}";
            var rule = JsonConvert.DeserializeObject<Rule>(jsonString);
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Rules = new List<Rule>
                        {
                            rule
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item1 = new TestModel
            {
                StringProperty = "alpha"
            };
            var item2 = new TestModel
            {
                StringProperty = "beta"
            };


            var result1 = _sut.ItemSatisfiesRules(item1);
            var result2 = _sut.ItemSatisfiesRules(item2);


            result1.Should().BeTrue();
            result2.Should().BeFalse();
        }

        [Test]
        public void When_RuleIsNotContainedWithEmptyArray_ShouldReturnFalse()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.StringProperty), OperatorType.IsNotContained, "")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                StringProperty = "test"
            };


            var result = _sut.ItemSatisfiesRulesWithMessage(item);


            result.Success.Should().BeTrue();
        }

        [Test]
        public void When_RuleIsOnFloatPropertyAndRuleRequiresIt_ShouldReturnTrue()
        {
            var catalog = new RulesCatalog
            {
                RulesSets = new List<RulesSet>
                {
                    new RulesSet
                    {
                        Description = "rule with single non matching value",
                        Rules = new List<Rule>
                        {
                            new Rule(nameof(TestModel.DoubleProperty), OperatorType.IsContained, "1.3,1.5")
                        }
                    }
                }
            };

            _sut.SetCatalog(catalog);
            var item = new TestModel
            {
                DoubleProperty = 1.3
            };


            var result = _sut.ItemSatisfiesRules(item);


            result.Should().BeTrue();
        }
    }
}