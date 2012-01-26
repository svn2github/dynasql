using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public class CMAPTable : TTFTable
    {
        public CMAPTable(long offset)
            : base(offset)
        {
        }

        private ushort _numTables;

        public ushort NumberOfTables
        {
            get { return _numTables; }
            set { _numTables = value; }
        }

        private CMAPRecordList _rec;

        public CMAPRecordList Records
        {
            get 
            {
                if (_rec == null)
                    _rec = new CMAPRecordList();
                return _rec;
            }
            set { _rec = value; }
        }

        public CMAPSubTable GetOffsetTable(CharacterPlatforms platform, CharacterEncodings enc)
        {
            foreach (CMAPRecord rec in this.Records)
            {
                if (rec.Platform == platform && rec.Encoding == enc)
                    return rec.SubTable;
            }
            return null;
        }

    }

    public class CMAPRecord
    {
        private CharacterPlatforms _platform;

        public CharacterPlatforms Platform
        {
            get { return _platform; }
            set { _platform = value; }
        }

        private CharacterEncodings _enc;

        public CharacterEncodings Encoding
        {
            get { return _enc; }
            set { _enc = value; }
        }

        private uint _offset;

        public uint MapOffset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        private CMAPSubTable _subtable;

        public CMAPSubTable SubTable
        {
            get { return _subtable; }
            set { _subtable = value; }
        }


        public override string ToString()
        {
            return Platform.ToString() + " " + Encoding.ToString();
        }

    }

    public class CMAPRecordList : List<CMAPRecord>
    {

        public new CMAPRecord this[int index]
        {
            get { return base[index]; }
            set { base[index] = value; }
        }
	
        public CMAPRecordList()
            : base()
        {
        }

        public CMAPRecordList(IEnumerable<CMAPRecord> records)
            : base(records)
        { }
    }
}
