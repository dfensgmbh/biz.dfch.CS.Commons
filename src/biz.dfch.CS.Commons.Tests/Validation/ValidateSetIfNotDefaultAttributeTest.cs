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

using System.ComponentModel.DataAnnotations;
using biz.dfch.CS.Commons.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Validation
{
    [TestClass]
    public class ValidateSetIfNotDefaultAttributeTest
    {

        public class ClassWithNonRequiredValidateSetAttributePropertiesAsArray : BaseDto
        {
            [ValidateSetIfNotDefault("item1", "item2", "item3", "valid-set-item")]
            public string StringPropertyWithValidateSet { get; set; }
        }
        
        public class ClassWithNonRequiredValidateSetAttributePropertiesAsCommaSeparatedString : BaseDto
        {
            [ValidateSetIfNotDefault("item1|item2|item3|valid-set-item")]
            public string StringPropertyWithValidateSet { get; set; }
        }
        
        public class ClassWithNonRequiredValidateSetAttributePropertiesAsSingleString : BaseDto
        {
            [ValidateSetIfNotDefault("valid-set-item")]
            public string StringPropertyWithValidateSet { get; set; }
        }
        
        public class ClassWithNonRequiredValidateSetAttributePropertiesWithDuplicateItems : BaseDto
        {
            [ValidateSetIfNotDefault("duplicate-set-item", "duplicate-set-item")]
            public string StringPropertyWithValidateSet { get; set; }
        }
        
        public class ClassWithRequiredValidateSetAttributePropertiesAsArray : BaseDto
        {
            [Required]
            [ValidateSetIfNotDefault("item1", "item2", "item3", "valid-set-item")]
            public string StringPropertyWithValidateSet { get; set; }
        }

        public enum EnumsToBeValidated : long
        {
            Value1 = 1,
            Value2 = 2,
            Value4 = 3,
            Value3 = 4,
        }

        public class ClassWithValidateSetEnum : BaseDto
        {
            [ValidateSetIfNotDefault(typeof(EnumsToBeValidated))]
            public string Property { get; set; }
        }
        
        public class ClassWithInvalidValidateSetEnum : BaseDto
        {
            [ValidateSetIfNotDefault(typeof(object))]
            public string Property { get; set; }
        }
        
        [TestMethod]
        public void NonRequiredPropertyWithValidSetItemReturnsTrue()
        {
            // Arrange
            var sut = new ClassWithNonRequiredValidateSetAttributePropertiesAsArray
            {
                StringPropertyWithValidateSet = "valid-set-item"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void NonRequiredPropertyWithValidSetItemReturnsFalse()
        {
            // Arrange
            var sut = new ClassWithNonRequiredValidateSetAttributePropertiesAsArray
            {
                StringPropertyWithValidateSet = "invalid-set-item"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NonRequiredPropertyWithValidSetItemReturnsTrueOnNullProperty()
        {
            // Arrange
            var sut = new ClassWithNonRequiredValidateSetAttributePropertiesAsArray();

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void ValidateSetReturnsRequiredSetWithErrorMessage()
        {
            // Arrange
            var sut = new ClassWithNonRequiredValidateSetAttributePropertiesAsArray
            {
                StringPropertyWithValidateSet = "invalid-set-item"
            };
            var isValid = sut.IsValid();
            Assert.IsFalse(isValid);

            // Act
            var results = sut.GetValidationResults();

            // Assert
            Assert.AreEqual(1, results.Count);
            var errorMessage = results[0].ErrorMessage;
            Assert.IsTrue(!string.IsNullOrWhiteSpace(errorMessage));
            
            Assert.IsTrue(errorMessage.Contains("item1|item2|item3|valid-set-item"), errorMessage);
        }

        [TestMethod]
        public void PropertyWithValidSetItemAsCommaSeparatedStringReturnsTrue()
        {
            // Arrange
            var sut = new ClassWithNonRequiredValidateSetAttributePropertiesAsArray
            {
                StringPropertyWithValidateSet = "valid-set-item"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void PropertyWithValidSetItemAsSingleStringReturnsTrue()
        {
            // Arrange
            var sut = new ClassWithNonRequiredValidateSetAttributePropertiesAsSingleString
            {
                StringPropertyWithValidateSet = "valid-set-item"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void ClassWithDuplicateReturnsTrue()
        {
            // Arrange
            var sut = new ClassWithNonRequiredValidateSetAttributePropertiesWithDuplicateItems()
            {
                StringPropertyWithValidateSet = "duplicate-set-item"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void RequiredPropertyWithValidSetItemReturnsTrue()
        {
            // Arrange
            var sut = new ClassWithRequiredValidateSetAttributePropertiesAsArray
            {
                StringPropertyWithValidateSet = "valid-set-item"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void RequiredPropertyWithValidSetItemReturnsFalse()
        {
            // Arrange
            var sut = new ClassWithRequiredValidateSetAttributePropertiesAsArray
            {
                StringPropertyWithValidateSet = "invalid-set-item"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RequiredPropertyWithValidSetItemReturnsFalseOnNullProperty()
        {
            // Arrange
            var sut = new ClassWithRequiredValidateSetAttributePropertiesAsArray();

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void ValidatingPropertyFromEnumSucceeds()
        {
            // Arrange
            var sut = new ClassWithValidateSetEnum
            {
                Property = EnumsToBeValidated.Value2.ToString()
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void ValidatingPropertyFromEnumFails()
        {
            // Arrange
            var sut = new ClassWithValidateSetEnum
            {
                Property = "invalid-enum-value"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void ValidatingPropertyFromEnumWithInvalidEnumTypeFails()
        {
            // Arrange
            var sut = new ClassWithInvalidValidateSetEnum
            {
                Property = "arbitrary-enum-value"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }
        
    }
}
