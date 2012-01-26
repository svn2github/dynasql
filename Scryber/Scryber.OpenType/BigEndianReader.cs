using System;
using System.Collections.Generic;
using System.Text;

namespace Scryber.OpenType
{
    public class BigEndianReader : IDisposable
    {
        public bool CanSeek
        {
            get { return this.BaseStream.CanSeek; }
        }

        public bool CanRead
        {
            get { return this.BaseStream.CanRead; }
        }

        public bool CanWrite
        {
            get { return false; }
        }
        
        public long Position
        {
            get { return this.BaseStream.Position; }
            set { this.BaseStream.Position = value; }
        }

        public long Length
        {
            get { return this.BaseStream.Length; }
        }

        private System.IO.Stream _base;
        protected System.IO.Stream BaseStream
        {
            get { return _base; }
        }

        public BigEndianReader(System.IO.Stream basestream)
        {
            if (basestream == null)
                throw new ArgumentNullException("basestream");
            if (basestream.CanRead == false)
                throw new ArgumentException("The base stream does not support reading!");
            if (basestream.CanSeek == false)
                throw new ArgumentException("The Reader requires a stream that supports seeking (changing position within the stream) as well as forward only reading");
            this._base = basestream;
        }

        public byte ReadByte()
        {
            return (byte)this.BaseStream.ReadByte();
        }

        public ushort ReadUInt16()
        {
            byte[] data = new byte[2];
            this.BaseStream.Read(data, 0, 2);
            BigEnd16 word = new BigEnd16(data);
            
            return word.UnsignedValue;
        }

        public short ReadInt16()
        {
            byte[] data = new byte[2];
            this.BaseStream.Read(data, 0, 2);
            BigEnd16 word = new BigEnd16(data);

            return word.SignedValue;
        }

        public uint ReadUInt32()
        {
            byte[] data = new byte[4];
            this.BaseStream.Read(data, 0, 4);
            BigEnd32 l = new BigEnd32(data, 0);

            return l.UnsignedValue;
        }

        public int ReadInt32()
        {
            byte[] data = new byte[4];
            this.BaseStream.Read(data, 0, 4);
            BigEnd32 l = new BigEnd32(data, 0);

            return l.SignedValue;
        }

        public ulong ReadUInt64()
        {
            byte[] data = new byte[8];
            this.BaseStream.Read(data, 0, 8);
            BigEnd64 l = new BigEnd64(data, 0);
            return l.UnsignedValue;
        }

        public long ReadInt64()
        {
            byte[] data = new byte[8];
            this.BaseStream.Read(data, 0, 8);
            BigEnd64 l = new BigEnd64(data, 0);
            return l.SignedLong;
        }

        public char[] ReadChars(int len)
        {
            byte[] data = new byte[len];
            this.BaseStream.Read(data, 0, len);
            char[] str = new char[len];
            for (int i = 0; i < len; i++)
            {
                str[i] = (char)data[i];
            }
            return str;
        }

        public string ReadString(int len)
        {
            return new string(this.ReadChars(len));
        }

        public string ReadUnicodeString(int len)
        {
            byte[] data = new byte[len];
            this.BaseStream.Read(data, 0, len);
            //this.SwapBytes(data);
            List<char> chars = new List<char>();

            for (int i = 0; i < data.Length; i+= 2)
            {
                BigEnd16 word = new BigEnd16(data, i);
                ushort character = word.UnsignedValue;
                chars.Add((char)character);

                
            }

            return new string(chars.ToArray());
        }

        public string ReadPascalString()
        {
            int len = this.BaseStream.ReadByte();
            return this.ReadString(len);
        }

        

        public byte[] Read(int len)
        {
            byte[] data = new byte[len];
            this.BaseStream.Read(data, 0, len);
            return data;
        }

        public Version ReadUShortVersion()
        {
            ushort major = this.ReadUInt16();

            return new Version((int)major, 0);
        }

        public float ReadFixed1616()
        {
            short major = this.ReadInt16();
            ushort minor = this.ReadUInt16();
            float mf = ((float)minor) / ((float)ushort.MaxValue);
            return ((float)major) + mf;
        }

        public Version ReadFixedVersion()
        {
            ushort major = this.ReadUInt16();
            ushort minor = this.ReadUInt16();
            return new Version((int)major, (int)minor);
        }

        private static DateTime _dateoffsetbase = new DateTime(1904, 1, 1, 0, 0, 0);
        public static DateTime DateOffsetBase
        {
            get { return _dateoffsetbase; }
        }

        public DateTime ReadDateTime()
        {
            ulong l = this.ReadUInt64();
            DateTime dt = DateOffsetBase;

            dt = dt.AddSeconds(l);

            return dt;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {

        }

        ~BigEndianReader()
        {
            this.Dispose(false);
        }
        #endregion

        
    }
}
