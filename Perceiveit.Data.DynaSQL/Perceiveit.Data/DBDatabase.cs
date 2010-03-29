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

namespace Perceiveit.Data
{
    public partial class DBDatabase
    {
        private DbProviderFactory _factory;
        private string _connectionStr;
        private string _provname;
        private DBDatabaseProperties _properties;

        
        
        #region public string ConnectionString {get;}

        public string ConnectionString
        {
            get { return _connectionStr; }
        }

        #endregion

        #region protected DbProviderFactory Factory {get;}

        protected DbProviderFactory Factory
        {
            get { return this._factory; }
        }

        #endregion

        #region public string ProviderName {get;}

        public string ProviderName
        {
            get { return _provname; }
        }

        #endregion


        //
        // .ctors
        //

        #region protected DBDataBase(string connection, string providerName, DbProviderFactory provider)

        protected DBDatabase(string connection, string providerName, DbProviderFactory provider)
        {
            this._connectionStr = connection;
            this._factory = provider;
            this._provname = providerName;
        }

        #endregion

        //
        // database properties and schema
        //

        #region public DBDatabaseProperties GetProperties()

        public DBDatabaseProperties GetProperties()
        {
            if (_properties == null)
                _properties = this.CreateProperties();
            return _properties;

        }

        #endregion

        #region public DBSchemaProvider GetSchemaProvider()

        /// <summary>
        /// Gets the DBSchemaProvider for this database
        /// </summary>
        /// <returns></returns>
        public DBSchemaProvider GetSchemaProvider()
        {
            return this.CreateSchemaProvider();
        }

        #endregion

        //
        // Execute read methods
        //


        #region public object ExecuteRead(string text, DBCallback callback)

        public object ExecuteRead(string text, DBCallback callback)
        {
            object returns;

            using (DbCommand cmd = this.CreateCommand(text))
            {
                cmd.Connection.Open();

                using (System.Data.Common.DbDataReader reader = cmd.ExecuteReader())
                {
                    returns = callback(reader);
                }
            }

            return returns;
        }

        #endregion

        #region public object ExecuteRead(string text, object context, DBContextCallback populator)

        public object ExecuteRead(string text, object context, DBContextCallback populator)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            if (null == populator)
                throw new ArgumentNullException("populator");

            object returns;

            using (DbCommand cmd = this.CreateCommand(text))
            {
                cmd.Connection.Open();

                using (System.Data.Common.DbDataReader reader = cmd.ExecuteReader())
                {
                    returns = populator(reader, context);
                }
            }

            return returns;
        }

        #endregion

        #region public object ExecuteRead(string text, CommandType type, DBCallback callback)

        public object ExecuteRead(string text, CommandType type, DBCallback callback)
        {
            object returns;

            using (DbCommand cmd = this.CreateCommand(text, type))
            {
                cmd.Connection.Open();

                using (System.Data.Common.DbDataReader reader = cmd.ExecuteReader())
                {
                    returns = callback(reader);
                }
            }

            return returns;
        }

        #endregion

        #region public object ExecuteRead(string text, CommandType type, object context, DBContextCallback populator)

        public object ExecuteRead(string text, CommandType type, object context, DBContextCallback populator)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            if (null == populator)
                throw new ArgumentNullException("populator");

            object returns;

            using (DbCommand cmd = this.CreateCommand(text, type))
            {
                cmd.Connection.Open();

                using (System.Data.Common.DbDataReader reader = cmd.ExecuteReader())
                {
                    returns = populator(reader, context);
                }
            }

            return returns;
        }

        #endregion

        #region public object ExecuteRead(DbCommand cmd, DBCallback populator)

        public object ExecuteRead(DbCommand cmd, DBCallback populator)
        {
            object returns = null;
            DbDataReader reader = null;
            bool opened = false;
            
            if(cmd == null)
                throw new ArgumentNullException("cmd");
            
            if (cmd.Connection == null)
                throw new ArgumentNullException("cmd.Connection");

            if (populator == null)
                throw new ArgumentNullException("callback");

            

            try
            {
                if (cmd.Connection.State == ConnectionState.Closed)
                {
                    cmd.Connection.Open();
                    opened = true;
                }

                reader = cmd.ExecuteReader();

                returns = populator(reader);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null != reader)
                    reader.Dispose();

                if (opened)
                    cmd.Connection.Close();
            }

            return returns;
        }

        #endregion

        #region public object ExecuteRead(DbCommand cmd, object context, DBContextCallback populator)

        public object ExecuteRead(DbCommand cmd, object context, DBContextCallback populator)
        {
            object returns = null;
            DbDataReader reader = null;
            bool opened = false;

            if (cmd == null)
                throw new ArgumentNullException("cmd");

            if (cmd.Connection == null)
                throw new ArgumentNullException("cmd.Connection");

            if (populator == null)
                throw new ArgumentNullException("callback");



            try
            {
                if (cmd.Connection.State == ConnectionState.Closed)
                {
                    cmd.Connection.Open();
                    opened = true;
                }

                reader = cmd.ExecuteReader();

                returns = populator(reader, context);

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (null != reader)
                    reader.Dispose();

                if (opened)
                    cmd.Connection.Close();
            }

            return returns;
        }

        #endregion

        #region public object ExecuteRead(DBQuery query, DBCallback populator)

        public object ExecuteRead(DBQuery query, DBCallback populator)
        {
            object returns;

            using (DbCommand cmd = this.CreateCommand(query))
            {
                cmd.Connection.Open();

                using (System.Data.Common.DbDataReader reader = cmd.ExecuteReader())
                {
                    returns = populator(reader);
                }
            }

            return returns;
        }

        #endregion

        #region public object ExecuteRead(DBQuery query, object context, DBContextCallback populator)

        public object ExecuteRead(DBQuery query, object context, DBContextCallback populator)
        {
            object returns;

            using (DbCommand cmd = this.CreateCommand(query))
            {
                cmd.Connection.Open();

                using (System.Data.Common.DbDataReader reader = cmd.ExecuteReader())
                {
                    returns = populator(reader, context);
                }
            }

            return returns;
        }

        #endregion

        #region public object ExecuteRead(DbTransaction transaction, DBQuery query, DBCallback populator)

        public object ExecuteRead(DbTransaction transaction, DBQuery query, DBCallback populator)
        {
            object returns;

            using (DbCommand cmd = this.CreateCommand(transaction, query))
            {
                using (System.Data.Common.DbDataReader reader = cmd.ExecuteReader())
                {
                    returns = populator(reader);
                }
            }

            return returns;
        }

        #endregion

        #region public object ExecuteRead(DbTransaction transaction, DBQuery query, object context, DBContextCallback populator)

        public object ExecuteRead(DbTransaction transaction, DBQuery query, object context, DBContextCallback populator)
        {
            object returns;

            using (DbCommand cmd = this.CreateCommand(transaction, query))
            {
                using (System.Data.Common.DbDataReader reader = cmd.ExecuteReader())
                {
                    returns = populator(reader, context);
                }
            }

            return returns;
        }

        #endregion


        //
        // ExecuteScalar methods
        //

        #region public object ExecuteScalar(DBQuery query)

        public object ExecuteScalar(DBQuery query)
        {
            object returns;

            using (DbCommand cmd = this.CreateCommand(query))
            {
                cmd.Connection.Open();

                returns = cmd.ExecuteScalar();
            }

            return returns;
        }

        #endregion

        #region public object ExecuteScalar(DbConnection connection, DBQuery query)

        public object ExecuteScalar(DbConnection connection, DBQuery query)
        {
            object returns;

            using (DbCommand cmd = this.CreateCommand(connection, query))
            {
                returns = cmd.ExecuteScalar();
            }

            return returns;
        }

        #endregion

        #region public object ExecuteScalar(DbTransaction transaction, DBQuery query)

        public object ExecuteScalar(DbTransaction transaction, DBQuery query)
        {
            object returns;

            using (DbCommand cmd = this.CreateCommand(transaction, query))
            {
                returns = cmd.ExecuteScalar();
            }

            return returns;
        }

        #endregion

        #region public object ExecuteScalar(string text)

        public object ExecuteScalar(string text)
        {
            object result;

            using (DbCommand cmd = this.CreateCommand(text))
            {
                cmd.Connection.Open();
                result = cmd.ExecuteScalar();
            }

            return result;
        }

        #endregion

        #region public object ExecuteScalar(string text, CommandType type)

        public object ExecuteScalar(string text, CommandType type)
        {
            object result;

            using (DbCommand cmd = this.CreateCommand(text, type))
            {
                result = this.ExecuteScalar(cmd);
            }

            return result;
        }

        #endregion

        #region public object ExecuteScalar(DbCommand cmd)

        public object ExecuteScalar(DbCommand cmd)
        {
            object result = null;
            bool opened = false;

            try
            {
                if (cmd.Connection.State == ConnectionState.Closed)
                {
                    cmd.Connection.Open();
                    opened = true;
                }
                result = cmd.ExecuteScalar();

            }
            finally
            {
                if (opened)
                    cmd.Connection.Close();
            }

            return result;
        }

        #endregion

        //
        // ExecuteNonQuery methods
        //

        #region public int ExecuteNonQuery(string text)

        public int ExecuteNonQuery(string text)
        {
            int result;

            using (DbCommand cmd = this.CreateCommand(text))
            {
                cmd.Connection.Open();
                result = cmd.ExecuteNonQuery();
            }

            return result;
        }

        #endregion

        #region public int ExecuteNonQuery(string text, CommandType type)

        public int ExecuteNonQuery(string text, CommandType type)
        {
            int result;

            using (DbCommand cmd = this.CreateCommand(text, type))
            {
                result = this.ExecuteNonQuery(cmd);
            }

            return result;
        }

        #endregion

        #region public int ExecuteNonQuery(DbCommand cmd)

        public int ExecuteNonQuery(DbCommand cmd)
        {
            int result = 0;
            bool opened = false;

            try
            {
                if (cmd.Connection.State == ConnectionState.Closed)
                {
                    cmd.Connection.Open();
                    opened = true;
                }
                result = cmd.ExecuteNonQuery();

            }
            finally
            {
                if (opened)
                    cmd.Connection.Close();
            }

            return result;
        }

        #endregion


        #region public int ExecuteNonQuery(DBQuery query)

        public int ExecuteNonQuery(DBQuery query)
        {
            int returns;

            using (DbCommand cmd = this.CreateCommand(query))
            {
                cmd.Connection.Open();

                returns = cmd.ExecuteNonQuery();
            }

            return returns;

        }

        #endregion

        #region public int ExecuteNonQuery(DbConnection connection, DBQuery query)

        public int ExecuteNonQuery(DbConnection connection, DBQuery query)
        {
            int returns;

            using (DbCommand cmd = this.CreateCommand(connection, query))
            {
                returns = cmd.ExecuteNonQuery();
            }

            return returns;
        }

        #endregion

        #region public int ExecuteNonQuery(DbTransaction transaction, DBQuery query)

        public int ExecuteNonQuery(DbTransaction transaction, DBQuery query)
        {
            int returns;

            using (DbCommand cmd = this.CreateCommand(transaction, query))
            {
                returns = cmd.ExecuteNonQuery();
            }

            return returns;
        }

        #endregion

        //
        // CreateConnection
        //

        #region public virtual DbConnection CreateConnection()

        public virtual DbConnection CreateConnection()
        {
            DbConnection con = this.Factory.CreateConnection();
            con.ConnectionString = this.ConnectionString;
            return con;
        }

        #endregion

        //
        // create command methods
        //

        #region public DbCommand CreateCommand(string text)

        public DbCommand CreateCommand(string text)
        {
            return CreateCommand(text, CommandType.Text);
        }

        #endregion

        #region public DbCommand CreateCommand(string text, CommandType type)

        public DbCommand CreateCommand(string text, CommandType type)
        {
            DbConnection con = this.CreateConnection();
            DbCommand cmd = this.CreateCommand(con, text);
            cmd.CommandType = type;
            cmd.Disposed += new EventHandler(DisposeCommandOwnedConnection);

            return cmd;
        }

        #endregion

        #region public DbCommand CreateCommand(DbTransaction transaction, string text)

        public DbCommand CreateCommand(DbTransaction transaction, string text)
        {
            DbCommand cmd = this.Factory.CreateCommand();
            cmd.Connection = transaction.Connection;

            //check for owned transaction (from a call to BeginTransaction)
            if (transaction is DBOwningTransaction)
                cmd.Transaction = ((DBOwningTransaction)transaction).InnerTransaction;
            else
                cmd.Transaction = transaction;
            cmd.CommandText = text;

            return cmd;
        }

        #endregion

        #region public virtual DbCommand CreateCommand(DbConnection connection, string text)

        public DbCommand CreateCommand(DbConnection connection, string text)
        {
            DbCommand cmd = this.Factory.CreateCommand();
            cmd.Connection = connection;
            cmd.CommandText = text;

            return cmd;
        }

        #endregion

        #region public DbCommand CreateCommand(DBQuery query)

        public DbCommand CreateCommand(DBQuery query)
        {
            DbConnection con = this.CreateConnection();
            DbCommand cmd = this.CreateCommand(con, query);
            cmd.Disposed += new EventHandler(DisposeCommandOwnedConnection);
            return cmd;
        }

        #endregion

        #region public DbCommand CreateCommand(DbTransaction transaction, DBQuery query)

        public DbCommand CreateCommand(DbTransaction transaction, DBQuery query)
        {
            DbCommand cmd = this.CreateCommand(transaction.Connection, query);

            if (transaction is DBOwningTransaction)
                cmd.Transaction = ((DBOwningTransaction)transaction).InnerTransaction;
            else
                cmd.Transaction = transaction;

            return cmd;
        }

        #endregion

        #region public virtual DbCommand CreateCommand(DbConnection connection, DBQuery query)

        public virtual DbCommand CreateCommand(DbConnection connection, DBQuery query)
        {
            DbCommand cmd;

            using (DBStatementBuilder sb = this.CreateStatementBuilder())
            {
                query.BuildStatement(sb);
                string cmdtext = sb.ToString().Trim();
                cmd = this.Factory.CreateCommand();
                cmd.Connection = connection;
                cmd.CommandText = cmdtext;
                cmd.CommandType = query.GetCommandType();

                if (sb.HasParameters)
                {
                    foreach (DBStatementBuilder.StatementParameter sparam in sb.Parameters)
                    {
                        DbParameter param = sb.CreateCommandParameter(cmd, sparam);
                        cmd.Parameters.Add(param);
                    }
                }
            }

            return cmd;
        }

        #endregion

        //
        // begin transaction
        //

        #region public DbTransaction BeginTransaction()

        public virtual DbTransaction BeginTransaction()
        {
            DbConnection con = this.CreateConnection();
            return new DBOwningTransaction(con);
        }

        #endregion

        //
        // add command parameter
        //

        #region public DbParameter AddCommandParameter(DbCommand cmd, string genericParameterName, DbType dbType)

        public DbParameter AddCommandParameter(DbCommand cmd, string genericParameterName, DbType dbType)
        {
            DbParameter p = cmd.CreateParameter();
            p.ParameterName = this.GetParameterName(genericParameterName);
            p.DbType = dbType;
            p.Direction = ParameterDirection.Input;
            cmd.Parameters.Add(p);
            return p;
        }

        #endregion

        #region public DbParameter AddCommandParameter(DbCommand cmd, string genericParameterName, DbType dbType, ParameterDirection direction)

        public DbParameter AddCommandParameter(DbCommand cmd, string genericParameterName,
                                               DbType dbType, ParameterDirection direction)
        {
            DbParameter p = cmd.CreateParameter();
            p.ParameterName = this.GetParameterName(genericParameterName);
            p.DbType = dbType;
            p.Direction = direction;
            cmd.Parameters.Add(p);
            return p;
        }

        #endregion

        #region public DbParameter AddCommandParameter(DbCommand cmd, string genericParameterName, DbType dbType, int size)

        public DbParameter AddCommandParameter(DbCommand cmd, string genericParameterName,
                                               DbType dbType, int size)
        {
            DbParameter p = cmd.CreateParameter();
            p.ParameterName = this.GetParameterName(genericParameterName);
            p.DbType = dbType;
            p.Size = size;
            cmd.Parameters.Add(p);
            return p;
        }

        #endregion

        #region public DbParameter AddCommandParameter(DbCommand cmd, string genericParameterName, DbType dbType, int size, ParameterDirection direction)

        public DbParameter AddCommandParameter(DbCommand cmd, string genericParameterName,
                                               DbType dbType, int size, ParameterDirection direction)
        {
            DbParameter p = cmd.CreateParameter();
            p.ParameterName = this.GetParameterName(genericParameterName);
            p.DbType = dbType;
            p.Direction = direction;
            p.Size = size;
            cmd.Parameters.Add(p);
            return p;
        }

        #endregion

        //
        // assign parameter value
        //

        #region public void AssignParameterValue(DbParameter param, object value)

        public void AssignParameterValue(DbParameter param, object value)
        {
            if (null == param)
                throw new ArgumentNullException("param");

            param.Value = value;

        }

        #endregion

        #region public void AssignParameterValue(DbCommand cmd, string genericParameterName, object value)

        public void AssignParameterValue(DbCommand cmd, string genericParameterName, object value)
        {
            if (null == cmd)
                throw new ArgumentNullException("cmd");

            DbParameter p = cmd.Parameters[GetParameterName(genericParameterName)];

            if (null == p)
                throw new NullReferenceException(string.Format(Errors.NoParameterWithGenericName, genericParameterName));

            p.Value = value;
        }

        #endregion


        //
        // support methods
        //

        #region protected string[] ExtractTableNames(DataSet ds)
        /// <summary>
        /// Creates an array of all the names of the tables in the dataset in the same order
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        protected string[] ExtractTableNames(DataSet ds)
        {
            if (null == ds)
                throw new ArgumentNullException("ds");

            List<string> all = new List<string>(ds.Tables.Count);
            foreach (DataTable dt in ds.Tables)
            {
                all.Add(dt.TableName);
            }
            return all.ToArray();
        }

        #endregion

        #region private static void DisposeCommandOwnedConnection(object sender, EventArgs e)

        /// <summary>
        /// Static event handler method to dispose of a connection when a DbCommand is disposed of
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DisposeCommandOwnedConnection(object sender, EventArgs e)
        {
            DbCommand cmd = (DbCommand)sender;
            if (cmd.Connection != null)
            {
                System.Diagnostics.Trace.WriteLine("Disposing owned connection for command created by DBDatabase");
                cmd.Connection.Dispose();
            }
        }

        #endregion

        #region protected virtual DBDatabaseProperties CreateProperties()

        protected virtual DBDatabaseProperties CreateProperties()
        {
            DBFactory factory = DBFactory.GetFactory(this.ProviderName);
            DBDatabaseProperties properties = factory.CreateDatabaseProperties(this);

            return properties;
        }

        #endregion

        #region private string GetParameterName(string genericname)

        private string GetParameterName(string genericname)
        {
            if (string.IsNullOrEmpty(genericname))
                throw new ArgumentNullException("genericname");

            return string.Format(this.GetProperties().ParameterFormat,genericname);
        }

        #endregion

        #region public virtual DBStatementBuilder CreateStatementBuilder()

        /// <summary>
        /// Creates the statement builder for this database
        /// </summary>
        /// <returns></returns>
        public virtual DBStatementBuilder CreateStatementBuilder()
        {
            DBFactory factory = DBFactory.GetFactory(this.ProviderName);
            DBStatementBuilder builder = factory.CreateStatementBuilder(this);

            return builder;
        }

        #endregion

        #region protected virtual DBSchemaProvider CreateSchemaProvider()

        protected virtual DBSchemaProvider CreateSchemaProvider()
        {
            DBFactory factory = DBFactory.GetFactory(this.ProviderName);
            DBSchemaProvider schema = factory.CreateSchemaProvider(this);
            
            return schema;
        }

        #endregion

        #region protected void CreateEmptyTables(DataSet ds, string[] tables)

        /// <summary>
        /// Adds the range of empty tables with the correct names as specified
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="tables"></param>
        protected void CreateEmptyTables(DataSet ds, string[] tables)
        {
            if (null == tables)
                return;
            for (int i = 0; i < tables.Length; i++)
            {
                string table = tables[i];
                if (string.IsNullOrEmpty(table))
                    throw new ArgumentNullException("tables[" + i.ToString() + "]");
                DataTable dt = new DataTable(table);
                ds.Tables.Add(dt);
            }
        }

        #endregion

        //
        // support classes
        //


        #region private class DBOwningTransaction : DbTransaction

        /// <summary>
        /// Transaction wrapper class to close of the connection 
        /// when the transaction is disposed.
        /// </summary>
        private class DBOwningTransaction : DbTransaction
        {
            private DbConnection _con;
            private bool _opened;
            private bool _active;
            private DbTransaction _innerTranny;

            public DbTransaction InnerTransaction
            {
                get { return this._innerTranny; }
            }

            public DBOwningTransaction(DbConnection ownedConnection)
            {
                this._con = ownedConnection;
                if (_con == null)
                    throw new ArgumentException("Cannot create an transaction without a valid connection");
                if (_con.State == System.Data.ConnectionState.Closed)
                {
                    _con.Open();
                    _opened = true;
                }
                else
                    _opened = false;

                this._innerTranny = _con.BeginTransaction();
                this._active = true;
            }

            public override void Commit()
            {
                this._innerTranny.Commit();
                this._active = false;
            }

            protected override DbConnection DbConnection
            {
                get { return this._con; }
            }

            public override System.Data.IsolationLevel IsolationLevel
            {
                get { return this._innerTranny.IsolationLevel; }
            }

            public override void Rollback()
            {
                this._innerTranny.Rollback();
                this._active = false;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_innerTranny != null)
                    {
                        //this transaction has not been committed
                        //or rolled back so default behaviour is
                        //to rollback, should happen when the
                        //inner transaction is disposed, but just
                        //to make sure do it now
                        if (_active)
                            _innerTranny.Rollback();

                        _innerTranny.Dispose();
                    }
                    if (_opened)
                        _con.Close();
                }

                base.Dispose(disposing);
            }
        }

        #endregion


        //
        // static factory methods
        //

        #region public static DBDataBase Create(string connectionStringName)

        /// <summary>
        /// Creates a new DBDatabase using the ConnectionStrings values set in the configuration file with the name specified
        /// </summary>
        /// <param name="connectionStringName">The name of the connection strings section to get the connection details from</param>
        /// <returns>A new DBDatabase</returns>
        public static DBDatabase Create(string connectionStringName)
        {
            if (string.IsNullOrEmpty(connectionStringName))
                throw new ArgumentNullException("connectionStringName", Errors.NoConnectionStringName);

            System.Configuration.ConnectionStringSettings settings = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName];

            if (settings == null)
                throw new ArgumentNullException("connectionStringName", String.Format(Errors.NoConfiguredConnectionWithName, connectionStringName));

            string con = settings.ConnectionString;
            string prov = settings.ProviderName;

            if (string.IsNullOrEmpty(con))
                throw new ArgumentNullException("connectionStringName", String.Format(Errors.ConnectionStringNotSetInConfig, connectionStringName));

            if (string.IsNullOrEmpty(prov))
                throw new ArgumentNullException("connectionStringName", String.Format(Errors.ProviderNameNotSetInConfig, connectionStringName));

            System.Data.Common.DbProviderFactory fact = System.Data.Common.DbProviderFactories.GetFactory(prov);

            if (null == fact)
                throw new ArgumentNullException("connectionStringName", String.Format(Errors.InvalidProviderNameInConfigForConnection, prov, connectionStringName));

            return new DBDatabase(con, prov, fact);
        }

        #endregion

        #region public static DBDataBase Create(string connectionString, string providername)

        /// <summary>
        /// Creates a new DBDatabase with the specified connectionString and provider name
        /// </summary>
        /// <param name="connectionString">The connection string to the required SQL Database</param>
        /// <param name="providername">The name of a valid DBProviderFactory to use when creating connections, commands etc.</param>
        /// <returns>A new DBDatabase</returns>
        public static DBDatabase Create(string connectionString, string providername)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString", String.Format(Errors.ConnectionStringNotSet));

            if (string.IsNullOrEmpty(providername))
                throw new ArgumentNullException("providername", String.Format(Errors.ProviderNameNotSet));

            System.Data.Common.DbProviderFactory fact = System.Data.Common.DbProviderFactories.GetFactory(providername);

            if (null == fact)
                throw new ArgumentNullException("providername", String.Format(Errors.InvalidProviderNameForConnection, providername));

            return new DBDatabase(connectionString, providername, fact);
        }

        #endregion


    }
}
