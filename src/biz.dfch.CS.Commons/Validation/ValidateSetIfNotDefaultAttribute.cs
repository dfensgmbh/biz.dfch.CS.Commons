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

namespace biz.dfch.CS.Commons.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateSetIfNotDefaultAttribute : DataTypeValidationBaseAttribute
    {
        private bool isInitialised = false;
        public HashSet<string> Elements { get; private set; }

        public const char SEPARATOR = '|';

        public ValidateSetIfNotDefaultAttribute(params string[] set)
        {
            Elements = new HashSet<string>();

            if (null == set || 0 >= set.Length)
            {
                return;
            }
            
            foreach (var element in set)
            {
                Elements.Add(element);
            }

            isInitialised = true;
        }
        
        public ValidateSetIfNotDefaultAttribute(string set)
            : this(set.Split(SEPARATOR))
        {
            // N/A
        }

        public ValidateSetIfNotDefaultAttribute(Type enumType)
        {
            Elements = new HashSet<string>();
            
            if (!enumType.IsEnum)
            {
                return;
            }

            foreach (var element in enumType.GetEnumValues())
            {
                Elements.Add(element.ToString());
            }

            isInitialised = true;
        }
        
        public override bool IsValid(object value)
        {
            if (!isInitialised)
            {
                return false;
            }

            if (!(value is string))
            {
                return true;
            }

            // ReSharper disable once TryCastAlwaysSucceeds
            var result = Elements.Contains(value as string);
            return result;
        }

        public override string FormatErrorMessage(string name)
        {
            var baseMessage = base.FormatErrorMessage(name);
            var message = string.Format("{0} [ValidateSet '{1}']", baseMessage, string.Join(SEPARATOR.ToString(), Elements));
            return message;
        }
    }
}
