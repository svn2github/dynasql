/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scryber.Native;

namespace Scryber
{
    internal class PDFWriter14 : PDFWriter
    {
        private const string TraceCategory = "PDFWriter";
        private const TraceLevel TraceDefaultLevel = TraceLevel.Debug;
        
        public override Version Version { get { return new Version(1, 5); } }

        internal static class Constants
        {
            internal const string StartObject = "obj\r\n";
            internal const string CommentStart = "%";
            internal const string EndObject = "\r\nendobj\r\n";
            internal const string StartStream = "\r\nstream\r\n";
            internal const string EndStream = "\r\nendstream";
            internal const string StartDictionary = "<< ";
            internal const string EndDictionary = " >>";
            internal const string StartName = "/";
            internal const string StartArray = "[";
            internal const string EndArray = "]";
            internal const string ArrayEntrySeparator = " ";
            internal const string DictionaryEntrySeparator = "\r\n";
            internal const string DictionaryNameValueSeparator = " ";
            internal const string CommentPrePend = "%";
            internal const string StartString = "(";
            internal const string EndString = ")";
            internal const string StartHexString = "<";
            internal const string EndHexString = ">";
            internal const string WhiteSpace = " ";
            internal const string NullString = "null";
            
        }

        private bool _finishedentry = false;

        protected bool FinishedEntry
        {
            get { return _finishedentry; }
            set { _finishedentry = value; }
        }

        #region Constructors

        public PDFWriter14(Stream stream, PDFTraceLog log)
            : this(stream, 0, log)
        {
        }

        

                
        public PDFWriter14(Stream stream, int gen, PDFTraceLog log)
            : base(stream, new PDFXRefTable(gen), log)
        {
        }

        

        #endregion

        public override void Open()
        {
            
        }

        public override void Close(string[] documentid)
        {
            this.BaseStream.Flush();
            this.Log(TraceLevel.Message, "Closing the writer and outputting indirect object data onto the underlying stream");

            this.BaseStream.WriteLine(String.Format("%PDF-{0}.{1}", this.Version.Major, this.Version.Minor));
            //this.BaseWriter.Write(Constants.CommentStart);
            //this.BaseWriter.Write("????");
            this.BaseStream.WriteLine();
            
            foreach (IIndirectObject pfo in this.XRefTable.References)
            {
                if (pfo.Deleted == false)
                {
                    this.Log("Writing indirect object data " + pfo.Number);
                    pfo.Offset = this.BaseStream.Position;
                    this.BaseStream.Write(pfo.Number.ToString());
                    this.BaseStream.Write(" ");
                    this.BaseStream.Write(pfo.Generation.ToString());
                    this.BaseStream.Write(" ");
                    this.BaseStream.Write(Constants.StartObject);
                    
                    pfo.ObjectData.WriteTo(this.BaseStream);
                    if (pfo.HasStream)
                    {
                        this.BaseStream.Write(Constants.StartStream);
                        pfo.Stream.WriteTo(this.BaseStream);
                        this.BaseStream.Write(Constants.EndStream);
                    }
                    this.BaseStream.Write(Constants.EndObject);
                    this.BaseStream.WriteLine();
                    this.BaseStream.Flush();
                }
            }
            this.XRefTable.Offset = this.BaseStream.Position;
            this.WriteXRefTable(this.XRefTable);
            this.BaseStream.WriteLine();
            this.BaseStream.WriteLine();
            this.BaseStream.WriteLine("trailer");
            this.BaseStream.Write(Constants.StartDictionary);
            this.BaseStream.Write(Constants.StartName);
            this.BaseStream.Write("Size");
            this.BaseStream.Write(Constants.WhiteSpace);
            this.BaseStream.WriteLine(this.XRefTable.References.Count.ToString());
           
            PDFObjectRef oref;
            if (this.ObjectDictionary.TryGetValue("Catalog", out oref))
            {
                this.BaseStream.Write(Constants.StartName);
                this.BaseStream.Write("Root");
                this.BaseStream.Write(Constants.WhiteSpace);
                this.BaseStream.Write(oref.Number + Constants.WhiteSpace + oref.Generation + " R");
                this.BaseStream.WriteLine();
            }

            if (this.ObjectDictionary.TryGetValue("Info", out oref))
            {
                this.BaseStream.Write(Constants.StartName);
                this.BaseStream.Write("Info");
                this.BaseStream.Write(Constants.WhiteSpace);
                this.BaseStream.Write(oref.Number + Constants.WhiteSpace + oref.Generation + " R");
                
            }

            if (documentid != null && documentid.Length > 0)
            {
                this.BaseStream.Write(Constants.StartName);
                this.BaseStream.Write("ID");
                this.BaseStream.Write(Constants.WhiteSpace);
                this.BaseStream.Write(Constants.StartArray);

                for (int i = 0; i < documentid.Length; i++)
                {
                    if (i > 0)
                        this.BaseStream.Write(Constants.ArrayEntrySeparator);
                    this.BaseStream.Write(documentid[i]);
                }
                this.BaseStream.WriteLine(Constants.EndArray);
            }
            this.BaseStream.WriteLine(Constants.EndDictionary);

            this.BaseStream.Flush();
            this.BaseStream.WriteLine("startxref");
            this.BaseStream.WriteLine(this.XRefTable.Offset.ToString());
            this.BaseStream.Write("%%EOF");
            this.Log("Written end of file marker");
            this.BaseStream.Flush();
        }

       
        private void WriteXRefTable(PDFXRefTable table)
        {
            this.Log("Outputting XRefTable onto the stream at position " + this.BaseStream.Position.ToString());
            this.BaseStream.WriteLine("xref");
            this.BaseStream.Write(table.References[0].Number.ToString());
            this.BaseStream.Write(" ");
            this.BaseStream.WriteLine(table.References.Count.ToString());
            foreach (IIndirectObject io in table.References)
            {
                if (io.Deleted)
                {
                    this.BaseStream.WriteLine(String.Format("{0:0000000000} 65535 f", io.Offset));
                }
                else if (io.Generation != table.Generation)
                {
                    this.BaseStream.WriteLine(String.Format("{0:0000000000} {1:00000} f", io.Offset, io.Generation));
                }
                else
                {
                    this.BaseStream.WriteLine(String.Format("{0:0000000000} {1:00000} n", io.Offset, io.Generation));
                }
            }
            this.BaseStream.Flush();
        }

        public override void WriteLine()
        {
            this.CurrentStream.WriteLine();
        }

        public override void WriteCommentLine()
        {
            this.CurrentStream.WriteLine(Constants.CommentPrePend);
        }
        public override void WriteCommentLine(string comment)
        {
            this.CurrentStream.Write(Constants.CommentPrePend);
            this.CurrentStream.WriteLine(comment);
        }

        public override void WriteComment(string comment)
        {
            this.CurrentStream.Write(Constants.CommentPrePend);
            this.CurrentStream.Write(comment);
        }

        public override void WriteCommentLine(string comment, params object[] args)
        {
            this.CurrentStream.Write(Constants.CommentPrePend);
            this.CurrentStream.WriteLine(String.Format(comment, args));
        }

        public override void WriteComment(string comment, params object[] args)
        {
            this.CurrentStream.Write(Constants.CommentPrePend);
            this.CurrentStream.Write(String.Format(comment, args));
        }

        public override PDFObjectRef BeginObject(string name)
        {
            PDFIndirectObject obj = new PDFIndirectObject();
            
            PDFObjectRef oref = this.InitializeIndirectObject(name, obj);
            this.Log("Begun a new indirect object: " + oref);
            return oref;
        }

        public override void BeginStream(PDFObjectRef onobject, IStreamFilter[] filters)
        {
            PDFIndirectObject pio = onobject.Reference as PDFIndirectObject;
            pio.InitStream(filters);
            this.Stack.Push(pio.Stream);
        }

        public override void EndObject()
        {
            PDFIndirectObject obj = this.Stack.Peek().IndirectObject;
            this.Log(String.Format("Ended indirect object {0} {1} R ", obj.Number, obj.Generation));
            
            this.ReleaseIndirectObject();
        }

        public override long EndStream()
        {
            PDFStream str = this.Stack.Pop();
            str.Flush();
            return str.Length;
        }

        public override void BeginDictionary()
        {
            CurrentStream.Write(Constants.StartDictionary);
            this.FinishedEntry = false;
        }

        public override void BeginDictionaryEntry(string name)
        {
            if (FinishedEntry)
                CurrentStream.Write(Constants.DictionaryNameValueSeparator);
            FinishedEntry = false;
            CurrentStream.Write(Constants.StartName);
            CurrentStream.Write(ValidateName(name));
            CurrentStream.Write(Constants.WhiteSpace);
        }

        
        public override void EndDictionaryEntry()
        {
            this.CurrentStream.Write(Constants.DictionaryEntrySeparator);
        }

        public override void EndDictionary()
        {
            this.FinishedEntry = false;
            CurrentStream.Write(Constants.EndDictionary);
        }


        public override void BeginArray()
        {
            CurrentStream.Write(Constants.StartArray);
            this.FinishedEntry = false;
        }

        public override void BeginArrayEntry()
        {
            if (FinishedEntry)
                CurrentStream.Write(Constants.ArrayEntrySeparator);
            this.FinishedEntry = false;
        }

        public override void EndArrayEntry()
        {
            CurrentStream.Write(Constants.ArrayEntrySeparator);
        }

        
        public override void EndArray()
        {
            this.FinishedEntry = false;
            CurrentStream.Write(Constants.EndArray);
        }

        private static bool UseHex = false;

        public override void WriteByteString(string byteString)
        {
            CurrentStream.Write(Constants.StartHexString);
            CurrentStream.Write(byteString);
            CurrentStream.Write(Constants.EndHexString);
        }

        public override void WriteStringLiteral(string value)
        {
            if (UseHex)
            {
                CurrentStream.Write(Constants.StartHexString);
                value = ToHex(value);
                CurrentStream.Write(value);
                CurrentStream.Write(Constants.EndHexString);
            }
            else
            {
                CurrentStream.Write(Constants.StartString);
                CurrentStream.Write(value);
                CurrentStream.Write(Constants.EndString);
            }
        }

        private string ToHex(string value)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                sb.Append(((int)value[i]).ToString("X"));
            }
            return sb.ToString();
        }
        

        public override void WriteBoolean(bool value)
        {
            if (value)
                CurrentStream.Write("true");
            else
                CurrentStream.Write("false");
        }

        public override void WriteDate(DateTime value)
        {
            CurrentStream.Write(value.ToString("(\\D:yyyyMMddhhmmss"));
            string offset = value.ToString("zzz");
            if (offset != "00:00")
            {
                offset = offset.Replace(':', '\'') + "'";
                CurrentStream.Write(offset);
            }
            CurrentStream.Write(")");
            
        }

        public override void WriteNumber(int value)
        {
            CurrentStream.Write(value.ToString());
        }
        public override void WriteNumber(long value)
        {
            CurrentStream.Write(value.ToString());
        }

        public override void WriteReal(double value)
        {
            CurrentStream.Write(String.Format("{0:####################0.0######}", value));
        }

        public override void WriteReal(decimal value)
        {
            CurrentStream.Write(String.Format("{0:####################0.0######}", value));
        }

        public override void WriteReal(float value)
        {
            CurrentStream.Write(String.Format("{0:##############0.0#####}", value));
        }

        public override void WriteName(string name)
        {
            CurrentStream.Write(Constants.StartName);
            CurrentStream.Write(ValidateName(name));

        }

        public override void WriteObjectRef(int number, int generation)
        {
            this.CurrentStream.Write(number.ToString());
            this.CurrentStream.Write(" ");
            this.CurrentStream.Write(generation.ToString());
            this.CurrentStream.Write(" R");
        }

        public override void WriteNull()
        {
            this.CurrentStream.Write(Constants.NullString);
        }

        public override void WriteFileObject(IFileObject obj)
        {
            obj.WriteData(this);
        }
        
        public override void WriteRaw(string data)
        {
            this.CurrentStream.Write(data);
        }

        public override void WriteRaw(byte[] buffer, int offset, int length)
        {
            this.CurrentStream.Write(buffer, offset, length);
        }

        
        public override void WriteSpace()
        {
            this.CurrentStream.Write(Constants.WhiteSpace);
        }

        protected override string GetOpCode(PDFOpCode op)
        {
            string s = String.Empty;

            switch (op)
            {
                case PDFOpCode.ColorSetFillSpace:
                    s = "cs";
                    break;
                case PDFOpCode.ColorSetStrokeSpace:
                    s = "CS";
                    break;
                case PDFOpCode.ColorFillPattern:
                    s = "scn";
                    break;
                case PDFOpCode.ColorStrokePattern:
                    s = "SCN";
                    break;
                case PDFOpCode.ColorFillGrayscaleSpace:
                    s = "g";
                    break;
                case PDFOpCode.ColorStrokeGrayscaleSpace:
                    s = "G";
                    break;
                case PDFOpCode.ColorFillRGBSpace:
                    s = "rg";
                    break;
                case PDFOpCode.ColorStrokeRGBSpace:
                    s = "RG";
                    break;
                case PDFOpCode.ColorFillCMYK:
                    s = "k";
                    break;
                case PDFOpCode.ColorStrokeCMYK:
                    s = "K";
                    break;
                case PDFOpCode.ColorFillOpacity:
                    s = "ca";
                    break;
                case PDFOpCode.ColorStrokeOpacity:
                    s = "CA";
                    break;
                case PDFOpCode.SaveState:
                    s = "q";
                    break;
                case PDFOpCode.RestoreState:
                    s = "Q";
                    break;
                case PDFOpCode.XobjPaint:
                    s = "Do";
                    break;
                case PDFOpCode.XobjBegin:
                    s = "BI";
                    break;
                case PDFOpCode.XobjImageData:
                    s = "ID";
                    break;
                case PDFOpCode.XobjEndImage:
                    s = "EI";
                    break;
                case PDFOpCode.TxtBegin:
                    s = "BT";
                    break;
                case PDFOpCode.TxtEnd:
                    s = "ET";
                    break;
                case PDFOpCode.TxtCharSpacing:
                    s = "Tc";
                    break;
                case PDFOpCode.TxtWordSpacing:
                    s = "Tw";
                    break;
                case PDFOpCode.TxtHScaling:
                    s = "Tz";
                    break;
                case PDFOpCode.TxtLeading:
                    s = "TL";
                    break;
                case PDFOpCode.TxtFont:
                    s = "Tf";
                    break;
                case PDFOpCode.TxtPaint:
                    s = "Tj";
                    break;
                case PDFOpCode.TxtPaintArray:
                    s = "TJ";
                    break;
                case PDFOpCode.TxtRenderMode:
                    s = "Tr";
                    break;
                case PDFOpCode.TxtRise:
                    s = "Ts";
                    break;
                case PDFOpCode.TxtMoveNextOffset:
                    s = "Td";
                    break;
                case PDFOpCode.TxtTransformMatrix:
                    s = "Tm";
                    break;
                case PDFOpCode.TxtNextLine:
                    s = "T*";
                    break;
                case PDFOpCode.GraphTransformMatrix:
                    s = "cm";
                    break;
                case PDFOpCode.GraphLineWidth:
                    s = "w";
                    break;
                case PDFOpCode.GraphLineCap:
                    s = "J";
                    break;
                case PDFOpCode.GraphLineJoin:
                    s = "j";
                    break;
                case PDFOpCode.GraphMiterLimit:
                    s = "M";
                    break;
                case PDFOpCode.GraphDashPattern:
                    s = "d";
                    break;
                case PDFOpCode.GraphRenderingIntent:
                    s = "ri";
                    break;
                case PDFOpCode.GraphFlatness:
                    s = "i";
                    break;
                case PDFOpCode.GraphApplyState:
                    s = "gs";
                    break;
                case PDFOpCode.GraphMove:
                    s = "m";
                    break;
                case PDFOpCode.GraphLineTo:
                    s = "l";
                    break;
                case PDFOpCode.GraphCurve2Handle:
                    s = "c";
                    break;
                case PDFOpCode.GraphCurve1HandleEnd:
                    s = "v";
                    break;
                case PDFOpCode.GraphCurve1HandleBegin:
                    s = "y";
                    break;
                case PDFOpCode.GraphClose:
                    s = "h";
                    break;
                case PDFOpCode.GraphRect:
                    s = "re";
                    break;
                case PDFOpCode.GraphStrokePath:
                    s = "S";
                    break;
                case PDFOpCode.GraphCloseAndStroke:
                    s = "s";
                    break;
                case PDFOpCode.GraphFillPath:
                    s = "f";
                    break;
                case PDFOpCode.GraphFillPathEvenOdd:
                    s = "f*";
                    break;
                case PDFOpCode.GraphFillAndStroke:
                    s = "B";
                    break;
                case PDFOpCode.GraphFillAndStrokeEvenOdd:
                    s = "B*";
                    break;
                case PDFOpCode.GraphCloseFillStroke:
                    s = "b";
                    break;
                case PDFOpCode.GraphCloseFileStrokeEvenOdd:
                    s = "b*";
                    break;
                case PDFOpCode.GraphNoOp:
                case PDFOpCode.GraphEndPath:
                    s = "n";
                    break;
                case PDFOpCode.GraphSetClip:
                    s = "W";
                    break;
                default:
                    break;
            }
            return s;
        }
        

        #region Validation Methods

        private string ValidateName(string name)
        {
            //TODO: Increase validation support
            name = name.Replace(" ", "#20");
            return name;
        }

        private string ValidateLiteral(string value)
        {
            //TODO: Implement Literal Validation
            return value;
        }

        #endregion

        
        private void Log(string message)
        {
            this.Log(TraceDefaultLevel, TraceCategory, message);
        }

        private void Log(TraceLevel level, string message)
        {
            this.Log(level, TraceCategory, message);
        }

        private void Log(TraceLevel level, string category, string message)
        {
            if (null != this.TraceLog)
                this.TraceLog.Add(level, category, message);
        }

        protected override void Dispose(bool disposing)
        {
            Log(TraceLevel.Message, "Disposing PDF writer");
            base.Dispose(disposing);
            if (disposing)
            {
                foreach (IIndirectObject pfo in this.XRefTable.References)
                {
                    Log(TraceLevel.Debug, "Disposing indirect object '" + pfo.ToString());
                    pfo.Dispose();
                }
            }
        }
    }
}
