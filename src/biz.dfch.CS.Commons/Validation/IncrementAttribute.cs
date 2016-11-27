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

namespace biz.dfch.CS.Commons.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IncrementAttribute : DataTypeValidationBaseAttribute
    {
        public double Increment { get; private set; }

        public string Expression { get; private set; }

        public IncrementAttribute(double increment)
        {
            Increment = increment;
        }

        public IncrementAttribute(double increment, string expression)
        {
            Increment = increment;
            Expression = expression;
        }

        public override bool IsValid(object value)
        {
            var result = false;
            try
            {
                var valueAsDecimal = (decimal) System.Convert.ChangeType(value, typeof(decimal), _formatProvider);
                
                result = 0 == (valueAsDecimal % (decimal) Increment);
                return result;
            }
            catch (Exception)
            {
                // N/A
            }

            return result;
        }

        public override string FormatErrorMessage(string name)
        {
            var baseMessage = base.FormatErrorMessage(name);
            var message = string.Format(_formatProvider, "{0} [increment '{1}']", baseMessage, Increment);
            return message;
        }
    }
}
