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
using biz.dfch.CS.Commons.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Converters
{
    [TestClass]
    public class ConvertibleBaseDtoConverterTest
    {
        public const string DICTIONARY_PARAMETERS_KEY_STRINGPROPERTY = "biz.dfch.CS.Commons.Public.DictionaryParametersKey.StringProperty";
        public const string DICTIONARY_PARAMETERS_KEY_LONGPROPERTY = "biz.dfch.CS.Commons.Public.DictionaryParametersKey.LongProperty";
        
        public const string CONVERSION_KEY_STRINGPROPERTY = "biz.dfch.CS.Commons.Public.ConversionKey.StringProperty";
        public const string CONVERSION_KEY_LONGPROPERTY = "biz.dfch.CS.Commons.Public.ConversionKey.LongProperty";
        
        public readonly string ExpectedStringPropertyDictionaryParametersKey = "arbitrary-string";
        public readonly long ExpectedLongPropertyDictionaryParametersKey = 42L;

        public readonly string ExpectedStringPropertyConversionKey = "arbitrary-string";
        public readonly long ExpectedLongPropertyConversionKey = 42L;

        public readonly string ExpectedStringPropertyNoAttribute = "StringPropertyNoAttribute";
        public readonly long ExpectedLongPropertyNoAttribute = 42L;

        public readonly string ExpectedStringPropertyRequired = "arbitrary-StringPropertyRequired";
        public readonly long ExpectedLongPropertyRequired = 5L;

        public class DictionaryParametersDtoImpl : DictionaryParametersBaseDto
        {
            [Required]
            [DictionaryParametersKey(DICTIONARY_PARAMETERS_KEY_STRINGPROPERTY)]
            public string StringPropertyDictionaryParametersKey { get; set; }

            [DictionaryParametersKey(DICTIONARY_PARAMETERS_KEY_LONGPROPERTY)]
            public long LongPropertyDictionaryParametersKey { get; set; }

            [ArbitraryConversionTest(CONVERSION_KEY_STRINGPROPERTY)]
            public string StringPropertyConversionKey { get; set; }

            [ArbitraryConversionTest(CONVERSION_KEY_LONGPROPERTY)]
            public long LongPropertyConversionKey { get; set; }

            [Required]
            public string StringPropertyRequired { get; set; }

            [Required]
            public long LongPropertyRequired { get; set; }

            public string StringPropertyNoAttribute { get; set; }

            public long LongPropertyNoAttribute { get; set; }

        }

        [TestMethod]
        public void ConvertFromDictionaryParametersKeyAttributeSucceeds()
        {
            // Arrange
            var parameters = new DictionaryParameters()
            {
                { DICTIONARY_PARAMETERS_KEY_STRINGPROPERTY, ExpectedStringPropertyDictionaryParametersKey }
                ,
                { DICTIONARY_PARAMETERS_KEY_LONGPROPERTY, ExpectedLongPropertyDictionaryParametersKey }
                ,
                { CONVERSION_KEY_STRINGPROPERTY, ExpectedStringPropertyConversionKey }
                ,
                { CONVERSION_KEY_LONGPROPERTY, ExpectedLongPropertyConversionKey }
                ,
                { "StringPropertyNoAttribute", ExpectedStringPropertyNoAttribute }
                ,
                { "LongPropertyNoAttribute", ExpectedLongPropertyNoAttribute }
                ,
                { "StringPropertyRequired", ExpectedStringPropertyRequired }
                ,
                { "LongPropertyRequired", ExpectedLongPropertyRequired }
            };
            var sut = new ConvertibleBaseDtoConverter();

            // Act
            var result = sut.Convert<DictionaryParametersDtoImpl, DictionaryParametersKeyAttribute>(parameters, false);

            // Assert
            Assert.AreEqual(ExpectedStringPropertyDictionaryParametersKey, result.StringPropertyDictionaryParametersKey);
            Assert.AreEqual(ExpectedLongPropertyDictionaryParametersKey, result.LongPropertyDictionaryParametersKey);

            Assert.AreNotEqual(ExpectedStringPropertyConversionKey, result.StringPropertyConversionKey);
            Assert.AreNotEqual(ExpectedLongPropertyConversionKey, result.LongPropertyConversionKey);
            
            Assert.AreNotEqual(ExpectedStringPropertyNoAttribute, result.StringPropertyNoAttribute);
            Assert.AreNotEqual(ExpectedLongPropertyNoAttribute, result.LongPropertyNoAttribute);
            
            Assert.AreNotEqual(ExpectedStringPropertyRequired, result.StringPropertyRequired);
            Assert.AreNotEqual(ExpectedLongPropertyRequired, result.LongPropertyRequired);

            Assert.IsFalse(result.IsValid());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ConvertFromDictionaryParametersKeyAttributeWithTypeMismatchThrowsArgumentException()
        {
            // Arrange
            var parameters = new DictionaryParameters()
            {
                { DICTIONARY_PARAMETERS_KEY_STRINGPROPERTY, ExpectedStringPropertyDictionaryParametersKey }
                ,
                { DICTIONARY_PARAMETERS_KEY_LONGPROPERTY, "invalid-data-type-for-mapped-proeprty" }
                ,
                { CONVERSION_KEY_STRINGPROPERTY, ExpectedStringPropertyConversionKey }
                ,
                { CONVERSION_KEY_LONGPROPERTY, ExpectedLongPropertyConversionKey }
                ,
                { "StringPropertyNoAttribute", ExpectedStringPropertyNoAttribute }
                ,
                { "LongPropertyNoAttribute", ExpectedLongPropertyNoAttribute }
                ,
                { "StringPropertyRequired", ExpectedStringPropertyRequired }
                ,
                { "LongPropertyRequired", ExpectedLongPropertyRequired }
            };
            var sut = new ConvertibleBaseDtoConverter();

            // Act
            var result = sut.Convert<DictionaryParametersDtoImpl, DictionaryParametersKeyAttribute>(parameters, false);

            // Assert
            // N/A
        }

        [TestMethod]
        public void ConvertFromDictionaryParametersKeyAttributeIncludeAllPropertiesSucceeds()
        {
            // Arrange
            var parameters = new DictionaryParameters()
            {
                { DICTIONARY_PARAMETERS_KEY_STRINGPROPERTY, ExpectedStringPropertyDictionaryParametersKey }
                ,
                { DICTIONARY_PARAMETERS_KEY_LONGPROPERTY, ExpectedLongPropertyDictionaryParametersKey }
                ,
                { CONVERSION_KEY_STRINGPROPERTY, ExpectedStringPropertyConversionKey }
                ,
                { CONVERSION_KEY_LONGPROPERTY, ExpectedLongPropertyConversionKey }
                ,
                { "StringPropertyNoAttribute", ExpectedStringPropertyNoAttribute }
                ,
                { "LongPropertyNoAttribute", ExpectedLongPropertyNoAttribute }
                ,
                { "StringPropertyRequired", ExpectedStringPropertyRequired }
                ,
                { "LongPropertyRequired", ExpectedLongPropertyRequired }
            };
            var sut = new ConvertibleBaseDtoConverter();

            // Act
            var result = sut.Convert<DictionaryParametersDtoImpl, DictionaryParametersKeyAttribute>(parameters, true);

            // Assert
            Assert.AreEqual(ExpectedStringPropertyDictionaryParametersKey, result.StringPropertyDictionaryParametersKey);
            Assert.AreEqual(ExpectedLongPropertyDictionaryParametersKey, result.LongPropertyDictionaryParametersKey);
            
            Assert.AreNotEqual(ExpectedStringPropertyConversionKey, result.StringPropertyConversionKey);
            Assert.AreNotEqual(ExpectedLongPropertyConversionKey, result.LongPropertyConversionKey);
            
            Assert.AreEqual(ExpectedStringPropertyNoAttribute, result.StringPropertyNoAttribute);
            Assert.AreEqual(ExpectedLongPropertyNoAttribute, result.LongPropertyNoAttribute);
            
            Assert.AreEqual(ExpectedStringPropertyRequired, result.StringPropertyRequired);
            Assert.AreEqual(ExpectedLongPropertyRequired, result.LongPropertyRequired);

            Assert.IsTrue(result.IsValid());
        }

        [TestMethod]
        public void ConvertViaDictionaryParametersKeyAttributeIncludeAllPropertiesSucceeds()
        {
            // Arrange
            var dto = new DictionaryParametersDtoImpl()
            {
                StringPropertyDictionaryParametersKey = ExpectedStringPropertyDictionaryParametersKey
                ,
                LongPropertyDictionaryParametersKey = ExpectedLongPropertyDictionaryParametersKey
                ,
                StringPropertyNoAttribute = ExpectedStringPropertyNoAttribute
            };

            var sut = new ConvertibleBaseDtoConverter();

            // Act
            var result = sut.Convert<DictionaryParametersKeyAttribute>(dto, true);

            // Assert
            Assert.IsTrue(result.ContainsKey(DICTIONARY_PARAMETERS_KEY_STRINGPROPERTY));
            Assert.AreEqual(ExpectedStringPropertyDictionaryParametersKey, result[DICTIONARY_PARAMETERS_KEY_STRINGPROPERTY]);

            Assert.IsTrue(result.ContainsKey(DICTIONARY_PARAMETERS_KEY_LONGPROPERTY));
            Assert.AreEqual(ExpectedLongPropertyDictionaryParametersKey, result[DICTIONARY_PARAMETERS_KEY_LONGPROPERTY]);
            
            Assert.IsTrue(result.ContainsKey("StringPropertyNoAttribute"));
            Assert.AreEqual(ExpectedStringPropertyNoAttribute, result["StringPropertyNoAttribute"]);

            Assert.AreEqual(8, result.Keys.Count);

            Assert.IsTrue(result.IsValid());
        }

        public class DictionaryParametersDtoImplWithDuplicateKey : DictionaryParametersBaseDto
        {
            [Required]
            [DictionaryParametersKey("StringPropertyNoAttribute")]
            public string StringPropertyDictionaryParametersKey { get; set; }

            [DictionaryParametersKey(DICTIONARY_PARAMETERS_KEY_LONGPROPERTY)]
            public long LongPropertyDictionaryParametersKey { get; set; }

            [ArbitraryConversionTest(CONVERSION_KEY_STRINGPROPERTY)]
            public string StringPropertyConversionKey { get; set; }

            [ArbitraryConversionTest(CONVERSION_KEY_LONGPROPERTY)]
            public long LongPropertyConversionKey { get; set; }

            [Required]
            public string StringPropertyRequired { get; set; }

            [Required]
            public long LongPropertyRequired { get; set; }

            public string StringPropertyNoAttribute { get; set; }

            public long LongPropertyNoAttribute { get; set; }

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ConvertViaDictionaryParametersKeyAttributeWithDuplicateKeysThrowsArgumentException()
        {
            // Arrange
            var dto = new DictionaryParametersDtoImplWithDuplicateKey()
            {
                StringPropertyDictionaryParametersKey = ExpectedStringPropertyDictionaryParametersKey
                ,
                LongPropertyDictionaryParametersKey = ExpectedLongPropertyDictionaryParametersKey
                ,
                StringPropertyNoAttribute = ExpectedStringPropertyNoAttribute
            };

            var sut = new ConvertibleBaseDtoConverter();

            // Act
            var result = sut.Convert<DictionaryParametersKeyAttribute>(dto, true);

            // Assert
            // N/A
        }
        
        [TestMethod]
        public void ConvertViaDictionaryParametersKeyAttributeSucceeds()
        {
            // Arrange
            var dto = new DictionaryParametersDtoImpl()
            {
                StringPropertyDictionaryParametersKey = ExpectedStringPropertyDictionaryParametersKey
                ,
                LongPropertyDictionaryParametersKey = ExpectedLongPropertyDictionaryParametersKey
                ,
                StringPropertyNoAttribute = ExpectedStringPropertyNoAttribute
            };

            var sut = new ConvertibleBaseDtoConverter();

            // Act
            var result = sut.Convert<DictionaryParametersKeyAttribute>(dto, false);

            // Assert
            Assert.IsTrue(result.ContainsKey(DICTIONARY_PARAMETERS_KEY_STRINGPROPERTY));
            Assert.AreEqual(ExpectedStringPropertyDictionaryParametersKey, result[DICTIONARY_PARAMETERS_KEY_STRINGPROPERTY]);

            Assert.IsTrue(result.ContainsKey(DICTIONARY_PARAMETERS_KEY_LONGPROPERTY));
            Assert.AreEqual(ExpectedLongPropertyDictionaryParametersKey, result[DICTIONARY_PARAMETERS_KEY_LONGPROPERTY]);
            
            Assert.IsFalse(result.ContainsKey("StringPropertyNoAttribute"));

            Assert.AreEqual(2, result.Keys.Count);

            Assert.IsTrue(result.IsValid());
        }
        
        [TestMethod]
        public void ConvertViaConversionKeyAttributeAndMergeWithDictionaryParametersKeyAttributeSucceeds()
        {
            // Arrange
            var dto = new DictionaryParametersDtoImpl()
            {
                StringPropertyDictionaryParametersKey = ExpectedStringPropertyDictionaryParametersKey
                ,
                LongPropertyDictionaryParametersKey = ExpectedLongPropertyDictionaryParametersKey
                ,
                StringPropertyNoAttribute = ExpectedStringPropertyNoAttribute
                ,
                StringPropertyConversionKey = ExpectedStringPropertyConversionKey
                ,
                LongPropertyConversionKey = ExpectedLongPropertyConversionKey
            };

            var sut = new ConvertibleBaseDtoConverter();

            // Act
            var resultFromConversionKeyAttribute = sut.Convert<ArbitraryConversionTestAttribute>(dto, false);
            var resultFromDictionaryParametersKeyAttribute = sut.Convert<DictionaryParametersKeyAttribute>(dto, false);

            // Assert
            Assert.IsTrue(resultFromConversionKeyAttribute.ContainsKey(CONVERSION_KEY_STRINGPROPERTY));
            Assert.AreEqual(ExpectedStringPropertyConversionKey, resultFromConversionKeyAttribute[CONVERSION_KEY_STRINGPROPERTY]);
            
            Assert.IsTrue(resultFromConversionKeyAttribute.ContainsKey(CONVERSION_KEY_LONGPROPERTY));
            Assert.AreEqual(ExpectedLongPropertyConversionKey, resultFromConversionKeyAttribute[CONVERSION_KEY_LONGPROPERTY]);

            Assert.IsTrue(resultFromConversionKeyAttribute.IsValid());

            Assert.IsTrue(resultFromDictionaryParametersKeyAttribute.ContainsKey(DICTIONARY_PARAMETERS_KEY_STRINGPROPERTY));
            Assert.AreEqual(ExpectedStringPropertyDictionaryParametersKey, resultFromDictionaryParametersKeyAttribute[DICTIONARY_PARAMETERS_KEY_STRINGPROPERTY]);

            Assert.IsTrue(resultFromDictionaryParametersKeyAttribute.ContainsKey(DICTIONARY_PARAMETERS_KEY_LONGPROPERTY));
            Assert.AreEqual(ExpectedLongPropertyDictionaryParametersKey, resultFromDictionaryParametersKeyAttribute[DICTIONARY_PARAMETERS_KEY_LONGPROPERTY]);
            
            Assert.IsFalse(resultFromDictionaryParametersKeyAttribute.ContainsKey("StringPropertyNoAttribute"));

            Assert.AreEqual(2, resultFromDictionaryParametersKeyAttribute.Keys.Count);

            Assert.IsTrue(resultFromDictionaryParametersKeyAttribute.IsValid());

            var isMergeable = resultFromConversionKeyAttribute.CanAdd(resultFromDictionaryParametersKeyAttribute);
            Assert.IsTrue(isMergeable);

            var keys1 = resultFromDictionaryParametersKeyAttribute.Keys.Count;
            var keys2 = resultFromConversionKeyAttribute.Keys.Count;

            var mergedDictionaryParameters = resultFromConversionKeyAttribute.Add(resultFromDictionaryParametersKeyAttribute);
            Assert.AreEqual(keys1 + keys2, mergedDictionaryParameters.Keys.Count);
        }
    }
}
