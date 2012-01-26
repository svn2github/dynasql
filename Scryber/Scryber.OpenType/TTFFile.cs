using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Scryber.OpenType.SubTables;

namespace Scryber.OpenType
{
    public class TTFFile
    {

        public TTFFile(byte[] data)
            : base()
        {
            this._path = string.Empty;
            this.Read(data);
        }

        public TTFFile(string path)
            : base()
        {
            this._path = path;
            this.Read(path);
        }

        private string _path;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        private TTFHeader _head;
        public TTFHeader Head
        {
            get { return _head; }
        }

        private TTFTableSet _tables;

        public TTFTableSet Tables
        {
            get
            {
                if (null == _tables)
                    _tables = new TTFTableSet(this.Directories);
                return _tables;
            }
        }

        private byte[] _alldata;

        public byte[] FileData 
        { 
            get { return _alldata; }
            private set { _alldata = value; }
        }

        private TTFDirectoryList _dirs;
        public TTFDirectoryList Directories
        {
            get { return _dirs; }
        }

        public void Read(string path)
        {
            this.Read(new System.IO.FileInfo(path));
        }

        public void Read(System.IO.FileInfo fi)
        {
            if (fi.Exists == false)
                throw new System.IO.FileNotFoundException("The font file at '" + fi.FullName + "' does not exist");

            using (System.IO.FileStream fs = fi.OpenRead())
            {
                this.Read(fs);
            }
        }

        public void Read(System.IO.Stream stream)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            try
            {
                //Copy the stream to a private data array
                byte[] buffer = new byte[4096];
                int count;
                while ((count = stream.Read(buffer, 0, 4096)) > 0)
                {
                    ms.Write(buffer, 0, count);
                }
                ms.Position = 0;
                using (BigEndianReader reader = new BigEndianReader(ms))
                {
                    this.Read(reader);
                }
                this.FileData = ms.ToArray();
            }
            catch (Exception ex)
            {
                throw new System.IO.IOException("Could not load the font file from the stream. " + ex.Message);
            }
            finally
            {
                ms = null;
            }
        }

        public void Read(byte[] data)
        {
            System.IO.MemoryStream ms = null;
            BigEndianReader ber = null;
            try
            {
                ms = new System.IO.MemoryStream(data);
                ber = new BigEndianReader(ms);
                this.Read(ber);
                this.FileData = data;
            }
            catch (Exception ex)
            {
                throw new System.IO.IOException("Could not load the font file from the stream. " + ex.Message);
            }
            finally
            {
                if (null != ber)
                    ber.Dispose();
                if (null != ms)
                    ms.Dispose();
            }
        }
        

        private void Read(BigEndianReader reader)
        {

            TTFHeader header;
            if (TTFHeader.TryReadHeader(reader, out header) == false)
                throw new NotSupportedException("The current stream is not a supported OpenType or TrueType font file");

            List<TTFDirectory> dirs;
            try
            {
                dirs = new List<TTFDirectory>();

                for (int i = 0; i < header.NumberOfTables; i++)
                {
                    TTFDirectory dir = new TTFDirectory();
                    dir.Read(reader);
                    dirs.Add(dir);
                }

                dirs.Sort(delegate(TTFDirectory one, TTFDirectory two) { return one.Offset.CompareTo(two.Offset); });
                this._dirs = new TTFDirectoryList(dirs);
                this._head = header;

                TTFTableFactory factory = this.GetFactory(header);
                foreach (TTFDirectory dir in dirs)
                {
                    TTFTable tbl = factory.ReadDirectory(dir, this, reader);
                    if(tbl != null)
                        dir.SetTable(tbl);
                }

                
            }
            catch (OutOfMemoryException) { throw; }
            catch (System.Threading.ThreadAbortException) { throw; }
            catch (StackOverflowException) { throw; }
            catch (TTFReadException) { throw; }
            catch (Exception ex) { throw new TTFReadException("Could not read the TTF File", ex); }

            
            
        }

        protected virtual TTFTableFactory GetFactory(TTFHeader header)
        {
            return header.Version.GetTableFactory();
        }

        public SizeF MeasureString(string s, double emsize)
        {
            HorizontalMetrics table = this.Directories["hmtx"].Table as HorizontalMetrics;
            CMAPTable cmap = this.Directories["cmap"].Table as CMAPTable;
            OS2Table os2 = this.Directories["OS/2"].Table as OS2Table;
            CMAPSubTable mac = cmap.GetOffsetTable(CharacterPlatforms.Macintosh, (CharacterEncodings)0);
            if (mac == null)
                mac = cmap.GetOffsetTable(CharacterPlatforms.Windows, CharacterEncodings.Unicode);

            HorizontalHeader hhead = this.Directories["hhea"].Table as HorizontalHeader;

            double len = 0.0;
            char[] chars = s.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                int moffset = (int)mac.GetCharacterGlyphOffset(chars[i]);
                //System.Diagnostics.Debug.WriteLine("Character '" + chars[i].ToString() + "' (" + ((byte)chars[i]).ToString() + ") has offset '" + moffset.ToString() + "' in mac encoding and '" + woffset + "' in windows encoding");

                if (moffset >= table.HMetrics.Count)
                    moffset = table.HMetrics.Count - 1;
                Scryber.OpenType.SubTables.HMetric metric;
                metric = table.HMetrics[moffset];
                if (i == 0)
                    len = -metric.LeftSideBearing;
                len += metric.AdvanceWidth;
            }
            FontHeader head = this.Directories["head"].Table as FontHeader;
            len = len / (double)head.UnitsPerEm;
            len = len * emsize;
            double h = ((double)(os2.TypoAscender - os2.TypoDescender + os2.TypoLineGap) / (double)head.UnitsPerEm) * emsize;
            return new SizeF((float)len, (float)h);
        }

        public static bool CanRead(string path)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(path);

            return CanRead(fi);
        }
        public static bool CanRead(System.IO.FileInfo fi)
        {
            if (fi.Exists == false)
                return false;
            else
            {
                using (System.IO.FileStream fs = fi.OpenRead())
                {
                    return CanRead(fs);
                }
            }
        }

        public static bool CanRead(System.IO.Stream stream)
        {
            BigEndianReader reader = new BigEndianReader(stream);
            return CanRead(reader);
            
        }

        public static bool CanRead(BigEndianReader reader)
        {
            long oldpos = reader.Position;
            reader.Position = 0;
            TTFHeader header;

            bool b = TTFHeader.TryReadHeader(reader, out header);

            reader.Position = oldpos;

            return b;
        }
    }
}
