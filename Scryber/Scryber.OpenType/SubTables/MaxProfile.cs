using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType.SubTables
{
    public class MaxProfile : TTFTable
    {
        public MaxProfile(long offset):base(offset)
        {
        }

        private ushort _numglyphs;

        public ushort NumberOfGlyphs
        {
            get { return _numglyphs; }
            set { _numglyphs = value; }
        }



    }

    public class MaxTTProfile : MaxProfile
    {

        public MaxTTProfile(long offset)
            : base(offset)
        {
        }

        private ushort _maxpts;

        public ushort MaxPoints
        {
            get { return _maxpts; }
            set { _maxpts = value; }
        }

        private ushort _maxcont;

        public ushort MaxContours
        {
            get { return _maxcont; }
            set { _maxcont = value; }
        }

        private ushort _maxcomppts;

        public ushort MaxCompositePoints
        {
            get { return _maxcomppts; }
            set { _maxcomppts = value; }
        }

        private ushort _maxcompcont;

        public ushort MaxCompositeContours
        {
            get { return _maxcompcont; }
            set { _maxcompcont = value; }
        }

        private ushort _maxzones;

        public ushort MaxZones
        {
            get { return _maxzones; }
            set { _maxzones = value; }
        }

        private ushort _maxtwil;

        public ushort MaxTwilightPoints
        {
            get { return _maxtwil; }
            set { _maxtwil = value; }
        }

        private ushort _maxstor;

        public ushort MaxStorage
        {
            get { return _maxstor; }
            set { _maxstor = value; }
        }

        private ushort _maxfunc;

        public ushort MaxFunctionDefinitions
        {
            get { return _maxfunc; }
            set { _maxfunc = value; }
        }

        private ushort _maxinst;

        public ushort MaxInstructionDefinitions
        {
            get { return _maxinst; }
            set { _maxinst = value; }
        }

        private ushort _maxstack;

        public ushort MaxStackComponents
        {
            get { return _maxstack; }
            set { _maxstack = value; }
        }

        private ushort _maxszinst;

        public ushort MaxSizeOfInstructions
        {
            get { return _maxszinst; }
            set { _maxszinst = value; }
        }

        private ushort _maxcompele;

        public ushort MaxComponentComponents
        {
            get { return _maxcompele; }
            set { _maxcompele = value; }
        }

        private ushort _maxcompdepth;

        public ushort MaxComponentDepth
        {
            get { return _maxcompdepth; }
            set { _maxcompdepth = value; }
        }

    }
}
