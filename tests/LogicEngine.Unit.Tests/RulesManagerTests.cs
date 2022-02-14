using System;
using System.Collections.Generic;
using System.Diagnostics;
using FluentAssertions;
using LogicEngine.Internals;
using LogicEngine.Models;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Serilog;

namespace LogicEngine.Unit.Tests;

[TestFixture]
public class RulesManagerTests
{
    private readonly RulesManager<TestModel> _sut;

    public RulesManagerTests()
    {
        var logger = new Mock<ILogger>();
        _sut = new(new RulesCompiler(logger.Object));
    }

    [TestCase(new[] { 1, 2 }, new[] { 1, 3 }, true)]
    [TestCase(new[] { 1, 2 }, new[] { 3, 4 }, false)]
    [TestCase(null, new[] { 3, 4 }, false)]
    [TestCase(new[] { 1, 2 }, null, false)]
    [TestCase(null, null, false)]
    public void When_ItemSatisfiesRulesWithInnerOverlapsOperator_ShouldReturnExpectedResult(
        IEnumerable<int> first,
        IEnumerable<int> second,
        bool expectedResult)
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerOverlaps,
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

    [TestCase(new[] { 1, 2 }, new[] { 1, 3 }, false)]
    [TestCase(new[] { 1, 2 }, new[] { 3, 4 }, true)]
    [TestCase(null, new[] { 3, 4 }, true)]
    [TestCase(new[] { 1, 2 }, null, true)]
    [TestCase(null, null, true)]
    public void When_ApplyInnerNotOverlapsOperator_ShouldReturnExpectedResult(IEnumerable<int> first,
        IEnumerable<int> second, bool expectedResult)
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerNotOverlaps,
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
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringStringDictionaryProperty), operatorType, keyToCheck)
                    }
                }
            }
        };

        _sut.SetCatalog(catalog);
        var item = new TestModel
        {
            StringStringDictionaryProperty = new()
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
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringStringDictionaryProperty), operatorType, valueToCheck)
                    }
                }
            }
        };

        _sut.SetCatalog(catalog);
        var item = new TestModel
        {
            StringStringDictionaryProperty = new()
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
                new()
                {
                    Rules = new List<Rule>
                    {
                        new($"{nameof(TestModel.StringStringDictionaryProperty)}[{ruleKey}]", operatorType,
                            ruleValue)
                    }
                }
            }
        };
        _sut.SetCatalog(catalog);
        var item = new TestModel
        {
            StringStringDictionaryProperty = new()
            {
                {presentKey, presentValue}
            }
        };

        var result = _sut.ItemSatisfiesRules(item);

        result.Should().Be(expectedResult);
    }

    [Test]
    public void When_FilterOnSetOfMatchingItems_ShouldReturnEquivalentResult()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule for enumerable containing 25 AND 28",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25"),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "28")
                    }
                },
                new()
                {
                    Description = "rule for enumerable containing 27",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "27")
                    }
                }
            }
        };
        _sut.SetCatalog(catalog);
        var items = new List<TestModel>
        {
            new()
            {
                IntEnumerableProperty = new[]
                {
                    25,
                    28
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    27
                }
            }
        };

        var result = _sut.Filter(items);

        result.Should().BeEquivalentTo(items);
    }

    [Test]
    public void When_FilterOnSetOfSomeMatchingItems_ShouldFilterThemOut()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule for enumerable containing 25 AND 28",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25"),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "28")
                    }
                },
                new()
                {
                    Description = "rule for enumerable containing 27",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "27")
                    }
                }
            }
        };
        _sut.SetCatalog(catalog);
        var items = new List<TestModel>
        {
            new()
            {
                IntEnumerableProperty = new[]
                {
                    28
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    27
                }
            }
        };

        var result = _sut.Filter(items);

        result.Should().BeEquivalentTo(new List<TestModel>
        {
            new()
            {
                IntEnumerableProperty = new[]
                {
                    27
                }
            }
        }, options => options.ComparingByMembers<TestModel>());
    }

    [Test]
    public void When_FirstOrDefaultOnSetOfSomeMatchingItems_ShouldReturnFirst()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule for enumerable containing 25 AND 28",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25"),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "28")
                    }
                },
                new()
                {
                    Description = "rule for enumerable containing 27",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "27")
                    }
                }
            }
        };
        _sut.SetCatalog(catalog);
        var items = new List<TestModel>
        {
            new()
            {
                IntEnumerableProperty = new[]
                {
                    28
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    27
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    25,
                    28
                }
            }
        };

        var result = _sut.FirstOrDefault(items);

        result.Should().BeEquivalentTo(new TestModel
        {
            IntEnumerableProperty = new[]
            {
                27
            }
        }, options => options.ComparingByMembers<TestModel>());
    }

    [Test]
    public void When_FirstOrDefaultOnSetOfNonMatchingItems_ShouldReturnDefault()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule for enumerable containing 25 AND 28",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25"),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "28")
                    }
                },
                new()
                {
                    Description = "rule for enumerable containing 27",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "27")
                    }
                }
            }
        };
        _sut.SetCatalog(catalog);
        var items = new List<TestModel>
        {
            new()
            {
                IntEnumerableProperty = new[]
                {
                    28
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    26
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    25,
                    29
                }
            }
        };

        var result = _sut.FirstOrDefault(items);

        result.Should().BeEquivalentTo(default(TestModel), options => options.ComparingByMembers<TestModel>());
    }

    [Test]
    public void When_ItemDoesNotSatisfiesRulesWithOverlapsOperatorWithValidData_ShouldReturnFalse()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Overlaps, "1,2,3,4")
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
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Overlaps, "1,2,3,4")
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
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.NotOverlaps, "1,2,3,4")
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
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntProperty), OperatorType.IsContained, "1,2")
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


        result.IsRight.Should().BeFalse();
    }

    [Test]
    public void When_ItemIntegerElementIsContainedInArrayAndRulesRequiresIt_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntProperty), OperatorType.IsContained, "1,2")
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


        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void When_ItemIntegerElementIsNotContainedInArrayAndRulesDoesNotRequireIt_ShouldReturnFalse()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntProperty), OperatorType.IsNotContained, "1,2")
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


        result.IsRight.Should().BeFalse();
    }

    [Test]
    public void When_ItemIntegerElementIsNotContainedInArrayAndRulesRequiresIt_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntProperty), OperatorType.IsNotContained, "1,2")
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


        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void When_ItemSatisfiesRulesWithContainsOperatorWith2ValidData_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25"),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "26")
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


        result.IsRight.Should().BeTrue("contains a matching item in enumerable property");
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void
        When_ItemSatisfiesRulesWithContainsOperatorWithACoupleOfValidAndInvalidDataOrASingleValidDatum_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule for enumerable containing 25 AND 28",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25"),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "28")
                    }
                },
                new()
                {
                    Description = "rule for enumerable containing 27",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "27")
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


        var result = _sut.ItemSatisfiesRulesWithMessage(item);

        var result2 = _sut.ItemSatisfiesRules(item);

        result.IsRight.Should().BeTrue("contains a matching item in enumerable property");
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemSatisfiesRulesWithContainsOperatorWithValidData_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule for enumerable containing 25",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25", null,
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


        result.IsRight.Should().BeTrue("contains a matching item in enumerable property");
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemSatisfiesRulesWithInternalContainsAndCorrectParameters_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerContains,
                            nameof(TestModel.IntProperty))
                    }
                }
            }
        };
        _sut.SetCatalog(catalog);
        var item = new TestModel
        {
            IntProperty = 2,
            IntEnumerableProperty = new[] { 1, 2, 3 }
        };


        var watch = Stopwatch.StartNew();
        var result = _sut.ItemSatisfiesRulesWithMessage(item);
        watch.Stop();
        Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

        watch = Stopwatch.StartNew();
        var result2 = _sut.ItemSatisfiesRules(item);
        watch.Stop();
        Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


        result.IsRight.Should().BeTrue();
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemSatisfiesRulesWithInternalContainsAndNonCorrectParameters_ShouldReturnFalse()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerContains,
                            nameof(TestModel.IntProperty))
                    }
                }
            }
        };
        _sut.SetCatalog(catalog);
        var item = new TestModel
        {
            IntProperty = 5,
            IntEnumerableProperty = new[] { 1, 2, 3 }
        };


        var watch = Stopwatch.StartNew();
        var result = _sut.ItemSatisfiesRulesWithMessage(item);
        watch.Stop();
        Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

        watch = Stopwatch.StartNew();
        var result2 = _sut.ItemSatisfiesRules(item);
        watch.Stop();
        Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


        result.IsRight.Should().BeFalse();
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemSatisfiesRulesWithInternalEqualAndCorrectParameters_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.InnerEqual,
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


        result.IsRight.Should().BeTrue();
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemSatisfiesRulesWithInternalEqualAndNoMatchingParameters_ShouldReturnFalse()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.InnerEqual,
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


        result.IsRight.Should().BeFalse();
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemSatisfiesRulesWithInternalGreaterThanAndCorrectParameters_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntProperty), OperatorType.InnerGreaterThan,
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


        result.IsRight.Should().BeTrue();
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemSatisfiesRulesWithInternalNotContainsAndContainedParameters_ShouldReturnFalse()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerNotContains,
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
            IntEnumerableProperty = new[] { 1, 2, 3 }
        };


        var watch = Stopwatch.StartNew();
        var result = _sut.ItemSatisfiesRulesWithMessage(item);
        watch.Stop();
        Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

        watch = Stopwatch.StartNew();
        var result2 = _sut.ItemSatisfiesRules(item);
        watch.Stop();
        Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


        result.IsRight.Should().BeFalse();
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemSatisfiesRulesWithInternalNotContainsAndNonCorrectParameters_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerNotContains,
                            nameof(TestModel.IntProperty))
                    }
                }
            }
        };
        _sut.SetCatalog(catalog);
        var item = new TestModel
        {
            IntProperty = 5,
            IntEnumerableProperty = new[] { 1, 2, 3 }
        };


        var watch = Stopwatch.StartNew();
        var result = _sut.ItemSatisfiesRulesWithMessage(item);
        watch.Stop();
        Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

        watch = Stopwatch.StartNew();
        var result2 = _sut.ItemSatisfiesRules(item);
        watch.Stop();
        Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


        result.IsRight.Should().BeTrue();
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemSatisfiesRulesWithMessageContainsOperatorWith2ValidData_ShouldReturnCorrectCodes()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25", "code_1",
                            null),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "26", "code_2",
                            null)
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25", "code_3",
                            null),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "27", "code_4",
                            null)
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "26", "code_5",
                            null),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "28", "code_6",
                            null)
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "26", "code_2",
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


        var result = _sut.ItemSatisfiesRulesWithMessage(item);

        var result2 = _sut.ItemSatisfiesRules(item);

        result.IsRight.Should().BeFalse();
        result.IsRight.Should().Be(result2);
        result.OnLeft(_ =>
        {
            _.Should().BeEquivalentTo("code_2", "code_4", "code_5", "code_6");
        });
    }

    [Test]
    public void When_ItemSatisfiesRulesWithMessageContainsOperatorWith2ValidData_ShouldReturnCorrectCodesWithNoullsOrEmptyStrings()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25", "code_1",
                            null),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "26", "code_2",
                            null)
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "25", "code_3",
                            null),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "27")
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "26", "code_5",
                            null),
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "28", "code_6",
                            null)
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "26", "code_2",
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


        var result = _sut.ItemSatisfiesRulesWithMessage(item);

        var result2 = _sut.ItemSatisfiesRules(item);

        result.IsRight.Should().BeFalse();
        result.IsRight.Should().Be(result2);
        result.OnLeft(_ =>
        {
            _.Should().BeEquivalentTo("code_2", "code_5", "code_6");
        });
    }
    [Test]
    public void When_ItemSatisfiesRulesWithNoRules_ShouldReturnTrue()
    {
        _sut.SetCatalog(new());
        var item = It.IsAny<TestModel>();


        var watch = Stopwatch.StartNew();
        var result = _sut.ItemSatisfiesRulesWithMessage(item);
        watch.Stop();
        Console.WriteLine($"ItemSatisfiesRulesWithMessage: {watch.ElapsedTicks} ticks");

        watch = Stopwatch.StartNew();
        var result2 = _sut.ItemSatisfiesRules(item);
        watch.Stop();
        Console.WriteLine($"ItemSatisfiesRules:            {watch.ElapsedTicks} ticks");


        result.IsRight.Should().BeTrue("I want to be able to allow full access");
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemSatisfiesRulesWithNotOverlapsOperatorWithValidData_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.NotOverlaps, "1,2,3,4")
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
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.IntEnumerableProperty), OperatorType.Overlaps, "1,2,3,4")
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
                new()
                {
                    Description = "rule with wrong property name",
                    Rules = new List<Rule>
                    {
                        new("StringProperty_WRONG_NAME", OperatorType.Equal, "some wrong value")
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

        result.IsRight.Should().BeTrue("any item satisfies an empty set rules catalog");
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemsSatisfiesRulesWithOneRuleWithCorrectParameters_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "valid rule with equality on a field",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "some value")
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


        result.IsRight.Should().BeTrue();
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemsSatisfiesRulesWithOneRuleWithCorrectParametersAndOneWithWrongParameters_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule for string equals to value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "some wrong value")
                    }
                },
                new()
                {
                    Description = "rule for string equals to another value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "some value")
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


        result.IsRight.Should().BeTrue();
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemsSatisfiesRulesWithOneRuleWithWrongParameters_ShouldReturnFalse()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "some wrong value")
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


        result.IsRight.Should().BeFalse();
        result.IsRight.Should().Be(result2);
    }

    [Test]
    public void When_ItemStringElementIsContainedInArrayAndRulesDoesNotRequireIt_ShouldReturnFalse()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.IsContained, "alpha,beta")
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


        result.IsRight.Should().BeFalse();
    }

    [Test]
    public void When_ItemStringElementIsContainedInArrayAndRulesRequiresIt_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.IsContained, "alpha,beta")
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


        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void When_RuleIsContainedWithEmptyArray_ShouldReturnFalse()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.IsContained, "")
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


        result.IsRight.Should().BeFalse();
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
                new()
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
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.IsNotContained, "")
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


        result.IsRight.Should().BeTrue();
    }

    [Test]
    public void When_RuleIsOnFloatPropertyAndRuleRequiresIt_ShouldReturnTrue()
    {
        var catalog = new RulesCatalog
        {
            RulesSets = new List<RulesSet>
            {
                new()
                {
                    Description = "rule with single non matching value",
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.DoubleProperty), OperatorType.IsContained, "1.3,1.5")
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