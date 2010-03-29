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
using System.Data;

namespace Perceiveit.Data.SqlClient
{
    internal class DBSqlClientFactory : DBFactory
    {
        private static readonly DbType[] SUPPORTED_TYPES = new DbType[] {DbType.Int32, DbType.Int64, DbType.AnsiString, DbType.AnsiStringFixedLength
                                                                        ,DbType.Boolean, DbType.Byte, DbType.Currency, DbType.DateTime
                                                                        ,DbType.Decimal, DbType.Double, DbType.Guid, DbType.Single
                                                                        ,DbType.String, DbType.StringFixedLength, DbType.Binary};
        public DBSqlClientFactory(string providerName)
            : base(providerName)
        {
        }

        
        
        protected override DBDatabaseProperties GetPropertiesFromDb(DBDatabase forDatabase)
        {
            string statement = "SELECT  SERVERPROPERTY('productversion') AS [version], SERVERPROPERTY ('productlevel') AS [level], SERVERPROPERTY ('edition') AS [edition]";
            DBDatabaseProperties props;

            props = (DBDatabaseProperties)forDatabase.ExecuteRead(statement, delegate(System.Data.Common.DbDataReader reader)
            {

                if (reader.Read())
                {
                    return new DBDatabaseProperties("SQL Server",
                            reader["edition"].ToString(),
                            reader["level"].ToString(),
                            "@{0}",
                            reader["version"].ToString(),
                            SupportedSchemaOptions.All,
                            false,
                            DBParameterLayout.Named,
                            SUPPORTED_TYPES);
                }
                else
                {
                    return DBDatabaseProperties.Unknown;

                }
            });
            return props;

        }

        protected override DBStatementBuilder CreateStatementBuilder(DBDatabase forDatabase, DBDatabaseProperties withProperties, System.IO.TextWriter writer, bool ownsWriter)
        {
            return new DBSQLClientStatementBuilder(forDatabase, withProperties, writer, ownsWriter);
        }

        protected override DBSchemaProvider CreateSchemaProvider(DBDatabase forDatabase, DBDatabaseProperties properties)
        {
            return new DBSQLClientSchemaProvider(forDatabase, properties);
        }
    }
}
