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
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;

namespace biz.dfch.CS.Commons.Converters
{
    public class EnvironmentVariableConverter
    {
        private const string METHOD_NAME_CONVERT_FROM_DICTIONARY = "Convert";
        private static readonly MethodInfo _methodInfoConvertFromDictionary;

        private static readonly IConvertibleBaseDtoConverter _converter;

        static EnvironmentVariableConverter()
        {
            _converter = new ConvertibleBaseDtoConverter();

            // determine generic Convert method and convert it to non-generic method
            // so it can easily be used from PowerShell
            _methodInfoConvertFromDictionary = typeof(EnvironmentVariableConverter)
                .GetMethod
                (
                    METHOD_NAME_CONVERT_FROM_DICTIONARY, 
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new [] { typeof(DictionaryParameters), typeof(bool) },
                    new ParameterModifier[] { }
                );
            Contract.Assert(null != _methodInfoConvertFromDictionary);
        }

        public static T Convert<T>(string jsonString) where T: EnvironmentVariableBaseDto, new()
        {
            var dictionaryParameters = new DictionaryParameters(jsonString);
            return Convert<T>(dictionaryParameters);
        }

        public static T Convert<T>(DictionaryParameters dictionaryParameters) 
            where T : EnvironmentVariableBaseDto, new()
        {
            var result = _converter.Convert<T, EnvironmentVariableAttribute>(dictionaryParameters, false);
            return result;
        }

        public static T Convert<T>(DictionaryParameters dictionaryParameters, bool includeAllProperties) 
            where T : EnvironmentVariableBaseDto, new()
        {
            var result = _converter.Convert<T, EnvironmentVariableAttribute>(dictionaryParameters, includeAllProperties);
            return result;
        }

        public static EnvironmentVariableBaseDto Convert(Type type, DictionaryParameters dictionaryParameters)
        {
            Contract.Requires(null != type);
            Contract.Ensures(null != Contract.Result<EnvironmentVariableBaseDto>());

            var genericMethodInfo = _methodInfoConvertFromDictionary.MakeGenericMethod(type);
            Contract.Assert(null != genericMethodInfo, type.FullName);

            var result = (EnvironmentVariableBaseDto) genericMethodInfo.Invoke(typeof(EnvironmentVariableConverter), new object[] { dictionaryParameters, false });
            return result;
        }

        public static EnvironmentVariableBaseDto Convert(Type type, DictionaryParameters dictionaryParameters, bool includeAllProperties) 
        {
            Contract.Requires(null != type);
            Contract.Ensures(null != Contract.Result<EnvironmentVariableBaseDto>());

            var genericMethodInfo = _methodInfoConvertFromDictionary.MakeGenericMethod(type);
            Contract.Assert(null != genericMethodInfo, type.FullName);

            var result = (EnvironmentVariableBaseDto) genericMethodInfo.Invoke(typeof(EnvironmentVariableConverter), new object[] { dictionaryParameters, includeAllProperties });
            return result;
        }

        public static DictionaryParameters Convert(EnvironmentVariableBaseDto environmentVariableBaseDto)
        {
            var result = _converter.Convert<EnvironmentVariableAttribute>(environmentVariableBaseDto, false);
            return result;
        }

        public static DictionaryParameters Convert(EnvironmentVariableBaseDto environmentVariableBaseDto, bool includeAllProperties)
        {
            var result = _converter.Convert<EnvironmentVariableAttribute>(environmentVariableBaseDto, includeAllProperties);
            return result;
        }

        public static IList<TBase> Convert<TBase>(DictionaryParameters dictionaryParameters, string version)
            where TBase : EnvironmentVariableBaseDto
        {
            var result = _converter.Convert<TBase, EnvironmentVariableAttribute>(dictionaryParameters, version);
            return result;
        }

        public static IList<TBase> Convert<TBase>(DictionaryParameters dictionaryParameters, string version, bool includeAllProperies)
            where TBase : EnvironmentVariableBaseDto
        {
            var result = _converter.Convert<TBase, EnvironmentVariableAttribute>(dictionaryParameters, version, includeAllProperies);
            return result;
        }

        public static void Import(EnvironmentVariableBaseDto environmentVariableBaseDto)
        {
            Contract.Requires(null != environmentVariableBaseDto);

            var propertyInfos = environmentVariableBaseDto
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            
            foreach (var propertyInfo in propertyInfos)
            {
                var attribute = (EnvironmentVariableAttribute) Attribute.GetCustomAttribute(propertyInfo, typeof(EnvironmentVariableAttribute));
                if (null == attribute)
                {
                    continue;
                }

                var stringValue = Environment.GetEnvironmentVariable(attribute.Name);
                if (null == stringValue)
                {
                    continue;
                }

                try
                {
                    propertyInfo.SetValue(environmentVariableBaseDto, stringValue, null);
                }
                catch (ArgumentException)
                {
                    var changedTypeValue = System.Convert.ChangeType(stringValue, propertyInfo.PropertyType, CultureInfo.InvariantCulture);
                    
                    propertyInfo.SetValue(environmentVariableBaseDto, changedTypeValue, null);
                }
            }
        }

        public static void Export(EnvironmentVariableBaseDto environmentVariableBaseDto)
        {
            Contract.Requires(null != environmentVariableBaseDto);

            var propertyInfos = environmentVariableBaseDto
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            
            foreach (var propertyInfo in propertyInfos)
            {
                var attribute = (EnvironmentVariableAttribute) Attribute.GetCustomAttribute(propertyInfo, typeof(EnvironmentVariableAttribute));
                if (null == attribute)
                {
                    continue;
                }

                var value = propertyInfo.GetValue(environmentVariableBaseDto, null);
                Environment.SetEnvironmentVariable(attribute.Name, value.ToString());
            }
        }

    }
}
