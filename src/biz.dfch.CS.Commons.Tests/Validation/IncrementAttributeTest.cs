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
    public class IncrementAttributeTest
    {
        private const double INCREMENT_VALUE = 2;
        
        public class ClassWithIncrement : BaseDto
        {
            [Increment(INCREMENT_VALUE)]
            public double DoubleProperty { get; set; }

            [Increment(INCREMENT_VALUE)]
            public float FloatProperty { get; set; }

            [Increment(INCREMENT_VALUE)]
            public decimal DecimalProperty { get; set; }

            [Increment(INCREMENT_VALUE)]
            public long LongProperty { get; set; }

            [Increment(INCREMENT_VALUE)]
            public int IntProperty { get; set; }

            [Increment(INCREMENT_VALUE)]
            public short ShortProperty { get; set; }
        }
            
        public class ClassWithIncrementOnString : BaseDto
        {
            [Increment(INCREMENT_VALUE)]
            public string StringProperty { get; set; }
        }
        
        public class ClassWithIncrementForErrorMessageValidation : BaseDto
        {
            [Increment(INCREMENT_VALUE)]
            public int IntProperty { get; set; }
        }

        [TestMethod]
        public void IncrementValidationReturnsTrue()
        {
            // Arrange
            short value = 42;
            var sut = new ClassWithIncrement
            {
                DoubleProperty = value,
                DecimalProperty = value,
                FloatProperty = value,
                IntProperty = value,
                ShortProperty = value,
                LongProperty = value
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }
            
        [TestMethod]
        public void IncrementValidationReturnsFalse()
        {
            // Arrange
            short value = 41;
            var sut = new ClassWithIncrement
            {
                DoubleProperty = value,
                DecimalProperty = value,
                FloatProperty = value,
                IntProperty = value,
                ShortProperty = value,
                LongProperty = value
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IncrementValidationOnStringReturnsFalse()
        {
            // Arrange
            short value = 41;
            var sut = new ClassWithIncrementOnString()
            {
                StringProperty = "arbitrary-string"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IncrementValidationWithErrorMessage()
        {
            // Arrange
            short value = 41;
            var sut = new ClassWithIncrementForErrorMessageValidation()
            {
                IntProperty = value,
            };
            
            // Act
            var isValid = sut.IsValid();
            Assert.IsFalse(isValid);

            var results = sut.GetValidationResults();

            // Assert
            Assert.AreEqual(1, results.Count);
            var errorMessage = results[0].ErrorMessage;
            Assert.IsTrue(!string.IsNullOrWhiteSpace(errorMessage));
            
            // DFTODO - check if stirng conversion should honour culture
            Assert.IsTrue(errorMessage.Contains(INCREMENT_VALUE.ToString()), errorMessage);
        }
    }
}
