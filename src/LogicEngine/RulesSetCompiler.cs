using System;
using System.Collections.Generic;
using System.Linq;
using LogicEngine.Interfaces;
using LogicEngine.Models;
using TinyFp;
using static TinyFp.Prelude;
using TinyFp.Extensions;

namespace LogicEngine;

public class RulesSetCompiler : IRulesSetCompiler
{
    private readonly ISingleRuleCompiler _singleRuleCompiler;

    public RulesSetCompiler(ISingleRuleCompiler singleRuleCompiler) => _singleRuleCompiler = singleRuleCompiler;

    public CompiledRulesSet<T> Compile<T>(RulesSet set) =>
        set
            .Rules
            .AsParallel()
            .Select(_singleRuleCompiler.Compile<T>)
            .Where(_ => _.IsSome)
            .Select(_ => _.Unwrap())
            .Select(_ => _.Executable)
            .ToArray()
            .Map(_ => new CompiledRulesSet<T>(_));

    public CompiledLabeledRulesSet<T> CompileLabeled<T>(RulesSet set) =>
        Try(() => set
                .Rules
                .AsParallel()
                .ToDictionary(_ => _.Code ?? string.Empty, _singleRuleCompiler.Compile<T>)
                .Where(_ => _.Value.IsSome)
                .Select(x =>
                    new KeyValuePair<string, Func<T, Either<string, Unit>>>(x.Key, x.Value.Unwrap().Executable))
                .ToArray()
                .ToOption(_ => _.Length == 0)
            )
            .Match(_ => _, _ => Option<KeyValuePair<string, Func<T, Either<string, Unit>>>[]>.None())
            .Map(_ => new CompiledLabeledRulesSet<T>(_));
}