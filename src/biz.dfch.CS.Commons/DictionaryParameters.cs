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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace biz.dfch.CS.Commons
{
    [Serializable]
    public class DictionaryParameters : Dictionary<string, object>, ISerializable
    {
        private const int MAX_RECURSION_COUNT = 64;
        private int recursionCount;

        public DictionaryParameters()
        {
            // N/A
        }

        public DictionaryParameters(Dictionary<string, object> dictionary)
        {
            Contract.Requires(null != dictionary);

            foreach (var item in dictionary)
            {
                Add(item.Key, item.Value);
            }
        }

        public DictionaryParameters(string json)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(json));

            var result = ConvertFromJson(json);
            foreach (var item in result)
            {
                Add(item.Key, item.Value);
            }
        }

        protected DictionaryParameters(SerializationInfo info, StreamingContext context)
             : base(info, context)
        {
            // N/A
        }

        public DictionaryParameters ConvertFromJson(string json)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(json));
            Contract.Ensures(null != Contract.Result<DictionaryParameters>());

            Contract.Assert(MAX_RECURSION_COUNT >= recursionCount);
            recursionCount++;

            var result = new DictionaryParameters();

            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            foreach (var dictionaryitem in dictionary)
            {
                if (null == dictionaryitem.Value)
                {
                    result.Add(dictionaryitem.Key, default(object));
                }
                else if(dictionaryitem.Value is JArray)
                {
                    var jarrValue = dictionaryitem.Value.ToString();
                    var jarr = JArray.Parse(jarrValue);
                    var list = new List<object>();
                    var jlist = jarr.ToList<object>();
                    foreach (var listitem in jlist)
                    {
                        if(null == listitem)
                        {
                            list.Add(default(object));
                        }
                        else if (listitem is JArray)
                        {

                        }
                        else if (listitem is JObject)
                        {
                            list.Add(ConvertFromJson(listitem.ToString()));
                        }
                        else
                        {
                            list.Add(listitem.ToString());
                        }
                    }

                    result.Add(dictionaryitem.Key, list);
                }
                else if(dictionaryitem.Value is JObject)
                {
                    var jobj = ConvertFromJson(dictionaryitem.Value.ToString());
                    result.Add(dictionaryitem.Key, jobj);
                }
                else
                {
                    result.Add(dictionaryitem.Key, dictionaryitem.Value);
                }
            }

            recursionCount--;

            return result;
        }

        public List<object> ConvertFromJsonArray(string json)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(json));
            Contract.Ensures(null != Contract.Result<List<object>>());

            var result = new List<object>();
            
            var jarr = JArray.Parse(json);
            var jlist = jarr.ToList<object>();
            foreach (var listitem in jlist)
            {
                if(null == listitem)
                {
                    result.Add(default(object));
                }
                else if (listitem is JArray)
                {
                    var jsonArray = ConvertFromJsonArray(json);
                    result.Add(jsonArray);
                }
                else if (listitem is JObject)
                {
                    var dictionaryParameters = ConvertFromJson(listitem.ToString());
                    result.Add(dictionaryParameters);
                }
                else
                {
                    result.Add(listitem.ToString());
                }
            }

            return result;
        }

        public T Convert<T>() 
            where T : new()
        {
            var t = new T();

            var propInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Contract.Assert(null != propInfos);
            
            foreach (var propInfo in propInfos)
            {
                object dictionaryPropertyValue;
                this.TryGetValue(propInfo.Name, out dictionaryPropertyValue);

                object propertyValue = null;
                var propertyType = propInfo.PropertyType;
                try
                {
                    if (dictionaryPropertyValue is IEnumerable)
                    {
                        propertyValue = dictionaryPropertyValue;
                    }
                    else if(dictionaryPropertyValue is IConvertible)
                    {
                        propertyValue = System.Convert.ChangeType(dictionaryPropertyValue, propertyType);
                    }
                    else
                    {
                        propertyValue = dictionaryPropertyValue;
                    }
                    propInfo.SetValue(t, propertyValue, null);

                    if (null == dictionaryPropertyValue && propInfo.CustomAttributes.Any())
                    {
                        var attribute = propInfo.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(DefaultValueAttribute));
                        if (null != attribute && 1 == attribute.ConstructorArguments.Count)
                        {
                            propInfo.SetValue(t, attribute.ConstructorArguments[0].Value, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Contract.Assert(null != propertyValue, string.Format(Message.DictionaryParameters_Convert__Conversion_FAILED, propInfo.Name, propertyType.Name, ex.Message));
                }
            }

            var context = new ValidationContext(t, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(t, context, results, true);
            if (!isValid)
            {
                foreach (var validationResult in results)
                {
                    Contract.Assert(isValid, string.Format(Message.DictionaryParameters_Convert__Object_Validation_FAILED, validationResult.ErrorMessage));
                }
            }

            return t;
        }

        public DictionaryParameters Convert<T>(T obj)
            where T : BaseDto
        {
            Contract.Requires(null != obj);
            Contract.Ensures(null != Contract.Result<DictionaryParameters>());

            var propInfos = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Contract.Assert(null != propInfos);

            foreach (var propInfo in propInfos)
            {
                var value = propInfo.GetValue(obj, null);

                this.Add(propInfo.Name, value);
            }

            return this;
        }

        public static bool IsValidJson(string json)
        {
            try
            {
                JToken.Parse(json);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
        }

        public string SerializeObject()
        {
            return JsonConvert.SerializeObject(this);
        }

        public string SerializeObject(JsonSerializerSettings jsonSerializerSettings)
        {
            Contract.Requires(null != jsonSerializerSettings);

            return JsonConvert.SerializeObject(this, jsonSerializerSettings);
        }

        public static object DeserializeObject(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type);
        }

        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static object DeserializeObject(string value, Type type, JsonSerializerSettings jsonSerializerSettings)
        {
            Contract.Requires(null != jsonSerializerSettings);

            return JsonConvert.DeserializeObject(value, type, jsonSerializerSettings);
        }

        public static T DeserializeObject<T>(string value, JsonSerializerSettings jsonSerializerSettings)
        {
            Contract.Requires(null != jsonSerializerSettings);

            return JsonConvert.DeserializeObject<T>(value, jsonSerializerSettings);
        }

        [Pure]
        public virtual bool IsValid()
        {
            if (0 < TryValidate().Count)
            {
                return false;
            }
            return true;
        }

        public virtual List<ValidationResult> GetValidationResults()
        {
            return TryValidate();
        }

        private List<ValidationResult> TryValidate()
        {
            var context = new ValidationContext(this, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(this, context, results, true);
            return results;
        }


        public virtual void Validate()
        {
            var results = TryValidate();
            var isValid = 0 >= results.Count ? true : false;

            if (isValid)
            {
                return;
            }

            foreach (var result in results)
            {
                Contract.Assert(isValid, result.ErrorMessage);
            }
        }

        public virtual object Get(string key)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));
            Contract.Requires(this.ContainsKey(key));

            var dictionaryParameterToBeRetrieved = this.First(k => k.Key == key);

            return dictionaryParameterToBeRetrieved.Value;
        }

        public virtual object GetOrDefault(string key, object defaultValue = default(object))
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));
            
            if(!this.ContainsKey(key))
            {
                return defaultValue;
            }

            var dictionaryParameterToBeRetrieved = this.First(k => k.Key == key);
            return dictionaryParameterToBeRetrieved.Value;
        }    

        public virtual object GetOrSelf(string key)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));
            
            if(!this.ContainsKey(key))
            {
                return this;
            }

            var dictionaryParameterToBeRetrieved = this.First(k => k.Key == key);
            return dictionaryParameterToBeRetrieved.Value;
        }

        public virtual DictionaryParameters Add(DictionaryParameters objectToMerge, bool replaceExistingKeys)
        {
            Contract.Requires(null != objectToMerge);
            Contract.Ensures(this == Contract.Result<DictionaryParameters>());

            if (replaceExistingKeys)
            {
                foreach (var key in objectToMerge.Keys)
                {
                    if (!ContainsKey(key))
                    {
                        continue;
                    }
                    var keyHasBeenRemoved = Remove(key);
                    Contract.Assert(keyHasBeenRemoved);
                }
            }
            else
            {
                foreach (var key in Keys)
                {
                    if (!objectToMerge.ContainsKey(key))
                    {
                        continue;
                    }
                    var keyHasBeenRemoved = objectToMerge.Remove(key);
                    Contract.Assert(keyHasBeenRemoved);
                }
            }

            var result = Add(objectToMerge);
            return result;
        }

        public virtual DictionaryParameters Add(DictionaryParameters objectToMerge)
        {
            Contract.Requires(null != objectToMerge);
            Contract.Ensures(this == Contract.Result<DictionaryParameters>());

            var result = this;

            objectToMerge.ToList().ForEach(p => result.Add(p.Key, p.Value));
            
            return result;
        }

        public virtual bool CanAdd(DictionaryParameters objectToMerge)
        {
            Contract.Requires(null != objectToMerge);

            var result = this.Keys.Intersect<string>(objectToMerge.Keys).FirstOrDefault();

            return null == result;
        }

        public override string ToString()
        {
            var result = JsonConvert.SerializeObject(this, Formatting.Indented);
            return result;
        }
    }
}
