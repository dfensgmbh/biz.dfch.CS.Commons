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
using System.Collections.Generic;
using System.Net;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests
{
    [TestClass]
    public class DictionaryParametersTest
    {
        class ArbitraryObject : BaseDto
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
            public NetworkCredential NetworkCredentialProperty { get; set; }
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConvertToDictionaryParametersThrowsContractException()
        {
            // Arrange
            var arbitraryObject = default(ArbitraryObject);

            var sut = new DictionaryParameters();

            // Act
            var result = sut.Convert(arbitraryObject);

            // Assert
            // N/A
        }

        [TestMethod]
        public void ConvertToDictionaryParametersSucceeds()
        {
            // Arrange
            var stringProperty = "arbitrary-string";
            var intProperty = 42;
            var networkCredentialProperty = new NetworkCredential("arbitrary-user", "arbitrary-password", "arbitrary-domain");

            var arbitraryObject = new ArbitraryObject();
            arbitraryObject.StringProperty = stringProperty;
            arbitraryObject.IntProperty = intProperty;
            arbitraryObject.NetworkCredentialProperty = networkCredentialProperty;

            var sut = new DictionaryParameters();

            // Act
            var result = sut.Convert(arbitraryObject);

            // Assert
            Assert.AreEqual(arbitraryObject.StringProperty, result["StringProperty"]);
            Assert.AreEqual(arbitraryObject.IntProperty, result["IntProperty"]);
            Assert.AreEqual(arbitraryObject.NetworkCredentialProperty, result["NetworkCredentialProperty"]);
        }

        [TestMethod]
        public void ConvertToDictionaryParametersAndBackSucceeds()
        {
            // Arrange
            var stringProperty = "arbitrary-string";
            var intProperty = 42;
            var networkCredentialProperty = new NetworkCredential("arbitrary-user", "arbitrary-password", "arbitrary-domain");

            var arbitraryObject = new ArbitraryObject
            {
                StringProperty = stringProperty,
                IntProperty = intProperty,
                NetworkCredentialProperty = networkCredentialProperty
            };

            var sut = new DictionaryParameters();

            // Act
            var resultDictionaryParameters = sut.Convert(arbitraryObject);
            var result = resultDictionaryParameters.Convert<ArbitraryObject>();

            // Assert
            Assert.AreEqual(arbitraryObject.StringProperty, result.StringProperty);
            Assert.AreEqual(arbitraryObject.IntProperty, result.IntProperty);
            Assert.AreEqual(arbitraryObject.NetworkCredentialProperty, result.NetworkCredentialProperty);
        }

/*
 * 
    $ht = @{}
    $ht.StringKey1 = "arbitrary-value1"
    $ht.EmptyStringKey2 = ""
    $ht.LongKey3 = 42L
    $ht.IntKey4 = 5
    $ht.ArbitraryObjectKey5 = New-Object System.Net.NetworkCredential("arbitrary-user", "arbitrary-password");
    $ht.ArrayKey6 = @("val1", "val2", "val3")

    Name                Value
    ----                -----
    StringKey1          arbitrary-value1
    EmptyStringKey2
    LongKey3            42
    IntKey4             5
    ArbitraryObjectKey5 System.Net.NetworkCredential
    ArrayKey6           {val1, val2, val3}
    NullKey7

    $ht | ConvertTo-Json
    {
      "NullKey7":  null,
      "ArrayKey6":  
      [
        "val1",
        "val2",
        "val3"
      ],
      "IntKey4":  5,
      "StringKey1":  "arbitrary-value1",
      "LongKey3":  42,
      "ArbitraryObjectKey5":  
      {
        "UserName":  "arbitrary-user",
        "Password":  "arbitrary-password",
        "SecurePassword":  
        {
          "Length":  18
        },
        "Domain":  ""
      },
      "EmptyStringKey2":  ""
    }

    ($ht | ConvertTo-Json -Compress).Replace('"', '\"')
    {\"NullKey7\":null,\"ArrayKey6\":[\"val1\",\"val2\",\"val3\"],\"IntKey4\":5,\"StringKey1\":\"arbitrary-value1\",\"LongKey3\":42,\"ArbitraryObjectKey5\":{\"UserName\":\"arbitrary-user\",\"Password\":\"arbitrary-password\",\"SecurePassword\":{\"Length\":18},\"Domain\":\"\"},\"EmptyStringKey2\":\"\"}
 *
 */
        [TestMethod]
        public void NewDictionaryParametersFromJsonStringSucceeds()
        {
            // Arrange
            var jsonString = "{\"NullKey7\":null,\"ArrayKey6\":[\"val1\",\"val2\",\"val3\"],\"IntKey4\":5,\"StringKey1\":\"arbitrary-value1\",\"LongKey3\":42,\"ArbitraryObjectKey5\":{\"UserName\":\"arbitrary-user\",\"Password\":\"arbitrary-password\",\"SecurePassword\":{\"Length\":18},\"Domain\":\"\"},\"EmptyStringKey2\":\"\"}";

            // Act
            var sut = new DictionaryParameters(jsonString);

            // Assert
            Assert.IsTrue(sut.ContainsKey("StringKey1"));
            Assert.AreEqual("arbitrary-value1", sut["StringKey1"]);
            Assert.IsTrue(sut.ContainsKey("EmptyStringKey2"));
            Assert.AreEqual("", sut["EmptyStringKey2"]);
            Assert.IsTrue(sut.ContainsKey("LongKey3"));
            Assert.AreEqual(42L, sut["LongKey3"]);
            Assert.IsTrue(sut.ContainsKey("IntKey4"));
            Assert.AreEqual(5L, sut["IntKey4"]);
            Assert.IsTrue(sut.ContainsKey("ArbitraryObjectKey5"));
            Assert.IsTrue(sut.ContainsKey("ArrayKey6"));
            Assert.IsTrue(sut["ArrayKey6"] is List<object>);
            var list = sut["ArrayKey6"] as List<object>;
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list.Contains("val1"));
            Assert.IsTrue(list.Contains("val2"));
            Assert.IsTrue(list.Contains("val3"));
            Assert.IsTrue(sut.ContainsKey("NullKey7"));
            Assert.IsNull(sut["NullKey7"]);
        }

        [TestMethod]
        public void GetWithExistentKeyFromDictionaryParametersReturnsValue()
        {
            // Arrange
            var keyName = "arbitrary-key";
            var value = "some-arbitrary-string";

            var sut = new DictionaryParameters
            {
                {keyName, value}
            };

            // Act
            var result = (string) sut.Get(keyName);

            // Assert
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetWithInexistentKeyFromDictionaryParametersThrowsContractException()
        {
            // Arrange
            var keyName = "arbitrary-key";
            var value = "some-arbitrary-string";

            var sut = new DictionaryParameters
            {
                {keyName, value}
            };

            // Act
            var result = sut.Get("inexistent-key-name");

            // Assert
            // N/A
        }

        [TestMethod]
        public void GetOrDefaultWithInexistentKeyFromDictionaryParametersReturnsDefaultValue()
        {
            // Arrange
            var keyName = "arbitrary-key";
            var value = "some-arbitrary-string";
            var defaultValue = "arbitrary-default-value";

            var sut = new DictionaryParameters
            {
                {keyName, value}
            };

            // Act
            var result = (string) sut.GetOrDefault("inexistent-key-name", defaultValue);

            // Assert
            Assert.AreEqual(defaultValue, result);
        }

        [TestMethod]
        public void GetOrDefaultWithExistentKeyFromDictionaryParametersReturnsValue()
        {
            // Arrange
            var keyName = "arbitrary-key";
            var value = "some-arbitrary-string";
            var defaultValue = "arbitrary-default-value";

            var sut = new DictionaryParameters
            {
                {keyName, value}
            };

            // Act
            var result = (string) sut.GetOrDefault(keyName, defaultValue);

            // Assert
            Assert.AreEqual(value, result);
        }

        [TestMethod]
        public void GetOrSelfWithInexistentKeyFromDictionaryParametersReturnsDefaultValue()
        {
            // Arrange
            var keyName = "arbitrary-key";
            var value = "some-arbitrary-string";

            var sut = new DictionaryParameters
            {
                {keyName, value}
            };

            // Act
            var result = sut.GetOrSelf("inexistent-key-name");

            // Assert
            Assert.AreEqual(sut, result);
        }

        [TestMethod]
        public void GetOrSelfWithExistentKeyFromDictionaryParametersReturnsValue()
        {
            // Arrange
            var keyName = "arbitrary-key";
            var value = "some-arbitrary-string";

            var sut = new DictionaryParameters
            {
                {keyName, value}
            };

            // Act
            var result = (string) sut.GetOrSelf(keyName);

            // Assert
            Assert.AreEqual(value, result);
        }

        public void CanAddReturnsTrue()
        {
            // Arrange
            var sut = new DictionaryParameters()
            {
                { "Key1.1", "arbitrary-value1-1" }
                ,
                { "Key1.2", 11L}
            };

            var objectToMerge = new DictionaryParameters()
            {
                { "Key2.1", "arbitrary-value-2.1" }
                ,
                { "Key2.2", 22L}
            };

            // Act
            var result = sut.CanAdd(objectToMerge);
            
            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanAddReturnsFalse()
        {
            // Arrange
            var sut = new DictionaryParameters()
            {
                { "Key1.1", "arbitrary-value1-1" }
                ,
                { "Key1.2", 11L}
                ,
                { "DuplicateKey", "arbitrary-value" }
            };

            var objectToMerge = new DictionaryParameters()
            {
                { "Key2.1", "arbitrary-value-2.1" }
                ,
                { "Key2.2", 22L}
                ,
                { "DuplicateKey", "arbitrary-value" }
            };

            // Act
            var result = sut.CanAdd(objectToMerge);
            
            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void AddSucceeds()
        {
            // Arrange
            var sut = new DictionaryParameters()
            {
                { "Key1.1", "arbitrary-value1-1" }
                ,
                { "Key1.2", 11L}
            };

            var objectToMerge = new DictionaryParameters()
            {
                { "Key2.1", "arbitrary-value-2.1" }
                ,
                { "Key2.2", 22L}
            };

            // Act
            var result = sut.Add(objectToMerge);
            
            // Assert
            Assert.AreEqual(4, result.Keys.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddThrowsArgumentException()
        {
            // Arrange
            var sut = new DictionaryParameters()
            {
                { "Key1.1", "arbitrary-value1-1" }
                ,
                { "Key1.2", 11L}
                ,
                { "DuplicateKey", "arbitrary-value" }
            };

            var objectToMerge = new DictionaryParameters()
            {
                { "Key2.1", "arbitrary-value-2.1" }
                ,
                { "Key2.2", 22L}
                ,
                { "DuplicateKey", "arbitrary-value" }
            };

            // Act
            var result = sut.Add(objectToMerge);
            
            // Assert
            // N/A
        }

        [TestMethod]
        public void DictionaryParametersToStringReturnsNestedObjectsAndIndentedJson()
        {
            // Arrange
            var nested = new DictionaryParameters()
            {
                { "Key2.1", "arbitrary-value-2.1" }
                ,
                { "Key2.2", 22L}
                ,
                { "ArbitraryKey", "arbitrary-nested-value" }
            };

            var sut = new DictionaryParameters()
            {
                { "Key1.1", "arbitrary-value1-1" }
                ,
                { "Key1.2", 11L}
                ,
                { "Nested", nested}
                ,
                { "Key1.3", "arbitrary-value" }
            };

            // Act
            var result = sut.ToString();

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
            Assert.IsTrue(result.Contains("arbitrary-nested-value"));
        }

        [TestMethod]
        public void DictionaryParametersReplaceExistingKeysTrueSucceeds()
        {
            var source = new DictionaryParameters()
            {
                { "key1", "value1-existing" }
                ,
                { "key2", "value2" }
            };

            var objectToMerge = new DictionaryParameters()
            {
                { "key1", "value1-new" }
                ,
                { "key3", "value3" }
            };

            var result = source.Add(objectToMerge, true);

            Assert.IsTrue(result.ContainsKey("key1"));
            Assert.AreEqual("value1-new", result["key1"]);
            Assert.IsTrue(result.ContainsKey("key2"));
            Assert.IsTrue(result.ContainsKey("key3"));
        }

        [TestMethod]
        public void DictionaryParametersReplaceExistingKeysFalseSucceeds()
        {
            var source = new DictionaryParameters()
            {
                { "key1", "value1" }
                ,
                { "key2", "value2" }
            };

            var objectToMerge = new DictionaryParameters()
            {
                { "key1", "value1-new" }
                ,
                { "key3", "value3" }
            };

            var result = source.Add(objectToMerge, false);

            Assert.IsTrue(result.ContainsKey("key1"));
            Assert.AreEqual("value1", result["key1"]);
            Assert.IsTrue(result.ContainsKey("key2"));
            Assert.IsTrue(result.ContainsKey("key3"));
        }

        [TestMethod]
        public void DictionaryParametersWithRecursion()
        {
            var json = Resources.ACTION_VIRTUALMACHINES_RESPONSE;

            var sut = new DictionaryParameters(json);

            Assert.IsNotNull(sut);
        }

    }
}
