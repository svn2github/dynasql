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
    public class ProfileDataSummaryCollection : System.Collections.ObjectModel.KeyedCollection<string, ProfilerExecSummary>
    {
        public ProfileDataSummaryCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        protected override string GetKeyForItem(ProfilerExecSummary item)
        {
            return GetKey(item.ConnectionID, item.SQL);
        }


        public bool TryGetSummary(ProfilerExecData data, out ProfilerExecSummary summary)
        {
            if (this.Count == 0)
            {
                summary = null;
                return false;
            }
            else
            {
                string key = GetKey(data.Connection, data.SQL);
                return this.Dictionary.TryGetValue(key, out summary);
            }

        }

        public string GetKey(string con, string sql)
        {
            return con + "||" + sql;
        }

    }
}
