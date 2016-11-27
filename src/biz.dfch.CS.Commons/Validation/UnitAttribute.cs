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

namespace biz.dfch.CS.Commons.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class UnitAttribute : DataTypeValidationBaseAttribute
    {
        public string Unit { get; private set; }

        public UnitAttribute(string value)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(value));
            
            Unit = value;
        }

        public override bool IsValid(object value)
        {
            return true;
        }
    }
}
