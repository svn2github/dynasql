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
using System.Diagnostics;
using System.Collections.Specialized;

namespace Perceiveit.Data.Profile
{
    /// <summary>
    /// Profiler that outputs to the console std output
    /// </summary>
    public class DBConsoleProfiler : DBProfilerBase
    {
        //
        // ctor(s)
        //

        #region public DBConsoleProfiler(string name, NameValueCollection settings)

        /// <summary>
        /// Creates a new Console profiler with the specified name and settings
        /// </summary>
        /// <param name="name"></param>
        /// <param name="settings"></param>
        public DBConsoleProfiler(string name, NameValueCollection settings)
            : base(name, true)
        {

        }

        #endregion

        //
        // implementation methods
        //

        #region protected override void RegisterExecutionComplete(ProfilerExecData data)

        /// <summary>
        /// overrides the base method to write a formatted string to the console.
        /// </summary>
        /// <param name="data"></param>
        protected override void RegisterExecutionComplete(ProfilerExecData data)
        {
            
            int index = 0;
            int sqllinelength = 70;
            string front = string.Format("{0:000000}:{1} -> ", data.ExecutionID, data.Connection);

            string full = data.SQL.Replace('\n', ' ').Replace("\r", "").Replace("\t", "");
            string padleft = "".PadLeft(front.Length);//format length of execution id and the colon and space.
            StringBuilder sb = new StringBuilder(front);
            while (index < full.Length)
            {
                if (index > 0)
                {
                    sb.Append(padleft);
                    sb.Append("...");
                }

                if (index + sqllinelength > full.Length)
                {
                    sb.Append(full, index, full.Length - index);
                }
                else
                {
                    sb.Append(full, index, sqllinelength);
                    sb.Append("...");
                }

                if (index == 0)
                {
                    sb.Append("   Duration:");
                    sb.Append(data.Duration);

                    //trim the line length to 3 less for the elipsis
                    index += 3;
                    sqllinelength -= 3;
                }

                sb.Append("\r\n");
                index += sqllinelength;
            }
            
            Console.Write(sb.ToString());
            base.RegisterExecutionComplete(data);
        }

        #endregion

        #region protected override void DumpAnExecutionSummary(ProfilerExecSummary summary)

        /// <summary>
        /// Overrides the base behaviour to output one execution summary to the console
        /// </summary>
        /// <param name="summary"></param>
        protected override void DumpAnExecutionSummary(ProfilerExecSummary summary)
        {
            int index = 0;
            int sqllinelength = 70;
            string front = string.Format("{0} -> ", summary.ConnectionID);
            string full = summary.SQL.Replace('\n',' ').Replace("\r","").Replace("\t","");
            string padleft = "".PadLeft(front.Length);//format length of execution id and the colon and space.
            StringBuilder sb = new StringBuilder();
            sb.Append(front);
            while (index < full.Length)
            {
                if (index > 0)
                {
                    sb.Append(padleft);
                    sb.Append("...");
                }

                if (index + sqllinelength > full.Length)
                {
                    sb.Append(full, index, full.Length - index);
                }
                else
                {
                    sb.Append(full, index, sqllinelength);
                    sb.Append("...");
                }

                if (index == 0)
                {
                    sb.Append("   Exec Count:");
                    sb.Append(summary.ExecCount);
                    if (summary.ExecCount > 1)
                    {
                        sb.Append(", Mean:");
                        sb.Append(summary.MeanDuration);
                        sb.Append(", Min:");
                        sb.Append(summary.MinDuration);
                        sb.Append(", Max:");
                        sb.Append(summary.MaxDuration);
                    }
                    else
                    {
                        sb.Append(", Duration:");
                        sb.Append(summary.MeanDuration);
                    }
                    index += 3;
                    sqllinelength -= 3;
                }
                sb.Append("\r\n");
                index += sqllinelength;
                //sqllinelength = 100;
            }

            Console.Write(sb.ToString());
            base.DumpAnExecutionSummary(summary);
        }

        #endregion


    }

    /// <summary>
    /// DBDebug profiler factory that always returns a the same (singleton) instance
    /// </summary>
    public class DBConsoleProfilerSingletonFactory : IDBProfilerFactory
    {

        #region public string Name
        /// <summary>
        /// NOT SUPPOTED: Cannot name a singleton profler.
        /// </summary>
        public string Name
        {
            get { return null; }
            set { throw new InvalidOperationException("Setting the name is not supported on the Singleton profiler"); }
        }

        #endregion

        #region public System.Collections.Specialized.NameValueCollection Settings {get;set;}


        /// <summary>
        /// Not supported - Cannot set the settings on the static instance
        /// </summary>
        public System.Collections.Specialized.NameValueCollection Settings
        {
            get { return null; }
            set { throw new InvalidOperationException("Settings are not supported on the Singleton Console profiler"); }
        }

        #endregion

        private static IDBProfiler _instance = new DBConsoleProfiler("Singleton", null);

        #region IDBProfilerFactory Members
        /// <summary>
        /// Gets the IDBProfiler for this ConsoleProfilerFactory
        /// </summary>
        /// <returns></returns>
        public IDBProfiler GetProfiler(string name)
        {
            return _instance;
        }

        #endregion

        /// <summary>
        /// Gets the always present default console profiler instance
        /// </summary>
        /// <returns></returns>
        public static IDBProfiler Instance() { return _instance; }
    }

    /// <summary>
    /// Implements the IDBProfilerFactory for the console profiler with customisable settings
    /// </summary>
    public class DBConsoleProfilerFactory : IDBProfilerFactory
    {

        #region public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the name of this Profiler
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region public System.Collections.Specialized.NameValueCollection Settings {get;set;}

        private System.Collections.Specialized.NameValueCollection _settings;

        /// <summary>
        /// Gets or sets the settings that this factory uses to create the console profiler
        /// </summary>
        public System.Collections.Specialized.NameValueCollection Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        #endregion

        #region IDBProfilerFactory Members
        /// <summary>
        /// Gets a new IDBProfiler 
        /// </summary>
        /// <returns></returns>
        IDBProfiler IDBProfilerFactory.GetProfiler(string name)
        {
            return GetProfiler(name);
        }

        #endregion

        #region public DBConsoleProfiler GetProfiler(string name)

        /// <summary>
        /// Returns a new DBConsoleProfiler (using a distinct set of this factories settings)
        /// </summary>
        /// <returns></returns>
        public DBConsoleProfiler GetProfiler(string name)
        {
            if (string.IsNullOrEmpty(name))
                name = this.Name;

            return new DBConsoleProfiler(name,CloneSettings());
        }

        #endregion

        #region private NameValueCollection CloneSettings()

        /// <summary>
        /// Clones any settings so that each profiler has their own copy as per the state when instantiated
        /// </summary>
        /// <returns></returns>
        private NameValueCollection CloneSettings()
        {
            if (null == this.Settings || this.Settings.Count == 0)
                return new NameValueCollection();
            else
            {
                return new NameValueCollection(this.Settings);
            }
        }

        #endregion

    }
}
