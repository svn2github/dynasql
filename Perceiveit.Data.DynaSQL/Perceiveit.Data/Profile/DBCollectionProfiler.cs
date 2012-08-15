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
    /// Wraps a number of IDBProfiler(s) into a single collection that can be assigned to a DBDatabase.AttachProfiler(...)
    /// </summary>
    public class DBCollectionProfiler : IDBProfiler, IEnumerable<IDBProfiler>
    {

        //
        // inner classes
        //

        #region internal class ProfilerExecDataMock : ProfilerExecData

        /// <summary>
        /// Mocks an array of ProfilerExecData entries into a single entry
        /// </summary>
        internal class ProfilerExecDataMock : ProfilerExecData
        {
            private IDBProfileExecData[] _inneritems;
            private int _changeLogIndex; //incrementing index to validate returns

            /// <summary>
            /// Gets teh change index which was valid when this instance was created
            /// </summary>
            public int ChangeIndex { get { return _changeLogIndex; } }

            /// <summary>
            /// Gets the Inner ProfilerExecDataItems
            /// </summary>
            public IDBProfileExecData[] InnerItems { get { return _inneritems; } }

            /// <summary>
            /// Creates a new instance containing all the inner items, and a collection change index
            /// </summary>
            public ProfilerExecDataMock(IDBProfileExecData[] inner, int changelogIndex)
                : base(inner[0].Started)
            {
                _inneritems = inner;
                _changeLogIndex = changelogIndex;
            }

            public override object[] Parameters
            {
                get
                {
                    return base.Parameters;
                }
            }

            public override TimeSpan Ended
            {
                get
                {
                    return base.Ended;
                }
                
            }
        }

        #endregion


        //
        // ivars
        //

        private List<IDBProfiler> _inneritems; // inner list of IDBProfilers
        private int _changeIndex; //change log so begin and end executions cannot confict with a modification to this collection
        private string _name; //the name of the DBCollectionProfiler

        //
        // properties
        //

        #region public string ProfilerName {get;}

        /// <summary>
        /// Gets the name of this profiler collection
        /// </summary>
        public string ProfilerName 
        {
            get { return _name; }
        }


        #endregion

        #region public int Count {get;}

        /// <summary>
        /// Gets the number of profilers in this collection
        /// </summary>
        public int Count
        {
            get { return _inneritems.Count; }
        }

        #endregion

        #region public IDBProfiler this[int index] {get;}

        /// <summary>
        /// Gets the profiler at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IDBProfiler this[int index]
        {
            get { return this._inneritems[index]; }
        }

        #endregion

        #region public bool CollectsSummaries {get;}

        /// <summary>
        /// Checks the inner profilers to see if any of them collect summararies. 
        /// If one or more do then returns true otherwise returns false.
        /// </summary>
        public bool CollectsSummaries
        {
            get
            {
                foreach (IDBProfiler prof in this._inneritems)
                {
                    if (prof.CollectsSummaries)
                        return true;
                }
                return false;
            }
        }

        #endregion

        //
        // .ctor
        //

        #region public DBCollectionProfiler(string name, IEnumerable<IDBProfiler> all)
        /// <summary>
        /// Creates a new instance of of the DBCollectionProfiler, to hold and refer to multiple profilers at once 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="all"></param>
        public DBCollectionProfiler(string name, IEnumerable<IDBProfiler> all)
        {
            if (null != all)
            {
                _inneritems = new List<IDBProfiler>(all);
            }
            else
                _inneritems = new List<IDBProfiler>();
            _changeIndex = 1;
            _name = name;
        }

        #endregion

        //
        // methods
        //

        #region public void Add(IDBProfiler profiler)
        /// <summary>
        /// Add a new profiler to this collection
        /// </summary>
        /// <param name="profiler"></param>
        public void Add(IDBProfiler profiler)
        {
            if (null == profiler)
                throw new ArgumentNullException("profiler");

            _changeIndex++;
            _inneritems.Add(profiler);
        }

        #endregion

        #region public bool Remove(IDBProfiler profiler)

        /// <summary>
        /// Remove the specified profiler from the collection
        /// </summary>
        /// <param name="profiler"></param>
        /// <returns></returns>
        public bool Remove(IDBProfiler profiler)
        {
            bool matched = _inneritems.Remove(profiler);
            if (matched)
                _changeIndex++;
            return matched;
        }

        #endregion

        #region public void RemoveAt(int index)

        /// <summary>
        /// Remove the profiler at the specified index
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            this._inneritems.RemoveAt(index);
            _changeIndex++;
        }

        #endregion


        //
        // IEnumerable<IDBProfiler> Implementation
        //

        #region public IEnumerator<IDBProfiler> GetEnumerator()

        /// <summary>
        /// Returns a new enumerator of IDBProfiler(s) within this collection
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IDBProfiler> GetEnumerator()
        {
            return _inneritems.GetEnumerator();
        }

        #endregion

        #region System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()

        /// <summary>
        /// Expicit IEnumerable implementation
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        //
        // IDBProfiler Implementation
        //


        /// <summary>
        /// Registers the start of an SQL execution - based on the parameters, and returns the ProfilerExecData to reference and send back to EndExecution
        /// </summary>
        /// <param name="connectonName"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDBProfileExecData BeginExecution(string connectonName, string sql, params object[] parameters)
        {
            int changelogindex = this._changeIndex;
            IDBProfileExecData[] all = new IDBProfileExecData[this.Count];
            for (int i = 0; i < all.Length; i++)
            {
                all[i] = this[i].BeginExecution(connectonName, sql, parameters);
            }
            return new ProfilerExecDataMock(all, changelogindex);
        }

        /// <summary>
        /// Registers the completion of an execution (calling end on each of the contained profilers)
        /// </summary>
        /// <param name="data"></param>
        public void EndExecution(IDBProfileExecData data)
        {
            if (!(data is ProfilerExecDataMock))
                throw new InvalidCastException("data");

            ProfilerExecDataMock mock = data as ProfilerExecDataMock;

            //make sure this collection has not changed since the begin exec call - we'd be in an indeterminate state.
            if (mock.ChangeIndex != this._changeIndex)
                throw new InvalidOperationException(Errors.ProfilerCollectionChangedBetweenStartAndEnd);
            
            for (int i = 0; i < _inneritems.Count; i++)
            {
                IDBProfiler prof = _inneritems[i];
                IDBProfileExecData innerdata = mock.InnerItems[i];
                prof.EndExecution(innerdata);
            }
        }

        /// <summary>
        /// Dumps the summaries
        /// </summary>
        public void DumpExecutionSummary()
        {
            foreach (IDBProfiler prof in this._inneritems)
            {
                prof.DumpExecutionSummary();
            }
        }
        
    }
}
