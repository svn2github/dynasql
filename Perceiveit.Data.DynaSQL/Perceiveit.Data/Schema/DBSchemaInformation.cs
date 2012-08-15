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
using System.Linq;
using System.Text;

namespace Perceiveit.Data.Schema
{
    /// <summary>
    /// Holds database engine specific information 
    /// </summary>
    public class DBSchemaInformation
    {
        private string _schemaName;
        private Dictionary<DBSchemaTypes, DBSchemaInfoLookup> _listlookup;

        public string SchemaName
        {
            get { return _schemaName; }
            set { _schemaName = value; }
        }


        internal DBSchemaInformation(string schemaName, Dictionary<DBSchemaTypes, DBSchemaInfoLookup> lists)
        {
            this._schemaName = schemaName;
            this._listlookup = lists;
        }

        public DBSchemaInfoLookup GetListLookup(DBSchemaTypes type)
        {
            DBSchemaInfoLookup value;
            if (!_listlookup.TryGetValue(type, out value))
                value = null;
            return value;
        }


        public const string SchemaOwner = "INFORMATION_SCHEMA";

        public static DBSchemaInformation CreateDefault()
        {
            Dictionary<DBSchemaTypes, DBSchemaInfoLookup> lists = new Dictionary<DBSchemaTypes, DBSchemaInfoLookup>();
            lists.Add(DBSchemaTypes.Table, new DBSchemaInfoLookup("TABLES","TABLE_NAME","TABLE_SCHEMA"));
            lists.Add(DBSchemaTypes.View, new DBSchemaInfoLookup("VIEWS", "TABLE_NAME", "TABLE_SCHEMA"));
            lists.Add(DBSchemaTypes.StoredProcedure, new DBSchemaInfoLookup("ROUTINES", "ROUTINE_NAME", "ROUTINE_SCHEMA"));
            lists.Add(DBSchemaTypes.Function, new DBSchemaInfoLookup("ROUTINES", "ROUTINE_NAME", "ROUTINE_SCHEMA"));
            lists.Add(DBSchemaTypes.Index, new DBSchemaInfoLookup("INDEXES", "INDEX_NAME", "TABLE_NAME"));
            lists.Add(DBSchemaTypes.ForeignKey, new DBSchemaInfoLookup("KEY_COLUMN_USAGE", "CONSTRAINT_NAME", "CONSTRIAINT_SCHEMA"));
            return new DBSchemaInformation("INFORMATION_SCHEMA", lists);
        }

        public static DBSchemaInformation CreateOracle()
        {
            Dictionary<DBSchemaTypes, DBSchemaInfoLookup> lists = new Dictionary<DBSchemaTypes, DBSchemaInfoLookup>();
            lists.Add(DBSchemaTypes.Table, new DBSchemaInfoLookup("all_tables", "TABLE_NAME", "OWNER"));
            lists.Add(DBSchemaTypes.View, new DBSchemaInfoLookup("all_views", "VIEW_NAME", "OWNER"));
            lists.Add(DBSchemaTypes.StoredProcedure, new DBSchemaInfoLookup("all_procedures", "PROCEDURE_NAME", "OWNER"));
            lists.Add(DBSchemaTypes.Function, new DBSchemaInfoLookup("all_procedures", "PROCEDURE_NAME", "OWNER"));
            lists.Add(DBSchemaTypes.Index, new DBSchemaInfoLookup("all_indexes", "INDEX_NAME", "OWNER", "TABLE_OWNER", "TABLE_NAME"));
            lists.Add(DBSchemaTypes.ForeignKey, new DBSchemaInfoLookup("all_constraints", "CONSTRAINT_NAME", "OWNER", 
                Query.DBComparison.Equal(Query.DBField.Field("CONSTRAINT_TYPE"), Query.DBConst.String("R"))));

            return new DBSchemaInformation("", lists);
        }
        
    }


    /// <summary>
    /// Relates TABLE and column names so that a specific object can be found against the information schema
    /// </summary>
    public class DBSchemaInfoLookup
    {
        /// <summary>
        /// Gets the name of the Table where the information is stored
        /// </summary>
        public string TableName { get; private set; }
        /// <summary>
        /// Gets the name of the column that contains the object name
        /// </summary>
        public string LookupNameColumn { get; private set; }
        /// <summary>
        /// Gets the name of the column that contains the object schema name
        /// </summary>
        public string LookupSchemaColumn { get; private set; }

        /// <summary>
        /// Gets the name of the column that contains the parents name (e.g the table of the index)
        /// </summary>
        public string LookUpParentNameColumn { get; private set; }

        /// <summary>
        /// Gets the name of the column that contains the parents schema (e.g the schema of the table of the index)
        /// </summary>
        public string LookupParentSchemaColumn { get; private set; }

        /// <summary>
        /// Gets any restrictions that should be applied to the view to only return the required data
        /// </summary>
        public Query.DBClause Restrictions { get; private set; }

        public DBSchemaInfoLookup(string table, string namecolumn, string schemacolumn)
            : this(table, namecolumn, schemacolumn, null, null, null)
        {
        }

        public DBSchemaInfoLookup(string table, string namecolumn, string schemacolumn, string ownernameColumn, string ownerschemaColumn)
            : this(table, namecolumn, schemacolumn, ownernameColumn, ownerschemaColumn, null)
        {
        }

        public DBSchemaInfoLookup(string table, string namecolumn, string schemacolumn, Query.DBClause restrictions)
            : this(table, namecolumn, schemacolumn, null, null, restrictions)
        {
        }
        

        public DBSchemaInfoLookup(string table, string namecolumn, string schemacolumn, string parentNameColumn, string parentSchemaColumn, Query.DBClause restrictions)
        {
            this.TableName = table;
            this.LookupNameColumn = namecolumn;
            this.LookupSchemaColumn = schemacolumn;
            this.LookUpParentNameColumn = parentNameColumn;
            this.LookupParentSchemaColumn = parentSchemaColumn;
            this.Restrictions = restrictions;
        }
    }
}
