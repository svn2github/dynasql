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

namespace Perceiveit.Data.OleDb
{
    /// <summary>
    /// OLEDB Schema provider specifically for MS Access database
    /// </summary>
    public class DBMSAccessSchemaProvider : DBSchemaProvider
    {
        #region internal DBMSAccessTypeMappingCollection Types {get;}

        private DBMSAccessTypeMappingCollection _types;

        internal DBMSAccessTypeMappingCollection Types
        {
            get
            {
                if (null == _types)
                {
                    DBSchemaMetaDataCollection col = this.AssertGetCollectionForName("DataTypes");

                    DataTable dt = this.GetCollectionData(col);
                    _types = DBMSAccessTypeMapping.LoadFromTable(dt);
                }
                return _types;
            }
        }

        #endregion

        /// <summary>
        /// Creates a new instance of the DBMSAccessSchemaProvider
        /// </summary>
        /// <param name="database"></param>
        /// <param name="properties"></param>
        public DBMSAccessSchemaProvider(DBDatabase database, DBDatabaseProperties properties)
            : base(database, properties)
        {
        }


        //
        // Table overrides
        //

        #region protected override DBSchemaTable LoadATable(DbConnection con, DBSchemaItemRef tableref)
        /// <summary>
        /// Loads the info on a specific table
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tableref"></param>
        /// <returns></returns>
        protected override DBSchemaTable LoadATable(DbConnection con, DBSchemaItemRef tableref)
        {
            DataTable dtTable = GetTableData(con, tableref);

            DBSchemaTable atable = null;

            if (null != dtTable && dtTable.Rows.Count > 0)
            {
                atable = new DBSchemaTable();
                this.FillTableData(atable, dtTable.Rows[0]);

                DataTable dtColumns = this.GetTableColumns(con, tableref);
                if (null != dtColumns)
                    this.FillTableColumns(atable.Columns, dtColumns);

                DBSchemaItemRefCollection idxs = new DBSchemaItemRefCollection();
                DBSchemaItemRefCollection fks = new DBSchemaItemRefCollection();

                this.LoadForeignKeyRefs(con, fks, tableref);
                this.LoadIndexRefs(con, idxs, tableref);

                DBSchemaIndexCollection indexes = new DBSchemaIndexCollection();
                foreach (DBSchemaItemRef idx in idxs)
                {
                    DBSchemaIndex same;
                    DBSchemaIndex anindex = this.LoadAnIndex(con, idx);
                    if (indexes.TryGetIndex(idx, out same))
                        same.Columns.AddRange(anindex.Columns);
                    else
                        indexes.Add(anindex);
                }
                foreach (DBSchemaIndex idx in indexes)
                {
                    if (idx.IsPrimaryKey)
                    {
                        foreach (DBSchemaIndexColumn idxcol in idx.Columns)
                        {
                            DBSchemaTableColumn tblcol;
                            if (atable.Columns.TryGetColumn(idxcol.ColumnName, out tblcol))
                                tblcol.PrimaryKey = true;
                        }
                    }
                }
                DBSchemaForeignKeyCollection foreignkeys = new DBSchemaForeignKeyCollection();
                foreach (DBSchemaItemRef fk in fks)
                {
                    DBSchemaForeignKey aForeignKey = this.LoadAForeignKey(con, fk);
                    foreignkeys.Add(aForeignKey);
                }

                atable.ForeignKeys = foreignkeys;
                atable.Indexes = indexes;
            }
            return atable;
        }

        #endregion

        #region protected override void LoadTableRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        /// <summary>
        /// Loads all the table refs into the collection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected override void LoadTableRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection tblcollection = this.AssertGetCollectionForType(DBMetaDataCollectionType.Tables);
            DataTable data = this.GetCollectionData(con, tblcollection);

            DataColumn catalogcol = GetColumn(data, "TABLE_CATALOG", false);
            DataColumn schemacol = GetColumn(data, "TABLE_SCHEMA", false);
            DataColumn namecol = GetColumn(data, "TABLE_NAME", true);
            DataColumn typecol = GetColumn(data, "TABLE_TYPE", true);

            foreach (DataRow row in data.Rows)
            {
                string type = GetColumnStringValue(row, typecol);
                if (string.Equals("TABLE", type, StringComparison.OrdinalIgnoreCase))
                {
                    DBSchemaItemRef tblref = this.LoadAnItemRef(row, catalogcol, schemacol, namecol, DBSchemaTypes.Table);
                    if (null != tblref)
                        intoCollection.Add(tblref);
                }
            }
        }

        #endregion

        #region protected override DataTable GetTableData(DbConnection con, DBSchemaItemRef tableref)
        /// <summary>
        /// Gets the Meta data DataTable about the specified table
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tableref"></param>
        /// <returns></returns>
        protected override DataTable GetTableData(DbConnection con, DBSchemaItemRef tableref)
        {
            System.Data.OleDb.OleDbConnection olecon = (System.Data.OleDb.OleDbConnection)con;
            DataTable dt = olecon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, new object[] { null, null, tableref.Name });

            dt.TableName = "Tables";
            WriteCollectionData("Tables", new string[] { tableref.Catalog, tableref.Schema, tableref.Name, null }, dt);

            return dt;
        }

        #endregion

        #region protected override void FillTableData(DBSchemaTable table, DataRow row)
        /// <summary>
        /// Fills the table parameter with the information from the row
        /// </summary>
        /// <param name="table"></param>
        /// <param name="row"></param>
        protected override void FillTableData(DBSchemaTable table, DataRow row)
        {
            DataTable tbl = row.Table;
            DataColumn name = GetColumn(tbl, "TABLE_NAME", true);
            table.Name = GetColumnStringValue(row, name);
        }

        #endregion

        #region protected override DataTable GetTableColumns(DbConnection con, DBSchemaItemRef tableref)
        /// <summary>
        /// Gets the MetaData table column information
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tableref"></param>
        /// <returns></returns>
        protected override DataTable GetTableColumns(DbConnection con, DBSchemaItemRef tableref)
        {
            System.Data.OleDb.OleDbConnection olecon = (System.Data.OleDb.OleDbConnection)con;
            DataTable dt = olecon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Columns, new object[] { null, null, tableref.Name });
            dt.TableName = "Table Columns";
            WriteCollectionData("Table Columns", new string[] { tableref.Catalog, tableref.Schema, tableref.Name, null }, dt);

            return dt;
        }

        #endregion

        #region protected override void FillTableColumns(DBSchemaTableColumnCollection atablecolumns, DataTable dtColumns)
        /// <summary>
        /// Fills the information in atablecolumns with the data from dtColumns
        /// </summary>
        /// <param name="atablecolumns"></param>
        /// <param name="dtColumns"></param>
        protected override void FillTableColumns(DBSchemaTableColumnCollection atablecolumns, DataTable dtColumns)
        {
            DataColumn TableNameColumn = GetColumn(dtColumns, "TABLE_NAME", true);
            DataColumn TableSchemaColumn = GetColumn(dtColumns, "TABLE_SCHEMA", false);
            DataColumn TableCatalogColumn = GetColumn(dtColumns, "TABLE_CATALOG", false);
            DataColumn ColumnNameColumn = GetColumn(dtColumns, "COLUMN_NAME", true);
            DataColumn OrdinalPositionColumn = GetColumn(dtColumns, "ORDINAL_POSITION", true);
            DataColumn DefaultValueColumn = GetColumn(dtColumns, "COLUMN_DEFAULT", false);
            DataColumn IsNullableColumn = GetColumn(dtColumns, "IS_NULLABLE", true);
            DataColumn DataTypeColumn = GetColumn(dtColumns, "DATA_TYPE", true);
            DataColumn MaxCharacterLengthColumn = GetColumn(dtColumns, "CHARACTER_MAXIMUM_LENGTH", false);
            DataColumn AutoNumberColumn = GetColumn(dtColumns, "AUTOINCREMENT", false);
            DataColumn PrimaryKeyColumn = GetColumn(dtColumns, "PRIMARY_KEY", false);

            foreach (DataRow row in dtColumns.Rows)
            {
                DBSchemaTableColumn col = new DBSchemaTableColumn();
                col.Name = GetColumnStringValue(row, ColumnNameColumn);
                col.OrdinalPosition = GetColumnIntValue(row, OrdinalPositionColumn);
                col.DefaultValue = GetColumnStringValue(row, DefaultValueColumn);
                col.Nullable = GetColumnBoolValue(row, IsNullableColumn);
                int type = GetColumnIntValue(row, DataTypeColumn, -1);
                DBMSAccessTypeMapping map;

                if (type > 0 && this.Types.TryGetType(type,out map))
                {
                    col.DbType = GetDbTypeForNativeType(map.TypeName);
                    col.Type = map.RuntimeType;
                    if (col.DbType == DbType.Object && null != map.RuntimeType)
                        col.DbType = DBHelper.GetDBTypeForRuntimeType(map.RuntimeType);
                }
                col.Size = GetColumnIntValue(row, MaxCharacterLengthColumn);
                col.AutoAssign = GetColumnBoolValue(row, AutoNumberColumn);
                col.PrimaryKey = GetColumnBoolValue(row, PrimaryKeyColumn);
                col.ReadOnly = col.AutoAssign;
                col.HasDefault = !string.IsNullOrEmpty(col.DefaultValue);

                atablecolumns.Add(col);
            }
        }

        #endregion


        //
        // Indexes
        //

        #region protected virtual void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection) + 1 overload

        /// <summary>
        /// Loads all the indexes in this providers data connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected override void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection viewcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.Indexes);
            DataTable data = this.GetCollectionData(con, viewcollection);

            DataColumn catalogcol = GetColumn(data, "index_catalog", false);
            DataColumn schemacol = GetColumn(data, "index_schema", false);
            DataColumn namecol = GetColumn(data, "index_name", true);

            DataColumn tablecatalogcol = GetColumn(data, "table_catalog", false);
            DataColumn tableschemacol = GetColumn(data, "table_schema", false);
            DataColumn tablenamecol = GetColumn(data, "table_name", true);
            
            //we need to shring this to take account of the fact that multiple entries are returned
            //for a single index on multiple columns
            List<DBSchemaItemRef> all = new List<DBSchemaItemRef>();
            this.LoadItemRefsWithContainer(data, all,
                    catalogcol, schemacol, namecol, DBSchemaTypes.Index,
                    tablecatalogcol, tableschemacol, tablenamecol, DBSchemaTypes.Table);
            Dictionary<string, DBSchemaItemRef> unique = new Dictionary<string, DBSchemaItemRef>();
            foreach (DBSchemaItemRef idx in all)
            {
                string key = idx.ToString();
                if (!unique.ContainsKey(key))
                    unique.Add(key, idx);

            }
            foreach (DBSchemaItemRef iref in unique.Values)
            {
                intoCollection.Add(iref);
            }
        }

        #endregion

        #region protected override void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)
        /// <summary>
        /// Loads any index information for the requested table
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        /// <param name="fortable"></param>
        protected override void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)
        {
            System.Data.OleDb.OleDbConnection olecon = (System.Data.OleDb.OleDbConnection)con;
            DataTable data = olecon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Indexes, new object[] { null, null, null, null, fortable.Name });
            
            //DBSchemaMetaDataCollection viewcollection =
            //    this.AssertGetCollectionForType(DBMetaDataCollectionType.Indexes);
            //DataTable data = this.GetCollectionData(con, viewcollection, null, null, null, fortable.Name, null);

            DataColumn catalogcol = GetColumn(data, "index_catalog", false);
            DataColumn schemacol = GetColumn(data, "index_schema", false);
            DataColumn namecol = GetColumn(data, "index_name", true);

            DataColumn tablecatalogcol = GetColumn(data, "table_catalog", false);
            DataColumn tableschemacol = GetColumn(data, "table_schema", false);
            DataColumn tablenamecol = GetColumn(data, "table_name", true);

            //we need to shring this to take account of the fact that multiple entries are returned
            //for a single index on multiple columns
            List<DBSchemaItemRef> all = new List<DBSchemaItemRef>();
            this.LoadItemRefsWithContainer(data, all,
                    catalogcol, schemacol, namecol, DBSchemaTypes.Index,
                    tablecatalogcol, tableschemacol, tablenamecol, DBSchemaTypes.Table);
            Dictionary<string, DBSchemaItemRef> unique = new Dictionary<string, DBSchemaItemRef>();
            foreach (DBSchemaItemRef idx in all)
            {
                string key = idx.ToString();
                if (!unique.ContainsKey(key))
                    unique.Add(key, idx);

            }
            foreach (DBSchemaItemRef iref in unique.Values)
            {
                intoCollection.Add(iref);
            }
        }

        #endregion

        #region protected override DBSchemaIndex LoadAnIndex(DbConnection con, DBSchemaItemRef idxref)
        /// <summary>
        /// Loads specific information about an index
        /// </summary>
        /// <param name="con"></param>
        /// <param name="idxref"></param>
        /// <returns></returns>
        protected override DBSchemaIndex LoadAnIndex(DbConnection con, DBSchemaItemRef idxref)
        {
            DataTable dtIdxs = this.GetIndexData(con, idxref);
            DBSchemaIndex anindex = null;

            if (null != dtIdxs && dtIdxs.Rows.Count > 0)
            {
                anindex = new DBSchemaIndex();
                this.FillIndexData(anindex, dtIdxs.Rows[0]);
                if (dtIdxs.Rows.Count > 1)
                {
                    DataColumn col = GetColumn(dtIdxs, "COLUMN_NAME", true);
                    //DataColumn tbl = GetColumn(dtIdxs, "TABLE_NAME", false);

                    for (int i = 1; i < dtIdxs.Rows.Count; i++)
                    {
                        string column = GetColumnStringValue(dtIdxs.Rows[i], col);
                        if (string.IsNullOrEmpty(column) == false)
                            anindex.Columns.Add(new DBSchemaIndexColumn(column));
                    }
                }

                //DataTable dtIdxColumns = this.GetIndexColumns(con, idxref);
                //if (null != dtIdxColumns)
                //    this.FillIndexColuns(anindex, dtIdxColumns);
            }
            return anindex;
        }

        #endregion

        #region protected override DataTable GetIndexData(DbConnection con, DBSchemaItemRef idxref)
        /// <summary>
        /// Gets the meta data DataTable for the specified index
        /// </summary>
        /// <param name="con"></param>
        /// <param name="idxref"></param>
        /// <returns></returns>
        protected override DataTable GetIndexData(DbConnection con, DBSchemaItemRef idxref)
        {
            System.Data.OleDb.OleDbConnection olecon = (System.Data.OleDb.OleDbConnection)con;
            string table = null;
            if (null != idxref.Container)
                table = idxref.Container.Name;

            DataTable data = olecon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Indexes, new object[] {null, null, idxref.Name, null, table});

            data.TableName = "Indexes";
            WriteCollectionData("Indexes", new string[] { idxref.Name }, data);

            return data;
           
        }

        #endregion

        #region protected override void FillIndexData(DBSchemaIndex anindex, DataRow dataRow)
        /// <summary>
        /// Populates the anindex from the meta data dataRow
        /// </summary>
        /// <param name="anindex"></param>
        /// <param name="dataRow"></param>
        protected override void FillIndexData(DBSchemaIndex anindex, DataRow dataRow)
        {
            DataTable dt = dataRow.Table;
            DataColumn name = GetColumn(dt, "INDEX_NAME", true);
            DataColumn pk = GetColumn(dt, "PRIMARY_KEY", false);
            DataColumn unique = GetColumn(dt, "UNIQUE", false);
            DataColumn col = GetColumn(dt, "COLUMN_NAME", true);
            DataColumn tbl = GetColumn(dt, "TABLE_NAME", false);

            anindex.Name = GetColumnStringValue(dataRow, name);
            anindex.IsPrimaryKey = GetColumnBoolValue(dataRow, pk);
            anindex.IsUnique = GetColumnBoolValue(dataRow, unique);
            
            string table = GetColumnStringValue(dataRow, tbl);
            if (string.IsNullOrEmpty(table) == false)
                anindex.TableReference = new DBSchemaItemRef(DBSchemaTypes.Table, table);

            string column = GetColumnStringValue(dataRow, col);
            if(string.IsNullOrEmpty(column) == false)
                anindex.Columns.Add(new DBSchemaIndexColumn(column));
        }

        #endregion

        #region protected override DataTable GetIndexColumns(DbConnection con, DBSchemaItemRef idxref)
        /// <summary>
        /// Ignored
        /// </summary>
        /// <param name="con"></param>
        /// <param name="idxref"></param>
        /// <returns></returns>
        protected override DataTable GetIndexColumns(DbConnection con, DBSchemaItemRef idxref)
        {
            //this is done on the FillIndexData method.
            return null;
        }

        #endregion

        #region protected override void FillIndexColuns(DBSchemaIndex anindex, DataTable dtIdxColumns)
        /// <summary>
        /// Ignored
        /// </summary>
        /// <param name="anindex"></param>
        /// <param name="dtIdxColumns"></param>
        protected override void FillIndexColuns(DBSchemaIndex anindex, DataTable dtIdxColumns)
        {
           //Do nothing - this is done on the FillIndexData method
        }

        #endregion


        //
        // Routines
        // The MSAccess support for procedures is limited and inconsistent - 
        // views are procedures and procedures are views and no inspection of the 
        // parameters are supported however - this is how it is reported!
        //

        #region protected virtual void LoadRoutineRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)

        /// <summary>
        /// Loads all the routines (Procedures and fuctions) in this providers data connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected override void LoadRoutineRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection viewcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.Procedures);
            DataTable data = this.GetCollectionData(con, viewcollection);

            WriteCollectionData("Procedures", new string[] { }, data);

            DataColumn catalogcol = GetColumn(data, "PROCEDURE_CATALOG", false);
            DataColumn schemacol = GetColumn(data, "PROCEDURE_SCHEMA", false);
            DataColumn namecol = GetColumn(data, "PROCEDURE_NAME", true);
            DataColumn typecol = GetColumn(data, "PROCEDURE_TYPE", true);

            foreach (DataRow row in data.Rows)
            {
                DBSchemaItemRef iref;
                string type = GetColumnStringValue(row, typecol);
                string name = GetColumnStringValue(row, namecol);
                
                //system procedures start with a tilde - remove these.
                if (string.IsNullOrEmpty(name) || name.StartsWith("~"))
                    row.Delete();
                else
                {
                    iref = this.LoadAnItemRef(row, catalogcol, schemacol, namecol, DBSchemaTypes.StoredProcedure);
                    if (null != iref)
                        intoCollection.Add(iref);
                }
            }
        }

        #endregion

        #region protected override DataTable GetRoutineData(DbConnection con, DBSchemaItemRef routineref)
        /// <summary>
        /// Gets the meta data DataTable for a routine
        /// </summary>
        /// <param name="con"></param>
        /// <param name="routineref"></param>
        /// <returns></returns>
        protected override DataTable GetRoutineData(DbConnection con, DBSchemaItemRef routineref)
        {
            System.Data.OleDb.OleDbConnection olecon = (System.Data.OleDb.OleDbConnection)con;
            DataTable dt = olecon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Procedures,
                new object[] { null, null, routineref.Name, null });

            dt.TableName = "Routines";

            WriteCollectionData("Routines", new string[] { routineref.Name }, dt);

            return dt;
        }

        #endregion

        #region protected override void FillRoutineData(DBSchemaRoutine aroutine, DataRow dataRow)
        /// <summary>
        /// Populates a routine from the meta data DataTable
        /// </summary>
        /// <param name="aroutine"></param>
        /// <param name="dataRow"></param>
        protected override void FillRoutineData(DBSchemaRoutine aroutine, DataRow dataRow)
        {
            //base.FillRoutineData(aroutine, dataRow);
            DataTable dt = dataRow.Table;
            DataColumn catalog = GetColumn(dt, "PROCEDURE_CATALOG", false);
            DataColumn schema = GetColumn(dt, "PROCEDURE_SCHEMA", false);
            DataColumn name = GetColumn(dt, "PROCEDURE_NAME", true);
            DataColumn definition = GetColumn(dt, "PROCEDURE_DEFINITION", false);

            aroutine.Catalog = GetColumnStringValue(dataRow, catalog);
            aroutine.Schema = GetColumnStringValue(dataRow, schema);
            aroutine.Name = GetColumnStringValue(dataRow, name);
        }

        #endregion

        /// <summary>
        /// Ignored
        /// </summary>
        /// <param name="con"></param>
        /// <param name="routineref"></param>
        /// <returns></returns>
        protected override DataTable GetRoutineParams(DbConnection con, DBSchemaItemRef routineref)
        {
            //Do nothing as the parameters canot be determined here
            return null;
        }

        /// <summary>
        /// Ignored
        /// </summary>
        /// <param name="aroutine"></param>
        /// <param name="dtSprocParams"></param>
        protected override void FillRoutineParams(DBSchemaRoutine aroutine, DataTable dtSprocParams)
        {
            //Do Nothing as parameter inspeciton is not supported
        }

        #region protected override DataTable GetSprocResultSchema(DbConnection con, DBSchemaRoutine routine)

        /// <summary>
        /// Gets the meta data DataTable for the routine results schema
        /// </summary>
        /// <param name="con"></param>
        /// <param name="routine"></param>
        /// <returns></returns>
        protected override DataTable GetSprocResultSchema(DbConnection con, DBSchemaRoutine routine)
        {
            //some access queries support the analysis, others don't????

            string fullname = "SELECT * FROM " + GetFullName(routine.GetReference());

            
            using (DbCommand cmd = this.Database.CreateCommand(con, fullname))
            {
                cmd.CommandType = CommandType.Text;
                if (routine.Parameters.Count > 0)
                {
                    foreach (DBSchemaParameter sp in routine.Parameters)
                    {
                        DbParameter param = this.Database.AddCommandParameter(cmd, sp.InvariantName, sp.DbType);
                        param.Value = DBNull.Value;
                        param.Direction = sp.Direction;


                    }
                }

                using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    DataTable dt = reader.GetSchemaTable();
                    if(null != dt)
                        WriteCollectionData("Procedures", new string[] { routine.Name }, dt);

                    return dt;
                }
            }
        }

        #endregion

        #region protected override void FillSprocResults(DBSchemaSproc asproc, DataTable dtColumns)

        /// <summary>
        /// Fills the sproc results data from the meta data columns table
        /// </summary>
        /// <param name="asproc"></param>
        /// <param name="dtColumns"></param>
        protected override void FillSprocResults(DBSchemaSproc asproc, DataTable dtColumns)
        {
            if (null == dtColumns)
                return;

            DataColumn ColumnNameColumn = GetColumn(dtColumns, "ColumnName", true);
            DataColumn OrdinalPositionColumn = GetColumn(dtColumns, "ColumnOrdinal", true);
            DataColumn DefaultValueColumn = GetColumn(dtColumns, "COLUMN_DEFAULT", false);
            DataColumn IsNullableColumn = GetColumn(dtColumns, "AllowDBNull", true);
            DataColumn DataTypeColumn = GetColumn(dtColumns, "DataType", true);
            DataColumn SizeColumn = GetColumn(dtColumns, "ColumnSize", true);
            DataColumn AutoIncrementColumn = GetColumn(dtColumns, "IsAutoIncrement", false);
            DataColumn IsReadOnly = GetColumn(dtColumns, "IsReadOnly", false);

            foreach (DataRow row in dtColumns.Rows)
            {
                DBSchemaViewColumn col = new DBSchemaViewColumn();
                col.Name = GetColumnStringValue(row, ColumnNameColumn);
                col.OrdinalPosition = GetColumnIntValue(row, OrdinalPositionColumn);
                col.DefaultValue = GetColumnStringValue(row, DefaultValueColumn);
                col.Nullable = GetColumnBoolValue(row, IsNullableColumn);
                string type = GetColumnStringValue(row, DataTypeColumn);
                if (!string.IsNullOrEmpty(type))
                {
                    col.Type = Type.GetType(type);
                    col.DbType = DBHelper.GetDBTypeForRuntimeType(col.Type);
                }
                col.Size = GetColumnIntValue(row, SizeColumn);
                col.ReadOnly = GetColumnBoolValue(row, IsReadOnly) || GetColumnBoolValue(row, AutoIncrementColumn);

                col.HasDefault = !string.IsNullOrEmpty(col.DefaultValue);
                
                asproc.Results.Add(col);
            }
        }

        #endregion


        //
        // ForeignKeys - complete
        //

        #region protected override DBSchemaForeignKey LoadAForeignKey(DbConnection con, DBSchemaItemRef fkref)
        /// <summary>
        /// returns the base method results
        /// </summary>
        /// <param name="con"></param>
        /// <param name="fkref"></param>
        /// <returns></returns>
        protected override DBSchemaForeignKey LoadAForeignKey(DbConnection con, DBSchemaItemRef fkref)
        {
            return base.LoadAForeignKey(con, fkref);
        }

        #endregion

        #region protected override void LoadForeignKeyRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)
        /// <summary>
        /// Loads all the foreign keys for a specific table
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        /// <param name="fortable"></param>
        protected override void LoadForeignKeyRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)
        {
            System.Data.OleDb.OleDbConnection olecon = (System.Data.OleDb.OleDbConnection)con;
            DataTable data = olecon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Foreign_Keys, 
                new object[] { null, null, null, null, null, fortable.Name });

            data.TableName = "Foreign Keys";

            WriteCollectionData("Foreign Keys", new string[] { }, data);

            DataColumn catalogcol = GetColumn(data, "FK_CATALOG", false);
            DataColumn schemacol = GetColumn(data, "FK_SCHEMA", false);
            DataColumn namecol = GetColumn(data, "FK_NAME", true);

            DataColumn containercatalogcol = GetColumn(data, "FK_TABLE_CATALOG", false);
            DataColumn containerschemacol = GetColumn(data, "FK_TABLE_SCHEMA", false);
            DataColumn containernamecol = GetColumn(data, "FK_TABLE_NAME", true);

            this.LoadItemRefsWithContainer(data, intoCollection,
                catalogcol, schemacol, namecol, DBSchemaTypes.ForeignKey,
                containercatalogcol, containerschemacol, containernamecol, DBSchemaTypes.Table);
        }

        #endregion

        #region protected override void LoadForeignKeyRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        /// <summary>
        /// Loads all the foreign keys
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected override void LoadForeignKeyRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            System.Data.OleDb.OleDbConnection olecon = (System.Data.OleDb.OleDbConnection)con;
            DataTable data = olecon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Foreign_Keys, 
                new object[] { });
            data.TableName = "Foreign Keys";

            WriteCollectionData("Foreign Keys", new string[] { }, data);

            DataColumn catalogcol = GetColumn(data, "FK_CATALOG", false);
            DataColumn schemacol = GetColumn(data, "FK_SCHEMA", false);
            DataColumn namecol = GetColumn(data, "FK_NAME", true);

            DataColumn containercatalogcol = GetColumn(data, "FK_TABLE_CATALOG", false);
            DataColumn containerschemacol = GetColumn(data, "FK_TABLE_SCHEMA", false);
            DataColumn containernamecol = GetColumn(data, "FK_TABLE_NAME", true);

            this.LoadItemRefsWithContainer(data, intoCollection,
                catalogcol, schemacol, namecol, DBSchemaTypes.ForeignKey,
                containercatalogcol, containerschemacol, containernamecol, DBSchemaTypes.Table);

        }

        #endregion

        #region protected override DataTable GetForeignKeyData(DbConnection con, DBSchemaItemRef fkref)
        /// <summary>
        /// Gets the meta data for a specific foreign key
        /// </summary>
        /// <param name="con"></param>
        /// <param name="fkref"></param>
        /// <returns></returns>
        protected override DataTable GetForeignKeyData(DbConnection con, DBSchemaItemRef fkref)
        {
            string table = null;
            if (fkref.Container != null && !string.IsNullOrEmpty(fkref.Container.Name))
                table = fkref.Container.Name;
            System.Data.OleDb.OleDbConnection olecon = (System.Data.OleDb.OleDbConnection)con;
            DataTable dt = olecon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Foreign_Keys,
                new object[] { null, null, null, null, null, table });

            dt.TableName = "Foreign Keys";
            DataColumn namecol = GetColumn(dt,"FK_NAME",true);

            foreach (DataRow row in dt.Rows)
            {
                if (!row[namecol].Equals(fkref.Name))
                    row.Delete();
            }
            dt.AcceptChanges();

            WriteCollectionData("Foreign Keys", new string[] { fkref.Catalog, fkref.Schema, fkref.Name, null }, dt);

            return dt;

        }

        #endregion

        #region protected override void FillForeignKeyData(DBSchemaForeignKey fk, DataRow dtFKRow)
        /// <summary>
        /// populates the meta data for a specific foreign key
        /// </summary>
        /// <param name="fk"></param>
        /// <param name="dtFKRow"></param>
        protected override void FillForeignKeyData(DBSchemaForeignKey fk, DataRow dtFKRow)
        {
            DataTable dt = dtFKRow.Table;
            
            DataColumn schemacol = GetColumn(dt, "FK_SCHEMA", false);
            DataColumn catalogcol = GetColumn(dt, "FK_CATALOG", false);
            DataColumn namecol = GetColumn(dt, "FK_NAME", true);

            DataColumn pkcatalogcol = GetColumn(dt, "PK_CATALOG_NAME", false);
            DataColumn pkschemacol = GetColumn(dt, "PK_SCHEMA_NAME", false);
            DataColumn pktablecol = GetColumn(dt, "PK_TABLE_NAME", true);
            DataColumn pkcolumncol = GetColumn(dt, "PK_COLUMN_NAME", true);

            DataColumn fkcatalogcol = GetColumn(dt, "FK_CATALOG_NAME", false);
            DataColumn fkschemacol = GetColumn(dt, "FK_SCHEMA_NAME", false);
            DataColumn fktablecol = GetColumn(dt, "FK_TABLE_NAME", false);
            DataColumn fkcolumncol = GetColumn(dt, "FK_COLUMN_NAME", false);

            fk.Catalog = GetColumnStringValue(dtFKRow, catalogcol);
            fk.Schema = GetColumnStringValue(dtFKRow, schemacol);
            fk.Name = GetColumnStringValue(dtFKRow, namecol);

            DBSchemaItemRef pktable = new DBSchemaItemRef(DBSchemaTypes.Table,
                GetColumnStringValue(dtFKRow, pkcatalogcol),
                GetColumnStringValue(dtFKRow, pkschemacol),
                GetColumnStringValue(dtFKRow, pktablecol));

            DBSchemaItemRef fktable = new DBSchemaItemRef(DBSchemaTypes.Table,
                            GetColumnStringValue(dtFKRow, fkcatalogcol),
                            GetColumnStringValue(dtFKRow, fkschemacol),
                            GetColumnStringValue(dtFKRow, fktablecol));

            fk.ForeignKeyTable = fktable;
            fk.PrimaryKeyTable = pktable;
            
            DBSchemaForeignKeyMapping map = new DBSchemaForeignKeyMapping();
            map.ForeignColumn = GetColumnStringValue(dtFKRow, fkcolumncol);
            map.PrimaryColumn = GetColumnStringValue(dtFKRow, pkcolumncol);

            fk.Mappings.Add(map);

        }

        #endregion
        /// <summary>
        /// Ignored
        /// </summary>
        /// <param name="con"></param>
        /// <param name="fkref"></param>
        /// <returns></returns>
        protected override DataTable GetForeignKeyColumns(DbConnection con, DBSchemaItemRef fkref)
        {
            // Should be done in the FillForeignData 
            return null;
        }

        /// <summary>
        /// Ignored
        /// </summary>
        /// <param name="anFk"></param>
        /// <param name="dtColumns"></param>
        protected override void FillForeignKeyColumns(DBSchemaForeignKey anFk, DataTable dtColumns)
        {
            // Should be done in the FillForeignKeyData
        }

        //
        // Views - complete
        //

        #region protected override void LoadViewRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        /// <summary>
        /// Calls base method
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected override void LoadViewRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            base.LoadViewRefs(con, intoCollection);
        }

        #endregion

        #region protected override DataTable GetViewData(DbConnection con, DBSchemaItemRef vref)
        /// <summary>
        /// Gets the meta data DataTable for the requested view
        /// </summary>
        /// <param name="con"></param>
        /// <param name="vref"></param>
        /// <returns></returns>
        protected override DataTable GetViewData(DbConnection con, DBSchemaItemRef vref)
        {
            System.Data.OleDb.OleDbConnection olecon = (System.Data.OleDb.OleDbConnection)con;
            DataTable dt = olecon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Views, new object[] { null, null, vref.Name });
            dt.TableName = "Views";
            WriteCollectionData("Views", new string[] { vref.Catalog, vref.Schema, vref.Name, null }, dt);

            return dt;

        }

        #endregion

        #region protected override void FillViewData(DBSchemaView view, DataRow row) 
        /// <summary>
        /// Populates the view with the information in the DataRow
        /// </summary>
        /// <param name="view"></param>
        /// <param name="row"></param>
        protected override void FillViewData(DBSchemaView view, DataRow row)
        {
            DataTable tbl = row.Table;
            DataColumn name = GetColumn(tbl, "TABLE_NAME", true);
            DataColumn update = GetColumn(tbl, "IS_UPDATABLE", true);

            view.Name = GetColumnStringValue(row, name);
            view.IsUpdateable = GetColumnBoolValue(row, update);


        }

        #endregion

        #region protected override DataTable GetViewColumns(DbConnection con, DBSchemaItemRef vref)
        /// <summary>
        /// Gets the meta data DataTable for the view's columns
        /// </summary>
        /// <param name="con"></param>
        /// <param name="vref"></param>
        /// <returns></returns>
        protected override DataTable GetViewColumns(DbConnection con, DBSchemaItemRef vref)
        {
            System.Data.OleDb.OleDbConnection olecon = (System.Data.OleDb.OleDbConnection)con;
            DataTable dt = olecon.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Columns, new object[] { null, null, vref.Name });
            dt.TableName = "View Columns";
            WriteCollectionData("View Columns", new string[] { vref.Catalog, vref.Schema, vref.Name, null }, dt);
            return dt;
        }

        #endregion

        #region protected override void FillViewColumns(DBSchemaViewColumnCollection aview, DataTable dtColumns)
        /// <summary>
        /// Fills the view info with the meta data Columns info
        /// </summary>
        /// <param name="aview"></param>
        /// <param name="dtColumns"></param>
        protected override void FillViewColumns(DBSchemaViewColumnCollection aview, DataTable dtColumns)
        {
            DataColumn ColumnNameColumn = GetColumn(dtColumns, "COLUMN_NAME", true);
            DataColumn OrdinalPositionColumn = GetColumn(dtColumns, "ORDINAL_POSITION", true);
            DataColumn DefaultValueColumn = GetColumn(dtColumns, "COLUMN_HASDEFAULT", false);
            DataColumn IsNullableColumn = GetColumn(dtColumns, "IS_NULLABLE", true);
            DataColumn DataTypeColumn = GetColumn(dtColumns, "DATA_TYPE", true);
            DataColumn MaxCharacterLengthColumn = GetColumn(dtColumns, "CHARACTER_MAXIMUM_LENGTH", false);
            DataColumn IsReadOnly = GetColumn(dtColumns, "ISREADONLY", false);

            foreach (DataRow row in dtColumns.Rows)
            {
                DBSchemaViewColumn col = new DBSchemaViewColumn();
                col.Name = GetColumnStringValue(row, ColumnNameColumn);
                col.OrdinalPosition = GetColumnIntValue(row, OrdinalPositionColumn);
                col.DefaultValue = GetColumnStringValue(row, DefaultValueColumn);
                col.Nullable = GetColumnBoolValue(row, IsNullableColumn);
                int type = GetColumnIntValue(row, DataTypeColumn, -1);
                DBMSAccessTypeMapping map = null;
                if (type > 0 && this.Types.TryGetType(type, out map))
                {
                    col.DbType = GetDbTypeForNativeType(map.TypeName);
                    if (null == map.RuntimeType)
                        col.Type = GetSystemTypeForNativeType(map.TypeName);
                    else
                    {
                        col.Type = map.RuntimeType;
                        if (col.DbType == DbType.Object)
                            col.DbType = DBHelper.GetDBTypeForRuntimeType(col.Type);
                    }
                }
                col.Size = GetColumnIntValue(row, MaxCharacterLengthColumn);
                col.ReadOnly = GetColumnBoolValue(row, IsReadOnly);
                col.HasDefault = GetColumnBoolValue(row, DefaultValueColumn);

                aview.Add(col);
            }
        }

        #endregion

       

        
    }
}
