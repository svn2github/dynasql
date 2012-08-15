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
using Perceiveit.Data.Schema;
using Perceiveit.Data.Query;
using Perceiveit.Data.Configuration;
using System.Data.Common;

namespace Perceiveit.Data
{
    /// <summary>
    /// Base class for all provider specific DBProviderImplementations. 
    /// The DBProviderImplementation enasbles the creation of provider specific 
    /// Statement builders, schema providers etc. <br/>
    /// Use the static GetImplementation(providername) method to retrieve an instance
    /// </summary>
    /// <remarks>Only one instance of a DBProviderImplementation should ever be created for an AppDomain.
    /// In the normal course of using the library you should not have to access this class at all, but use the
    /// DBDatabase instead. Only if you need to do some
    /// </remarks>
    public abstract class DBProviderImplementation
    {
        //
        // static implementation
        //

        #region static vars

        private static object _lock; //threadsafe lock
        
        #endregion

        #region static ..ctor

        /// <summary>
        /// Static constructor that initializes the known factories
        /// </summary>
        static DBProviderImplementation()
        {
            _lock = new object();
        }

        #endregion

        #region private static DBProviderImplementationConfigSection ConfiguredImplementations {get;}

        /// <summary>
        /// Gets the configured implementations for this application domain
        /// </summary>
        private static DBProviderImplementationConfigSection ConfiguredImplementations
        {
            get
            {
                return DBConfigurationSection.GetSection().Implementations;
            }
        }

        #endregion

        #region public static void RegisterFactory(string providername, DBFactory factory) + 1 overload

        /// <summary>
        /// Enables the dynamic registration of a new DBFactory tat can be used for a specific provider
        /// </summary>
        /// <param name="imp"></param>
        public static void RegisterImplementation(DBProviderImplementation imp)
        {
            if (null == imp)
                throw new ArgumentNullException("imp");

            RegisterImplementation(imp.ProviderName, imp);
        }

        /// <summary>
        /// Enables the dynamic registration of a new DBFactory that can be used with a specific provider
        /// </summary>
        /// <param name="providername">The name of the provider that the DBFactory is registered against</param>
        /// <param name="imp">The factory to register</param>
        public static void RegisterImplementation(string providername, DBProviderImplementation imp)
        {
            if (null == imp)
                throw new ArgumentNullException("factory");
            
            if (string.IsNullOrEmpty(providername))
                throw new ArgumentNullException("providername");

            lock (_lock)
            {
                if (ConfiguredImplementations.Contains(providername))
                    ConfiguredImplementations.Remove(providername);

                ConfiguredImplementations.Add(providername, imp);
                
            }
        }

        #endregion

        #region public static bool IsFactoryRegistered(string providerName)

        /// <summary>
        /// Checks the currently registered DBFactories and returns true if 
        /// there is a DBFactory set against this provider
        /// </summary>
        /// <param name="providerName">The name of the provider</param>
        /// <returns>True if a factory is registered otherwise false</returns>
        public static bool IsFactoryRegistered(string providerName)
        {
            bool contains;

            lock (_lock)
            {
                contains = ConfiguredImplementations.Contains(providerName);
            }
            return contains;
        }

        #endregion

        #region public static DBProviderImplementation GetImplementation(string providerName)

        /// <summary>
        /// Gets the DBProviderImplementation associated with the specified provider name. 
        /// It is an error if there is no implementation registered for the requested provider. Thread safe
        /// </summary>
        /// <param name="providerName">The provider specific name for a Database</param>
        /// <returns>The known DBProviderImplementation</returns>
        /// <remarks>The get implementation is thread safe, and will throw an ArgumentException if it is not known.</remarks>
        public static DBProviderImplementation GetImplementation(string providerName)
        {
            DBProviderImplementation imp;

            lock (_lock)
            {
                imp = ConfiguredImplementations.Get(providerName);
            }

            if(null == imp)
                throw new ArgumentException(string.Format(Errors.InvalidProviderNameForConnection, providerName), "providerName");

            return imp;
        }

        #endregion

        //
        // instance implementation
        //


        #region ivars

        private string _providername;
        private Dictionary<string, DBDatabaseProperties> _knownProps = new Dictionary<string, DBDatabaseProperties>();

        
        #endregion

        //
        // public properties
        //

        #region public string ProviderName {get;}

        /// <summary>
        /// Gets the provider name for this DBProviderImplementation
        /// </summary>
        public string ProviderName
        {
            get { return this._providername; }
        }

        #endregion

        //
        // .ctor
        //

        #region protected DBProviderImplementation(string providername)

        /// <summary>
        /// Protected constructor for the DBProviderImplementation
        /// </summary>
        /// <param name="providername"></param>
        protected DBProviderImplementation(string providername)
        {
            this._providername = providername;
        }

        #endregion 

        //
        // public methods
        //

        #region public virtual DBDatabase CreateDatabase(string connection, string providerName, DbProviderFactory factory)

        /// <summary>
        /// Instantiates a new DBDatabase with the specified parameters and returns the instance to the caller. 
        /// </summary>
        /// <param name="connection">The connection string the DBDatabase should use</param>
        /// <param name="providerName">The connections' provider name (e.g. System.Data.SqlClient)</param>
        /// <param name="factory">The common .NET DbProviderFactory</param>
        /// <returns></returns>
        public virtual DBDatabase CreateDatabase(string connection, string providerName, DbProviderFactory factory)
        {
            if (string.IsNullOrEmpty(connection))
                throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentNullException("providerName");
            if (null == factory)
                throw new ArgumentNullException("factory");

            return new DBDatabase(connection, providerName, factory);
        }

        #endregion

        #region public DBSchemaProvider CreateSchemaProvider(DBDataBase forDatabase)

        /// <summary>
        /// Creates a new SchemaProvider for the specified database
        /// </summary>
        /// <param name="forDatabase"></param>
        /// <returns></returns>
        public DBSchemaProvider CreateSchemaProvider(DBDatabase forDatabase)
        {
            if (null == forDatabase)
                throw new ArgumentNullException("forDatabase");
            if (string.IsNullOrEmpty(forDatabase.ConnectionString))
                throw new ArgumentNullException("forDatabase.ConnectionString");

            DBDatabaseProperties properties = this.CreateDatabaseProperties(forDatabase);

            return this.CreateSchemaProvider(forDatabase, properties);
        }

        #endregion

        #region public DBStatementBuilder CreateStatementBuilder(DBDataBase forDatabase, System.IO.TextWriter textWriter)

        /// <summary>
        /// Creates a new StatementBuilder for the specified database 
        /// using the textWriter as a backing store
        /// </summary>
        /// <param name="forDatabase"></param>
        /// <param name="textWriter"></param>
        /// <returns></returns>
        public DBStatementBuilder CreateStatementBuilder(DBDatabase forDatabase, System.IO.TextWriter textWriter)
        {
            if (null == forDatabase)
                throw new ArgumentNullException("forDatabase");
            if (string.IsNullOrEmpty(forDatabase.ConnectionString))
                throw new ArgumentNullException("forDatabase.ConnectionString");

            if (null == textWriter)
                throw new ArgumentNullException("textWriter");

            DBDatabaseProperties properties = this.CreateDatabaseProperties(forDatabase);

            return this.CreateStatementBuilder(forDatabase, properties, textWriter, false);
        }

        /// <summary>
        /// Creates a new StatementBuilder for the specified database 
        /// </summary>
        /// <param name="forDatabase"></param>
        /// <returns></returns>
        public DBStatementBuilder CreateStatementBuilder(DBDatabase forDatabase)
        {
            DBStatementBuilder builder = null;
            System.IO.StringWriter writer = new System.IO.StringWriter();

            try
            {
                builder = CreateStatementBuilder(forDatabase, writer);
            }
            catch (Exception)
            {
                // if we failed to initialize the builder 
                // then we need to close the writer
                // and re-throw the exception.

                writer.Dispose();
                throw;
            }

            return builder;
            
        }



        #endregion

        #region public virtual DBDatabaseProperties CreateDatabaseProperties(DBDataBase forDatabase)

        /// <summary>
        /// Creates the Set of DatabaseProperties for the specified Database
        /// </summary>
        /// <param name="forDatabase"></param>
        /// <returns></returns>
        public virtual DBDatabaseProperties CreateDatabaseProperties(DBDatabase forDatabase)
        {
            if (null == forDatabase)
                throw new ArgumentNullException("forDatabase");
            if (string.IsNullOrEmpty(forDatabase.ConnectionString))
                throw new ArgumentNullException("forDatabase.ConnectionString");

            DBDatabaseProperties properties;

            //check to see if we have already retrieved the properties
            //this is not thread safe, but the impact is multiple calls to the database rather than blocking 
            //a number of threads - so considered appropriate
            string key = forDatabase.ProviderName + "|" + forDatabase.ConnectionString;
            if (_knownProps.TryGetValue(key, out properties) == false)
            {
                properties = this.GetPropertiesFromDb(forDatabase);
                _knownProps[key] = properties;
            }
            
            return properties;
        }

        #endregion


        protected virtual void FillNotSupported(TypedOperationCollection all)
        {
            Array values = Enum.GetValues(typeof(DBSchemaOperation));

            //We don't support databases 
            DBSchemaTypes type = DBSchemaTypes.Database;
            foreach (DBSchemaOperation op in values)
            {
                all.Add(type, op);
            }

            //We don't support users
            type = DBSchemaTypes.User;
            foreach (DBSchemaOperation op in values)
            {
                all.Add(type, op);
            }

            //we don't support groups or roles
            type = DBSchemaTypes.Group;
            foreach (DBSchemaOperation op in values)
            {
                all.Add(type, op);
            }

            //can't create functions
            type = DBSchemaTypes.Function;
            foreach (DBSchemaOperation op in values)
            {
                if(op != DBSchemaOperation.Exec)
                    all.Add(type, op);
            }

            //can't create sprocs
            type = DBSchemaTypes.StoredProcedure;
            foreach (DBSchemaOperation op in values)
            {
                if(op != DBSchemaOperation.Exec)
                    all.Add(type, op);
            }
        }

        //
        // support methods
        //

        #region protected System.Collections.Specialized.NameValueCollection SplitConnectionString(string connection, char sectionSeparator, char keyvalueSeparator)

        /// <summary>
        /// Splits a (connection) string into its constituent parts based upon the section separator and key value separator
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="sectionSeparator"></param>
        /// <param name="keyvalueSeparator"></param>
        /// <returns></returns>
        protected virtual System.Collections.Specialized.NameValueCollection SplitConnectionString(string connection, char sectionSeparator, char keyvalueSeparator)
        {
            if (string.IsNullOrEmpty(connection))
                throw new ArgumentNullException("connection");

            string[] all = connection.Split(sectionSeparator);
            System.Collections.Specialized.NameValueCollection nvCol = new System.Collections.Specialized.NameValueCollection(all.Length, StringComparer.OrdinalIgnoreCase);
            foreach (string s in all)
            {
                string[] kv = s.Split(keyvalueSeparator);

                if (kv.Length == 2)
                    nvCol.Add(kv[0].Trim(), kv[1].Trim());

                else if (kv.Length == 1)
                    nvCol.Add(kv[0].Trim(), kv[0].Trim());

                else //more than 2 so rejoin the ones at the end
                {
                    nvCol.Add(kv[0].Trim(), string.Join(keyvalueSeparator.ToString(), kv, 1, kv.Length - 1));
                }
            }
            return nvCol;
        }


        #endregion

        #region protected virtual string GetDataSourceNameFromConnection(DBDatabase forDatabase, char sectionSeparator, char keyValueSeparator, string datasourcekey)
        
        /// <summary>
        /// Extracted the value from the database connection string
        /// </summary>
        /// <param name="forDatabase"></param>
        /// <param name="keyValueSeparator"></param>
        /// <param name="sectionSeparator"></param>
        /// <param name="datasourcekey"></param>
        /// <returns></returns>
        protected virtual string GetDataSourceNameFromConnection(DBDatabase forDatabase, char sectionSeparator, char keyValueSeparator, string datasourcekey)
        {
            if (null == forDatabase)
                throw new ArgumentNullException("forDatabase");
            if (string.IsNullOrEmpty(forDatabase.ConnectionString))
                throw new ArgumentNullException("forDatabase.ConnectionString");

            string dbname = "";
            try
            {
                string con = forDatabase.ConnectionString;
                System.Collections.Specialized.NameValueCollection nvCol;
                nvCol = this.SplitConnectionString(con, sectionSeparator, keyValueSeparator);

                dbname = nvCol[datasourcekey];
            }
            catch (Exception)
            { //we are not looking for errors here 
            }

            return dbname;
        }

        #endregion

        //
        // abstract methods
        //

        /// <summary>
        /// Abstract method that inheritors must override to return a new instance of 
        /// the provider specific database properties
        /// </summary>
        /// <param name="forDatabase">The database (and contained connection) that the properties must represent</param>
        /// <returns>A new instance of the DBDatabaseProperties</returns>
        protected abstract DBDatabaseProperties GetPropertiesFromDb(DBDatabase forDatabase);

        /// <summary>
        /// Abstract method that inheritors must override to return a new instance of
        /// the provider specific database statement builder
        /// </summary>
        /// <param name="forDatabase">The database (and contained connection) that the builder must generate statements for</param>
        /// <param name="withProperties">The properties of the database.</param>
        /// <param name="writer">The TextWriter the statement builder uses to output the SQL statemtent to.</param>
        /// <param name="ownsWriter">true if the new instance owns the text writer and should dispose of it when the builder is disposed</param>
        /// <returns>A new DBStatementBuilder subclass</returns>
        protected abstract DBStatementBuilder CreateStatementBuilder(DBDatabase forDatabase, DBDatabaseProperties withProperties, System.IO.TextWriter writer, bool ownsWriter);

        /// <summary>
        /// Abstract method that inheritors must override to return a new instance of
        /// the provider specific database schema provider
        /// </summary>
        /// <param name="forDatabase">The database (and contained connection) that the builder must generate statements for</param>
        /// <param name="properties">The properties of the database.</param>
        /// <returns>A new schema provider</returns>
        protected abstract DBSchemaProvider CreateSchemaProvider(DBDatabase forDatabase, DBDatabaseProperties properties);

        
    }
}
