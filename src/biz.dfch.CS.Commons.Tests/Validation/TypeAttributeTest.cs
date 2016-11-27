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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Commons.Validation;

namespace biz.dfch.CS.Commons.Tests.Validation
{
    [TestClass]
    public class TypeAttributeTest
    {
        public class ClassWithTypeAttributeStringProperty : BaseDto
        {
            [Type("System.String")]
            public object Property { get; set; }
        }

        [TestMethod]
        public void StringWithStringSucceeds()
        {
            var sut = new ClassWithTypeAttributeStringProperty
            {
                Property = "arbitrary-string"
            };

            var result = sut.IsValid();

            Assert.IsTrue(result);
        }
            
        [TestMethod]
        public void StringWithNullFails()
        {
            var sut = new ClassWithTypeAttributeStringProperty
            {
                Property = null
            };

            var result = sut.IsValid();

            Assert.IsFalse(result);
        }
            
        [TestMethod]
        public void StringWithNumberSucceeds()
        {
            var sut = new ClassWithTypeAttributeStringProperty
            {
                Property = 42L
            };

            var result = sut.IsValid();

            Assert.IsTrue(result);
        }
            
        [TestMethod]
        public void StringWithGuidFails()
        {
            var sut = new ClassWithTypeAttributeStringProperty
            {
                Property = Guid.NewGuid()
            };

            var result = sut.IsValid();

            Assert.IsFalse(result);
        }
            
        public class ClassWithTypeAttributeDoubleProperty : BaseDto
        {
            [Type("System.Double")]
            public object Property { get; set; }
        }

        [TestMethod]
        public void DoubleWithStringFails()
        {
            var sut = new ClassWithTypeAttributeDoubleProperty
            {
                Property = "arbitrary-string"
            };

            var result = sut.IsValid();

            Assert.IsFalse(result);
        }
            
        [TestMethod]
        public void DoubleWithNullFails()
        {
            var sut = new ClassWithTypeAttributeDoubleProperty
            {
                Property = null
            };

            var result = sut.IsValid();

            Assert.IsFalse(result);
        }
            
        [TestMethod]
        public void DoubleWithNumberSucceeds()
        {
            var sut = new ClassWithTypeAttributeDoubleProperty
            {
                Property = 42L
            };

            var result = sut.IsValid();

            Assert.IsTrue(result);
        }
            
        [TestMethod]
        public void DoubleWithGuidFails()
        {
            var sut = new ClassWithTypeAttributeDoubleProperty
            {
                Property = Guid.NewGuid()
            };

            var result = sut.IsValid();

            Assert.IsFalse(result);
        }
            
        public class ClassWithTypeAttributeGuidProperty : BaseDto
        {
            [Type("System.Guid")]
            public object Property { get; set; }
        }

        [TestMethod]
        public void GuidWithStringFails()
        {
            var sut = new ClassWithTypeAttributeGuidProperty
            {
                Property = "arbitrary-string"
            };

            var result = sut.IsValid();

            Assert.IsFalse(result);
        }
            
        [TestMethod]
        public void GuidWithNullFails()
        {
            var sut = new ClassWithTypeAttributeGuidProperty
            {
                Property = null
            };

            var result = sut.IsValid();

            Assert.IsFalse(result);
        }
            
        [TestMethod]
        public void GuidWithNumberFails()
        {
            var sut = new ClassWithTypeAttributeGuidProperty
            {
                Property = 42L
            };

            var result = sut.IsValid();

            Assert.IsFalse(result);
        }
            
        [TestMethod]
        public void GuidWithGuidSucceeds()
        {
            var sut = new ClassWithTypeAttributeGuidProperty
            {
                Property = Guid.NewGuid()
            };

            var result = sut.IsValid();

            Assert.IsTrue(result);
        }
         
    }
}
