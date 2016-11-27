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
using biz.dfch.CS.Commons.Converters;
using biz.dfch.CS.Commons.Validation;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Validation
{
    [TestClass]
    public class DateRangeTest
    {
        [TestMethod]
        [ExpectContractFailure]
        public void EndGreaterStartThrowsContractException()
        {
            var start = new DateTimeOffset(2016, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2014, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));
            
            var sut = new DateRange(start, end);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void StartContainingTimeInformationThrowsContractException()
        {
            var start = new DateTimeOffset(2014, 5, 6, 1, 1, 1, 1, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2016, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));
            
            var sut = new DateRange(start, end);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void EndContainingTimeInformationThrowsContractException()
        {
            var start = new DateTimeOffset(2014, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2016, 5, 6, 1, 1, 1, 1, TimeSpan.FromHours(1));
            
            var sut = new DateRange(start, end);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void StartContainingTicksThrowsContractException()
        {
            var start = new DateTimeOffset(2014, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1)).AddTicks(1000);
            var end = new DateTimeOffset(2016, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));
            
            var sut = new DateRange(start, end);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void EndContainingTicksThrowsContractException()
        {
            var start = new DateTimeOffset(2014, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2016, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1)).AddTicks(1000);
            
            var sut = new DateRange(start, end);
        }

        [TestMethod]
        public void ValidStartAndEndSucceeds()
        {
            var start = new DateTimeOffset(2014, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2016, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));
            
            var sut = new DateRange(start, end);

            Assert.AreEqual(start, sut.Start);
            Assert.AreEqual(end, sut.End);
            Assert.AreEqual(end - start, sut.TimeSpan);
        }

        [TestMethod]
        public void ValidStartAndTimeComponentsSucceeds()
        {
            var start = new DateTimeOffset(2014, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));

            var years = 2;
            var months = 3;
            var days = 17;

            var end = start.AddYears(years).AddMonths(months).AddDays(days);

            var sut = new DateRange(start, years, months, days);

            Assert.IsNotNull(sut);

            Assert.AreEqual(start, sut.Start);
            Assert.AreEqual(end, sut.End);
            Assert.AreEqual(end - start, sut.TimeSpan);
        }

        [TestMethod]
        public void FromStringSucceeds()
        {
            var start = new DateTimeOffset(2014, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2015, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));

            var iso8601DatePeriod = string.Concat(start.ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", end.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            var iso8601DatePeriodUtc = string.Concat(start.ToUniversalTime().ToString("yyyy-MM-ddzzz"), "/", end.ToUniversalTime().ToString("yyyy-MM-ddzzz"));;
            
            var sut = new DateRange(iso8601DatePeriod);
            Assert.IsNotNull(sut);

            Assert.AreEqual(start, sut.Start);
            Assert.AreEqual(end, sut.End);
            Assert.AreEqual(end - start, sut.TimeSpan);

            var result = sut.ToString();
            Assert.AreEqual(iso8601DatePeriodUtc, result);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void FromStringContainingTimeInformationThrowsContractException()
        {
            var start = new DateTimeOffset(2014, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));
            var end = new DateTimeOffset(2015, 5, 6, 0, 0, 1, 0, TimeSpan.FromHours(1));

            var iso8601DatePeriod = string.Concat(start.ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", end.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            
            var sut = new DateRange(iso8601DatePeriod);
        }

        [TestMethod]
        public void FromStringWithDurationSucceeds()
        {
            var start = new DateTimeOffset(2014, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));

            var years = 2;
            var months = 3;
            var days = 17;
            var hours = 0;
            var minutes = 0;
            var seconds = 0;

            var end = start.AddYears(years).AddMonths(months).AddDays(days).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            var iso8601DatePeriodUtc = string.Concat(start.ToUniversalTime().ToString("yyyy-MM-ddzzz"), "/", end.ToUniversalTime().ToString("yyyy-MM-ddzzz"));;
            
            var iso8601DatePeriod = string.Concat(start.ToString("yyyy-MM-ddTHH:mm:sszzz"), "/", string.Format("P{0}Y{1}M{2}DT{3}H{4}M{5}S", years, months, days, hours, minutes, seconds));
            
            var sut = new DateRange(iso8601DatePeriod);
            Assert.IsNotNull(sut);

            Assert.AreEqual(start, sut.Start);
            Assert.AreEqual(end, sut.End);
            Assert.AreEqual(end - start, sut.TimeSpan);

            var result = sut.ToString();
            Assert.AreEqual(iso8601DatePeriodUtc, result);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void FromStringWithDurationContaingTimeInformationThrowsContractException()
        {
            var start = new DateTimeOffset(2014, 5, 6, 0, 0, 0, 0, TimeSpan.FromHours(1));

            var years = 2;
            var months = 3;
            var days = 17;
            var hours = 0;
            var minutes = 0;
            var seconds = 1;

            var end = start.AddYears(years).AddMonths(months).AddDays(days).AddHours(hours).AddMinutes(minutes).AddSeconds(seconds);
            
            var iso8601DatePeriod = string.Concat(start.ToString("yyyy-MM-ddzzz"), "/", string.Format("P{0}Y{1}M{2}DT{3}H{4}M{5}S", years, months, days, hours, minutes, seconds));
            
            var sut = new DateRange(iso8601DatePeriod);
        }

        // the following tests have a dependency on ConversionKeyBaseDto and its related classes
        // they therefore strictly do not belong in this test class

        public const string CONVERSION_KEY_NAME = "arbitrary.conversion.key.name";
        public class ClassWithDateRange : ConversionKeyBaseDto
        {
            [ConversionKey(CONVERSION_KEY_NAME)]
            public DateRange Property { get; set; }
        }

        [TestMethod]
        public void ConvertingFromConversionKeySucceeds()
        {
            var start = new DateTimeOffset(2016, 4, 1, 0, 0, 0, 0, TimeSpan.FromHours(2));
            var end = start.AddYears(1).AddMonths(2).AddDays(3);
            var iso8601DatePeriod = string.Concat(start.ToString("yyyy-MM-ddzzz"), "/", end.ToString("yyyy-MM-ddzzz"));
            var iso8601DatePeriodUtc = string.Concat(start.ToUniversalTime().ToString("yyyy-MM-ddzzz"), "/", end.ToUniversalTime().ToString("yyyy-MM-ddzzz"));

            var parameters = new DictionaryParameters()
            {
                { CONVERSION_KEY_NAME, iso8601DatePeriod }
            };

            var result = ConversionKeyConverter.Convert<ClassWithDateRange>(parameters);

            Assert.AreEqual(iso8601DatePeriodUtc, result.Property.ToString());
            Assert.AreEqual(start, result.Property.Start);
            Assert.AreEqual(end, result.Property.End);
            Assert.AreEqual(end - start, result.Property.TimeSpan);
        }

        [TestMethod]
        public void ConvertingFromConversionKeyUtcSucceeds()
        {
            var start = new DateTimeOffset(2016, 4, 1, 0, 0, 0, 0, TimeSpan.FromHours(0));
            var end = start.AddYears(1).AddMonths(2).AddDays(3);
            var iso8601DatePeriodUtc = string.Concat(start.ToUniversalTime().ToString("yyyy-MM-ddzzz"), "/", end.ToUniversalTime().ToString("yyyy-MM-ddzzz"));

            var parameters = new DictionaryParameters()
            {
                { CONVERSION_KEY_NAME, iso8601DatePeriodUtc }
            };

            var result = ConversionKeyConverter.Convert<ClassWithDateRange>(parameters);

            Assert.AreEqual(iso8601DatePeriodUtc, result.Property.ToString());
            Assert.AreEqual(start, result.Property.Start);
            Assert.AreEqual(end, result.Property.End);
            Assert.AreEqual(end - start, result.Property.TimeSpan);
        }
    }
}
