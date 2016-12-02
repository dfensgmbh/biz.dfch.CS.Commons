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
    public class EnvironmentVariableConverterTest
    {
        public const string STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION = "biz.dfch.CS.Commons.EnvironmentVariableAttribute.StringPropertyWithAnnotation";
        public const string LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION = "biz.dfch.CS.Commons.EnvironmentVariableAttribute.LongPropertyWithAnnotation";

        public class ClassWithEnvironmentVariableAttributes : EnvironmentVariableBaseDto
        {
            [EnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            [DictionaryParametersKey(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public string StringPropertyWithAnnotation { get; set; }

            [EnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public long LongPropertyWithAnnotation { get; set; }

            public string StringPropertyWithoutAnnotation { get; set; }

            public long LongPropertyWithoutAnnotation { get; set; }
        }

        public class ClassWithStaticEnvironmentVariableAttributes : EnvironmentVariableBaseDto
        {
            [EnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            [DictionaryParametersKey(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public string StringPropertyWithAnnotation { get; set; }

            [EnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public static long StaticLongPropertyWithAnnotation { get; set; }

            public string StringPropertyWithoutAnnotation { get; set; }

            public long LongPropertyWithoutAnnotation { get; set; }
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConvertEnvironmentVariableBaseDtoWithNullEnvironmentVariableBaseDtoThrowsContractFailure()
        {
            // Arrange
            var environmentVariableBaseDto = default(EnvironmentVariableBaseDto);
            var sut = new EnvironmentVariableConverter();

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var result = EnvironmentVariableConverter.Convert(environmentVariableBaseDto);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConvertEnvironmentVariableBaseDtoWithNullEnvironmentVariableBaseDtoThrowsContractFailure2()
        {
            // Arrange
            var dictionaryParameters = default(DictionaryParameters);
            var sut = new EnvironmentVariableConverter();

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var result = EnvironmentVariableConverter.Convert<ClassWithEnvironmentVariableAttributes>(dictionaryParameters);

            // Assert
            // N/A
        }

        [TestMethod]
        public void ConvertFromDictionaryParametersSucceeds()
        {
            // Arrange
            var extraEnvironmentVariableName = "biz.dfch.CS.Commons.EnvironmentVariable.ThatIsNotDefinedInTheTargetDataTransferObjectAndWillBeIgnoredOnConversion";

            var parameters = new DictionaryParameters
            {
                { STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, "arbitrary-string-value" }
                ,
                { LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, 42L }
                ,
                { extraEnvironmentVariableName, "some-other-arbitrary-string-value" }
            };

            // Act
            var result = EnvironmentVariableConverter.Convert<ClassWithEnvironmentVariableAttributes>(parameters);

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
            var result = EnvironmentVariableConverter.Convert<ClassWithEnvironmentVariableAttributes>(parameters);

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
            var sut = new ClassWithEnvironmentVariableAttributes()
            {
                StringPropertyWithAnnotation = "arbitrary-StringPropertyWithAnnotation",
                LongPropertyWithAnnotation = 42L,
                StringPropertyWithoutAnnotation = "arbitrary-StringPropertyWithoutAnnotation",
                LongPropertyWithoutAnnotation = 8L
            };

            // Act
            var result = EnvironmentVariableConverter.Convert(sut);

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

        public class ClassWithEnvironmentVariableAttributesAsTBase : EnvironmentVariableBaseDto
        {
            [EnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public virtual string StringPropertyWithAnnotation { get; set; }

            [EnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public virtual long LongPropertyWithAnnotation { get; set; }
        }

        public class ClassWithEnvironmentVariableAttributesAsDerivedType1 : ClassWithEnvironmentVariableAttributesAsTBase
        {
            [EnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE1)]
            public override string StringPropertyWithAnnotation { get; set; }

            [EnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE1)]
            public override long LongPropertyWithAnnotation { get; set; }
        }

        public class ClassWithEnvironmentVariableAttributesAsDerivedType2 : ClassWithEnvironmentVariableAttributesAsTBase
        {
            [EnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE2)]
            public override string StringPropertyWithAnnotation { get; set; }

            [EnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION_TYPE2)]
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
            var result = EnvironmentVariableConverter.Convert<ClassWithEnvironmentVariableAttributesAsTBase>(dictionaryParameters, null);

            // Assert

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);

            foreach (var dto in result)
            {
                Assert.IsTrue(dto.IsValid(), dto.GetType().FullName);
            }

            Assert.IsTrue(result[0] is ClassWithEnvironmentVariableAttributesAsDerivedType1 || result[1] is ClassWithEnvironmentVariableAttributesAsDerivedType1);
            Assert.IsTrue(result[0] is ClassWithEnvironmentVariableAttributesAsDerivedType2 || result[1] is ClassWithEnvironmentVariableAttributesAsDerivedType2);

            var type1 = result[0] is ClassWithEnvironmentVariableAttributesAsDerivedType1 ? result[0] : result[1];
            Assert.IsNotNull(type1);
            var type2 = result[0] is ClassWithEnvironmentVariableAttributesAsDerivedType2 ? result[0] : result[1];
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
            const string extraEnvironmentVariableName = "biz.dfch.CS.Commons.EnvironmentVariable.ThatIsNotDefinedInTheTargetDataTransferObjectAndWillBeIgnoredOnConversion";

            var parameters = new DictionaryParameters
            {
                { STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, "arbitrary-string-value" }
                ,
                { LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, 42L }
                ,
                { extraEnvironmentVariableName, "some-other-arbitrary-string-value" }
            };

            // Act
            var resultBase = EnvironmentVariableConverter.Convert(typeof(ClassWithEnvironmentVariableAttributes), parameters);

            // Assert
            Assert.IsTrue(resultBase is ClassWithEnvironmentVariableAttributes);
            var result = resultBase as ClassWithEnvironmentVariableAttributes;
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

        public class EnvironmentVariableClass : EnvironmentVariableBaseDto
        {
            public const string BAG_NAME_INT = "IntProperty";
            public const string BAG_NAME_LONG = "LongProperty";
            public const string BAG_NAME_DOUBLE = "DoubleProperty";
            
            [EnvironmentVariable(BAG_NAME_INT)]
            public int IntProperty { get; set; }
            
            [EnvironmentVariable(BAG_NAME_LONG)]
            public long LongProperty { get; set; }
            
            [EnvironmentVariable(BAG_NAME_DOUBLE)]
            public double DoubleProperty { get; set; }
        }

        [TestMethod]
        public void ConverterTestWithMatchingTypesSucceeds()
        {
            var sut = new DictionaryParameters()
            {
                {EnvironmentVariableClass.BAG_NAME_INT, 42}
                ,
                {EnvironmentVariableClass.BAG_NAME_LONG, int.MaxValue + 1L}
                ,
                {EnvironmentVariableClass.BAG_NAME_DOUBLE, 4.2d}
            };

            var result = EnvironmentVariableConverter.Convert<EnvironmentVariableClass>(sut);

            Assert.IsTrue(result.IsValid());
        }

        [TestMethod]
        public void ConverterTestWithStringTypesCanBeConvertedAndSucceeds()
        {
            var sut = new DictionaryParameters()
            {
                {EnvironmentVariableClass.BAG_NAME_INT, "42"}
                ,
                {EnvironmentVariableClass.BAG_NAME_LONG, (int.MaxValue + 1L).ToString()}
                ,
                {EnvironmentVariableClass.BAG_NAME_DOUBLE, 4.2d.ToString()}
            };

            var result = EnvironmentVariableConverter.Convert<EnvironmentVariableClass>(sut);

            Assert.IsTrue(result.IsValid());
        }

        public class MyEnvironmentVariableBaseDto : EnvironmentVariableBaseDto
        {
            public const string NAME_PROPERTY_NAME = "net.sharedop.MyEnvironmentVariableBaseDto.Name";
            public const string NAME_PROPERTY_VALUE = "arbitrary-value";
            public const string VALUE_PROPERTY_NAME = "net.sharedop.MyEnvironmentVariableBaseDto.Value";
            public const long VALUE_PROPERTY_VALUE = 8;

            [EnvironmentVariable(NAME_PROPERTY_NAME)]
            [Required]
            public string StringProperty { get; set; }
            
            [EnvironmentVariable(VALUE_PROPERTY_NAME)]
            [Range(8, 15)]
            public long LongProperty { get; set; }
        }

        [TestMethod]
        public void ConvertingEnvironmentVariableBaseDtoToDictionaryParametersSucceeds()
        {
            var sut = new MyEnvironmentVariableBaseDto()
            {
                StringProperty = MyEnvironmentVariableBaseDto.NAME_PROPERTY_VALUE
                ,
                LongProperty = MyEnvironmentVariableBaseDto.VALUE_PROPERTY_VALUE
            };
            Assert.IsTrue(sut.IsValid());

            var parameters = EnvironmentVariableConverter.Convert(sut);
            Assert.IsNotNull(parameters);
            Assert.AreEqual(2, parameters.Count);
            Assert.IsTrue(parameters.ContainsKey(MyEnvironmentVariableBaseDto.NAME_PROPERTY_NAME));
            Assert.AreEqual(MyEnvironmentVariableBaseDto.NAME_PROPERTY_VALUE, parameters[MyEnvironmentVariableBaseDto.NAME_PROPERTY_NAME]);
            Assert.IsTrue(parameters.ContainsKey(MyEnvironmentVariableBaseDto.VALUE_PROPERTY_NAME));
            Assert.AreEqual(MyEnvironmentVariableBaseDto.VALUE_PROPERTY_VALUE, parameters[MyEnvironmentVariableBaseDto.VALUE_PROPERTY_NAME]);
        }

        [TestMethod]
        public void ImportTestSucceeds()
        {
            // Initialise
            System.Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            System.Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);

            string stringParam;
            var stringValue = "tralala";
            string longParam;
            var longValue = 42L;

            stringParam = System.Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(stringParam);
            longParam = System.Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(longParam);

            System.Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, stringValue);
            System.Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, longValue.ToString());

            var sut = new ClassWithEnvironmentVariableAttributes();

            EnvironmentVariableConverter.Import(sut);

            Assert.AreEqual(longValue, sut.LongPropertyWithAnnotation);
            Assert.AreEqual(stringValue, sut.StringPropertyWithAnnotation);
            Assert.AreEqual(0, sut.LongPropertyWithoutAnnotation);
            Assert.IsNull(sut.StringPropertyWithoutAnnotation);

            // Cleanup
            System.Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            System.Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            stringParam = System.Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(stringParam);
            longParam = System.Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(longParam);
        }

        [TestMethod]
        public void ImportStaticTestSucceeds()
        {
            // Initialise
            System.Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            System.Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);

            string stringParam;
            var stringValue = "tralala";
            string longParam;
            var longValue = 42L;

            stringParam = System.Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(stringParam);
            longParam = System.Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(longParam);

            System.Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, stringValue);
            System.Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, longValue.ToString());

            var sut = new ClassWithStaticEnvironmentVariableAttributes();

            EnvironmentVariableConverter.Import(sut);

            Assert.AreEqual(longValue, ClassWithStaticEnvironmentVariableAttributes.StaticLongPropertyWithAnnotation);
            Assert.AreEqual(stringValue, sut.StringPropertyWithAnnotation);
            Assert.AreEqual(0, sut.LongPropertyWithoutAnnotation);
            Assert.IsNull(sut.StringPropertyWithoutAnnotation);

            // Cleanup
            System.Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            System.Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            stringParam = System.Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(stringParam);
            longParam = System.Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(longParam);
        }

        [TestMethod]
        public void ExportTestSucceeds()
        {
            // Initialise
            System.Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            System.Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);

            string stringParam;
            var stringValue = "tralala";
            string longParam;
            var longValue = 42L;

            stringParam = System.Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(stringParam);
            longParam = System.Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(longParam);

            var sut = new ClassWithEnvironmentVariableAttributes
            {
                StringPropertyWithAnnotation = stringValue,
                LongPropertyWithAnnotation = longValue,
            };

            EnvironmentVariableConverter.Export(sut);

            stringParam = System.Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.AreEqual(stringValue, stringParam);
            longParam = System.Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.AreEqual(longValue.ToString(), longParam);

            // Cleanup
            System.Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            System.Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            stringParam = System.Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(stringParam);
            longParam = System.Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(longParam);
        }
    }
}
