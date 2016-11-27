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
using System.Globalization;
using biz.dfch.CS.Commons.Validation;

namespace biz.dfch.CS.Commons.Converters
{
    public class ConvertibleString : IConvertible
    {
        private readonly string property;
        
        public ConvertibleString(string value)
        {
            property = value;
        }

        public TypeCode GetTypeCode()
        {
            return property.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(property, provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            if (1 != property.Length)
            {
                throw new InvalidCastException();
            }
            
            return property[0];
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            if (1 != property.Length)
            {
                throw new InvalidCastException();
            }

            return (sbyte) property[0];
        }

        public byte ToByte(IFormatProvider provider)
        {
            if (1 != property.Length)
            {
                throw new InvalidCastException();
            }

            return (byte) property[0];
        }

        public short ToInt16(IFormatProvider provider)
        {
            return short.Parse(property, provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ushort.Parse(property, provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return int.Parse(property, provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return uint.Parse(property, provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return long.Parse(property, provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ulong.Parse(property, provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return float.Parse(property, provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return double.Parse(property.Replace(",", "."), CultureInfo.InvariantCulture);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return decimal.Parse(property, provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return DateTime.Parse(property, provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return property;
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            object result;

            if (typeof(DateRange) == conversionType)
            {
                result = new DateRange(property);
            }
            else if (typeof(DateTimeRange) == conversionType)
            {
                result = new DateTimeRange(property);
            }
            else
            {
                result = property;
            }

            return result;
        }
    }
}
