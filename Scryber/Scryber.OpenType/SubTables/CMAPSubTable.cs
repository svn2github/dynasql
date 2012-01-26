using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public abstract class CMAPSubTable
    {
        private ushort _format;
        public ushort Format
        {
            get { return _format; }
        }

        public CMAPSubTable(ushort format)
        {
            this._format = format;
        }

        public int GetCharacterGlyphOffset(string s, int charIndex)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentNullException("s", "The string for getting the character offset cannot be null or empty");
            if (charIndex < 0 || charIndex >= s.Length)
                throw new ArgumentOutOfRangeException("charIndex", "The charIndex parameter must e between 0 and the length of the string -1");

            return this.GetCharacterGlyphOffset(s[charIndex]);
        }

        public abstract int GetCharacterGlyphOffset(char c);
    }
}
