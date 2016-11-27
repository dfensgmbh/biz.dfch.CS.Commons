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

using biz.dfch.CS.Commons.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Validation
{
    [TestClass]
    public class CurrencyAttributeTest
    {
        public class ClassWithCurrencyAttribute : BaseDto
        {
            [Currency]
            public string Currency { get; set; }
        }

        public class ClassWithCurrencyOnNumericAttribute : BaseDto
        {
            [Currency]
            public double Currency { get; set; }
        }

        [TestMethod]
        public void ValidatingCurrencySucceeds()
        {
            var sut = new ClassWithCurrencyAttribute()
            {
                Currency = "EUR"
            };

            Assert.IsTrue(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingCurrencyOnNumericFails()
        {
            var sut = new ClassWithCurrencyOnNumericAttribute()
            {
                Currency = 42L
            };

            Assert.IsFalse(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingCurrencyFails()
        {
            var sut = new ClassWithCurrencyAttribute()
            {
                Currency = "XYZ"
            };

            Assert.IsFalse(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingNullCurrencyFails()
        {
            var sut = new ClassWithCurrencyAttribute()
            {
                Currency = default(string)
            };

            Assert.IsFalse(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingEmptyCurrencyFails()
        {
            var sut = new ClassWithCurrencyAttribute()
            {
                Currency = string.Empty
            };

            Assert.IsFalse(sut.IsValid());
        }
    }
}
