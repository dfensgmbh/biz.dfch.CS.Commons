/**
 * Copyright 2015 d-fens GmbH
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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace biz.dfch.CS.Commons
{
    public abstract class BaseDto
    {
        private const int VALIDATION_COUNT_ZERO = 0;

        protected BaseDto()
        {
            var propInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Contract.Assert(null != propInfos);

            foreach (var propInfo in propInfos)
            {
                var attribute = propInfo.GetCustomAttribute<DefaultValueAttribute>();
                if (null == attribute)
                {
                    continue;
                }

                propInfo.SetValue(this, attribute.Value);
            }
        }
            
        public virtual string SerializeObject()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static object DeserializeObject(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type);
        }

        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        [Pure]
        public virtual bool IsValid()
        {
            return VALIDATION_COUNT_ZERO >= TryValidate().Count;
        }

        [Pure]
        public virtual bool IsValid(string propertyName)
        {
            return VALIDATION_COUNT_ZERO >= TryValidate(propertyName).Count;
        }

        [Pure]
        public virtual bool IsValid(string propertyName, object value)
        {
            return VALIDATION_COUNT_ZERO >= TryValidate(propertyName, value).Count;
        }

        public virtual IList<ValidationResult> GetValidationResults()
        {
            Contract.Ensures(null != Contract.Result<IList<ValidationResult>>());

            return TryValidate();
        }

        public virtual IList<ValidationResult> GetValidationResults(string propertyName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(propertyName));
            Contract.Ensures(null != Contract.Result<IList<ValidationResult>>());

            return TryValidate(propertyName);
        }

        public virtual IList<ValidationResult> GetValidationResults(string propertyName, object value)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(propertyName));
            Contract.Ensures(null != Contract.Result<IList<ValidationResult>>());

            return TryValidate(propertyName, value);
        }

        public virtual IList<string> GetErrorMessages()
        {
            Contract.Ensures(null != Contract.Result<IList<string>>());

            return TryValidate().Select(result => result.ErrorMessage).ToList();
        }
        
        public virtual IList<string> GetErrorMessages(string propertyName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(propertyName));
            Contract.Ensures(null != Contract.Result<IList<string>>());

            return TryValidate(propertyName).Select(result => result.ErrorMessage).ToList();
        }
        
        public virtual IList<string> GetErrorMessages(string propertyName, object value)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(propertyName));
            Contract.Ensures(null != Contract.Result<IList<string>>());

            var results = TryValidate(propertyName, value);

            return results.Select(result => result.ErrorMessage).ToList();
        }
        
        private IList<ValidationResult> TryValidate()
        {
            var context = new ValidationContext(this, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(this, context, results, true);
            return results;
        }

        private IList<ValidationResult> TryValidate(string propertyName)
        {
            var context = new ValidationContext(this, serviceProvider: null, items: null)
            {
                MemberName = propertyName
            };

            var propertyInfo = GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            Contract.Assert(null != propertyInfo);
            var value = propertyInfo.GetValue(this, null);

            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateProperty(value, context, results);
            return results;
        }

        private IList<ValidationResult> TryValidate(string propertyName, object value)
        {
            var context = new ValidationContext(this, serviceProvider: null, items: null)
            {
                MemberName = propertyName
            };

            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateProperty(value, context, results);
            return results;
        }

        public virtual void Validate()
        {
            var results = TryValidate();
            var isValid = VALIDATION_COUNT_ZERO >= results.Count;

            if (isValid)
            {
                return;
            }

            Contract.Assert(isValid, results[0].ErrorMessage);
        }

        public virtual void Validate(string propertyName)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(propertyName));

            var results = TryValidate(propertyName);
            var isValid = VALIDATION_COUNT_ZERO >= results.Count;

            if (isValid)
            {
                return;
            }

            Contract.Assert(isValid, results[0].ErrorMessage);
        }

        public virtual void Validate(string propertyName, object value)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(propertyName));

            var results = TryValidate(propertyName, value);
            var isValid = VALIDATION_COUNT_ZERO >= results.Count;

            if (isValid)
            {
                return;
            }

            Contract.Assert(isValid, results[0].ErrorMessage);
        }
    }
}
