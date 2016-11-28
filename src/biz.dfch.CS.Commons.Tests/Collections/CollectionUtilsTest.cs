/**
 * Copyright 2014-2016 d-fens GmbH
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
using System.Collections.Specialized;
using biz.dfch.CS.Commons.Collections;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Collections
{
    [TestClass]
    public class CollectionUtilsTest
    {
        [TestMethod]
        public void NameValueCollectionComparerWithEqualCollectionsShouldReturnTrue()
        {
            // Arrange
            var left = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            var right = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            // Act
            var fReturnExactOrder = CollectionUtils.NameValueCollectionComparer(left, right, true);
            var fReturnAnyOrder = CollectionUtils.NameValueCollectionComparer(left, right, false);

            // Assert
            Assert.IsTrue(fReturnExactOrder);
            Assert.IsTrue(fReturnAnyOrder);
        }

        [TestMethod]
        public void NameValueCollectionComparerWithEqualNotExactCollectionsShouldReturnFalse()
        {
            // Arrange
            var left = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            var right = new NameValueCollection
            {
                {"arbitrary-name2", "arbitrary-value2"},
                {"arbitrary-name1", "arbitrary-value1"}
            };

            // Act
            var fReturnExactOrder = CollectionUtils.NameValueCollectionComparer(left, right, true);
            var fReturnAnyOrder = CollectionUtils.NameValueCollectionComparer(left, right, false);

            // Assert
            Assert.IsFalse(fReturnExactOrder);
            Assert.IsTrue(fReturnAnyOrder);
        }

        [TestMethod]
        public void NameValueCollectionComparerWithNotEqualCollectionsShouldReturnFalse()
        {
            // Arrange
            var left = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            var right = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value-slightly-different"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            // Act
            var fReturnExactOrder = CollectionUtils.NameValueCollectionComparer(left, right, true);
            var fReturnAnyOrder = CollectionUtils.NameValueCollectionComparer(left, right, false);

            // Assert
            Assert.IsFalse(fReturnExactOrder);
            Assert.IsFalse(fReturnAnyOrder);
        }

        [TestMethod]
        public void NameValueCollectionComparerWithEmptyCollectionsShouldReturnTrue()
        {
            // Arrange
            var left = new NameValueCollection();
            var right = new NameValueCollection();

            // Act
            var fReturnExactOrder = CollectionUtils.NameValueCollectionComparer(left, right, true);
            var fReturnAnyOrder = CollectionUtils.NameValueCollectionComparer(left, right, false);

            // Assert
            Assert.IsTrue(fReturnExactOrder);
            Assert.IsTrue(fReturnAnyOrder);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "right")]
        public void NameValueCollectionComparerWithNullCollection1ShouldThrowContractException()
        {
            // Arrange
            var left = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            var right = default(NameValueCollection);

            // Act
            var fReturnExactOrder = CollectionUtils.NameValueCollectionComparer(left, right, true);

            // Assert
            Assert.IsFalse(fReturnExactOrder);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "right")]
        public void NameValueCollectionComparerWithNullCollection2ShouldThrowContractException()
        {
            // Arrange
            var left = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            var right = default(NameValueCollection);

            // Act
            var fReturnAnyOrder = CollectionUtils.NameValueCollectionComparer(left, right, false);

            // Assert
            Assert.IsFalse(fReturnAnyOrder);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "left")]
        public void NameValueCollectionComparerWithNullCollection3ShouldThrowContractException()
        {
            // Arrange
            var left = default(NameValueCollection);

            var right = new NameValueCollection
            {
                {"arbitrary-name2", "arbitrary-value2"},
                {"arbitrary-name1", "arbitrary-value1"}
            };

            // Act
            var fReturnExactOrder = CollectionUtils.NameValueCollectionComparer(left, right, true);

            // Assert
            Assert.IsFalse(fReturnExactOrder);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "left")]
        public void NameValueCollectionComparerWithNullCollection4ShouldThrowContractException()
        {
            // Arrange
            var left = default(NameValueCollection);

            var right = new NameValueCollection
            {
                {"arbitrary-name2", "arbitrary-value2"},
                {"arbitrary-name1", "arbitrary-value1"}
            };

            // Act
            var fReturnAnyOrder = CollectionUtils.NameValueCollectionComparer(left, right, false);

            // Assert
            Assert.IsFalse(fReturnAnyOrder);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "Precondition.+left")]
        public void DictionaryMergeWithLeftNullThrowsContractException()
        {
            var left = default(Dictionary<string, object>);
            var right = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueR2"},
                {"arbitrary-name1", "arbitrary-valueR1"},
            };

            CollectionUtils.Merge(left, right);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "Precondition.+right")]
        public void DictionaryMergeWithRightNullThrowsContractException()
        {
            var left = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueL2"},
                {"arbitrary-name1", "arbitrary-valueL1"},
            };
            var right = default(Dictionary<string, object>);

            CollectionUtils.Merge(left, right);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "Precondition.+mergeOption")]
        public void DictionaryMergeWithInvalidMergeOptionThrowsContractException()
        {
            var left = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueL2"},
                {"arbitrary-name1", "arbitrary-valueL1"},
            };
            var right = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueR2"},
                {"arbitrary-name1", "arbitrary-valueR1"},
            };

            CollectionUtils.Merge(left, right, (MergeOptions) 42);
        }

        [TestMethod]
        public void DictionaryMergeWithEmptyDictionariesReturnsEmptyDictionary()
        {
            var left = new Dictionary<string, object>();
            var right = new Dictionary<string, object>();

            var result = CollectionUtils.Merge(left, right);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "ContainsKey.+arbitrary-name2")]
        public void DictionaryMergeDuplicateKeysThrowsContractException()
        {
            var left = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueL2"},
                {"arbitrary-name1", "arbitrary-valueL1"},
            };
            var right = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueR2"},
                {"arbitrary-name1", "arbitrary-valueR1"},
            };

            CollectionUtils.Merge(left, right);
        }

        [TestMethod]
        public void DictionaryMergeDuplicateKeysNullReturnsNull()
        {
            var left = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueL2"},
                {"arbitrary-name1", "arbitrary-valueL1"},
            };
            var right = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueR2"},
                {"arbitrary-name1", "arbitrary-valueR1"},
            };

            var result = CollectionUtils.Merge(left, right, MergeOptions.NullOnDuplicateKeys);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void DictionaryMergeDuplicateKeysOverWriteLeftSucceeds()
        {
            var left = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueL2"},
                {"arbitrary-name1", "arbitrary-valueL1"},
            };
            var right = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueR2"},
                {"arbitrary-name1", "arbitrary-valueR1"},
            };

            var result = CollectionUtils.Merge(left, right, MergeOptions.OverwriteLeft);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("arbitrary-valueR2", result["arbitrary-name2"]);
            Assert.AreEqual("arbitrary-valueR1", result["arbitrary-name1"]);
        }

        [TestMethod]
        public void DictionaryMergeDuplicateKeysOverWriteRightSucceeds()
        {
            var left = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueL2"},
                {"arbitrary-name1", "arbitrary-valueL1"},
            };
            var right = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueR2"},
                {"arbitrary-name1", "arbitrary-valueR1"},
            };

            var result = CollectionUtils.Merge(left, right, MergeOptions.OverwriteRight);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("arbitrary-valueL2", result["arbitrary-name2"]);
            Assert.AreEqual("arbitrary-valueL1", result["arbitrary-name1"]);
        }

        [TestMethod]
        public void DictionaryMergeDuplicateKeysOutersectSucceeds()
        {
            var left = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueL2"},
                {"arbitrary-name1", "arbitrary-valueL1"},
                {"arbitrary-name0", "arbitrary-valueL0"},
            };
            var right = new Dictionary<string, object>
            {
                {"arbitrary-name3", "arbitrary-valueR3"},
                {"arbitrary-name2", "arbitrary-valueR2"},
                {"arbitrary-name1", "arbitrary-valueR1"},
            };

            var result = CollectionUtils.Merge(left, right, MergeOptions.Outersect);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("arbitrary-valueR3", result["arbitrary-name3"]);
            Assert.AreEqual("arbitrary-valueL0", result["arbitrary-name0"]);
        }

        [TestMethod]
        public void DictionaryMergeDuplicateKeysIntersect1Succeeds()
        {
            var left = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueL2"},
                {"arbitrary-name1", "arbitrary-valueL1"},
                {"arbitrary-name0", "arbitrary-valueL0"},
            };
            var right = new Dictionary<string, object>
            {
                {"arbitrary-name3", "arbitrary-valueR3"},
                {"arbitrary-name2", "arbitrary-valueR2"},
                {"arbitrary-name1", "arbitrary-valueR1"},
            };

            var result = CollectionUtils.Merge(left, right, MergeOptions.Intersect);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("arbitrary-valueL2", result["arbitrary-name2"]);
            Assert.AreEqual("arbitrary-valueL1", result["arbitrary-name1"]);
        }

        [TestMethod]
        public void DictionaryMergeDuplicateKeysIntersect2Succeeds()
        {
            var left = new Dictionary<string, object>
            {
                {"arbitrary-name3", "arbitrary-valueR3"},
                {"arbitrary-name2", "arbitrary-valueR2"},
                {"arbitrary-name1", "arbitrary-valueR1"},
            };
            var right = new Dictionary<string, object>
            {
                {"arbitrary-name2", "arbitrary-valueL2"},
                {"arbitrary-name1", "arbitrary-valueL1"},
                {"arbitrary-name0", "arbitrary-valueL0"},
            };

            var result = CollectionUtils.Merge(left, right, MergeOptions.Intersect);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("arbitrary-valueR2", result["arbitrary-name2"]);
            Assert.AreEqual("arbitrary-valueR1", result["arbitrary-name1"]);
        }

        [TestMethod]
        public void NonGenericDictionaryMergeDuplicateKeysIntersect2Succeeds()
        {
            var left = new DictionaryParameters
            {
                {"arbitrary-name3", "arbitrary-valueR3"},
                {"arbitrary-name2", "arbitrary-valueR2"},
                {"arbitrary-name1", "arbitrary-valueR1"},
            };
            var right = new DictionaryParameters
            {
                {"arbitrary-name2", "arbitrary-valueL2"},
                {"arbitrary-name1", "arbitrary-valueL1"},
                {"arbitrary-name0", "arbitrary-valueL0"},
            };

            var result = CollectionUtils.Merge(left, right, MergeOptions.Intersect);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("arbitrary-valueR2", result["arbitrary-name2"]);
            Assert.AreEqual("arbitrary-valueR1", result["arbitrary-name1"]);
        }
    }
}
