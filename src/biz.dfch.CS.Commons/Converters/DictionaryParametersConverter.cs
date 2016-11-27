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

namespace biz.dfch.CS.Commons.Converters
{
    public class DictionaryParametersConverter
    {
        private static readonly IConvertibleBaseDtoConverter _converter;

        static DictionaryParametersConverter()
        {
            _converter = new ConvertibleBaseDtoConverter();
        }

        public static DictionaryParameters Convert(DictionaryParametersBaseDto dictionaryParametersBaseDto)
        {
            var result = _converter.Convert<DictionaryParametersKeyAttribute>(dictionaryParametersBaseDto, false);
            return result;
        }

        public static T Convert<T>(DictionaryParameters dictionaryParameters) 
            where T : DictionaryParametersBaseDto, new()
        {
            var result = _converter.Convert<T, DictionaryParametersKeyAttribute>(dictionaryParameters, false);
            return result;
        }

        public static DictionaryParameters Convert(DictionaryParametersBaseDto dictionaryParametersBaseDto, bool includeAllProperties)
        {
            var result = _converter.Convert<DictionaryParametersKeyAttribute>(dictionaryParametersBaseDto, includeAllProperties);
            return result;
        }

        public static T Convert<T>(DictionaryParameters dictionaryParameters, bool includeAllProperties) 
            where T : DictionaryParametersBaseDto, new()
        {
            var result = _converter.Convert<T, DictionaryParametersKeyAttribute>(dictionaryParameters, includeAllProperties);
            return result;
        }
    }
}
