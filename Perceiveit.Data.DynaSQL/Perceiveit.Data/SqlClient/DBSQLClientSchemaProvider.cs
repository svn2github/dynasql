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
using System.Data;
using System.Data.Common;
using Perceiveit.Data.Schema;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.SqlClient
{
    public class DBSQLClientSchemaProvider : DBSchemaProvider
    {
        //
        // .ctor
        //       

        public DBSQLClientSchemaProvider(DBDatabase database, DBDatabaseProperties properties)
            : base(database, properties)
        {
        }

        //
        // DoGetSchemaItems + support methods
        //

        #region protected override IEnumerable<DBSchemaItemRef> DoGetSchemaItems(DBSchemaTypes types)

        protected override IEnumerable<DBSchemaItemRef> DoGetSchemaItems(DBSchemaTypes types)
        {
            List<DBSchemaItemRef> items = new List<DBSchemaItemRef>();
            using (DbConnection con = this.Database.CreateConnection())
            {
                con.Open();
                if (IsSchemaType(types, DBSchemaTypes.Table))
                    this.LoadTableRefs(con, items);
                if (IsSchemaType(types, DBSchemaTypes.View))
                    this.LoadViewRefs(con, items);
                if (IsSchemaType(types, DBSchemaTypes.Index))
                    this.LoadIndexRefs(con, items);
                if (IsSchemaType(types, DBSchemaTypes.StoredProcedure)
                    && IsSchemaType(types, DBSchemaTypes.Function))
                    this.LoadSprocAndFunctionRefs(con, items);
                else if (IsSchemaType(types, DBSchemaTypes.Function))
                    this.LoadFunctionRefs(con, items);
                else if (IsSchemaType(types, DBSchemaTypes.StoredProcedure))
                    this.LoadSprocRefs(con, items);
            }

            return items;
        }

        #endregion

        #region protected int LoadTableRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)

        /// <summary>
        /// Loads all the Tables for a specific Database connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="addtocollection"></param>
        /// <returns></returns>
        protected int LoadTableRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)
        {
            int count = 0;
            DataTable dt = con.GetSchema(SchemaTablesName);
            DataColumn catalog = dt.Columns["TABLE_CATALOG"];
            DataColumn schema = dt.Columns["TABLE_SCHEMA"];
            DataColumn name = dt.Columns["TABLE_NAME"];
            DataColumn type = dt.Columns["TABLE_TYPE"];

            foreach (DataRow row in dt.Rows)
            {
                if (row[type].ToString() == "BASE TABLE")
                {
                    DBSchemaItemRef iref = new DBSchemaItemRef();
                    iref.Catalog = row[catalog].ToString();
                    iref.Schema = row[schema].ToString();
                    iref.Name = row[name].ToString();
                    iref.Type = DBSchemaTypes.Table;

                    addtocollection.Add(iref);
                    count++;
                }
            }

            return count;
        }

        #endregion

        #region protected int LoadViewRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)

        /// <summary>
        /// Loads all the Views for a specific database
        /// </summary>
        /// <param name="con"></param>
        /// <param name="addtocollection"></param>
        /// <returns></returns>
        protected int LoadViewRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)
        {
            int count = 0;
            DataTable dt = con.GetSchema(SchemaViewsName);
            DataColumn catalog = dt.Columns["TABLE_CATALOG"];
            DataColumn schema = dt.Columns["TABLE_SCHEMA"];
            DataColumn name = dt.Columns["TABLE_NAME"];

            foreach (DataRow row in dt.Rows)
            {
                DBSchemaItemRef iref = new DBSchemaItemRef();
                iref.Catalog = row[catalog].ToString();
                iref.Schema = row[schema].ToString();
                iref.Name = row[name].ToString();
                iref.Type = DBSchemaTypes.View;

                addtocollection.Add(iref);
                count++;
            }
            return count;
        }

        #endregion

        #region protected int LoadIndexRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)

        /// <summary>
        /// Loads all the indexes for a specific database connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="addtocollection"></param>
        /// <returns></returns>
        protected int LoadIndexRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)
        {
            int count = 0;
            DataTable dt = con.GetSchema(SchemaIndexesName);
            DataColumn catalog = dt.Columns["constraint_catalog"];
            DataColumn schema = dt.Columns["constraint_schema"];
            DataColumn name = dt.Columns["index_name"];

            foreach (DataRow row in dt.Rows)
            {
                DBSchemaItemRef iref = new DBSchemaItemRef();
                iref.Catalog = row[catalog].ToString();
                iref.Schema = row[schema].ToString();
                iref.Name = row[name].ToString();
                iref.Type = DBSchemaTypes.Index;

                addtocollection.Add(iref);
                count++;
            }
            return count;
        }

        #endregion

        #region protected int LoadSprocAndFunctionRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)

        /// <summary>
        /// Loads all the sprocs and functions for a specific database connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="addtocollection"></param>
        /// <returns></returns>
        protected int LoadSprocAndFunctionRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)
        {
            int count = 0;
            DataTable dt = con.GetSchema(SchemaProceduresName);
            DataColumn catalog = dt.Columns["ROUTINE_CATALOG"];
            DataColumn schema = dt.Columns["ROUTINE_SCHEMA"];
            DataColumn name = dt.Columns["ROUTINE_NAME"];
            DataColumn type = dt.Columns["ROUTINE_TYPE"];

            foreach (DataRow row in dt.Rows)
            {
                
                DBSchemaItemRef iref = new DBSchemaItemRef();
                iref.Catalog = row[catalog].ToString();
                iref.Schema = row[schema].ToString();
                iref.Name = row[name].ToString();
                string stype = row[type].ToString().ToUpper();
                if (stype == "FUNCTION")
                    iref.Type = DBSchemaTypes.Function;
                else if (stype == "PROCEDURE")
                    iref.Type = DBSchemaTypes.StoredProcedure;
                else
                    iref.Type = (DBSchemaTypes)0;

                addtocollection.Add(iref);
                count++;
            }
            return count;
        }


        #endregion

        #region protected int LoadSprocRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)
        
        /// <summary>
        /// Loads all the Stored Procedures for a specific database connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="addtocollection"></param>
        /// <returns></returns>
        protected int LoadSprocRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)
        {
            int count = 0;
            DataTable dt = con.GetSchema(SchemaProceduresName);
            DataColumn catalog = dt.Columns["ROUTINE_CATALOG"];
            DataColumn schema = dt.Columns["ROUTINE_SCHEMA"];
            DataColumn name = dt.Columns["ROUTINE_NAME"];
            DataColumn type = dt.Columns["ROUTINE_TYPE"];

            foreach (DataRow row in dt.Rows)
            {
                string stype = row[type].ToString().ToUpper();
                if (stype == "PROCEDURE")
                {
                    DBSchemaItemRef iref = new DBSchemaItemRef();
                    iref.Catalog = row[catalog].ToString();
                    iref.Schema = row[schema].ToString();
                    iref.Name = row[name].ToString();
                    iref.Type = DBSchemaTypes.StoredProcedure;

                    addtocollection.Add(iref);
                    count++;
                }
            }
            return count;
        }

        #endregion

        #region protected int LoadFunctionRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)
        
        /// <summary>
        /// Loads all the functions for a specific database connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="addtocollection"></param>
        /// <returns></returns>
        protected int LoadFunctionRefs(DbConnection con, List<DBSchemaItemRef> addtocollection)
        {
            int count = 0;
            DataTable dt = con.GetSchema(SchemaProceduresName);
            DataColumn catalog = dt.Columns["ROUTINE_CATALOG"];
            DataColumn schema = dt.Columns["ROUTINE_SCHEMA"];
            DataColumn name = dt.Columns["ROUTINE_NAME"];
            DataColumn type = dt.Columns["ROUTINE_TYPE"];

            foreach (DataRow row in dt.Rows)
            {
                string stype = row[type].ToString().ToUpper();

                if (stype == "FUNCTION")
                {
                    DBSchemaItemRef iref = new DBSchemaItemRef();
                    iref.Catalog = row[catalog].ToString();
                    iref.Schema = row[schema].ToString();
                    iref.Name = row[name].ToString();
                    iref.Type = DBSchemaTypes.Function;

                    addtocollection.Add(iref);
                    count++;
                }
            }
            return count;
        }

        #endregion


        //
        // load a view
        //

        #region private DBSchemaView LoadAView(DbConnection con, DBSchemaItemRef forRef)

        protected override DBSchemaView LoadAView(DbConnection con, DBSchemaItemRef forRef)
        {
            DBSchemaView view = null;
            string catalog = String.IsNullOrEmpty(forRef.Catalog) ? null : forRef.Catalog;
            string schema = String.IsNullOrEmpty(forRef.Schema) ? null : forRef.Schema;
            string name = forRef.Name;
            DataTable dtviews = con.GetSchema(SchemaViewsName, new string[] { catalog, schema, name });
            DataTable dbviewcols = con.GetSchema(SchemaViewColumnsName, new string[] { catalog, schema, name, null });

            if (dtviews.Rows.Count > 0)
            {
                DataRow dr = dtviews.Rows[0];
                view = CreateSchemaView(dr);

                using (DbCommand cmd = con.CreateCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT TOP 1 FROM [";
                    if (string.IsNullOrEmpty(view.Catalog) == false)
                        cmd.CommandText += view.Catalog + "].[";
                    if (string.IsNullOrEmpty(view.Schema) == false)
                        cmd.CommandText += view.Schema + "].[";
                    cmd.CommandText += view.Name;
                    cmd.CommandText += "]";

                    cmd.CommandType = CommandType.Text;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        DataTable dt = reader.GetSchemaTable();
                    }
                }
                FillViewColumns(view, dbviewcols);
            }

            return view;

        }

        #endregion

        #region private void FillViewColumns(DBSchemaView view, DataTable columns)

        private void FillViewColumns(DBSchemaView view, DataTable columns)
        {
            DataColumn colCatalog = columns.Columns["VIEW_CATALOG"];
            DataColumn colSchema = columns.Columns["VIEW_SCHEMA"];
            DataColumn colTable = columns.Columns["VIEW_NAME"];
            DataColumn colName = columns.Columns["COLUMN_NAME"];
            //DataColumn colType = columns.Columns["DATA_TYPE"];
            //DataColumn colDefault = columns.Columns["COLUMN_DEFAULT"];
            //DataColumn colMaxLen = columns.Columns["CHARACTER_MAXIMUM_LENGTH"];
            //DataColumn colExtra = columns.Columns["EXTRA"];
            //DataColumn colNull = columns.Columns["IS_NULLABLE"];
            //DataColumn colOrdinal = columns.Columns["ORDINAL_POSITION"];

            foreach (DataRow col in columns.Rows)
            {
                if (view.Catalog == col[colCatalog].ToString()
                    && view.Schema == col[colSchema].ToString()
                    && view.Name == col[colTable].ToString())
                {
                    DBSchemaViewColumn column = new DBSchemaViewColumn();
                    column.Name = col[colName].ToString();
                    column.ReadOnly = !view.IsUpdateable;

                    //string type = col[colType].ToString();
                    //string len = col[colMaxLen].ToString();
                    //string ord = col[colOrdinal].ToString();
                    //string nullable = col[colNull].ToString();
                    //string ctype = col["COLUMN_TYPE"].ToString();
                    //string ckey = col["COLUMN_KEY"].ToString();
                    //bool hasdef = col.IsNull(colDefault) == false;
                    //string defval;
                    //if (hasdef)
                    //    defval = col[colDefault].ToString();
                    //else
                    //    defval = string.Empty;

                    //column.DbType = GetDbTypeForSqlType(type);
                    //column.Nullable = (nullable.ToLower() == "yes");
                    //column.HasDefault = hasdef;
                    //column.DefaultValue = defval;
                    //int i;
                    //if (int.TryParse(ord, out i))
                    //    column.OrdinalPosition = i;
                    //else
                    //    column.OrdinalPosition = -1;

                    //column.Type = DBHelper.GetRuntimeTypeForDbType(column.DbType);

                    //if (string.IsNullOrEmpty(len) == false && int.TryParse(len, out i))
                    //    column.Size = i;
                    //else
                    //    column.Size = -1;

                    view.Columns.Add(column);
                }
            }
        }

        #endregion

        #region private DBSchemaView CreateSchemaView(DataRow dr)

        private DBSchemaView CreateSchemaView(DataRow dr)
        {
            DBSchemaView view = new DBSchemaView();
            view.Catalog = dr["TABLE_CATALOG"].ToString();
            view.Schema = dr["TABLE_SCHEMA"].ToString();
            view.Name = dr["TABLE_NAME"].ToString();

            string update = dr["IS_UPDATABLE"].ToString();
            view.IsUpdateable = false;
            if (string.IsNullOrEmpty(update) == false)
            {
                bool b;
                if (update.ToLower() == "yes")
                    view.IsUpdateable = true;
                else if (bool.TryParse(update, out b))
                    view.IsUpdateable = b;
            }

            return view;
        }

        #endregion


        //
        // DoCheckExists + support methods
        // 

        protected override bool DoCheckExists(DBSchemaItemRef itemRef)
        {
            throw new NotImplementedException();
        }

        //
        // DoGenerateCreateScript + support methods
        //

        protected override string DoGenerateCreateScript(DBSchemaTypes type, DBSchemaItem schema, DBQuery script, DBStatementBuilder builder)
        {
            throw new NotImplementedException();
        }

        //
        // DoGenerateDropScript + support methods
        //

        protected override string DoGenerateDropScript(DBSchemaItemRef itemRef, DBStatementBuilder builder)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Gets the correct DbType for a MySql column type.
        /// </summary>
        #region protected override DbType GetDbTypeForSqlType(string type)

        /// <param name="type"></param>
        /// <returns></returns>
        protected override DbType GetDbTypeForSqlType(string type)
        {
            DbType dbtype = DbType.Object;
            if (string.IsNullOrEmpty(type))
                dbtype = DbType.Object;
            else
            {
                switch (type.ToLower())
                {
                    case ("tinyint"):
                        dbtype = DbType.SByte;
                        break;
                    case ("smallint"):
                        dbtype = DbType.Int16;
                        break;
                    case ("int"):
                        dbtype = DbType.Int32;
                        break;
                    case ("bigint"):
                        dbtype = DbType.Int64;
                        break;

                    case ("tinyint unsigned"):
                        dbtype = DbType.Byte;
                        break;
                    case ("smallint unsigned"):
                        dbtype = DbType.UInt16;
                        break;
                    case ("mediumint unsigned"):
                    case ("int unsigned"):
                        dbtype = DbType.UInt32;
                        break;
                    case ("bigint unsigned"):
                        dbtype = DbType.UInt64;
                        break;
                    case ("bit"):
                        dbtype = DbType.Boolean;
                        break;
                    case ("float"):
                        dbtype = DbType.Single;
                        break;
                    case ("double"):
                        dbtype = DbType.Double;
                        break;
                    case ("decimal"):
                    case ("numeric"):
                        dbtype = DbType.Decimal;
                        break;
                    case ("date"):
                        dbtype = DbType.Date;
                        break;
                    case ("datetime"):
                        dbtype = DbType.DateTime;
                        break;
                    case ("time"):
                        dbtype = DbType.Time;
                        break;
                    case ("timestamp"):
                        dbtype = DbType.DateTimeOffset;
                        break;
                    case ("year"):
                        dbtype = DbType.UInt16;
                        break;
                    case ("char"):
                        dbtype = DbType.AnsiStringFixedLength;
                        break;
                    case ("varchar"):
                    case ("text"):
                    case ("enum"):
                    case ("set"):
                        dbtype = DbType.AnsiString;
                        break;
                    case("nchar"):
                        dbtype = DbType.StringFixedLength;
                        break;
                    case("nvarchar"):
                        dbtype = DbType.String;
                        break;                        
                    case ("binary"):
                    case ("varbinary"):
                    case ("blob"):
                        dbtype = DbType.Binary;
                        break;
                    default:
                        dbtype = DbType.Object;
                        break;
                }
            }
            return dbtype;
        }

        #endregion
    }
}
