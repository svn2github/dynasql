using System;
using System.Collections.Generic;
using System.Text;
using Scryber.OpenType.SubTables;

namespace Scryber.OpenType
{
    public class TTFRef
    {
        private const string OS2Table = "OS/2";
        private const string NameTable = "name";
        private const string FontHeaderTable = "head";
        private const ushort FamilyNameID = 1;

        private string _path;

        public string FullPath
        {
            get { return _path; }
        }

        private string _family;

        public string FamilyName
        {
            get { return _family; }
            protected set { this._family = value; }
        }

        private SubTables.WeightClass _fontWeight;

        public SubTables.WeightClass FontWeight
        {
            get { return _fontWeight; }
            protected set { _fontWeight = value; }
        }

        private SubTables.WidthClass _fwidth;

        public SubTables.WidthClass FontWidth
        {
            get { return _fwidth; }
            protected set { _fwidth = value; }
        }


        private SubTables.FontRestrictions _retrictions;

        public SubTables.FontRestrictions FontRestrictions
        {
            get { return _retrictions; }
            protected set { this._retrictions = value; }
        }

        private SubTables.FontSelection _fsel;

        public SubTables.FontSelection FontSelection
        {
            get { return _fsel; }
            protected set { _fsel = value; }
        }


        public bool CanEmbed
        {
            get
            {
                bool b = (this.FontRestrictions & Scryber.OpenType.SubTables.FontRestrictions.NoEmbedding) == 0;
                if (b)
                    b = (this.FontRestrictions & Scryber.OpenType.SubTables.FontRestrictions.PreviewPrintEmbedding) > 0;
                return b;
            }
        }

        public TTFRef(string fullpath)
        {
            this._path = fullpath;
        }

        public string GetFullName()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.FamilyName);

            if (this.FontSelection != FontSelection.Regular)
            {
                sb.Append(" ");
                sb.Append(this.FontSelection.ToString());
            }
            return sb.ToString();
        }

        public static TTFRef[] LoadRefs(System.IO.DirectoryInfo dir)
        {
            List<TTFRef> matched = new List<TTFRef>();
            System.IO.FileInfo[] fis = dir.GetFiles("*.ttf");
            if (fis != null && fis.Length > 0)
            {
                for (int i = 0; i < fis.Length; i++)
                {
                    TTFRef aref = LoadRef(fis[i]);
                    if (aref != null)
                        matched.Add(aref);
                }
            }

            return matched.ToArray();
        }


        public static TTFRef LoadRef(string path)
        {
            return LoadRef(new System.IO.FileInfo(path));
        }

        public static TTFRef LoadRef(System.IO.FileInfo fi)
        {
            if (fi.Exists == false)
                return null;

            using (System.IO.FileStream fs = fi.OpenRead())
            {
                return LoadRef(fs, fi.FullName);
            }
        }

        public static TTFRef LoadRef(System.IO.FileStream fs, string fullpath)
        {
            using (BigEndianReader reader = new BigEndianReader(fs))
            {
                return LoadRef(reader, fullpath);
            }
        }

        public static TTFRef LoadRef(BigEndianReader reader, string fullpath)
        {
            TTFHeader head;
            if (TTFHeader.TryReadHeader(reader, out head) == false)
                return null;

            TTFDirectoryList list = new TTFDirectoryList();

            for (int i = 0; i < head.NumberOfTables; i++)
            {
                TTFDirectory dir = new TTFDirectory();
                dir.Read(reader);
                list.Add(dir);
            }

            TTFTableFactory fact = head.Version.GetTableFactory();
            //SubTables.FontHeader fhead = fact.ReadDirectory(FontHeaderTable, list, reader) as SubTables.FontHeader;
            SubTables.NamingTable ntable = fact.ReadDirectory(NameTable, list, reader) as SubTables.NamingTable;
            SubTables.OS2Table os2table = fact.ReadDirectory(OS2Table, list, reader) as SubTables.OS2Table;


            if (ntable == null)
                throw new ArgumentNullException("The required '" + NameTable + "' is not present in this font file. The OpenType file is corrupt");
            if(os2table == null)
                throw new ArgumentNullException("The required '" + OS2Table + "' is not present in this font file. The OpenType file is corrupt");
            //if (fhead == null)
            //    throw new ArgumentNullException("The required '" + FontHeaderTable + "' is not present in this font file. The OpenType file is corrupt");


            
            TTFRef ttfref = new TTFRef(fullpath);
            NameEntry entry;
            if (ntable.Names.TryGetEntry(FamilyNameID, out entry))
            {
                ttfref.FamilyName = entry.ToString();  
            }

            ttfref.FontRestrictions = os2table.FSType;
            ttfref.FontWidth = os2table.WidthClass;
            ttfref.FontWeight = os2table.WeightClass;
            ttfref.FontSelection = os2table.Selection;

            return ttfref;

        }


        
    }
}
