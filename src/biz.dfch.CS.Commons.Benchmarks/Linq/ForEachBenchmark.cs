/**
 * Copyright 2017 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Linq;
using BenchmarkDotNet.Attributes;

namespace biz.dfch.CS.Commons.Benchmarks.Linq
{
    public class ForEachBenchmark
    {
        [Benchmark]
        public static void ForEachViaToList()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Data._enumerableList.ForEachViaToList(e => string.IsNullOrEmpty(e));
        }

        [Benchmark]
        public static void ForEachViaAny()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Data._enumerableList.ForEachViaAny(e => string.IsNullOrEmpty(e));
        }

        [Benchmark]
        public static void ForEachViaAll()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Data._enumerableList.ForEachViaAll(e => string.IsNullOrEmpty(e));
        }

        [Benchmark]
        public static void ForEachViaParallel()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Data._enumerableList.ForEachViaParallel(e => string.IsNullOrEmpty(e));
        }

        [Benchmark]
        public static void ForEachManual()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Data._enumerableList.ToList().ForEach(e => string.IsNullOrEmpty(e));
        }

        [Benchmark]
        public static void ForEachViaFor()
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            foreach (var item in Data._enumerableList)
            {
                string.IsNullOrEmpty(item);
            }
        }
    }
}
