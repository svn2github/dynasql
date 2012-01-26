using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public class CMAP_0_SubTable : CMAPSubTable
    {
        private ushort _len;

        public ushort Length
        {
            get { return _len; }
            set { _len = value; }
        }

        private ushort _lang;

        public ushort Language
        {
            get { return _lang; }
            set { _lang = value; }
        }

        private byte[] _glyphoffsets;

        public byte[] GlyphOffsets
        {
            get { return _glyphoffsets; }
            set { _glyphoffsets = value; }
        }

        public CMAP_0_SubTable(ushort format) : base(format)
        {
            if (format != 0)
                throw new ArgumentOutOfRangeException("format", "The format for the Apple Standard character set can only be 0");
        }

        public override int GetCharacterGlyphOffset(char c)
        {
            byte b = (byte)c;
            return _glyphoffsets[b];
        }
    }
}
