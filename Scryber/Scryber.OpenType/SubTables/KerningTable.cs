using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public class KerningTable : TTFTable
    {
        public KerningTable(long offset)
            : base(offset)
        {
        }

        private int _tablecount;

        public int TableCount
        {
            get { return _tablecount; }
            set { _tablecount = value; }
        }

        private List<KerningSubTable> _subs;

        public List<KerningSubTable> SubTables
        {
            get { return _subs; }
            set { _subs = value; }
        }

    }

    public class KerningSubTable
    {
        private Version _vers;

        public Version Version
        {
            get { return _vers; }
            set { _vers = value; }
        }

        private ushort _len;

        public ushort Length
        {
            get { return _len; }
            set { _len = value; }
        }

        private KerningCoverage _coverage;

        public KerningCoverage Coverage
        {
            get { return _coverage; }
            set { _coverage = value; }
        }

        private KerningFormat _format;

        public KerningFormat Format
        {
            get { return _format; }
            set { _format = value; }
        }

        private KerningFormatData _formatdata;

        public KerningFormatData KerningFormatData
        {
            get { return _formatdata; }
            set { _formatdata = value; }
        }

        public override string ToString()
        {
            string s = this.Coverage.ToString();
            if (string.IsNullOrEmpty(s))
                s = "VerticalData, KerningValues, Parallel";

            string format = this.Format.ToString();
            if (string.IsNullOrEmpty(format) == false)
                s = "Coverage {" + s + "}, Format {" + format + "}";
            else
                s = "Coverage {" + s + "}";

            return s;
        }

    }

    public abstract class KerningFormatData
    {
    }

    public class KerningFormat0 : KerningFormatData
    {
        private ushort _paircount;

        public ushort PairCount
        {
            get { return _paircount; }
            set { _paircount = value; }
        }

        private ushort _searchrange;

        public ushort SearchRange
        {
            get { return _searchrange; }
            set { _searchrange = value; }
        }

        private ushort _entrysel;

        public ushort EntrySelector
        {
            get { return _entrysel; }
            set { _entrysel = value; }
        }

        private ushort _rangeshift;

        public ushort RangeShift
        {
            get { return _rangeshift; }
            set { _rangeshift = value; }
        }

        private List<Kerning0Pair> _kernpair;

        public List<Kerning0Pair> KerningPairs
        {
            get { return _kernpair; }
            set { _kernpair = value; }
        }

        public override string ToString()
        {
            return this.PairCount.ToString() + " Format 0 kerning pairs";
        }
	
    }

    public class Kerning0Pair
    {
        private ushort _left;

        public ushort LeftGlyphIndex
        {
            get { return _left; }
            set { _left = value; }
        }

        private ushort _right;

        public ushort RightGlyphIndex
        {
            get { return _right; }
            set { _right = value; }
        }

        public override int GetHashCode()
        {
            return (this.LeftGlyphIndex << 16) + this.RightGlyphIndex;
        }

        private short _val;

        public short Value
        {
            get { return _val; }
            set { _val = value; }
        }

        public override string ToString()
        {
            return "(" + LeftGlyphIndex.ToString() + ", " + this.RightGlyphIndex + ") = " + this.Value.ToString();
        }
    }
}
