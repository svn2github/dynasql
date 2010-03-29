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
using Perceiveit.Data.Query;
using Perceiveit.Data.Schema;

namespace Perceiveit.Data.MySqlClient
{
    internal class DBMySqlSchemaProvider : DBSchemaProvider
    {

        public DBMySqlSchemaProvider(DBDatabase database, DBDatabaseProperties properties)
            : base(database, properties)
        {
        }
        
        //
        // DoGetSchemaItems + support methods
        //

        #region protected override IEnumerable<DBSchemaItemRef> DoGetSchemaItems(DBSchemaTypes types)

        /// <summary>
        /// Gets all the SchemaItem references for the specified types
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
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
            DataColumn catalog = dt.Columns["INDEX_CATALOG"];
            DataColumn schema = dt.Columns["INDEX_SCHEMA"];
            DataColumn name = dt.Columns["INDEX_NAME"];
            DataColumn tbl = dt.Columns["TABLE_NAME"];

            foreach (DataRow row in dt.Rows)
            {
                DBSchemaItemRef iref = new DBSchemaItemRef();
                iref.Catalog = row[catalog].ToString();
                iref.Schema = row[schema].ToString();
                iref.Name = row[name].ToString();
                iref.Type = DBSchemaTypes.Index;
                DBSchemaItemRef tblref = new DBSchemaItemRef(DBSchemaTypes.Table, iref.Catalog, iref.Schema, row[tbl].ToString());
                iref.Container = tblref;
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
        // DoCheckExists
        //

        #region protected override bool DoCheckExists(DBSchemaItemRef itemRef, DBSchemaItemRef container)

        protected override bool DoCheckExists(DBSchemaItemRef itemRef)
        {
            string catalog = null;
            if (!string.IsNullOrEmpty(itemRef.Catalog))
                catalog = itemRef.Catalog;

            string schema = null;
            if (!string.IsNullOrEmpty(itemRef.Schema))
                schema = itemRef.Schema;

            string name = itemRef.Name;
            DataTable dt = null;

            using (DbConnection con = this.Database.CreateConnection())
            {
                con.Open();
                switch (itemRef.Type)
                {
                    case DBSchemaTypes.Table:
                        dt = con.GetSchema(SchemaTablesName, new String[] { catalog, schema, name, "BASE TABLE" });
                        break;
                    case DBSchemaTypes.View:
                        dt = con.GetSchema(SchemaViewsName, new string[] { catalog, schema, name });
                        break;
                    case DBSchemaTypes.StoredProcedure:
                        dt = con.GetSchema(SchemaProceduresName, new string[] { catalog, schema, name, "PROCEDURE" });
                        break;
                    case DBSchemaTypes.Function:
                        dt = con.GetSchema(SchemaProceduresName, new string[] { catalog, schema, name, "FUNCTION" });
                        break;
                    case DBSchemaTypes.Index:
                        if (null != itemRef.Container && !string.IsNullOrEmpty(itemRef.Container.Name) && itemRef.Container.Type == DBSchemaTypes.Table)
                        {
                            dt = con.GetSchema(SchemaIndexesName, new string[] { catalog, schema, itemRef.Container.Name, name });
                        }
                        else
                            dt = con.GetSchema(SchemaIndexesName, new string[] { catalog, schema, null, name});
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("itemRef.Type");

                }
            }

            return dt.Rows.Count > 0;

        }

        #endregion


       
        
        //
        // Load a view + support methods
        //

        #region private DBSchemaItem LoadAView(DbConnection con, DBSchemaItemRef forRef)

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
            DataColumn colType = columns.Columns["DATA_TYPE"];
            DataColumn colDefault = columns.Columns["COLUMN_DEFAULT"];
            DataColumn colMaxLen = columns.Columns["CHARACTER_MAXIMUM_LENGTH"];
            DataColumn colExtra = columns.Columns["EXTRA"];
            DataColumn colNull = columns.Columns["IS_NULLABLE"];
            DataColumn colOrdinal = columns.Columns["ORDINAL_POSITION"];

            foreach (DataRow col in columns.Rows)
            {
                if (view.Catalog == col[colCatalog].ToString()
                    && view.Schema == col[colSchema].ToString()
                    && view.Name == col[colTable].ToString())
                {
                    DBSchemaViewColumn column = new DBSchemaViewColumn();
                    column.Name = col[colName].ToString();
                    string type = col[colType].ToString();
                    string len = col[colMaxLen].ToString();
                    string ord = col[colOrdinal].ToString();
                    string nullable = col[colNull].ToString();
                    string ctype = col["COLUMN_TYPE"].ToString();
                    string ckey = col["COLUMN_KEY"].ToString();
                    bool hasdef = col.IsNull(colDefault) ==  false;
                    string defval;
                    if (hasdef)
                        defval = col[colDefault].ToString();
                    else
                        defval = string.Empty;

                    column.DbType = GetDbTypeForSqlType(type);
                    column.Nullable = (nullable.ToLower() == "yes");
                    column.HasDefault = hasdef;
                    column.DefaultValue = defval;
                    int i;
                    if (int.TryParse(ord, out i))
                        column.OrdinalPosition = i;
                    else
                        column.OrdinalPosition = -1;

                    column.ReadOnly = !view.IsUpdateable;
                    column.Type = DBHelper.GetRuntimeTypeForDbType(column.DbType);

                    if (string.IsNullOrEmpty(len) == false && int.TryParse(len, out i))
                        column.Size = i;
                    else
                        column.Size = -1;

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
            if (string.IsNullOrEmpty(update) == false && update.ToLower() == "yes")
                view.IsUpdateable = true;
            else
                view.IsUpdateable = false;

            return view;
        }

        #endregion

        //
        // Load an index + support methods
        //

        #region private DBSchemaItem LoadAnIndex(DbConnection con, DBSchemaItemRef forRef)

        private DBSchemaItem LoadAnIndex(DbConnection con, DBSchemaItemRef forRef)
        {
            DBSchemaIndex ix = null;
            string catalog = String.IsNullOrEmpty(forRef.Catalog) ? null : forRef.Catalog;
            string schema = String.IsNullOrEmpty(forRef.Schema) ? null : forRef.Schema;
             
            string name = forRef.Name;
            DataTable dt = con.GetSchema(SchemaIndexesName, new string[] { catalog, schema, null, name });
            DataTable dbcols = con.GetSchema(SchemaIndexColumnsName, new string[] { catalog, schema, null, name, null });

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                ix = CreateSchemaIndex(dr);

                FillIndexColumns(ix, dbcols);
            }

            return ix;
        }

        #endregion

        #region private DBSchemaIndex CreateSchemaIndex(DataRow row)

        private DBSchemaIndex CreateSchemaIndex(DataRow row)
        {
            DBSchemaIndex ix = new DBSchemaIndex();
            ix.Catalog = row["INDEX_CATALOG"].ToString();
            ix.Schema = row["INDEX_SCHEMA"].ToString();
            ix.Name = row["INDEX_NAME"].ToString();
            string s = row["TABLE_NAME"].ToString();
            ix.TableReference = new DBSchemaItemRef(DBSchemaTypes.Table, ix.Catalog, ix.Schema, s);


            s = row["PRIMARY"].ToString();
            if (string.IsNullOrEmpty(s) == false && s.ToUpper() == "YES")
                ix.IsPrimaryKey = true;
            else
                ix.IsPrimaryKey = false;

            s = row["UNIQUE"].ToString();
            if (string.IsNullOrEmpty(s) == false && s.ToUpper() == "YES")
                ix.IsUnique = true;
            else
                ix.IsUnique = false;

            return ix;
        }

        #endregion

        #region private void FillIndexColumns(DBSchemaIndex ix, DataTable ixcols)

        private void FillIndexColumns(DBSchemaIndex ix, DataTable ixcols)
        {
            DataColumn ixcatalog = ixcols.Columns["INDEX_CATALOG"];
            DataColumn ixschema = ixcols.Columns["INDEX_SCHEMA"];
            DataColumn ixname = ixcols.Columns["INDEX_NAME"];
            DataColumn ixcolName = ixcols.Columns["COLUMN_NAME"];
            DataColumn ixordinal = ixcols.Columns["ORDINAL_POSITION"];
            DataColumn ixorder = ixcols.Columns["SORT_ORDER"];

            foreach (DataRow ixColumnRow in ixcols.Rows)
            {
                if (string.Equals(ix.Catalog, ixColumnRow[ixcatalog].ToString()) == false)
                    continue;
                if (string.Equals(ix.Schema, ixColumnRow[ixschema].ToString()) == false)
                    continue;
                if (string.Equals(ix.Name, ixColumnRow[ixname].ToString()) == false)
                    continue;

                DBSchemaIndexColumn ixcolumn = new DBSchemaIndexColumn();
                ixcolumn.Name = ixColumnRow[ixcolName].ToString();
                ixcolumn.SortOrder = (ixColumnRow[ixorder].ToString().ToUpper() == "A") ? Order.Ascending : Order.Descending;

                int i;
                if (int.TryParse(ixColumnRow[ixordinal].ToString(), out i))
                    ixcolumn.OrdinalPosition = i;
                else
                    ixcolumn.OrdinalPosition = -1;

                ix.Columns.Add(ixcolumn);

            }

        }

        #endregion

        //
        // LoadASproc + support methods
        //

        #region private DBSchemaItem LoadASproc(DbConnection con, DBSchemaItemRef forRef, string sprocType)


        private DBSchemaItem LoadASproc(DbConnection con, DBSchemaItemRef forRef, string sprocType)
        {
            DBSchemaSproc sproc = null;
            string catalog = String.IsNullOrEmpty(forRef.Catalog) ? null : forRef.Catalog;
            string schema = String.IsNullOrEmpty(forRef.Schema) ? null : forRef.Schema;

            string name = forRef.Name;
            DataTable dt = con.GetSchema(SchemaProceduresName, new string[] { catalog, schema, name, sprocType });
            DataTable dbcols = con.GetSchema("Procedure Parameters", new string[] { catalog, schema, name, sprocType });

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                sproc = CreateSchemaProcedure(dr, sprocType);

                FillProcedureParameters(sproc, dbcols, sprocType == "FUNCTION");
            }

            return sproc;
        }

        #endregion

        #region private DBSchemaSproc CreateSchemaProcedure(DataRow dr, string sproctype)

        private DBSchemaSproc CreateSchemaProcedure(DataRow dr, string sproctype)
        {
            DBSchemaSproc sproc;

            if (sproctype == "PROCEDURE")
                sproc = new DBSchemaSproc();
            else
            {
                sproc = new DBSchemaFunction();
                string dtd = dr["DTD_IDENTIFIER"].ToString();
                if (string.IsNullOrEmpty(dtd) == false)
                {
                    DbType returnType = GetDbTypeForMySqlDtd(dtd);
                    ((DBSchemaFunction)sproc).ReturnDbType = returnType;
                    ((DBSchemaFunction)sproc).ReturnRuntimeType = DBHelper.GetRuntimeTypeForDbType(returnType);
                }
            }
            sproc.Catalog = dr["ROUTINE_CATALOG"].ToString();
            sproc.Schema = dr["ROUTINE_SCHEMA"].ToString();
            sproc.Name = dr["ROUTINE_NAME"].ToString();

            return sproc;
        }

        #endregion

        #region private void FillProcedureParameters(DBSchemaSproc sproc, DataTable spparams, bool isFunction)

        private void FillProcedureParameters(DBSchemaSproc sproc, DataTable spparams, bool isFunction)
        {
            DataColumn spname = spparams.Columns["PARAMETER_NAME"];
            DataColumn sptype = spparams.Columns["DATA_TYPE"];
            DataColumn splen = spparams.Columns["CHARACTER_MAXIMUM_LENGTH"];
            DataColumn sppos = spparams.Columns["ORDINAL_POSITION"];
            DataColumn spmode = spparams.Columns["PARAMETER_MODE"];

            foreach (DataRow row in spparams.Rows)
            {
                string name = row[spname].ToString();
                string type = row[sptype].ToString();
                string len = row[splen].ToString();
                string pos = row[sppos].ToString();
                string mode = row[spmode].ToString();


                DbType dbtype = GetDbTypeForSqlType(type);
                Type systype = DBHelper.GetRuntimeTypeForDbType(dbtype);



                DBSchemaParameter param = new DBSchemaParameter();
                param.Name = name;
                param.DbType = dbtype;

                ParameterDirection dir = ParameterDirection.Input;
                if (name == "RETURN_VALUE")
                {
                    dir = ParameterDirection.ReturnValue;
                }
                else if (mode == "OUT")
                    dir = ParameterDirection.Output;
                else if (mode == "INOUT")
                    dir = ParameterDirection.InputOutput;

                param.Direction = dir;
                int i;
                if (int.TryParse(pos, out i))
                    param.ParameterIndex = i;
                else
                    param.ParameterIndex = -1;

                if (int.TryParse(len, out i))
                    param.ParameterSize = i;
                else
                    param.ParameterSize = -1;

                sproc.Parameters.Add(param);

            }
        }

        #endregion

        //
        // DoGenerateCreateScript
        //

        #region protected override string DoGenerateCreateScript(DBSchemaTypes type, DBSchemaItem schema, DBQuery script, DBStatementBuilder builder)

        protected override string DoGenerateCreateScript(DBSchemaTypes type, DBSchemaItem schema, DBQuery script, DBStatementBuilder builder)
        {
            string sql;
            switch (type)
            {
                case DBSchemaTypes.Table:
                    builder.GenerateCreateTableScript((DBSchemaTable)schema);
                    break;
                case DBSchemaTypes.View:
                    builder.GenerateCreateViewScript((DBSchemaView)schema, script);
                    break;
                case DBSchemaTypes.StoredProcedure:
                    builder.GenerateCreateProcedureScript((DBSchemaSproc)schema,(DBScript)script);
                    break;
                case DBSchemaTypes.Function:
                    builder.GenerateCreateFunctionScript((DBSchemaFunction)schema, (DBScript) script);
                    break;
                case DBSchemaTypes.Index:
                    builder.GenerateCreateIndexScript((DBSchemaIndex)schema);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("type");
                    break;
            }
            MySqlClient.DBMySqlStatementBuilder mysqlbuilder = (MySqlClient.DBMySqlStatementBuilder)builder;

            sql = builder.ToString();

            return sql;
        }

        #endregion

        //
        // DoGenerateDropScript
        //

        #region protected override string DoGenerateDropScript(DBSchemaItemRef itemRef, DBStatementBuilder builder)

        protected override string DoGenerateDropScript(DBSchemaItemRef itemRef, DBStatementBuilder builder)
        {
            string sql;
            switch (itemRef.Type)
            {
                case DBSchemaTypes.Table:
                    builder.GenerateDropTableScript(itemRef);
                    break;
                case DBSchemaTypes.View:
                    builder.GenerateDropViewScript(itemRef);
                    break;
                case DBSchemaTypes.StoredProcedure:
                    builder.GenerateDropProcedureScript(itemRef);
                    break;
                case DBSchemaTypes.Function:
                    builder.GenerateDropFunctionScript(itemRef);
                    break;
                case DBSchemaTypes.Index:
                    builder.GenerateDropIndexScript(itemRef);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("itemRef.Type");
                    break;
            }
            sql = builder.ToString();

            return sql;
        }

        #endregion

        //
        // support methods
        //

        /// <summary>
        /// Gets the correct DbType for the MySql DTD_IDENTIFIER type e.g 'int(4) UNSIGNED'
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        #region private DbType GetDbTypeForMySqlDtd(string type)

        private DbType GetDbTypeForMySqlDtd(string type)
        {
            if (type.IndexOf(' ') > -1)
            {
                type = type.Split(' ')[0];
            }
            if (type.IndexOf('(') > -1)
            {
                type = type.Split('(')[0];
            }
            return GetDbTypeForSqlType(type);
        }

        #endregion


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
                    case ("mediumint"):
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
                        dbtype = DbType.StringFixedLength;
                        break;
                    case ("varchar"):
                    case ("text"):
                    case ("enum"):
                    case ("set"):
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
