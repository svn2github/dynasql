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
    public class ProfilerExecSummary
    {
        private string _sql;
        private string _con;
        private int _exec;
        private long _dur, _min, _max;
        private List<long> _all;

        public string SQL
        {
            get { return _sql; }
            private set { _sql = value; }
        }

        public string ConnectionID
        {
            get { return _con; }
            private set { _con = value; }
        }

        public int ExecCount
        {
            get { return _exec; }
            private set { _exec = value; }
        }

        public TimeSpan MaxDuration 
        {
            get { return new TimeSpan(_max); }
        }

        public TimeSpan MinDuration
        {
            get { return new TimeSpan(_min); }
        }
       
        public TimeSpan TotalDuration
        {
            get { return new TimeSpan(_dur); } 
        }

        public TimeSpan MeanDuration
        {
            get
            {
                if (_exec > 1)
                {
                    long avg = _dur / (long)_exec;
                    return new TimeSpan(avg);
                }
                else if (_exec == 1)
                {
                    return new TimeSpan(_dur);
                }
                else
                    return TimeSpan.Zero;
            }
        }

        public TimeSpan MedianDuration
        {
            get
            {
                if (_exec > 1)
                {
                    List<long> sorted = new List<long>(_all);
                    sorted.Sort();
                    int index = sorted.Count / 2;
                    return new TimeSpan(sorted[index]);
                }
                else if (_exec == 1)
                    return new TimeSpan(_dur);
                else
                    return TimeSpan.Zero;
            }
        }

        public TimeSpan[] AllInExecutionOrder()
        {
            if (_exec > 1)
            {
                TimeSpan[] all = new TimeSpan[_all.Count];
                for (int i = 0; i < _all.Count; i++)
                {
                    all[i] = new TimeSpan(_all[i]);
                }
                return all;
            }
            else if (_exec == 1)
            {
                return new TimeSpan[] { new TimeSpan(_dur) };
            }
            else
                return new TimeSpan[] { };
        }

        public ProfilerExecSummary(string connection, string sql)
        {
            this._con = connection;
            this._sql = sql;
            this._exec = 0;
            this._dur = this._max = this._min = 0L;
            this._all = null;
        }



        public virtual void RegisterExecution(ProfilerExecData dataexec)
        {
            if (null == dataexec)
                throw new ArgumentNullException("dataexec");
            else if (!string.Equals(dataexec.SQL, this.SQL, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentOutOfRangeException("dataexec", "The SQL strings do not match");

            
            long val = dataexec.Duration.Ticks;

            //update _all list
            if (_exec > 0)
            {
                //second run so init _all list and add the first
                if (_exec == 1)
                {
                    _all = new List<long>();
                    _all.Add(_dur);
                }
                _all.Add(val);
            }

            //update total + mean
            _dur += val;
            _exec += 1;

            //update min max
            if (_exec == 1)
            {
                _min = val;
                _max = val;
            }
            else
            {
                _max = Math.Max(_max, val);
                _min = Math.Min(_min, val);
            }

        }
    }
}
