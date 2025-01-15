using LogicEngine.Compilers;
using LogicEngine.Internals;
using LogicEngine.Models;
using System.Collections.Generic;
using TinyFp.Extensions;

namespace LogicEngine.Unit.Tests.Compilers;

public class RuleCompilerTests
{
    private RuleCompiler _sut;

    [SetUp]
    public void SetUp() => _sut = new RuleCompiler();

    [TestCase(OperatorType.Equal)]
    [TestCase(OperatorType.StringStartsWith)]
    [TestCase(OperatorType.StringEndsWith)]
    [TestCase(OperatorType.StringContains)]
    [TestCase(OperatorType.StringEndsWith)]
    [TestCase(OperatorType.StringRegexIsMatch)]
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

        result.IsNone.ShouldBeTrue();
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

        result.IsSome.ShouldBeTrue();
    }

    [TestCase(OperatorType.StringStartsWith)]
    [TestCase(OperatorType.StringEndsWith)]
    [TestCase(OperatorType.StringContains)]
    [TestCase(OperatorType.StringRegexIsMatch)]
    public void Compile_WhenMethodRulesAreCorrectWithStrings_ShouldReturnSome(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.StringProperty), op, "3", null);

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
    }

    [TestCase(OperatorType.StringStartsWith)]
    [TestCase(OperatorType.StringEndsWith)]
    [TestCase(OperatorType.StringContains)]
    [TestCase(OperatorType.StringRegexIsMatch)]
    public void Compile_WhenMethodRulesAreNotCorrectWithStrings_ShouldReturnSome(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.IntProperty), op, "3", null);

        var result = _sut.Compile<TestModel>(rule);

        result.IsNone.ShouldBeTrue();
    }

    [TestCase(OperatorType.Equal)]
    [TestCase(OperatorType.NotEqual)]
    public void Compile_WhenDirectRulesAreCorrectWithEnums_ShouldReturnSome(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.EnumProperty), op, "One", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
    }

    [TestCase(OperatorType.Contains)]
    [TestCase(OperatorType.NotContains)]
    public void Compile_WhenEnumerableRulesAreCorrect_ShouldReturnSome(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), op, "3", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
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

        result.IsSome.ShouldBeTrue();
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

        result.IsNone.ShouldBeTrue();
    }

    [TestCase(OperatorType.InnerContains, nameof(TestModel.IntEnumerableProperty), nameof(TestModel.IntProperty))]
    [TestCase(OperatorType.InnerNotContains, nameof(TestModel.IntEnumerableProperty), nameof(TestModel.IntProperty))]
    [TestCase(OperatorType.InnerContains, nameof(TestModel.StringArrayProperty), nameof(TestModel.StringProperty))]
    [TestCase(OperatorType.InnerNotContains, nameof(TestModel.StringArrayProperty), nameof(TestModel.StringProperty))]
    [TestCase(OperatorType.InnerOverlaps, nameof(TestModel.IntEnumerableProperty), nameof(TestModel.IntEnumerableProperty2))]
    [TestCase(OperatorType.InnerNotOverlaps, nameof(TestModel.IntEnumerableProperty), nameof(TestModel.IntEnumerableProperty2))]
    [TestCase(OperatorType.InnerOverlaps, nameof(TestModel.StringArrayProperty), nameof(TestModel.StringArrayProperty2))]
    [TestCase(OperatorType.InnerNotOverlaps, nameof(TestModel.StringArrayProperty), nameof(TestModel.StringArrayProperty2))]
    public void Compile_WhenRulesWithInternalEnumerableOperatorAreCorrect_ShouldReturnSome(OperatorType op, string property, string value)
    {
        var rule = new Rule(property, op, value, "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
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

        result.IsSome.ShouldBe(expectedSome);
    }

    [TestCase(OperatorType.ContainsKey)]
    [TestCase(OperatorType.NotContainsKey)]
    [TestCase(OperatorType.ContainsValue)]
    [TestCase(OperatorType.NotContainsValue)]
    public void When_CompileRulesWithGenericKeyValueOperator_ShouldCompileRules(OperatorType type)
    {
        var rule = new Rule(nameof(TestModel.StringStringDictionaryProperty), type, "some", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
    }

    [TestCase(OperatorType.KeyContainsValue)]
    [TestCase(OperatorType.NotKeyContainsValue)]
    public void When_CompileRulesWithKeyAndValueOperator_ShouldCompileRules(OperatorType type)
    {
        var rule = new Rule("StringStringDictionaryProperty[key]", type, "value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
    }

    [TestCase(OperatorType.InnerContains)]
    [TestCase(OperatorType.InnerNotContains)]
    [TestCase(OperatorType.InnerOverlaps)]
    [TestCase(OperatorType.InnerNotOverlaps)]
    public void When_CompileRulesWithNotMatchingTypes_ShouldNotCompileRules(OperatorType type)
    {
        var rule = new Rule("StringEnumerableProperty", type, "IntProperty", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsNone.ShouldBeTrue();
    }

    [TestCase(OperatorType.IsContained)]
    [TestCase(OperatorType.IsNotContained)]
    public void Compile_WhenInverseEnumerableRulesAreCorrect_ShouldReturnSome(OperatorType op)
    {
        var rule = new Rule(nameof(TestModel.StringProperty), op, nameof(TestModel.StringArrayProperty), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
    }

    [Test]
    public void When_CompileRulesWithNotExistingOperatorType_ShouldNotCompileRules()
    {
        var rule = new Rule(nameof(TestModel.StringEnumerableProperty), (OperatorType)1000, "StringProperty", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsNone.ShouldBeTrue();
    }

    [Test]
    public void When_RuleIsForArrays_ShouldCompileRule()
    {
        var rule = new Rule(nameof(TestModel.StringArrayProperty), OperatorType.Contains, "value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
    }

    [Test]
    public void Compile_WhenRuleIsEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.Equal, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 13
            }).IsLeft.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.NotEqual, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12
            }).IsLeft.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 13
            }).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsGreaterThan_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.GreaterThan, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12
            }).IsLeft.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 13
            }).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsGreaterThanOrEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.GreaterThanOrEqual, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 13
            }).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsLessThan_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.LessThan, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntProperty = 11
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 13
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsLessThanOrEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.LessThanOrEqual, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 11
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 13
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleStringStartsWith_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringProperty), OperatorType.StringStartsWith, "StringCased_", "string does not start with StringCased_");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            var expectedLeft1 = cr.DetailedApply(new TestModel
            {
                StringProperty = "shouldbeleftStringCased_"
            });
            expectedLeft1.IsLeft.ShouldBeTrue();
            expectedLeft1.UnwrapLeft().ShouldBe("string does not start with StringCased_");
            var expectedLeft2 = cr.DetailedApply(new TestModel
            {
                StringProperty = null
            });
            expectedLeft2.IsLeft.ShouldBeTrue();
            expectedLeft2.UnwrapLeft().ShouldBe("string does not start with StringCased_");

            cr.DetailedApply(new TestModel
            {
                StringProperty = "StringCased_teststring"
            }).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleStringEndsWith_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringProperty), OperatorType.StringEndsWith, "_StringCased", "string does not end with _StringCased");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            var expectedLeft1 = cr.DetailedApply(new TestModel
            {
                StringProperty = "should_StringCasedbeleft"
            });
            expectedLeft1.IsLeft.ShouldBeTrue();
            expectedLeft1.UnwrapLeft().ShouldBe("string does not end with _StringCased");
            var expectedLeft2 = cr.DetailedApply(new TestModel
            {
                StringProperty = null
            });
            expectedLeft2.IsLeft.ShouldBeTrue();
            expectedLeft2.UnwrapLeft().ShouldBe("string does not end with _StringCased");

            cr.DetailedApply(new TestModel
            {
                StringProperty = "teststring_StringCased"
            }).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleStringContains_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringProperty), OperatorType.StringContains, "_StringCased_", "string does not contain _StringCased_");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            var expectedLeft1 = cr.DetailedApply(new TestModel
            {
                StringProperty = "StringCase_should_StringCasedbeleft_StringCased"
            });
            expectedLeft1.IsLeft.ShouldBeTrue();
            expectedLeft1.UnwrapLeft().ShouldBe("string does not contain _StringCased_");
            var expectedLeft2 = cr.DetailedApply(new TestModel
            {
                StringProperty = null
            });
            expectedLeft2.IsLeft.ShouldBeTrue();
            expectedLeft2.UnwrapLeft().ShouldBe("string does not contain _StringCased_");

            cr.DetailedApply(new TestModel
            {
                StringProperty = "StringCased_should_StringCased_beleft_StringCased"
            }).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleStringRegexIsMatch_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringProperty), OperatorType.StringRegexIsMatch, "(?i)(\\W|^)(baloney|darn)(\\W|$)", "string does not match regex (?i)(\\W|^)(baloney|darn)(\\W|$)");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            var expectedLeft1 = cr.DetailedApply(new TestModel
            {
                StringProperty = "feiwufuih sfojiwoej pwjiejfpo kjkjkkkk ejwijfdarn fjeqijfp o"
            });
            expectedLeft1.IsLeft.ShouldBeTrue();
            expectedLeft1.UnwrapLeft().ShouldBe("string does not match regex (?i)(\\W|^)(baloney|darn)(\\W|$)");
            var expectedLeft2 = cr.DetailedApply(new TestModel
            {
                StringProperty = null
            });
            expectedLeft2.IsLeft.ShouldBeTrue();
            expectedLeft2.UnwrapLeft().ShouldBe("string does not match regex (?i)(\\W|^)(baloney|darn)(\\W|$)");

            cr.DetailedApply(new TestModel
            {
                StringProperty = "feiwufuih sfojiwoej pwjiejfpo kjkjkkkk ejwijf darn fjeqijfp o"
            }).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsContains_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Contains, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 12, 13]
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 13]
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotContains_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.NotContains, "12", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 12, 13]
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 13]
            }).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsOverlaps_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.Overlaps, "12,13", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 12, 13]
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 13]
            }).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotOverlaps_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.NotOverlaps, "12,13", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 12, 13]
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 13]
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsContainsKey_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringStringDictionaryProperty), OperatorType.ContainsKey, "my_key", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_key", "my_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_value"},
                    {"another_key", "another_value"},
                }
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel())
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotContainsKey_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringStringDictionaryProperty), OperatorType.NotContainsKey, "my_key", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_key", "my_value"},
                    {"another_key", "another_value"},
                }
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel()).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsContainsValue_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringStringDictionaryProperty), OperatorType.ContainsValue, "my_value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_key", "my_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_wrong_value"},
                    {"another_key", "another_value"},
                }
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel())
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotContainsValue_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.StringStringDictionaryProperty), OperatorType.NotContainsValue, "my_value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    { "my_key", "my_value" },
                    { "another_key", "another_value" },
                }
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_wrong_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel()).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsKeyContainsValue_ShouldReturnExpectedFunction()
    {
        var rule = new Rule($"{nameof(TestModel.StringStringDictionaryProperty)}[my_key]", OperatorType.KeyContainsValue, "my_value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    { "my_key", "my_value" },
                    { "another_key", "another_value" },
                }
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    { "my_key", "my_wrong_value" },
                    { "another_key", "another_value" },
                }
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_wrong_value"},
                    {"another_key", "another_value"},
                }
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel())
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsNotKeyContainsValue_ShouldReturnExpectedFunction()
    {
        var rule = new Rule($"{nameof(TestModel.StringStringDictionaryProperty)}[my_key]", OperatorType.NotKeyContainsValue, "my_value", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    { "my_key", "my_value" },
                    { "another_key", "another_value" },
                }
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    { "my_key", "my_wrong_value" },
                    { "another_key", "another_value" },
                }
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                StringStringDictionaryProperty = new Dictionary<string, string>
                {
                    {"my_wrong_key", "my_wrong_value"},
                    {"another_key", "another_value"},
                }
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel()).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsIsContained_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.IsContained, "12,13", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel())
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsIsNotContained_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.IsNotContained, "12,13", "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel()).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerEqualOnSameProperty_ShouldAlwaysReturnRight()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerEqual, nameof(TestModel.IntProperty), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel()).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerEqual, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14,
                IntProperty2 = 13
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel()).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerNotEqualOnSameProperty_ShouldAlwaysReturnLeft()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerNotEqual, nameof(TestModel.IntProperty), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel())
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerNotEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerNotEqual, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14,
                IntProperty2 = 13
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel())
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerGreaterThan_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerGreaterThan, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14,
                IntProperty2 = 13
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel())
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerGreaterThanOrEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerGreaterThanOrEqual, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14,
                IntProperty2 = 13
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel()).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerLessThan_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerLessThan, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntProperty = 13,
                IntProperty2 = 14
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel())
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerLessThanOrEqual_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntProperty), OperatorType.InnerLessThanOrEqual, nameof(TestModel.IntProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12,
                IntProperty2 = 12
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 13,
                IntProperty2 = 14
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel()).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerContains_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerContains, nameof(TestModel.IntProperty), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12,
                IntEnumerableProperty = [11, 12]
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 13,
                IntEnumerableProperty = [11, 12]
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel())
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerNotContains_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerNotContains, nameof(TestModel.IntProperty), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntProperty = 12,
                IntEnumerableProperty = [11, 12]
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntProperty = 13,
                IntEnumerableProperty = [11, 12]
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntProperty = 14
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel()).IsRight.ShouldBeTrue();
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerOverlaps_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerOverlaps, nameof(TestModel.IntEnumerableProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 12],
                IntEnumerableProperty2 = [11, 13]
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 12],
                IntEnumerableProperty2 = [13, 14]
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 12],
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel())
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });
        });
    }

    [Test]
    public void Compile_WhenRuleIsInnerNotOverlaps_ShouldReturnExpectedFunction()
    {
        var rule = new Rule(nameof(TestModel.IntEnumerableProperty), OperatorType.InnerNotOverlaps, nameof(TestModel.IntEnumerableProperty2), "code");

        var result = _sut.Compile<TestModel>(rule);

        result.IsSome.ShouldBeTrue();
        result.OnSome(cr =>
        {
            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 12],
                IntEnumerableProperty2 = [11, 13]
            })
            .Do(e =>
            {
                e.IsLeft.ShouldBeTrue();
                e.UnwrapLeft().ShouldBe("code");
            });

            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 12],
                IntEnumerableProperty2 = [13, 14]
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel
            {
                IntEnumerableProperty = [11, 12],
            }).IsRight.ShouldBeTrue();

            cr.DetailedApply(new TestModel()).IsRight.ShouldBeTrue();
        });
    }
}