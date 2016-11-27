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
    public class GuidAttributeTest
    {
        public class ClassWithGuidAttribute : BaseDto
        {
            [Guid]
            public string Property { get; set; }
        }

        public class ClassWithGuidOnNumericAttribute : BaseDto
        {
            [Guid]
            public double Property { get; set; }
        }

        [TestMethod]
        public void ValidatingGuidSucceeds()
        {
            var sut = new ClassWithGuidAttribute()
            {
                Property = "cb44e6c5-c55e-43a9-b37d-06d202d0484c"
            };

            Assert.IsTrue(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingEmptyGuidSucceeds()
        {
            var sut = new ClassWithGuidAttribute()
            {
                Property = "00000000-0000-0000-0000-000000000000"
            };

            Assert.IsTrue(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingGuidOnNumericFails()
        {
            var sut = new ClassWithGuidOnNumericAttribute()
            {
                Property = 42L
            };

            Assert.IsFalse(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingGuidFails()
        {
            var sut = new ClassWithGuidAttribute()
            {
                Property = "XYZ"
            };

            Assert.IsFalse(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingNullGuidFails()
        {
            var sut = new ClassWithGuidAttribute()
            {
                Property = default(string)
            };

            Assert.IsFalse(sut.IsValid());
        }

        [TestMethod]
        public void ValidatingEmptyGuidFails()
        {
            var sut = new ClassWithGuidAttribute()
            {
                Property = string.Empty
            };

            Assert.IsFalse(sut.IsValid());
        }
    }
}
