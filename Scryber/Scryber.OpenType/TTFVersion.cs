using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType
{
    public abstract class TTFVersion
    {

        public abstract override string ToString();

        public abstract TTFTableFactory GetTableFactory();


        #region public static TTFVersion GetVersion(BigEndianReader reader)

        public static bool TryGetVersion(BigEndianReader reader, out TTFVersion vers)
        {
            vers = null;
            byte[] data = reader.Read(4);
            char[] chars = ConvertToChars(data,4);

            if (chars[0] == 'O' && chars[1] == 'T' && chars[2] == 'T' && chars[3] == 'O')
            {
                //CCF Format is not supported
            }
            else if (chars[0] == 't' && chars[1] == 'r' && chars[2] == 'u' && chars[3] == 'e')
                vers = new TTFTrueTypeVersion(new string(chars));

            else if (chars[0] == 't' && chars[1] == 'y' && chars[2] == 'p' && chars[3] == '1')
                vers = new TTFTrueTypeVersion(new string(chars));
            else
            {
                BigEnd16 wrd1 = new BigEnd16(data, 0);
                BigEnd16 wrd2 = new BigEnd16(data, 2);

                if (((int)wrd1.UnsignedValue) == 1 && ((int)wrd2.UnsignedValue) == 0)
                    vers = new TTFOpenType1Version(wrd1.UnsignedValue, wrd2.UnsignedValue);

                
            }
            
            return vers != null;
        }
        public static TTFVersion GetVersion(BigEndianReader reader)
        {
            TTFVersion version;

            if (TryGetVersion(reader, out version) == false)
                throw new TTFReadException("The version could not be identified");

            return version;
        }

        private static char[] ConvertToChars(byte[] data, int count)
        {
            char[] chars = new char[count];

            for (int i = 0; i < count; i++)
            {
                chars[i] = (char)data[i];
            }
            return chars;
        }

        #endregion

    }

    public class TTFOpenType1Version : TTFVersion
    {
        private Version _innervers;
        protected Version InnerVersion
        {
            get { return _innervers; }
        }

        public TTFOpenType1Version(UInt16 major, UInt16 minor)
        {
            this._innervers = new Version((int)major, (int)minor);
            
            if (this._innervers != new Version("1.0"))
                throw new TTFReadException("The open type version can only be version 1.0");
        }

        public override string ToString()
        {
            return "Open Type " + InnerVersion.ToString();
        }

        public override TTFTableFactory GetTableFactory()
        {
            return new TTFOpenTypeTableFactory(false);
        }
    }

    public class TTFTrueTypeVersion : TTFVersion
    {
        private string _versid;
        public string VersionIdentifier
        {
            get { return _versid; }
        }

        public TTFTrueTypeVersion(string id)
        {
            if (string.IsNullOrEmpty(id) || (id.Equals("TRUE", StringComparison.CurrentCultureIgnoreCase) || id.Equals("typ1", StringComparison.CurrentCultureIgnoreCase)) == false)
                throw new TTFReadException("The true type version must be either 'true' or 'typ1'");

            this._versid = id;
        }

        public override string ToString()
        {
            return "True Type : " + this.VersionIdentifier;
        }

        public override TTFTableFactory GetTableFactory()
        {
            return null;
        }
    }
}
