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

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests
{
    [TestClass]
    public class BaseDtoTest
    {
        private const string STRING_PROPERTY_DEFAULT = "tralala";
        private const string STRING_PROPERTY_OVERRIDDEN = "tralala-overriden";
        private const long LONG_PROPERTY_DEFAULT = 42L;
        private const long LONG_PROPERTY2_OVERRIDDEN = 5L;
        private const long DOUBLE_PROPERTY_DEFAULT = 42L;

        public class BaseDtoImplWithDefaultConstructor : BaseDto
        {
            [DefaultValue(STRING_PROPERTY_DEFAULT)]
            public virtual string StringProperty { get; set; }

            [DefaultValue(LONG_PROPERTY_DEFAULT)]
            public virtual long LongProperty { get; set; }

            public virtual double DoubleProperty { get; set; }
        }
            
        public class BaseDtoImplWithConstructor : BaseDto
        {
            public BaseDtoImplWithConstructor()
            {
                LongProperty2 = LONG_PROPERTY2_OVERRIDDEN;
            }

            [DefaultValue(STRING_PROPERTY_DEFAULT)]
            public string StringProperty { get; set; }

            [DefaultValue(LONG_PROPERTY_DEFAULT)]
            public long LongProperty { get; set; }

            [DefaultValue(LONG_PROPERTY_DEFAULT)]
            public long LongProperty2 { get; set; }

            public double DoubleProperty { get; set; }
        }

        public class DerivedFromBaseDtoImplWithDefaultConstructor 
            : BaseDtoImplWithDefaultConstructor
        {
            [DefaultValue(STRING_PROPERTY_OVERRIDDEN)]
            public override string StringProperty { get; set; }

            public override long LongProperty { get; set; }

            [DefaultValue(DOUBLE_PROPERTY_DEFAULT)]
            public override double DoubleProperty { get; set; }
        }
            
        [TestMethod]
        public void TestWithDefaultConstructor()
        {
            var sut = new BaseDtoImplWithDefaultConstructor();
            
            Assert.IsNotNull(sut);
            Assert.AreEqual(STRING_PROPERTY_DEFAULT, sut.StringProperty);
            Assert.AreEqual(LONG_PROPERTY_DEFAULT, sut.LongProperty);
            Assert.AreEqual(0.0, sut.DoubleProperty);
        }

        [TestMethod]
        public void TestWithoutConstructor()
        {
            var sut = new BaseDtoImplWithConstructor();
            
            Assert.IsNotNull(sut);
            Assert.AreEqual(STRING_PROPERTY_DEFAULT, sut.StringProperty);
            Assert.AreEqual(LONG_PROPERTY_DEFAULT, sut.LongProperty);
            Assert.AreEqual(LONG_PROPERTY2_OVERRIDDEN, sut.LongProperty2);
            Assert.AreEqual(0.0, sut.DoubleProperty);
        }

        [TestMethod]
        public void TestWithDerivedClass()
        {
            var sut = new DerivedFromBaseDtoImplWithDefaultConstructor();
            
            Assert.IsNotNull(sut);
            Assert.AreEqual(STRING_PROPERTY_OVERRIDDEN, sut.StringProperty);
            Assert.AreEqual(LONG_PROPERTY_DEFAULT, sut.LongProperty);
            Assert.AreEqual(DOUBLE_PROPERTY_DEFAULT, sut.DoubleProperty);
        }

        public class BaseDtoImplementation1 : BaseDto
        {
            [Required]
            public string RequiredStringProperty1 { get; set; }

            [Required]
            public string RequiredStringProperty2 { get; set; }
        }
            
        [TestMethod]
        public void TestIsValidSinglePropertySucceeds()
        {
            var sut = new BaseDtoImplementation1
            {
                RequiredStringProperty1 = "arbitrary-value"
            };

            var isValidProperty1 = sut.IsValid("RequiredStringProperty1");
            Assert.IsTrue(isValidProperty1);

            var isValidProperty2 = sut.IsValid("RequiredStringProperty2");
            Assert.IsFalse(isValidProperty2);

            var isValidClass = sut.IsValid();
            Assert.IsFalse(isValidClass);
        }

        [TestMethod]
        public void TestIsValidSinglePropertyAndValueSucceeds()
        {
            var sut = new BaseDtoImplementation1
            {
                RequiredStringProperty1 = "arbitrary-value"
            };

            var isValidProperty2 = sut.IsValid("RequiredStringProperty2", null);
            Assert.IsFalse(isValidProperty2);
        }

        [TestMethod]
        public void TestGetErrorMessagesSinglePropertySucceeds()
        {
            var sut = new BaseDtoImplementation1
            {
                RequiredStringProperty1 = "arbitrary-value"
            };

            var errorMessages1 = sut.GetErrorMessages("RequiredStringProperty1");
            Assert.AreEqual(0, errorMessages1.Count);

            var errorMessages2 = sut.GetErrorMessages("RequiredStringProperty2");
            Assert.AreEqual(1, errorMessages2.Count);

            var errorMessages = sut.GetErrorMessages();
            Assert.AreEqual(1, errorMessages.Count);
        }

        [TestMethod]
        public void TestGetErrorMessagesSinglePropertyAndValueSucceeds()
        {
            var sut = new BaseDtoImplementation1
            {
                RequiredStringProperty1 = "arbitrary-value"
            };

            var errorMessages2 = sut.GetErrorMessages("RequiredStringProperty2", null);
            Assert.AreEqual(1, errorMessages2.Count);
        }

        [TestMethod]
        public void TestGetValidationResultsSinglePropertySucceeds()
        {
            var sut = new BaseDtoImplementation1
            {
                RequiredStringProperty1 = "arbitrary-value"
            };

            var validationResults1 = sut.GetValidationResults("RequiredStringProperty1");
            Assert.AreEqual(0, validationResults1.Count);

            var validationResults2 = sut.GetValidationResults("RequiredStringProperty2");
            Assert.AreEqual(1, validationResults2.Count);

            var validationResults = sut.GetValidationResults();
            Assert.AreEqual(1, validationResults.Count);
        }

        [TestMethod]
        public void TestGetValidationResultsSinglePropertyAndValueSucceeds()
        {
            var sut = new BaseDtoImplementation1
            {
                RequiredStringProperty1 = "arbitrary-value"
            };

            var validationResults2 = sut.GetValidationResults("RequiredStringProperty2", null);
            Assert.AreEqual(1, validationResults2.Count);
        }

        [TestMethod]
        public void TestValidateSinglePropertySucceeds()
        {
            var sut = new BaseDtoImplementation1
            {
                RequiredStringProperty1 = "arbitrary-value"
            };

            sut.Validate("RequiredStringProperty1");
        }

        [TestMethod]
        [ExpectContractFailure]
        public void TestValidateSinglePropertyThrowsContractException()
        {
            var sut = new BaseDtoImplementation1
            {
                RequiredStringProperty1 = "arbitrary-value"
            };

            sut.Validate("RequiredStringProperty2");
        }

        [TestMethod]
        [ExpectContractFailure]
        public void TestValidateClassThrowsContractException()
        {
            var sut = new BaseDtoImplementation1
            {
                RequiredStringProperty1 = "arbitrary-value"
            };

            sut.Validate("RequiredStringProperty2");
        }
    }
}
