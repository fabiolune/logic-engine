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
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct"){Code = "code"}
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct"){Code = "code"},
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct"){Code = "code"},
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
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct"){Code = "code"},
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"},
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct"){Code = "code"},
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong"){Code = "code"}
                    }
                }
            }, string.Empty);

}