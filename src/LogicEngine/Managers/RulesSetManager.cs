//using System;
//using System.Collections.Generic;
//using System.Linq;
//using LogicEngine.Interfaces.Compilers;
//using LogicEngine.Interfaces.Managers;
//using LogicEngine.Models;
//using TinyFp;
//using TinyFp.Extensions;

//namespace LogicEngine.Managers;

//public class RulesSetManager<T> : IRulesSetManager<T> where T : new()
//{
//    private static readonly Func<T, Option<string>> AlwaysNoneFirstMatching = _ => Option<string>.None();
//    private static readonly Func<T, bool> AlwaysFalseIsSatisfied = _ => false;

//    private Func<T, bool> _isSatisfied = AlwaysFalseIsSatisfied;
//    private Func<T, Option<string>> _firstMatching = AlwaysNoneFirstMatching;

//    private readonly IRulesSetCompiler _compiler;

//    public RulesSet Set
//    {
//        set => (_firstMatching, _isSatisfied) =
//            value
//                .Map(_compiler.CompileLabeled<T>)
//                .Executables
//                .Map(e => (FirstMatchingWithRules(e), IsSatisfiedWithRules(e)))
//                .OrElse(() => (AlwaysNoneFirstMatching, AlwaysFalseIsSatisfied));
//    }

//    public RulesSetManager(IRulesSetCompiler compiler) => _compiler = compiler;

//    /// <summary>
//    /// Given a set of rules, returns the code of the first one that is matched by "item"
//    /// </summary>
//    /// <param name="item"></param>
//    /// <returns><![CDATA[Option<string>]]></returns>
//    public Option<string> FirstMatching(T item) => _firstMatching(item);

//    private static Func<T, Option<string>> FirstMatchingWithRules(IList<KeyValuePair<string, Func<T, Either<string, Unit>>>> rules) =>
//        item =>
//            rules
//                .Select(x => (x.Key, x.Value(item)))
//                .FirstOrDefault(x => x.Item2.IsRight)
//                .ToOption(x => x.Equals(default))
//                .Map(_ => _.Key);

//    private static Func<T, bool> IsSatisfiedWithRules(IList<KeyValuePair<string, Func<T, Either<string, Unit>>>> rules) =>
//        item => //!rules.TakeWhile(f => f.Value(item).IsLeft).Any();
//            rules.All(f => f.Value(item).IsRight);

//    public bool ItemSatisfiesRules(T item) => _isSatisfied(item);

//    public Either<string[], Unit> ItemSatisfiesRulesWithMessage(T item)
//    {
//        throw new NotImplementedException();
//    }
//}