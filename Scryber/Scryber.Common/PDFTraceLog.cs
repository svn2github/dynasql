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

namespace Scryber
{
    public abstract class PDFTraceLog : IDisposable
    {

        public const TraceRecordLevel DefaultTraceRecordLevel = TraceRecordLevel.All;

        private System.Diagnostics.Stopwatch _stopwatch;
        private int _level;
        private string _inset = string.Empty;
        private const string InsetString = "  ";
        

        public TraceRecordLevel RecordLevel
        {
            get { return (TraceRecordLevel)this._level; }
        }

        public string Indent
        {
            get { return this._inset; }
        }

        public PDFTraceLog(TraceRecordLevel recordlevel)
        {
            this._level = (int)recordlevel;
            this._stopwatch = new System.Diagnostics.Stopwatch();
            //this.Add(TraceLevel.Message, "Trace log started");
            this._stopwatch.Start();
        }

        public TimeSpan GetTimeStamp()
        {
            return _stopwatch.Elapsed;
        }

        public void IncrementIndent()
        {
            //_inset += InsetString;
        }

        public void DecrementIndent()
        {
            //if (_inset.Length > InsetString.Length)
            //    _inset = _inset.Substring(0, _inset.Length - InsetString.Length);
            //else
            //    _inset = string.Empty;
        }

        public void Begin(TraceLevel level, string message)
        {
            this.IncrementIndent();
            this.Add(level, "BEGIN", message);
        }

        public void Begin(string message)
        {
            this.IncrementIndent();
            this.Add(TraceLevel.Message,"BEGIN", message);
        }

        public void End(string message)
        {
            this.Add(TraceLevel.Message,"END", message);
            this.DecrementIndent();
        }

        public void End(TraceLevel level, string message)
        {
            this.Add(level,"END", message);
            this.DecrementIndent();
        }

        public void Add(string category, string message)
        {
            this.Add(TraceLevel.Message, category, message);
        }

        public void Add(TraceLevel level, string category, string message)
        {
            if (ShouldLog(level))
                this.Record(this.Indent, level, GetTimeStamp(), category, message, null);
        }

        
        public void Add(Exception ex)
        {
            if(ex != null)
                this.Add(TraceLevel.Error, ex.Message, ex);
        }

        public void Add(TraceLevel level, string message, Exception ex)
        {
            if (ShouldLog(level))
                this.Record(this.Indent, level, this.GetTimeStamp(), ex.GetType().Name, message, ex);
        }

        public void Add(TraceLevel level, string category, string message, Exception ex)
        {
            if (ShouldLog(level))
                this.Record(this.Indent, level, this.GetTimeStamp(), category, message, ex);
        }

        public bool ShouldLog(TraceLevel traceLevel)
        {
            return (int)traceLevel >= this._level;
        }

        internal protected abstract void Record(string inset, TraceLevel level, TimeSpan timestamp, string category, string message, Exception ex);

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                this._stopwatch.Stop();
                this._stopwatch = null;
            }
        }

        ~PDFTraceLog()
        {
            this.Dispose(false);
        }

        
    }

    public class DiagnosticsTraceLog : PDFTraceLog
    {
        public DiagnosticsTraceLog(TraceRecordLevel level)
            : base(level)
        {
        }

        private int _traceindex = 1;
        internal protected override void Record(string inset, TraceLevel level, TimeSpan timestamp, string category, string message, Exception ex)
        {
            _traceindex++;
            StringBuilder sb = new StringBuilder();
            string s = _traceindex.ToString().PadLeft(5, '0');
            sb.Append(s);
            sb.Append(" | ");
            s = timestamp.ToString();
            sb.Append(s);
            sb.Append(" | ");
            sb.Append(level.ToString().PadLeft(10));
            sb.Append(":");
            if (string.IsNullOrEmpty(inset) == false)
                sb.Append(inset);
            if (string.IsNullOrEmpty(category) == false)
            {
                sb.Append(category);
                sb.Append(" ");
            }
            sb.AppendLine(message);

            while (ex != null)
            {
                sb.Append("\t");
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.StackTrace);
                sb.AppendLine("");
                ex = ex.InnerException;
            }
            System.Diagnostics.Trace.Write(sb.ToString());
        }
    }

    public class PDFDiagnoticsTraceLogFactory : IPDFTraceLogFactory
    {

        public PDFTraceLog CreateLog(TraceRecordLevel level)
        {
            return new DiagnosticsTraceLog(level);
        }
    }

    public class DoNothingTraceLog : PDFTraceLog
    {
        public DoNothingTraceLog()
            : base(TraceRecordLevel.Off)
        { }

        internal protected override void Record(string inset, TraceLevel level, TimeSpan timestamp, string category, string message, Exception ex)
        {
            
        }
    }

    public class CompositeTraceLog : PDFTraceLog
    {
        private IEnumerable<PDFTraceLog> _inner;

        public CompositeTraceLog(TraceRecordLevel level, IEnumerable<PDFTraceLog> entries)
            : base(level)
        {
            if (null == entries)
                throw new ArgumentNullException("entries");
            this._inner = entries;
        }

        internal protected override void Record(string inset, TraceLevel level, TimeSpan timestamp, string category, string message, Exception ex)
        {
            foreach (PDFTraceLog log in _inner)
            {
                log.Record(inset, level, timestamp, category, message, ex);
            }

        }
    }
}
