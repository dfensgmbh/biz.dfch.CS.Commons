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
    public class EnvironmentVariableBaseDtoTest
    {
        public const string STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION = "biz.dfch.CS.Commons.EnvironmentVariableAttribute.StringPropertyWithAnnotation";
        public const string LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION = "biz.dfch.CS.Commons.EnvironmentVariableAttribute.LongPropertyWithAnnotation";

        public class ClassWithEnvironmentVariableAttributes : EnvironmentVariableBaseDto
        {
            [EnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public string StringPropertyWithAnnotation { get; set; }

            [EnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION)]
            public long LongPropertyWithAnnotation { get; set; }

            public string StringPropertyWithoutAnnotation { get; set; }

            public long LongPropertyWithoutAnnotation { get; set; }
        }

        [TestMethod]
        public void ImportTestSucceeds()
        {
            // Initialise
            Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);

            string stringParam;
            var stringValue = "tralala";
            string longParam;
            var longValue = 42L;

            stringParam = Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(stringParam);
            longParam = Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(longParam);

            Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, stringValue);
            Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, longValue.ToString());

            var sut = new ClassWithEnvironmentVariableAttributes();
            sut.Import();

            Assert.AreEqual(longValue, sut.LongPropertyWithAnnotation);
            Assert.AreEqual(stringValue, sut.StringPropertyWithAnnotation);
            Assert.AreEqual(0, sut.LongPropertyWithoutAnnotation);
            Assert.IsNull(sut.StringPropertyWithoutAnnotation);

            // Cleanup
            Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            stringParam = Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(stringParam);
            longParam = Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(longParam);
        }

        [TestMethod]
        public void ExportTestSucceeds()
        {
            // Initialise
            Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);

            string stringParam;
            var stringValue = "tralala";
            string longParam;
            var longValue = 42L;

            stringParam = Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(stringParam);
            longParam = Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(longParam);

            var sut = new ClassWithEnvironmentVariableAttributes
            {
                StringPropertyWithAnnotation = stringValue,
                LongPropertyWithAnnotation = longValue,
            };

            sut.Export();

            stringParam = Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.AreEqual(stringValue, stringParam);
            longParam = Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.AreEqual(longValue.ToString(), longParam);

            // Cleanup
            Environment.SetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            Environment.SetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION, null);
            stringParam = Environment.GetEnvironmentVariable(STRING_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(stringParam);
            longParam = Environment.GetEnvironmentVariable(LONG_PROPERTY_WITH_ANNOTATION_ANNOTATION);
            Assert.IsNull(longParam);
        }
    }
}
