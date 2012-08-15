/*  Copyright 2010 PerceiveIT Limited
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
using System.Data;

namespace Perceiveit.Data.MySqlClient
{
    /// <summary>
    /// Supports the DBProviderImplementation for the "MySql.Data.MySqlClient" provider
    /// </summary>
    public class DBMySqlImplementation : DBProviderImplementation
    {
        private static readonly DbType[] SUPPORTED_TYPES = new DbType[] {DbType.Int32, DbType.Int64, DbType.AnsiString, DbType.AnsiStringFixedLength
                                                                        ,DbType.Boolean, DbType.Byte, DbType.Currency, DbType.DateTime
                                                                        ,DbType.Decimal, DbType.Double, DbType.Guid, DbType.Single
                                                                        ,DbType.String, DbType.StringFixedLength, DbType.Binary};

        private static readonly TopType[] SUPPORTED_TOPTYPES = new TopType[] { TopType.Count, TopType.Range };

        public DBMySqlImplementation()
            : this("MySql.Data.MySqlClient") 
        { 
        }

        public DBMySqlImplementation(string providername)
            : base(providername)
        {
        }

        protected override DBDatabaseProperties GetPropertiesFromDb(DBDatabase forDatabase)
        {
            DBDatabaseProperties props = null;
            string vers;
            string edition = string.Empty;

            string db = this.GetDataSourceNameFromConnection(forDatabase, ';','=',"Database");

            string versionFunction = "SELECT VERSION()";
            
            try
            {
                object value = forDatabase.ExecuteScalar(versionFunction, System.Data.CommandType.Text);
                vers = Convert.ToString(value);
                //format of the version is similar to '5.0.87-standard'
                if (string.IsNullOrEmpty(vers) == false)
                {
                    int index = vers.IndexOf('-');
                    if (index > -1)
                    {
                        edition = vers.Substring(index + 1).Trim();
                        vers = vers.Substring(0, index - 1);
                    }
                }
                TypedOperationCollection unsupported = new TypedOperationCollection();
                                                

                props = new DBDatabaseProperties(db, "MySql",
                                                string.Empty,
                                                edition, "?{0}",
                                                new Version(vers),
                                                SupportedSchemaOptions.All,
                                                false, DBParameterLayout.Named,
                                                SUPPORTED_TYPES, SUPPORTED_TOPTYPES,
                                                DBSchemaInformation.CreateDefault(),
                                                unsupported
                                                );
            }
            catch (Exception ex)
            {
                throw new System.Data.DataException(Errors.CannotGetPropertiesFromDB, ex);
            }
            return props;


        }

        protected override void FillNotSupported(TypedOperationCollection all)
        {
            base.FillNotSupported(all);
            all.Add(new TypedOperation(DBSchemaTypes.Index,DBSchemaOperation.CheckExists));
            all.Add(new TypedOperation(DBSchemaTypes.Index,DBSchemaOperation.CheckNotExists));
            all.Add(new TypedOperation(DBSchemaTypes.View, DBSchemaOperation.CheckNotExists));
        }

        protected override DBStatementBuilder CreateStatementBuilder(DBDatabase forDatabase, DBDatabaseProperties withProperties, System.IO.TextWriter writer, bool ownsWriter)
        {
            return new DBMySqlStatementBuilder(forDatabase, withProperties, writer, ownsWriter);
        }

        protected override DBSchemaProvider CreateSchemaProvider(DBDatabase forDatabase, DBDatabaseProperties properties)
        {
            return new DBMySqlSchemaProvider(forDatabase, properties);
        }
    }
}
