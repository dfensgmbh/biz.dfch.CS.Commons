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

using biz.dfch.CS.Commons.Converters;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Converters
{
    [TestClass]
    public class DictionaryParametersKeyConverterTest
    {
        public const string StringPropertyWithAnnotationAnnotation = "biz.dfch.CS.Commons.DictionaryParametersKeyAttribute.StringPropertyWithAnnotation";
        public const string LongPropertyWithAnnotationAnnotation = "biz.dfch.CS.Commons.DictionaryParametersKeyAttribute.LongPropertyWithAnnotation";

        public class ClassWithDictionaryParametersKeyAttributes : DictionaryParametersBaseDto
        {
            [DictionaryParametersKey(StringPropertyWithAnnotationAnnotation)]
            public string StringPropertyWithAnnotation { get; set; }

            [DictionaryParametersKey(LongPropertyWithAnnotationAnnotation)]
            public long LongPropertyWithAnnotation { get; set; }

            public string StringPropertyWithoutAnnotation { get; set; }

            public long LongPropertyWithoutAnnotation { get; set; }
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConvertDictionaryParametersKeyBaseDtoWithNullDictionaryParametersKeyBaseDtoThrowsContractFailure()
        {
            // Arrange
            var ConversionKeyBaseDto = default(DictionaryParametersBaseDto);
            var sut = new DictionaryParametersConverter();

            // Act
            var result = DictionaryParametersConverter.Convert(ConversionKeyBaseDto);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConvertDictionaryParametersKeyBaseDtoWithNullDictionaryParametersKeyBaseDtoThrowsContractFailure2()
        {
            // Arrange
            var dictionaryParameters = default(DictionaryParameters);
            var sut = new DictionaryParametersConverter();

            // Act
            var result = DictionaryParametersConverter.Convert<ClassWithDictionaryParametersKeyAttributes>(dictionaryParameters);

            // Assert
            // N/A
        }

        [TestMethod]
        public void ConvertFromDictionaryParametersSucceeds()
        {
            // Arrange
            var extraDictionaryParametersKeyName = "biz.dfch.CS.Commons.DictionaryParametersKey.ThatIsNotDefinedInTheTargetDataTransferObjectAndWillBeIgnoredOnConversion";

            var parameters = new DictionaryParameters
            {
                {StringPropertyWithAnnotationAnnotation, "arbitrary-string-value"},
                {LongPropertyWithAnnotationAnnotation, 42L},
                {extraDictionaryParametersKeyName, "some-other-arbitrary-string-value"}
            };

            // Act
            var result = DictionaryParametersConverter.Convert<ClassWithDictionaryParametersKeyAttributes>(parameters);

            // Assert
            Assert.IsNotNull(result);
            if(!result.IsValid())
            {
                var validationResults = result.GetValidationResults();
                foreach(var validationResult in validationResults)
                {
                    System.Diagnostics.Debug.WriteLine(validationResult.ErrorMessage);
                }
            }
            Assert.IsTrue(result.IsValid());

            Assert.AreEqual("arbitrary-string-value", result.StringPropertyWithAnnotation);
            Assert.AreEqual(42L, result.LongPropertyWithAnnotation);
            Assert.IsNull(result.StringPropertyWithoutAnnotation);
            Assert.AreEqual(0, result.LongPropertyWithoutAnnotation);
        }

        [TestMethod]
        public void ConvertToDictionaryParametersSucceeds()
        {
            // Arrange
            var sut = new ClassWithDictionaryParametersKeyAttributes
            {
                StringPropertyWithAnnotation = "arbitrary-StringPropertyWithAnnotation",
                LongPropertyWithAnnotation = 42L,
                StringPropertyWithoutAnnotation = "arbitrary-StringPropertyWithoutAnnotation",
                LongPropertyWithoutAnnotation = 8L
            };

            // Act
            var result = DictionaryParametersConverter.Convert(sut);

            // Assert
            Assert.IsNotNull(result);
            if(!result.IsValid())
            {
                var validationResults = result.GetValidationResults();
                foreach(var validationResult in validationResults)
                {
                    System.Diagnostics.Debug.WriteLine(validationResult.ErrorMessage);
                }
            }
            Assert.IsTrue(result.IsValid());

            Assert.AreEqual(2, result.Keys.Count);

            Assert.IsTrue(result.ContainsKey(StringPropertyWithAnnotationAnnotation));
            Assert.AreEqual("arbitrary-StringPropertyWithAnnotation", result[StringPropertyWithAnnotationAnnotation]);
            Assert.IsTrue(result.ContainsKey(LongPropertyWithAnnotationAnnotation));
            Assert.AreEqual(42L, result[LongPropertyWithAnnotationAnnotation]);
        }
    }
}
