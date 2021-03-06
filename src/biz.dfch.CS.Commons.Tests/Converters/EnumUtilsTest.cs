﻿/**
 * Copyright 2015-2016 d-fens GmbH
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
    public class EnumUtilsTest
    {
        [TestMethod]
        public void ParseForValidValueIgnoringCaseReturnsCorrespondingEnumValue()
        {
            Assert.AreEqual(EnumExample.None, EnumUtils.Parse<EnumExample>("None"));
            Assert.AreEqual(EnumExample.None, EnumUtils.Parse<EnumExample>("NOne"));
        }

        [TestMethod]
        public void ParseForValidValueNotIgnoringCaseReturnsCorrespondingEnumValueIfCasesMatch()
        {
            Assert.AreEqual(EnumExample.Critical, EnumUtils.Parse<EnumExample>("Critical", false));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseWithNonCaseMatchingValueNotIgnoringCaseThrowsArgumentException()
        {
            EnumUtils.Parse<EnumExample>("cRITICAL", false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ParseWithInvalidValueThrowsArgumentException()
        {
            EnumUtils.Parse<EnumExample>("invalid-value", false);
        }
    }
}
