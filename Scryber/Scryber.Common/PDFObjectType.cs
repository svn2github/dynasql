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
using System.Runtime.InteropServices;

namespace Scryber
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PDFObjectType : IEquatable<PDFObjectType>, IComparable, IComparable<PDFObjectType>
    {
        [FieldOffset(0)]
        private byte _zero;

        [FieldOffset(1)]
        private byte _one;

        [FieldOffset(2)]
        private byte _two;

        [FieldOffset(3)]
        private byte _three;

        [FieldOffset(0)]
        private int _value;
        

        public PDFObjectType(string type)
            : this(GetValidType(type))
        {
        }


        public PDFObjectType (char[] type) : this(ConvertToBytes(type))
        {
        }

        private PDFObjectType(byte[] bytes)
        {
            this._value = 0;
            this._zero = bytes[0];
            this._one = bytes[1];
            this._two = bytes[2];
            this._three = bytes[3];
        }

        

        public override string ToString()
        {
            return new string(new char[4] { (char)_zero, (char)_one, (char)_two, (char)_three });
        }

        public override bool Equals(object obj)
        {
            if (null == obj || !(obj is PDFObjectType))
                return false;
            else
                return this.Equals((PDFObjectType)obj);
        }

        public bool Equals(PDFObjectType type)
        {
            return this._value.Equals(type._value);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        int IComparable.CompareTo(object obj)
        {
            if (null == obj || !(obj is PDFObjectType))
                return -1;
            else
                return CompareTo((PDFObjectType)obj);
        }

        public int CompareTo(PDFObjectType other)
        {
            return this._value.CompareTo(other._value);
        }

        private static char[] GetValidType(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s", String.Format(Errors.TypeStringOnlyNChars, 4, ""));
            else if (s.Length != 4)
                throw new ArgumentOutOfRangeException("s", String.Format(Errors.TypeStringOnlyNChars, 4, s));
            else
                return new char[] { s[0], s[1], s[2], s[3] };
        }

        public static byte[] ConvertToBytes(char[] chars)
        {
            if (chars == null)
                throw new ArgumentNullException("chars", String.Format(Errors.TypeStringOnlyNChars, 4, ""));
            else if (chars.Length != 4)
                throw new ArgumentOutOfRangeException("chars", String.Format(Errors.TypeStringOnlyNChars, 4, new string(chars)));
            else
            {
                byte[] bytes = new byte[4];
                bytes[0] = (byte)chars[0];
                bytes[1] = (byte)chars[1];
                bytes[2] = (byte)chars[2];
                bytes[3] = (byte)chars[3];
                return bytes;
            }
        }

        public static readonly PDFObjectType Empty = new PDFObjectType();

        public static bool operator ==(PDFObjectType one, PDFObjectType two)
        {
            return one.Equals(two);
        }

        public static bool operator !=(PDFObjectType one, PDFObjectType two)
        {
            return one.Equals(two) == false;
        }

        public static bool Equals(PDFObjectType one, PDFObjectType two)
        {
            return one.Equals(two);
        }


        public static explicit operator PDFObjectType(string s)
        {
            return new PDFObjectType(s);
        }

        public static PDFObjectType FromString(string s)
        {
            return new PDFObjectType(s);
        }
        
    }
}
