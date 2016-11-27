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
    public class IpAddressAttributeTest
    {
        public class ClassWithIpAddressAttribute : BaseDto
        {
            [IpAddress]
            public string Property { get; set; }
        }

        public class ClassWithIpAddressOnNumericAttribute : BaseDto
        {
            [IpAddress]
            public double Property { get; set; }
        }

        [TestMethod]
        public void ValidatingIpAddressV4Succeeds()
        {
            var sut = new ClassWithIpAddressAttribute()
            {
                Property = "127.0.0.1"
            };

            Assert.IsTrue(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingIpAddressV6Succeeds()
        {
            var sut = new ClassWithIpAddressAttribute()
            {
                Property = "fe80::295c:c42d:80ce:ba26%17"
            };

            Assert.IsTrue(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingIpAddressOnNumericFails()
        {
            var sut = new ClassWithIpAddressOnNumericAttribute()
            {
                Property = 42L
            };

            Assert.IsFalse(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingIpAddressFails()
        {
            var sut = new ClassWithIpAddressAttribute()
            {
                Property = "XYZ"
            };

            Assert.IsFalse(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingNullIpAddressFails()
        {
            var sut = new ClassWithIpAddressAttribute()
            {
                Property = default(string)
            };

            Assert.IsFalse(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingEmptyIpAddressFails()
        {
            var sut = new ClassWithIpAddressAttribute()
            {
                Property = string.Empty
            };

            Assert.IsFalse(sut.IsValid());
        }
    }
}
