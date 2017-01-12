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
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using Newtonsoft.Json;

namespace biz.dfch.CS.Commons.Converters
{
    [JsonObject]
    [Serializable]
    public abstract class ConversionKeyBaseDto : ConvertibleBaseDto, ISerializable
    {
        // this class acts only as a base for Dtos 
        // that need to convert ConversionKey entities

        protected ConversionKeyBaseDto()
        {
            // public, parameter-less empty default constructor needed
        }

        protected ConversionKeyBaseDto(SerializationInfo info, StreamingContext context)
        {
            // N/A
        }
        
        public DictionaryParameters Convert()
        {
            var result = Converter.Convert<ConversionKeyAttribute>(this, false);
            return result;
        }

        public DictionaryParameters Convert(bool includeAllProperties)
        {
            var result = Converter.Convert<ConversionKeyAttribute>(this, includeAllProperties);
            return result;
        }

        [SecurityCritical]
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var propertyInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in propertyInfos)
            {
                var name = propertyInfo.Name;
                var value = propertyInfo.GetValue(this, null);
                info.AddValue(name, value);
            }
        }
        
        [SecurityCritical]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (null == info)
            {
                throw new ArgumentNullException(nameof(info));
            }

            GetObjectData(info, context);
        }
    }
}
