using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public class PanoseArray
    {
        private byte[] _data;

        public PanoseArray()
            : this(new byte[10])
        {
        }

        public PanoseArray(byte[] data)
        {
            if (data == null || data.Length != 10)
                throw new ArgumentException("The Panose data should be a byte array of length 10");

            this._data = data;
        }

        public byte FamilyType { get { return this._data[0]; } set { this._data[0] = value; } }

        public byte SerifStyle { get { return this._data[1]; } set { this._data[1] = value; } }

        public byte Weight { get { return this._data[2]; } set { this._data[2] = value; } }

        public byte Proportion { get { return this._data[3]; } set { this._data[3] = value; } }

        public byte Contrast { get { return this._data[4]; } set { this._data[4] = value; } }

        public byte StrokeVariation { get { return this._data[5]; } set { this._data[5] = value; } }

        public byte ArmStyle { get { return this._data[6]; } set { this._data[6] = value; } }

        public byte Letterform { get { return this._data[7]; } set { this._data[7] = value; } }

        public byte Midline { get { return this._data[8]; } set { this._data[8] = value; } }

        public byte XHeight { get { return this._data[9]; } set { this._data[9] = value; } }
    }
}
