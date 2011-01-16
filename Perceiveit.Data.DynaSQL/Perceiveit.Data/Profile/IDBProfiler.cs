/*  Copyright 2009 PerceiveIT Limited
 *  This file is part of the DynaSQL library.
 *
*  DynaSQL is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  DynaSQL is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Query in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.Profile
{

    /// <summary>
    /// Defines a Profiler interface that the DBDatabase can use to record executions.
    /// </summary>
    /// <remarks>
    /// To explicity use and implementing profiler with the DBDatabase. Create an instance to the required profiler (e.g DBConsoleProfiler)
    /// and call the AttachProfiler on the DBDatabase instance (passing 'true' to start the profiling). The DBDatabase will then register 
    /// the start and finish of a SQL statement with this profiler through it BeginExecution and EndExecution methods.
    /// </remarks>
    public interface IDBProfiler
    {

        /// <summary>
        /// Gets the name of this profiler
        /// </summary>
        string ProfilerName { get; }


        /// <summary>
        /// Returns the flag to identify if this profiler supports the collection of summaries (amagamations and averages of executions)
        /// </summary>
        bool CollectsSummaries { get; }


        /// <summary>
        /// Method invoke on a profiler to enable it to record the start of an execution. Returns the IDBProfileExecData as a reference to the stating execution
        /// </summary>
        /// <param name="connectonName">The name of the connection the statement will be executed with</param>
        /// <param name="sql">The full SQL statement to be executed</param>
        /// <param name="parameters">Any/all parameters that are passed with the sql execution</param>
        /// <returns>An instance of the IDBProfileExecData that can be used to register the end of the execution.</returns>
        IDBProfileExecData BeginExecution(string connectonName, string sql, params object[] parameters);

        /// <summary>
        /// Method to invoke on a profiler to enble it to record the end of an execution.
        /// </summary>
        /// <param name="data"></param>
        void EndExecution(IDBProfileExecData data);

        /// <summary>
        /// If the Profiler supports collection of summary data
        /// </summary>
        void DumpExecutionSummary();
    }

    /// <summary>
    /// An IDBProfilerFactory creates instances of an IDBProfiler. Usually set up from the Configuration file.
    /// </summary>
    /// <remarks>the implementor of an IDBFactory must implement</remarks>
    public interface IDBProfilerFactory
    {
        /// <summary>
        /// Get or sets the name of this factory
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A colection of 'Settings' used in theconstruction of an IDBProfiler instance. 
        /// Supports custiomisation of any profilers - see specific implementations documentation for details of the settings they support
        /// </summary>
        System.Collections.Specialized.NameValueCollection Settings { get; set; }

        /// <summary>
        /// Creates and returns an IDBProfiler appropritate for the factory (correctly configured from any settings).
        /// </summary>
        /// <returns></returns>
        IDBProfiler GetProfiler(string forDbName);

    }

    /// <summary>
    /// A read-only data structure returned from a IDBProfiler and should be passed back when execution has completed
    /// </summary>
    public interface IDBProfileExecData
    {
        /// <summary>
        /// Gets the offset from base time when the registered execution started (can be Zero if it wasn't / hasn't been started).
        /// </summary>
        TimeSpan Started { get; }

        /// <summary>
        /// Gets the offset from the base time when the registered execution ended (can be Zero if it wasn't / hasn't been completed).
        /// </summary>
        TimeSpan Ended { get;}

        /// <summary>
        /// Gets the flag that identifies if this profiler has ended
        /// </summary>
        bool HasEnded {get;}

        /// <summary>
        /// Gets the duration of the execution (or Zero if it has not been started and ended).
        /// </summary>
        TimeSpan Duration { get; }
    }
}
