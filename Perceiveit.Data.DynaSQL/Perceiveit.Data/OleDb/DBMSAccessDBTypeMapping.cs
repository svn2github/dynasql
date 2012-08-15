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
using System.Data;

namespace Perceiveit.Data.OleDb
{
    /// <summary>
    /// Maps MSAccess types to runtime Types of Dot.Net 
    /// </summary>
    public class DBMSAccessTypeMapping
    {
        public int NativeType { get; set; }

        public string TypeName { get; set; }

        public Type RuntimeType { get; set; }

        public int ProviderType { get; set; }

        internal static DBMSAccessTypeMappingCollection LoadFromTable(System.Data.DataTable dt)
        {
            DBMSAccessTypeMappingCollection col = new DBMSAccessTypeMappingCollection();
            DataColumn provtype = Schema.DBSchemaProvider.GetColumn(dt, "ProviderDbType", true);
            DataColumn name = Schema.DBSchemaProvider.GetColumn(dt, "TypeName", true);
            DataColumn runtime = Schema.DBSchemaProvider.GetColumn(dt, "DataType", true);
            DataColumn native = Schema.DBSchemaProvider.GetColumn(dt, "NativeDataType", true);
            foreach (DataRow row in dt.Rows)
            {
                DBMSAccessTypeMapping map = new DBMSAccessTypeMapping();
                map.NativeType = Schema.DBSchemaProvider.GetColumnIntValue(row, native);
                map.TypeName = Schema.DBSchemaProvider.GetColumnStringValue(row, name);
                map.ProviderType = Schema.DBSchemaProvider.GetColumnIntValue(row, provtype);
                string type = Schema.DBSchemaProvider.GetColumnStringValue(row, runtime);

                if (!string.IsNullOrEmpty(type))
                {
                    map.RuntimeType = Type.GetType(type, false);
                }
                col.Add(map);

            }

            return col;
        }
    }

    internal class DBMSAccessTypeMappingCollection : List<DBMSAccessTypeMapping>
    {

        public bool TryGetType(int datatype, out DBMSAccessTypeMapping map)
        {
            if (this.Count > 0)
            {
                //check the ProviderDbType first
                foreach (DBMSAccessTypeMapping item in this)
                {
                    if (item.ProviderType == datatype)
                    {
                        map = item;
                        return true;
                    }
                }
                //if not found then check the NativeDataType
                foreach (DBMSAccessTypeMapping item in this)
                {
                    if (item.NativeType == datatype)
                    {
                        map = item;
                        return true;
                    }
                }
                
            }
            map = null;
            return false;
        }
    }
}
