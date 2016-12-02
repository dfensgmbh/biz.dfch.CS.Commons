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
using biz.dfch.CS.Commons.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Converters
{
    [TestClass]
    public class EnvironmentVariableAttributeTest
    {
        public const string StringPropertyWithAnnotationAnnotation = "biz.dfch.CS.Commons.EnvironmentVariableAttribute.StringPropertyWithAnnotation";
        public const string LongPropertyWithAnnotationAnnotation = "biz.dfch.CS.Commons.EnvironmentVariableAttribute.LongPropertyWithAnnotation";

        [TestMethod]
        public void StringPropertyWithAnnotation()
        {
            // Arrange, Act
            var sut = new Converters.EnvironmentVariableConverterTest.ClassWithEnvironmentVariableAttributes();

            // Assert
            var propertyInfo = sut.GetType().GetProperty("StringPropertyWithAnnotation");
            Assert.IsNotNull(propertyInfo);

            var attribute = (EnvironmentVariableAttribute) Attribute.GetCustomAttribute(propertyInfo, typeof(EnvironmentVariableAttribute));
            Assert.IsNotNull(attribute);

            Assert.AreEqual(StringPropertyWithAnnotationAnnotation, attribute.Name);
        }

        [TestMethod]
        public void LongPropertyWithAnnotation()
        {
            // Arrange, Act
            var sut = new Converters.EnvironmentVariableConverterTest.ClassWithEnvironmentVariableAttributes();

            // Assert
            var propertyInfo = sut.GetType().GetProperty("LongPropertyWithAnnotation");
            Assert.IsNotNull(propertyInfo);

            var attribute = (EnvironmentVariableAttribute) Attribute.GetCustomAttribute(propertyInfo, typeof(EnvironmentVariableAttribute));
            Assert.IsNotNull(attribute);

            Assert.AreEqual(LongPropertyWithAnnotationAnnotation, attribute.Name);
        }

        [TestMethod]
        public void StringPropertyWithoutAnnotation()
        {
            // Arrange, Act
            var sut = new Converters.EnvironmentVariableConverterTest.ClassWithEnvironmentVariableAttributes();

            // Assert
            var propertyInfo = sut.GetType().GetProperty("StringPropertyWithoutAnnotation");
            Assert.IsNotNull(propertyInfo);

            var attribute = (EnvironmentVariableAttribute) Attribute.GetCustomAttribute(propertyInfo, typeof(EnvironmentVariableAttribute));
            Assert.IsNull(attribute);
        }

        [TestMethod]
        public void LongPropertyWithoutAnnotation()
        {
            // Arrange, Act
            var sut = new Converters.EnvironmentVariableConverterTest.ClassWithEnvironmentVariableAttributes();

            // Assert
            var propertyInfo = sut.GetType().GetProperty("LongPropertyWithoutAnnotation");
            Assert.IsNotNull(propertyInfo);

            var attribute = (EnvironmentVariableAttribute) Attribute.GetCustomAttribute(propertyInfo, typeof(EnvironmentVariableAttribute));
            Assert.IsNull(attribute);
        }
    }
}
