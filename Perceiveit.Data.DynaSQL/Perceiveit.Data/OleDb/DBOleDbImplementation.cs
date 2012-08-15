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
using System.Data;

namespace Perceiveit.Data.OleDb
{
    /// <summary>
    /// Defines a DBProviderImplementation for the "System.Data.OleDb" database engine. NOTE: The only fuly supported engine within this is MS Access
    /// and the JET engine. Other's may need to extend this class to provider their own support.
    /// </summary>
    public class DBOleDbImplementation : DBProviderImplementation
    {
        private static readonly DbType[] SUPPORTED_TYPES = new DbType[] {DbType.Int32, DbType.Int64, DbType.AnsiString, DbType.AnsiStringFixedLength
                                                                        ,DbType.Boolean, DbType.Byte, DbType.Currency, DbType.DateTime
                                                                        ,DbType.Decimal, DbType.Double, DbType.Guid, DbType.Single
                                                                        ,DbType.String, DbType.StringFixedLength, DbType.Binary};

        private static readonly DbType[] SUPPORTED_ACCESS_TYPES = new DbType[] {DbType.Int32, DbType.Int64, DbType.AnsiString, DbType.AnsiStringFixedLength
                                                                        ,DbType.Boolean, DbType.Byte, DbType.Currency, DbType.DateTime
                                                                        ,DbType.Decimal, DbType.Double, DbType.Guid, DbType.Single
                                                                        ,DbType.String, DbType.StringFixedLength, DbType.Binary};

        internal const string MSAccessProductName = "MS Access";

        public DBOleDbImplementation()
            : this("System.Data.OleDb")
        {
        }

        public DBOleDbImplementation(string providerName)
            : base(providerName)
        {
        }

        
        
        protected override DBDatabaseProperties GetPropertiesFromDb(DBDatabase forDatabase)
        {
            string con = forDatabase.ConnectionString;
            //looking for 'Provider=Microsoft.ACE.OLEDB.12.0;'
            int index = con.IndexOf("Provider=");
            string provider = string.Empty;

            if (index > -1)
            {
                provider = con.Substring(index + "Provider=".Length);
                index = provider.IndexOf(';');
                if (index > 0)
                    provider = provider.Substring(0, index);
                else
                    provider = string.Empty;
            }
             DBDatabaseProperties props;
            if (!string.IsNullOrEmpty(provider))
            {
                if (this.TryGetProviderSpecificProperties(provider, forDatabase, out props))
                    return props;
            }

            TypedOperationCollection unsupported = new TypedOperationCollection();
            this.FillNotSupported(unsupported);
            string datasource = GetDataSourceNameFromConnection(forDatabase, ';', '=', "data source");
            props = new DBDatabaseProperties(datasource,"OleDb","?", "?", "?{0}", new Version("0.0"),
                SupportedSchemaOptions.TablesViewsAndIndexes, false, DBParameterLayout.Positional,
                SUPPORTED_TYPES, new TopType[] { TopType.Count },
                DBSchemaInformation.CreateDefault(), unsupported);
            return props;

        }


        protected virtual bool TryGetProviderSpecificProperties(string provider, DBDatabase database, out DBDatabaseProperties props)
        {
            props = null;
            switch (provider.ToLower())
            {
                case("microsoft.ace.oledb.12.0"):
                    string datasource = GetDataSourceNameFromConnection(database, ';', '=', "data source");

                    if (!string.IsNullOrEmpty(datasource))
                    {
                        if (datasource.StartsWith("\""))
                            datasource = datasource.Substring(1, datasource.Length - 2);

                        datasource = System.IO.Path.GetFileNameWithoutExtension(datasource);
                    }
                    TypedOperationCollection unsupported = new TypedOperationCollection();
                    this.FillNotSupported(unsupported);
                                                   
                    props = new DBDatabaseProperties(datasource, MSAccessProductName, "Microsoft", "OLEDB", "?", new Version("12.0"),
                            SupportedSchemaOptions.TablesViewsAndIndexes | DBSchemaTypes.StoredProcedure, false, DBParameterLayout.Positional,
                            SUPPORTED_ACCESS_TYPES, new TopType[] { TopType.Count },
                            Schema.DBSchemaInformation.CreateDefault(), unsupported);
                    break;
                default:
                    break;
            }
            return props != null;
        }

        
        protected override void FillNotSupported(TypedOperationCollection all)
        {
            all.Add(new TypedOperation(DBSchemaTypes.CommandScripts, DBSchemaOperation.CheckExists));
            all.Add(new TypedOperation(DBSchemaTypes.Database, DBSchemaOperation.CheckExists));
            all.Add(new TypedOperation(DBSchemaTypes.ForeignKey, DBSchemaOperation.CheckExists));
            all.Add(new TypedOperation(DBSchemaTypes.Function, DBSchemaOperation.CheckExists));
            all.Add(new TypedOperation(DBSchemaTypes.Group, DBSchemaOperation.CheckExists));
            all.Add(new TypedOperation(DBSchemaTypes.Index, DBSchemaOperation.CheckExists));
            all.Add(new TypedOperation(DBSchemaTypes.StoredProcedure, DBSchemaOperation.CheckExists));
            all.Add(new TypedOperation(DBSchemaTypes.Table, DBSchemaOperation.CheckExists));
            all.Add(new TypedOperation(DBSchemaTypes.User, DBSchemaOperation.CheckExists));
            all.Add(new TypedOperation(DBSchemaTypes.View, DBSchemaOperation.CheckExists));

            all.Add(new TypedOperation(DBSchemaTypes.CommandScripts, DBSchemaOperation.CheckNotExists));
            all.Add(new TypedOperation(DBSchemaTypes.Database, DBSchemaOperation.CheckNotExists));
            all.Add(new TypedOperation(DBSchemaTypes.ForeignKey, DBSchemaOperation.CheckNotExists));
            all.Add(new TypedOperation(DBSchemaTypes.Function, DBSchemaOperation.CheckNotExists));
            all.Add(new TypedOperation(DBSchemaTypes.Group, DBSchemaOperation.CheckNotExists));
            all.Add(new TypedOperation(DBSchemaTypes.Index, DBSchemaOperation.CheckExists));
            all.Add(new TypedOperation(DBSchemaTypes.StoredProcedure, DBSchemaOperation.CheckNotExists));
            all.Add(new TypedOperation(DBSchemaTypes.Table, DBSchemaOperation.CheckNotExists));
            all.Add(new TypedOperation(DBSchemaTypes.User, DBSchemaOperation.CheckNotExists));
            all.Add(new TypedOperation(DBSchemaTypes.View, DBSchemaOperation.CheckNotExists));

            base.FillNotSupported(all);
        }

        protected override DBStatementBuilder CreateStatementBuilder(DBDatabase forDatabase, DBDatabaseProperties withProperties, System.IO.TextWriter writer, bool ownsWriter)
        {
            return new DBOleDbStatementBuilder(forDatabase, withProperties, writer, ownsWriter);
        }

        protected override DBSchemaProvider CreateSchemaProvider(DBDatabase forDatabase, DBDatabaseProperties properties)
        {
            if (null != properties && properties.ProductName == MSAccessProductName)
                return new DBMSAccessSchemaProvider(forDatabase, properties);
            else
                throw new NotSupportedException("The Schema provider is not supported for the product '" + properties.ProductName + "'");
        }
    }
}
