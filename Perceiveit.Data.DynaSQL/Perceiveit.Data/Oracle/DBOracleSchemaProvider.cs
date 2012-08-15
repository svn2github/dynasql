using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Perceiveit.Data.Schema;
using System.Data.Common;
using System.Data;

namespace Perceiveit.Data.Oracle
{
    public class DBOracleSchemaProvider : DBSchemaProvider
    {

        public DBOracleSchemaProvider(DBDatabase database, DBDatabaseProperties properties)
            : base(database, properties)
        {
        }


        //
        // Table Schema
        //


        protected override DataTable GetTableData(DbConnection con, DBSchemaItemRef tableref)
        {
            DBSchemaMetaDataCollection tblcol = this.AssertGetCollectionForType(DBMetaDataCollectionType.Tables);
            DataTable dtTable = this.GetCollectionData(con, tblcol, tableref.Schema, tableref.Name);

            return dtTable;
        }

        protected override void LoadTableRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection tblcollection = this.AssertGetCollectionForType(DBMetaDataCollectionType.Tables);
            DataTable data = this.GetCollectionData(con, tblcollection);

            DataColumn catalogcol = GetColumn(data, "CATALOG", false);
            DataColumn schemacol = GetColumn(data, "OWNER", false);
            DataColumn namecol = GetColumn(data, "TABLE_NAME", true);

            this.LoadItemRefs(data, intoCollection, catalogcol, schemacol, namecol, DBSchemaTypes.Table);
        }

        protected override void FillTableData(DBSchemaTable table, DataRow row)
        {
            DataColumn cat = GetColumn(row.Table, "CATALOG", false);
            DataColumn schema = GetColumn(row.Table, "OWNER", false);
            DataColumn name = GetColumn(row.Table, "TABLE_NAME", true);

            table.Catalog = GetColumnStringValue(row, cat);
            table.Schema = GetColumnStringValue(row, schema);
            table.Name = GetColumnStringValue(row, name);
        }

        protected override DataTable GetTableColumns(DbConnection con, DBSchemaItemRef tableref)
        {
            DBSchemaMetaDataCollection colmscol = this.AssertGetCollectionForType(DBMetaDataCollectionType.Columns);
            DataTable dt = this.GetCollectionData(con, colmscol, tableref.Schema, tableref.Name);

            DBSchemaMetaDataCollection pks = this.AssertGetCollectionForName("PrimaryKeys");
            DataTable pkcols = this.GetCollectionData(con, pks, tableref.Schema, tableref.Name);
            if (pkcols.Rows.Count > 0)
            {
                string owner = pkcols.Rows[0]["OWNER"].ToString();
                string indexname = pkcols.Rows[0]["INDEX_NAME"].ToString();
                DBSchemaMetaDataCollection ix = this.AssertGetCollectionForType(DBMetaDataCollectionType.IndexColumns);

                pkcols = this.GetCollectionData(con, ix, owner, indexname);

                FillPrimaryKeys(dt, pkcols);
            }
            return dt;
        }

        private static void FillPrimaryKeys(DataTable tabledata, DataTable pkcols)
        {
            DataColumn pkTableColumn = tabledata.Columns.Add("PRIMARY_KEY", typeof(bool));
            DataColumn tableColNameColumn = tabledata.Columns["COLUMN_NAME"];
            DataColumn pkcolsNameCol = pkcols.Columns["COLUMN_NAME"];

            //Go through each primary key
            //update the relevant row in the tabledata
            //setting PRIMARY_KEY = true
            foreach (DataRow pkRow in pkcols.Rows)
            {
                string colname = GetColumnStringValue(pkRow, pkcolsNameCol);

                if (!string.IsNullOrEmpty(colname))
                {
                    foreach (DataRow tablerow in tabledata.Rows)
                    {
                        string name = GetColumnStringValue(tablerow, tableColNameColumn);
                        if (string.Equals(colname, name))
                        {
                            tablerow[pkTableColumn] = true;
                        }
                    }
                }
            }
        }

        protected override void FillTableColumns(DBSchemaTableColumnCollection atablecolumns, DataTable dtColumns)
        {
            DataColumn TableNameColumn = GetColumn(dtColumns, "TABLE_NAME", true);
            DataColumn TableSchemaColumn = GetColumn(dtColumns, "OWNER", false);
            DataColumn TableCatalogColumn = GetColumn(dtColumns, "TABLE_CATALOG", false);
            DataColumn ColumnNameColumn = GetColumn(dtColumns, "COLUMN_NAME", true);
            DataColumn OrdinalPositionColumn = GetColumn(dtColumns, "ID", true);
            DataColumn DefaultValueColumn = GetColumn(dtColumns, "COLUMN_DEFAULT", false);
            DataColumn IsNullableColumn = GetColumn(dtColumns, "NULLABLE", true);
            DataColumn DataTypeColumn = GetColumn(dtColumns, "DATATYPE", true);
            DataColumn MaxCharacterLengthColumn = GetColumn(dtColumns, "LENGTH", false);
            DataColumn CharacterSetColumn = GetColumn(dtColumns, "CHARACTER_SET_NAME", false);
            DataColumn AutoNumberColumn = GetColumn(dtColumns, "AUTOINCREMENT", false);
            DataColumn PrimaryKeyColumn = GetColumn(dtColumns, "PRIMARY_KEY", false);

            foreach (DataRow row in dtColumns.Rows)
            {
                DBSchemaTableColumn col = new DBSchemaTableColumn();
                col.Name = GetColumnStringValue(row, ColumnNameColumn);
                col.OrdinalPosition = GetColumnIntValue(row, OrdinalPositionColumn);
                col.DefaultValue = GetColumnStringValue(row, DefaultValueColumn);
                col.Nullable = GetColumnBoolValue(row, IsNullableColumn);
                col.NativeType = GetColumnStringValue(row, DataTypeColumn);
                col.DbType = GetDbTypeForNativeType(col.NativeType, GetColumnStringValue(row, CharacterSetColumn));
                col.Type = GetSystemTypeForNativeType(GetColumnStringValue(row, DataTypeColumn));
                
                col.Size = GetColumnIntValue(row, MaxCharacterLengthColumn);
                //2 byte string values - divide by 2
                if (col.DbType == DbType.String || col.DbType == DbType.StringFixedLength)
                    col.Size = col.Size / 2;
                
                col.AutoAssign = GetColumnBoolValue(row, AutoNumberColumn);
                col.PrimaryKey = GetColumnBoolValue(row, PrimaryKeyColumn);
                col.ReadOnly = col.AutoAssign;
                col.HasDefault = !string.IsNullOrEmpty(col.DefaultValue);

                atablecolumns.Add(col);
            }

        }


        //
        // foreign key schema
        //


        protected override DBSchemaForeignKey LoadAForeignKey(DbConnection con, DBSchemaItemRef fkref)
        {
            DataTable dt = this.GetForeignKeyData(con, fkref);

            if (dt.Rows.Count > 0)
            {
                DBSchemaIndex pk;
                DBSchemaForeignKey fk = new DBSchemaForeignKey();
                this.FillForeignKeyData(fk, dt.Rows[0], out pk);

                //TODO Fill Columns
                DBSchemaItemRef pkIxdRef = pk.GetReference();
                pkIxdRef.Container = pk.TableReference;

                DataTable pkCols = GetIndexColumns(con, pkIxdRef);
                this.FillIndexColuns(pk, pkCols);

                DBSchemaItemRef fkIdxRef = fk.GetReference();
                fkIdxRef.Container = fk.ForeignKeyTable;

                DataTable fkCols = GetForeignKeyColumns(con, fkIdxRef);
                this.FillForeignKeyColumns(fk, fkCols, pkCols);

                return fk;
            }
            else
                return null;
        }

        protected override DataTable GetForeignKeyData(DbConnection con, DBSchemaItemRef fkref)
        {
            DBSchemaMetaDataCollection fkcol = this.AssertGetCollectionForType(DBMetaDataCollectionType.ForeignKeys);
            DataTable dtFK = this.GetCollectionData(con, fkcol, fkref.Schema, null, fkref.Name);

            return dtFK;
        }

        protected override DataTable GetForeignKeyColumns(DbConnection con, DBSchemaItemRef fkref)
        {
            DBSchemaMetaDataCollection colmscol = this.AssertGetCollectionForType(DBMetaDataCollectionType.ForeignKeyColumns);
            DataTable dtfkc = this.GetCollectionData(con, colmscol, fkref.Schema, null, fkref.Name);
            return dtfkc;
        }

        protected void FillForeignKeyColumns(DBSchemaForeignKey anFk, DataTable fkColumns, DataTable pkColumns)
        {
            if (fkColumns.Rows.Count == pkColumns.Rows.Count)
            {
                DataColumn fkNameCol = fkColumns.Columns["COLUMN_NAME"];
                DataColumn pkNameCol = pkColumns.Columns["COLUMN_NAME"];

                for (int i = 0; i < fkColumns.Rows.Count; i++)
                {
                    DBSchemaForeignKeyMapping mapping = new DBSchemaForeignKeyMapping();
                    mapping.PrimaryColumn = GetColumnStringValue(pkColumns.Rows[i], pkNameCol);
                    mapping.ForeignColumn = GetColumnStringValue(fkColumns.Rows[i], fkNameCol);
                    anFk.Mappings.Add(mapping);
                }
            }
            else
            {
                //Cannot do anything as we cannot match
            }
        }

        protected override void FillForeignKeyData(DBSchemaForeignKey fk, DataRow dtFKRow)
        {
            DBSchemaIndex related_pk;
            this.FillForeignKeyData(fk, dtFKRow, out related_pk);
        }

        protected void FillForeignKeyData(DBSchemaForeignKey fk, DataRow dtFKRow, out DBSchemaIndex related_pk)
        {
            DataColumn catalog = GetColumn(dtFKRow.Table, "foreign_key_catalog", false);
            DataColumn schema = GetColumn(dtFKRow.Table, "foreign_key_owner", false);
            DataColumn name = GetColumn(dtFKRow.Table, "foreign_key_constraint_name", true);
            DataColumn table = GetColumn(dtFKRow.Table, "foreign_key_table_name", false);

            DataColumn pk_table = GetColumn(dtFKRow.Table, "primary_key_table_name",false);
            DataColumn pk_owner = GetColumn(dtFKRow.Table,"primary_key_owner",false);
            DataColumn pk_constraint = GetColumn(dtFKRow.Table,"primary_key_constraint_name",false);

            fk.Catalog = GetColumnStringValue(dtFKRow, catalog);
            fk.Schema = GetColumnStringValue(dtFKRow, schema);
            fk.Name = GetColumnStringValue(dtFKRow, name);

            string fk_table_value = GetColumnStringValue(dtFKRow, table);
            fk.ForeignKeyTable = new DBSchemaItemRef(DBSchemaTypes.Table, fk.Schema, fk_table_value);

            string pk_table_value = GetColumnStringValue(dtFKRow,pk_table);
            string pk_owner_value = GetColumnStringValue(dtFKRow, pk_owner);
            string pk_constraint_value = GetColumnStringValue(dtFKRow, pk_constraint);


            related_pk = new DBSchemaIndex(pk_owner_value, pk_constraint_value);
            related_pk.TableReference = new DBSchemaItemRef(DBSchemaTypes.Table, pk_owner_value, pk_table_value);
            fk.PrimaryKeyTable = related_pk.TableReference;

        }


        protected override void LoadForeignKeyRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection viewcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.ForeignKeys);
            DataTable data = this.GetCollectionData(con, viewcollection);

            DataColumn catalogcol = GetColumn(data, "foreign_key_catalog", false);
            DataColumn schemacol = GetColumn(data, "foreign_key_owner", false);
            DataColumn namecol = GetColumn(data, "foreign_key_constraint_name", true);

            DataColumn containercatalogcol = GetColumn(data, "foreign_key_table_catalog", false);
            DataColumn containerschemacol = GetColumn(data, "foreign_key_table_owner", false);
            DataColumn containernamecol = GetColumn(data, "foreign_key_table_name", true);

            this.LoadItemRefsWithContainer(data, intoCollection,
                catalogcol, schemacol, namecol, DBSchemaTypes.ForeignKey,
                containercatalogcol, containerschemacol, containernamecol, DBSchemaTypes.Table);
        }

        protected override void LoadForeignKeyRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)
        {
            DBSchemaMetaDataCollection viewcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.ForeignKeys);
            DataTable data = this.GetCollectionData(con, viewcollection, fortable.Schema, fortable.Name, null);

            DataColumn catalogcol = GetColumn(data, "foreign_key_catalog", false);
            DataColumn schemacol = GetColumn(data, "foreign_key_owner", false);
            DataColumn namecol = GetColumn(data, "foreign_key_constraint_name", true);

            DataColumn containercatalogcol = GetColumn(data, "foreign_key_table_catalog", false);
            DataColumn containerschemacol = GetColumn(data, "foreign_key_table_owner", false);
            DataColumn containernamecol = GetColumn(data, "foreign_key_table_name", true);

            this.LoadItemRefsWithContainer(data, intoCollection,
                catalogcol, schemacol, namecol, DBSchemaTypes.ForeignKey,
                containercatalogcol, containerschemacol, containernamecol, DBSchemaTypes.Table);
        }

        //
        // indexes
        //

        protected override void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection idxcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.Indexes);
            DataTable data = this.GetCollectionData(con, idxcollection);

            DataColumn catalogcol = GetColumn(data, "catalog", false);
            DataColumn schemacol = GetColumn(data, "owner", false);
            DataColumn namecol = GetColumn(data, "index_name", true);

            DataColumn tablecatalogcol = GetColumn(data, "table_catalog", false);
            DataColumn tableschemacol = GetColumn(data, "table_owner", false);
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



        /// <summary>
        /// Loads all the indexes in this providers data connection for the specified table
        /// </summary>
        /// <param name="con"></param>
        /// <param name="fortable"></param>
        /// <param name="intoCollection"></param>
        protected override void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)
        {
            DBSchemaMetaDataCollection idxcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.Indexes);

            DataTable data = this.GetCollectionData(con, idxcollection, fortable.Schema, fortable.Name, string.Empty);
            int i = data.Rows.Count;

            DataColumn catalogcol = GetColumn(data, "catalog", false);
            DataColumn schemacol = GetColumn(data, "owner", false);
            DataColumn namecol = GetColumn(data, "index_name", true);

            DataColumn tablecatalogcol = GetColumn(data, "table_catalog", false);
            DataColumn tableschemacol = GetColumn(data, "table_owner", false);
            DataColumn tablenamecol = GetColumn(data, "table_name", true);

            this.LoadItemRefsWithContainer(data, intoCollection,
                    catalogcol, schemacol, namecol, DBSchemaTypes.Index,
                    tablecatalogcol, tableschemacol, tablenamecol, DBSchemaTypes.Table);
        }

        protected override DataTable GetIndexData(DbConnection con, DBSchemaItemRef idxref)
        {
            DBSchemaMetaDataCollection idxcol = this.AssertGetCollectionForType(DBMetaDataCollectionType.Indexes);
            string table;
            if (idxref.Container != null)
                table = idxref.Container.Name;
            else
                table = null;
            return this.GetCollectionData(con, idxcol, idxref.Schema, idxref.Name);
        }

        protected override void FillIndexData(DBSchemaIndex anindex, DataRow dataRow)
        {
            DataColumn catalog = GetColumn(dataRow.Table, "CATALOG", false);
            DataColumn schema = GetColumn(dataRow.Table, "OWNER", false);
            DataColumn name = GetColumn(dataRow.Table, "INDEX_NAME", true);

            DataColumn tblcatalog = GetColumn(dataRow.Table, "TABLE_CATALOG", false);
            DataColumn tblschema = GetColumn(dataRow.Table, "TABLE_OWNER", false);
            DataColumn tblname = GetColumn(dataRow.Table, "TABLE_NAME", true);

            anindex.Catalog = GetColumnStringValue(dataRow, catalog);
            anindex.Schema = GetColumnStringValue(dataRow, schema);
            anindex.Name = GetColumnStringValue(dataRow, name);

            DBSchemaItemRef tblref = new DBSchemaItemRef(DBSchemaTypes.Table,
                GetColumnStringValue(dataRow, tblcatalog), GetColumnStringValue(dataRow, tblschema), GetColumnStringValue(dataRow, tblname));
            anindex.TableReference = tblref;
        }

        protected override DataTable GetIndexColumns(DbConnection con, DBSchemaItemRef idxref)
        {
            DBSchemaMetaDataCollection col = this.AssertGetCollectionForType(DBMetaDataCollectionType.IndexColumns);
            string ownertable = idxref.Container == null ? null : idxref.Container.Name;

            DataTable dt = this.GetCollectionData(con, col, idxref.Schema, idxref.Name, null, ownertable);

            return dt;
        }


        protected override DbType GetDbTypeForNativeType(string providerDataType, string characterset)
        {
            if (providerDataType.Equals("NUMBER", StringComparison.OrdinalIgnoreCase))
                return DbType.Int64;
            else if (providerDataType.Equals("VARCHAR2", StringComparison.OrdinalIgnoreCase))
                return DbType.AnsiString;
            else if (providerDataType.Equals("NVARCHAR2", StringComparison.OrdinalIgnoreCase))
                return DbType.String;
            else
                return base.GetDbTypeForNativeType(providerDataType, characterset);
        }

        //
        // views
        //

        protected override void LoadViewRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection viewcollection = this.AssertGetCollectionForType(DBMetaDataCollectionType.Views);
            DataTable data = this.GetCollectionData(con, viewcollection);

            DataColumn catalogcol = GetColumn(data, "CATALOG", false);
            DataColumn schemacol = GetColumn(data, "OWNER", false);
            DataColumn namecol = GetColumn(data, "VIEW_NAME", true);

            this.LoadItemRefs(data, intoCollection, catalogcol, schemacol, namecol, DBSchemaTypes.View);

        }

        protected override DataTable GetViewData(DbConnection con, DBSchemaItemRef vref)
        {
            DBSchemaMetaDataCollection vcol = this.AssertGetCollectionForType(DBMetaDataCollectionType.Views);
            DataTable dtView = this.GetCollectionData(con, vcol, vref.Schema, vref.Name);

            return dtView;
        }

        protected override void FillViewData(DBSchemaView view, DataRow row)
        {
            DataColumn cat = GetColumn(row.Table, "CATALOG", false);
            DataColumn schema = GetColumn(row.Table, "OWNER", false);
            DataColumn name = GetColumn(row.Table, "VIEW_NAME", true);
            DataColumn update = GetColumn(row.Table, "READ_ONLY", false);

            view.Catalog = GetColumnStringValue(row, cat);
            view.Schema = GetColumnStringValue(row, schema);
            view.Name = GetColumnStringValue(row, name);
            view.IsUpdateable = !GetColumnBoolValue(row, update, false);
        }

        protected override DataTable GetViewColumns(DbConnection con, DBSchemaItemRef vref)
        {
            DBSchemaMetaDataCollection colmscol = this.AssertGetCollectionForType(DBMetaDataCollectionType.Columns);
            DataTable dtVCols = this.GetCollectionData(con, colmscol, vref.Schema, vref.Name);

            return dtVCols;
        }

        protected override void FillViewColumns(DBSchemaViewColumnCollection aview, DataTable dtColumns)
        {
            DataColumn TableNameColumn = GetColumn(dtColumns, "TABLE_NAME", true);
            DataColumn TableSchemaColumn = GetColumn(dtColumns, "OWNER", false);
            DataColumn TableCatalogColumn = GetColumn(dtColumns, "TABLE_CATALOG", false);
            DataColumn ColumnNameColumn = GetColumn(dtColumns, "COLUMN_NAME", true);
            DataColumn OrdinalPositionColumn = GetColumn(dtColumns, "ID", true);
            DataColumn DefaultValueColumn = GetColumn(dtColumns, "COLUMN_DEFAULT", false);
            DataColumn IsNullableColumn = GetColumn(dtColumns, "NULLABLE", true);
            DataColumn DataTypeColumn = GetColumn(dtColumns, "DATATYPE", true);
            DataColumn MaxCharacterLengthColumn = GetColumn(dtColumns, "LENGTH", false);
            DataColumn AutoNumberColumn = GetColumn(dtColumns, "AUTOINCREMENT", false);
            DataColumn PrimaryKeyColumn = GetColumn(dtColumns, "PRIMARY_KEY", false);
            DataColumn CharacterSetColumn = GetColumn(dtColumns, "CHARACTER_SET_NAME", false);

            foreach (DataRow row in dtColumns.Rows)
            {
                DBSchemaViewColumn col = new DBSchemaViewColumn();
                col.Name = GetColumnStringValue(row, ColumnNameColumn);
                col.OrdinalPosition = GetColumnIntValue(row, OrdinalPositionColumn);
                col.DefaultValue = GetColumnStringValue(row, DefaultValueColumn);
                col.Nullable = GetColumnBoolValue(row, IsNullableColumn);
                col.NativeType = GetColumnStringValue(row, DataTypeColumn);
                col.DbType = GetDbTypeForNativeType(col.NativeType, GetColumnStringValue(row, CharacterSetColumn));
                col.Type = GetSystemTypeForNativeType(GetColumnStringValue(row, DataTypeColumn));
                col.Size = GetColumnIntValue(row, MaxCharacterLengthColumn);
                col.HasDefault = !string.IsNullOrEmpty(col.DefaultValue);

                aview.Add(col);
            }
        }

        //
        // stored procedures
        //

        protected override void LoadRoutineRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            LoadStoredProcedureRefs(con, intoCollection);
        }

        protected override void LoadStoredProcedureRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection viewcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.Procedures);
            DataTable data = this.GetCollectionData(con, viewcollection);

            DataColumn catalogcol = GetColumn(data, "CATALOG", false);
            DataColumn schemacol = GetColumn(data, "OWNER", false);
            DataColumn namecol = GetColumn(data, "OBJECT_NAME", true);
            

            foreach (DataRow row in data.Rows)
            {
                DBSchemaItemRef iref;
                iref = this.LoadAnItemRef(row, catalogcol, schemacol, namecol, DBSchemaTypes.StoredProcedure);
                intoCollection.Add(iref);
                
            }
        }

        protected override DataTable GetRoutineData(DbConnection con, DBSchemaItemRef routineref)
        {
            DBSchemaMetaDataCollection routinecol = this.AssertGetCollectionForType(DBMetaDataCollectionType.Procedures);
            return this.GetCollectionData(con, routinecol, routineref.Schema, routineref.Name);
        }

        protected override void FillRoutineData(DBSchemaRoutine aroutine, DataRow dataRow)
        {
            DataColumn catalog = GetColumn(dataRow.Table, "CATALOG", false);
            DataColumn schema = GetColumn(dataRow.Table, "OWNER", false);
            DataColumn name = GetColumn(dataRow.Table, "OBJECT_NAME", true);

            aroutine.Name = GetColumnStringValue(dataRow, name);
            aroutine.Schema = GetColumnStringValue(dataRow, schema);
            aroutine.Catalog = GetColumnStringValue(dataRow, catalog);
        }

        protected override DataTable GetRoutineParams(DbConnection con, DBSchemaItemRef routineref)
        {
            DBSchemaMetaDataCollection routinecol = this.AssertGetCollectionForType(DBMetaDataCollectionType.ProcedureParameters);
            return this.GetCollectionData(con, routinecol, routineref.Schema, routineref.Name);
        }

        protected override void FillRoutineParams(DBSchemaRoutine aroutine, DataTable dtSprocParams)
        {
            DataColumn pos = GetColumn(dtSprocParams, "POSITION", true);
            DataColumn direction = GetColumn(dtSprocParams, "IN_OUT", true);
            DataColumn isResult = GetColumn(dtSprocParams, "IS_RESULT", false);
            DataColumn name = GetColumn(dtSprocParams, "ARGUMENT_NAME", true);
            DataColumn type = GetColumn(dtSprocParams, "DATA_TYPE", true);
            DataColumn strSize = GetColumn(dtSprocParams, "CHARACTER_LENGTH", false);
            DataColumn CharacterSetColumn = GetColumn(dtSprocParams, "CHARACTER_SET_NAME", false);

            foreach (DataRow row in dtSprocParams.Rows)
            {
                DBSchemaParameter param = new DBSchemaParameter();
                param.NativeName = GetColumnStringValue(row, name);
                param.InvariantName = GetInvariantParameterName(param.NativeName);
                param.ParameterIndex = GetColumnIntValue(row, pos, -1);
                string dirvalue = GetColumnStringValue(row, direction);
                param.Direction = GetParameterDirectionFromSchemaValue(dirvalue);
                if (GetColumnBoolValue(row, isResult))
                    param.Direction = ParameterDirection.ReturnValue;
                param.NativeType = GetColumnStringValue(row, type);
                param.DbType = GetDbTypeForNativeType(param.NativeType, GetColumnStringValue(row, CharacterSetColumn));
                param.RuntimeType = GetSystemTypeForNativeType(GetColumnStringValue(row, type));
                param.ParameterSize = GetColumnIntValue(row, strSize, -1);

                aroutine.Parameters.Add(param);
            }
        }

        protected override string GetFullName(DBSchemaItemRef iref)
        {
            return "\"" + iref.FullName.Replace(".", "\".\"") + "\"";
        }

        protected override DataTable GetSprocResultSchema(DbConnection con, DBSchemaRoutine routine)
        {
            //Doesn't support the CommandBehaviour of SchemaOnly
            return null;

            string fullname = GetFullName(routine.GetReference());
            Query.DBExecQuery exec = Query.DBQuery.Exec(routine.Schema, routine.Name);
            foreach (DBSchemaParameter p in routine.Parameters)
            {
                Query.DBParam qp = Query.DBParam.Param(p.InvariantName, p.DbType, p.Direction);
                exec.WithParam(qp);
            }

            using (DbCommand cmd = this.Database.CreateCommand(con, exec))
            {

                using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                {
                    DataTable dt = reader.GetSchemaTable();
                    return dt;
                }
            }
        }
    }
}
