/**
 * Copyright 2016 d-fens GmbH
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

namespace biz.dfch.CS.Commons
{
    public static partial class Constants
    {
        public static partial class Validation
        {
            public static class Iso8601
            {
                public static readonly string[] DateTimeFormats =
                {
                    // with dash, space and colon
                    "yyyy-MM-dd HH:mm:sszzz",
                    "yyyy-MM-dd HH:mm:sszz",
                    "yyyy-MM-dd HH:mm:ssz",
                    "yyyy-MM-dd HH:mm:ss",
                    "yyyy-MM-dd HH:mm:ssZ",

                    "yyyy-MM-dd HH:mmzzz",
                    "yyyy-MM-dd HH:mmzz",
                    "yyyy-MM-dd HH:mmz",
                    "yyyy-MM-dd HH:mm",
                    "yyyy-MM-dd HH:mmZ",

                    "yyyy-MM-dd HHzzz",
                    "yyyy-MM-dd HHzz",
                    "yyyy-MM-dd HHz",
                    "yyyy-MM-dd HH",
                    "yyyy-MM-dd HHZ",

                    "yyyy-MM-ddzzz",
                    "yyyy-MM-ddzz",
                    "yyyy-MM-ddz",
                    "yyyy-MM-dd",
                    "yyyy-MM-ddZ",

                    // with dash, T and colon
                    "yyyy-MM-ddTHH:mm:sszzz",
                    "yyyy-MM-ddTHH:mm:sszz",
                    "yyyy-MM-ddTHH:mm:ssz",
                    "yyyy-MM-ddTHH:mm:ss",
                    "yyyy-MM-ddTHH:mm:ssZ",

                    "yyyy-MM-ddTHH:mmzzz",
                    "yyyy-MM-ddTHH:mmzz",
                    "yyyy-MM-ddTHH:mmz",
                    "yyyy-MM-ddTHH:mm",
                    "yyyy-MM-ddTHH:mmZ",

                    "yyyy-MM-ddTHHzzz",
                    "yyyy-MM-ddTHHzz",
                    "yyyy-MM-ddTHHz",
                    "yyyy-MM-ddTHH",
                    "yyyy-MM-ddTHHZ",

                    // with space
                    "yyyyMMdd HHmmsszzz",
                    "yyyyMMdd HHmmsszz",
                    "yyyyMMdd HHmmssz",
                    "yyyyMMdd HHmmss",
                    "yyyyMMdd HHmmssZ",

                    "yyyyMMdd HHmmzzz",
                    "yyyyMMdd HHmmzz",
                    "yyyyMMdd HHmmz",
                    "yyyyMMdd HHmm",
                    "yyyyMMdd HHmmZ",

                    "yyyyMMdd HHzzz",
                    "yyyyMMdd HHzz",
                    "yyyyMMdd HHz",
                    "yyyyMMdd HH",
                    "yyyyMMdd HHZ",

                    "yyyyMMddzzz",
                    "yyyyMMddzz",
                    "yyyyMMddz",
                    "yyyyMMdd",
                    "yyyyMMddZ",

                    // with T
                    "yyyyMMddTHHmmsszzz",
                    "yyyyMMddTHHmmsszz",
                    "yyyyMMddTHHmmssz",
                    "yyyyMMddTHHmmss",
                    "yyyyMMddTHHmmssZ",

                    "yyyyMMddTHHmmzzz",
                    "yyyyMMddTHHmmzz",
                    "yyyyMMddTHHmmz",
                    "yyyyMMddTHHmm",
                    "yyyyMMddTHHmmZ",

                    "yyyyMMddTHHzzz",
                    "yyyyMMddTHHzz",
                    "yyyyMMddTHHz",
                    "yyyyMMddTHH",
                    "yyyyMMddTHHZ",
                };

                public static readonly string[] StandardDateTimeFormats =
                {
                    // with dash, T and colon
                    "yyyy-MM-ddTHH:mm:ss.fffzzz",
                    "yyyy-MM-ddTHH:mm:ss.fffzz",
                    "yyyy-MM-ddTHH:mm:ss.fffz",
                    "yyyy-MM-ddTHH:mm:ss.fff",
                    "yyyy-MM-ddTHH:mm:ss.fffZ",

                    "yyyy-MM-ddTHH:mm:sszzz",
                    "yyyy-MM-ddTHH:mm:sszz",
                    "yyyy-MM-ddTHH:mm:ssz",
                    "yyyy-MM-ddTHH:mm:ss",
                    "yyyy-MM-ddTHH:mm:ssZ",

                    "yyyy-MM-ddTHH:mmzzz",
                    "yyyy-MM-ddTHH:mmzz",
                    "yyyy-MM-ddTHH:mmz",
                    "yyyy-MM-ddTHH:mm",
                    "yyyy-MM-ddTHH:mmZ",

                    "yyyy-MM-ddTHHzzz",
                    "yyyy-MM-ddTHHzz",
                    "yyyy-MM-ddTHHz",
                    "yyyy-MM-ddTHH",
                    "yyyy-MM-ddTHHZ",

                    "yyyy-MM-ddzzz",
                    "yyyy-MM-ddzz",
                    "yyyy-MM-ddz",
                    "yyyy-MM-dd",
                    "yyyy-MM-ddZ",
                };
                
                public static readonly string[] DateFormats =
                {
                    // with dash
                    "yyyy-MM-ddzzz",
                    "yyyy-MM-ddzz",
                    "yyyy-MM-ddz",
                    "yyyy-MM-dd",
                    "yyyy-MM-ddZ",

                    // without dash
                    "yyyyMMddzzz",
                    "yyyyMMddzz",
                    "yyyyMMddz",
                    "yyyyMMdd",
                    "yyyyMMddZ",
                };
            }
        }
    }
}
