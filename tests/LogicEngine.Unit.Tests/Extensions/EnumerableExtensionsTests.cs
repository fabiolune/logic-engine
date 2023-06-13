﻿using LogicEngine.Extensions;
using LogicEngine.Interfaces;
using System;

namespace LogicEngine.Unit.Tests.Extensions;

public class EnumerableExtensionsTests
{
    private static readonly TestModel Item1 = new() { IntProperty = 1 };
    private static readonly TestModel Item2 = new() { IntProperty = 2 };

    internal static object[] FilterTestCases =
    {
        new object[]
        {
            true,
            true,
            new[]{ Item1, Item2 }
        },
        new object[]
        {
            true,
            false,
            new[]{ Item1 }
        },
        new object[]
        {
            false,
            true,
            new[]{ Item2 }
        },
        new object[]
        {
            false,
            false,
            Array.Empty<TestModel>()
        }
    };

    internal static object[] FirstOrDefaultTestCases =
    {
        new object[]
        {
            true,
            true,
            Item1
        },
        new object[]
        {
            true,
            false,
            Item1
        },
        new object[]
        {
            false,
            true,
            Item2
        },
        new object[]
        {
            false,
            false,
            default(TestModel)
        }
    };

    [TestCaseSource(nameof(FirstOrDefaultTestCases))]
    public void Filter_ShouldReturnItemsForWhichApplyIsTrue(bool apply1, bool apply2, TestModel expected)
    {
        var mockApplyable = new Mock<IApplyable<TestModel>>();

        var data = new[] { Item1, Item2};

        mockApplyable
            .Setup(a => a.Apply(Item1))
            .Returns(apply1);

        mockApplyable
            .Setup(a => a.Apply(Item2))
            .Returns(apply2);
        
        data.FirstOrDefault(mockApplyable.Object)
            .Should()
            .BeEquivalentTo(expected);
    }

    [TestCaseSource(nameof(FilterTestCases))]
    public void Filter_ShouldReturnItemsForWhichApplyIsTrue(bool apply1, bool apply2, TestModel[] expected)
    {
        var mockApplyable = new Mock<IApplyable<TestModel>>();

        var data = new[] { Item1, Item2 };

        mockApplyable
            .Setup(a => a.Apply(Item1))
            .Returns(apply1);

        mockApplyable
            .Setup(a => a.Apply(Item2))
            .Returns(apply2);

        data.Filter(mockApplyable.Object)
            .Should()
            .BeEquivalentTo(expected);
    }
}