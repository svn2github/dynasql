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

namespace Perceiveit.Data.SqLite
{
    internal class DBSqLiteFactory : DBFactory
    {
        private static readonly DbType[] SUPPORTED_TYPES = new DbType[] {DbType.Int32, DbType.Int64, DbType.AnsiString, DbType.AnsiStringFixedLength
                                                                        ,DbType.Boolean, DbType.Byte, DbType.Currency, DbType.DateTime
                                                                        ,DbType.Decimal, DbType.Double, DbType.Guid, DbType.Single
                                                                        ,DbType.String, DbType.StringFixedLength, DbType.Binary};

        internal DBSqLiteFactory(string providerName)
            : base(providerName)
        {
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

            DBDatabaseProperties props = new DBDatabaseProperties("SQLite", db, "", "@{0}", vers,
                                                SupportedSchemaOptions.TablesViewsAndIndexes | DBSchemaTypes.CommandScripts, false, DBParameterLayout.Named, SUPPORTED_TYPES);
            
            return props;
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
