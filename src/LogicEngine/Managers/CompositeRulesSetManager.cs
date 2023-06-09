//using LogicEngine.Interfaces.Compilers;
//using LogicEngine.Interfaces.Managers;
//using LogicEngine.Models;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using TinyFp;
//using TinyFp.Extensions;

//namespace LogicEngine.Managers;

//public class CompositeRulesSetManager<T, TKey> : ICompositeRulesSetManager<T, TKey> where T : new()
//{
//    private readonly IRulesSetCompiler _compiler;
//    private Func<T, Option<TKey>> _func = _ => Option<TKey>.None();

//    public CompositeRulesSetManager(IRulesSetCompiler compiler) => _compiler = compiler;

//    public IEnumerable<(TKey, RulesSet)> Set
//    {
//        set => _func =
//            value
//                .ToOption(v => !v.Any())
//                .Map(v => v.Where(kv => kv.Item1 is not null))
//                .Map<Func<T, Option<TKey>>>(v => (item) => v.Select(t => (t.Item1, _compiler.Compile<T>(t.Item2)))
//                .Select(t => (t.Item1, t.Item2._executables))
//                .FirstOrDefault(t => t.Executables.All(f => f(item).IsRight))
//                .ToOption(t => t.Item1 is null)
//                .Map(t => t.Item1))
//                .OrElse(() => _ => Option<TKey>.None());
//    }

//    //public IEnumerable<(TKey, RulesSet)> Set2
//    //{
//    //    set => _func =
//    //        value
//    //            .ToOption(v => !v.Any())
//    //            .Map(e => e.Select(t => (t.Item1, new RulesSetManager<T>(_compiler) { Set = t.Item2 })))
//    //            .Map(e => FirstMatchingWithRules(e))

//    //            //.Map(v => v.Where(kv => kv.Item1 is not null))
//    //            //.Map<Func<T, Option<TKey>>>(v => (item) => v.Select(t => (t.Item1, _compiler.Compile<T>(t.Item2)))
//    //            //.Select(t => (t.Item1, t.Item2.Executables))
//    //            //.FirstOrDefault(t => t.Executables.All(f => f(item).IsRight))
//    //            //.ToOption(t => t.Item1 is null)
//    //            //.Map(t => t.Item1))
//    //            .OrElse(() => _ => Option<TKey>.None());
//    //}

//    /// <summary>
//    /// Given a set of rulesets and corresponding TKey keys, returns the TKey corresponding to the first ruleset matched by "item"
//    /// </summary>
//    /// <param name="item"></param>
//    /// <returns><![CDATA[Option<TKey>]]></returns>
//    public Option<TKey> FirstMatching(T item) => _func(item);

//    private static Func<T, Option<TKey>> FirstMatchingWithRules(IEnumerable<(TKey, RulesManager<T>)> data) =>
//        item => Option<TKey>.None();
//}
