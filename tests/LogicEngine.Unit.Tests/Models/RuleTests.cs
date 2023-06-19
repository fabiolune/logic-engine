using LogicEngine.Internals;
using LogicEngine.Models;

namespace LogicEngine.Unit.Tests.Models;

public class RuleTests
{
    [TestCase(OperatorType.Equal, "Equal")]
    [TestCase(OperatorType.GreaterThan, "GreaterThan")]
    [TestCase(OperatorType.GreaterThanOrEqual, "GreaterThanOrEqual")]
    [TestCase(OperatorType.LessThan, "LessThan")]
    [TestCase(OperatorType.LessThanOrEqual, "LessThanOrEqual")]
    [TestCase(OperatorType.NotEqual, "NotEqual")]
    [TestCase(OperatorType.Contains, "Contains")]
    [TestCase(OperatorType.NotContains, "NotContains")]
    [TestCase(OperatorType.Overlaps, "Overlaps")]
    [TestCase(OperatorType.NotOverlaps, "NotOverlaps")]
    [TestCase(OperatorType.ContainsKey, "ContainsKey")]
    [TestCase(OperatorType.NotContainsKey, "NotContainsKey")]
    [TestCase(OperatorType.ContainsValue, "ContainsValue")]
    [TestCase(OperatorType.NotContainsValue, "NotContainsValue")]
    [TestCase(OperatorType.KeyContainsValue, "KeyContainsValue")]
    [TestCase(OperatorType.NotKeyContainsValue, "NotKeyContainsValue")]
    [TestCase(OperatorType.IsContained, "IsContained")]
    [TestCase(OperatorType.IsNotContained, "IsNotContained")]
    [TestCase(OperatorType.InnerEqual, "InnerEqual")]
    [TestCase(OperatorType.InnerGreaterThan, "InnerGreaterThan")]
    [TestCase(OperatorType.InnerGreaterThanOrEqual, "InnerGreaterThanOrEqual")]
    [TestCase(OperatorType.InnerLessThan, "InnerLessThan")]
    [TestCase(OperatorType.InnerLessThanOrEqual, "InnerLessThanOrEqual")]
    [TestCase(OperatorType.InnerNotEqual, "InnerNotEqual")]
    [TestCase(OperatorType.InnerContains, "InnerContains")]
    [TestCase(OperatorType.InnerNotContains, "InnerNotContains")]
    [TestCase(OperatorType.InnerOverlaps, "InnerOverlaps")]
    [TestCase(OperatorType.InnerNotOverlaps, "InnerNotOverlaps")]
    public void ToString_ShouldReturnExpectedSerialization(OperatorType type, string expected)
    {
        var rule = new Rule("property", type, "value", "code");

        var result = rule.ToString();

        result
            .Replace("\n", "")
            .Replace("\r", "")
            .Should()
            .Be($"[Rule (property: property, operator: {expected}, value: value, code: code)]");
    }
}