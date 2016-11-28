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

using biz.dfch.CS.Commons.Collections;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.Commons.Tests.Collections
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class IDictionaryExtensionsTest
    {
        [TestMethod]
        [ExpectContractFailure(MessagePattern = "ContainsKey.+key1")]
        public void ThrowOnDuplicateKeys()
        {
            var source = new DictionaryParameters
            {
                {"key1", "valueS1"}, 
                {"key2", "valueS2"},
            };
            var target = new DictionaryParameters
            {
                {"key1", "valueT1"}, 
                {"key2", "valueT2"},
            };

            var result = source.Merge(target);
            
            Assert.IsNull(result);
        }

        [TestMethod]
        public void NullOnDuplicateKeys()
        {
            var source = new DictionaryParameters
            {
                {"key1", "valueS1"}, 
                {"key2", "valueS2"},
            };
            var target = new DictionaryParameters
            {
                {"key1", "valueT1"}, 
                {"key2", "valueT2"},
            };

            var result = source.Merge(target, MergeOptions.NullOnDuplicateKeys);
            
            Assert.IsNull(result);
        }

        [TestMethod]
        public void OverwriteLeftSucceeds()
        {
            var source = new DictionaryParameters
            {
                {"key3", "valueS3"}, 
                {"key2", "valueS2"},
            };
            var target = new DictionaryParameters
            {
                {"key1", "valueT1"}, 
                {"key2", "valueT2"},
            };

            var result = source.Merge(target, MergeOptions.OverwriteLeft);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("valueT2", result["key2"]);
        }

        [TestMethod]
        public void OverwriteRightSucceeds()
        {
            var source = new DictionaryParameters
            {
                {"key3", "valueS3"}, 
                {"key2", "valueS2"},
            };
            var target = new DictionaryParameters
            {
                {"key1", "valueT1"}, 
                {"key2", "valueT2"},
            };

            var result = source.Merge(target, MergeOptions.OverwriteRight);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("valueS2", result["key2"]);
        }

        [TestMethod]
        public void OutersectSucceeds()
        {
            var source = new DictionaryParameters
            {
                {"key3", "valueS3"}, 
                {"key2", "valueS2"},
            };
            var target = new DictionaryParameters
            {
                {"key1", "valueT1"}, 
                {"key2", "valueT2"},
            };

            var result = source.Merge(target, MergeOptions.Outersect);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(!result.ContainsKey("key2"));
        }

        [TestMethod]
        public void IntersectSucceeds()
        {
            var source = new DictionaryParameters
            {
                {"key3", "valueS3"}, 
                {"key2", "valueS2"},
            };
            var target = new DictionaryParameters
            {
                {"key1", "valueT1"}, 
                {"key2", "valueT2"},
            };

            var result = source.Merge(target, MergeOptions.Intersect);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.ContainsKey("key2"));
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "source")]
        public void NullSource()
        {
            var source = default(DictionaryParameters);
            var target = new DictionaryParameters
            {
                {"key1", "valueT1"}, 
                {"key2", "valueT2"},
            };

            source.Merge(target);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "target")]
        public void NullTarget()
        {
            var source = new DictionaryParameters
            {
                {"key1", "valueT1"}, 
                {"key2", "valueT2"},
            };
            var target = default(DictionaryParameters);

            source.Merge(target);
        }
    }
}
