using System.Diagnostics.CodeAnalysis;
using LogicEngine.Internals;
using LogicEngine.Models;

namespace Previous;

[ExcludeFromCodeCoverage]
internal class Data
{
    internal struct TestModel
    {
        public string StringProperty { get; set; }
    }
    internal static RulesCatalog ShortCircuitCatalog =>
        new()
        {
            RulesSets = new List<RulesSet>
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
            }
        };

    internal static RulesCatalog FullExecutingCatalog =>
        new()
        {
            RulesSets = new List<RulesSet>
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
            }
        };

}