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
    /// <summary>
    /// Schema provider for the System.Data.SqlClient database engine
    /// </summary>
    public class DBSQLClientSchemaProvider : DBSchemaProvider
    {
        private const bool DebugWriteSchemaOutput = false;

        //
        // .ctor
        //       

        #region public DBSQLClientSchemaProvider(DBDatabase database, DBDatabaseProperties properties)

        /// <summary>
        /// Creates a new instance of the schema provider for an SqlClient database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="properties"></param>
        public DBSQLClientSchemaProvider(DBDatabase database, DBDatabaseProperties properties)
            : base(database, properties)
        {
        }

        #endregion

        //
        // overrides
        //

        #region protected override void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)

        protected override void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)
        {
            //Cannot get SqlClient to limit the returned indexes to a single table so
            //we need to load all of them and then restrict to the required table.
            List<DBSchemaItemRef> all = new List<DBSchemaItemRef>();
            base.LoadIndexRefs(con, all);

            foreach (DBSchemaItemRef iref in all)
            {
                if (iref.Container != null && iref.Container.Equals(fortable))
                {
                    intoCollection.Add(iref);
                    iref.Container = fortable;
                }
            }
        }

        #endregion

        #region protected override void LoadTableRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)

        /// <summary>
        /// Overriden because default behaviour for the Get Tables collection returns views too.
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected override void LoadTableRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection tblcollection = this.AssertGetCollectionForType(DBMetaDataCollectionType.Tables);
            DataTable data = this.GetCollectionData(con, tblcollection);

            DataColumn catalogcol = GetColumn(data, "TABLE_CATALOG", true);
            DataColumn schemacol = GetColumn(data, "TABLE_SCHEMA", true);
            DataColumn namecol = GetColumn(data, "TABLE_NAME", true);
            DataColumn typecol = GetColumn(data,"TABLE_TYPE",true);

            foreach (DataRow row in data.Rows)
            {
                if (GetColumnStringValue(row, typecol).Equals("BASE TABLE", StringComparison.OrdinalIgnoreCase))
                {
                    DBSchemaItemRef tbl = this.LoadAnItemRef(row, catalogcol, schemacol, namecol, DBSchemaTypes.Table);
                    if (null != tbl)
                        intoCollection.Add(tbl);
                }
            }

        }

        #endregion

        #region protected override Get/Fill ViewColums

        protected override DataTable GetViewColumns(DbConnection con, DBSchemaItemRef vref)
        {
            string sql = "SELECT * FROM [" + vref.FullName.Replace(".", "].[") + "] WHERE 1=0";
            using (DbCommand cmd = this.Database.CreateCommand(con, sql))
            {
                using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly))
                {
                    DataTable dt = reader.GetSchemaTable();
                    dt.WriteXml("C:\\SchemaOutput\\ViewColumns_" + vref.Name.Replace(" ", "_") + ".xml");
                    return dt;
                }
            }
        }

        protected override void FillViewColumns(DBSchemaViewColumnCollection aview, DataTable dtColumns)
        {
            DataColumn autoinc = GetColumn(dtColumns, "IsAutoIncrement", true);
            DataColumn colname = GetColumn(dtColumns, "ColumnName", true);
            DataColumn ispk = GetColumn(dtColumns, "IsKey", true);
            DataColumn isRO = GetColumn(dtColumns, "IsReadOnly", true);
            DataColumn type = GetColumn(dtColumns, "DataType", true);
            DataColumn allownull = GetColumn(dtColumns, "AllowDBNull", true);
            DataColumn pos = GetColumn(dtColumns, "ColumnOrdinal", true);
            DataColumn size = GetColumn(dtColumns, "ColumnSize", true);
            DataColumn dataType = GetColumn(dtColumns, "DataTypeName", true);

            foreach (DataRow coldef in dtColumns.Rows)
            {
                DBSchemaViewColumn tcol = new DBSchemaViewColumn();
                tcol.Name = GetColumnStringValue(coldef, colname);
                tcol.HasDefault = false;
                tcol.Nullable = GetColumnBoolValue(coldef, allownull);
                tcol.OrdinalPosition = GetColumnIntValue(coldef, pos);
                tcol.ReadOnly = GetColumnBoolValue(coldef, isRO);
                tcol.Size = GetColumnIntValue(coldef, size);
                tcol.Type = Type.GetType(GetColumnStringValue(coldef, type));
                tcol.DbType = this.GetDbTypeForNativeType(GetColumnStringValue(coldef, dataType));

                aview.Add(tcol);
            }
        }

        #endregion

        #region protected override Get/Fill TableColumns

        protected override DataTable GetTableColumns(DbConnection con, DBSchemaItemRef tableref)
        {
           
            string sql = "SELECT * FROM [" + tableref.FullName.Replace(".","].[") + "] WHERE 1=0";

            using (DbCommand cmd = this.Database.CreateCommand(con,sql))
            {
                using (DbDataReader reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly))
                {
                    DataTable dt = reader.GetSchemaTable();
                    dt.WriteXml("C:\\SchemaOutput\\" + tableref.Name + ".xml");
                    dt.AcceptChanges();
                    return dt;
                }
            }
        }


        protected override void FillTableColumns(DBSchemaTableColumnCollection atablecolumns, DataTable dtColumns)
        {
            DataColumn autoinc = GetColumn(dtColumns, "IsAutoIncrement", true);
            DataColumn colname = GetColumn(dtColumns, "ColumnName", true);
            DataColumn ispk = GetColumn(dtColumns, "IsKey", true);
            DataColumn isRO = GetColumn(dtColumns, "IsReadOnly", true);
            DataColumn type = GetColumn(dtColumns, "DataType", true);
            DataColumn allownull = GetColumn(dtColumns, "AllowDBNull", true);
            DataColumn pos = GetColumn(dtColumns, "ColumnOrdinal", true);
            DataColumn size = GetColumn(dtColumns, "ColumnSize", true);
            DataColumn dataType = GetColumn(dtColumns, "DataTypeName", true);

            foreach (DataRow coldef in dtColumns.Rows)
            {
                DBSchemaTableColumn tcol = new DBSchemaTableColumn();
                tcol.Name = GetColumnStringValue(coldef, colname);
                tcol.AutoAssign = GetColumnBoolValue(coldef, autoinc);
                tcol.HasDefault = false;
                tcol.Nullable = GetColumnBoolValue(coldef, allownull);
                tcol.OrdinalPosition = GetColumnIntValue(coldef, pos);
                tcol.PrimaryKey = GetColumnBoolValue(coldef, ispk);
                tcol.ReadOnly = GetColumnBoolValue(coldef, isRO);
                tcol.Size = GetColumnIntValue(coldef, size);
                tcol.Type = Type.GetType(GetColumnStringValue(coldef, type));
                tcol.DbType = this.GetDbTypeForNativeType(GetColumnStringValue(coldef, dataType));

                atablecolumns.Add(tcol);
            }
        }

        #endregion

        #region protected override Get/Fill ForeignKeyColumns

        protected override DataTable GetForeignKeyColumns(DbConnection con, DBSchemaItemRef fkref)
        {
            string sql = @"SELECT f.name AS ForeignKeyName,   
   OBJECT_NAME(f.parent_object_id) AS FKTableName, 
   OBJECT_SCHEMA_NAME(f.parent_object_id) AS FKTableSchema,
   DB_NAME() AS FKTableCatalog,
   COL_NAME(fc.parent_object_id, fc.parent_column_id) AS FKColumnName, 
   OBJECT_NAME (f.referenced_object_id) AS PKTableName, 
   OBJECT_SCHEMA_NAME(f.referenced_object_id) AS PKTableSchema,
   DB_NAME() AS PKTableCatalog,
   COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS PKColumnName 
FROM sys.foreign_keys AS f 
INNER JOIN sys.foreign_key_columns AS fc 
   ON f.OBJECT_ID = fc.constraint_object_id
WHERE f.name LIKE @name";
            DataSet ds = new DataSet();
            DataTable tbl = ds.Tables.Add("ForeignKeys");

            using (DbCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = sql;
                this.Database.AddCommandParameter(cmd, "name", DbType.String).Value = fkref.Name;
                this.Database.PopulateDataSet(ds, cmd, LoadOption.OverwriteChanges,"ForeignKeys");
            }

            return ds.Tables["ForeignKeys"];
        }

        protected override void FillForeignKeyColumns(DBSchemaForeignKey anFk, DataTable dtColumns)
        {
            DataColumn fktblcol = GetColumn(dtColumns, "FKTableName", true);
            DataColumn fkschcol = GetColumn(dtColumns, "FKTableSchema", true);
            DataColumn fkcatcol = GetColumn(dtColumns, "FKTableCatalog", true);
            DataColumn fkcolumn = GetColumn(dtColumns, "FKColumnName", true);

            DataColumn pktblcol = GetColumn(dtColumns, "PKTableName", true);
            DataColumn pkschcol = GetColumn(dtColumns, "PKTableSchema", true);
            DataColumn pkcatcol = GetColumn(dtColumns, "PKTableCatalog", true);
            DataColumn pkcolumn = GetColumn(dtColumns, "PKColumnName", true);

            bool first = true;
            foreach (DataRow row in dtColumns.Rows)
            {
                if (first)
                {
                    first = false;
                    //populate the references to the foreign key tables and primary key tables
                    DBSchemaItemRef fktbl = new DBSchemaItemRef(DBSchemaTypes.Table,
                        GetColumnStringValue(row, fkcatcol), GetColumnStringValue(row, fkschcol), GetColumnStringValue(row, fktblcol));
                    anFk.ForeignKeyTable = fktbl;

                    DBSchemaItemRef pktbl = new DBSchemaItemRef(DBSchemaTypes.Table,
                        GetColumnStringValue(row, pkcatcol), GetColumnStringValue(row, pkschcol), GetColumnStringValue(row, pktblcol));
                    anFk.PrimaryKeyTable = pktbl;
                                        
                }
                DBSchemaForeignKeyMapping map = new DBSchemaForeignKeyMapping();
                map.ForeignColumn = GetColumnStringValue(row, fkcolumn);
                map.PrimaryColumn = GetColumnStringValue(row, pkcolumn);

                anFk.Mappings.Add(map);
            }

        }

        #endregion

    }
}
