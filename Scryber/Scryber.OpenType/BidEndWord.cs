using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Scryber.OpenType
{
    [StructLayout(LayoutKind.Explicit, Size = 2, CharSet = CharSet.Ansi)]
    public struct BigEnd16
    {
        [FieldOffset(1)]
        private byte _lobyte;
        public byte LoByte
        {
            get { return _lobyte; }
            set { _lobyte = value; }
        }

        [FieldOffset(0)]
        private byte _hibyte;
        public byte HiByte
        {
            get { return _hibyte; }
            set { _hibyte = value; }
        }

        [FieldOffset(0)]
        private ushort _value;
        public ushort UnsignedValue
        {
            get { return _value; }
            set { _value = value; }
        }

        [FieldOffset(0)]
        private short _sval;

        public short SignedValue
        {
            get { return _sval; }
            set { _sval = value; }
        }

        public BigEnd16(byte[] bigenddata)
            : this(bigenddata, 0)
        {
        }

        public BigEnd16(byte[] bigenddata, int offset)
        {
            this._sval = 0;
            this._value = 0;
            this._lobyte = bigenddata[offset++];
            this._hibyte = bigenddata[offset];
        }

    }
}
