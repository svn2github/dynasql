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
using Perceiveit.Data.Schema;
using Perceiveit.Data.Query;

namespace Perceiveit.Data
{
    public abstract class DBFactory
    {
        //
        // static implementation
        //

        #region static vars

        private static object _lock;
        private static Dictionary<string, DBFactory> _factories;

        #endregion

        #region static ..ctor

        /// <summary>
        /// Static constructor that initializes the factories
        /// </summary>
        static DBFactory()
        {
            _lock = new object();
            _factories = new Dictionary<string, DBFactory>(StringComparer.InvariantCultureIgnoreCase);

            _factories.Add("System.Data.SqlClient", new SqlClient.DBSqlClientFactory("System.Data.SqlClient"));
            _factories.Add("System.Data.SqLite", new SqLite.DBSqLiteFactory("System.Data.SqLite"));
            _factories.Add("MySql.Data.MySqlClient", new MySqlClient.DBMySqlFactory("System.Data.MySqlClient"));
            _factories.Add("System.Data.OleDb", new OleDb.DBOleDbFactory("System.Data.OleDb"));
        }

        #endregion

        #region public static void RegisterFactory(string providername, DBFactory factory) + 1 overload

        /// <summary>
        /// Enables the dynamic registration of a new DBFactory tat can be used for a specific provider
        /// </summary>
        /// <param name="factory"></param>
        public static void RegisterFactory(DBFactory factory)
        {
            if (null == factory)
                throw new ArgumentNullException("factory");

            RegisterFactory(factory.ProviderName, factory);
        }

        /// <summary>
        /// Enables the dynamic registration of a new DBFactory that can be used with a specific provider
        /// </summary>
        /// <param name="providername">The name of the provider that the DBFactory is registered against</param>
        /// <param name="factory">The factory to register</param>
        public static void RegisterFactory(string providername, DBFactory factory)
        {
            if (null == factory)
                throw new ArgumentNullException("factory");
            
            if (string.IsNullOrEmpty(providername))
                throw new ArgumentNullException("providername");

            lock (_lock)
            {
                if(_factories.ContainsKey(providername))
                    _factories[providername] = factory;
                else
                    _factories.Add(providername,factory);
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
                contains = _factories.ContainsKey(providerName);
            }
            return contains;
        }

        #endregion

        #region public static DBFactory GetFactory(string providerName)

        /// <summary>
        /// Gets the DBFactory associated with the specified provider name. It is an error if there is no factory registered
        /// </summary>
        /// <param name="providerName">The provider specific name for a Database</param>
        /// <returns>The known DBFactory</returns>
        public static DBFactory GetFactory(string providerName)
        {
            DBFactory fact;

            lock (_lock)
            {
                _factories.TryGetValue(providerName, out fact);
            }

            if(null == fact)
                throw new ArgumentException(string.Format(Errors.InvalidProviderNameForConnection, providerName), "providerName");

            return fact;
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
        /// Gets the provider name for this DBFactory
        /// </summary>
        public string ProviderName
        {
            get { return this._providername; }
        }

        #endregion

        //
        // .ctor
        //

        #region protected DBFactory(string providername)

        /// <summary>
        /// Protected constructor for the DBFactory
        /// </summary>
        /// <param name="providername"></param>
        protected DBFactory(string providername)
        {
            this._providername = providername;
        }

        #endregion 

        //
        // public methods
        //

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
            if (_knownProps.TryGetValue(forDatabase.ConnectionString, out properties) == false)
            {
                properties = this.GetPropertiesFromDb(forDatabase);
                _knownProps[forDatabase.ConnectionString] = properties;
            }
            return properties;
        }

        #endregion


        //
        // abstract methods
        //

        protected abstract DBDatabaseProperties GetPropertiesFromDb(DBDatabase forDatabase);

        protected abstract DBStatementBuilder CreateStatementBuilder(DBDatabase forDatabase, DBDatabaseProperties withProperties, System.IO.TextWriter writer, bool ownsWriter);

        protected abstract DBSchemaProvider CreateSchemaProvider(DBDatabase forDatabase, DBDatabaseProperties properties);
    }
}
