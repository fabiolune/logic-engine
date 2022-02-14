# Logic Engine

![GitHub CI](https://github.com/fabiolune/logic-engine/actions/workflows/dotnet.yml/badge.svg)
[![codecov](https://codecov.io/gh/fabiolune/logic-engine/branch/main/graph/badge.svg?token=EYWA9ONWVX)](https://codecov.io/gh/fabiolune/logic-engine)

The __logic-engine__ is a simple dotnet library to help introducing flexible logic systems.

It supports a generic set of rules that get compiled into executable code, allowing the possibility to dynamically change your business logic and adapt it to different needs without changing the core of your system.

The core functionalities are encapsulated in different components, both logical and functional.

## The Rule
The rule object represents the building block for the system. A rule is an abstraction for a function acting on the value of a type and returning a boolean response.

Given a type to be applied to, a rule is defined by a set of fields
- `Property`: identifies the property against which to execute the evaluation
- `Operator`: defines the operation to execute on the property
- `Value`: identifies the value against which compare the result of the operator on the property
- `Code`: the error code to be generated when the rules applied on an object fails (returns `false`)
- `Description`: a generic description of the rule, without real behavior

## The Operator

The `Operator` can assume different possible values depending on the `Property` it is applied to and on the value the result should be compared to.

Based on this considerations there are different types of operators. The rules categorization is also influenced by some implementation details.

### Direct operators

These operators directly compare the `Property` to the `Value` considered as a constant:
- `Equal`: equality on value types (strings, numbers, ...)
- `NotEqual`: inequality on value types (strings, numbers, ...)
- `GreaterThan`: only applies to numbers
- `GreaterThanOrEqual`: only applies to numbers
- `LessThan`: only applies to numbers
- `LessThanOrEqual`: only applies to numbers

```cs
public class MyClass
{
    public string StringProperty {get; set;}
    public int IntegerProperty {get; set;}
}

var stringRule = new Rule("StringProperty", OperatorType.Equal, "Some Value");
var integerRule = new Rule("IntegerProperty", OperatorType.Equal, "10");
```
> sample rules with direct operators

### Internal direct operators

Internal direct rules are similar to direct rules, but they are meant to be applied on values that are other fields of the same type; in this case `Value` should correspond to the name of another field in the analyzed type:

- `InnerEqual`: equality between two value typed fields
- `InnerNotEqual`: equality between two value typed fields
- `InnerGreaterThan`: only applies when `Property` and `Value` are numbers
- `InnerGreaterThanOrEqual`: only applies when `Property` and `Value` are numbers
- `InnerLessThan`: only applies when `Property` and `Value` are numbers
- `InnerLessThanOrEqual`: only applies when `Property` and `Value` are numbers

```cs
public class MyClass
{
    public string StringProperty1 {get; set;}
    public string StringProperty2 {get; set;}
    public int IntegerProperty1 {get; set;}
    public int IntegerProperty2 {get; set;}
}

var stringRule = new Rule("StringProperty1", OperatorType.InnerEqual, "StringProperty2");
var integerRule = new Rule("IntegerProperty1", OperatorType.InnerGreaterThan, "IntegerProperty2");
```
> sample rules with internal direct operators

### Enumerable operators

These rules apply to operand ot generic enumerable type:

- `Contains`: checks that `Property` contains `Value`
- `NotContains`: checks that `Property` does not `Value`
- `Overlaps`: checks that `Property` has a non empty intersection with `Value`
- `NotOverlaps`: checks that `Property` has an empty intersection with `Value`

```cs
public class MyClass
{
    public IEnumerable<string> StringEnumerableProperty {get; set;}
}

var rule1 = new Rule("StringEnumerableProperty", OperatorType.Contains, "value");
var rule2 = new Rule("StringEnumerableProperty", OperatorType.Overlaps, "value1,value2");
```
> sample rules with enumerable operators

### Internal enumerable operators

These operators act on enumerable fields by comparing them against fields of the same type:

- `InnerContains`: 
- `InnerNotContains`: 
- `InnerOverlaps`: 
- `InnerNotOverlaps`: 

```cs
public class MyClass
{
    public IEnumerable<int> EnumerableProperty1 {get; set;}
    public IEnumerable<int> EnumerableProperty2 {get; set;}
    public int IntegerField {get; set;}
}

var rule1 = new Rule("EnumerableProperty1", OperatorType.InnerContains, "IntegerField");
var rule2 = new Rule("EnumerableProperty1", OperatorType.InnerOverlaps, "EnumerableProperty2");
```
> sample rules for internal enumerable operators

### Key-value operators

These operators act on dictionary-like objects:

- `ContainsKey`: checks that the `Property` contains the specfic key defined by the `Value`
- `NotContainsKey`: checks that the `Property` doesn't contain the specfic key defined by the `Value`
- `ContainsValue`:  checks that the dictionary `Property` contains a value defined by the `Value`
- `NotContainsValue`: checks that the dictionary `Property` doesn't contain a value defined by the `Value`
- `KeyContainsValue`: checks that the dictionary `Property` has a key with a specific value
- `NotKeyContainsValue`: checks that the dictionary `Property` doesn't have a key with a specific value

```cs
public class MyClass
{
    public IDictionary<string, int> DictProperty {get; set;}
}

var rule1 = new Rule("DictProperty", OperatorType.ContainsKey, "mykey");
var rule2 = new Rule("DictProperty", OperatorType.KeyContainsValue, "mykey[myvalue]");
```
> sample rules for key-value enumerable operators

### Inverse enumerable operators

These rules apply to scalars against enumerable fields:

- `IsContained`: checks that `Property` is contained in a specific set
- `IsNotContained`: checks that `Property` is not contained in a specific set

```cs
public class MyClass
{
    public int IntProperty {get; set;}
}

var rule1 = new Rule("IntProperty", OperatorType.IsContained, "1,2,3");
```
> sample rules for inverse enumerable operators

## RulesSet and RulesCatalog

A `RulesSet` is basically a set of rules. From a functional point of view it represents a boolean typed function composed by a set of functions on a given type.

---
__DEFINITION__

A `Rule` is satisfied by an item `t` of type `T` if the associated function `f: T ──► bool` returns true if `f(t)` is `true`.

---

---
__DEFINITION__

A `RulesSet` is satisfied by an item `t` of type `T` if and only if all the functions of the set are satisfied by `t`.

---

A `RulesCatalog` represents a set of `RulesSet`, and functionally corresponds to a boolean typed function composed by a set of sets of functions on a given type.

---
__DEFINITION__

A `RulesCatalog` is satisfied by an item `t` of type `T` if at least one of its `RulesSet` is satisfied by `t`.

---

From these considerations, it is clear that a `RulesSet` corresponds to the logical `AND` operator on its functions, and a `RulesCatalog` corresponds to the logical `OR` operator on its `RulesSet`.

## The Algebraic model

As discussed above, composite types `RulesSet` and `RulesCatalog` represent logical operations on the field of functions `f: T ──► bool`; it seems than possible to define an algebraic model where catalogs can be added or multiplied together to generate more complex catalogs.

The sum of two `RulesCatalog` objects is a `RulesCatalog` with a set of `RulesSet` obtained by simply concatenating the two sets of `RulesSet`:

```
c1 = {rs1, rs2, rs3}
c2 = {rs4, rs5}

──► c1 + c2 = {rs1, rs2, rs3, rs4, rs5}
```
> sum of two `RulesCatalog`

The product of two catalogs is a catalog with a set of all the `RulesSet` obtained concatenating a set of the first catalog with one of the second.

```
c1 = {rs1, rs2, rs3}
c2 = {rs4, rs5}

──► c1 + c2 = {(rs1+rs4), (rs1+rs5), (rs2+rs4), (rs2+rs5), (rs3+rs4), (rs3+rs5)}
```
> product of two `RulesCatalog`

## The RulesCompiler

The `RulesCompiler` is the component that transforms the formal business logic defined by a `RulesCatalog` into compiled code to be executed on item of a specific type: every rule becomes a function `Func<T, Either<string, Unit>>`, or, in analytic formalism, `f : T ──► Either<string, Unit>`.

The monadic notation captures the posible outputs of the function:
- the left type of the `Either` (`string`) represents a non matching result represented by the code of the executed rule
- the right type (`Unit`) represents insted a matching result for which no additional details are needed.

The functional programming notation is obtained thanks to Franco Melandri's [__Tiny FP__](https://github.com/FrancoMelandri/tiny-fp) library.

Extending the above concept of rule compilation, the `RulesCompiler` can convert a set of `Rules` into an `IEnumerable<Func<T, Either<string, Unit>>>`.

## The RulesManager

The `RulesManager<T>` is responsible for the application of the rules functions to an item of type `T`.

Given a `RulesCatalog` (provided using the `SetCatalog` method), the manager exposes two main functionalities:
- `ItemSatisfiesRules`: it just returns a true or false boolean value that represents if the catalog is satisfied by the item under consideration or not
- `ItemSatisfiesRulesWithMessage`: this method is similar to the previous and returns a Unit value if the item satisfies the catalog; if not a set of all the codes associated to rules not matched is returned[^1]

The main difference between the two method is the circuit breaking approach used in `ItemSatisfiesRules`: since a catalog is an `OR` combination of `AND` combined rules, the it is satisfied if at least one `RulesSet` is. This effectively means that as soon as a `RulesSet` returns a success, no other sets will be evaluated[^2].

Given the above nature of the catalog, an item does not satisfy it if at least one rule in every set is not satisfied, hence to make sure all the possible codes are collected, a full scan of all the rules is needed in the `ItemSatisfiesRulesWithMessage` method.

Additional methods of the manager allow to operate on an `IEnumerable<T>`, in particular it is possible to:
- apply a filter based on the catalog
- extract the first item satisfying the catalog

## How to install

If you are using `nuget.org` you can add the dependency in your project using
```shell
dotnet add package logic-engine --version 1.0.0
```

To install the __logic-engine__ library from GitHub's packages system please refer to the [packages page](https://github.com/fabiolune?tab=packages&repo_name=logic-engine).

[^1]: null or empty codes are removed because they don't carry reusable info
[^2]: from a technical perspective this is obtained with a concrete implementation of the railway pattern