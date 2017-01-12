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
using System.Reflection;

namespace biz.dfch.CS.Commons.Converters
{
    public class ConversionKeyConverter
    {
        private static readonly MethodInfo _methodInfoConvertFromDictionary;

        private static readonly IConvertibleBaseDtoConverter _converter;

        static ConversionKeyConverter()
        {
            _converter = new ConvertibleBaseDtoConverter();

            // determine generic Convert method and convert it to non-generic method
            // so it can easily be used from PowerShell
            _methodInfoConvertFromDictionary = typeof(ConversionKeyConverter)
                .GetMethod
                (
                    nameof(ConversionKeyConverter.Convert), 
                    BindingFlags.Public | BindingFlags.Static,
                    null,
                    new [] { typeof(DictionaryParameters), typeof(bool) },
                    new ParameterModifier[] { }
                );
            Contract.Assert(null != _methodInfoConvertFromDictionary);
        }

        public static T Convert<T>(string jsonString) where T: ConversionKeyBaseDto, new()
        {
            var dictionaryParameters = new DictionaryParameters(jsonString);
            return Convert<T>(dictionaryParameters);
        }

        public static T Convert<T>(DictionaryParameters dictionaryParameters) 
            where T : ConversionKeyBaseDto, new()
        {
            var result = _converter.Convert<T, ConversionKeyAttribute>(dictionaryParameters, false);
            return result;
        }

        public static T Convert<T>(DictionaryParameters dictionaryParameters, bool includeAllProperties) 
            where T : ConversionKeyBaseDto, new()
        {
            var result = _converter.Convert<T, ConversionKeyAttribute>(dictionaryParameters, includeAllProperties);
            return result;
        }

        public static ConversionKeyBaseDto Convert(Type type, DictionaryParameters dictionaryParameters)
        {
            Contract.Requires(null != type);
            Contract.Ensures(null != Contract.Result<ConversionKeyBaseDto>());

            var genericMethodInfo = _methodInfoConvertFromDictionary.MakeGenericMethod(type);
            Contract.Assert(null != genericMethodInfo, type.FullName);

            var result = (ConversionKeyBaseDto) genericMethodInfo.Invoke(typeof(ConversionKeyConverter), new object[] { dictionaryParameters, false });
            return result;
        }

        public static ConversionKeyBaseDto Convert(Type type, DictionaryParameters dictionaryParameters, bool includeAllProperties) 
        {
            Contract.Requires(null != type);
            Contract.Ensures(null != Contract.Result<ConversionKeyBaseDto>());

            var genericMethodInfo = _methodInfoConvertFromDictionary.MakeGenericMethod(type);
            Contract.Assert(null != genericMethodInfo, type.FullName);

            var result = (ConversionKeyBaseDto) genericMethodInfo.Invoke(typeof(ConversionKeyConverter), new object[] { dictionaryParameters, includeAllProperties });
            return result;
        }

        public static DictionaryParameters Convert(ConversionKeyBaseDto conversionKeyBaseDto)
        {
            var result = _converter.Convert<ConversionKeyAttribute>(conversionKeyBaseDto, false);
            return result;
        }

        public static DictionaryParameters Convert(ConversionKeyBaseDto conversionKeyBaseDto, bool includeAllProperties)
        {
            var result = _converter.Convert<ConversionKeyAttribute>(conversionKeyBaseDto, includeAllProperties);
            return result;
        }

        public static IList<TBase> Convert<TBase>(DictionaryParameters dictionaryParameters, string version)
            where TBase : ConversionKeyBaseDto
        {
            var result = _converter.Convert<TBase, ConversionKeyAttribute>(dictionaryParameters, version);
            return result;
        }

        public static IList<TBase> Convert<TBase>(DictionaryParameters dictionaryParameters, string version, bool includeAllProperies)
            where TBase : ConversionKeyBaseDto
        {
            var result = _converter.Convert<TBase, ConversionKeyAttribute>(dictionaryParameters, version, includeAllProperies);
            return result;
        }
    }
}
