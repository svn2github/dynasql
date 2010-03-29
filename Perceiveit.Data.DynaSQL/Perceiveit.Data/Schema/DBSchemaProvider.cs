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
using System.Data;
using System.Data.Common;
using System.Text;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.Schema
{
    public abstract partial class DBSchemaProvider
    {
        #region const

        protected const string SchemaTablesName = "TABLES";
        protected const string SchemaViewsName = "VIEWS";
        protected const string SchemaViewColumnsName = "VIEWCOLUMNS";
        protected const string SchemaIndexesName = "INDEXES";
        protected const string SchemaProceduresName = "PROCEDURES";
        protected const string SchemaColumnsName = "COLUMNS";
        protected const string SchemaIndexColumnsName = "INDEXCOLUMNS";

        #endregion


        #region ivars

        private DBDatabase _db;
        private DBDatabaseProperties _props;

        #endregion

        
        //
        // public properties
        //

        #region public DBDataBase Database {get; protected set;}

        /// <summary>
        /// Gets the DBDatabase this 
        /// schema provider is referring to
        /// </summary>
        public DBDatabase Database 
        {
            get { return this._db; }
            protected set { this._db = value; }
        }

        #endregion

        #region public DBDatabaseProperties Properties {get; protected set;}

        /// <summary>
        /// Gets the properties 
        /// </summary>
        public DBDatabaseProperties Properties
        {
            get { return this._props; }
            protected set { this._props = value; }
        }

        #endregion

        #region public virtual DBSchemaTypes SupportedSchemaTypes
        
        /// <summary>
        /// Gets the Supported schema types for this DbSchemaProvider
        /// </summary>
        public virtual DBSchemaTypes SupportedSchemaTypes
        {
            get { return this.Properties.SupportedSchemas; }
        }

        #endregion

        //
        // .ctor
        //

        #region protected DBSchemaProvider(DBDataBase database, DBDatabaseProperties properties)

        /// <summary>
        /// Creates a new DBSchemaProvider for the specified database
        /// </summary>
        /// <param name="database">The database</param>
        protected DBSchemaProvider(DBDatabase database, DBDatabaseProperties properties)
        {
            if (null == database)
                throw new ArgumentNullException("database");

            if (null == properties)
                throw new ArgumentNullException("properties");

            this.Database = database;
            this.Properties = properties;
        }

        #endregion



        

        //
        // GetIndexSchema
        //

        #region public DBSchemaIndex GetIndexSchema(string name + DBSchemaItemRef belongsToTable) + 2 overloads

        public DBSchemaIndex GetIndexSchema(string name, DBSchemaItemRef belongsToTable)
        {
            return this.GetIndexSchema(string.Empty, string.Empty, name, belongsToTable);
        }

        public DBSchemaIndex GetIndexSchema(string catalog, string schema, string name, DBSchemaItemRef belongsToTable)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            DBSchemaItemRef itemref = new DBSchemaItemRef();
            itemref.Catalog = catalog;
            itemref.Schema = schema;
            itemref.Name = name;
            itemref.Type = DBSchemaTypes.Index;
            itemref.Container = belongsToTable;
            return GetSchema(itemref) as DBSchemaIndex;
        }

        public DBSchemaIndex GetIndexSchema(DBSchemaItemRef indexRef)
        {
            if (null == indexRef)
                throw new ArgumentNullException("indexRef");
            if (string.IsNullOrEmpty(indexRef.Name))
                throw new ArgumentNullException("indexRef.Name");
            if (indexRef.Type != DBSchemaTypes.Index)
                throw new ArgumentOutOfRangeException("indexRef.Type");

            throw new NotImplementedException(this.GetType().FullName + ".GetIndexSchema");
        }

        #endregion

        //
        // GetStoredProcedureSchema
        //

        #region public DBSchemaSproc GetStoredProcedureSchema(string catalog, string schema, string name)

        public DBSchemaSproc GetStoredProcedureSchema(string name)
        {
            return this.GetStoredProcedureSchema(string.Empty, string.Empty, name);
        }

        public DBSchemaSproc GetStoredProcedureSchema(string catalog, string schema, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            DBSchemaItemRef itemref = new DBSchemaItemRef();
            itemref.Catalog = catalog;
            itemref.Schema = schema;
            itemref.Name = name;
            itemref.Type = DBSchemaTypes.StoredProcedure;

            return this.GetStoredProcedureSchema(itemref);
        }

        public DBSchemaSproc GetStoredProcedureSchema(DBSchemaItemRef sprocRef)
        {
            if (null == sprocRef)
                throw new ArgumentNullException("sprocRef");
            if (string.IsNullOrEmpty(sprocRef.Name))
                throw new ArgumentNullException("sprocRef.Name");
            if (sprocRef.Type != DBSchemaTypes.StoredProcedure)
                throw new ArgumentOutOfRangeException("sprocRef.Type");

            throw new NotImplementedException(this.GetType().FullName + ".GetStoredProcedureSchema");
        }

        #endregion

        //
        // GetFunctionSchema
        //

        #region public DBSchemaFunction GetFunctionSchema(string catalog, string owner, string name)+ 1 overload

        public DBSchemaFunction GetFunctionSchema(string name)
        {
            return this.GetFunctionSchema(string.Empty, string.Empty, name);
        }

        public DBSchemaFunction GetFunctionSchema(string catalog, string owner, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            DBSchemaItemRef itemref = new DBSchemaItemRef();
            itemref.Catalog = catalog;
            itemref.Schema = owner;
            itemref.Name = name;
            itemref.Type = DBSchemaTypes.Function;

            return GetFunctionSchema(itemref);
        }

        public DBSchemaFunction GetFunctionSchema(DBSchemaItemRef funcRef)
        {
            if (null == funcRef)
                throw new ArgumentNullException("funcRef");
            if (string.IsNullOrEmpty(funcRef.Name))
                throw new ArgumentNullException("funcRef.Name");
            if (funcRef.Type != DBSchemaTypes.Function)
                throw new ArgumentOutOfRangeException("funcRef.Type");

            throw new NotImplementedException(this.GetType().FullName + ".GetFunctionSchema");
            
        }

        #endregion

        //
        // GetSchema
        //

        #region public DBSchemaItem GetSchema(DBSchemaItemRef schemaRef) + 1 overload

        /// <summary>
        /// Gets the SchemaItem for the specified reference
        /// </summary>
        /// <param name="schemaRef"></param>
        /// <returns></returns>
        public DBSchemaItem GetSchema(DBSchemaItemRef schemaRef)
        {
            if (null == schemaRef)
                throw new ArgumentNullException("schemRef");
            if (schemaRef.Type == DBSchemaTypes.Table)
                return GetTableSchema(schemaRef);
            else if (schemaRef.Type == DBSchemaTypes.View)
                return GetViewSchema(schemaRef);
            else if (schemaRef.Type == DBSchemaTypes.Index)
                return GetIndexSchema(schemaRef);
            else if (schemaRef.Type == DBSchemaTypes.StoredProcedure)
                return GetStoredProcedureSchema(schemaRef);
            else if (schemaRef.Type == DBSchemaTypes.Function)
                return GetFunctionSchema(schemaRef);
            else
                throw new ArgumentOutOfRangeException("schemaRef.Type");
        }

        
        #endregion

        //
        //GetSchemaItems
        //

        #region public IEnumerable<DBSchemaItemRef> GetSchemaItems(DBSchemaTypes types) + 1 overload

        public IEnumerable<DBSchemaItemRef> GetSchemaItems()
        {
            return this.DoGetSchemaItems(Perceiveit.Data.Query.SupportedSchemaOptions.All);
        }

        public IEnumerable<DBSchemaItemRef> GetSchemaItems(DBSchemaTypes types)
        {
            return this.DoGetSchemaItems(types);
        }

        #endregion

        //
        //CheckXXXExists
        //

        #region public bool CheckTableExists(string name) + 1 overload

        public bool CheckTableExists(string name)
        {
            return this.CheckTableExists(null, null, name);
        }

        public bool CheckTableExists(string catalog, string schema, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return CheckExists(new DBSchemaItemRef(DBSchemaTypes.Table, catalog, schema, name));
        }

        #endregion

        #region public bool CheckViewExists(string name) + 1 overload

        public bool CheckViewExists(string name)
        {
            return this.CheckViewExists(null, null, name);
        }

        public bool CheckViewExists(string catalog, string schema, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return CheckExists(new DBSchemaItemRef(DBSchemaTypes.View, catalog, schema, name));
        }

        #endregion

        #region public bool CheckStoredProcedureExists(string name) + 1 overload

        public bool CheckStoredProcedureExists(string name)
        {
            return this.CheckStoredProcedureExists(null, null, name);
        }

        public bool CheckStoredProcedureExists(string catalog, string schema, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return this.CheckExists(new DBSchemaItemRef(DBSchemaTypes.StoredProcedure, catalog, schema, name));
        }

        #endregion

        #region public bool CheckIndexExists(string name, DBSchemaItemRef belongsToTable) + 1 overload

        public bool CheckIndexExists(string name, DBSchemaItemRef belongsToTable)
        {
            return this.CheckIndexExists(null, null, name, belongsToTable);
        }


        public bool CheckIndexExists(string catalog, string schema, string name, DBSchemaItemRef belongsToTable)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            DBSchemaItemRef ix = new DBSchemaItemRef(DBSchemaTypes.Index, catalog, schema, name);
            ix.Container = belongsToTable;
            return this.CheckExists(ix);
        }

        #endregion

        #region public bool CheckExists(DBSchemaItemRef schemaRef)

        
        /// <summary>
        /// Checks the database connection to see if the specified schemaRef item exists, and returns true if it was found.
        /// </summary>
        /// <param name="schemaRef"></param>
        /// <returns></returns>
        public bool CheckExists(DBSchemaItemRef schemaRef)
        {
            if (null == schemaRef)
                throw new ArgumentNullException("schemaRef");

            return this.DoCheckExists(schemaRef);
        }

        #endregion

        //
        // DropXXX
        //

        #region public void DropTable(string tableName) + 1 overload

        public void DropTable(string tableName)
        {
            this.DropTable(null, null, tableName);
        }

        public void DropTable(string catalog, string schema, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            
            DBSchemaItemRef itemref = new DBSchemaItemRef(DBSchemaTypes.Table, catalog, schema, name);
            this.DropSchemaItem(itemref);
        }

        #endregion

        #region public void DropView(string viewName) + 1 overload

        public void DropView(string viewName)
        {
            this.DropView(null, null, viewName);
        }

        public void DropView(string catalog, string schema, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            
            DBSchemaItemRef iref = new DBSchemaItemRef(DBSchemaTypes.View, catalog, schema, name);
            this.DropSchemaItem(iref);
        }
        
        #endregion

        #region public void DropStoredProcedure(string name) + 1 overload

        public void DropStoredProcedure(string name)
        {
            this.DropStoredProcedure(null, null, name);
        }

        public void DropStoredProcedure(string catalog, string schema, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            
            DBSchemaItemRef iref = new DBSchemaItemRef(DBSchemaTypes.StoredProcedure, catalog, schema, name);
            this.DropSchemaItem(iref);
        }

        #endregion

        #region public void DropIndex(string name) + 1 overload

        public void DropIndex(string name)
        {
            this.DropIndex(null, null, name);
        }

        public void DropIndex(string catalog, string schema, string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            
            DBSchemaItemRef iref = new DBSchemaItemRef(DBSchemaTypes.Index, catalog, schema, name);
            this.DropSchemaItem(iref);
        }

        #endregion

        #region public void DropSchemaItem(DBSchemaItemRef itemRef)

        public void DropSchemaItem(DBSchemaItemRef itemRef)
        {
            if (null == itemRef)
                throw new ArgumentNullException("itemRef");
            
            if (string.IsNullOrEmpty(itemRef.Name))
                throw new ArgumentNullException("itemRef.Name");

            string sql = this.GenerateDropScript(itemRef);

            this.ExecuteScript(sql);
        }

        #endregion

        //
        // CreateXXX
        //

        #region public void CreateTable(DBSchemaTable tbl)

        public void CreateTable(DBSchemaTable tbl)
        {
            if (null == tbl)
                throw new ArgumentNullException("tbl");
            string sql = this.GenerateCreateScript(DBSchemaTypes.Table, tbl, null);

            this.ExecuteScript(sql);
        }

        #endregion

        #region public void CreateIndex(DBSchemaIndex index)

        public void CreateIndex(DBSchemaIndex index)
        {
            if (null == index)
                throw new ArgumentNullException("index");
            string sql = this.GenerateCreateScript(DBSchemaTypes.Index, index, null);

            this.ExecuteScript(sql);
        }

        #endregion

        #region public void CreateView(DBSchemaView view, DBSelectQuery script)

        public void CreateView(DBSchemaView view, DBSelectQuery script)
        {
            if (null == view)
                throw new ArgumentNullException("view");
            if (null == script)
                throw new ArgumentNullException("script");

            string sql = this.GenerateCreateScript(DBSchemaTypes.View, view, script);

            this.ExecuteScript(sql);
        }

        #endregion

        #region public void CreateStoredProcedure(DBSchemaSproc sproc, DBScript script)

        public void CreateStoredProcedure(DBSchemaSproc sproc, DBScript script)
        {
            if (null == sproc)
                throw new ArgumentNullException("sproc");
            if (null == script)
                throw new ArgumentNullException("script");

            string sql = this.GenerateCreateScript(DBSchemaTypes.StoredProcedure, sproc, script);

            this.ExecuteScript(sql);
        }

        #endregion

        #region public void CreateFunction(DBSchemaSproc sproc, DBScript script)

        public void CreateFunction(DBSchemaSproc sproc, DBScript script)
        {
            if (null == sproc)
                throw new ArgumentNullException("sproc");
            if (null == script)
                throw new ArgumentNullException("script");

            string sql = this.GenerateCreateScript(DBSchemaTypes.Function, sproc, script);

            this.ExecuteScript(sql);
        }

        #endregion


        //
        // support methods
        //


        /// <summary>
        /// Checks the tomatch to see if it includes the tocompare flag
        /// </summary>
        #region protected bool IsSchemaType(DBSchemaTypes tomatch, DBSchemaTypes tocompare)

        /// <param name="tomatch"></param>
        /// <param name="tocompare"></param>
        /// <returns></returns>
        protected bool IsSchemaType(DBSchemaTypes tomatch, DBSchemaTypes tocompare)
        {
            return (tocompare & tomatch) > 0;
        }

        #endregion

        #region protected virtual object ExecuteScript(string sql)

        /// <summary>
        /// Executes the specified sqlstatement agains this SchemaProviders database
        /// </summary>
        /// <param name="sql"></param>
        protected virtual object ExecuteScript(string sql)
        {
            return this.Database.ExecuteNonQuery(sql, System.Data.CommandType.Text);
        }

        #endregion

        #region protected string GenerateCreateScript(DBSchemaTypes type, DBSchemaItem schema, DBQuery script)
        /// <summary>
        /// Generates a CREATE XXX sql script using the  provider specific 
        /// StatementBuilder and a this instances DoGenerateCreateScript method
        /// </summary>
        /// <param name="type">The type of script to create</param>
        /// <param name="schema">The schema to create</param>
        /// <param name="script">An associated DBQuery if any for the schema item</param>
        /// <returns>The SQL to execute which will Create the requested schema item</returns>
        protected string GenerateCreateScript(DBSchemaTypes type, DBSchemaItem schema, DBQuery script)
        {
            using (DBStatementBuilder sb = DBFactory.GetFactory(this.Database.ProviderName).CreateStatementBuilder(this.Database))
            {
                if (null == sb)
                    throw new NullReferenceException(string.Format(Errors.CannotCreateStatementBuilderForProvider, this.Database.ProviderName));

                return this.DoGenerateCreateScript(type, schema, script, sb);
            }
        }

        #endregion

        #region protected string GenerateDropScript(DBSchemaItemRef itemRef)

        /// <summary>
        /// Generates the DROP XXX sql script using the provider specific SqlStatementBuilder
        /// and  this instances DoGenerateDropScript method
        /// </summary>
        /// <param name="itemRef">A reference to the Item to DROP</param>
        /// <returns>The SQL to execute</returns>
        protected string GenerateDropScript(DBSchemaItemRef itemRef)
        {
            using (DBStatementBuilder sb = DBFactory.GetFactory(this.Database.ProviderName).CreateStatementBuilder(this.Database))
            {
                if (null == sb)
                    throw new NullReferenceException(string.Format(Errors.CannotCreateStatementBuilderForProvider, this.Database.ProviderName));

                return this.DoGenerateDropScript(itemRef, sb);
            }
        }
        #endregion

        #region protected DataColumn GetColumn(DataTable table, string columnName, bool required)

        protected DataColumn GetColumn(DataTable table, string columnName, bool required)
        {
            DataColumn column;
            if (string.IsNullOrEmpty(columnName))
                column = null;
            else
                column = table.Columns[columnName];

            if (required && null == column)
                throw new DBSchemaProviderException(string.Format(Errors.SchemaColumnNotFound, columnName));
            return column;
        }

        #endregion

        #region protected string GetColumnValue(DataRow row, DataColumn column)

        protected string GetColumnValue(DataRow row, DataColumn column)
        {
            if (column == null)
            {
                return string.Empty;
            }
            else
            {
                return row[column].ToString();
            }
        }

        #endregion

        //
        // abstract methods
        //

        protected abstract IEnumerable<DBSchemaItemRef> DoGetSchemaItems(DBSchemaTypes types);

        protected abstract bool DoCheckExists(DBSchemaItemRef itemRef);

        protected abstract string DoGenerateDropScript(DBSchemaItemRef itemRef, DBStatementBuilder builder);

        protected abstract string DoGenerateCreateScript(DBSchemaTypes type, DBSchemaItem schema, DBQuery script, DBStatementBuilder builder);

        protected abstract DbType GetDbTypeForSqlType(string sqlType);
        
    }
}
