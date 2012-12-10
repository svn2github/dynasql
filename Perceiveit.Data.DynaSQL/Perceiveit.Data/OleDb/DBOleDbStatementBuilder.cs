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
using Perceiveit.Data.Query;

namespace Perceiveit.Data.OleDb
{
    /// <summary>
    /// Implements the DBStatementBuilder for the OleDb provider - specifically Ms Access
    /// </summary>
    public class DBOleDbStatementBuilder : DBStatementBuilder
    {

        public DBOleDbStatementBuilder(DBDatabase forDatabase, DBDatabaseProperties properties, System.IO.TextWriter tw, bool ownswriter)
            : base(forDatabase, properties, tw, ownswriter)
        {
        }


        public override void BeginDateLiteral()
        {
            this.WriteRawSQLString("#");
            //base.BeginDateLiteral();
        }

        public override void EndDateLiteral()
        {
            this.WriteRawSQLString("#");
            //base.EndDateLiteral();
        }

        public override void BeginScript()
        {
            if (this.CurrentlyCreating == DBSchemaTypes.StoredProcedure)
            {
                throw new NotSupportedException();
            }
            else
            {
                //this.Writer.Write("BEGIN");
                this.IncrementStatementBlock();
                //this.BeginNewLine();
            }
        }

        public override void EndScript()
        {
            if (this.CurrentlyCreating == DBSchemaTypes.StoredProcedure)
            {
                throw new NotSupportedException();
            }
            else
            {
                this.DecrementStatementBlock();
                //this.BeginNewLine();
                //this.Writer.WriteLine("END");
            }
        }

        public override void WriteStatementTerminator()
        {
            base.WriteStatementTerminator();
        }

        private bool _lastwasIdentityFunction = false;
        public override void BeginFunction(Function function, string name)
        {
            if (function == Function.LastID)
            {
                this.WriteRawSQLString("@@Identity");//don't like this, but it looks like the only way
                                            //any other options then I'd go with them happily
                _lastwasIdentityFunction = true;
            }
            else if (function == Function.GetDate)
            {
                this.WriteRawSQLString("Now");
            }
            else
                base.BeginFunction(function, name);
        }

        public override void BeginFunctionParameterList()
        {
            if (!_lastwasIdentityFunction)
                base.BeginFunctionParameterList(); //Don't put the opening parenthese
        }

        public override void EndFunctionParameterList()
        {
            if (!_lastwasIdentityFunction)
                base.EndFunctionParameterList(); //Don't put the closing parenthese
            _lastwasIdentityFunction = false;
        }

        protected override object ConvertParamValueToNativeValue(System.Data.DbType type, object value)
        {
            object converted;
            if (value is DateTime)
            {
                DateTime dtvalue = (DateTime)value;

                if (type == System.Data.DbType.DateTime)
                {
                    converted = new DateTime(dtvalue.Year, dtvalue.Month, dtvalue.Day, dtvalue.Hour, dtvalue.Minute, dtvalue.Second);
                }
                else if (type == System.Data.DbType.Date)
                {
                    converted = new DateTime(dtvalue.Year, dtvalue.Month, dtvalue.Day);
                }
                else if (type == System.Data.DbType.DateTime2)
                {
                    converted = new DateTime(dtvalue.Year, dtvalue.Month, dtvalue.Day, dtvalue.Hour, dtvalue.Minute, dtvalue.Second);
                }
                else
                    converted = base.ConvertParamValueToNativeValue(type, value);
            }
            else
                converted = base.ConvertParamValueToNativeValue(type, value);

            return converted;
        }

        protected override string GetNativeTypeForDbType(System.Data.DbType dbType, int setSize, int accuracy, DBColumnFlags flags, out string options)
        {
            switch (dbType)
            {

                case System.Data.DbType.Guid:
                    options = "";
                    return " VARCHAR(50)";

                case System.Data.DbType.Int16:
                case System.Data.DbType.Int32:
                case System.Data.DbType.Int64:
                case System.Data.DbType.UInt16:
                case System.Data.DbType.UInt32:
                case System.Data.DbType.UInt64:
                    options = string.Empty;
                    if ((flags & DBColumnFlags.AutoAssign) > 0)
                        return "AUTOINCREMENT";
                    else
                        return " INTEGER";

                default:
                    return base.GetNativeTypeForDbType(dbType, setSize, accuracy, flags, out options);
            }

        }

        public override void WriteColumnFlags(DBColumnFlags flags, DBClause defaultValue)
        {
            if ((flags & DBColumnFlags.HasDefault) > 0)
                throw new NotSupportedException(Errors.OLEDbDoesNotSupportDefaultValues);

            if ((flags & DBColumnFlags.PrimaryKey) > 0)
                this.WriteRawSQLString(" PRIMARY KEY");

            if ((flags & DBColumnFlags.Nullable) > 0)
                this.WriteRawSQLString(" NULL");
            else
                this.WriteRawSQLString(" NOT NULL");
        }

        public override void BeginEntityDefinition()
        {
            this.WriteRawSQLString(" AS ");
        }

        public override void EndEntityDefinition()
        {

        }

        public override void BeginCreate(DBSchemaTypes type, string owner, string name, string options, bool checknotexists)
        {
            if (type == DBSchemaTypes.StoredProcedure)
                throw new NotSupportedException(string.Format(Errors.DatabaseEngineDoesntSupportType, type));

            if (checknotexists)
                throw new InvalidOperationException(String.Format(Errors.DBDoesntSupportIfExistsForThisType, "MS Access", type));

            if (type == DBSchemaTypes.ForeignKey && !string.IsNullOrEmpty(name))
            {
                this.WriteRawSQLString("CONSTRAINT ");
                this.BeginIdentifier();
                this.WriteObjectName(name);
                this.EndIdentifier();
                this.WriteSpace();
            }

            this.BeginCreate(type, options);

            if (checknotexists)
                base.EndCheckNotExists(type, owner, name);


            if (type != DBSchemaTypes.ForeignKey)
                this.WriteSourceTable(owner, name, string.Empty);
        }

        public override void BeginCreate(DBSchemaTypes type, string options)
        {
            if (type == DBSchemaTypes.StoredProcedure)
                throw new NotSupportedException(string.Format(Errors.DatabaseEngineDoesntSupportType, type));

            base.BeginCreate(type, options);

            //Special case where the CREATE VIEW .. AS cannot have parenthese after - causes exception
            if (type == DBSchemaTypes.View)
                this.DecrementStatementDepth();
        }

        public override void BeginDropStatement(DBSchemaTypes type, string owner, string name, bool checkExists)
        {
            if (type == DBSchemaTypes.StoredProcedure)
                throw new NotSupportedException(string.Format(Errors.DatabaseEngineDoesntSupportType, type));

            if (checkExists)
                throw new InvalidOperationException(String.Format(Errors.DBDoesntSupportIfExistsForThisType, "MS Access", type));

            base.BeginDropStatement(type, owner, name, checkExists);
        }


        public override void EndCreate(DBSchemaTypes type, bool checknotexists)
        {
            if (type == DBSchemaTypes.StoredProcedure)
                throw new NotSupportedException(string.Format(Errors.DatabaseEngineDoesntSupportType, type));

            if (type != DBSchemaTypes.ForeignKey)
            {
                //Special case where the CREATE VIEW .. AS cannot have parenthese after - causes exception
                if (type == DBSchemaTypes.View)
                    this.IncrementStatementDepth();
            }
            base.EndCreate(type, checknotexists);
        }
    }
}
