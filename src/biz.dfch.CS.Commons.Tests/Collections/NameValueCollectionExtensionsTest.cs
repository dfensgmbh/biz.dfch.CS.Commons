/**
 * Copytarget 2014-2016 d-fens GmbH
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

using System.Collections.Specialized;
using biz.dfch.CS.Commons.Collections;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Collections
{
    [TestClass]
    public class NameValueCollectionExtensionsTest
    {
        [TestMethod]
        public void NameValueCollectionComparerWithEqualCollectionsShouldReturnTrue()
        {
            // Arrange
            var source = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            var target = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            // Act
            var fReturnExactOrder = source.Compare(target, true);
            var fReturnAnyOrder = source.Compare(target, false);

            // Assert
            Assert.IsTrue(fReturnExactOrder);
            Assert.IsTrue(fReturnAnyOrder);
        }

        [TestMethod]
        public void NameValueCollectionComparerWithEqualNotExactCollectionsShouldReturnFalse()
        {
            // Arrange
            var source = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            var target = new NameValueCollection
            {
                {"arbitrary-name2", "arbitrary-value2"},
                {"arbitrary-name1", "arbitrary-value1"}
            };

            // Act
            var fReturnExactOrder = source.Compare(target, true);
            var fReturnAnyOrder = source.Compare(target, false);

            // Assert
            Assert.IsFalse(fReturnExactOrder);
            Assert.IsTrue(fReturnAnyOrder);
        }

        [TestMethod]
        public void NameValueCollectionComparerWithNotEqualCollectionsShouldReturnFalse()
        {
            // Arrange
            var source = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            var target = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value-slightly-different"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            // Act
            var fReturnExactOrder = source.Compare(target, true);
            var fReturnAnyOrder = source.Compare(target, false);

            // Assert
            Assert.IsFalse(fReturnExactOrder);
            Assert.IsFalse(fReturnAnyOrder);
        }

        [TestMethod]
        public void NameValueCollectionComparerWithEmptyCollectionsShouldReturnTrue()
        {
            // Arrange
            var source = new NameValueCollection();
            var target = new NameValueCollection();

            // Act
            var fReturnExactOrder = source.Compare(target, true);
            var fReturnAnyOrder = source.Compare(target, false);

            // Assert
            Assert.IsTrue(fReturnExactOrder);
            Assert.IsTrue(fReturnAnyOrder);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "target")]
        public void NameValueCollectionComparerWithNullCollection1ShouldThrowContractException()
        {
            // Arrange
            var source = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            var target = default(NameValueCollection);

            // Act
            var fReturnExactOrder = source.Compare(target, true);

            // Assert
            Assert.IsFalse(fReturnExactOrder);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "target")]
        public void NameValueCollectionComparerWithNullCollection2ShouldThrowContractException()
        {
            // Arrange
            var source = new NameValueCollection
            {
                {"arbitrary-name1", "arbitrary-value1"},
                {"arbitrary-name2", "arbitrary-value2"}
            };

            var target = default(NameValueCollection);

            // Act
            var fReturnAnyOrder = source.Compare(target, false);

            // Assert
            Assert.IsFalse(fReturnAnyOrder);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "source")]
        public void NameValueCollectionComparerWithNullCollection3ShouldThrowContractException()
        {
            // Arrange
            var source = default(NameValueCollection);

            var target = new NameValueCollection
            {
                {"arbitrary-name2", "arbitrary-value2"},
                {"arbitrary-name1", "arbitrary-value1"}
            };

            // Act
            var fReturnExactOrder = source.Compare(target, true);

            // Assert
            Assert.IsFalse(fReturnExactOrder);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "source")]
        public void NameValueCollectionComparerWithNullCollection4ShouldThrowContractException()
        {
            // Arrange
            var source = default(NameValueCollection);

            var target = new NameValueCollection
            {
                {"arbitrary-name2", "arbitrary-value2"},
                {"arbitrary-name1", "arbitrary-value1"}
            };

            // Act
            var fReturnAnyOrder = source.Compare(target, false);

            // Assert
            Assert.IsFalse(fReturnAnyOrder);
        }
    }
}
