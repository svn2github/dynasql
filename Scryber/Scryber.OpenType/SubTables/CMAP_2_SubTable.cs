using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public class CMAP_2_SubTable : CMAPSubTable
    {

        public CMAP_2_SubTable(ushort format)
            : base(format)
        {
            if (format != 2)
                throw new ArgumentOutOfRangeException("format", "The format for the High-Byte mapping table can only be 2");

        }

        public override int GetCharacterGlyphOffset(char c)
        {
            throw new NotSupportedException("Searching of the format 2 (High-Byte mapping) table is not supported");
        }
    }
}
