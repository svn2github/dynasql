using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public class NameRecord
    {
        private ushort _platformid;

        public ushort PlatformID
        {
            get { return _platformid; }
            set { _platformid = value; }
        }

        private ushort _encid;

        public ushort EncodingID
        {
            get { return _encid; }
            set { _encid = value; }
        }

        private ushort _langid;

        public ushort LanguageID
        {
            get { return _langid; }
            set { _langid = value; }
        }

        private ushort _nameid;

        public ushort NameID
        {
            get { return _nameid; }
            set { _nameid = value; }
        }

        private ushort _len;

        public ushort StringLength
        {
            get { return _len; }
            set { _len = value; }
        }

        private ushort _soffset;

        public ushort StringDataOffset
        {
            get { return _soffset; }
            set { _soffset = value; }
        }

        private string _val;

        public string Value
        {
            get { return _val; }
            set { _val = value; }
        }

        public override string ToString()
        {
            return this.Value + " (platform : " + this.PlatformID.ToString() +
                ", encoding: " + this.EncodingID.ToString() + ", language: " +
                this.LanguageID.ToString() + ")";
        }

    }
}
