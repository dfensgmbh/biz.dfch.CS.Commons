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

using System.ComponentModel;
using BenchmarkDotNet.Attributes;

namespace biz.dfch.CS.Commons.Benchmarks
{
    public class BaseDtoBenchmark
    {
        public class ArbitraryBaseDto : BaseDto
        {
            public string StringProperty { get; set; }

            public long LongProperty { get; set; }
        }

        public class ArbitraryBaseDtoWithDefaultValue : BaseDto
        {
            [DefaultValue("arbitrary string")]
            public string StringProperty { get; set; }

            [DefaultValue(42L)]
            public long LongProperty { get; set; }
        }

        public class ArbitraryClass
        {
            public string StringProperty { get; set; }

            public long LongProperty { get; set; }
        }

        public class ArbitraryClassWithDefaultValue
        {
            [DefaultValue("arbitrary string")]
            public string StringProperty { get; set; }

            [DefaultValue(42L)]
            public long LongProperty { get; set; }
        }

        [Benchmark]
        public static void BaseDto()
        {
            var sut = new ArbitraryBaseDto();
        }

        [Benchmark]
        public static void BaseDtoWithDefaultValue()
        {
            var sut = new ArbitraryBaseDtoWithDefaultValue();
        }

        [Benchmark]
        public static void Class()
        {
            var sut = new ArbitraryClass();
        }

        [Benchmark]
        public static void ClassWithDefaultValue()
        {
            var sut = new ArbitraryClassWithDefaultValue();
        }
    }
}
