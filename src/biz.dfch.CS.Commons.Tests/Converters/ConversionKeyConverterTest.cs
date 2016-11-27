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
using biz.dfch.CS.Commons.Converters;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Converters
{
    [TestClass]
    public class ConversionKeyConverterTest
    {
        public const string STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION = "biz.dfch.CS.Commons.ConversionKeyAttribute.StringPropertyWithAnnotation";
        public const string LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION = "biz.dfch.CS.Commons.ConversionKeyAttribute.LongPropertyWithAnnotation";

        public class ClassWithConversionKeyAttributes : ConversionKeyBaseDto
        {
            [ConversionKey(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            [DictionaryParametersKey(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public string StringPropertyWithAnnotation { get; set; }

            [ConversionKey(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public long LongPropertyWithAnnotation { get; set; }

            public string StringPropertyWithoutAnnotation { get; set; }

            public long LongPropertyWithoutAnnotation { get; set; }
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConvertConversionKeyBaseDtoWithNullConversionKeyBaseDtoThrowsContractFailure()
        {
            // Arrange
            var ConversionKeyBaseDto = default(ConversionKeyBaseDto);
            var sut = new ConversionKeyConverter();

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var result = ConversionKeyConverter.Convert(ConversionKeyBaseDto);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConvertConversionKeyBaseDtoWithNullConversionKeyBaseDtoThrowsContractFailure2()
        {
            // Arrange
            var dictionaryParameters = default(DictionaryParameters);
            var sut = new ConversionKeyConverter();

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var result = ConversionKeyConverter.Convert<ClassWithConversionKeyAttributes>(dictionaryParameters);

            // Assert
            // N/A
        }

        [TestMethod]
        public void ConvertFromDictionaryParametersSucceeds()
        {
            // Arrange
            var extraConversionKeyName = "biz.dfch.CS.Commons.ConversionKey.ThatIsNotDefinedInTheTargetDataTransferObjectAndWillBeIgnoredOnConversion";

            var parameters = new DictionaryParameters
            {
                { STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, "arbitrary-string-value" }
                ,
                { LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, 42L }
                ,
                { extraConversionKeyName, "some-other-arbitrary-string-value" }
            };

            // Act
            var result = ConversionKeyConverter.Convert<ClassWithConversionKeyAttributes>(parameters);

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
        public void ConvertFromDictionaryParametersWithTypeConversionSucceeds()
        {
            // Arrange
            var parameters = new DictionaryParameters
            {
                { STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, "arbitrary-string-value" }
                ,
                { LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, 42L.ToString() }
            };

            // Act
            var result = ConversionKeyConverter.Convert<ClassWithConversionKeyAttributes>(parameters);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid());

            Assert.AreEqual("arbitrary-string-value", result.StringPropertyWithAnnotation);
            Assert.AreEqual(42L, result.LongPropertyWithAnnotation);
        }

        [TestMethod]
        public void ConvertToDictionaryParametersSucceeds()
        {
            // Arrange
            var sut = new ClassWithConversionKeyAttributes()
            {
                StringPropertyWithAnnotation = "arbitrary-StringPropertyWithAnnotation",
                LongPropertyWithAnnotation = 42L,
                StringPropertyWithoutAnnotation = "arbitrary-StringPropertyWithoutAnnotation",
                LongPropertyWithoutAnnotation = 8L
            };

            // Act
            var result = ConversionKeyConverter.Convert(sut);

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

            Assert.IsTrue(result.ContainsKey(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION));
            Assert.AreEqual("arbitrary-StringPropertyWithAnnotation", result[STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION]);
            Assert.IsTrue(result.ContainsKey(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION));
            Assert.AreEqual(42L, result[LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION]);
        }

        public const string STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE1 = "arbitrary-conversion-key-name1-type1";
        public const string LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE1 = "arbitrary-conversion-key-name2-type1";
        public const string STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE2 = "arbitrary-conversion-key-name1-type2";
        public const string LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE2 = "arbitrary-conversion-key-name2-type2";

        public class ClassWithConversionKeyAttributesAsTBase : ConversionKeyBaseDto
        {
            [ConversionKey(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public virtual string StringPropertyWithAnnotation { get; set; }

            [ConversionKey(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public virtual long LongPropertyWithAnnotation { get; set; }
        }

        public class ClassWithConversionKeyAttributesAsDerivedType1 : ClassWithConversionKeyAttributesAsTBase
        {
            [ConversionKey(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE1)]
            public override string StringPropertyWithAnnotation { get; set; }

            [ConversionKey(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE1)]
            public override long LongPropertyWithAnnotation { get; set; }
        }

        public class ClassWithConversionKeyAttributesAsDerivedType2 : ClassWithConversionKeyAttributesAsTBase
        {
            [ConversionKey(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE2)]
            public override string StringPropertyWithAnnotation { get; set; }

            [ConversionKey(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE2)]
            public override long LongPropertyWithAnnotation { get; set; }
        }

        [TestMethod]
        public void ConvertingViaBaseDtoSucceeds()
        {
            // Arrange
            const string name = "arbitrary-name";
            const string description = "optional-description";
            const string size = "40";

            var StringPropertyWithAnnotationAnnotationValue = "arbitrary-string";
            var LongPropertyWithAnnotationAnnotationValue = 42L;
            var StringPropertyWithAnnotationAnnotationType1Value = "arbitrary-string-1";
            var LongPropertyWithAnnotationAnnotationType1Value = 1L;
            var StringPropertyWithAnnotationAnnotationType2Value = "arbitrary-string-2";
            var LongPropertyWithAnnotationAnnotationType2Value = 2L;

            var dictionaryParameters = new DictionaryParameters()
            {
                { STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, StringPropertyWithAnnotationAnnotationValue }
                ,
                { LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, LongPropertyWithAnnotationAnnotationValue}
                ,
                { STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE1, StringPropertyWithAnnotationAnnotationType1Value}
                ,
                { LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE1, LongPropertyWithAnnotationAnnotationType1Value }
                ,
                { STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE2, StringPropertyWithAnnotationAnnotationType2Value}
                ,
                { LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE2, LongPropertyWithAnnotationAnnotationType2Value }
                ,
                { "arbitrary-other-key", "arbitrary-value" }
            };

            // Act
            var result = ConversionKeyConverter.Convert<ClassWithConversionKeyAttributesAsTBase>(dictionaryParameters, null);

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);

            foreach (var dto in result)
            {
                Assert.IsTrue(dto.IsValid(), dto.GetType().FullName);
            }

            Assert.IsTrue(result[0] is ClassWithConversionKeyAttributesAsDerivedType1 || result[1] is ClassWithConversionKeyAttributesAsDerivedType1);
            Assert.IsTrue(result[0] is ClassWithConversionKeyAttributesAsDerivedType2 || result[1] is ClassWithConversionKeyAttributesAsDerivedType2);

            var type1 = result[0] is ClassWithConversionKeyAttributesAsDerivedType1 ? result[0] : result[1];
            Assert.IsNotNull(type1);
            var type2 = result[0] is ClassWithConversionKeyAttributesAsDerivedType2 ? result[0] : result[1];
            Assert.IsNotNull(type2);

            Assert.AreEqual(StringPropertyWithAnnotationAnnotationType1Value, type1.StringPropertyWithAnnotation);
            Assert.AreEqual(LongPropertyWithAnnotationAnnotationType1Value, type1.LongPropertyWithAnnotation);

            Assert.AreEqual(StringPropertyWithAnnotationAnnotationType2Value, type2.StringPropertyWithAnnotation);
            Assert.AreEqual(LongPropertyWithAnnotationAnnotationType2Value, type2.LongPropertyWithAnnotation);
        }

        [TestMethod]
        public void ConverterTestWithTypeInsteadOfGenericSucceeds()
        {
            // Arrange
            const string extraConversionKeyName = "biz.dfch.CS.Commons.ConversionKey.ThatIsNotDefinedInTheTargetDataTransferObjectAndWillBeIgnoredOnConversion";

            var parameters = new DictionaryParameters
            {
                { STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, "arbitrary-string-value" }
                ,
                { LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, 42L }
                ,
                { extraConversionKeyName, "some-other-arbitrary-string-value" }
            };

            // Act
            var resultBase = ConversionKeyConverter.Convert(typeof(ClassWithConversionKeyAttributes), parameters);

            // Assert
            Assert.IsTrue(resultBase is ClassWithConversionKeyAttributes);
            var result = resultBase as ClassWithConversionKeyAttributes;
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

        public class ConversionKeyClass : ConversionKeyBaseDto
        {
            public const string BAG_NAME_INT = "IntProperty";
            public const string BAG_NAME_LONG = "LongProperty";
            public const string BAG_NAME_DOUBLE = "DoubleProperty";
            
            [ConversionKey(BAG_NAME_INT)]
            public int IntProperty { get; set; }
            
            [ConversionKey(BAG_NAME_LONG)]
            public long LongProperty { get; set; }
            
            [ConversionKey(BAG_NAME_DOUBLE)]
            public double DoubleProperty { get; set; }
        }

        [TestMethod]
        public void ConverterTestWithMatchingTypesSucceeds()
        {
            var sut = new DictionaryParameters()
            {
                {ConversionKeyClass.BAG_NAME_INT, 42}
                ,
                {ConversionKeyClass.BAG_NAME_LONG, int.MaxValue + 1L}
                ,
                {ConversionKeyClass.BAG_NAME_DOUBLE, 4.2d}
            };

            var result = ConversionKeyConverter.Convert<ConversionKeyClass>(sut);

            Assert.IsTrue(result.IsValid());
        }

        [TestMethod]
        public void ConverterTestWithStringTypesCanBeConvertedAndSucceeds()
        {
            var sut = new DictionaryParameters()
            {
                {ConversionKeyClass.BAG_NAME_INT, "42"}
                ,
                {ConversionKeyClass.BAG_NAME_LONG, (int.MaxValue + 1L).ToString()}
                ,
                {ConversionKeyClass.BAG_NAME_DOUBLE, 4.2d.ToString()}
            };

            var result = ConversionKeyConverter.Convert<ConversionKeyClass>(sut);

            Assert.IsTrue(result.IsValid());
        }

        public class MyConversionKeyBaseDto : ConversionKeyBaseDto
        {
            public const string NAME_PROPERTY_NAME = "net.sharedop.MyConversionKeyBaseDto.Name";
            public const string NAME_PROPERTY_VALUE = "arbitrary-value";
            public const string VALUE_PROPERTY_NAME = "net.sharedop.MyConversionKeyBaseDto.Value";
            public const long VALUE_PROPERTY_VALUE = 8;

            [ConversionKey(NAME_PROPERTY_NAME)]
            [Required]
            public string StringProperty { get; set; }
            
            [ConversionKey(VALUE_PROPERTY_NAME)]
            [Range(8, 15)]
            public long LongProperty { get; set; }
        }

        [TestMethod]
        public void ConvertingConversionKeyBaseDtoToDictionaryParametersSucceeds()
        {
            var sut = new MyConversionKeyBaseDto()
            {
                StringProperty = MyConversionKeyBaseDto.NAME_PROPERTY_VALUE
                ,
                LongProperty = MyConversionKeyBaseDto.VALUE_PROPERTY_VALUE
            };
            Assert.IsTrue(sut.IsValid());

            var parameters = ConversionKeyConverter.Convert(sut);
            Assert.IsNotNull(parameters);
            Assert.AreEqual(2, parameters.Count);
            Assert.IsTrue(parameters.ContainsKey(MyConversionKeyBaseDto.NAME_PROPERTY_NAME));
            Assert.AreEqual(MyConversionKeyBaseDto.NAME_PROPERTY_VALUE, parameters[MyConversionKeyBaseDto.NAME_PROPERTY_NAME]);
            Assert.IsTrue(parameters.ContainsKey(MyConversionKeyBaseDto.VALUE_PROPERTY_NAME));
            Assert.AreEqual(MyConversionKeyBaseDto.VALUE_PROPERTY_VALUE, parameters[MyConversionKeyBaseDto.VALUE_PROPERTY_NAME]);
        }
            
    }
}
