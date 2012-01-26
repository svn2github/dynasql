using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType
{
    public class TTFDirectory
    {
        private string _tag;
        public string Tag
        {
            get { return _tag; }
        }

        private uint _checksum;
        public uint CheckSum
        {
            get { return _checksum; }
        }

        private uint _offset;
        public uint Offset
        {
            get { return _offset; }
        }

        private uint _len;
        public uint Length
        {
            get { return _len; }
        }

        private TTFTable _tbl;
        public TTFTable Table
        {
            get { return _tbl; }
        }


        public void Read(BigEndianReader reader)
        {
            this._tag = reader.ReadString(4);
            //this._tag = new string(tag);
            this._checksum = reader.ReadUInt32();
            this._offset = reader.ReadUInt32();
            this._len = reader.ReadUInt32();
        }

        public override string ToString()
        {
            return "Directory : " + this.Tag + " (from '" + this.Offset.ToString() + "' to '" + (this.Length + this.Offset).ToString() + "'";
        }

        internal void SetTable(TTFTable tbl)
        {
            this._tbl = tbl;
        }
    }

    public class TTFDirectoryList : System.Collections.ObjectModel.KeyedCollection<string, TTFDirectory>
    {
        public TTFDirectoryList()
            : base()
        {
        }

        public TTFDirectoryList(IEnumerable<TTFDirectory> items)
            : this()
        {
            if (items != null)
            {
                foreach (TTFDirectory item in items)
                {
                    this.Add(item);
                }
            }
        }

        protected override string GetKeyForItem(TTFDirectory item)
        {
            return item.Tag;
        }
    }
}
