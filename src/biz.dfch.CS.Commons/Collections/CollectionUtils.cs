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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;

namespace biz.dfch.CS.Commons.Collections
{
    public static class CollectionUtils
    {
        public static bool NameValueCollectionComparer
        (
            NameValueCollection left
            ,
            NameValueCollection right
        )
        {
            Contract.Requires(null != left);
            Contract.Requires(null != right);

            return NameValueCollectionComparer(left, right, false);
        }

        public static bool NameValueCollectionComparer
        (
            NameValueCollection left
            ,
            NameValueCollection right
            ,
            bool exactOrder
        )
        {
            Contract.Requires(null != left);
            Contract.Requires(null != right);

            if(null == left || null == right)
            {
                return false;
            }

            var leftKeys = left.AllKeys;
            var rightKeys = right.AllKeys;
            
            if (leftKeys.Length != rightKeys.Length)
            {
                return false;
            }

            return exactOrder
                ? leftKeys
                    .SequenceEqual(rightKeys) && leftKeys.All(key => left[key] == right[key])
                : leftKeys
                    .OrderBy(key => key)
                    .SequenceEqual(rightKeys.OrderBy(key => key)) && leftKeys.All(key => left[key] == right[key]);
        }

        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right)
        {
            Contract.Requires(null != left);
            Contract.Requires(null != right);
            Contract.Ensures(null != Contract.Result<IDictionary<TKey, TValue>>());

            return Merge(left, right, MergeOptions.ThrowOnDuplicateKeys);
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public static IDictionary<TKey, TValue> Merge<TKey, TValue>(IDictionary<TKey, TValue> left, IDictionary<TKey, TValue> right, MergeOptions mergeOption)
        {
            Contract.Requires(null != left);
            Contract.Requires(null != right);
            Contract.Requires(Enum.IsDefined(typeof(MergeOptions), mergeOption));

            var mergedDictionary = left.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            Contract.Assert(null != mergedDictionary);

            foreach (var kvp in right)
            {
                switch (mergeOption)
                {
                    case MergeOptions.OverwriteLeft:
                        if (mergedDictionary.ContainsKey(kvp.Key))
                        {
                            mergedDictionary[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                            mergedDictionary.Add(kvp.Key, kvp.Value);
                        }
                        break;
                    
                    case MergeOptions.OverwriteRight:
                        if (!left.ContainsKey(kvp.Key))
                        {
                            mergedDictionary.Add(kvp.Key, kvp.Value);
                        }
                        break;
                    
                    case MergeOptions.ThrowOnDuplicateKeys:
                        Contract.Assert(!mergedDictionary.ContainsKey(kvp.Key), kvp.Key.ToString());
                        mergedDictionary.Add(kvp.Key, kvp.Value);
                        break;
                    
                    case MergeOptions.NullOnDuplicateKeys:
                        if (left.ContainsKey(kvp.Key))
                        {
                            return null;
                        }
                        break;
                    
                    case MergeOptions.Intersect:
                        if (!mergedDictionary.ContainsKey(kvp.Key))
                        {
                            mergedDictionary.Remove(kvp.Key);
                        }
                        break;
                    
                    case MergeOptions.Outersect:
                        if (mergedDictionary.ContainsKey(kvp.Key))
                        {
                            mergedDictionary.Remove(kvp.Key);
                        }
                        else
                        {
                            mergedDictionary.Add(kvp.Key, kvp.Value);
                        }
                        break;

                    default:
                        var isKnownMergeOption = false;
                        Contract.Assert(isKnownMergeOption);
                        break;
                }
            }

            if (MergeOptions.Intersect == mergeOption)
            {
                foreach (var kvp in left)
                {
                    if (!right.ContainsKey(kvp.Key))
                    {
                        mergedDictionary.Remove(kvp.Key);
                    }
                }
            }

            Contract.Assert(null != mergedDictionary);
            return mergedDictionary;
        }


        // this method is provided to facilitate calling from PowerShell
        public static DictionaryParameters Merge(DictionaryParameters left, DictionaryParameters right)
        {
            Contract.Requires(null != left);
            Contract.Requires(null != right);
            Contract.Ensures(null != Contract.Result<DictionaryParameters>());

            return Merge(left, right, MergeOptions.ThrowOnDuplicateKeys);
        }

        // this method is provided to facilitate calling from PowerShell
        public static DictionaryParameters Merge(DictionaryParameters left, DictionaryParameters right, MergeOptions mergeOption)
        {
            Contract.Requires(null != left);
            Contract.Requires(null != right);

            var mergedDictionary = Merge<string, object>(left, right, mergeOption);
            if (null == mergedDictionary)
            {
                return null;
            }

            var result = new DictionaryParameters();
            
            foreach (var kvp in mergedDictionary)
            {
                result.Add(kvp.Key, kvp.Value);
            }

            return result;
        }
    }
}
