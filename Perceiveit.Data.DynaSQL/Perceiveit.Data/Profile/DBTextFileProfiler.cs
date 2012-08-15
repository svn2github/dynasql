/*  Copyright 2009 PerceiveIT Limited
 *  This file is part of the DynaSQL library.
 *
*  DynaSQL is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  DynaSQL is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Query in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.Profile
{
    /// <summary>
    /// Simple Text file profiler
    /// </summary>
    public class DBTextFileProfiler : DBProfilerBase
    {
        //TODO: Add max size for the profiler log
        private const string FilePathSettingKey = "file-path";

        private static object _filelock = new object();

        private string _filepath;


        public override System.Collections.Specialized.NameValueCollection Settings
        {
            get
            {
                return base.Settings;
            }
            set
            {
                base.Settings = value;
                _filepath = null; //reset the filepath incase the Settings value has changed
            }
        }


        protected string FilePath
        {
            get
            {
                if (string.IsNullOrEmpty(_filepath))
                    _filepath = GetFilePath();
                return _filepath;
            }
        }

        public DBTextFileProfiler(string name)
            : base(name, false) //we don't collect summaries
        {
            _filepath = null;
        }

        private const string HeaderString = "Index\tDuration (ms)\tSQL\tParameters\r\n";

        /// <summary>
        /// Main method to write the line of SQL to the text file
        /// </summary>
        /// <param name="data"></param>
        protected override void RegisterExecutionComplete(ProfilerExecData data)
        {
            base.RegisterExecutionComplete(data);

            StringBuilder sb = new StringBuilder();
            string baseformat = "{0:0000000}";
            
            string sql = data.SQL.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ');
            
            string dur = string.Format("{0:0.0000}", data.Duration.TotalMilliseconds);
            if (dur.Length < 12)
                dur = dur.PadLeft(12);

            
            sb.AppendFormat(baseformat, data.ExecutionID);
            sb.Append("\t");
            sb.Append(dur);
            sb.Append("\t");
            sb.Append(sql);

            if (data.Parameters != null && data.Parameters.Length > 0)
            {
                foreach (object p in data.Parameters)
                {
                    sb.Append('\t');
                    if (null == p || p is DBNull)
                        sb.Append("[NULL]");
                    else
                        sb.Append(p);
                }
            }
            sb.Append("\r\n");

            string path = this.FilePath;
            path = path.Replace("[Date]", DateTime.Today.ToString("yyyy_MM_dd"))
                       .Replace("[Name]", this.ProfilerName)
                       .Replace("[Connection]",data.Connection);
                
            lock (_filelock)
            {
                if (!System.IO.File.Exists(path))
                    System.IO.File.AppendAllText(path, HeaderString);
                System.IO.File.AppendAllText(path, sb.ToString());
            }
        }

        //
        // support methods
        //

        protected virtual string GetFilePath()
        {
            string path = null;
            if (null != this.Settings && this.Settings.Count > 0)
            {
                path = this.Settings[FilePathSettingKey];
            }
            if (string.IsNullOrEmpty(path))
                path = System.IO.Path.Combine("C:/temp/", "DBProfiler_[Date].log");

            return path;
        }

        
    }

    /// <summary>
    /// Profiler factory that creates new instances of the DBTextFileProfiler
    /// </summary>
    public class DBTextFileProfilerFactory : IDBProfilerFactory
    {

        

        private System.Collections.Specialized.NameValueCollection _settings; 

        #region IDBProfilerFactory Members

        /// <summary>
        /// Gets or Sets the name to begiven to any instances of this profiler
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// Gets or sets the collection of settings that all instances thisfactory creates should use.
        /// </summary>
        public System.Collections.Specialized.NameValueCollection Settings
        {
            get
            {
                if (null == _settings)
                    _settings = new System.Collections.Specialized.NameValueCollection();
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }

        /// <summary>
        /// Returns a new initialized Profiler
        /// </summary>
        /// <returns></returns>
        public IDBProfiler GetProfiler(string name)
        {
            if(string.IsNullOrEmpty(name))
                name = this.Name;

            DBTextFileProfiler profiler = new DBTextFileProfiler(this.Name);
            profiler.Settings = this.Settings;
            return profiler;
        }

        #endregion
    }
}
