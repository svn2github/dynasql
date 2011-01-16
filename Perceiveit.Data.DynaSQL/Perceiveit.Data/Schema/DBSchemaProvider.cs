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
    /// <summary>
    /// Supports the extraction of schema information from the specified database. 
    /// Use the DBDatabase GetProvider() method to create instances of this class
    /// </summary>
    /// <remarks>An instace of the DBSchemaProvider internally caches retrieved references to all the 
    /// database objects so new tables sprocs etc will not become visible to an 
    /// existing instance of the provider, however new instances will 
    /// always retrieve the latest database schema</remarks>
    public class DBSchemaProvider
    {
        /// <summary>
        /// Flag to output all the received collection data from any call to con.GetSchema()
        /// </summary>
        private const bool OUTPUTCOLLECTIONDATA = true;

        /// <summary>
        /// The path to the directory where the schema data should be stored
        /// </summary>
        private const string OUTPUTDATAPATH = "C:\\SchemaOutput\\";

        #region ivars

        private DBDatabase _db;
        private DBDatabaseProperties _props;
        private DBSchemaMetaDataCollectionSet _collections;
        private Dictionary<DBSchemaTypes, List<DBSchemaItemRef>> _cache;

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

        #region public DBSchemaMetaDataCollectionSet Collections

        /// <summary>
        /// Gets the set of DBSchemaMetaDataCollections supported by this provider
        /// </summary>
        public DBSchemaMetaDataCollectionSet Collections
        {
            get
            {
                return _collections;
            }
        }

        #endregion

        
        //
        // .ctor
        //

        #region internal DBSchemaProvider(DBDatabase database)

        /// <summary>
        /// Creates a new instance of the DBSchemaProvider
        /// </summary>
        /// <param name="database"></param>
        /// <param name="properties"></param>
        public DBSchemaProvider(DBDatabase database, DBDatabaseProperties properties)
        {
            if (null == database)
                throw new ArgumentNullException("database");

            this._db = database;
            this._props = properties;
            this._cache = new Dictionary<DBSchemaTypes, List<DBSchemaItemRef>>();
            InitMetaDataCollections();
            
        }

        

        #endregion

        //
        // Get ItemRefs public methods
        //

        #region public IEnumerable<DBSchemaItemRef> GetAllTables()

        /// <summary>
        /// Gets references to all the Tables defined in this providers data connection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllTables()
        {

            List<DBSchemaItemRef> all;
            if (!this.TryGetReferencesFromCache(DBSchemaTypes.Table, out all))
            {
                all = new List<DBSchemaItemRef>();

                using (DbConnection con = this.Database.CreateConnection())
                {
                    con.Open();
                    this.LoadTableRefs(con, all);
                }
                this.AddToCache(DBSchemaTypes.Table, all);
            }
            return all.AsReadOnly();
        }

        

        #endregion

        #region public IEnumerable<DBSchemaItemRef> GetAllViews()

        /// <summary>
        /// Gets references to all the Vviews defined in this providers data connection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllViews()
        {
            List<DBSchemaItemRef> all;
            if (!this.TryGetReferencesFromCache(DBSchemaTypes.View, out all))
            {
                all = new List<DBSchemaItemRef>();
                using (DbConnection con = this.Database.CreateConnection())
                {
                    con.Open();
                    this.LoadViewRefs(con, all);
                }
                this.AddToCache(DBSchemaTypes.View, all);
            }
            return all.AsReadOnly();
        }

        #endregion

        #region public IEnumerable<DBSchemaItemRef> GetAllIndexs()

        /// <summary>
        /// Gets references to all the Indexes defined in this providers data connection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllIndexs()
        {
            List<DBSchemaItemRef> all;
            if (!this.TryGetReferencesFromCache(DBSchemaTypes.Index, out all))
            {
                all = new List<DBSchemaItemRef>();
                using (DbConnection con = this.Database.CreateConnection())
                {
                    con.Open();
                    this.LoadIndexRefs(con, all);
                }
                this.AddToCache(DBSchemaTypes.Index, all);
            }
            return all.AsReadOnly();
        }

        #endregion

        #region public IEnumerable<DBSchemaItemRef> GetAllIndexs(string forTable) + 2 overloads

        /// <summary>
        /// Gets references to all the Indexes defined in this providers data connection for the specified table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllIndexs(string forTable)
        {
            if (string.IsNullOrEmpty(forTable))
                throw new ArgumentNullException("forTable");

            IEnumerable<DBSchemaItemRef> all = this.GetAllIndexs();
            
            List<DBSchemaItemRef> match = new List<DBSchemaItemRef>();

            foreach (DBSchemaItemRef idx in all)
            {
                if (null != idx.Container && forTable.Equals(idx.Container.Name, StringComparison.OrdinalIgnoreCase))
                    match.Add(idx);
            }
            
            return match.AsReadOnly();
        }

        /// <summary>
        /// Gets references to all the Indexes defined in this providers data connection for the specified table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllIndexs(string catalog, string schema, string table)
        {
            if (string.IsNullOrEmpty(table))
                throw new ArgumentNullException("table");

            DBSchemaItemRef iref = new DBSchemaItemRef(DBSchemaTypes.Table, catalog, schema, table);

            return GetAllIndexs(iref);
        }

        /// <summary>
        /// Gets references to all the Indexes defined in this providers data connection for the specified table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllIndexs(DBSchemaItemRef forTable)
        {
            if (null == forTable)
                throw new ArgumentNullException("forTable");
            if (string.IsNullOrEmpty(forTable.Name))
                throw new ArgumentNullException("forTable.Name");
            if (forTable.Type != DBSchemaTypes.Table && forTable.Type != DBSchemaTypes.View)
                throw new ArgumentOutOfRangeException("forTable.Type");

            IEnumerable<DBSchemaItemRef> all = this.GetAllIndexs();

            List<DBSchemaItemRef> match = new List<DBSchemaItemRef>();

            foreach (DBSchemaItemRef idx in all)
            {
                if (null != idx.Container && forTable.Equals(idx.Container))
                    match.Add(idx);
            }

            return match.AsReadOnly();
        }

        #endregion

        #region public IEnumerable<DBSchemaItemRef> GetAllStoredProcedures()

        /// <summary>
        /// Gets references to all the stored procedures defined in this providers data connection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllStoredProcedures()
        {
            List<DBSchemaItemRef> all;

            if (!this.TryGetReferencesFromCache(DBSchemaTypes.StoredProcedure, out all))
            {
                all = new List<DBSchemaItemRef>();
                using (DbConnection con = this.Database.CreateConnection())
                {
                    con.Open();
                    this.LoadStoredProcedureRefs(con, all);
                }
                this.AddToCache(DBSchemaTypes.StoredProcedure, all);
            }
            return all.AsReadOnly();
        }

        #endregion

        #region public IEnumerable<DBSchemaItemRef> GetAllFunctions()

        /// <summary>
        /// Gets references to all the functions defined in this providers data connection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllFunctions()
        {
            List<DBSchemaItemRef> all;
            if (!this.TryGetReferencesFromCache(DBSchemaTypes.Function, out all))
            {
                all = new List<DBSchemaItemRef>();
                using (DbConnection con = this.Database.CreateConnection())
                {
                    con.Open();
                    this.LoadFunctionRefs(con, all);
                }
                this.AddToCache(DBSchemaTypes.Function, all);
            }
            return all.AsReadOnly();
        }

        #endregion

        #region  public IEnumerable<DBSchemaItemRef> GetAllRoutines()

        /// <summary>
        /// Gets references to all the routines defined in this providers data connection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllRoutines()
        {
            List<DBSchemaItemRef> all;
            if (!this.TryGetReferencesFromCache(DBSchemaTypes.StoredProcedure | DBSchemaTypes.Function, out all))
            {
                all = new List<DBSchemaItemRef>();
                using (DbConnection con = this.Database.CreateConnection())
                {
                    con.Open();
                    this.LoadRoutineRefs(con, all);
                }
                this.AddToCache(DBSchemaTypes.Function | DBSchemaTypes.StoredProcedure, all);
            }
            return all.AsReadOnly();
        }

        #endregion

        #region public IEnumerable<DBSchemaItemRef> GetAllForeignKeys()

        /// <summary>
        /// Gets references to all the Foreign keys in this providers data connection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllForeignKeys()
        {
            List<DBSchemaItemRef> all;
            if (!this.TryGetReferencesFromCache(DBSchemaTypes.ForeignKey, out all))
            {
                all = new List<DBSchemaItemRef>();
                using (DbConnection con = this.Database.CreateConnection())
                {
                    con.Open();
                    this.LoadForeignKeyRefs(con, all);
                }
                this.AddToCache(DBSchemaTypes.ForeignKey, all);
            }
            return all.AsReadOnly();
        }

        #endregion

        #region public IEnumerable<DBSchemaItemRef> GetAllForeignKeys(string forTable) + 2 overloads

        /// <summary>
        /// Gets references to all the Foreign keys in this providers data connection for the specified table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllForeignKeys(string tablename)
        {
            if (string.IsNullOrEmpty(tablename))
                throw new ArgumentNullException("tablename");

            IEnumerable<DBSchemaItemRef> all = this.GetAllForeignKeys();

            List<DBSchemaItemRef> match = new List<DBSchemaItemRef>();

            foreach (DBSchemaItemRef idx in all)
            {
                if (null != idx.Container && tablename.Equals(idx.Container.Name, StringComparison.OrdinalIgnoreCase))
                    match.Add(idx);
            }

            return match.AsReadOnly();
        }

        /// <summary>
        /// Gets references to all the Foreign keys in this providers data connection for the specified table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllForeignKeys(string tablecatalog, string tableschema, string tablename)
        {
            if (string.IsNullOrEmpty(tablename))
                throw new ArgumentNullException("tablename");

            DBSchemaItemRef iref =  new DBSchemaItemRef(DBSchemaTypes.Table, tablecatalog, tableschema, tablename);

            return GetAllForeignKeys(iref);
        }

        /// <summary>
        /// Gets references to all the Foreign keys in this providers data connection for the specified table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllForeignKeys(DBSchemaItemRef table)
        {
            if (null == table)
                throw new ArgumentNullException("table");
            if (string.IsNullOrEmpty(table.Name))
                throw new ArgumentNullException("table.Name");
            if (table.Type != DBSchemaTypes.Table && table.Type != DBSchemaTypes.View)
                throw new ArgumentOutOfRangeException("table.Type");

            IEnumerable<DBSchemaItemRef> all = this.GetAllForeignKeys();

            List<DBSchemaItemRef> match = new List<DBSchemaItemRef>();

            foreach (DBSchemaItemRef idx in all)
            {
                if (null != idx.Container && table.Equals(idx.Container))
                    match.Add(idx);
            }

            return match.AsReadOnly();
        }

        #endregion
       
        #region public IEnumerable<DBSchemaItemRef> GetAllRefs()

        /// <summary>
        /// Gets references to all the supported schema types for this providers data connection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetAllRefs()
        {
            return GetRefs(this.Properties.SupportedSchemas);
        }

        #endregion

        #region public IEnumerable<DBSchemaItemRef> GetRefs(DBSchemaTypes forTypes)

        /// <summary>
        /// Gets references to all the requested schema types for this providers data connection
        /// </summary>
        /// <param name="forTypes"></param>
        /// <returns></returns>
        public IEnumerable<DBSchemaItemRef> GetRefs(DBSchemaTypes forTypes)
        {
            List<DBSchemaItemRef> all = new List<DBSchemaItemRef>();
            using (DbConnection con = this.Database.CreateConnection())
            {
                con.Open();
                if (IncludesSchemaType(forTypes, DBSchemaTypes.Table))
                    this.GetFromCacheOrLoad(DBSchemaTypes.Table, con, all, new LoadHandler(this.LoadTableRefs));
                if (IncludesSchemaType(forTypes, DBSchemaTypes.View))
                    this.GetFromCacheOrLoad(DBSchemaTypes.Table, con, all, new LoadHandler(this.LoadViewRefs));
                if (IncludesSchemaType(forTypes, DBSchemaTypes.Index))
                    this.GetFromCacheOrLoad(DBSchemaTypes.Table, con, all, new LoadHandler(this.LoadIndexRefs));
                
                //if we want both Functions and Sprocs then load through the Routines methods as an optimization
                if (IncludesSchemaType(forTypes, DBSchemaTypes.Function) && IncludesSchemaType(forTypes, DBSchemaTypes.StoredProcedure))
                    this.GetFromCacheOrLoad(DBSchemaTypes.Table, con, all, new LoadHandler(this.LoadRoutineRefs));
                else if (IncludesSchemaType(forTypes, DBSchemaTypes.Function))
                    this.GetFromCacheOrLoad(DBSchemaTypes.Table, con, all, new LoadHandler(this.LoadFunctionRefs));
                else if (IncludesSchemaType(forTypes, DBSchemaTypes.StoredProcedure))
                    this.GetFromCacheOrLoad(DBSchemaTypes.Table, con, all, new LoadHandler(this.LoadStoredProcedureRefs));
            }
            return all.AsReadOnly();
        }

        private delegate void LoadHandler(DbConnection con, List<DBSchemaItemRef> addtocollection);

        private void GetFromCacheOrLoad(DBSchemaTypes type, DbConnection con, List<DBSchemaItemRef> addtocollection, LoadHandler handler)
        {
            List<DBSchemaItemRef> all;
            if (!this.TryGetReferencesFromCache(type, out all))
            {
                all = new List<DBSchemaItemRef>();
                handler(con, all);
                AddToCache(type, all);
            }
            addtocollection.AddRange(all);
        }

        #endregion

        //
        // Get Schema Items public methods
        //

        #region public DBSchemaTable GetTable(string name) + 3 overloads

        /// <summary>
        /// Gets the schema definition for any table with the 
        /// specified name from the providers database
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A fully populated DBSchemaTable or null if not found</returns>
        public DBSchemaTable GetTable(string name)
        {
            return (DBSchemaTable)GetSchema(name, DBSchemaTypes.Table);
        }

        /// <summary>
        /// Gets the schema definition for any table with the 
        /// specified name and schema owner in the specified catalog from the providers database
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <returns>A fully populated DBSchemaTable or null if not found</returns>
        public DBSchemaTable GetTable(string catalog, string schema, string name)
        {
            return (DBSchemaTable)GetSchema(catalog, schema, name, DBSchemaTypes.Table);
        }

        /// <summary>
        /// Gets the schema definition for any table with the 
        /// specified reference from the providers database - the reference must be a table type
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns>A fully populated DBSchemaTable or null if not found</returns>
        public DBSchemaTable GetTable(DBSchemaItemRef tbl)
        {
            if (tbl.Type != DBSchemaTypes.Table)
                throw new ArgumentOutOfRangeException("tbl", "Reference is not a table type");
            return (DBSchemaTable)GetSchema(tbl);
        }

        #endregion

        #region public DBSchemaView GetView(string name) + 3 overloads

        /// <summary>
        /// Gets the schema definition for any view with the 
        /// specified name from the providers database
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaView GetView(string name)
        {
            return (DBSchemaView)GetSchema(name, DBSchemaTypes.View);
        }

        /// <summary>
        /// Gets the schema definition for any view with the 
        /// specified name and schema owner in the specified catalog from the providers database
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaView GetView(string catalog, string schema, string name)
        {
            return (DBSchemaView)GetSchema(catalog, schema, name, DBSchemaTypes.View);
        }

        /// <summary>
        /// Gets the schema definition for any view with the 
        /// specified reference from the providers database - the reference must be a view type
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public DBSchemaView GetView(DBSchemaItemRef view)
        {
            if (view.Type != DBSchemaTypes.View)
                throw new ArgumentOutOfRangeException("view", "Reference is not a view type");
            return (DBSchemaView)GetSchema(view);
        }

        #endregion

        #region public DBSchemaSproc GetProcedure(string name) + 3 overloads

        /// <summary>
        /// Gets the schema definition for any procedure with the 
        /// specified name from the providers database
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaSproc GetProcedure(string name)
        {
            return (DBSchemaSproc)GetSchema(name, DBSchemaTypes.StoredProcedure);
        }

        /// <summary>
        /// Gets the schema definition for any procedure with the 
        /// specified name and schema owner in the specified catalog from the providers database
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaSproc GetProcedure(string catalog, string schema, string name)
        {
            return (DBSchemaSproc)GetSchema(catalog, schema, name, DBSchemaTypes.StoredProcedure);
        }

        /// <summary>
        /// Gets the schema definition for any procedure with the 
        /// specified reference from the providers database - the reference must be a procedure type
        /// </summary>
        /// <param name="sproc"></param>
        /// <returns></returns>
        public DBSchemaSproc GetProcedure(DBSchemaItemRef sproc)
        {
            if (sproc.Type != DBSchemaTypes.StoredProcedure)
                throw new ArgumentOutOfRangeException("sproc", "Reference is not a Stored Procedure type");
            return (DBSchemaSproc)GetSchema(sproc);
        }

        #endregion

        #region public DBSchemaFunction GetFunction(string name) + 3 overloads

        /// <summary>
        /// Gets the schema definition for any Function with the 
        /// specified name from the providers database
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaFunction GetFunction(string name)
        {
            return (DBSchemaFunction)GetSchema(name, DBSchemaTypes.Function);
        }

        /// <summary>
        /// Gets the schema definition for any Function with the 
        /// specified name and schema owner in the specified catalog from the providers database
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaFunction GetFunction(string catalog, string schema, string name)
        {
            return (DBSchemaFunction)GetSchema(catalog, schema, name, DBSchemaTypes.Function);
        }

        /// <summary>
        /// Gets the schema definition for any Function with the 
        /// specified reference from the providers database - the reference must be a procedure type
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public DBSchemaFunction GetFunction(DBSchemaItemRef func)
        {
            if (func.Type != DBSchemaTypes.Function)
                throw new ArgumentOutOfRangeException("Func", "Reference is not a Function type");
            return (DBSchemaFunction)GetSchema(func);
        }

        #endregion

        #region public DBSchemaIndex GetIndex(string name) + 3 overloads

        /// <summary>
        /// Gets the schema definition for any Index with the 
        /// specified name from the providers database
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaIndex GetIndex(string name)
        {
            return (DBSchemaIndex)GetSchema(name, DBSchemaTypes.Index);
        }

        /// <summary>
        /// Gets the schema definition for any Index with the 
        /// specified name and schema owner in the specified catalog from the providers database
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaIndex GetIndex(string catalog, string schema, string name)
        {
            return (DBSchemaIndex)GetSchema(catalog, schema, name, DBSchemaTypes.Index);
        }

        /// <summary>
        /// Gets the schema definition for any Index with the 
        /// specified reference from the providers database - the reference must be a procedure type
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public DBSchemaIndex GetIndex(DBSchemaItemRef idx)
        {
            if (idx.Type != DBSchemaTypes.Index)
                throw new ArgumentOutOfRangeException("idx", "Reference is not a Index type");
            return (DBSchemaIndex)GetSchema(idx);
        }

        #endregion

        #region public DBSchemaForeignKey GetForeignKey(string name) + 3 overloads

        /// <summary>
        /// Gets the schema definition for any ForeignKey with the 
        /// specified name from the providers database
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaForeignKey GetForeignKey(string name)
        {
            return (DBSchemaForeignKey)GetSchema(name, DBSchemaTypes.ForeignKey);
        }

        /// <summary>
        /// Gets the schema definition for any ForeignKey with the 
        /// specified name and schema owner in the specified catalog from the providers database
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public DBSchemaForeignKey GetForeignKey(string catalog, string schema, string name)
        {
            return (DBSchemaForeignKey)GetSchema(catalog, schema, name, DBSchemaTypes.ForeignKey);
        }

        /// <summary>
        /// Gets the schema definition for any ForeignKey with the 
        /// specified reference from the providers database - the reference must be a procedure type
        /// </summary>
        /// <param name="fk"></param>
        /// <returns></returns>
        public DBSchemaForeignKey GetForeignKey(DBSchemaItemRef fk)
        {
            if (fk.Type != DBSchemaTypes.ForeignKey)
                throw new ArgumentOutOfRangeException("fk", "Reference is not a ForeignKey type");
            return (DBSchemaForeignKey)GetSchema(fk);
        }

        #endregion


        #region public virtual DBSchemaItem GetSchema(DBSchemaItemRef iref) + 2 overloads
        /// <summary>
        /// Gets the schema definition for an item in the database with the name and type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DBSchemaItem GetSchema(string name, DBSchemaTypes type)
        {
            DBSchemaItemRef iref = new DBSchemaItemRef(type, name);
            return GetSchema(iref);
        }

        /// <summary>
        /// Gets a schema definition for an item in the database with the specified criteria
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public DBSchemaItem GetSchema(string catalog, string schema, string name, DBSchemaTypes type)
        {
            DBSchemaItemRef iref = new DBSchemaItemRef(type, catalog, schema, name);
            return GetSchema(iref);
        }

        /// <summary>
        /// Gets a schema definition for an item in the database with the specified criteria
        /// </summary>
        /// <param name="iref"></param>
        /// <returns></returns>
        public virtual DBSchemaItem GetSchema(DBSchemaItemRef iref)
        {
            if (null == iref)
                throw new ArgumentNullException("iref");
 
            DBSchemaItem item = null;
            using (DbConnection con = this.Database.CreateConnection())
            {
                con.Open();
                switch (iref.Type)
                {
                    case DBSchemaTypes.Table:
                        item = this.LoadATable(con, iref);
                        break;
                    case DBSchemaTypes.View:
                        item = this.LoadAView(con, iref);
                        break;
                    case DBSchemaTypes.StoredProcedure:
                        item = this.LoadASproc(con, iref);
                        break;
                    case DBSchemaTypes.Function:
                        item = this.LoadAFunction(con, iref);
                        break;
                    case DBSchemaTypes.Index:
                        item = this.LoadAnIndex(con, iref);
                        break;
                    case DBSchemaTypes.ForeignKey:
                        item = this.LoadAForeignKey(con, iref);
                        break;
                    case DBSchemaTypes.CommandScripts:
                    default:
                        throw new ArgumentOutOfRangeException("iref.Type");

                }
            }

            return item;
        }

        #endregion

        //
        // protected loadXXX methods
        //

        #region protected virtual void LoadMetaDataCollections(DbConnection con, DBSchemaMetaDataCollectionSet intoCollection)
        
        /// <summary>
        /// loads all the available meta data collections for this providers data connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected virtual void LoadMetaDataCollections(DbConnection con, DBSchemaMetaDataCollectionSet intoCollection)
        {
            DataTable tbl = con.GetSchema(DbMetaDataCollectionNames.MetaDataCollections);
            if (OUTPUTCOLLECTIONDATA)
            {
                if (System.IO.Directory.Exists(OUTPUTDATAPATH) == false)
                    System.IO.Directory.CreateDirectory(OUTPUTDATAPATH);

                tbl.WriteXml(OUTPUTDATAPATH + "MetadataCollections.xml");
            }
            DataColumn namecol = GetColumn(tbl, DbMetaDataColumnNames.CollectionName, true);
            DataColumn restrictionscol = GetColumn(tbl, DbMetaDataColumnNames.NumberOfRestrictions, true);
            DataColumn partscol = GetColumn(tbl, DbMetaDataColumnNames.NumberOfIdentifierParts, true);

            foreach (DataRow dr in tbl.Rows)
            {
                string name = GetColumnStringValue(dr, namecol);
                DBMetaDataCollectionType type = GetMetDataCollectionTypeForName(name);
                int numrest = GetColumnIntValue(dr, restrictionscol);
                int numparts = GetColumnIntValue(dr, partscol);
                DBSchemaMetaDataCollection col = new DBSchemaMetaDataCollection(name, numrest, numparts, type);
                intoCollection.Add(col);
            }
            
        }

        #endregion

        #region protected virtual DataTable LoadRestrictions(DbConnection con)
        /// <summary>
        /// Loads this providers restrictions
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        protected virtual DataTable LoadRestrictions(DbConnection con)
        {
            DataTable tbl = null;
            try
            {
                tbl = con.GetSchema(DbMetaDataCollectionNames.Restrictions);
                if (OUTPUTCOLLECTIONDATA)
                {
                    tbl.WriteXml(OUTPUTDATAPATH + "Restrictions.xml");
                }
            }
            catch (Exception) { }
            return tbl;
        }

        #endregion

        // load references for types

        #region protected virtual void LoadTableRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)

        /// <summary>
        /// Loads all the tables in this providers data connection.
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected virtual void LoadTableRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection tblcollection = this.AssertGetCollectionForType(DBMetaDataCollectionType.Tables);
            DataTable data = this.GetCollectionData(con, tblcollection);
            
            DataColumn catalogcol = GetColumn(data, "TABLE_CATALOG", false);
            DataColumn schemacol = GetColumn(data, "TABLE_SCHEMA", false);
            DataColumn namecol = GetColumn(data, "TABLE_NAME",true);

            this.LoadItemRefs(data, intoCollection, catalogcol, schemacol, namecol, DBSchemaTypes.Table);
        }

        #endregion

        #region protected virtual void LoadViewRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)

        /// <summary>
        /// Loads all the views in this providers data connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected virtual void LoadViewRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection viewcollection = this.AssertGetCollectionForType(DBMetaDataCollectionType.Views);
            DataTable data = this.GetCollectionData(con, viewcollection);

            DataColumn catalogcol = GetColumn(data, "TABLE_CATALOG", false);
            DataColumn schemacol = GetColumn(data, "TABLE_SCHEMA", false);
            DataColumn namecol = GetColumn(data, "TABLE_NAME", true);

            this.LoadItemRefs(data, intoCollection, catalogcol, schemacol, namecol, DBSchemaTypes.View);
        }

        #endregion

        #region protected virtual void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection) + 1 overload

        /// <summary>
        /// Loads all the indexes in this providers data connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected virtual void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection viewcollection = 
                this.AssertGetCollectionForType(DBMetaDataCollectionType.Indexes);
            DataTable data = this.GetCollectionData(con, viewcollection);

            DataColumn catalogcol = GetColumn(data, "constraint_catalog", false);
            DataColumn schemacol = GetColumn(data, "constraint_schema", false);
            DataColumn namecol = GetColumn(data, "constraint_name", true);

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

        

        /// <summary>
        /// Loads all the indexes in this providers data connection for the specified table
        /// </summary>
        /// <param name="con"></param>
        /// <param name="fortable"></param>
        /// <param name="intoCollection"></param>
        protected virtual void LoadIndexRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)
        {
            DBSchemaMetaDataCollection viewcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.Indexes);

            DataTable data = this.GetCollectionData(con, viewcollection, fortable.Catalog, fortable.Schema,  fortable.Name, string.Empty);
            
            DataColumn catalogcol = GetColumn(data, "constraint_catalog", false);
            DataColumn schemacol = GetColumn(data, "constraint_schema", false);
            DataColumn namecol = GetColumn(data, "constraint_name", true);

            DataColumn tablecatalogcol = GetColumn(data, "table_catalog", false);
            DataColumn tableschemacol = GetColumn(data, "table_schema", false);
            DataColumn tablenamecol = GetColumn(data, "table_name", true);

            this.LoadItemRefsWithContainer(data, intoCollection,
                    catalogcol, schemacol, namecol, DBSchemaTypes.Index,
                    tablecatalogcol, tableschemacol, tablenamecol, DBSchemaTypes.Table);
        }

        #endregion

        #region protected virtual void LoadRoutineRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)

        /// <summary>
        /// Loads all the routines (Procedures and fuctions) in this providers data connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected virtual void LoadRoutineRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection viewcollection = 
                this.AssertGetCollectionForType(DBMetaDataCollectionType.Procedures);
            DataTable data = this.GetCollectionData(con, viewcollection);

            DataColumn catalogcol = GetColumn(data, "ROUTINE_CATALOG", false);
            DataColumn schemacol = GetColumn(data, "ROUTINE_SCHEMA", false);
            DataColumn namecol = GetColumn(data, "ROUTINE_NAME", true);
            DataColumn typecol = GetColumn(data, "ROUTINE_TYPE", true);

            foreach (DataRow row in data.Rows)
            {
                DBSchemaItemRef iref;
                string type = GetColumnStringValue(row, typecol);
                
                if (string.Equals(type, "PROCEDURE", StringComparison.OrdinalIgnoreCase))
                {
                    iref = this.LoadAnItemRef(row,catalogcol,schemacol,namecol,DBSchemaTypes.StoredProcedure);
                    intoCollection.Add(iref);
                }
                else if (string.Equals(type, "FUNCTION", StringComparison.OrdinalIgnoreCase))
                {
                    iref = this.LoadAnItemRef(row, catalogcol, schemacol, namecol, DBSchemaTypes.Function);
                    intoCollection.Add(iref);
                }
            }
        }

        #endregion

        #region protected virtual void LoadStoredProcedureRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)

        /// <summary>
        /// Loads all the Procedures (not functions) in this providers data connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected virtual void LoadStoredProcedureRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection viewcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.Procedures);
            DataTable data = this.GetCollectionData(con, viewcollection);

            DataColumn catalogcol = GetColumn(data, "ROUTINE_CATALOG", false);
            DataColumn schemacol = GetColumn(data, "ROUTINE_SCHEMA", false);
            DataColumn namecol = GetColumn(data, "ROUTINE_NAME", true);
            DataColumn typecol = GetColumn(data, "ROUTINE_TYPE", true);

            foreach (DataRow row in data.Rows)
            {
                DBSchemaItemRef iref;
                string type = GetColumnStringValue(row, typecol);

                if (string.Equals(type, "PROCEDURE", StringComparison.OrdinalIgnoreCase))
                {
                    iref = this.LoadAnItemRef(row, catalogcol, schemacol, namecol, DBSchemaTypes.StoredProcedure);
                    intoCollection.Add(iref);
                }
            }
        }

        #endregion

        #region  protected virtual void LoadFunctionRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)

        /// <summary>
        /// Loads all the functions (not procedures) in this providers data connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected virtual void LoadFunctionRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection viewcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.Procedures);
            DataTable data = this.GetCollectionData(con, viewcollection);

            DataColumn catalogcol = GetColumn(data, "ROUTINE_CATALOG", false);
            DataColumn schemacol = GetColumn(data, "ROUTINE_SCHEMA", false);
            DataColumn namecol = GetColumn(data, "ROUTINE_NAME", true);
            DataColumn typecol = GetColumn(data, "ROUTINE_TYPE", true);

            foreach (DataRow row in data.Rows)
            {
                DBSchemaItemRef iref;
                string type = GetColumnStringValue(row, typecol);

                if (string.Equals(type, "FUNCTION", StringComparison.OrdinalIgnoreCase))
                {
                    iref = this.LoadAnItemRef(row, catalogcol, schemacol, namecol, DBSchemaTypes.Function);
                    intoCollection.Add(iref);
                }
            }
        }

        #endregion

        #region protected virtual void LoadForeignKeyRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection) + 1 overload

        /// <summary>
        /// Loads all the ForeignKeys in this providers data connection
        /// </summary>
        /// <param name="con"></param>
        /// <param name="intoCollection"></param>
        protected virtual void LoadForeignKeyRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection)
        {
            DBSchemaMetaDataCollection viewcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.ForeignKeys);
            DataTable data = this.GetCollectionData(con, viewcollection);

            DataColumn catalogcol = GetColumn(data, "constraint_catalog", false);
            DataColumn schemacol = GetColumn(data, "constraint_schema", false);
            DataColumn namecol = GetColumn(data, "constraint_name", true);

            DataColumn containercatalogcol = GetColumn(data, "table_catalog", false);
            DataColumn containerschemacol = GetColumn(data, "table_schema", false);
            DataColumn containernamecol = GetColumn(data, "table_name", true);

            this.LoadItemRefsWithContainer(data, intoCollection, 
                catalogcol, schemacol, namecol, DBSchemaTypes.ForeignKey,
                containercatalogcol,containerschemacol, containernamecol, DBSchemaTypes.Table);
        }

        /// <summary>
        /// Loads all the ForeignKeys in this providers data connection for the specified table
        /// </summary>
        /// <param name="con"></param>
        /// <param name="fortable"></param>
        /// <param name="intoCollection"></param>
        protected virtual void LoadForeignKeyRefs(DbConnection con, IList<DBSchemaItemRef> intoCollection, DBSchemaItemRef fortable)
        {
            DBSchemaMetaDataCollection viewcollection =
                this.AssertGetCollectionForType(DBMetaDataCollectionType.ForeignKeys);
            DataTable data = this.GetCollectionData(con, viewcollection, fortable.Catalog, fortable.Schema, fortable.Name, null);

            DataColumn catalogcol = GetColumn(data, "constraint_catalog", true);
            DataColumn schemacol = GetColumn(data, "constraint_schema", true);
            DataColumn namecol = GetColumn(data, "constraint_name", true);

            DataColumn containercatalogcol = GetColumn(data, "table_catalog", true);
            DataColumn containerschemacol = GetColumn(data, "table_schema", true);
            DataColumn containernamecol = GetColumn(data, "table_name", true);

            this.LoadItemRefsWithContainer(data, intoCollection,
                catalogcol, schemacol, namecol, DBSchemaTypes.ForeignKey,
                containercatalogcol, containerschemacol, containernamecol, DBSchemaTypes.Table);
        }

        #endregion

        // load SchemaItem's 

        #region protected virtual DBSchemaTable LoadATable(DbConnection con, DBSchemaItemRef tableref) + support methods
        /// <summary>
        /// Loads the DBSchemaTable from the database with the tableref identity
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tableref"></param>
        /// <returns></returns>
        protected virtual DBSchemaTable LoadATable(DbConnection con, DBSchemaItemRef tableref)
        {
            DataTable dtTable = GetTableData(con, tableref);

            DBSchemaTable atable = null;

            if(null != dtTable && dtTable.Rows.Count > 0)
            {
                atable = new DBSchemaTable();
                this.FillTableData(atable, dtTable.Rows[0]);

                DataTable dtColumns = this.GetTableColumns(con, tableref);
                if(null != dtColumns)
                    this.FillTableColumns(atable.Columns, dtColumns);

                DBSchemaItemRefCollection idxs = new DBSchemaItemRefCollection();
                DBSchemaItemRefCollection fks = new DBSchemaItemRefCollection();

                this.LoadForeignKeyRefs(con, fks, tableref);
                this.LoadIndexRefs(con, idxs, tableref);

                DBSchemaIndexCollection indexes = new DBSchemaIndexCollection();
                foreach (DBSchemaItemRef idx in idxs)
                {
                    DBSchemaIndex anindex = this.LoadAnIndex(con, idx);
                    indexes.Add(anindex);
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
        /// <summary>
        /// Returns a DataTable that describes the table
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tableref"></param>
        /// <returns></returns>
        protected virtual DataTable GetTableData(DbConnection con, DBSchemaItemRef tableref)
        {
            DBSchemaMetaDataCollection tblcol = this.AssertGetCollectionForType(DBMetaDataCollectionType.Tables);
            DataTable dtTable = this.GetCollectionData(con, tblcol, tableref.Catalog, tableref.Schema, tableref.Name);

            return dtTable;
        }

        /// <summary>
        /// Returns a DataTable that describes all the columns in the table
        /// </summary>
        /// <param name="con"></param>
        /// <param name="tableref"></param>
        /// <returns></returns>
        protected virtual DataTable GetTableColumns(DbConnection con, DBSchemaItemRef tableref)
        {
            DBSchemaMetaDataCollection colmscol = this.AssertGetCollectionForType(DBMetaDataCollectionType.Columns);
            return this.GetCollectionData(con, colmscol, tableref.Catalog, tableref.Schema, tableref.Name);
        }

        /// <summary>
        /// Fills the table information from the row
        /// </summary>
        /// <param name="table"></param>
        /// <param name="row"></param>
        protected virtual void FillTableData(DBSchemaTable table, DataRow row)
        {
            DataColumn cat = GetColumn(row.Table, "table_catalog", false);
            DataColumn schema = GetColumn(row.Table, "table_schema", false);
            DataColumn name = GetColumn(row.Table, "table_name", true);

            table.Catalog = GetColumnStringValue(row, cat);
            table.Schema = GetColumnStringValue(row, schema);
            table.Name = GetColumnStringValue(row, name);
        }

        /// <summary>
        /// Fills the column collection information from the data table
        /// </summary>
        /// <param name="atablecolumns"></param>
        /// <param name="dtColumns"></param>
        protected virtual void FillTableColumns(DBSchemaTableColumnCollection atablecolumns, DataTable dtColumns)
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
                col.DbType = GetDbTypeForNativeType(GetColumnStringValue(row, DataTypeColumn));
                col.Type = GetSystemTypeForNativeType(GetColumnStringValue(row, DataTypeColumn));
                col.Size = GetColumnIntValue(row,MaxCharacterLengthColumn);
                col.AutoAssign = GetColumnBoolValue(row, AutoNumberColumn);
                col.PrimaryKey = GetColumnBoolValue(row, PrimaryKeyColumn);
                col.ReadOnly = col.AutoAssign;
                col.HasDefault = !string.IsNullOrEmpty(col.DefaultValue);

                atablecolumns.Add(col);
            }

        }

        

        #endregion

        #region protected virtual DBSchemaForeignKey LoadAForeignKey(DbConnection con, DBSchemaItemRef fkref) + support methods
        
        /// <summary>
        /// Loads all the data associated with the specified ForeignKey reference
        /// </summary>
        /// <param name="con"></param>
        /// <param name="fkref"></param>
        /// <returns></returns>
        protected virtual DBSchemaForeignKey LoadAForeignKey(DbConnection con, DBSchemaItemRef fkref)
        {
            DataTable dtFK = GetForeignKeyData(con, fkref);
            DBSchemaForeignKey anFk = null;

            if (null != dtFK && dtFK.Rows.Count > 0)
            {
                anFk = new DBSchemaForeignKey();
                this.FillForeignKeyData(anFk, dtFK.Rows[0]);
                
                DataTable dtColumns = this.GetForeignKeyColumns(con, fkref);
                if(null != dtColumns)
                    this.FillForeignKeyColumns(anFk, dtColumns);
            }
            return anFk;
        }

        /// <summary>
        /// Get the DataTable that describes the foreign key
        /// </summary>
        /// <param name="con"></param>
        /// <param name="fkref"></param>
        /// <returns></returns>
        protected virtual DataTable GetForeignKeyData(DbConnection con, DBSchemaItemRef fkref)
        {
            DBSchemaMetaDataCollection fkcol = this.AssertGetCollectionForType(DBMetaDataCollectionType.ForeignKeys);
            DataTable dtFK = this.GetCollectionData(con, fkcol, fkref.Catalog, fkref.Schema, null, fkref.Name);

            return dtFK;
        }

        /// <summary>
        /// populates the fk information from the row
        /// </summary>
        /// <param name="fk"></param>
        /// <param name="dtFKRow"></param>
        protected virtual void FillForeignKeyData(DBSchemaForeignKey fk, DataRow dtFKRow)
        {
            DataColumn catalog = GetColumn(dtFKRow.Table, "CONSTRAINT_CATALOG", false);
            DataColumn schema = GetColumn(dtFKRow.Table, "CONSTRAINT_SCHEMA", false);
            DataColumn name = GetColumn(dtFKRow.Table, "CONSTRAINT_NAME", true);

            fk.Catalog = GetColumnStringValue(dtFKRow, catalog);
            fk.Schema = GetColumnStringValue(dtFKRow, schema);
            fk.Name = GetColumnStringValue(dtFKRow, name);
        }

        /// <summary>
        /// Returns a datatable of all the Foreign Key columns for a specified. 
        /// </summary>
        /// <param name="con"></param>
        /// <param name="fkref"></param>
        /// <returns></returns>
        /// <remarks>NOTE: The base implementation only supports single key mappings. Inheritors should 
        /// override the default behaviour to support access to foreign keys mapped to multiple columns
        /// when provider supports it.</remarks>
        protected virtual DataTable GetForeignKeyColumns(DbConnection con, DBSchemaItemRef fkref)
        {
            DBSchemaMetaDataCollection colmscol = this.AssertGetCollectionForType(DBMetaDataCollectionType.IndexColumns);

            string sql = @"SELECT 
    CONSTRAINT_CATALOG AS FKCatalog, 
    CONSTRAINT_SCHEMA  AS FKSchema,
    CONSTRAINT_NAME AS FKConstraintName,
    TABLE_CATALOG AS FKTableCatalog, 
    TABLE_SCHEMA AS FKTableSchema,
    TABLE_NAME AS FKTable,
    COLUMN_NAME AS FKColumn,
    REFERENCED_TABLE_CATALOG AS PKCatalog,
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

        /// <summary>
        /// populates the fk columns from the data table
        /// </summary>
        /// <param name="anFk"></param>
        /// <param name="dtColumns"></param>
        protected virtual void FillForeignKeyColumns(DBSchemaForeignKey anFk, DataTable dtColumns)
        {
            DataColumn FKCatalog = GetColumn(dtColumns,"FKCatalog",false);
            DataColumn FKSchema = GetColumn(dtColumns, "FKSchema", false);
            DataColumn FKTable = GetColumn(dtColumns, "FKTable", true);
            DataColumn FKColumn = GetColumn(dtColumns, "FKColumn", true);

            DataColumn PKCatalog = GetColumn(dtColumns, "PKCatalog", false);
            DataColumn PKSchema = GetColumn(dtColumns, "PKSchema", false);
            DataColumn PKTable = GetColumn(dtColumns, "PKTable", true);
            DataColumn PKColumn = GetColumn(dtColumns, "PKColumn", true);

            bool first = true;

            foreach (DataRow row in dtColumns.Rows)
            {
                if (first)
                {
                    first = false;
                    //populate the references to the foreign key tables and primary key tables
                    DBSchemaItemRef fktbl = new DBSchemaItemRef(DBSchemaTypes.Table,
                        GetColumnStringValue(row, FKCatalog), GetColumnStringValue(row, FKSchema), GetColumnStringValue(row, FKTable));
                    anFk.ForeignKeyTable = fktbl;

                    DBSchemaItemRef pktbl = new DBSchemaItemRef(DBSchemaTypes.Table,
                        GetColumnStringValue(row, PKCatalog), GetColumnStringValue(row, PKSchema), GetColumnStringValue(row, PKTable));
                    anFk.PrimaryKeyTable = pktbl;

                }

                DBSchemaForeignKeyMapping map = new DBSchemaForeignKeyMapping();
                map.ForeignColumn = GetColumnStringValue(row, FKColumn);
                map.PrimaryColumn = GetColumnStringValue(row, PKColumn);

                anFk.Mappings.Add(map);
            }
        }

        #endregion

        #region protected virtual DBSchemaView LoadAView(DbConnection con, DBSchemaItemRef vref) + support methods
        /// <summary>
        /// Loads the schema information for the requested view
        /// </summary>
        /// <param name="con"></param>
        /// <param name="vref"></param>
        /// <returns></returns>
        protected virtual DBSchemaView LoadAView(DbConnection con, DBSchemaItemRef vref)
        {
            DataTable dtView = GetViewData(con, vref);
            DBSchemaView aview = null;

            if (null != dtView && dtView.Rows.Count > 0)
            {
                aview = new DBSchemaView();
                this.FillViewData(aview, dtView.Rows[0]);

                DataTable dtColumns = this.GetViewColumns(con, vref);
                if (null != dtColumns)
                {
                    this.FillViewColumns(aview.Columns, dtColumns);

                    if (aview.IsUpdateable == false)
                    {
                        foreach (DBSchemaViewColumn vcol in aview.Columns)
                        {
                            vcol.ReadOnly = true;
                        }
                    }
                }
            }

            return aview;

        }

        /// <summary>
        /// Gets the DataTable describing the view
        /// </summary>
        /// <param name="con"></param>
        /// <param name="vref"></param>
        /// <returns></returns>
        protected virtual DataTable GetViewData(DbConnection con, DBSchemaItemRef vref)
        {
            DBSchemaMetaDataCollection vcol = this.AssertGetCollectionForType(DBMetaDataCollectionType.Views);
            DataTable dtView = this.GetCollectionData(con, vcol, vref.Catalog, vref.Schema, vref.Name);

            return dtView;
        }
        /// <summary>
        /// Gets the data table describing theview columns
        /// </summary>
        /// <param name="con"></param>
        /// <param name="vref"></param>
        /// <returns></returns>
        protected virtual DataTable GetViewColumns(DbConnection con, DBSchemaItemRef vref)
        {
            DBSchemaMetaDataCollection colmscol = this.AssertGetCollectionForType(DBMetaDataCollectionType.ViewColumns);
            return this.GetCollectionData(con, colmscol, vref.Catalog, vref.Schema, vref.Name);
        }

        /// <summary>
        /// Populats the schema view from the row
        /// </summary>
        /// <param name="view"></param>
        /// <param name="row"></param>
        protected virtual void FillViewData(DBSchemaView view, DataRow row)
        {
            DataColumn cat = GetColumn(row.Table, "table_catalog", false);
            DataColumn schema = GetColumn(row.Table, "table_schema", false);
            DataColumn name = GetColumn(row.Table, "table_name", true);
            DataColumn update = GetColumn(row.Table, "is_updatable", false);

            view.Catalog = GetColumnStringValue(row, cat);
            view.Schema = GetColumnStringValue(row, schema);
            view.Name = GetColumnStringValue(row, name);
            view.IsUpdateable = GetColumnBoolValue(row, update, false);
        }

        /// <summary>
        /// Populates the schema view columns from teh data table
        /// </summary>
        /// <param name="aview"></param>
        /// <param name="dtColumns"></param>
        protected virtual void FillViewColumns(DBSchemaViewColumnCollection aview, DataTable dtColumns)
        {
            DataColumn ColumnNameColumn = GetColumn(dtColumns, "COLUMN_NAME", true);
            DataColumn OrdinalPositionColumn = GetColumn(dtColumns, "ORDINAL_POSITION", true);
            DataColumn DefaultValueColumn = GetColumn(dtColumns, "COLUMN_DEFAULT", false);
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
                col.DbType = GetDbTypeForNativeType(GetColumnStringValue(row, DataTypeColumn));
                col.Type = GetSystemTypeForNativeType(GetColumnStringValue(row, DataTypeColumn));
                col.Size = GetColumnIntValue(row, MaxCharacterLengthColumn);
                col.ReadOnly = GetColumnBoolValue(row, IsReadOnly);
                col.HasDefault = !string.IsNullOrEmpty(col.DefaultValue);

                aview.Add(col);
            }
        }

        #endregion

        #region protected virtual DBSchemaIndex LoadAnIndex(DbConnection con, DBSchemaItemRef idxref) + support methods
        /// <summary>
        /// Loads the schema information for the requested index
        /// </summary>
        /// <param name="con"></param>
        /// <param name="idxref"></param>
        /// <returns></returns>
        protected virtual DBSchemaIndex LoadAnIndex(DbConnection con, DBSchemaItemRef idxref)
        {
            DataTable dtIdxs = this.GetIndexData(con, idxref);
            DBSchemaIndex anindex = null;

            if (null != dtIdxs && dtIdxs.Rows.Count > 0)
            {
                anindex = new DBSchemaIndex();
                this.FillIndexData(anindex, dtIdxs.Rows[0]);

                DataTable dtIdxColumns = this.GetIndexColumns(con, idxref);
                if(null != dtIdxColumns)
                    this.FillIndexColuns(anindex, dtIdxColumns);
            }
            return anindex;

        }
        /// <summary>
        /// Gets the data table describing the index
        /// </summary>
        /// <param name="con"></param>
        /// <param name="idxref"></param>
        /// <returns></returns>
        protected virtual DataTable GetIndexData(DbConnection con, DBSchemaItemRef idxref)
        {
            DBSchemaMetaDataCollection idxcol = this.AssertGetCollectionForType(DBMetaDataCollectionType.Indexes);
            string table;
            if (idxref.Container != null)
                table = idxref.Container.Name;
            else
                table = null;
            return this.GetCollectionData(con, idxcol, null, null, table, idxref.Name);
        }

        /// <summary>
        /// Gets the DataTable describing the Index Columns
        /// </summary>
        /// <param name="con"></param>
        /// <param name="idxref"></param>
        /// <returns></returns>
        protected virtual DataTable GetIndexColumns(DbConnection con, DBSchemaItemRef idxref)
        {
            DBSchemaMetaDataCollection col = this.AssertGetCollectionForType(DBMetaDataCollectionType.IndexColumns);
            string ownertable = idxref.Container == null ? null : idxref.Container.Name;

            return this.GetCollectionData(con, col, null, null, ownertable, idxref.Name);
        }

        /// <summary>
        /// Populates the Index from the information in the data row
        /// </summary>
        /// <param name="anindex"></param>
        /// <param name="dataRow"></param>
        protected virtual void FillIndexData(DBSchemaIndex anindex, DataRow dataRow)
        {
            DataColumn catalog = GetColumn(dataRow.Table, "constraint_catalog", false);
            DataColumn schema = GetColumn(dataRow.Table, "constraint_schema", false);
            DataColumn name = GetColumn(dataRow.Table, "constraint_name", true);

            DataColumn tblcatalog = GetColumn(dataRow.Table, "table_catalog", false);
            DataColumn tblschema = GetColumn(dataRow.Table, "table_schema", false);
            DataColumn tblname = GetColumn(dataRow.Table, "table_name", true);

            anindex.Catalog = GetColumnStringValue(dataRow, catalog);
            anindex.Schema = GetColumnStringValue(dataRow, schema);
            anindex.Name = GetColumnStringValue(dataRow, name);

            DBSchemaItemRef tblref = new DBSchemaItemRef(DBSchemaTypes.Table,
                GetColumnStringValue(dataRow, tblcatalog), GetColumnStringValue(dataRow, tblschema), GetColumnStringValue(dataRow, tblname));
            anindex.TableReference = tblref;

        }

        /// <summary>
        /// Populates the index columns from the DataTable
        /// </summary>
        /// <param name="anindex"></param>
        /// <param name="dtIdxColumns"></param>
        protected virtual void FillIndexColuns(DBSchemaIndex anindex, DataTable dtIdxColumns)
        {
            DataColumn nameCol = GetColumn(dtIdxColumns, "COLUMN_NAME", true);

            foreach (DataRow row in dtIdxColumns.Rows)
            {
                string name = GetColumnStringValue(row, nameCol);
                if (string.IsNullOrEmpty(name) == false)
                {
                    DBSchemaIndexColumn col = new DBSchemaIndexColumn(name);
                    anindex.Columns.Add(col);
                }
            }
        }

        #endregion

        #region protected virtual DBSchemaSproc LoadASproc(DbConnection con, DBSchemaItemRef sprocref) + support methods

        /// <summary>
        /// Loads the schema information  for the requested stored procedure
        /// </summary>
        /// <param name="con"></param>
        /// <param name="sprocref"></param>
        /// <returns></returns>
        protected virtual DBSchemaSproc LoadASproc(DbConnection con, DBSchemaItemRef sprocref)
        {
            DataTable dtSproc = this.GetRoutineData(con, sprocref);
            DBSchemaSproc asproc = null;

            if (null != dtSproc && dtSproc.Rows.Count > 0)
            {
                asproc = new DBSchemaSproc();
                this.FillRoutineData(asproc, dtSproc.Rows[0]);

                DataTable dtSprocParams = this.GetRoutineParams(con, sprocref);
                if(null != dtSprocParams)
                    this.FillRoutineParams(asproc, dtSprocParams);

                DataTable dtSprocResults = this.GetSprocResultSchema(con, asproc);

                if(null != dtSprocResults)
                    this.FillSprocResults(asproc, dtSprocResults);
            }

            return asproc;
        }

        /// <summary>
        /// Returns the DataTable describing the resquested stored procedure
        /// </summary>
        /// <param name="con"></param>
        /// <param name="routine"></param>
        /// <returns></returns>
        protected virtual DataTable GetSprocResultSchema(DbConnection con, DBSchemaRoutine routine)
        {
            string fullname = GetFullName(routine.GetReference());

            using (DbCommand cmd = this.Database.CreateCommand(con, fullname))
            {
                cmd.CommandType = CommandType.StoredProcedure;
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
                    if(null != dt && OUTPUTCOLLECTIONDATA)
                    {
                        dt.WriteXml(OUTPUTDATAPATH + "routineresults_" + routine.Name + ".xml");
                    }

                    return dt;
                }
            }
        }

        /// <summary>
        /// Populates the DBSchemaSproc with the inofrmation in the data table
        /// </summary>
        /// <param name="asproc"></param>
        /// <param name="dtSprocResults"></param>
        protected virtual void FillSprocResults(DBSchemaSproc asproc, DataTable dtSprocResults)
        {
            this.FillViewColumns(asproc.Results, dtSprocResults);
        }

        #endregion

        #region protected virtual DBSchemaFunction LoadAFunction(DbConnection con, DBSchemaItemRef funcref)
        /// <summary>
        /// Loads the schema information for the requested function
        /// </summary>
        /// <param name="con"></param>
        /// <param name="funcref"></param>
        /// <returns></returns>
        protected virtual DBSchemaFunction LoadAFunction(DbConnection con, DBSchemaItemRef funcref)
        {
            DataTable dtFunc = this.GetRoutineData(con, funcref);
            DBSchemaFunction afunc = null;

            if (null != dtFunc && dtFunc.Rows.Count > 0)
            {
                afunc = new DBSchemaFunction();
                this.FillRoutineData(afunc, dtFunc.Rows[0]);

                DataTable dtSprocParams = this.GetRoutineParams(con, funcref);
                if (null != dtSprocParams)
                    this.FillRoutineParams(afunc, dtSprocParams);

                for (int i = afunc.Parameters.Count - 1; i >= 0; i--)
                {
                    DBSchemaParameter p = afunc.Parameters[i];
                    if (p.Direction == ParameterDirection.ReturnValue)
                    {
                        afunc.ReturnParameter = p;
                        afunc.Parameters.Remove(p);
                    }
                }
            }

            return afunc;
        }

        #endregion

        //shared routine methods

        #region protected virtual DataTable GetRoutineData(DbConnection con, DBSchemaItemRef routineref)
        /// <summary>
        /// Gets the routine data (name, schema owner, catalog etc) for the specified reference
        /// </summary>
        /// <param name="con"></param>
        /// <param name="routineref"></param>
        /// <returns></returns>
        protected virtual DataTable GetRoutineData(DbConnection con, DBSchemaItemRef routineref)
        {
            DBSchemaMetaDataCollection routinecol = this.AssertGetCollectionForType(DBMetaDataCollectionType.Procedures);
            return this.GetCollectionData(con, routinecol, routineref.Catalog, routineref.Schema, routineref.Name);
        }

        #endregion

        #region protected virtual void FillRoutineData(DBSchemaRoutine aroutine, DataRow dataRow)
        /// <summary>
        /// populates the values of the routine from the row - returned via GetRoutineData
        /// </summary>
        /// <param name="aroutine"></param>
        /// <param name="dataRow"></param>
        protected virtual void FillRoutineData(DBSchemaRoutine aroutine, DataRow dataRow)
        {
            DataColumn catalog = GetColumn(dataRow.Table, "ROUTINE_CATALOG", false);
            DataColumn schema = GetColumn(dataRow.Table, "ROUTINE_SCHEMA", false);
            DataColumn name = GetColumn(dataRow.Table, "ROUTINE_NAME", true);

            aroutine.Name = GetColumnStringValue(dataRow, name);
            aroutine.Schema = GetColumnStringValue(dataRow, schema);
            aroutine.Catalog = GetColumnStringValue(dataRow, catalog);
        }

        #endregion

        #region protected virtual DataTable GetRoutineParams(DbConnection con, DBSchemaItemRef routineref)

        /// <summary>
        /// Loads the Routine (function or procedure) parameters for the specified reference
        /// </summary>
        /// <param name="con"></param>
        /// <param name="routineref"></param>
        /// <returns></returns>
        protected virtual DataTable GetRoutineParams(DbConnection con, DBSchemaItemRef routineref)
        {
            DBSchemaMetaDataCollection routinecol = this.AssertGetCollectionForType(DBMetaDataCollectionType.ProcedureParameters);
            return this.GetCollectionData(con, routinecol, routineref.Catalog, routineref.Schema, routineref.Name);
        }

        #endregion

        #region protected virtual void FillRoutineParams(DBSchemaRoutine aroutine, DataTable dtSprocParams)

        /// <summary>
        /// Populates a routine with the specified parameters in the data table
        /// </summary>
        /// <param name="aroutine"></param>
        /// <param name="dtSprocParams"></param>
        protected virtual void FillRoutineParams(DBSchemaRoutine aroutine, DataTable dtSprocParams)
        {
            DataColumn pos = GetColumn(dtSprocParams, "ORDINAL_POSITION", true);
            DataColumn direction = GetColumn(dtSprocParams, "PARAMETER_MODE", true);
            DataColumn isResult = GetColumn(dtSprocParams, "IS_RESULT", false);
            DataColumn name = GetColumn(dtSprocParams, "PARAMETER_NAME", true);
            DataColumn type = GetColumn(dtSprocParams, "DATA_TYPE", true);
            DataColumn strSize = GetColumn(dtSprocParams, "CHARACTER_MAXIMUM_LENGTH", false);

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
                param.DbType = GetDbTypeForNativeType(GetColumnStringValue(row, type));
                param.RuntimeType = GetSystemTypeForNativeType(GetColumnStringValue(row, type));
                param.ParameterSize = GetColumnIntValue(row, strSize, -1);

                aroutine.Parameters.Add(param);
            }
        }

        #endregion

        #region protected virtual ParameterDirection GetParameterDirectionFromSchemaValue(string dirvalue)

        /// <summary>
        /// Parses the provider specific direction string value to a ParameterDirection enumeration value
        /// </summary>
        /// <param name="dirvalue"></param>
        /// <returns></returns>
        protected virtual ParameterDirection GetParameterDirectionFromSchemaValue(string dirvalue)
        {
            ParameterDirection direction;
            switch (dirvalue.ToUpper())
            {
                case "IN":
                case "INPUT":
                    direction = ParameterDirection.Input;
                    break;
                case "OUT":
                case "OUTPUT":
                    direction = ParameterDirection.Output;
                    break;
                case "RET":
                case "RETURN":
                    direction = ParameterDirection.ReturnValue;
                    break;
                default:
                    direction = ParameterDirection.InputOutput;
                    break;
            }

            return direction;
        }

        #endregion

        //
        // support methods
        //

        // cache methods

        #region protected bool TryGetReferencesFromCache(DBSchemaTypes type, out List<DBSchemaItemRef> all)

        /// <summary>
        /// Attempts to retrieve the list of items from the internal cache
        /// </summary>
        /// <param name="type"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        protected bool TryGetReferencesFromCache(DBSchemaTypes type, out List<DBSchemaItemRef> all)
        {
            return _cache.TryGetValue(type, out all);
        }

        #endregion

        #region protected void AddToCache(DBSchemaTypes type, List<DBSchemaItemRef> tocache)

        /// <summary>
        /// Adds the specified items to the internal cache based on the type - replaces any existing references for the type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tocache"></param>
        protected void AddToCache(DBSchemaTypes type, List<DBSchemaItemRef> tocache)
        {
            if (_cache.ContainsKey(type))
                _cache.Remove(type);
            _cache.Add(type, tocache);
        }

        #endregion

        //type matching

        #region protected virtual DbType GetDbTypeForNativeType(string providerDataType)
        /// <summary>
        /// Gets the DbType for the provider data type
        /// </summary>
        /// <param name="providerDataType"></param>
        /// <returns></returns>
        protected virtual DbType GetDbTypeForNativeType(string providerDataType)
        {
            DbType dbtype = DbType.Object;
            if (string.IsNullOrEmpty(providerDataType))
                dbtype = DbType.Object;
            else
            {
                switch (providerDataType.ToLower())
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
                    case ("money"):
                    case ("smallmoney"):
                        dbtype = DbType.Currency;
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
                    case ("real"):
                        dbtype = DbType.Single;
                        break;
                    case ("float"):
                    case ("double"):
                        dbtype = DbType.Double;
                        break;
                    case ("decimal"):
                    case ("numeric"):
                        dbtype = DbType.Decimal;
                        break;
                    case ("smalldatetime"):
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
                        dbtype = DbType.Binary;
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
                    case ("nchar"):
                        dbtype = DbType.StringFixedLength;
                        break;
                    case ("ntext"):
                    case ("nvarchar"):
                        dbtype = DbType.String;
                        break;
                    case ("binary"):
                    case ("varbinary"):
                    case ("image"):
                    case ("blob"):
                        dbtype = DbType.Binary;
                        break;
                    case ("uniqueidentifier"):
                        dbtype = DbType.Guid;
                        break;
                    case ("xml"):
                        dbtype = DbType.Xml;
                        break;
                    default:
                        dbtype = DbType.Object;
                        break;
                }
            }
            return dbtype;
        }

        #endregion

        #region protected virtual Type GetSystemTypeForNativeType(string providerDataType)
        /// <summary>
        /// Gets the System Type for the provider data type
        /// </summary>
        /// <param name="providerDataType"></param>
        /// <returns></returns>
        protected virtual Type GetSystemTypeForNativeType(string providerDataType)
        {
            System.Type type;
            if (string.IsNullOrEmpty(providerDataType))
                type = typeof(Object);
            else
            {
                switch (providerDataType.ToLower())
                {
                    case ("tinyint"):
                        type = typeof(SByte);
                        break;
                    case ("smallint"):
                        type = typeof(Int16);
                        break;
                    case ("int"):
                        type = typeof(Int32);
                        break;
                    case ("bigint"):
                        type = typeof(Int64);
                        break;
                    case ("money"):
                    case ("smallmoney"):
                        type = typeof(Double);
                        break;
                    case ("tinyint unsigned"):
                        type = typeof(Byte);
                        break;
                    case ("smallint unsigned"):
                        type = typeof(UInt16);
                        break;
                    case ("mediumint unsigned"):
                    case ("int unsigned"):
                        type = typeof(UInt32);
                        break;
                    case ("bigint unsigned"):
                        type = typeof(UInt64);
                        break;
                    case ("bit"):
                        type = typeof(Boolean);
                        break;
                    case ("real"):
                        type = typeof(Single);
                        break;
                    case ("float"):
                    case ("double"):
                        type = typeof(Double);
                        break;
                    case ("decimal"):
                    case ("numeric"):
                        type = typeof(Decimal);
                        break;
                    case ("datetime"):
                    case ("smalldatetime"):
                    case ("time"):
                    case ("date"):
                        type = typeof(DateTime);
                        break;

                    case ("binary"):
                    case ("varbinary"):
                    case ("image"):
                    case ("blob"):
                    case ("timestamp"):
                        type = typeof(byte[]);
                        break;
                    case ("year"):
                        type = typeof(UInt16);
                        break;
                    case ("varchar"):
                    case ("text"):
                    case ("enum"):
                    case ("set"):
                    case ("nchar"):
                    case ("ntext"):
                    case ("nvarchar"):
                    case ("char"):
                        type = typeof(String);
                        break;
                    case ("uniqueidentifier"):
                        type = typeof(Guid);
                        break;
                    case ("xml"):
                        type = typeof(System.Xml.XmlNode);
                        break;
                    default:
                        type = typeof(Object);
                        break;
                }
            }
            return type;
        }

        #endregion


        // get meta data collections

        #region private void InitMetaDataCollections()

        /// <summary>
        /// Loads all the meta data collections for this instances database connection
        /// </summary>
        private void InitMetaDataCollections()
        {
            DbConnection con = null;
            try
            {
                con = this.Database.CreateConnection();
                con.Open();
                this._collections = new DBSchemaMetaDataCollectionSet();
                this.LoadMetaDataCollections(con, this._collections);
                //this.LoadRestrictions(con);
            }
            catch (Exception ex)
            {
                throw new System.Data.DataException("The current provider does not appear to support meta data analysis. See the inner connection for more details", ex);
            }
            finally
            {
                if (con != null)
                    con.Dispose();
            }
        }

        #endregion

        #region protected DBSchemaMetaDataCollection AssertGetCollectionForType(DBMetaDataCollectionType type)

        /// <summary>
        /// Attempts to retrieve the Meta Data Collection with the specified name, 
        /// throwing an exception if the collection is not supported
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected DBSchemaMetaDataCollection AssertGetCollectionForType(DBMetaDataCollectionType type)
        {
            if (type == DBMetaDataCollectionType.Other)
                throw new ArgumentOutOfRangeException("Cannot get collection data for the 'Other' type");
            
            DBSchemaMetaDataCollection col = this.Collections[type];

            if (null == col)
                throw new NotSupportedException("The collection type '" + type.ToString() + "' is not supported by the Database Provider");

            return col;
        }

        #endregion

        #region protected DBSchemaMetaDataCollection AssertGetCollectionForName(string typename)

        /// <summary>
        /// Attempts to retrieve the Meta Data Collection with the specified name, 
        /// throwing an exception if the collection is not supported
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        protected DBSchemaMetaDataCollection AssertGetCollectionForName(string typename)
        {
            if (string.IsNullOrEmpty(typename))
                throw new ArgumentNullException("typename");

            DBSchemaMetaDataCollection col = this.Collections[typename];

            if (null == col)
                throw new NotSupportedException("The collection type '" + typename + "' is not supported by the Database Provider");

            return col;
        }

        #endregion

        #region protected bool TryGetCollectionForType(DBMetaDataCollectionType type, out DBSchemaMetaDataCollection col)

        /// <summary>
        /// Attempts to retrieve the Meta Data Collection with the specified type, 
        /// returning true if the collection was found otherwise false. DBMetaDataCollectionType.Other will always return false
        /// </summary>
        /// <param name="type"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        protected bool TryGetCollectionForType(DBMetaDataCollectionType type, out DBSchemaMetaDataCollection col)
        {
            if (type == DBMetaDataCollectionType.Other)
            {
                col = null;
                return false;
            }

            col = this.Collections[type];

            if (null == col)
                return false;
            else
                return true;
        }

        #endregion

        #region protected bool TryGetCollectionForType(string typename, out DBSchemaMetaDataCollection col)

        /// <summary>
        /// Attempts to retrieve the Meta Data Collection with the specified name, 
        /// returning true if the collection was found otherwise false. DBMetaDataCollectionType.Other will always return false
        /// </summary>
        /// <param name="typename"></param>
        /// <param name="col">The collection to check</param>
        /// <returns></returns>
        protected bool TryGetCollectionForType(string typename, out DBSchemaMetaDataCollection col)
        {
            if (string.IsNullOrEmpty(typename))
            {
                col = null;
                return false;
            }

            col = this.Collections[typename];

            if (null == col)
                return false;
            else
                return true;
        }

        #endregion

        #region protected DataTable GetCollectionData(DBMetaDataCollectionType type) + 5 overloads
        /// <summary>
        /// returns the meta data information from the database for the requested criteria.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected DataTable GetCollectionData(DBMetaDataCollectionType type)
        {
            DBSchemaMetaDataCollection col = this.AssertGetCollectionForType(type);
            return GetCollectionData(col);
        }

        /// <summary>
        /// returns the meta data information from the database for the requested criteria.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        protected DataTable GetCollectionData(DBMetaDataCollectionType type, params string[] restrictions)
        {
            DBSchemaMetaDataCollection col = this.AssertGetCollectionForType(type);
            return GetCollectionData(col, restrictions);
        }
        /// <summary>
        /// returns the meta data information from the database for the requested criteria.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        protected DataTable GetCollectionData(DBSchemaMetaDataCollection collection)
        {
            using (DbConnection con = this.Database.CreateConnection())
            {
                con.Open();
                return GetCollectionData(con, collection);
            }
        }

        /// <summary>
        /// returns the meta data information from the database for the requested criteria.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        protected DataTable GetCollectionData(DBSchemaMetaDataCollection collection, params string[] restrictions)
        {
            using (DbConnection con = this.Database.CreateConnection())
            {
                con.Open();
                return GetCollectionData(con, collection, restrictions);
            }
        }

        /// <summary>
        /// returns the meta data information from the database for the requested criteria.
        /// </summary>
        /// <param name="con"></param>
        /// <param name="collection"></param>
        /// <returns></returns>
        protected DataTable GetCollectionData(DbConnection con, DBSchemaMetaDataCollection collection)
        {
            return this.GetCollectionData(con, collection, new string[] { });

            
        }

        /// <summary>
        /// returns the meta data information from the database for the requested criteria. 
        /// Inheritors can override this method, and all overloads will call this method
        /// </summary>
        /// <param name="con"></param>
        /// <param name="collection"></param>
        /// <param name="restrictions"></param>
        /// <returns></returns>
        protected virtual DataTable GetCollectionData(DbConnection con, DBSchemaMetaDataCollection collection, params string[] restrictions)
        {
            //convert empty strings to null
            if (null != restrictions)
            {
                for (int i = 0; i < restrictions.Length; i++)
                {
                    string s = restrictions[i];
                    if (string.IsNullOrEmpty(s))
                        s = null;
                }
            }

            DataTable data = con.GetSchema(collection.CollectionName, restrictions);
            if (null != data)
            {
                if (string.IsNullOrEmpty(data.TableName))
                    data.TableName = collection.CollectionName;

                WriteCollectionData(collection.CollectionName, restrictions, data);
            }
            return data;
        }

        /// <summary>
        /// Writes the collection data as an xml file if  the 'DEBUG' compilation option is set and also
        /// #def OUTPUTCOLLECTIONDATA = true. Data is writen to the constant OUTPUTDATAPATH path
        /// 
        /// </summary>
        /// <param name="collectionname"></param>
        /// <param name="restrictions"></param>
        /// <param name="data"></param>
        [System.Diagnostics.Conditional("DEBUG")]
        protected static void WriteCollectionData(string collectionname, string[] restrictions, DataTable data)
        {
            if (OUTPUTCOLLECTIONDATA)
            {
                string file = collectionname.Replace('\\', '_').Replace('/', '_');
                foreach (string s in restrictions)
                {
                    if (string.IsNullOrEmpty(s) == false)
                        file += "_" + s.Replace('\\', '_').Replace('/', '_');
                }
                data.WriteXml(OUTPUTDATAPATH + file + ".xml");
            }
        }

        #endregion

        #region protected virtual DBMetaDataCollectionType GetMetDataCollectionTypeForName(string collectionName)
        
        /// <summary>
        /// Maps the name of the Metadata collection to the standard meta data collection type. 
        /// Not all types are supported and will return the type 'Other' for these
        /// </summary>
        /// <param name="collectionName">The name of the collection to identify</param>
        /// <returns>The specific type or 'Other'</returns>
        protected virtual DBMetaDataCollectionType GetMetDataCollectionTypeForName(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName))
                return DBMetaDataCollectionType.Other;
            DBMetaDataCollectionType type;

            switch (collectionName.ToLower().Trim())
            {
                case ("metadatacollections"):
                    type = DBMetaDataCollectionType.MetaDataCollections;
                    break;

                case ("datasourceinformation"):
                    type = DBMetaDataCollectionType.DataSourceInformation;
                    break;

                case ("databases"):
                    type = DBMetaDataCollectionType.Databases;
                    break;

                case ("tables"):
                    type = DBMetaDataCollectionType.Tables;
                    break;

                case ("columns"):
                    type = DBMetaDataCollectionType.Columns;
                    break;

                case ("views"):
                    type = DBMetaDataCollectionType.Views;
                    break;

                case ("viewcolumns"):
                    type = DBMetaDataCollectionType.ViewColumns;
                    break;

                case ("procedures"):
                    type = DBMetaDataCollectionType.Procedures;
                    break;
                case ("procedure parameters"):
                case ("procedureparameters"):
                    type = DBMetaDataCollectionType.ProcedureParameters;
                    break;
                case ("foreign keys"):
                case ("foreignkeys"):
                    type = DBMetaDataCollectionType.ForeignKeys;
                    break;

                case ("indexes"):
                    type = DBMetaDataCollectionType.Indexes;
                    break;
                //case("foreign key columns"):
                //    type = DBMetaDataCollectionType.ForeignKeys

                case ("indexcolumns"):
                    type = DBMetaDataCollectionType.IndexColumns;
                    break;

                default:
                    type = DBMetaDataCollectionType.Other;
                    break;
            }

            return type;
        }

        #endregion


        //item ref loading

        #region protected void LoadItemRefs(DataTable data, IList<DBSchemaItemRef> intoCollection ...)

        /// <summary>
        /// Loads all item references from the data table and populates a DBSchemaItemRef for each 
        /// with the specified columns and types into the provided collection
        /// </summary>
        /// <param name="data"></param>
        /// <param name="intoCollection"></param>
        /// <param name="catalogcol"></param>
        /// <param name="schemacol"></param>
        /// <param name="namecol"></param>
        /// <param name="type"></param>
        protected void LoadItemRefs(DataTable data, IList<DBSchemaItemRef> intoCollection,
            DataColumn catalogcol, DataColumn schemacol, DataColumn namecol, DBSchemaTypes type)
        {
            foreach (DataRow dr in data.Rows)
            {
                DBSchemaItemRef iref = LoadAnItemRef(dr, catalogcol, schemacol, namecol, type);
                intoCollection.Add(iref);
            }
        }

        
        #endregion

        #region protected DBSchemaItemRef LoadAnItemRef(DataRow dr ...)

        /// <summary>
        /// Creates and populates a single item references from the 
        /// provided data row using the specfied columns and type
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="catalogcol"></param>
        /// <param name="schemacol"></param>
        /// <param name="namecol"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected DBSchemaItemRef LoadAnItemRef(DataRow dr,
            DataColumn catalogcol, DataColumn schemacol, DataColumn namecol, DBSchemaTypes type)
        {
            string name = GetColumnStringValue(dr, namecol);
            string schema = GetColumnStringValue(dr, schemacol);
            string catalog = GetColumnStringValue(dr, catalogcol);
            DBSchemaItemRef iref = new DBSchemaItemRef(type, catalog, schema, name);
            return iref;
        }

        #endregion

        #region protected virtual void LoadItemRefsWithContainer(DataTable data ...)

        /// <summary>
        /// Creates and populates all item references using the specified columns to extract information
        /// </summary>
        /// <param name="data"></param>
        /// <param name="intoCollection"></param>
        /// <param name="catalogcol"></param>
        /// <param name="schemacol"></param>
        /// <param name="namecol"></param>
        /// <param name="type"></param>
        /// <param name="containercatalogcol"></param>
        /// <param name="containerschemacol"></param>
        /// <param name="containernamecol"></param>
        /// <param name="containertype"></param>
        protected virtual void LoadItemRefsWithContainer(DataTable data, IList<DBSchemaItemRef> intoCollection,
            DataColumn catalogcol, DataColumn schemacol, DataColumn namecol, DBSchemaTypes type,
            DataColumn containercatalogcol, DataColumn containerschemacol, DataColumn containernamecol, 
            DBSchemaTypes containertype)
        {
            Dictionary<string, DBSchemaItemRef> containers = new Dictionary<string, DBSchemaItemRef>();

            foreach (DataRow dr in data.Rows)
            {
                DBSchemaItemRef iref = LoadAnItemRefWithContainer(dr, catalogcol, schemacol, namecol, type,
                                        containercatalogcol, containerschemacol, containernamecol,
                                        containers, containertype);
                if (null != iref)
                    intoCollection.Add(iref);
            }
        }

        #endregion

        #region protected DBSchemaItemRef LoadAnItemRefWithContainer(DataRow dr ...)

        /// <summary>
        /// Creates and populates a single item references from the 
        /// provided data row using the specfied columns and type and then 
        /// sets the items container based upon the parameters
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="catalogcol"></param>
        /// <param name="schemacol"></param>
        /// <param name="namecol"></param>
        /// <param name="type"></param>
        /// <param name="containercatalogcol"></param>
        /// <param name="containerschemacol"></param>
        /// <param name="containernamecol"></param>
        /// <param name="containers"></param>
        /// <param name="containertype"></param>
        /// <returns></returns>
        protected DBSchemaItemRef LoadAnItemRefWithContainer(DataRow dr,
            DataColumn catalogcol, DataColumn schemacol, DataColumn namecol, DBSchemaTypes type,
            DataColumn containercatalogcol, DataColumn containerschemacol, DataColumn containernamecol,
            Dictionary<string,DBSchemaItemRef> containers, DBSchemaTypes containertype)
        {

            string name = GetColumnStringValue(dr, namecol);
            string schema = GetColumnStringValue(dr, schemacol);
            string catalog = GetColumnStringValue(dr, catalogcol);
            DBSchemaItemRef iref = new DBSchemaItemRef(type, catalog, schema, name);

            //create a new container for the item
            string containercatalog = GetColumnStringValue(dr, containercatalogcol);
            string containerschema = GetColumnStringValue(dr, containerschemacol);
            string containername = GetColumnStringValue(dr, containernamecol);
            DBSchemaItemRef container = new DBSchemaItemRef(containertype, containercatalog, containerschema, containername);

            //check the container to see if it has already been created and assigned 
            DBSchemaItemRef exist;
            if (containers.TryGetValue(container.ToString(), out exist))
                //it has so just set the items container to it
                iref.Container = exist;
            else
            {
                //it has not so add it to the dictionary and set it as the
                //items container.
                containers.Add(container.ToString(), container);
                iref.Container = container;
            }
            
            return iref;
        }

        #endregion

        // name conversion

        #region protected virtual string GetFullName(DBSchemaItemRef iref)

        /// <summary>
        /// Gets a provider specific full name for the item reference
        /// </summary>
        /// <param name="iref"></param>
        /// <returns></returns>
        /// <remarks>This implementation uses the default square brackets as name terminators. 
        /// Inheritors may override this method to provide their own implementation or terminators</remarks>
        protected virtual string GetFullName(DBSchemaItemRef iref)
        {
            return "[" + iref.FullName.Replace(".", "].[") + "]";
        }

        #endregion

        #region protected virtual string GetInvariantParameterName(string pname)

        /// <summary>
        /// Removes the provider specific parameter name character(s) from the native name
        /// </summary>
        /// <param name="pname"></param>
        /// <returns></returns>
        /// <remarks>The default implementation will remove an @ sign or ? from the front of the native name
        /// If this does not support the provider specific implementation then inheritors should override this
        /// method to strip their own character(s)</remarks>
        protected virtual string GetInvariantParameterName(string pname)
        {
            if (string.IsNullOrEmpty(pname))
                return pname;
            else if (pname.StartsWith("@"))
                return pname.Substring(1);
            else if (pname.StartsWith("?"))
                return pname.Substring(1);
            else
                return pname;
        }

        #endregion


        //
        //static methods
        //


        //schema type checking

        #region protected static bool IncludesSchemaType(DBSchemaTypes compare, DBSchemaTypes required)

        /// <summary>
        /// Checks the compare types flags to see if the required falg is set
        /// </summary>
        /// <param name="compare"></param>
        /// <param name="required"></param>
        /// <returns></returns>
        protected static bool IncludesSchemaType(DBSchemaTypes compare, DBSchemaTypes required)
        {
            return (compare & required) > 0;
        }

        #endregion


        // get columns and column values helper methods

        #region public static DataColumn GetColumn(DataTable table, string columnName, bool required)

        /// <summary>
        /// Attempts to get a DataCoulmn from the specified table. 
        /// If not found and the required flag is set then a NullReferenceException is thrown otherwise null is returned
        /// </summary>
        /// <param name="table">The table whose collection of columns are to be searched</param>
        /// <param name="columnName">The name of the column to be found</param>
        /// <param name="required">True if this method should ensure that the column exists</param>
        /// <returns>The matching DataColumn, or null if it is not in the table and the column is not required</returns>
        public static DataColumn GetColumn(DataTable table, string columnName, bool required)
        {
            if (table.Columns.Contains(columnName))
                return table.Columns[columnName];
            else if (!required)
                return null;
            else
                throw new NullReferenceException("A column with the name '" + columnName + "' could not be found in the schema table");
        }

        #endregion

        #region public static string GetColumnStringValue(DataRow row, DataColumn column, string defaultValue) + 1 overload

        /// <summary>
        /// Gets the string value from the row for the specified column. 
        /// If the data is not a string type then it is converted to a string (using the ToString() method)
        /// </summary>
        /// <param name="row">The row to  get the data from</param>
        /// <param name="column">The column within the row to return the data for</param>
        /// <returns>A string representation of the data</returns>
        /// <remarks>If the column is null then an empty string is returned, but if the row is null an Argumentexception is raised </remarks>
        public static string GetColumnStringValue(DataRow row, DataColumn column)
        {
            if (null == row)
                throw new ArgumentNullException("row");

            if (null == column)
                return string.Empty;
            else
                return row[column].ToString();
        }

        /// <summary>
        /// Gets the string value from the row for the specified column. If it's value is null then the default value is used
        /// If the data is not a string type then it is converted to a string (using the ToString() method)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetColumnStringValue(DataRow row, DataColumn column, string defaultValue)
        {
            if (null == column)
                return defaultValue;
            else
            {
                string val = row[column].ToString();
                if (string.IsNullOrEmpty(val))
                    return defaultValue;
                else
                    return val;
            }
        }

        #endregion

        #region public static int GetColumnIntValue(DataRow row, DataColumn column, int defaultValue) + 1 overload

        /// <summary>
        /// Attempts to get the integer value for the specified column in the provided row.
        /// </summary>
        /// <param name="row">The row to get the column value from</param>
        /// <param name="column">The column to get the value for</param>
        /// <returns>The integer value of the row, or -1</returns>
        /// <remarks>If the row is null then an ArgumentNullException is raised.
        /// If the column is null, or the row does not contain the specified colum, or the 
        /// value cannot be converted to an integer then -1 is returned</remarks>
        public static int GetColumnIntValue(DataRow row, DataColumn column)
        {
            return GetColumnIntValue(row, column, -1);
        }

        /// <summary>
        /// Attempts to get the integer value for the specified column in the provided row.
        /// </summary>
        /// <param name="row">The row to get the column value from</param>
        /// <param name="column">The column to get the value for</param>
        /// <param name="defaultValue">The default value to return 
        /// if the actual value cannot be converted to an integer</param>
        /// <returns>The integer value of the row, or the default value</returns>
        /// <remarks>If the row is null then an ArgumentNullException is raised.
        /// If the column is null, or the row does not contain the specified colum, or the 
        /// value cannot be converted to an integer then the default value is returned</remarks>
        public static int GetColumnIntValue(DataRow row, DataColumn column, int defaultValue)
        {
            int value;
            if (null == row)
                throw new ArgumentNullException("row");

            if (null == column)
                return defaultValue;
            else if (int.TryParse(GetColumnStringValue(row, column), out value))
                return value;
            else
                return defaultValue;
        }

        #endregion

        #region public static int GetColumnBoolValue(DataRow row, DataColumn column, int defaultValue) + 1 overload

        /// <summary>
        /// Attempts to get the bool value for the specified column in the provided row.
        /// </summary>
        /// <param name="row">The row to get the column value from</param>
        /// <param name="column">The column to get the value for</param>
        /// <returns>The boolean value of the row, or false</returns>
        /// <remarks>If the row is null then an ArgumentNullException is raised.
        /// If the column is null, or the row does not contain the specified colum, or the 
        /// value cannot be converted to an boolean then false is returned</remarks>
        public static bool GetColumnBoolValue(DataRow row, DataColumn column)
        {
            return GetColumnBoolValue(row, column, false);
        }

        /// <summary>
        /// Attempts to get the boolean value for the specified column in the provided row.
        /// </summary>
        /// <param name="row">The row to get the column value from</param>
        /// <param name="column">The column to get the value for</param>
        /// <param name="defaultValue">The default value to return 
        /// if the actual value cannot be converted to an boolean</param>
        /// <returns>The boolean value of the row, or the default value</returns>
        /// <remarks>If the row is null then an ArgumentNullException is raised.
        /// If the column is null, or the row does not contain the specified colum, or the 
        /// value cannot be converted to an boolean then the default value is returned</remarks>
        public static bool GetColumnBoolValue(DataRow row, DataColumn column, bool defaultValue)
        {
            bool value;
            if (null == row)
                throw new ArgumentNullException("row");

            if (null == column)
                return defaultValue;
            string str = GetColumnStringValue(row, column);
            if (bool.TryParse(str, out value))
                return value;
            else if (String.Equals(str, "YES", StringComparison.OrdinalIgnoreCase))
                return true;
            else if (String.Equals(str, "NO", StringComparison.OrdinalIgnoreCase))
                return false;
            else
                return defaultValue;
        }

        #endregion

        #region public static double GetColumnRealValue(DataRow row, DataColumn column, int defaultValue) + 1 overload

        /// <summary>
        /// Attempts to get the double value for the specified column in the provided row.
        /// </summary>
        /// <param name="row">The row to get the column value from</param>
        /// <param name="column">The column to get the value for</param>
        /// <returns>The double value of the row, or -1</returns>
        /// <remarks>If the row is null then an ArgumentNullException is raised.
        /// If the column is null, or the row does not contain the specified colum, or the 
        /// value cannot be converted to an double then -1.0 is returned</remarks>
        public static double GetColumnRealValue(DataRow row, DataColumn column)
        {
            return GetColumnRealValue(row, column, -1.0);
        }

        /// <summary>
        /// Attempts to get the double value for the specified column in the provided row.
        /// </summary>
        /// <param name="row">The row to get the column value from</param>
        /// <param name="column">The column to get the value for</param>
        /// <param name="defaultValue">The default value to return 
        /// if the actual value cannot be converted to an double</param>
        /// <returns>The double value of the row, or the default value</returns>
        /// <remarks>If the row is null then an ArgumentNullException is raised.
        /// If the column is null, or the row does not contain the specified colum, or the 
        /// value cannot be converted to a double then the default value is returned</remarks>
        public static double GetColumnRealValue(DataRow row, DataColumn column, double defaultValue)
        {
            double value;
            if (null == row)
                throw new ArgumentNullException("row");

            if (null == column)
                return defaultValue;
            else if (double.TryParse(GetColumnStringValue(row, column), out value))
                return value;
            else
                return defaultValue;
        }

        #endregion

    }
}
