using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using biz.dfch.CS.Commons.Linq;

namespace biz.dfch.CS.Commons.Benchmarks.Linq
{
    public static class EnumerableExtensions
    {
        public static void ForEachViaToList<TSource>(this IEnumerable<TSource> source, Action<TSource> predicate)
        {
            Contract.Requires(null != source);
            Contract.Requires(null != predicate);

            source.ToList().ForEach(predicate);
        }

        public static void ForEachViaAny<TSource>(this IEnumerable<TSource> source, Action<TSource> predicate)
        {
            Contract.Requires(null != source);
            Contract.Requires(null != predicate);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            source.Any(e =>
            {
                predicate(e);
                return false;
            });
        }

        public static void ForEachViaAll<TSource>(this IEnumerable<TSource> source, Action<TSource> predicate)
        {
            Contract.Requires(null != source);
            Contract.Requires(null != predicate);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            source.All(e =>
            {
                predicate(e);
                return true;
            });
        }

        public static void ForEachViaParallel<TSource>(this IEnumerable<TSource> source, Action<TSource> predicate)
        {
            Contract.Requires(null != source);
            Contract.Requires(null != predicate);

            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            source.AsParallel().ForEach(predicate);
        }
    }
}
