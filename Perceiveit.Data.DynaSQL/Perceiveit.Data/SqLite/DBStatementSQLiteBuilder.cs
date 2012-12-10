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
using System.Data.Common;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.SqLite
{
    /// <summary>
    /// Builds statements for the SQLite database engine
    /// </summary>
    public class DBStatementSQLiteBuilder : DBStatementBuilder
    {
        #region ivars

        /// <summary>
        /// Defines the value for the TopN function that is later output on the sql string as Limits
        /// </summary>
        private int _limits = -1;

        /// <summary>
        /// defines the value for the offset at which to start outputting the rows and appended to the LIMIT value
        /// </summary>
        private int _offset = -1;

        /// <summary>
        /// The date("now") reference so we know when to put "now" in the parameters
        /// </summary>
        private bool _lastwasdatefunction = false;

        #endregion


        public DBStatementSQLiteBuilder(DBDatabase forDatabase, DBDatabaseProperties properties, System.IO.TextWriter tw, bool ownswriter)
            : base(forDatabase, properties, tw, ownswriter)
        {
        }

        private int _concatfunction = 0;

        public override void BeginFunction(Function function, string name)
        {
            if (function == Function.LastID)
                this.WriteRawSQLString("last_insert_rowid");

            else if (function == Function.GetDate)
            {
                this.WriteRawSQLString("CURRENT_TIMESTAMP");
                //this.WriteRaw("date");
                this._lastwasdatefunction = true;
            }
            else if (function == Function.Concatenate)
            {
                _concatfunction++;
            }
            else
                base.BeginFunction(function, name);
        }

        public override void EndFunction(Function function, string name)
        {
            _concatfunction = 0;
            base.EndFunction(function, name);
        }

        public override void BeginFunctionParameterList()
        {
            if (!_lastwasdatefunction)
                base.BeginFunctionParameterList();
        }

        public override void EndFunctionParameterList()
        {
            if (!_lastwasdatefunction)
                base.EndFunctionParameterList();
            this._lastwasdatefunction = false;
        }

        public override void BeginFunctionParameter(int index)
        {
            if(_concatfunction == 1 && index > 0)
                this.WriteRawSQLString(" || ");
            else
                base.BeginFunctionParameter(index);
        }

        public override void WriteReferenceSeparator()
        {
            
            base.WriteReferenceSeparator();
        }

        public override void BeginScript()
        {
            //this.Writer.Write("BEGIN");
            this.IncrementStatementBlock();
            //this.BeginNewLine();
        }

        public override void EndScript()
        {
            this.DecrementStatementBlock();
            //this.BeginNewLine();
            //this.Writer.WriteLine("END");
        }

        public override string GetCreateOption(CreateOptions option)
        {
            //Get rid of the unsupported CLUSTERED and NONCLUSTERED options
            if ((option & CreateOptions.NonClustered) > 0)
                option &= ~CreateOptions.NonClustered;
            if ((option & CreateOptions.Clustered) > 0)
                option &= ~CreateOptions.Clustered;

            return base.GetCreateOption(option);
        }

        public override void BeginDeclareStatement(DBParam param)
        {
            //base.BeginDeclareStatement(param);
        }

        public override void EndDeclareStatement()
        {
            //base.EndDeclareStatement();
        }

        public override void WriteParameter(DBParam param, bool writetype, bool includeDirection)
        {
            if (!writetype)
                base.WriteParameter(param, writetype, includeDirection);
        }

        public override void WriteTop(double limit, double offset, TopType topType)
        {
            

            if (this.StatementDepth == 1)
            {
                if (topType == TopType.Percent)
                    throw new NotSupportedException("This provider does not support the top percent syntax");

                this._limits = (int)limit;
                this._offset = (int)offset;
            }
        }

        public override void EndSelectStatement()
        {
            _insertOrUpdateSelectCount--;
            if (this.StatementDepth == 1 && this._limits > 0)
            {
                this.WriteRawSQLString(" LIMIT ");
                this.Write(_limits);
                if (this._offset > 0)
                {
                    this.WriteRawSQLString(" OFFSET ");
                    this.Write(_offset);
                }
            }
            base.EndSelectStatement();
        }

        public override void WriteColumnFlags(DBColumnFlags flags, DBClause defaultValue)
        {
            if ((flags & DBColumnFlags.PrimaryKey) > 0)
                this.WriteRawSQLString(" PRIMARY KEY");

            if ((flags & DBColumnFlags.AutoAssign) > 0)
                this.WriteRawSQLString(" AUTOINCREMENT");

            this.WriteSpace();
            if ((flags & DBColumnFlags.Nullable) > 0)
                this.WriteNull();
            else
                this.WriteNotNull();

            
            if ((flags & DBColumnFlags.HasDefault) > 0)
            {
                this.WriteRawSQLString(" DEFAULT ");
                defaultValue.BuildStatement(this);
            }
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
                    return " INTEGER";

                default:
                    return base.GetNativeTypeForDbType(dbType, setSize, accuracy, flags, out options);
            }
            
        }

        public override void BeginEntityDefinition()
        {
            this.WriteRawSQLString(" AS ");
        }

        public override void EndEntityDefinition()
        {
            
        }

       

        //SQLite is the only DB Provider that does not support the Drop index .. ON TABLE
        //So we need to flag this and not write anything to do with the table.
        private bool _droppingIndex = false;

        public override void BeginDropStatement(DBSchemaTypes type, string owner, string name, bool checkExists)
        {
            if (type == DBSchemaTypes.StoredProcedure)
                throw new NotSupportedException(string.Format(Errors.DatabaseEngineDoesntSupportType, type));

            this.BeginDropStatement(type);

            if (checkExists)
                this.WriteCheckExists(type, owner, name);

            this.WriteSourceTable(owner, name, string.Empty);

            if (type == DBSchemaTypes.Index)
                _droppingIndex = true;
        }

        public override void BeginReferenceOn()
        {
            if (!_droppingIndex)
            {
                base.BeginReferenceOn();
            }
            else
            {
#if STRICT_SQL
                throw new ArgumentException("ON", Errors.SqlLiteDoesntSupportOnReferencesForIndexes);
#endif
            }
        }

        public override void WriteSourceTable(string catalog, string schemaOwner, string sourceTable, string alias)
        {
            if(!_droppingIndex)
                base.WriteSourceTable(catalog, schemaOwner, sourceTable, alias);
        }

        public override void EndReferenceOn()
        {
            base.EndReferenceOn();

            //Clear the dropping index flag
            _droppingIndex = false;
        }

        private void WriteCheckExists(DBSchemaTypes type, string owner, string name)
        {
            this.WriteRawSQLString(" IF EXISTS ");
        }

        public override void BeginCheckExists(DBSchemaTypes type, string owner, string name)
        {
            if (type == DBSchemaTypes.StoredProcedure)
                throw new NotSupportedException(string.Format(Errors.DatabaseEngineDoesntSupportType, type));
        }

        public override void EndCheckExists(DBSchemaTypes type, string owner, string name)
        {

        }

        

        public override void BeginCreate(DBSchemaTypes type, string owner, string name, string options, bool checknotexists)
        {
            if (type == DBSchemaTypes.StoredProcedure)
                throw new NotSupportedException(string.Format(Errors.DatabaseEngineDoesntSupportType, type));

            bool isconstraint = this.IsConstraintType(type);

            if (isconstraint && !string.IsNullOrEmpty(name))
            {
                this.WriteRawSQLString("CONSTRAINT ");
                this.BeginIdentifier();
                this.WriteObjectName(name);
                this.EndIdentifier();
                this.WriteSpace();
            }

            this.BeginCreate(type, options);

            if (checknotexists)
                this.WriteCheckNotExists(type, owner, name);

            if (!isconstraint)
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

        public override void EndCreate(DBSchemaTypes type, bool checknotexists)
        {
            //Special case where the CREATE VIEW .. AS cannot have parenthese after - causes exception
            if (type == DBSchemaTypes.View)
                this.IncrementStatementDepth();

            base.EndCreate(type, checknotexists);
        }

        private void WriteCheckNotExists(DBSchemaTypes type, string owner, string name)
        {
            this.WriteRawSQLString(" IF NOT EXISTS ");
        }

        public override void BeginCheckNotExists(DBSchemaTypes type, string owner, string name)
        {

        }

        public override void EndCheckNotExists(DBSchemaTypes type, string owner, string name)
        {

        }

        private bool _insertOrUpdate = false; //If we are inserting or updating we dont want a '(..)' around any Select part
        private int _insertOrUpdateSelectCount = 0;
        
        public override void BeginInsertStatement()
        {
            _insertOrUpdate = true;
            _insertOrUpdateSelectCount = 0;
            base.BeginInsertStatement();
        }

        public override void BeginUpdateStatement()
        {
            _insertOrUpdate = true;
            _insertOrUpdateSelectCount = 0;
            base.BeginUpdateStatement();
        }

        public override void EndUpdateStatement()
        {
            base.EndUpdateStatement();
            _insertOrUpdate = false;
        }

        public override void EndInsertStatement()
        {
            base.EndInsertStatement();
            _insertOrUpdate = false;
        }

        public override void BeginSelectStatement()
        {
            _insertOrUpdateSelectCount += 1;
            base.BeginSelectStatement();
           
        }

        public override void BeginSubStatement()
        {
            if (!_insertOrUpdate || _insertOrUpdateSelectCount > 1)
                base.BeginSubStatement();
        }

        public override void EndSubStatement()
        {
            if (!_insertOrUpdate || _insertOrUpdateSelectCount > 1)
                base.EndSubStatement();
        }

        
    }
}
