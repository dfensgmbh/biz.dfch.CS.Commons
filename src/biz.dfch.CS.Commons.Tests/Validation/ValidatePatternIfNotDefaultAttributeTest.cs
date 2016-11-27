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
using System.ComponentModel.DataAnnotations;
using biz.dfch.CS.Commons.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Validation
{
    [TestClass]
    public class ValidatePatternIfNotDefaultAttributeTest
    {
        private const string PATTERN_FOR_CLASS_WITH_VALIDATE_PATTERN_ATTRIBUTE_PROPERTIES = "\\d+";
        public class ClassWithNonRequiredValidatePatternAttributeProperties : BaseDto
        {
            [ValidatePatternIfNotDefault(PATTERN_FOR_CLASS_WITH_VALIDATE_PATTERN_ATTRIBUTE_PROPERTIES)]
            public string StringPropertyWithValidatePatternThatOnlyAccecptDigits { get; set; }
        }

        private const string PATTERN_FOR_CLASS_WITH_VALIDATE_PATTERN_ATTRIBUTE_PROPERTIES_AND_INVALID_REGEX =
            "invalid-regex-with-missing-closing-bracket-(";
        public class ClassWithValidatePatternAttributePropertiesAndInvalidRegex : BaseDto
        {
            [ValidatePatternIfNotDefault(PATTERN_FOR_CLASS_WITH_VALIDATE_PATTERN_ATTRIBUTE_PROPERTIES_AND_INVALID_REGEX)]
            public string StringPropertyWithValidatePatternAndInvalidRegex { get; set; }
        }

        public class ClassWithRequiredValidatePatternAttributeProperties : BaseDto
        {
            [Required]
            [ValidatePatternIfNotDefault(PATTERN_FOR_CLASS_WITH_VALIDATE_PATTERN_ATTRIBUTE_PROPERTIES)]
            public string StringPropertyWithValidatePatternThatOnlyAccecptDigits { get; set; }
        }

        [TestMethod]
        public void NonRequiredValidatePatternReturnsTrue()
        {
            // Arrange
            var sut = new ClassWithNonRequiredValidatePatternAttributeProperties
            {
                StringPropertyWithValidatePatternThatOnlyAccecptDigits = "1234567890"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NonRequiredValidatePatternReturnsFalse()
        {
            // Arrange
            var sut = new ClassWithNonRequiredValidatePatternAttributeProperties
            {
                StringPropertyWithValidatePatternThatOnlyAccecptDigits = "invalid-contents-for-regex-pattern"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void NonRequiredValidatePatternReturnsTrueOnNullProperty()
        {
            // Arrange
            var sut = new ClassWithNonRequiredValidatePatternAttributeProperties();

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidatePatternReturnsRequiredPatternWithErrorMessage()
        {
            // Arrange
            var sut = new ClassWithNonRequiredValidatePatternAttributeProperties
            {
                StringPropertyWithValidatePatternThatOnlyAccecptDigits = "invalid-contents-for-regex-pattern"
            };
            var isValid = sut.IsValid();
            Assert.IsFalse(isValid);

            // Act
            var results = sut.GetValidationResults();

            // Assert
            Assert.AreEqual(1, results.Count);
            var errorMessage = results[0].ErrorMessage;
            Assert.IsTrue(!string.IsNullOrWhiteSpace(errorMessage));
            
            Assert.IsTrue(errorMessage.Contains(PATTERN_FOR_CLASS_WITH_VALIDATE_PATTERN_ATTRIBUTE_PROPERTIES), errorMessage);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ValidatePatternWithInvalidRegexThrowsArgumentException()
        {
            // Arrange
            var sut = new ClassWithValidatePatternAttributePropertiesAndInvalidRegex
            {
                StringPropertyWithValidatePatternAndInvalidRegex = "arbitrary-content-as-regex-pattern-is-invalid-anyway"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            // N/A
        }

        [TestMethod]
        public void RequiredValidatePatternReturnsTrue()
        {
            // Arrange
            var sut = new ClassWithRequiredValidatePatternAttributeProperties
            {
                StringPropertyWithValidatePatternThatOnlyAccecptDigits = "1234567890"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RequiredValidatePatternReturnsFalse()
        {
            // Arrange
            var sut = new ClassWithRequiredValidatePatternAttributeProperties
            {
                StringPropertyWithValidatePatternThatOnlyAccecptDigits = "invalid-contents-for-regex-pattern"
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RequiredValidatePatternReturnsFalseOnNullProperty()
        {
            // Arrange
            var sut = new ClassWithRequiredValidatePatternAttributeProperties();

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }

    }
}
