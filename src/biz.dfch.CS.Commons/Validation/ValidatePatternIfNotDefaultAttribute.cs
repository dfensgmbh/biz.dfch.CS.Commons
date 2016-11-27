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
using System.Diagnostics.Contracts;
using System.Text.RegularExpressions;

namespace biz.dfch.CS.Commons.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidatePatternIfNotDefaultAttribute : DataTypeValidationBaseAttribute
    {
        public readonly string Pattern;

        public ValidatePatternIfNotDefaultAttribute(string value)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(value));

            Pattern = value;
        }

        public override bool IsValid(object value)
        {
            if (!(value is string))
            {
                return true;
            }
            
            // ReSharper disable once TryCastAlwaysSucceeds
            var result = Regex.IsMatch(value as string, this.Pattern);
            
            return result;
        }

        public override string FormatErrorMessage(string name)
        {
            var baseMessage = base.FormatErrorMessage(name);
            var message = string.Format("{0} ['{1}']", baseMessage, Pattern);
            return message;
        }
    }
}
