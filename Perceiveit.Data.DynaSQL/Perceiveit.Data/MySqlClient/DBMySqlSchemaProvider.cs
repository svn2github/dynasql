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
using System.Data.Common;
using Perceiveit.Data.Query;
using Perceiveit.Data.Schema;

namespace Perceiveit.Data.MySqlClient
{
    /// <summary>
    /// Implements the MySql SchemaProvider
    /// </summary>
    public class DBMySqlSchemaProvider : DBSchemaProvider
    {
        //
        // .ctor
        //

        #region public DBMySqlSchemaProvider(DBDatabase database, DBDatabaseProperties properties)

        public DBMySqlSchemaProvider(DBDatabase database, DBDatabaseProperties properties)
            : base(database, properties)
        {
        }

        #endregion

        protected override string GetFullName(DBSchemaItemRef iref)
        {
            return "`" + iref.FullName.Replace(".", "`.`") + "`";
        }

        protected override void LoadTableRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection tblcollection = this.AssertGetCollectionForType(DBMetaDataCollectionType.Tables);
            DataTable data = this.GetCollectionData(con, tblcollection, null, null, null, "BASE TABLE");

            DataColumn catalogcol = GetColumn(data, "TABLE_CATALOG", false);
            DataColumn schemacol = GetColumn(data, "TABLE_SCHEMA", false);
            DataColumn namecol = GetColumn(data, "TABLE_NAME", true);

            this.LoadItemRefs(data, intoCollection, catalogcol, schemacol, namecol, DBSchemaTypes.Table);
        }

        protected override void LoadViewRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection tblcollection = this.AssertGetCollectionForType(DBMetaDataCollectionType.Views);
            DataTable data = this.GetCollectionData(con, tblcollection, null, null, null, "SYSTEM VIEW");

            DataColumn catalogcol = GetColumn(data, "TABLE_CATALOG", false);
            DataColumn schemacol = GetColumn(data, "TABLE_SCHEMA", false);
            DataColumn namecol = GetColumn(data, "TABLE_NAME", true);

            this.LoadItemRefs(data, intoCollection, catalogcol, schemacol, namecol, DBSchemaTypes.View);
        }

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

            //If there are no explicit schema columns for the containing table then use the column for the index.
            if (null == tableschemacol && null != schemacol)
                tableschemacol = schemacol;
            if (null == tablecatalogcol && null != catalogcol)
                tablecatalogcol = catalogcol;

            this.LoadItemRefsWithContainer(data, intoCollection,
                    catalogcol, schemacol, namecol, DBSchemaTypes.Index,
                    tablecatalogcol, tableschemacol, tablenamecol, DBSchemaTypes.Table);
        }

        protected override void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)
        {
            DBSchemaMetaDataCollection viewcollection =
                 this.AssertGetCollectionForType(DBMetaDataCollectionType.Indexes);
            DataTable data = this.GetCollectionData(con, viewcollection, fortable.Catalog, fortable.Schema, fortable.Name, null);

            DataColumn catalogcol = GetColumn(data, "index_catalog", false);
            DataColumn schemacol = GetColumn(data, "index_schema", false);
            DataColumn namecol = GetColumn(data, "index_name", true);

            DataColumn tablecatalogcol = GetColumn(data, "table_catalog", false);
            DataColumn tableschemacol = GetColumn(data, "table_schema", false);
            DataColumn tablenamecol = GetColumn(data, "table_name", true);

            //If there are no explicit schema columns for the containing table then use the column for the index.
            if (null == tableschemacol && null != schemacol)
                tableschemacol = schemacol;
            if (null == tablecatalogcol && null != catalogcol)
                tablecatalogcol = catalogcol;

            this.LoadItemRefsWithContainer(data, intoCollection,
                    catalogcol, schemacol, namecol, DBSchemaTypes.Index,
                    tablecatalogcol, tableschemacol, tablenamecol, DBSchemaTypes.Table);
        }

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
            DataColumn CharacterSetColumn = GetColumn(dtColumns, "CHARACTER_SET_NAME", false);
            DataColumn KeyColumn = GetColumn(dtColumns, "COLUMN_KEY", false);
            DataColumn ExtraColumn = GetColumn(dtColumns, "EXTRA", false);

            foreach (DataRow row in dtColumns.Rows)
            {
                DBSchemaTableColumn col = new DBSchemaTableColumn();
                col.Name = GetColumnStringValue(row, ColumnNameColumn);
                col.OrdinalPosition = GetColumnIntValue(row, OrdinalPositionColumn);
                col.DefaultValue = GetColumnStringValue(row, DefaultValueColumn);
                col.NativeType = GetColumnStringValue(row, DataTypeColumn);
                col.Nullable = GetColumnBoolValue(row, IsNullableColumn);
                col.DbType = GetDbTypeForNativeType(col.NativeType, GetColumnStringValue(row, CharacterSetColumn));
                col.Type = GetSystemTypeForNativeType(col.NativeType);
                col.Size = GetColumnIntValue(row, MaxCharacterLengthColumn);

                string extra = GetColumnStringValue(row, ExtraColumn,"");
                col.AutoAssign = extra.IndexOf("auto_increment") > -1;

                string key = GetColumnStringValue(row, KeyColumn, "");
                col.PrimaryKey = key.IndexOf("PRI") > -1;

                col.ReadOnly = col.AutoAssign;
                col.HasDefault = !string.IsNullOrEmpty(col.DefaultValue);

                atablecolumns.Add(col);
            }

        }

        protected override void FillSprocResults(DBSchemaSproc asproc, DataTable dtSprocResults)
        {
            DataColumn ColumnNameColumn = GetColumn(dtSprocResults, "COLUMNNAME", true);
            DataColumn OrdinalPositionColumn = GetColumn(dtSprocResults, "COLUMNORDINAL", true);
            DataColumn DefaultValueColumn = GetColumn(dtSprocResults, "COLUMNDEFAULT", false);
            DataColumn IsNullableColumn = GetColumn(dtSprocResults, "ALLOWDBNULL", true);
            DataColumn DataTypeColumn = GetColumn(dtSprocResults, "DATATYPE", true);
            DataColumn MaxCharacterLengthColumn = GetColumn(dtSprocResults, "COLUMNSIZE", false);
            DataColumn IsReadOnly = GetColumn(dtSprocResults, "ISREADONLY", false);

            foreach (DataRow row in dtSprocResults.Rows)
            {
                DBSchemaViewColumn col = new DBSchemaViewColumn();
                col.Name = GetColumnStringValue(row, ColumnNameColumn);
                col.OrdinalPosition = GetColumnIntValue(row, OrdinalPositionColumn);
                col.DefaultValue = GetColumnStringValue(row, DefaultValueColumn);
                col.Nullable = GetColumnBoolValue(row, IsNullableColumn);
                string type = GetColumnStringValue(row, DataTypeColumn);
                if (!string.IsNullOrEmpty(type))
                {
                    Type t = Type.GetType(type);
                    col.DbType = DBHelper.GetDBTypeForRuntimeType(t);
                    col.Type = t;
                }
                col.NativeType = type;
                col.Size = GetColumnIntValue(row, MaxCharacterLengthColumn);
                col.ReadOnly = GetColumnBoolValue(row, IsReadOnly);
                col.HasDefault = !string.IsNullOrEmpty(col.DefaultValue);

                asproc.Results.Add(col);
            }
        }

        protected override DataTable GetSprocResultSchema(DbConnection con, DBSchemaRoutine routine)
        {
            return null; //Doesn't support the SchemaOnly option on the command
        }

        protected override void FillIndexData(DBSchemaIndex anindex, DataRow dataRow)
        {
            DataColumn catalog = GetColumn(dataRow.Table, "index_catalog", false);
            DataColumn schema = GetColumn(dataRow.Table, "index_schema", false);
            DataColumn name = GetColumn(dataRow.Table, "index_name", true);

            DataColumn tblcatalog = GetColumn(dataRow.Table, "table_catalog", false);
            DataColumn tblschema = GetColumn(dataRow.Table, "table_schema", false);
            DataColumn tblname = GetColumn(dataRow.Table, "table_name", true);
            DataColumn unique = GetColumn(dataRow.Table, "unique", false);
            DataColumn pk = GetColumn(dataRow.Table, "primary", false);

            anindex.Catalog = GetColumnStringValue(dataRow, catalog);
            anindex.Schema = GetColumnStringValue(dataRow, schema);
            anindex.Name = GetColumnStringValue(dataRow, name);
            anindex.IsPrimaryKey = GetColumnBoolValue(dataRow, pk);
            anindex.IsUnique = GetColumnBoolValue(dataRow, unique);


            DBSchemaItemRef tblref = new DBSchemaItemRef(DBSchemaTypes.Table,
                GetColumnStringValue(dataRow, tblcatalog), GetColumnStringValue(dataRow, tblschema), GetColumnStringValue(dataRow, tblname));
            anindex.TableReference = tblref;

        }

        protected override DataTable GetForeignKeyData(DbConnection con, DBSchemaItemRef fkref)
        {
            string table = null;
            if (fkref.Container != null)
                table = fkref.Container.Name;

            DBSchemaMetaDataCollection fkcol = this.AssertGetCollectionForType(DBMetaDataCollectionType.ForeignKeys);
            DataTable dtFK = this.GetCollectionData(con, fkcol, fkref.Catalog, fkref.Schema, table, null);
            for (int i = dtFK.Rows.Count - 1; i >= 0; i--)
            {
                DataRow row = dtFK.Rows[i];
                if (row["constraint_name"].ToString().Equals(fkref.Name, StringComparison.OrdinalIgnoreCase) == false)
                    row.Delete();
            }
            dtFK.AcceptChanges();

            return dtFK;
        }

        protected override DataTable GetForeignKeyColumns(DbConnection con, DBSchemaItemRef fkref)
        {
            DBSchemaMetaDataCollection colmscol = this.AssertGetCollectionForType(DBMetaDataCollectionType.IndexColumns);

            string sql = @"SELECT 
    CONSTRAINT_SCHEMA  AS FKSchema,
    CONSTRAINT_NAME AS FKConstraintName,
    TABLE_SCHEMA AS FKTableSchema,
    TABLE_NAME AS FKTable,
    COLUMN_NAME AS FKColumn,
    REFERENCED_TABLE_SCHEMA AS PKSchema, 
    REFERENCED_TABLE_NAME AS PKTable, 
    REFERENCED_COLUMN_NAME AS PKColumn
    
    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE CONSTRAINT_NAME = {0} AND 
    TABLE_SCHEMA = {1} AND TABLE_NAME = {2}";


            using (DbCommand cmd = this.Database.CreateCommand(con, ""))
            {
                DbParameter pname = this.Database.AddCommandParameter(cmd, "name", DbType.String);
                //DbParameter pcatalog = this.Database.AddCommandParameter(cmd, "catalog", DbType.String);
                DbParameter pschema = this.Database.AddCommandParameter(cmd, "schema", DbType.String);
                DbParameter ptable = this.Database.AddCommandParameter(cmd, "table", DbType.String);

                //we format here to get the native name from the parameter
                cmd.CommandText = string.Format(sql, pname.ParameterName, pschema.ParameterName, ptable.ParameterName);

                pname.Value = fkref.Name;
                if (fkref.Container != null)
                {
                    ptable.Value = fkref.Container.Name;
                    pschema.Value = fkref.Container.Schema;
                }
                else
                {
                    ptable.Value = "%";
                    pschema.Value = "%";
                }

                DataSet ds = new DataSet();
                DataTable dt = ds.Tables.Add("ForeignKeys");
                this.Database.PopulateDataSet(ds, cmd, LoadOption.OverwriteChanges, "ForeignKeys");
                ds.AcceptChanges();
                dt.WriteXml("C:\\SchemaOutput\\ForeignKeys" + fkref.Name + ".xml");
                return dt;
            }

        }


    }
}
