using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    
    public class UnicodeRanges : UIntBitRange
    {
        

        public UnicodeRanges(uint zero, uint one, uint two, uint three) : this(new uint[] {zero,one,two,three})
        {
            
        }

        public UnicodeRanges(uint[] data)
            : base(data, 4)
        { }

        public bool IsBitSet(UnicodeRangeBit bit)
        {
            return this.IsBitSet((int)bit);
        }

        public void SetBit(UnicodeRangeBit bit)
        {
            this.SetBit((int)bit);
        }

        public void ClearBit(UnicodeRangeBit bit)
        {
            this.ClearBit((int)bit);
        }

        public override string ToString()
        {
            return base.BuildString(typeof(UnicodeRangeBit), ", ");
        }

        public string Value
        {
            get { return this.ToString(); }
        }
        
    }
}
