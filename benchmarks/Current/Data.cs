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
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct", "code")
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct", "code"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "code"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct", "code"),
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
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_0"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_1"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_2"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_3"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_4"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_5"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_6"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_7"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_8"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_9"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_a"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_b"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_c"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_d"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_e"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_f"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_g"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_h"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_i"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_l"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","1_code_m"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_0"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_1"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_2"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_3"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_4"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_5"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_6"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_7"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_8"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_9"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_a"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_b"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_c"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_d"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_e"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_f"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_g"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_h"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_i"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_l"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong","2_code_m"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct", "code"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_0"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_1"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_2"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_3"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_4"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_5"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_6"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_7"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_8"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_9"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_a"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_b"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_c"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_d"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_e"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_f"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_g"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_h"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_i"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_l"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "3_code_m"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_0"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_1"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_2"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_3"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_4"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_5"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_6"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_7"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_8"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_9"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_a"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_b"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_c"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_d"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_e"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_f"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_g"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_h"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_i"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_l"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "4_code_m"),
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "correct", "code"),
                    }
                },
                new()
                {
                    Rules = new List<Rule>
                    {
                        new(nameof(TestModel.StringProperty), OperatorType.Equal, "wrong", "5_code_0")
                    }
                }
            }, string.Empty);

}