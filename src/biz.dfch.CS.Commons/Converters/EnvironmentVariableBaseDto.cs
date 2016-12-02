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

using Newtonsoft.Json;

namespace biz.dfch.CS.Commons.Converters
{
    public abstract class EnvironmentVariableBaseDto : ConvertibleBaseDto
    {
        // this class acts only as a base for Dtos 
        // that need to convert ConversionKey entities

        public DictionaryParameters Convert()
        {
            var result = Converter.Convert<EnvironmentVariableAttribute>(this, false);
            return result;
        }

        public DictionaryParameters Convert(bool includeAllProperties)
        {
            var result = Converter.Convert<EnvironmentVariableAttribute>(this, includeAllProperties);
            return result;
        }

        public void Import()
        {
            EnvironmentVariableConverter.Import(this);
        }

        public void Export()
        {
            EnvironmentVariableConverter.Export(this);
        }
    }
}
