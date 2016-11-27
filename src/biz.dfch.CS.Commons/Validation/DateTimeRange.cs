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
using System.Globalization;
using System.Text.RegularExpressions;

namespace biz.dfch.CS.Commons.Validation
{
    public class DateTimeRange
    {
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public TimeSpan TimeSpan { get; set; }

        private const string ISO8601_DATE_TIME_PERIOD_ERROR_MESSAGE = 
            "iso8601DatePeriod must be in the following formats: " + 
            "'CCYY[-]MM[-]DD[T ]hh[:]mm[:]ss[zzz]/CCYY[-]MM[-]DD[T ]hh[:]mm[:]ss[zzz]' " + 
            "or 'CCYY[-]MM[-]DD[T ]hh[:]mm[:]ss[zzz]/PnYnMnDTnHnMnS'.";

        public DateTimeRange(string iso8601DateTimePeriod)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(iso8601DateTimePeriod));
            Contract.Requires(iso8601DateTimePeriod.Contains("/"));

            var iso8601DatePeriodSegments = iso8601DateTimePeriod.Split('/');
            Contract.Assert(2 == iso8601DatePeriodSegments.Length, ISO8601_DATE_TIME_PERIOD_ERROR_MESSAGE);

            var startSegment = iso8601DatePeriodSegments[0];
            var endOrDurationSegment = iso8601DatePeriodSegments[1];

            DateTimeOffset start;
            var result = DateTimeOffset.TryParseExact(startSegment, Constants.Validation.Iso8601.DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out start);
            Contract.Assert(result, string.Concat(ISO8601_DATE_TIME_PERIOD_ERROR_MESSAGE, " START segment is invalid."));
            Start = start;

            if (!endOrDurationSegment.StartsWith("P"))
            {
                DateTimeOffset end;
                result = DateTimeOffset.TryParseExact(endOrDurationSegment, Constants.Validation.Iso8601.DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out end);
                Contract.Assert(result, string.Concat(ISO8601_DATE_TIME_PERIOD_ERROR_MESSAGE, " END segment is invalid."));
                Contract.Assert(end >= start);

                End = end;
                TimeSpan = end - start;
            }
            else
            {
	            var match = Regex.Match(endOrDurationSegment, @"^P(\d+)Y(\d+)M(\d+)DT(\d+)H(\d+)M(\d+)S$");
                Contract.Assert(match.Success, string.Concat(ISO8601_DATE_TIME_PERIOD_ERROR_MESSAGE, " DURATION segment is invalid."));

                var timeSpan = new TimeSpan
                (
                    int.Parse(match.Groups[4].Value)
                    , 
                    int.Parse(match.Groups[5].Value)
                    ,
                    int.Parse(match.Groups[6].Value)
                );
                
                var end = start.Add(timeSpan);
                end = end.AddYears(int.Parse(match.Groups[1].Value));
                end = end.AddMonths(int.Parse(match.Groups[2].Value));
                end = end.AddDays(int.Parse(match.Groups[3].Value));
                Contract.Assert(end >= start);
                
                End = end;
                TimeSpan = end - start;
            }
        }

        public DateTimeRange(DateTimeOffset start)
            : this(start, DateTimeOffset.MaxValue)
        {
            // N/A
        }

        public DateTimeRange(DateTimeOffset start, int years, int months, int days, int hours, int minutes, int seconds)
            : this(start, start.AddYears(years).AddMonths(months).AddDays(days).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds))
        {
            // N/A
        }

        public DateTimeRange(DateTimeOffset start, int years, int months, int days, int hours, int minutes)
            : this(start, start.AddYears(years).AddMonths(months).AddDays(days).AddHours(hours).AddMinutes(minutes))
        {
            // N/A
        }

        public DateTimeRange(DateTimeOffset start, int years, int months, int days, int hours)
            : this(start, start.AddYears(years).AddMonths(months).AddDays(days).AddHours(hours))
        {
            // N/A
        }

        public DateTimeRange(DateTimeOffset start, int years, int months, int days)
            : this(start, start.AddYears(years).AddMonths(months).AddDays(days))
        {
            // N/A
        }

        public DateTimeRange(DateTimeOffset start, int years, int months)
            : this(start, start.AddYears(years).AddMonths(months))
        {
            // N/A
        }

        public DateTimeRange(DateTimeOffset start, int years)
            : this(start, start.AddYears(years))
        {
            // N/A
        }

        public DateTimeRange(DateTimeOffset start, TimeSpan timeSpan)
            : this(start, start.Add(timeSpan))
        {
            // N/A
        }

        public DateTimeRange(DateTimeOffset start, DateTimeOffset end)
        {
            Contract.Requires(null != start);
            Contract.Requires(null != end);
            Contract.Requires(end >= start);

            Start = start;
            End = end;
            TimeSpan = end - start;
        }

        public override string ToString()
        {
            var result = string.Concat(Start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", End.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"));
            return result;
        }
    }
}
