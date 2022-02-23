using System.Runtime.Serialization;
using LogicEngine.Internals;
using Newtonsoft.Json;

namespace LogicEngine.Models;

public record Rule
{
    [JsonConstructor]
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
    public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
}