using FluentAssertions;
using NUnit.Framework;
using RulesEngine.Internals;
using RulesEngine.Models;

namespace RulesEngine.Unit.Tests.Models
{
    public class RuleTests
    {
        [TestCase(OperatorType.Equal, 1)]
        [TestCase(OperatorType.GreaterThan, 2)]
        [TestCase(OperatorType.GreaterThanOrEqual, 3)]
        [TestCase(OperatorType.LessThan, 4)]
        [TestCase(OperatorType.LessThanOrEqual, 5)]
        [TestCase(OperatorType.NotEqual, 6)]
        [TestCase(OperatorType.Contains, 7)]
        [TestCase(OperatorType.NotContains, 8)]
        [TestCase(OperatorType.Overlaps, 9)]
        [TestCase(OperatorType.NotOverlaps, 10)]
        [TestCase(OperatorType.ContainsKey, 11)]
        [TestCase(OperatorType.NotContainsKey, 12)]
        [TestCase(OperatorType.ContainsValue, 13)]
        [TestCase(OperatorType.NotContainsValue, 14)]
        [TestCase(OperatorType.KeyContainsValue, 15)]
        [TestCase(OperatorType.NotKeyContainsValue, 16)]
        [TestCase(OperatorType.IsContained, 17)]
        [TestCase(OperatorType.IsNotContained, 18)]
        [TestCase(OperatorType.InnerEqual, 19)]
        [TestCase(OperatorType.InnerGreaterThan, 20)]
        [TestCase(OperatorType.InnerGreaterThanOrEqual, 21)]
        [TestCase(OperatorType.InnerLessThan, 22)]
        [TestCase(OperatorType.InnerLessThanOrEqual, 23)]
        [TestCase(OperatorType.InnerNotEqual, 24)]
        [TestCase(OperatorType.InnerContains, 25)]
        [TestCase(OperatorType.InnerNotContains, 26)]
        [TestCase(OperatorType.InnerOverlaps, 27)]
        [TestCase(OperatorType.InnerNotOverlaps, 28)]
        public void ToString_ShouldReturnJsonSerialization(OperatorType type, int expected)
        {
            var rule = new Rule("property", type, "value");

            var result = rule.ToString();

            result
                .Replace("\n", "")
                .Replace("\r", "")
                .Should()
                .Be("{  \"Property\": \"property\",  \"Operator\": " + expected + ",  \"Value\": \"value\",  \"Description\": null,  \"Code\": null}");
        }
    }
}