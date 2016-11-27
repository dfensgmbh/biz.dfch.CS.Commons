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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace biz.dfch.CS.Commons.Converters
{
    public class ConvertibleBaseDtoConverter : IConvertibleBaseDtoConverter
    {
        private static readonly MethodInfo _methodInfoConvertFromDictionary;
        private const string METHOD_NAME_CONVERT_FROM_DICTIONARY = "Convert";

        static ConvertibleBaseDtoConverter()
        {
            _methodInfoConvertFromDictionary = typeof(ConvertibleBaseDtoConverter)
                .GetMethod
                (
                    METHOD_NAME_CONVERT_FROM_DICTIONARY, 
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new [] { typeof(DictionaryParameters), typeof(bool) },
                    new ParameterModifier[] { }
                );
            Contract.Assert(null != _methodInfoConvertFromDictionary);
        }

        public T Convert<T, TAttribute>(DictionaryParameters dictionaryParameters)
            where T : ConvertibleBaseDto, new()
            where TAttribute : ConversionKeyBaseAttribute
        {
            return this.Convert<T, TAttribute>(dictionaryParameters, false);
        }

        public T Convert<T, TAttribute>(DictionaryParameters dictionaryParameters, bool includeAllProperties)
            where T : ConvertibleBaseDto, new()
            where TAttribute : ConversionKeyBaseAttribute
        {
            // create Dto to be returned
            var t = new T();

            // get all defined properties in Dto
            var propertyInfos = typeof(T)
                .GetProperties
                (
                    BindingFlags.Static | 
                    BindingFlags.Instance | 
                    BindingFlags.Public | 
                    BindingFlags.FlattenHierarchy
                );
            Contract.Assert(null != propertyInfos);

            foreach(var propertyInfo in propertyInfos)
            {
                object value;

                // get annotation of property
                var attribute = (TAttribute) Attribute.GetCustomAttribute(propertyInfo, typeof(TAttribute));
                
                // skip if no attribute found ...
                if(null == attribute)
                {
                    // but add property if we should include all properties
                    if(includeAllProperties)
                    {
                        // get value from DictionaryParameters
                        var couldValueFromPropertyInDicionaryParametersBeRetrieved = 
                            dictionaryParameters.TryGetValue(propertyInfo.Name, out value);
                        if(!couldValueFromPropertyInDicionaryParametersBeRetrieved)
                        {
                            continue;
                        }

                        // assign value from Dictionary key to our Dto
                        SetPropertyValue(propertyInfo, t, value);
                    }
                    continue;
                }

                // find property in DictionaryParameters
                // skip if key not found
                if (!dictionaryParameters.ContainsKey(attribute.Name))
                {
                    continue;
                }

                // get value from DictionaryParameters
                var couldValueFromExistingKeyInDicionaryParametersBeRetrieved = dictionaryParameters.TryGetValue(attribute.Name, out value);
                Contract.Assert(couldValueFromExistingKeyInDicionaryParametersBeRetrieved);
                
                // assign value from Dictionary key to our Dto
                SetPropertyValue(propertyInfo, t, value);
            }

            return t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetPropertyValue<T>(PropertyInfo propertyInfo, T type, object value)
            where T : ConvertibleBaseDto
        {
            try
            {
                propertyInfo.SetValue(type, value, null);
            }
            catch (ArgumentException)
            {
                object changedTypeValue;
                    
                var valueAsString = value as string;
                if (valueAsString != null)
                {
                    var convertibleString = new ConvertibleString(valueAsString);
                    changedTypeValue = System.Convert.ChangeType(convertibleString, propertyInfo.PropertyType, CultureInfo.InvariantCulture);
                }
                else
                {
                    changedTypeValue = System.Convert.ChangeType(value, propertyInfo.PropertyType, CultureInfo.InvariantCulture);
                }
                    
                propertyInfo.SetValue(type, changedTypeValue, null);

            }
        }

        public DictionaryParameters Convert<TAttribute>(ConvertibleBaseDto convertibleDto)
            where TAttribute : ConversionKeyBaseAttribute
        {
            return Convert<TAttribute>(convertibleDto, false);
        }

        public DictionaryParameters Convert<TAttribute>(ConvertibleBaseDto convertibleDto, bool includeAllProperties)
            where TAttribute: ConversionKeyBaseAttribute
        {
            // create Dto to be returned
            var dictionaryParameters = new DictionaryParameters();

            // get all defined properties in Dto
            var propertyInfos = convertibleDto
                .GetType()
                .GetProperties
                (
                    BindingFlags.Static | 
                    BindingFlags.Instance | 
                    BindingFlags.Public | 
                    BindingFlags.FlattenHierarchy
                );
            Contract.Assert(null != propertyInfos);

            foreach(var propertyInfo in propertyInfos)
            {
                // get annotation of property
                var attribute = (TAttribute) Attribute.GetCustomAttribute(propertyInfo, typeof(TAttribute));
                
                // skip if no attribute found ...
                if(null == attribute)
                {
                    // but add property if we should include all properties
                    if(includeAllProperties)
                    {
                        dictionaryParameters.Add(propertyInfo.Name, propertyInfo.GetValue(convertibleDto, null));
                    }
                    continue;
                }

                // assert that we do not have the annotated property in our dictionary
                Contract.Assert(!dictionaryParameters.ContainsKey(attribute.Name));

                // add value to dictionary
                dictionaryParameters.Add(attribute.Name, propertyInfo.GetValue(convertibleDto, null));
            }

            return dictionaryParameters;
        }

        public IList<TBase> Convert<TBase, TAttribute>(DictionaryParameters dictionaryParameters, string version)
            where TBase : ConvertibleBaseDto
            where TAttribute : ConversionKeyBaseAttribute
        {
            return Convert<TBase, TAttribute>(dictionaryParameters, version, false);
        }

        // returns a list of all deserialised DTOs based on TBase
        // if nameSpace is null then the namespace of TBase is used for resolving derived types
        public IList<TBase> Convert<TBase, TAttribute>(DictionaryParameters dictionaryParameters, string nameSpace, bool includeAllProperties)
            where TBase : ConvertibleBaseDto
            where TAttribute : ConversionKeyBaseAttribute
        {
            // create list to be returned
            var tBaseList = new List<TBase>();

            if(string.IsNullOrWhiteSpace(nameSpace))
            {
                nameSpace = typeof(TBase).Namespace;
                Contract.Assert(!string.IsNullOrWhiteSpace(nameSpace));
            }

            var types = GetDerivedTypes<TBase>(typeof(TBase).Assembly, nameSpace);

            foreach (var type in types)
            {
                var genericMethodInfo = _methodInfoConvertFromDictionary.MakeGenericMethod(type, typeof(TAttribute));
                Contract.Assert(null != genericMethodInfo, typeof(TAttribute).FullName);
                
                var dtoDerivedFromTBase = (TBase) genericMethodInfo.Invoke(this, new object[] { dictionaryParameters, includeAllProperties });
                tBaseList.Add(dtoDerivedFromTBase);
            }
            
            return tBaseList;
        }

        private IList<Type> GetDerivedTypes<TBase>(string nameSpace)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var typesPerAssembly = GetDerivedTypes<TBase>(assembly, nameSpace);
                types.AddRange(typesPerAssembly);
            }
            return types;
        }
        
        private IEnumerable<Type> GetDerivedTypes<TBase>(Assembly assembly, string nameSpace)
        {
            var types = assembly
                .GetTypes()
                .Where
                (
                    e => 
                        (e.IsPublic || e.IsNested) &&
                        !e.IsAbstract && 
                        e.BaseType == typeof(TBase) && 
                        !string.IsNullOrWhiteSpace(e.Namespace) && 
                        e.Namespace == nameSpace
                );
            return types;
        }
    }
}
