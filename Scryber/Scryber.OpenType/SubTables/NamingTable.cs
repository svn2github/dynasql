using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public class NamingTable : TTFTable
    {

        public NamingTable(long offset)
            : base(offset)
        {
        }

        private ushort _format;

        public ushort Format
        {
            get { return _format; }
            set { _format = value; }
        }

        private ushort _count;

        public ushort Count
        {
            get { return _count; }
            set { _count = value; }
        }

        private ushort _stringOffset;

        public ushort StringOffset
        {
            get { return _stringOffset; }
            set { _stringOffset = value; }
        }

        private NameEntryList _names = new NameEntryList();

        public NameEntryList Names
        {
            get { return _names; }
            set { _names = value; }
        }



    }

    public class NameEntryList : System.Collections.ObjectModel.KeyedCollection<int, NameEntry>
    {
        protected override int GetKeyForItem(NameEntry item)
        {
            return item.NameID;
        }

        public bool TryGetEntry(int nameid, out NameEntry entry)
        {
            if (this.Count == 0)
            {
                entry = null;
                return false;
            }
            else
                return this.Dictionary.TryGetValue(nameid, out entry);
        }
    }

    public class NameEntry
    {
        private int _nameid;

        public int NameID
        {
            get { return _nameid; }
            set { _nameid = value; }
        }

        private string _unicodeenty;

        public string InvariantName
        {
            get { return _unicodeenty; }
            set { _unicodeenty = value; }
        }

        private string _local;

        public string LocalName
        {
            get { return _local; }
            set { _local = value; }
        }


        private List<NameRecord> _items = new List<NameRecord>();

        public List<NameRecord> NameItems
        {
            get { return _items; }
            set { _items = value; }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.LocalName) == false)
                return LocalName;
            else if (string.IsNullOrEmpty(this.InvariantName) == false)
                return InvariantName;
            else
            {
                string s = string.Empty;
                foreach (NameRecord rec in this.NameItems)
                {
                    if (string.IsNullOrEmpty(rec.Value) == false)
                    {
                        s = rec.Value;
                        break;
                    }
                }
                return s;
            }
        }

    }

    

}
