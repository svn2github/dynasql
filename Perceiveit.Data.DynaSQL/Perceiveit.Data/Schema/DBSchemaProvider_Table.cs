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

namespace Perceiveit.Data.Schema
{
    //
    // Table methods
    //

    public abstract partial class DBSchemaProvider
    {

        //
        // inner classes
        //

        #region protected class TableMappingClass

        /// <summary>
        /// A mapping class that references the names of the SchemaTable columns
        /// used to derrive a SchemaTable
        /// </summary>
        /// <remarks>
        /// Inheritors can either override the 
        /// GetTableMapping method of the DBSchemaProvider class to change the names,
        /// or create a subclass and initialise their own values
        /// </remarks>
        protected class TableMappingClass
        {
            private string _tblnamecol, _tblshcemacol, _tblcatalogcol, _tbltypecol;

            public string TableNameColumn { get { return _tblnamecol; } set { _tblnamecol = value; } }
            public string TableSchemaColumn { get { return _tblshcemacol; } set { _tblshcemacol = value; } }
            public string TableCatalogColumn { get { return _tblcatalogcol; } set { _tblcatalogcol = value; } }
            public string TableTypeColumn { get { return _tbltypecol; } set { _tbltypecol = value; } }


            public TableMappingClass()
            {
                this.TableNameColumn = "TABLE_NAME";
                this.TableSchemaColumn = "TABLE_SCHEMA";
                this.TableCatalogColumn = "TABLE_CATALOG";
                this.TableTypeColumn = "TABLE_TYPE";
            }

        }

        #endregion

        #region protected class TableColumnMappingClass

        /// <summary>
        /// A mapping class that references the names of the SchemaColumnTable columns
        /// used to derrive a SchemaTableColumn.</summary>
        /// <remarks>
        /// Inheritors can either override the 
        /// GetTableColumnMapping method of the DBSchemaProvider class to change the names,
        /// or create a subclass and initialise their own values
        /// </remarks>
        protected class TableColumnMappingClass
        {
            private string _tblnamecol, _tblschemacol, _tblcatalogcol, _colnamecol, _ordcol,
                           _defvalcol, _isnulllcol, _datatypecol, _maxcharlencol, _autonumcol, _pkcol;

            public string TableNameColumn { get { return _tblnamecol; } set { _tblnamecol = value; } }
            public string TableSchemaColumn { get { return _tblschemacol; } set { _tblschemacol = value; } }
            public string TableCatalogColumn { get { return _tblcatalogcol; } set { _tblcatalogcol = value; } }
            public string ColumnNameColumn { get { return _colnamecol; } set { _colnamecol = value; } }
            public string OrdinalPositionColumn { get { return _ordcol; } set { _ordcol = value; } }
            public string DefaultValueColumn { get { return _defvalcol; } set { _defvalcol = value; } }
            public string IsNullableColumn { get { return _isnulllcol; } set { _isnulllcol = value; } }
            public string DataTypeColumn { get { return _datatypecol; } set { _datatypecol = value; } }
            public string MaxCharacterLengthColumn { get { return _maxcharlencol; } set { _maxcharlencol = value; } }
            public string AutoNumberColumn { get { return _autonumcol; } set { _autonumcol = value; } }
            public string PrimaryKeyColumn { get { return _pkcol; } set { _pkcol = value; } }

            public TableColumnMappingClass()
            {
                TableNameColumn = "TABLE_NAME";
                TableSchemaColumn = "TABLE_SCHEMA";
                TableCatalogColumn = "TABLE_CATALOG";
                ColumnNameColumn = "COLUMN_NAME";
                OrdinalPositionColumn = "ORDINAL_POSITION";
                DefaultValueColumn = "COLUMN_DEFAULT";
                IsNullableColumn = "IS_NULLABLE";
                DataTypeColumn = "DATA_TYPE";
                MaxCharacterLengthColumn = "CHARACTER_MAXIMUM_LENGTH";
                AutoNumberColumn = "AUTOINCREMENT";
                PrimaryKeyColumn = "PRIMARY_KEY";
            }
        }

        #endregion

        //
        // public methods
        //

        #region public DBSchemaTable GetTableSchema(string catalog, string owner, string name) + 2 overloads

        /// <summary>
        /// Loads and returns the Schema for the specified table
        /// </summary>
        /// <param name="name">The required name of the table to get teh schema for</param>
        /// <returns></returns>
        public DBSchemaTable GetTableSchema(string name)
        {
            return this.GetTableSchema(string.Empty, string.Empty, name);
        }

        /// <summary>
        /// Loads and returns the Schema for the specified table
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="schema"></param>
        /// <param name="name">The required name of the table to get the schema for. Cannot be null or empty</param>
        /// <returns></returns>
        public DBSchemaTable GetTableSchema(string catalog, string schema, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            DBSchemaItemRef itemref = new DBSchemaItemRef();
            itemref.Catalog = catalog;
            itemref.Schema = schema;
            itemref.Name = name;
            itemref.Type = DBSchemaTypes.Table;

            return this.GetTableSchema(itemref);

        }

        public DBSchemaTable GetTableSchema(DBSchemaItemRef tableRef)
        {
            if (null == tableRef)
                throw new ArgumentNullException("tableRef");
            if (string.IsNullOrEmpty(tableRef.Name))
                throw new ArgumentNullException("tableRef.Name");
            if (tableRef.Type != DBSchemaTypes.Table)
                throw new ArgumentOutOfRangeException("tableRef.Type");

            using (DbConnection con = this.Database.CreateConnection())
            {
                con.Open();
                return this.LoadATable(con, tableRef);
            }
        }

        #endregion


        //
        // protected implementation
        //

        /// <summary>
        /// Loads a complete schema for the specified Table schema reference including all columns, and indexes.
        /// </summary>
        #region protected virtual DBSchemaTable LoadATable(DbConnection con, DBSchemaItemRef forRef)

        /// <param name="con"></param>
        /// <param name="forRef"></param>
        /// <returns></returns>
        protected virtual DBSchemaTable LoadATable(DbConnection con, DBSchemaItemRef forRef)
        {
            DBSchemaTable sTbl = null;
            string catalog = string.IsNullOrEmpty(forRef.Catalog) ? null : forRef.Catalog;
            string schema = string.IsNullOrEmpty(forRef.Schema) ? null : forRef.Schema;

            DataTable tbl = con.GetSchema(SchemaTablesName, new string[] { catalog, schema, forRef.Name, null });
            DataTable columns = con.GetSchema(SchemaColumnsName, new string[] { catalog, schema, forRef.Name, null });
            
            if (tbl.Rows.Count > 0)
            {
                DataRow tblRow = tbl.Rows[0];
                TableMappingClass tblmapping = this.GetTableMapping();
                TableColumnMappingClass tblcolmapping = this.GetTableColumnMapping();

                sTbl = CreateSchemaTable(tblmapping, tblRow);

                FillTableColumns(sTbl, tblcolmapping, columns);
            }


            return sTbl;

        }

        #endregion

        /// <summary>
        /// Creates a new DBSchemaTable for a returned GetSchema DbTable definition
        /// </summary>
        #region protected virtual DBSchemaTable CreateSchemaTable(TableMappingClass mapping, DataRow tblRow)

        /// <param name="tblRow"></param>
        /// <returns></returns>
        protected virtual DBSchemaTable CreateSchemaTable(TableMappingClass mapping, DataRow tblRow)
        {
            DBSchemaTable sTbl = new DBSchemaTable();

            DataColumn col = GetColumn(tblRow.Table, mapping.TableCatalogColumn, false);
            sTbl.Catalog = this.GetColumnValue(tblRow, col);

            col = GetColumn(tblRow.Table, mapping.TableSchemaColumn, false);
            sTbl.Schema = this.GetColumnValue(tblRow,col);

            col = GetColumn(tblRow.Table, mapping.TableNameColumn, false);
            sTbl.Name = this.GetColumnValue(tblRow, col);

            return sTbl;
        }

        #endregion

        /// <summary>
        /// Creates and populates the DBSchemaTable.Columns property based upon a returned DataTable for the GetSchema("Columns") query
        /// </summary>
        #region private void FillTableColumns(DBSchemaTable sTbl,  DataTable columns)

        /// <param name="sTbl"></param>
        /// <param name="columns"></param>
        protected virtual void FillTableColumns(DBSchemaTable sTbl, TableColumnMappingClass mapping, DataTable columns)
        {
            DataColumn colCatalog = this.GetColumn(columns, mapping.TableCatalogColumn, false);
            DataColumn colSchema = this.GetColumn(columns, mapping.TableSchemaColumn, false);
            DataColumn colTable = this.GetColumn(columns, mapping.TableNameColumn, true);
            DataColumn colName = this.GetColumn(columns, mapping.ColumnNameColumn, true);
            DataColumn colDefault = this.GetColumn(columns, mapping.DefaultValueColumn, false);
            DataColumn colType = this.GetColumn(columns, mapping.DataTypeColumn, true);
            DataColumn colMaxLen = this.GetColumn(columns, mapping.MaxCharacterLengthColumn, false);
            DataColumn colAuto = this.GetColumn(columns, mapping.AutoNumberColumn, false);
            DataColumn colNull = this.GetColumn(columns, mapping.IsNullableColumn, false);
            DataColumn colOrdinal = this.GetColumn(columns, mapping.OrdinalPositionColumn, false);
            DataColumn colPK = this.GetColumn(columns, mapping.PrimaryKeyColumn, false);

            foreach (DataRow colRow in columns.Rows)
            {
                if (sTbl.Name == colRow[colTable].ToString())
                {
                    DBSchemaTableColumn column = new DBSchemaTableColumn();
                    column.Name = GetColumnValue(colRow, colName);

                    if (string.IsNullOrEmpty(column.Name))
                        throw new DBSchemaProviderException(string.Format(Errors.SchemaColumnCannotBeEmpty, mapping.ColumnNameColumn));

                    string type = GetColumnValue(colRow, colType);
                    string len = GetColumnValue(colRow, colMaxLen);
                    string auto = GetColumnValue(colRow, colAuto);
                    string ord = GetColumnValue(colRow, colOrdinal);
                    string nullable = GetColumnValue(colRow, colNull);
                    string def = GetColumnValue(colRow, colDefault);
                    string pk = GetColumnValue(colRow, colPK);
                    int i;
                    bool b;

                    //Set AutoAssign
                    if (auto.IndexOf("auto_increment") > -1)
                        b = true;
                    else if (auto.ToLower() == "yes")
                        b = true;
                    else if (bool.TryParse(auto, out b) == false)
                        b = false;
                    column.AutoAssign = b;

                    //Set DbType
                    column.DbType = GetDbTypeForSqlType(type);

                    //Set DefaultValue and HasDefault
                    column.HasDefault = string.IsNullOrEmpty(def) == false;
                    column.DefaultValue = def;

                    //Set IsNullable
                    if (nullable.ToLower() == "yes")
                        b = true;
                    else if (!bool.TryParse(nullable, out b))
                        b = false;
                    column.Nullable = b;

                    //Set ordinal position
                    if (int.TryParse(ord, out i))
                        column.OrdinalPosition = i;
                    else
                        column.OrdinalPosition = -1;

                    //Set readonly (same as autoassign)
                    column.ReadOnly = column.AutoAssign;

                    //Set column.Type
                    column.Type = DBHelper.GetRuntimeTypeForDbType(column.DbType);

                    //Set Primary Key
                    if (pk.ToLower() == "yes")
                        b = true;
                    else if (!bool.TryParse(pk, out b))
                        b = false;
                    column.PrimaryKey = b;

                    //Set Size
                    if (string.IsNullOrEmpty(len) == false && int.TryParse(len, out i))
                        column.Size = i;
                    else
                        column.Size = -1;

                    
                    sTbl.Columns.Add(column);
                }
            }
        }

        #endregion

        /// <summary>
        /// Virtual method that instantiates the TableMappingClass to identify the names fo the columns returned from
        /// a DbConnections GetSchema() method. So that a DBSchemaTable can be correctly initialized.
        /// </summary>
        #region protected virtual TableMappingClass GetTableMapping()
        /// <returns></returns>
        protected virtual TableMappingClass GetTableMapping()
        {
            return new TableMappingClass();
        }

        #endregion

        /// <summary>
        /// Virtual method that instantiates the TableColumnMapping class to identify the names fo the columns returned from
        /// a DbConnections GetSchema() method. So that a DBSchemaTableColumn can be correctly initialized.
        /// </summary>
        #region protected virtual TableColumnMappingClass GetTableColumnMapping()
        /// <returns>A new fully initialized TableColumnMapping class</returns>
        protected virtual TableColumnMappingClass GetTableColumnMapping()
        {
            return new TableColumnMappingClass();
        }

        #endregion


    }
}
