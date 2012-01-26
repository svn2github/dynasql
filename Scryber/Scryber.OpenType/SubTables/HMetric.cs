using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Scryber.OpenType.SubTables
{
    public struct HMetric
    {
        private char _c;

        public char Character
        {
            get { return _c; }
            set { _c = value; }
        }

        private ushort _advance;

        public ushort AdvanceWidth
        {
            get { return _advance; }
            set { _advance = value; }
        }

        private short _leftsizebearing;

        public short LeftSideBearing
        {
            get { return _leftsizebearing; }
            set { _leftsizebearing = value; }
        }

        public HMetric(ushort advancewidth, short leftbearing, char c)
        {
            this._c = c;
            this._advance = advancewidth;
            this._leftsizebearing = leftbearing;
        }

        public override string ToString()
        {
            return "HMetric '" + this.Character.ToString() + "' {aw: " + this.AdvanceWidth.ToString() + ", lsb: " + this.LeftSideBearing.ToString() + "}";
        }

    }
}
