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
    /// Acts as a base class for standard IDBProfilers so 
    /// new methods of profiling can be quickly implemented
    /// </summary>
    public abstract class DBProfilerBase : IDBProfiler
    {

        //
        // ivars
        //

        static int _nextid = 1; //indexing

        ProfileDataSummaryCollection _summaries = new ProfileDataSummaryCollection(); //collection of connection and SQL string unique summaries
        object _summaryLock = new object(); //lock for adding to the summariescollection in a thread safe manner
        private string _name; // the identifiable name of this profiler
        private bool _collect; //flag to check if this instance is collecting summaries
        private Stopwatch _running; //the global stopwatch for recording elapsted times
        private NameValueCollection _settings;//any settings associated with the profiler

        //
        // properties
        //

        #region  public string ProfilerName

        /// <summary>
        /// Gets the name of this profiler
        /// </summary>
        public string ProfilerName
        {
            get { return this._name; }
        }

        #endregion

        #region public bool CollectsSummaries {get;}

        /// <summary>
        /// Gets the flag to identify if this profiler is collecting summaries
        /// </summary>
        public bool CollectsSummaries
        {
            get { return _collect; }
        }

        #endregion

        #region public virtual NameValueCollection Settings {get;set;}

        /// <summary>
        /// Gets or Sets the settings associated with this Profiler
        /// </summary>
        public virtual NameValueCollection Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        #endregion

        //
        // .ctor
        //

        #region public DBProfilerBase(string name, bool collectSummary)
        
        /// <summary>
        /// Constructor accepts a name and a flag for summary collection
        /// </summary>
        /// <param name="name">The name of this Profiler</param>
        /// <param name="collectSummary">True if summaries need to be collected</param>
        protected DBProfilerBase(string name, bool collectSummary)
        {
            if(string.IsNullOrEmpty(name))
                name = "[Un-named Profiler]";

            this._name = name;
            this._collect = collectSummary;
            this._running = Stopwatch.StartNew();
        }

        #endregion

        //
        // public methods
        //

       
        #region public IDBProfileExecData BeginExecution(string connectionName, string sql, params object[] parameters)

        /// <summary>
        /// Registers the start of an execution and returns the ProfileData instance which 
        /// encapsulates the required data and identifies it. 
        /// </summary>
        /// <param name="connectionName"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDBProfileExecData BeginExecution(string connectionName, string sql, params object[] parameters)
        {
            int id = _nextid++;//should place this in a lock, but the impact of is greater than the chance of clash
            if (_nextid == int.MaxValue)
                _nextid = 0;
            ProfilerExecData data = new ProfilerExecData(connectionName, id, sql, _running.Elapsed);
            data.Parameters = parameters;
            this.RegisterBeginExecution(id, data);
            return data;
        }

        #endregion


        #region public void EndExecution(IDBProfileExecData data)

        /// <summary>
        /// Registers the end of the execution for a specific execution data - this is the ProfileData returned from the BeginExecution method
        /// </summary>
        /// <param name="data">The profile data returned from a BeginExecution method</param>
        public void EndExecution(IDBProfileExecData data)
        {
            if (null == data)
                throw new ArgumentNullException("data");
            if (!(data is ProfilerExecData))
                throw new InvalidCastException("data");

            ProfilerExecData execdata = data as ProfilerExecData;

            execdata.Ended = _running.Elapsed;
            this.RegisterExecutionComplete(execdata);

            if (this._collect)
                this.UpdateExecutionSummary(execdata);
        }

        #endregion


        #region public void DumpExecutionSummary()


        /// <summary>
        /// Dumps all the collected summaries for distinct SQL strings and
        /// the name of the connection it was executed under to this profilers output
        /// </summary>
        public void DumpExecutionSummary()
        {
            if (this.CollectsSummaries)
            {
                //make sure the lock is in place so we do not get any dirty writes
                lock (_summaryLock)
                {
                    foreach (ProfilerExecSummary sum in _summaries)
                    {
                        this.DumpAnExecutionSummary(sum);
                    }
                    _summaries.Clear();
                }
            }
        }

        #endregion


        //
        // support methods
        //

        #region private void UpdateExecutionSummary(ProfileData data)

        /// <summary>
        /// Updates a ProfileDataSummary by calling RegisterExecution 
        /// </summary>
        /// <param name="data"></param>
        private void UpdateExecutionSummary(ProfilerExecData data)
        {
            ProfilerExecSummary summary;
            if (!_summaries.TryGetSummary(data, out summary))
            {
                lock (_summaryLock)
                {
                    if (!_summaries.TryGetSummary(data, out summary))
                    {
                        summary = new ProfilerExecSummary(data.Connection, data.SQL);
                        _summaries.Add(summary);
                    }
                }
            }
            summary.RegisterExecution(data);
        }

        #endregion


        //
        // protected virtual methods that should be overriden as required.
        //

        #region protected virtual void RegisterBeginExecution(int execid, ProfileData data)

        /// <summary>
        /// Inheritors should override this method if they wish to
        /// record the commencement of an DBExecution. Default implementation does nothing
        /// </summary>
        /// <param name="execid"></param>
        /// <param name="data"></param>
        protected virtual void RegisterBeginExecution(int execid, ProfilerExecData data)
        {
            //Empty method that can be overwritten
        }

        #endregion

        #region protected virtual void RegisterExecutionComplete(ProfileData data)

        /// <summary>
        /// Inheritors should override this method if they wish to
        /// record the completion of a Database execution. Default implementation does nothing
        /// </summary>
        /// <param name="data">The data associated with the Execution</param>
        protected virtual void RegisterExecutionComplete(ProfilerExecData data)
        {
            //Inheritors should override, or do nothing
        }

        #endregion



        #region protected virtual void BeginExecutionSummaryDump(int count)

        /// <summary>
        /// Override this method if the implementing class needs to output anything before the Summary dump starts.
        /// Default implementation does nothing.
        /// </summary>
        /// <param name="count">The total number of summaries to execute</param>
        protected virtual void BeginExecutionSummaryDump(int count)
        {
        }

        #endregion

        #region protected virtual void DumpAnExecutionSummary(ProfileDataSummary summary)

        /// <summary>
        /// Inheritors should override this method if they wish to
        /// record an summary set of executions against an individual SQL string. Default implementation does nothing
        /// </summary>
        /// <param name="summary"></param>
        protected virtual void DumpAnExecutionSummary(ProfilerExecSummary summary)
        {
            //Inheritors should override, or do nothing
        }

        #endregion

        #region protected virtual void EndExecutionSummaryDump(int count, IEnumerable<ProfilerExecSummary> summaries)

        /// <summary>
        /// Override this method if the implementing class needs to output anything after the Summary dump has completed.
        /// Default implementation does nothing.
        /// </summary>
        /// <param name="count">The total number of summaries</param>
        /// <param name="summaries">An enumerable list of all the summaries</param>
        protected virtual void EndExecutionSummaryDump(int count, IEnumerable<ProfilerExecSummary> summaries)
        {
        }

        #endregion

    }
}
