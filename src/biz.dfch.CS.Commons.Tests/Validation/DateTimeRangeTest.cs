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
using System.Globalization;
using biz.dfch.CS.Commons.Converters;
using biz.dfch.CS.Commons.Validation;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Validation
{
    [TestClass]
    public class DateTimeRangeTest
    {
        [TestMethod]
        [ExpectContractFailure]
        public void EndGreaterStartThrowsContractException()
        {
            var start = new DateTimeOffset(2016, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2014, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));
            
            var sut = new DateTimeRange(start, end);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void NagativeTimeSpanThrowsContractException()
        {
            var start = new DateTimeOffset(2016, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));
            var timeSpan = new TimeSpan(-5, -21, -42);

            var sut = new DateTimeRange(start, timeSpan);
        }

        [TestMethod]
        public void StartAndEndSucceeds()
        {
            var start = new DateTimeOffset(2014, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2016, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));
            
            var sut = new DateTimeRange(start, end);

            Assert.AreEqual(start, sut.Start);
            Assert.AreEqual(end - start, sut.TimeSpan);
            Assert.AreEqual(end, sut.End);
        }

        [TestMethod]
        public void StartAndTimeSpanSucceeds()
        {
            var start = new DateTimeOffset(2016, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));
            var timeSpan = new TimeSpan(5, 21, 42);

            var sut = new DateTimeRange(start, timeSpan);

            Assert.AreEqual(start, sut.Start);
            Assert.AreEqual(timeSpan, sut.TimeSpan);
            Assert.AreEqual(start.Add(timeSpan), sut.End);
        }

        [TestMethod]
        public void ValidStartAndEndSucceeds()
        {
            var start = new DateTimeOffset(2014, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2015, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));
            
            var sut = new DateTimeRange(start, end);

            Assert.IsNotNull(sut);

            Assert.AreEqual(start, sut.Start);
            Assert.AreEqual(end, sut.End);
            Assert.AreEqual(end - start, sut.TimeSpan);
        }

        [TestMethod]
        public void ValidStartAndTimeCompnentsSucceeds()
        {
            var start = new DateTimeOffset(2014, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));

            var years = 2;
            var months = 3;
            var days = 17;
            var hours = 8;
            var minutes = 0;
            var seconds = 42;

            var end = start.AddYears(years).AddMonths(months).AddDays(days).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);

            var sut = new DateTimeRange(start, years, months, days, hours, minutes, seconds);

            Assert.IsNotNull(sut);

            Assert.AreEqual(start, sut.Start);
            Assert.AreEqual(end, sut.End);
            Assert.AreEqual(end - start, sut.TimeSpan);
        }

        [TestMethod]
        public void ToStringSucceeds()
        {
            var start = new DateTimeOffset(2014, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2015, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));
            
            var sut = new DateTimeRange(start, end);

            Assert.IsNotNull(sut);

            var result = sut.ToString();

            Assert.IsTrue(result.StartsWith(start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")));
            Assert.IsTrue(result.Contains("/"));
            Assert.IsTrue(result.EndsWith(end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")));
        }

        [TestMethod]
        public void FromStringSucceeds()
        {
            var start = new DateTimeOffset(2014, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2015, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));

            var iso8601DatePeriod = string.Concat(start.ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", end.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            var iso8601DatePeriodUtc = string.Concat(start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"));;
            
            var sut = new DateTimeRange(iso8601DatePeriod);
            Assert.IsNotNull(sut);

            Assert.AreEqual(start, sut.Start);
            Assert.AreEqual(end, sut.End);
            Assert.AreEqual(end - start, sut.TimeSpan);

            var result = sut.ToString();
            Assert.AreEqual(iso8601DatePeriodUtc, result);
        }

        [TestMethod]
        public void FromStringWithDurationSucceeds()
        {
            var start = new DateTimeOffset(2014, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));

            var years = 2;
            var months = 3;
            var days = 17;
            var hours = 8;
            var minutes = 0;
            var seconds = 42;

            var end = start.AddYears(years).AddMonths(months).AddDays(days).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            var iso8601DatePeriodUtc = string.Concat(start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"));;
            
            var iso8601DatePeriod = string.Concat(start.ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", string.Format("P{0}Y{1}M{2}DT{3}H{4}M{5}S", years, months, days, hours, minutes, seconds));
            
            var sut = new DateTimeRange(iso8601DatePeriod);
            Assert.IsNotNull(sut);

            Assert.AreEqual(start, sut.Start);
            Assert.AreEqual(end, sut.End);
            Assert.AreEqual(end - start, sut.TimeSpan);

            var result = sut.ToString();
            Assert.AreEqual(iso8601DatePeriodUtc, result);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void FromStringWithInvalidDurationThrowsContractException()
        {
            var start = new DateTimeOffset(2014, 5, 6, 5, 21, 42, 0, TimeSpan.FromHours(1));

            var years = 2;
            var months = 3;
            var days = 17;
            var hours = 8;
            var minutes = 0;

            var iso8601DatePeriod = string.Concat(start.ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", string.Format("P{0}Y{1}M{2}DT{3}H{4}M", years, months, days, hours, minutes));
            
            var sut = new DateTimeRange(iso8601DatePeriod);
        }

        [TestMethod]
        public void DateTimeOffsetParsingIso8601Succeeds()
        {
            string[] examples =
            {
                // with dash, space and colon
                "2001-08-15 13:05:42+02:00",
                "2001-08-15 13:05:42+02",
                "2001-08-15 13:05:42+2",
                "2001-08-15 13:05:42Z",
                "2001-08-15 13:05:42",
                
                "2001-08-15 13:05+02:00",
                "2001-08-15 13:05+02",
                "2001-08-15 13:05+2",
                "2001-08-15 13:05Z",
                "2001-08-15 13:05",
                
                "2001-08-15 13+02:00",
                "2001-08-15 13+02",
                "2001-08-15 13+2",
                "2001-08-15 13Z",
                "2001-08-15 13",
                
                "2001-08-15+02:00",
                "2001-08-15+02",
                "2001-08-15+2",
                "2001-08-15",
                "2001-08-15Z",

                // with dash, T and colon
                "2001-08-15T13:05:42+02:00",
                "2001-08-15T13:05:42+02",
                "2001-08-15T13:05:42+2",
                "2001-08-15T13:05:42Z",
                "2001-08-15T13:05:42",

                "2001-08-15T13:05+02:00",
                "2001-08-15T13:05+02",
                "2001-08-15T13:05+2",
                "2001-08-15T13:05Z",
                "2001-08-15T13:05",
                
                "2001-08-15T13+02:00",
                "2001-08-15T13+02",
                "2001-08-15T13+2",
                "2001-08-15T13Z",
                "2001-08-15T13",

                // with space
                "20010815 130542+02:00",
                "20010815 130542+02",
                "20010815 130542+2",
                "20010815 130542Z",
                "20010815 130542",
                
                "20010815 1305+02:00",
                "20010815 1305+02",
                "20010815 1305+2",
                "20010815 1305Z",
                "20010815 1305",
                
                "20010815 13+02:00",
                "20010815 13+02",
                "20010815 13+2",
                "20010815 13Z",
                "20010815 13",

                "20010815+02:00",
                "20010815+02",
                "20010815+2",
                "20010815",
                "20010815Z",

                // with T
                "20010815T130542+02:00",
                "20010815T130542+02",
                "20010815T130542+2",
                "20010815T130542Z",
                "20010815T130542",

                "20010815T1305+02:00",
                "20010815T1305+02",
                "20010815T1305+2",
                "20010815T1305Z",
                "20010815T1305",
                
                "20010815T13+02:00",
                "20010815T13+02",
                "20010815T13+2",
                "20010815T13Z",
                "20010815T13",
            };

            foreach (var example in examples)
            {
                DateTimeOffset dateTimeOffset;
                var result = DateTimeOffset.TryParseExact(example, Constants.Validation.Iso8601.DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset);
                Assert.IsTrue(result, example);
            }
        }

        // the following tests have a dependency on ConversionKeyBaseDto and its related classes
        // they therefore strictly do not belong in this test class

        [TestMethod]
        public void DateTimeOffsetParsingIso8601WithFractionsFails()
        {
            var example = "2001-08-15T13:05:42.123+02:00";

            DateTimeOffset dateTimeOffset;
            var result = DateTimeOffset.TryParseExact(example, Constants.Validation.Iso8601.DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset);
            Assert.IsFalse(result, example);
        }

        public const string CONVERSION_KEY_NAME = "arbitrary.conversion.key.name";
        public class ClassWithDateTimeRange : ConversionKeyBaseDto
        {
            [ConversionKey(CONVERSION_KEY_NAME)]
            public DateTimeRange Property { get; set; }
        }

        [TestMethod]
        public void ConvertingFromConversionKeySucceeds()
        {
            var start = new DateTimeOffset(2016, 4, 1, 1, 2, 3, 0, TimeSpan.FromHours(2));
            var end = start.AddYears(1).AddMonths(2).AddDays(3).AddHours(4).AddMinutes(5).AddSeconds(6);
            var iso8601DatePeriod = string.Concat(start.ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", end.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            var iso8601DatePeriodUtc = string.Concat(start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"));

            var parameters = new DictionaryParameters()
            {
                { CONVERSION_KEY_NAME, iso8601DatePeriod }
            };

            var result = ConversionKeyConverter.Convert<ClassWithDateTimeRange>(parameters);

            Assert.AreEqual(iso8601DatePeriodUtc, result.Property.ToString());
            Assert.AreEqual(start, result.Property.Start);
            Assert.AreEqual(end, result.Property.End);
            Assert.AreEqual(end - start, result.Property.TimeSpan);
        }

        [TestMethod]
        public void ConvertingFromConversionKeyUtcSucceeds()
        {
            var start = new DateTimeOffset(2016, 4, 1, 1, 2, 3, 0, TimeSpan.FromHours(2));
            var end = start.AddYears(1).AddMonths(2).AddDays(3).AddHours(4).AddMinutes(5).AddSeconds(6);
            var iso8601DatePeriodUtc = string.Concat(start.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", end.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:sszzz"));

            var parameters = new DictionaryParameters()
            {
                { CONVERSION_KEY_NAME, iso8601DatePeriodUtc }
            };

            var result = ConversionKeyConverter.Convert<ClassWithDateTimeRange>(parameters);

            Assert.AreEqual(iso8601DatePeriodUtc, result.Property.ToString());
            Assert.AreEqual(start, result.Property.Start);
            Assert.AreEqual(end, result.Property.End);
            Assert.AreEqual(end - start, result.Property.TimeSpan);
        }
    }
}
