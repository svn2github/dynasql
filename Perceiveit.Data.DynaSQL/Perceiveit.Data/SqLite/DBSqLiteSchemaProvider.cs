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
using Perceiveit.Data.Schema;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.SqLite
{
    /// <summary>
    /// Loads schema information for an SQLite database
    /// </summary>
    public class DBSqLiteSchemaProvider : DBSchemaProvider
    {
        //
        // .ctor
        //

        internal DBSqLiteSchemaProvider(DBDatabase forDatabase, DBDatabaseProperties properties)
            : base(forDatabase, properties)
        {
        }

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

            this.LoadItemRefsWithContainer(data, intoCollection,
                    catalogcol, schemacol, namecol, DBSchemaTypes.Index,
                    tablecatalogcol, tableschemacol, tablenamecol, DBSchemaTypes.Table);
        }

        #endregion


        /// <summary>
        /// Loads all the indexes in this providers data connection for the specified table
        /// </summary>
        /// <param name="con"></param>
        /// <param name="fortable"></param>
        /// <param name="intoCollection"></param>
        protected override void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)
        {
            IEnumerable<DBSchemaItemRef> ixs = this.GetAllIndexs();
            foreach (DBSchemaItemRef ixref in ixs)
            {
                if (ixref.Container != null && ixref.Container.Equals(fortable))
                    intoCollection.Add(ixref);
            }
        }

        protected override void FillIndexData(DBSchemaIndex anindex, DataRow dataRow)
        {
            DataColumn catalog = GetColumn(dataRow.Table, "index_catalog", false);
            DataColumn schema = GetColumn(dataRow.Table, "index_schema", false);
            DataColumn name = GetColumn(dataRow.Table, "index_name", true);
            //The SQLite Connection GetSchema() method may incorrectly report this as true 
            DataColumn ixPK = GetColumn(dataRow.Table, "primary_key", false);
            DataColumn unique = GetColumn(dataRow.Table, "unique", false);

            DataColumn tblcatalog = GetColumn(dataRow.Table, "table_catalog", false);
            DataColumn tblschema = GetColumn(dataRow.Table, "table_schema", false);
            DataColumn tblname = GetColumn(dataRow.Table, "table_name", true);

            anindex.Catalog = GetColumnStringValue(dataRow, catalog);
            anindex.Schema = GetColumnStringValue(dataRow, schema);
            anindex.Name = GetColumnStringValue(dataRow, name);

            DBSchemaItemRef tblref = new DBSchemaItemRef(DBSchemaTypes.Table,
                GetColumnStringValue(dataRow, tblcatalog), GetColumnStringValue(dataRow, tblschema), GetColumnStringValue(dataRow, tblname));
            anindex.TableReference = tblref;

            anindex.IsPrimaryKey = GetColumnBoolValue(dataRow, ixPK, false);
            anindex.IsUnique = GetColumnBoolValue(dataRow, unique, false);

        }


        protected override DBSchemaForeignKey LoadAForeignKey(DbConnection con, DBSchemaItemRef fkref)
        {
            DataTable dtFK = GetForeignKeyData(con, fkref);
            DBSchemaForeignKey anFk = null;

            if (null != dtFK && dtFK.Rows.Count > 0)
            {
                anFk = new DBSchemaForeignKey();
                this.FillForeignKeyData(anFk, dtFK.Rows[0]);

                
            }
            return anFk;
        }

        protected override void FillForeignKeyData(DBSchemaForeignKey fk, DataRow dtFKRow)
        {
            DataTable tbl = dtFKRow.Table;
            DataColumn catalog = GetColumn(tbl, "CONSTRAINT_CATALOG", false);
            DataColumn schema = GetColumn(tbl, "CONSTRAINT_SCHEMA", false);
            DataColumn name = GetColumn(tbl, "CONSTRAINT_NAME", true);
            
            fk.Catalog = GetColumnStringValue(dtFKRow, catalog);
            fk.Schema = GetColumnStringValue(dtFKRow, schema);
            fk.Name = GetColumnStringValue(dtFKRow, name);

            DataColumn fkFromCatalog = GetColumn(tbl, "TABLE_CATALOG", false);
            DataColumn fkFromSchema = GetColumn(tbl, "TABLE_SCHEMA", false);
            DataColumn fkFromTable = GetColumn(tbl, "TABLE_NAME", false);
            DataColumn fkFromColumn = GetColumn(tbl, "FKEY_FROM_COLUMN", true);

            DataColumn fkToCatalog = GetColumn(tbl, "FKEY_TO_CATALOG", false);
            DataColumn fkToSchema = GetColumn(tbl, "FKEY_TO_SCHEMA", false);
            DataColumn fkToTable = GetColumn(tbl, "FKEY_TO_TABLE", true);
            DataColumn fkToColumn = GetColumn(tbl, "FKEY_TO_COLUMN", true);
            
            fk.ForeignKeyTable = new DBSchemaItemRef(DBSchemaTypes.Table,
                GetColumnStringValue(dtFKRow, fkFromCatalog),
                GetColumnStringValue(dtFKRow, fkFromSchema),
                GetColumnStringValue(dtFKRow, fkFromTable));

            fk.PrimaryKeyTable = new DBSchemaItemRef(DBSchemaTypes.Table,
                GetColumnStringValue(dtFKRow, fkToCatalog),
                GetColumnStringValue(dtFKRow, fkToSchema),
                GetColumnStringValue(dtFKRow, fkToTable));
            
            DBSchemaForeignKeyMapping map = new DBSchemaForeignKeyMapping();
            map.ForeignColumn = GetColumnStringValue(dtFKRow, fkFromColumn);
            map.PrimaryColumn = GetColumnStringValue(dtFKRow, fkToColumn);
            fk.Mappings.Add(map);
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
            DataColumn DataTypeColumn = GetColumn(dtColumns, "EDM_TYPE", true);
            DataColumn MaxCharacterLengthColumn = GetColumn(dtColumns, "CHARACTER_MAXIMUM_LENGTH", false);
            DataColumn AutoNumberColumn = GetColumn(dtColumns, "AUTOINCREMENT", false);
            DataColumn PrimaryKeyColumn = GetColumn(dtColumns, "PRIMARY_KEY", false);
            DataColumn CharacterSetColumn = GetColumn(dtColumns, "CHARACTER_SET_NAME", false);
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
                col.AutoAssign = GetColumnBoolValue(row, AutoNumberColumn);
                col.PrimaryKey = GetColumnBoolValue(row, PrimaryKeyColumn);
                col.ReadOnly = col.AutoAssign;
                col.HasDefault = !string.IsNullOrEmpty(col.DefaultValue);

                atablecolumns.Add(col);
            }

        }

    }
}
