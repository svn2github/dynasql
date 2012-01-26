using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public class PostscriptTable : TTFTable
    {

        public PostscriptTable(long offset)
            : base(offset)
        {

        }

        private float _italangle;

        public float ItalicAngle
        {
            get { return _italangle; }
            set { _italangle = value; }
        }

        private short _underlinepos;

        public short UnderlinePosition
        {
            get { return _underlinepos; }
            set { _underlinepos = value; }
        }

        private short _uthick;

        public short UnderlineThickness
        {
            get { return _uthick; }
            set { _uthick = value; }
        }

        private uint _fixedpitch;

        public uint FixedPitch
        {
            get { return _fixedpitch; }
            set { _fixedpitch = value; }
        }

        public bool IsMonoSpaced
        {
            get { return FixedPitch != 0; }
        }

        private uint _minmemTT;

        public uint MinMemoryOpenType
        {
            get { return _minmemTT; }
            set { _minmemTT = value; }
        }

        private uint _maxmemTT;

        public uint MaxMemoryOpenType
        {
            get { return _maxmemTT; }
            set { _maxmemTT = value; }
        }

        private uint _minmemT1;

        public uint MinMemoryType1
        {
            get { return _minmemT1; }
            set { _minmemT1 = value; }
        }

        private uint _maxmemT1;

        public uint MaxMemoryType1
        {
            get { return _maxmemT1; }
            set { _maxmemT1 = value; }
        }


        private List<GlyphName> _names;

        public List<GlyphName> Names
        {
            get { return _names; }
            set { _names = value; }
        }

    }

    public class GlyphName
    {
        private int _index;

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return "[empty] {" + this.Index.ToString() + "}";
            }
            else
                return this.Name + " {" + this.Index.ToString() + "}";
        }

    }
}
