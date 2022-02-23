using System.Collections.Generic;
using FluentAssertions;
using LogicEngine.Internals;
using LogicEngine.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace LogicEngine.Unit.Tests;

public class SingleRuleCompilerTests
{
    private SingleRuleCompiler _sut;

    [SetUp]
    public void SetUp()
    {
        _sut = new SingleRuleCompiler();
    }

    [TestCase(OperatorType.Equal)]
    [TestCase(OperatorType.GreaterThan)]
    [TestCase(OperatorType.GreaterThanOrEqual)]
    [TestCase(OperatorType.LessThan)]
    [TestCase(OperatorType.LessThanOrEqual)]
    [TestCase(OperatorType.NotEqual)]
    [TestCase(OperatorType.Contains)]
    [TestCase(OperatorType.NotContains)]
    [TestCase(OperatorType.Overlaps)]
    [TestCase(OperatorType.NotOverlaps)]
    [TestCase(OperatorType.ContainsKey)]
    [TestCase(OperatorType.NotContainsKey)]
    [TestCase(OperatorType.ContainsValue)]
    [TestCase(OperatorType.NotContainsValue)]
    [TestCase(OperatorType.KeyContainsValue)]
    [TestCase(OperatorType.NotKeyContainsValue)]
    [TestCase(OperatorType.IsContained)]
    [TestCase(OperatorType.IsNotContained)]
    [TestCase(OperatorType.InnerEqual)]
    [TestCase(OperatorType.InnerGreaterThan)]
    [TestCase(OperatorType.InnerGreaterThanOrEqual)]
    [TestCase(OperatorType.InnerLessThan)]
    [TestCase(OperatorType.InnerLessThanOrEqual)]
    [TestCase(OperatorType.InnerNotEqual)]
    [TestCase(OperatorType.InnerContains)]
    [TestCase(OperatorType.InnerNotContains)]
    [TestCase(OperatorType.InnerOverlaps)]
    [TestCase(OperatorType.InnerNotOverlaps)]
    public void Compile_WhenPropertyIsWrong_ShouldReturnNone(OperatorType type)
    {
        var rule = new Rule("StringPropertyWrong", type, "value 1", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsNone.Should().BeTrue();
    }

    [TestCase(OperatorType.Equal)]
    [TestCase(OperatorType.NotEqual)]
    [TestCase(OperatorType.GreaterThan)]
    [TestCase(OperatorType.LessThan)]
    [TestCase(OperatorType.GreaterThanOrEqual)]
    [TestCase(OperatorType.LessThanOrEqual)]
    public void Compile_WhenDirectRulesAreCorrect_ShouldReturnSome(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.IntProperty), op, "3", null);

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
    }

    [TestCase(OperatorType.Equal)]
    [TestCase(OperatorType.NotEqual)]
    public void Compile_WhenDirectRulesAreCorrectWithEnums_ShouldReturnSome(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.EnumProperty), op, "One", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
    }

    [TestCase(OperatorType.Contains)]
    [TestCase(OperatorType.NotContains)]
    public void Compile_WhenEnumerableRulesAreCorrect_ShouldReturnSome(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), op, "3", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
    }

    [TestCase(OperatorType.InnerEqual)]
    [TestCase(OperatorType.InnerNotEqual)]
    [TestCase(OperatorType.InnerGreaterThan)]
    [TestCase(OperatorType.InnerGreaterThanOrEqual)]
    [TestCase(OperatorType.InnerLessThan)]
    [TestCase(OperatorType.InnerLessThanOrEqual)]
    public void Compile_WhenInnerRulesAreCorrect_ShouldReturnSome(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.IntProperty), op, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
    }

    [TestCase(OperatorType.InnerEqual)]
    [TestCase(OperatorType.InnerNotEqual)]
    [TestCase(OperatorType.InnerGreaterThan)]
    [TestCase(OperatorType.InnerGreaterThanOrEqual)]
    [TestCase(OperatorType.InnerLessThan)]
    [TestCase(OperatorType.InnerLessThanOrEqual)]
    public void Compile_WhenRulesWithInternalOperatorDoNotMatchType_ShouldReturnNone(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.StringProperty), op, nameof(TestModel.IntProperty), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsNone.Should().BeTrue();
    }

    [TestCase(OperatorType.InnerContains, nameof(TestModel.IntEnumerableProperty), nameof(TestModel.IntProperty))]
    [TestCase(OperatorType.InnerNotContains, nameof(TestModel.IntEnumerableProperty), nameof(TestModel.IntProperty))]
    [TestCase(OperatorType.InnerContains, nameof(TestModel.StringArrayProperty), nameof(TestModel.StringProperty))]
    [TestCase(OperatorType.InnerNotContains, nameof(TestModel.StringArrayProperty), nameof(TestModel.StringProperty))]
    public void Compile_WhenRulesWithInternalEnumerableOperatorAreCorrect_ShouldReturnSome(OperatorType op, string property, string value)
    {
        var rule = new Rule(property, op, value, "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
    }

    [TestCase(OperatorType.InnerOverlaps, nameof(TestModel.IntEnumerableProperty), nameof(TestModel.IntEnumerableProperty2))]
    [TestCase(OperatorType.InnerNotOverlaps, nameof(TestModel.IntEnumerableProperty), nameof(TestModel.IntEnumerableProperty2))]
    [TestCase(OperatorType.InnerOverlaps, nameof(TestModel.StringArrayProperty), nameof(TestModel.StringArrayProperty2))]
    [TestCase(OperatorType.InnerNotOverlaps, nameof(TestModel.StringArrayProperty), nameof(TestModel.StringArrayProperty2))]
    public void When_CompileRulesWithInternalEnumerableCompareOperator_ShouldCompileRule(OperatorType op, string property, string value)
    {
        var rule = new Rule(property, op, value, "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
    }

    [TestCase("IntEnumerableProperty", "1,2", OperatorType.Overlaps, true)]
    [TestCase("IntEnumerableProperty", "a,b", OperatorType.Overlaps, false)]
    [TestCase("IntEnumerableProperty", "a,1", OperatorType.Overlaps, false)]
    [TestCase("StringEnumerableProperty", "1,2", OperatorType.Overlaps, true)]
    [TestCase("StringEnumerableProperty", "a,b", OperatorType.Overlaps, true)]
    [TestCase("IntEnumerableProperty", "1,2", OperatorType.NotOverlaps, true)]
    [TestCase("IntEnumerableProperty", "a,b", OperatorType.NotOverlaps, false)]
    [TestCase("IntEnumerableProperty", "a,1", OperatorType.NotOverlaps, false)]
    [TestCase("StringEnumerableProperty", "1,2", OperatorType.NotOverlaps, true)]
    [TestCase("StringEnumerableProperty", "a,b", OperatorType.NotOverlaps, true)]
    public void When_CompileRulesWithOverlappingOperators_ShouldCompileRules(string propertyName,
        string constantValue, OperatorType op, bool expectedSome)
    {
        var rule = new Rule(propertyName, op, constantValue, "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().Be(expectedSome);
    }

    [TestCase(OperatorType.ContainsKey)]
    [TestCase(OperatorType.NotContainsKey)]
    [TestCase(OperatorType.ContainsValue)]
    [TestCase(OperatorType.NotContainsValue)]
    public void When_CompileRulesWithGenericKeyValueOperator_ShouldCompileRules(OperatorType type)
    {
        var rule = new Rule(nameof(TestModel.StringStringDictionaryProperty), type, "some", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
    }

    [TestCase(OperatorType.KeyContainsValue)]
    [TestCase(OperatorType.NotKeyContainsValue)]
    public void When_CompileRulesWithKeyAndValueOperator_ShouldCompileRules(OperatorType type)
    {
        var rule = new Rule("StringStringDictionaryProperty[key]", type, "value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
    }

    [TestCase(OperatorType.InnerContains)]
    [TestCase(OperatorType.InnerNotContains)]
    [TestCase(OperatorType.InnerOverlaps)]
    [TestCase(OperatorType.InnerNotOverlaps)]
    public void When_CompileRulesWithNotMatchingTypes_ShouldNotCompileRules(OperatorType type)
    {
        var rule = new Rule("StringEnumerableProperty", type, "IntProperty", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsNone.Should().BeTrue();
    }

    [TestCase(OperatorType.IsContained)]
    [TestCase(OperatorType.IsNotContained)]
    public void Compile_WhenInverseEnumerableRulesAreCorrect_ShouldReturnSome(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.StringProperty), op, nameof(TestModel.StringArrayProperty), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
    }

    [Test]
    public void When_CompileRulesFromSerializedJson_ShouldReturnCompiledRules()
    {
        const string jsonString = "{\"property\": \"StringProperty\", \"operator\": \"Equal\", \"value\": \"some value\", \"code\": \"some code\"}";
        var rule = JsonConvert.DeserializeObject<Rule>(jsonString);

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
    }

    [Test]
    public void When_CompileRulesWithNotExistingOperatorType_ShouldNotCompileRules()
    {
        var rule = new Rule(nameof(TestModel.StringEnumerableProperty), (OperatorType)1000, "StringProperty", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsNone.Should().BeTrue();
    }

    [Test]
    public void When_RuleIsForArrays_ShouldCompileRule()
    {
        var rule = new Rule(nameof(TestModel.StringArrayProperty), OperatorType.Contains, "value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
    }

    [Test]
    public void Compile_WhenRuleIsEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.Equal, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 13
            }).IsLeft.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.NotEqual, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12
            }).IsLeft.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 13
            }).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsGreaterThan_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.GreaterThan, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12
            }).IsLeft.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 13
            }).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsGreaterThanOrEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.GreaterThanOrEqual, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 13
            }).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsLessThan_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.LessThan, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntProperty = 11
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 13
            }).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsLessThanOrEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.LessThanOrEqual, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 11
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 13
            }).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsContains_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 12, 13 }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 13 }
            }).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotContains_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.NotContains, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 12, 13 }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 13 }
            }).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsOverlaps_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Overlaps, "12,13", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 12, 13 }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 13 }
            }).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotOverlaps_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.NotOverlaps, "12,13", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 12, 13 }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 13 }
            }).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsContainsKey_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringStringDictionaryProperty), OperatorType.ContainsKey, "my_key", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_key", "my_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel()).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotContainsKey_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringStringDictionaryProperty), OperatorType.NotContainsKey, "my_key", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_key", "my_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel()).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsContainsValue_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringStringDictionaryProperty), OperatorType.ContainsValue, "my_value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_key", "my_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_wrong_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel()).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotContainsValue_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringStringDictionaryProperty), OperatorType.NotContainsValue, "my_value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    { "my_key", "my_value" },
                    { "another_key", "another_value" },
                }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_wrong_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel()).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsKeyContainsValue_ShouldReturnExpectedFunction()
    {
        var rule = new Rule($"{nameof(TestModel.StringStringDictionaryProperty)}[my_key]", OperatorType.KeyContainsValue, "my_value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    { "my_key", "my_value" },
                    { "another_key", "another_value" },
                }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    { "my_key", "my_wrong_value" },
                    { "another_key", "another_value" },
                }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_wrong_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel()).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotKeyContainsValue_ShouldReturnExpectedFunction()
    {
        var rule = new Rule($"{nameof(TestModel.StringStringDictionaryProperty)}[my_key]", OperatorType.NotKeyContainsValue, "my_value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    { "my_key", "my_value" },
                    { "another_key", "another_value" },
                }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    { "my_key", "my_wrong_value" },
                    { "another_key", "another_value" },
                }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_wrong_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel()).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsIsContained_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.IsContained, "12,13", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel()).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsIsNotContained_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.IsNotContained, "12,13", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel()).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerEqualOnSameProperty_ShouldAlwaysReturnRight()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerEqual, nameof(TestModel.IntProperty), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel()).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerEqual, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 14,
                IntProperty2 = 13
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel()).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerNotEqualOnSameProperty_ShouldAlwaysReturnLeft()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerNotEqual, nameof(TestModel.IntProperty), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel()).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerNotEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerNotEqual, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntProperty = 14,
                IntProperty2 = 13
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel()).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerGreaterThan_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerGreaterThan, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntProperty = 14,
                IntProperty2 = 13
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel()).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerGreaterThanOrEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerGreaterThanOrEqual, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 14,
                IntProperty2 = 13
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel()).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerLessThan_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerLessThan, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntProperty = 13,
                IntProperty2 = 14
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel()).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerLessThanOrEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerLessThanOrEqual, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 13,
                IntProperty2 = 14
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel()).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerContains_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerContains, nameof(TestModel.IntProperty), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12,
                IntEnumerableProperty = new[] { 11, 12 }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 13,
                IntEnumerableProperty = new[] { 11, 12 }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel()).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerNotContains_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerNotContains, nameof(TestModel.IntProperty), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntProperty = 12,
                IntEnumerableProperty = new[] { 11, 12 }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntProperty = 13,
                IntEnumerableProperty = new[] { 11, 12 }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntProperty = 14
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel()).IsRight.Should().BeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerOverlaps_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerOverlaps, nameof(TestModel.IntEnumerableProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 12 },
                IntEnumerableProperty2 = new[] { 11, 13 }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 12 },
                IntEnumerableProperty2 = new[] { 13, 14 }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 12 },
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel()).IsRight.Should().BeFalse();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerNotOverlaps_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerNotOverlaps, nameof(TestModel.IntEnumerableProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.Should().BeTrue();
        result.OnSome(_ =>
        {
            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 12 },
                IntEnumerableProperty2 = new[] { 11, 13 }
            }).IsRight.Should().BeFalse();

            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 12 },
                IntEnumerableProperty2 = new[] { 13, 14 }
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel
            {
                IntEnumerableProperty = new[] { 11, 12 },
            }).IsRight.Should().BeTrue();

            _.Executable(new TestModel()).IsRight.Should().BeTrue();
        });
    }
}