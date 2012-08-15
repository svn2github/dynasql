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
    public class ProfilerExecData : IDBProfileExecData
    {
        public string Connection { get; set; }
        public int ExecutionID { get; set; }
        public string SQL { get; set; }
        public virtual object[] Parameters { get; set; }
        public TimeSpan Started { get; set; }
        public virtual TimeSpan Ended { get; set; }

        public bool HasEnded { get { return Ended != TimeSpan.Zero; } }

        public TimeSpan Duration
        {
            get
            {
                if (Ended == TimeSpan.Zero || Started == TimeSpan.Zero)
                    return TimeSpan.Zero;
                else
                    return Ended - Started;
            }
        }

        public ProfilerExecData(string connection, int execid, string sql, TimeSpan starttime) 
        {
            this.Connection = connection;
            this.ExecutionID = execid;
            this.SQL = sql;
            this.Started = starttime;
            this.Ended = TimeSpan.Zero;
        }
        
        /// <summary>
        /// supports inheritors on the minimal profile exec data interface
        /// </summary>
        /// <param name="starttime"></param>
        protected ProfilerExecData(TimeSpan starttime)
            : this(string.Empty, -1, string.Empty, starttime)
        {

        }

        public override int GetHashCode()
        {
            return this.ExecutionID;
        }

        public bool Equals(ProfilerExecData data)
        {
            return this.ExecutionID == data.ExecutionID && this.Connection == data.Connection;
        }


        public override bool Equals(object obj)
        {
            return this.Equals((ProfilerExecData)obj);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Exec");
            sb.Append(ExecutionID.ToString("{0:000000}"));
            sb.Append(" ");
            sb.Append(this.SQL.Replace("\n", " _ "));
            if (null != this.Parameters && this.Parameters.Length > 0)
            {
                sb.Append(", Parameters : ");
                foreach (object obj in this.Parameters)
                {
                    if (null == obj || obj is DBNull)
                        sb.Append("[NULL]");
                    else
                    {
                        sb.Append("'");
                        sb.Append("'");
                        sb.Append(obj.ToString());
                    }
                    sb.Append(",");
                }

                sb.Length -= 1;
            }
            sb.Append("Duration : ");
            sb.Append(this.Ended - this.Started);

            return sb.ToString();
        }
    }
}
