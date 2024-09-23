using LogicEngine.Internals;
using LogicEngine.Models;

namespace LogicEngine.Unit.Tests;

[TestFixture]
public class RuleSetCompositionTests
{
    [Test]
    public void RuleSetProduct_ShouldReturn_ProperSum()
    {
        var rule1 = new Rule("a", OperatorType.Equal, "b", "code1");
        var rule2 = new Rule("a", OperatorType.Equal, "b", "code1");

        var rule3 = new Rule("a", OperatorType.Equal, "b", "code3");
        var rule4 = new Rule("a", OperatorType.Equal, "b", "code4");

        var set1 = new RulesSet([rule1, rule2], "set 1");
        var set2 = new RulesSet([rule3, rule4], "set 2");

        var prodSet = set1 * set2;

        prodSet.Rules.Should().BeEquivalentTo(new[] { rule1, rule2, rule3, rule4 });
        prodSet.Name.Should().Be("(set 1 AND set 2)");
    }
}