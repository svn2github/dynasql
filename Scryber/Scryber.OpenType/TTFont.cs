using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType
{
    public class TTFont
    {
        private TTFFile _file;
        public TTFFile File
        {
            get { return _file; }
        }

        private int _sizeinpts;

        public int SizeInPoints
        {
            get { return _sizeinpts; }
            set { _sizeinpts = value; }
        }

        public TTFont(TTFFile file, int sizeInPoints)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (sizeInPoints < 1)
                throw new ArgumentException("sizeInPoints");

            this._file = file;
            this._sizeinpts = sizeInPoints;
        }

        public string FamilyName
        {
            get
            {
                return File.Head.Version.ToString();
            }
        }
	
    }
}
