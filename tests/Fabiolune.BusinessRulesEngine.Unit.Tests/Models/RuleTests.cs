using Fabiolune.BusinessRulesEngine.Internals;
using Fabiolune.BusinessRulesEngine.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Fabiolune.BusinessRulesEngine.Unit.Tests.Models
{
    public class RuleTests
    {
        [Test]
        public void ToString_ShouldReturnJsonSerialization()
        {
            var rule = new Rule("property", OperatorType.Contains, "value");

            var result = rule.ToString();

            result
                .Replace("\n", "")
                .Replace("\r", "")
                .Should()
                .Be("{  \"Property\": \"property\",  \"Operator\": 7,  \"Value\": \"value\",  \"Description\": null,  \"Code\": null}");
        }
    }
}