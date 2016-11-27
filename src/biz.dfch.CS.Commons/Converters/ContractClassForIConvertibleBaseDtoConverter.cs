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

using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace biz.dfch.CS.Commons.Converters
{
    [ContractClassFor(typeof(IConvertibleBaseDtoConverter))]
    public abstract class ContractClassForIConvertibleBaseDtoConverter : IConvertibleBaseDtoConverter
    {
        public DictionaryParameters Convert<TAttribute>(ConvertibleBaseDto convertibleDto)
            where TAttribute: ConversionKeyBaseAttribute
        {
            return default(DictionaryParameters);
        }

        public DictionaryParameters Convert<TAttribute>(ConvertibleBaseDto convertibleDto, bool includeAllProperties) 
            where TAttribute : ConversionKeyBaseAttribute
        {
            Contract.Requires(null != convertibleDto);
            Contract.Ensures(null != Contract.Result<DictionaryParameters>());

            return default(DictionaryParameters);
        }

        public T Convert<T, TAttribute>(DictionaryParameters dictionaryParameters)
            where T : ConvertibleBaseDto, new()
            where TAttribute : ConversionKeyBaseAttribute
        {
            return default(T);
        }

        public T Convert<T, TAttribute>(DictionaryParameters dictionaryParameters, bool includeAllProperties)
            where T : ConvertibleBaseDto, new()
            where TAttribute : ConversionKeyBaseAttribute
        {
            Contract.Requires(null != dictionaryParameters);
            Contract.Ensures(null != Contract.Result<T>());

            return default(T);
        }

        public IList<TBase> Convert<TBase, TAttribute>(DictionaryParameters dictionaryParameters, string nameSpace) 
            where TBase : ConvertibleBaseDto
            where TAttribute : ConversionKeyBaseAttribute
        {
            return default(IList<TBase>);
        }

        public IList<TBase> Convert<TBase, TAttribute>(DictionaryParameters dictionaryParameters, string nameSpace, bool includeAllProperties) 
            where TBase : ConvertibleBaseDto
            where TAttribute : ConversionKeyBaseAttribute
        {
            Contract.Requires(null != dictionaryParameters);
            Contract.Ensures(null != Contract.Result<IList<TBase>>());

            return default(IList<TBase>);
        }
    }
}
