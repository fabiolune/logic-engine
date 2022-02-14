using System;
using System.Collections.Generic;
using FluentAssertions;
using LogicEngine.Internals;
using LogicEngine.Models;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Serilog;

namespace LogicEngine.Unit.Tests;

[TestFixture]
public class RulesCompilerTests
{
    private readonly RulesCompiler _sut;
    private readonly Mock<ILogger> _mockLogger;

    public RulesCompilerTests()
    {
        _mockLogger = new Mock<ILogger>();
        _sut = new RulesCompiler(_mockLogger.Object);
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
    public void CompileRules_WhenPassedRuleWithWrongPropertyName_ShouldDiscardIt(OperatorType type)
    {
        var rule = new Rule("StringPropertyWrong", type, "value 1");

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(0);
        _mockLogger.Verify(_ => _.Error(It.IsAny<Exception>(), 
            "{Component} raised an exception with {Message} when compiling {Rule}", 
            nameof(RulesCompiler),
            It.IsAny<string>(), rule), Times.Once);
    }

    [TestCase(OperatorType.Equal)]
    [TestCase(OperatorType.NotEqual)]
    [TestCase(OperatorType.GreaterThan)]
    [TestCase(OperatorType.LessThan)]
    [TestCase(OperatorType.GreaterThanOrEqual)]
    [TestCase(OperatorType.LessThanOrEqual)]
    public void When_CompileRulesWithDirectOperations_ShouldReturnCompiledRules(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.IntProperty), op, "3");

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(1);
    }

    [TestCase(OperatorType.Contains)]
    [TestCase(OperatorType.NotContains)]
    public void When_CompileRulesWithContainingOperation_ShouldReturnRightCompiledRule(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), op, "3");

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(1);
    }

    [TestCase(OperatorType.InnerEqual)]
    [TestCase(OperatorType.InnerNotEqual)]
    [TestCase(OperatorType.InnerGreaterThan)]
    [TestCase(OperatorType.InnerGreaterThanOrEqual)]
    [TestCase(OperatorType.InnerLessThan)]
    [TestCase(OperatorType.InnerLessThanOrEqual)]
    public void When_CompileRulesWithCorrectInnerOperation_ShouldReturnCompiledRule(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.IntProperty), op, nameof(TestModel.IntProperty2));

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(1);
    }

    [TestCase(OperatorType.InnerEqual)]
    [TestCase(OperatorType.InnerNotEqual)]
    [TestCase(OperatorType.InnerGreaterThan)]
    [TestCase(OperatorType.InnerGreaterThanOrEqual)]
    [TestCase(OperatorType.InnerLessThan)]
    [TestCase(OperatorType.InnerLessThanOrEqual)]
    public void When_CompileRulesWithInternalOperatorAndNonMatchingTypes_ShouldSkipRule(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.StringProperty), op, nameof(TestModel.IntProperty));

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(0);
    }

    [TestCase(OperatorType.InnerContains)]
    [TestCase(OperatorType.InnerNotContains)]
    public void When_CompileRulesWithInternalEnumerableOperator_ShouldCompileRule(OperatorType op)
    {
        var rule1 = new Rule(nameof(TestModel.IntEnumerableProperty), op, nameof(TestModel.IntProperty));
        var rule2 = new Rule(nameof(TestModel.StringArrayProperty), op, nameof(TestModel.StringProperty));

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule1,
            rule2
        });

        compiledRules.Should().HaveCount(2);
    }

    [TestCase(OperatorType.InnerOverlaps)]
    [TestCase(OperatorType.InnerNotOverlaps)]
    public void When_CompileRulesWithInternalEnumerableCompareOperator_ShouldCompileRule(OperatorType op)
    {
        var rule1 = new Rule(nameof(TestModel.IntEnumerableProperty), op, nameof(TestModel.IntEnumerableProperty2));
        var rule2 = new Rule(nameof(TestModel.StringArrayProperty), op, nameof(TestModel.StringArrayProperty2));

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule1,
            rule2
        });

        compiledRules.Should().HaveCount(2);
    }

    [TestCase("IntEnumerableProperty", "1,2", 1, OperatorType.Overlaps)]
    [TestCase("IntEnumerableProperty", "a,b", 0, OperatorType.Overlaps)]
    [TestCase("IntEnumerableProperty", "a,1", 0, OperatorType.Overlaps)]
    [TestCase("StringEnumerableProperty", "1,2", 1, OperatorType.Overlaps)]
    [TestCase("StringEnumerableProperty", "a,b", 1, OperatorType.Overlaps)]
    [TestCase("IntEnumerableProperty", "1,2", 1, OperatorType.NotOverlaps)]
    [TestCase("IntEnumerableProperty", "a,b", 0, OperatorType.NotOverlaps)]
    [TestCase("IntEnumerableProperty", "a,1", 0, OperatorType.NotOverlaps)]
    [TestCase("StringEnumerableProperty", "1,2", 1, OperatorType.NotOverlaps)]
    [TestCase("StringEnumerableProperty", "a,b", 1, OperatorType.NotOverlaps)]
    public void When_CompileRulesWithOverlappingOperators_ShouldCompileRules(string propertyName,
        string constantValue, int expectedCount, OperatorType op)
    {
        var rule = new Rule(propertyName, op, constantValue);

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(expectedCount);
    }

    [TestCase(OperatorType.ContainsKey)]
    [TestCase(OperatorType.NotContainsKey)]
    [TestCase(OperatorType.ContainsValue)]
    [TestCase(OperatorType.NotContainsValue)]
    public void When_CompileRulesWithGenericKeyValueOperator_ShouldCompileRules(OperatorType type)
    {
        var rule = new Rule(nameof(TestModel.StringStringDictionaryProperty), type, "some");

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(1);
    }

    [TestCase(OperatorType.KeyContainsValue)]
    [TestCase(OperatorType.NotKeyContainsValue)]
    public void When_CompileRulesWithKeyAndValueOperator_ShouldCompileRules(OperatorType type)
    {
        var rule = new Rule("StringStringDictionaryProperty[key]", type, "value");

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(1);
    }

    [TestCase(OperatorType.InnerContains)]
    [TestCase(OperatorType.InnerNotContains)]
    [TestCase(OperatorType.InnerOverlaps)]
    [TestCase(OperatorType.InnerNotOverlaps)]
    public void When_CompileRulesWithNotMatchingTypes_ShouldNotCompileRules(OperatorType type)
    {
        var rule = new Rule("StringEnumerableProperty", type, "IntProperty");

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(0);
    }

    [Test]
    public void CompileRules_WhenPassedValidRules_ShouldReturnCorrectAmountOfCompiledRules()
    {
        var rule1 = new Rule(nameof(TestModel.StringProperty), OperatorType.Equal, "value 1");
        var rule2 = new Rule(nameof(TestModel.StringProperty), OperatorType.Equal, "value 2");
        var rule3 = new Rule(nameof(TestModel.EnumProperty), OperatorType.Equal, "Two");

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule1, rule2, rule3
        });

        compiledRules.Should().HaveCount(3);
    }

    [Test]
    public void When_CompileRulesFromSerializedJson_ShouldReturnCompiledRules()
    {
        var jsonString =
            "{\"property\": \"StringProperty\", \"operator\": \"Equal\", \"value\": \"some value\", \"code\": \"some code\", \"description\": \"some description\"}";
        var rule = JsonConvert.DeserializeObject<Rule>(jsonString);

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(1);
    }

    [Test]
    public void When_CompileRulesWithNotExistingOperatorType_ShouldNotCompileRules()
    {
        var rule = new Rule(nameof(TestModel.StringEnumerableProperty), (OperatorType) 1000, "StringProperty");

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(0);
    }

    [Test]
    public void When_RuleIsForArrays_ShouldCompileRule()
    {
        var rule = new Rule(nameof(TestModel.StringArrayProperty), OperatorType.Contains, "value");

        var compiledRules = _sut.CompileRules<TestModel>(new List<Rule>
        {
            rule
        });

        compiledRules.Should().HaveCount(1);
    }
}