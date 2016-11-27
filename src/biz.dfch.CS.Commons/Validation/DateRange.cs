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

using System;
using System.Diagnostics.Contracts;

namespace biz.dfch.CS.Commons.Validation
{
    public class DateRange : DateTimeRange
    {
        private const string ISO8601_DATE_PERIOD_ERROR_MESSAGE = 
            "iso8601DatePeriod must be in the following formats: " + 
            "'CCYY[-]MM[-]DD[zzz]/CCYY[-]MM[-]DD[zzz]' " + 
            "or 'CCYY[-]MM[-]DD[zzz]/PnYnMnDT0H0M0S'.";

        public DateRange(string iso8601DateTimePeriod)
            : base(iso8601DateTimePeriod)
        {
            Contract.Ensures(0 == Start.Hour && 0 == Start.Minute && 0 == Start.Second && 0 == Start.Millisecond && 0 == (Start.Ticks % 10000000), ISO8601_DATE_PERIOD_ERROR_MESSAGE);
            Contract.Ensures(0 == End.Hour && 0 == End.Minute && 0 == End.Second && 0 == End.Millisecond && 0 == (End.Ticks % 10000000), ISO8601_DATE_PERIOD_ERROR_MESSAGE);

            // all code is executed in base class
        }
            
        public DateRange(DateTimeOffset start)
            : base(new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, start.Offset), DateTimeOffset.MaxValue)
        {
            // N/A
        }

        public DateRange(DateTimeOffset start, DateTimeOffset end)
            : base(new DateTimeOffset(start.Year, start.Month, start.Day, 0, 0, 0, start.Offset), new DateTimeOffset(end.Year, end.Month, end.Day, 0, 0, 0, end.Offset))
        {
            Contract.Requires(0 == start.Hour && 0 == start.Minute && 0 == start.Second && 0 == start.Millisecond && 0 == (start.Ticks % 10000000));
            Contract.Requires(0 == end.Hour && 0 == end.Minute && 0 == end.Second && 0 == end.Millisecond && 0 == (end.Ticks % 10000000));

            // all code is executed in base class
        }

        public DateRange(DateTimeOffset start, int years, int months, int days)
            : this(start, start.AddYears(years).AddMonths(months).AddDays(days))
        {
            // N/A
        }

        public DateRange(DateTimeOffset start, int years, int months)
            : this(start, start.AddYears(years).AddMonths(months))
        {
            // N/A
        }

        public DateRange(DateTimeOffset start, int years)
            : this(start, start.AddYears(years))
        {
            // N/A
        }

        public override string ToString()
        {
            var result = string.Concat(Start.ToUniversalTime().ToString("yyyy-MM-ddzzz"), "/", End.ToUniversalTime().ToString("yyyy-MM-ddzzz"));
            return result;
        }
    }
}
