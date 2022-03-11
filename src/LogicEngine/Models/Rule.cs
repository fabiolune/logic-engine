using System.Runtime.Serialization;
using LogicEngine.Internals;

namespace LogicEngine.Models;

public record Rule
{
    public Rule(string property, OperatorType @operator, string value, string code)
    {
        Property = property;
        Operator = @operator;
        Value = value;
        Code = code;
    }

    [DataMember(Name = "property")]
    public string Property { get; init; }
    [DataMember(Name = "operator")]
    public OperatorType Operator { get; init; }
    [DataMember(Name = "value")]
    public string Value { get; init; }
    [DataMember(Name = "code")]
    public string Code { get; init; }
    public override string ToString() => 
        $"[Rule (property: {Property}, operator: {Operator:G}, value: {Value}, code: {Code})]";
}