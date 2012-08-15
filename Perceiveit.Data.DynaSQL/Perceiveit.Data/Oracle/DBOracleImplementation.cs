using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Perceiveit.Data.Query;


namespace Perceiveit.Data.Oracle
{
    public class DBOracleImplementation : DBProviderImplementation
    {
        private static readonly DbType[] SUPPORTED_TYPES = new DbType[] {DbType.Int32, DbType.Int64, DbType.AnsiString, DbType.AnsiStringFixedLength
                                                                        ,DbType.Boolean, DbType.Byte, DbType.Currency, DbType.DateTime
                                                                        ,DbType.Decimal, DbType.Double, DbType.Guid, DbType.Single
                                                                        ,DbType.String, DbType.StringFixedLength, DbType.Binary};

        private static readonly TopType[] SUPPORTED_TOP_TYPES = new TopType[] { TopType.Count, TopType.Range };


        public const string OracleProviderName = "System.Data.OracleClient";

        public DBOracleImplementation()
            : this(OracleProviderName)
        {
        }

        public DBOracleImplementation(string providername)
            : base(providername)
        {
        }

        protected override DBDatabaseProperties GetPropertiesFromDb(DBDatabase forDatabase)
        {
            TypedOperationCollection unsupported = new TypedOperationCollection();
            this.FillNotSupported(unsupported);
            return new DBDatabaseProperties(forDatabase.Name, "Oracle", "?", "?", ":{0}", new Version(0, 0),
                 SupportedSchemaOptions.All & ~DBSchemaTypes.CommandScripts, true, DBParameterLayout.Named,
                 SUPPORTED_TYPES, SUPPORTED_TOP_TYPES,
                 Schema.DBSchemaInformation.CreateOracle(), unsupported);

        }

        protected override Query.DBStatementBuilder CreateStatementBuilder(DBDatabase forDatabase, DBDatabaseProperties withProperties, System.IO.TextWriter writer, bool ownsWriter)
        {
            return new DBOracleStatementBuilder(forDatabase, withProperties, writer, ownsWriter);
        }

        protected override Schema.DBSchemaProvider CreateSchemaProvider(DBDatabase forDatabase, DBDatabaseProperties properties)
        {
            return new DBOracleSchemaProvider(forDatabase, properties);
        }
    }
}
