using LogicEngine.Internals;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace LogicEngine.Models;

public record Rule
{
    private string _code;

    public Rule(string property, OperatorType type, string value, string code)
    {
        Property = property;
        Operator = type;
        Value = value;
        Code = code;
    }

    /// <summary>
    /// Name of the property the rule is applied to
    /// </summary>
    [DataMember(Name = "property")]
    [Required]
    public string Property { get; init; }
    
    /// <summary>
    /// Operator to apply to the property
    /// </summary>
    [DataMember(Name = "operator")]
    [Required]
    public OperatorType Operator { get; init; }
    
    /// <summary>
    /// Value to compare the property to or name of the property to compare with <see cref="Property"/>
    /// </summary>
    [DataMember(Name = "value")]
    [Required]
    public string Value { get; init; }
    
    /// <summary>
    /// Code to return if the rule is not satisfied
    /// </summary>
    [DataMember(Name = "code")]
    [Required]
    public string Code { get => _code; init => _code = value ?? string.Empty; }
    public override string ToString() =>
        $"[Rule (property: {Property}, operator: {Operator:G}, value: {Value}, code: {Code})]";
}