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
    /// Provider implementations for the 'System.Data.SqLite' database engine
    /// </summary>
    public class DBSqLiteImplementaion : DBProviderImplementation
    {
        private static readonly DbType[] SUPPORTED_TYPES = new DbType[] {DbType.Int32, DbType.Int64, DbType.AnsiString, DbType.AnsiStringFixedLength
                                                                        ,DbType.Boolean, DbType.Byte, DbType.Currency, DbType.DateTime
                                                                        ,DbType.Decimal, DbType.Double, DbType.Guid, DbType.Single
                                                                        ,DbType.String, DbType.StringFixedLength, DbType.Binary};

        private static readonly TopType[] SUPPORTED_TOP = new TopType[] { TopType.Count, TopType.Range };

        

        internal DBSqLiteImplementaion()
            : this("System.Data.SqLite")
        {
        }

        internal DBSqLiteImplementaion(string providerName)
            : base(providerName)
        {
        }


        private string GetDataSourceNameFromConnection(DBDatabase forDatabase)
        {
            string dbname = this.GetDataSourceNameFromConnection(forDatabase, ';', '=', "datasource");
           
            if (!string.IsNullOrEmpty(dbname))
                dbname = System.IO.Path.GetFileNameWithoutExtension(dbname);

            return dbname;
        }

        protected override DBDatabaseProperties GetPropertiesFromDb(DBDatabase forDatabase)
        {
            string db;
            string vers;
            using (DbCommand cmd = forDatabase.CreateCommand("SELECT sqlite_version()", CommandType.Text))
            {
                object versValue = forDatabase.ExecuteScalar(cmd);
                db = cmd.Connection.Database;
                vers = (null == versValue) ? string.Empty : versValue.ToString();
            }

            string dbname = GetDataSourceNameFromConnection(forDatabase);
            TypedOperationCollection unsupported = new TypedOperationCollection();
            this.FillNotSupported(unsupported);

            DBDatabaseProperties props = new DBDatabaseProperties(dbname, "SQLite", db, "", "@{0}", new Version(vers),
                                                SupportedSchemaOptions.TablesViewsAndIndexes | DBSchemaTypes.CommandScripts, false,
                                                DBParameterLayout.Named, SUPPORTED_TYPES, new TopType[] { TopType.Count, TopType.Range },
                                                Schema.DBSchemaInformation.CreateDefault(),
                                                unsupported);
            
            return props;
        }


        /// <summary>
        /// Adds the CreateOn for indexes - cannot use the CREATE INDEX [Name] ON TABLE only supports CREATE INDEX [Name]
        /// </summary>
        /// <param name="all"></param>
        protected override void FillNotSupported(TypedOperationCollection all)
        {
            all.Add(DBSchemaTypes.Index, DBSchemaOperation.CreateOn);
            base.FillNotSupported(all);
        }

        protected override DBStatementBuilder CreateStatementBuilder(DBDatabase forDatabase, DBDatabaseProperties withProperties, System.IO.TextWriter writer, bool ownsWriter)
        {
            return new DBStatementSQLiteBuilder(forDatabase, withProperties, writer, ownsWriter);
        }

        protected override DBSchemaProvider CreateSchemaProvider(DBDatabase forDatabase, DBDatabaseProperties properties)
        {
            return new DBSqLiteSchemaProvider(forDatabase, properties);
        }
    }
}
