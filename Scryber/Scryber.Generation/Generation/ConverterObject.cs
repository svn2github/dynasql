/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;

namespace Scryber.Generation
{
    internal static class ConvertObjects
    {
        internal static object ToInt32(string value, Type requiredType)
        {
            return int.Parse(value);
        }

        internal static object ToInt16(string value, Type requiredType)
        {
            return short.Parse(value);
        }

        internal static object ToInt64(string value, Type requiredType)
        {
            return long.Parse(value);
        }

        internal static object ToUInt32(string value, Type requiredType)
        {
            return uint.Parse(value);
        }

        internal static object ToUInt16(string value, Type requiredType)
        {
            return ushort.Parse(value);
        }

        internal static object ToUInt64(string value, Type requiredType)
        {
            return ulong.Parse(value);
        }

        internal static object ToFloat(string value, Type requiredType)
        {
            return float.Parse(value);
        }

        internal static object ToDouble(string value, Type requiredType)
        {
            return double.Parse(value);
        }

        internal static object ToDecimal(string value, Type requiredType)
        {
            return decimal.Parse(value);
        }

        internal static object ToString(string value, Type requiredType)
        {
            return value;
        }

        internal static object ToDateTime(string value, Type requiredType)
        {
            return DateTime.Parse(value);
        }

        internal static object ToTimeSpan(string value, Type requiredType)
        {
            return TimeSpan.Parse(value);
        }

        internal static object ToEnum(string value, Type requiredType)
        {
            if (value.IndexOf(' ') > -1)
                value = value.Replace(' ', ',');
            return Enum.Parse(requiredType, value);
        }

        internal static object ToByte(string value, Type requiredType)
        {
            return byte.Parse(value);
        }

        internal static object ToSByte(string value, Type requiredType)
        {
            return sbyte.Parse(value);
        }

        internal static object ToChar(string value, Type requiredType)
        {
            return char.Parse(value);
        }

        internal static object ToGuid(string value, Type requiredType)
        {
            return new Guid(value);
        }

        internal static object ToBool(string value, Type requiredType)
        {
            return bool.Parse(value);
        }

        internal static object ToDBNull(string value, Type requiredType)
        {
            return DBNull.Value;
        }

        internal static object ToUri(string value, Type requiredType)
        {
            return new Uri(value);
        }

       
    }
}
