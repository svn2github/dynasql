using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Scryber.OpenType
{
    [StructLayout(LayoutKind.Explicit,Size=4,CharSet=CharSet.Ansi)]
    public struct BigEnd32
    {
        [FieldOffset(2)]
        private BigEnd16 _loword;
        public BigEnd16 LoWord
        {
            get { return _loword; }
        }

        [FieldOffset(0)]
        private BigEnd16 _hiword;
        public BigEnd16 HiWord
        {
            get { return _hiword; }
        }

        [FieldOffset(0)]
        private uint _val;
        public uint UnsignedValue
        {
            get { return _val; }
        }

        [FieldOffset(0)]
        private int _sval;

        public int SignedValue
        {
            get { return _sval; }
        }

        public BigEnd32(byte[] data)
            : this(data, 0)
        {
        }

        public BigEnd32(byte[] data, int offset)
        {
            this._sval = 0;
            this._val = 0;
            this._loword = new BigEnd16(data, offset);
            this._hiword = new BigEnd16(data, offset + 2);
        }
    }
}
