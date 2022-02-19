using System.Diagnostics.CodeAnalysis;
using LogicEngine.Internals;
using LogicEngine.Models;

namespace Current;

[ExcludeFromCodeCoverage]
internal class Data
{
    internal struct TestModel
    {
        public string StringProperty { get; set; }
    }
    internal static RulesCatalog ShortCircuitCatalog =>
        new(new List<RulesSet>
            {
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct")
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct"),
                    }
                }
            }, string.Empty);

    internal static RulesCatalog FullExecutingCatalog =>
        new(new List<RulesSet>
            {
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct")
                    }
                }
            }, string.Empty);

}