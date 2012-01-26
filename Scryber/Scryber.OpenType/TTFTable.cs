using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType
{
    public abstract class TTFTable
    {
        private long _offset;
        public long FileOffset
        {
            get { return _offset; }
        }

        private Version _vers;

        public Version TableVersion
        {
            get { return _vers; }
            set { _vers = value; }
        }

        public TTFTable(long offset)
        {
            this._offset = offset;
        }

        
    }
}
