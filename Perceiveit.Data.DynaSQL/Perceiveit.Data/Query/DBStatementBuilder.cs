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


namespace Perceiveit.Data.Query
{
    public abstract class DBStatementBuilder: IDisposable
    {
        //
        // inner classes
        //

        #region protected class StatementParameter
        
        /// <summary>
        /// Inner class for collecting all the parameters 
        /// defined in a statement
        /// </summary>
        public class StatementParameter
        {

            #region public string ParameterName {get;}

            private string _paramName;

            public string ParameterName
            {
                get { return this._paramName; }
                set { this._paramName = value; }
            }

            #endregion

            #region public IDBValueSource ValueSource

            private IDBValueSource _valuesource;

            public IDBValueSource ValueSource
            {
                get { return _valuesource; }
                set { _valuesource = value; }
            }

            #endregion

            #region public int Index

            private int _index;

            public int ParameterIndex
            {
                get { return _index; }
                set { _index = value; }
            }

            #endregion

        }

        #endregion

        #region protected class StatementParameterList

        /// <summary>
        /// Inner list of statement parameter references
        /// </summary>
        public class StatementParameterList : System.Collections.ObjectModel.KeyedCollection<string,StatementParameter>
        {
            protected override string GetKeyForItem(StatementParameter item)
            {
                return item.ParameterName;
            }

            protected override void RemoveItem(int index)
            {
                throw new NotSupportedException("Cannot remove items from this collection");
            }
        }

        #endregion

        //
        // properties
        // 


        #region public System.IO.TextWriter Writer {get;}

        private System.IO.TextWriter _tw;

        public System.IO.TextWriter Writer
        {
            get { return this._tw; }
        }

        #endregion

        #region public bool OwnsWriter {get;}

        private bool _ownswriter;

        public bool OwnsWriter
        {
            get { return _ownswriter; }
        }

        #endregion

        #region public virtual bool SupportsMultipleStatements {get;}

        /// <summary>
        /// Identifies if the connected database supports multiple properties
        /// </summary>
        public virtual bool SupportsMultipleStatements
        {
            get
            {
                return this.DatabaseProperties.CheckSupports(DBSchemaTypes.CommandScripts);
            }
        }

        #endregion

        #region protected public virtual StatementParameterList Parameters {get;}

        private StatementParameterList _params;

        /// <summary>
        /// Gets the list of parameters in the built statement
        /// </summary>
        public StatementParameterList Parameters
        {
            get
            {
                if (_params == null)
                    _params = new StatementParameterList();
                return _params;
            }
        }

        #endregion

        #region protected public virtual bool HasParameters {get;}

        public virtual bool HasParameters
        {
            get { return this._params != null && this._params.Count > 0; }
        }

        #endregion

        #region protected virtual string ParameterNamePrefix {get;}

        protected virtual string ParameterNamePrefix
        {
            get { return "_param"; }
        }

        #endregion

        #region protected virtual string DateFormatString {get;}

        protected virtual string DateFormatString
        {
            get
            {
                return "yyyy-MM-dd hh:mm:ss";
            }
        }

        #endregion

        #region public int StatementDepth {get;}

        private int _depth;

        public int StatementDepth
        {
            get { return _depth; }
        }

        #endregion

        #region public int StatementInset {get;}

        private int _inset;

        public int StatementInset
        {
            get { return _inset; }
        }

        #endregion

        #region protected DBDataBase Database {get;}

        private DBDatabase _db;

        /// <summary>
        /// Gets the Database this builder is writing a statement for
        /// </summary>
        protected DBDatabase Database
        {
            get { return this._db; }
        }

        #endregion

        #region protected DBDatabaseProperties DatabaseProperties {get;}

        private DBDatabaseProperties _dbprops;

        /// <summary>
        /// Gets the set of Database Properties that this builder is writing a statement for
        /// </summary>
        protected DBDatabaseProperties DatabaseProperties
        {
            get { return this._dbprops; }
        }

        #endregion

        //
        // .ctor(s)
        //

        #region protected DBStatementBuilder(System.IO.TextWriter writer, bool ownsWriter)

        protected DBStatementBuilder(DBDatabase database, DBDatabaseProperties properties, System.IO.TextWriter writer, bool ownsWriter)
        {
            if (null == database)
                throw new ArgumentNullException("database");

            if (null == properties)
                throw new ArgumentNullException("properties");

            if (null == writer)
                throw new ArgumentException("Cannot create a DBStatementBuilder with a null TextWriter", "writer");

            this._tw = writer;
            this._ownswriter = ownsWriter;
            this._db = database;
            this._dbprops = properties;
        }

        #endregion

       
        //
        // support methods
        //


        #region protected int GetNextID()

        private int _nextpid = 1;

        protected int GetNextID()
        {
            int id = _nextpid;
            _nextpid++;
            return id;
        }

        #endregion

        #region protected int IncrementStatementDepth()

        /// <summary>
        /// Increments the current inset and  statement depth 
        /// returning the new statement depth
        /// </summary>
        /// <returns></returns>
        protected int IncrementStatementDepth()
        {
            this._inset++;
            this._depth++;
            return this._depth;
        }

        #endregion

        #region  protected int IncrementStatementBlock()
        /// <summary>
        /// Increments and returns the current inset
        /// </summary>
        /// <returns></returns>
        protected int IncrementStatementBlock()
        {
            this._inset++;
            return this._inset;
        }

        #endregion

        #region protected void DecrementStatementDepth()

        /// <summary>
        /// Decreases the Statement depth and inset by one
        /// </summary>
        protected void DecrementStatementDepth()
        {
            if (this._depth > 0)
                this._depth--;
            if (this._inset > 0)
                this._inset--;
        }

        #endregion

        #region protected void DecrementStatementBlock()

        /// <summary>
        /// Decreases the inset by one
        /// </summary>
        protected void DecrementStatementBlock()
        {
            if (this._inset > 0)
                this._inset--;
        }

        #endregion

        #region protected void BeginNewLine()

        /// <summary>
        /// Outputs the new line and inset onto the current writer
        /// </summary>
        public virtual void BeginNewLine()
        {
            string line = "\r\n";
            if (this.StatementInset > 0)
                line = line.PadRight(line.Length + this.StatementInset, '\t');
            this.Writer.Write(line);
        }

        #endregion
        
        #region protected virtual string GetAllFieldIdentifier()

        protected virtual string GetAllFieldIdentifier()
        {
            return "*";
        }

        #endregion

        #region protected virtual string EscapeString(string value)

        protected virtual string EscapeString(string value)
        {
            return value.Replace("'", "''");
        }

        #endregion


        //
        // builder begin end section methods - public virtual
        //


        #region public virtual void BeginFunctionParameterList() + EndFunctionParameterList() + AppendReferenceSeparator()
        
        /// <summary>
        /// Appends the start of a new function parameter list to the statement '( '
        /// </summary>
        public virtual void BeginFunctionParameterList()
        {
            this.Writer.Write("(");
        }
        /// <summary>
        /// Appends the end of a function parameter list to the statement ') '
        /// </summary>
        public virtual void EndFunctionParameterList()
        {
            this.Writer.Write(") ");
        }

        /// <summary>
        /// Appends an individual separator to the statement ', '
        /// </summary>
        public virtual void AppendReferenceSeparator()
        {
            this.Writer.Write(", ");
        }

        #endregion

        #region public virtual void BeginSubSelect() + EndSubSelect()

        public virtual void BeginSubStatement()
        {
            this.Writer.Write(" (");
        }

        public virtual void EndSubStatement()
        {
            this.Writer.Write(") ");
            this.BeginNewLine();
        }

        #endregion

        #region public virtual void BeginIdentifier() + EndIdentifier() + AppendIdSeparator()

        public virtual void BeginIdentifier()
        {
            this.Writer.Write("[");
        }

        public virtual void EndIdentifier()
        {
            this.Writer.Write("]");
        }

        public virtual void AppendIdSeparator()
        {
            this.Writer.Write(".");
        }

        #endregion

        #region public virtual void BeginAlias() + EndAlias()

        public virtual void BeginAlias()
        {
            this.Writer.Write(" AS ");
        }

        public virtual void EndAlias()
        {

        }

        #endregion

        #region public virtual void BeginSelectStatement() + EndSelectStatement()

        public virtual void BeginSelectStatement()
        {
            this.BeginNewLine();
            if (this.StatementDepth > 0)
                this.BeginSubStatement();

            this.Writer.Write("SELECT ");
            this.IncrementStatementDepth();
            
        }

        public virtual void EndSelectStatement()
        {
            if (this.StatementDepth > 1)
                this.EndSubStatement();
            else
                this.WriteStatementTerminator(); 
            
            this.DecrementStatementDepth();
            
        }

        public virtual void WriteStatementTerminator()
        {
            this.WriteRaw(";");
        }

        #endregion

        #region public virtual void BeginFromList() + EndFromList()

        public virtual void BeginFromList()
        {
            this.BeginNewLine();
            this.Writer.Write("FROM ");
            this.IncrementStatementBlock();
        }

        public virtual void EndFromList()
        {
            this.DecrementStatementBlock();
        }

        #endregion

        #region public virtual void BeginBlock() + EndBlock()

        public virtual void BeginBlock()
        {
            this.Writer.Write(" (");
        }

        public virtual void EndBlock()
        {
            this.Writer.Write(") ");
        }

        #endregion

        #region public virtual void BeginWhereStatement() + EndWhereStatement()

        public virtual void BeginWhereStatement()
        {
            this.BeginNewLine();
            this.Writer.Write("WHERE ");
            this.IncrementStatementBlock();

        }

        public virtual void EndWhereStatement()
        {
            this.DecrementStatementBlock();
        }

        #endregion

        #region  public virtual void BeginOrderStatement() + EndOrderStatement()

        public virtual void BeginOrderStatement()
        {
            this.BeginNewLine();
            this.Writer.Write("ORDER BY ");
            this.IncrementStatementBlock();
        }

        public virtual void EndOrderStatement()
        {
            this.DecrementStatementBlock();
        }

        #endregion 

        #region public void BeginOrderClause(Order order) + EndOrderClause(Order order)

        public void BeginOrderClause(Order order)
        {

        }

        public virtual void EndOrderClause(Order order)
        {
            string o;
            switch (order)
            {
                case Order.Ascending:
                    o = " ASC";
                    break;
                case Order.Descending:
                    o = " DESC";
                    break;
                case Order.Default:
                    o = "";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("The value of the order by enumeration '" + order.ToString() + "' could not be recognised");
            }
            this.Writer.Write(o);
        }

        #endregion

        #region public virtual void BeginJoin(JoinType joinType) + EndJoin(JoinType joinType)

        public virtual void BeginJoin(JoinType joinType)
        {
            string j;
            switch (joinType)
            {
                case JoinType.InnerJoin:
                    j = " INNER JOIN ";
                    break;
                case JoinType.LeftOuter:
                    j = " LEFT OUTER JOIN ";
                    break;
                case JoinType.CrossProduct:
                case JoinType.Join:
                    j = " JOIN ";
                    break;
                case JoinType.RightOuter:
                    j = " RIGHT OUTER JOIN ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("The join type '" + joinType.ToString() + "' is not recognised");
            }
            this.Writer.Write(j);
            this.IncrementStatementBlock();

        }

        public virtual void EndJoin(JoinType joinType)
        {
            this.DecrementStatementBlock();
        }

        #endregion

        #region public virtual void BeginJoinOnList() + EndJoinOnList()

        public virtual void BeginJoinOnList()
        {
            this.BeginNewLine();
            this.Writer.Write(" ON ");
            this.IncrementStatementBlock();
        }

        public virtual void EndJoinOnList()
        {
            this.DecrementStatementBlock();
        }

        #endregion

        #region public virtual void BeginDateLiteral() + EndDateLiteral()

        public virtual void BeginDateLiteral()
        {
            this.Writer.Write("'");
        }

        public virtual void EndDateLiteral()
        {
            this.Writer.Write("'");
        }

        #endregion

        #region public virtual void BeingStringLiteral() + EndStringLiteral()

        public virtual void BeingStringLiteral()
        {
            this.Writer.Write("'");
        }

        public virtual void EndStringLiteral()
        {
            this.Writer.Write("'");
        }

        #endregion

        #region public virtual void BeginGroupByStatement() + EndGroupByStatement()

        public virtual void BeginGroupByStatement()
        {
            this.BeginNewLine();
            this.Writer.Write("GROUP BY ");
            this.IncrementStatementBlock();
        }

        public virtual void EndGroupByStatement()
        {
            this.DecrementStatementBlock();
        }

        #endregion

        #region public virtual void BeginHavingStatement() + EndHavingStatement()

        public virtual void BeginHavingStatement()
        {
            this.Writer.WriteLine();
            this.Writer.Write("HAVING ");
        }

        public virtual void EndHavingStatement()
        {

        }

        #endregion

        #region public void BeginUpdateStatement() + EndUpdateStatement()

        public virtual void BeginUpdateStatement()
        {
            if (this.StatementDepth > 0)
                this.BeginSubStatement();
            this.IncrementStatementDepth();

            this.Writer.Write("UPDATE ");
        }

        public virtual void EndUpdateStatement()
        {
            if (this.StatementDepth > 1)
                this.EndSubStatement();
            else
                this.WriteStatementTerminator();
                

            this.DecrementStatementDepth();

            this.Writer.WriteLine();
        }

        #endregion

        #region public virtual void BeginSetValueList() + EndSetValueList()

        public virtual void BeginSetValueList()
        {
            this.Writer.WriteLine();
            this.Writer.Write(" SET ");
        }

        public virtual void EndSetValueList()
        {
        }

        #endregion

        #region public virtual void BeginAssignValue() + EndAssignValue()

        public virtual void BeginAssignValue()
        {

        }

        public virtual void EndAssignValue()
        {

        }

        #endregion

        #region public virtual void BeginInsertStatement() + EndInsertStatement()

        public virtual void BeginInsertStatement()
        {
            if (this.StatementDepth > 0)
                this.BeginSubStatement();

            this.IncrementStatementDepth();

            this.Writer.Write("INSERT INTO ");
        }

        public virtual void EndInsertStatement()
        {
            if (this.StatementDepth > 1)
                this.EndSubStatement();
            else
                this.WriteStatementTerminator();

            this.DecrementStatementDepth();

            this.Writer.WriteLine();
        }

        #endregion

        #region public virtual void BeginInsertFieldList() + EndInsertFieldList()

        public virtual void BeginInsertFieldList()
        {
            this.BeginNewLine();
            this.Writer.Write("(");
        }

        public virtual void EndInsertFieldList()
        {
            this.Writer.Write(")");
        }

        #endregion

        #region public virtual void BeginInsertValueList() +  EndInsertValueList()

        public virtual void BeginInsertValueList()
        {
            this.BeginNewLine();
            this.Writer.Write("VALUES (");
            this.IncrementStatementBlock();
        }

        public virtual void EndInsertValueList()
        {
            this.Writer.Write(")");
            this.DecrementStatementBlock();
        }

        #endregion

        #region public virtual void BeginDeleteStatement() + EndDeleteStatement()

        public virtual void BeginDeleteStatement()
        {
            if (this.StatementDepth > 0)
                this.BeginSubStatement();
            this.IncrementStatementDepth();

            this.Writer.Write("DELETE FROM ");
        }

        public virtual void EndDeleteStatement()
        {
            if (this.StatementDepth > 1)
                this.EndSubStatement();
            else
                this.WriteStatementTerminator();

            this.DecrementStatementDepth();

            this.Writer.WriteLine();
        }

        #endregion

        #region public virtual void BeginFunction(Function function, string name) + EndFunction(Function function, string name)

        public virtual void BeginFunction(Function function, string name)
        {
            switch (function)
            {
                case Function.GetDate:
                    name = "GETDATE";
                    break;
                case Function.LastID:
                    name = "SCOPE_IDENTITY";
                    break;
                case Function.IsNull:
                    name = "ISNULL";
                    break;
                case Function.Unknown:
                default:
                    if (null == name)
                        throw new ArgumentNullException("name");

                    name = name.ToUpper();
                    break;
            }
            this.Writer.Write(name);
        }

        public virtual void EndFunction(Function function, string name)
        {

        }

        #endregion

        #region public virtual void BeginAggregateFunction(AggregateFunction function, string name) + EndAggregateFunction(AggregateFunction function, string name)

        public virtual void BeginAggregateFunction(AggregateFunction function, string name)
        {
            switch (function)
            {
                default:
                    if (null == name)
                        throw new ArgumentNullException("name");

                    name = name.ToUpper();
                    break;
            }
            this.Writer.Write(name);
        }

        public virtual void EndAggregateFunction(AggregateFunction function, string name)
        {
        }

        #endregion

        #region public virtual void BeginScript() + EndScript()

        public virtual void BeginScript()
        {
            this.Writer.Write("BEGIN");
            this.IncrementStatementBlock();
            this.BeginNewLine();
        }

        public virtual void EndScript()
        {
            this.DecrementStatementBlock();
            this.BeginNewLine();
            this.Writer.WriteLine("END");
        }

        #endregion

        #region public virtual void BeginTertiaryOperator(Operator p) + ContinueTertiaryOperator(Operator p) + EndTertiaryOperator(Operator p)

        public virtual void BeginTertiaryOperator(Operator p)
        {
            switch (p)
            {
                case Operator.Between:
                    this.Writer.Write(" BETWEEN ");
                    break;
                default:
                    throw new ArgumentException("The operator '" + p.ToString() + "' is not a valid or known Tertiary operator");

            }
        }

        public virtual void ContinueTertiaryOperator(Operator p)
        {
            switch (p)
            {
                case Operator.Between:
                    this.Writer.Write(" AND ");
                    break;
                default:
                    throw new ArgumentException("The operator '" + p.ToString() + "' is not a valid or known Tertiary operator");

            }
        }

        internal void EndTertiaryOperator(Operator p)
        {

        }

        #endregion

        #region public virtual void BeginExecuteStatement() + EndExecuteStatement()

        public virtual void BeginExecuteStatement()
        {
            if (!this.DatabaseProperties.CheckSupports(DBSchemaTypes.StoredProcedure))
                throw new System.Data.DataException("Current database does not support stored procedures");
            
            this.Writer.Write("EXEC ");
        }

        public virtual void EndExecuteStatement()
        {
            this.Writer.Write("\r\n");
        }

        #endregion

        #region public virtual void BeginExecuteParameters() + EndExecuteParameters()

        bool hasparameter = false;

        public virtual void BeginExecuteParameters()
        {
            this.Writer.Write(" ");
            hasparameter = false;
        }
        
        public virtual void EndExecuteParameters()
        {
            hasparameter = false;
        }

        #endregion

        #region public virtual void BeginExecuteAParameter() + EndExecuteAParameter() + AppendParameterSeparator

        public virtual void BeginExecuteAParameter()
        {
            if (hasparameter)
                this.AppendParameterSeparator();
        }

        protected virtual void AppendParameterSeparator()
        {
            this.Writer.Write(",");
        }

        public virtual void EndExecuteAParameter()
        {
            this.Writer.Write(" ");
            hasparameter = true;
        }

        #endregion

        #region protected virtual void BeginDrop(DBSchemaTypes type) + EndDrop(type)

        protected virtual void BeginDrop(DBSchemaTypes type)
        {
            this.WriteRaw("DROP ");
            switch (type)
            {
                case DBSchemaTypes.Table:
                    this.WriteRaw("TABLE ");
                    break;
                case DBSchemaTypes.View:
                    this.WriteRaw("VIEW ");
                    break;
                case DBSchemaTypes.StoredProcedure:
                    this.WriteRaw("PROCEDURE ");
                    break;
                case DBSchemaTypes.Function:
                    this.WriteRaw("FUNCTION ");
                    break;
                case DBSchemaTypes.Index:
                    this.WriteRaw("INDEX ");
                    break;
                case (DBSchemaTypes)0:
                default:
                    throw new ArgumentOutOfRangeException("type");
                    
            }
        }

        protected virtual void EndDrop(DBSchemaTypes type)
        {
        }

        #endregion

        #region protected virtual void BeginCreate(DBSchemaTypes type) + EndCreate(type)

        protected virtual void BeginCreate(DBSchemaTypes type, string options)
        {
            this.WriteRaw("CREATE ");
            if (string.IsNullOrEmpty(options) == false)
            {
                this.WriteRaw(options);
                if (!options.EndsWith(" "))
                    this.WriteRaw(" ");
            }

            switch (type)
            {
                case DBSchemaTypes.Table:
                    this.WriteRaw("TABLE ");
                    break;
                case DBSchemaTypes.View:
                    this.WriteRaw("VIEW ");
                    break;
                case DBSchemaTypes.StoredProcedure:
                    this.WriteRaw("PROCEDURE ");
                    break;
                case DBSchemaTypes.Function:
                    this.WriteRaw("FUNCTION ");
                    break;
                case DBSchemaTypes.Index:
                    this.WriteRaw("INDEX ");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        protected virtual void EndCreate(DBSchemaTypes type)
        {
        }

        #endregion


        //
        // parameter addition, conversion and naming
        //

        #region public virtual string RegisterParameter(IDBValueSource source)

        /// <summary>
        /// Registers an IDBValueSource as a parameter in the statement and returns a name for this parameter.
        /// Note that this name is not the Database native parameter name 
        /// </summary>
        /// <param name="source">The source that identifies the parameter</param>
        /// <returns>The distinct name of the parameter</returns>
        public virtual string RegisterParameter(IDBValueSource source)
        {
            bool declared = true;
            string paramName = source.Name;

            if (string.IsNullOrEmpty(paramName))
            {
                paramName = this.GetUniqueParameterName(ParameterNamePrefix);
                declared = false;
            }
            else if (this.Parameters.Contains(paramName))
            {
                return paramName;
                //if (this.DatabaseProperties.ParameterLayout == DBParameterLayout.Named)
                //    //already registered
                //    return paramName;
                //else
                //    paramName = this.GetUniqueParameterName(ParameterNamePrefix);
            }
            else
                declared = true;

            StatementParameter sp = new StatementParameter();
            sp.ParameterName = paramName;
            sp.ValueSource = source;
            sp.ParameterIndex = this.Parameters.Count;
            if (!declared)
                sp.ValueSource.Name = paramName;

            this.Parameters.Add(sp);

            return paramName;
        }

        #endregion

        #region public virtual void WriteParameterReference(string paramname)

        /// <summary>
        /// Appends a parameter reference to the statement in database native format
        /// </summary>
        /// <param name="paramname">The coded parameter name</param>
        /// <exception cref="ArgumentNullException" >Thown if the paramname value is null or empty</exception>
        public virtual void WriteParameterReference(string paramname)
        {
            if (string.IsNullOrEmpty(paramname))
                throw new ArgumentNullException("Cannot append a null or empty parameter reference - implementors: Use the GetUniqueID to support unnamed parameters");

            this.Writer.Write(this.GetNativeParameterName(paramname));
        }

        #endregion

        #region public virtual string GetUniqueParameterName(string prefix)

        public virtual string GetUniqueParameterName(string prefix)
        {
            string name = "_param" + this.GetNextID();
            return name;
        }

        #endregion

        #region public virtual DbParameter CreateCommandParameter(DbCommand cmd, StatementParameter sparam)

        public virtual DbParameter CreateCommandParameter(DbCommand cmd, StatementParameter sparam)
        {
            DbParameter p = cmd.CreateParameter();
            PopulateParameter(p, sparam);

            return p;

        }

        internal void PopulateParameter(DbParameter p, StatementParameter sparam)
        {
            string name = sparam.ParameterName;
            //if (string.IsNullOrEmpty(name))
            //    p.SourceColumn = this.GetUniqueParameterName(DBParam.ParameterNamePrefix);
            //else
            //    p.SourceColumn = name;

            p.ParameterName = this.GetNativeParameterName(name);

            object value = sparam.ValueSource.Value;
            if (value is DBConst)
            {
                DBConst constant = (DBConst)value;

                if (sparam.ValueSource.HasType)
                    p.DbType = sparam.ValueSource.DbType;
                else
                    p.DbType = constant.Type;

                value = constant.Value;
            }
            else
            {
                if (sparam.ValueSource.HasType)
                    p.DbType = sparam.ValueSource.DbType;
                else
                    p.DbType = this.GetDbType(value);

                if (sparam.ValueSource.Size > 0)
                    p.Size = sparam.ValueSource.Size;
                else
                    p.Size = 0;

                value = ConvertParamValueToNativeValue(p.DbType, value);
            }
            p.Direction = sparam.ValueSource.Direction;
            p.Value = value;
        }

        #endregion

        #region protected virtual System.Data.DbType GetDbType(object val)


        protected virtual System.Data.DbType GetDbType(object val)
        {
            return DBHelper.GetDBTypeForObject(val);
        }

        #endregion

        #region protected virtual object ConvertParamValueToNativeValue(DbType type, object value)

        protected virtual object ConvertParamValueToNativeValue(DbType type, object value)
        {
            if (null == value)
                value = DBNull.Value;
            return value;
        }

        #endregion

        #region public virtual string GetNativeParameterName(string paramName)

        public virtual string GetNativeParameterName(string paramName)
        {
            return string.Format(this.DatabaseProperties.ParameterFormat, paramName);
        }

        #endregion

        //
        // write methods
        //

        #region public virtual void WriteTop(double value, double offset, TopType topType)

        public virtual void WriteTop(double count, double offset, TopType topType)
        {
            if (Array.IndexOf<TopType>(this.DatabaseProperties.SupportedTopTypes, topType) < 0)
                throw new NotSupportedException("The top type '" + topType.ToString() + "' is not supported by this database");

            this.Writer.Write(" TOP ");
            if (topType == TopType.Percent)
            {
                this.Writer.Write(count);
                this.Writer.Write(" PERCENT ");
            }
            else if(topType == TopType.Count)
            {
                this.Writer.Write((int)count);
                this.Writer.Write(" ");
            }
            else if (topType == TopType.Range)
            {
                this.Writer.Write((int)count);
                this.Writer.Write(", ");
                this.Writer.Write((int)offset);
            }

        }

        #endregion

        #region public virtual void WriteDistinct()

        public virtual void WriteDistinct()
        {
            this.Writer.Write(" DISTINCT ");
        }

        #endregion

        #region public virtual void WriteRaw(string str)

        public virtual void WriteRaw(string str)
        {
            this.Writer.Write(str);
        }

        #endregion

        #region public virtual void Write(int val) + 8 overloads

        public virtual void Write(int val)
        {
            this.WriteLiteral(DbType.Int32, val);
        }

        public virtual void Write(bool val)
        {
            this.WriteLiteral(DbType.Boolean, val);
        }

        public virtual void Write(decimal d)
        {
            this.WriteLiteral(DbType.Decimal, d);
        }

        public virtual void Write(double val)
        {
            this.WriteLiteral(DbType.Double, val);
        }

        public virtual void Write(long val)
        {
            this.WriteLiteral(DbType.Int64, val);
        }

        public virtual void Write(float val)
        {
            this.WriteLiteral(DbType.Single, val);
        }

        public virtual void Write(char val)
        {
            this.WriteLiteral(DbType.StringFixedLength, val);
        }

        public virtual void Write(Guid id)
        {
            this.WriteLiteral(DbType.Guid, id.ToString());
        }

        public virtual void Write(DateTime dt)
        {
            this.WriteLiteral(DbType.DateTime, dt);
        }

        public virtual void Write(string value)
        {
            this.WriteLiteral(DbType.String, value);
        }

        #endregion

        #region public virtual void WriteFormat(string format, object arg) + 2 overloads

        public virtual void WriteFormat(string format, object arg)
        {
            this.Writer.Write(format, arg);
        }

        public virtual void WriteFormat(string format, object arg1, object arg2)
        {
            this.Writer.Write(format, arg1, arg2);
        }

        public virtual void WriteFormat(string format, params object[] args)
        {
            this.Writer.Write(format, args);
        }

        #endregion

        #region public virtual void WriteOperator(Operator operation)

        public virtual void WriteOperator(Operator operation)
        {
            string ops;

            switch (operation)
            {
                case Operator.Equals:
                    ops = " = ";
                    break;
                case Operator.LessThan:
                    ops = " < ";
                    break;
                case Operator.GreaterThan:
                    ops = " > ";
                    break;
                case Operator.LessThanEqual:
                    ops = " <= ";
                    break;
                case Operator.GreaterThanEqual:
                    ops = " >= ";
                    break;
                case Operator.In:
                    ops = " IN ";
                    break;
                case Operator.NotIn:
                    ops = " NOT IN ";
                    break;
                case Operator.NotEqual:
                    ops = " <> ";
                    break;
                case Operator.Like:
                    ops = " LIKE ";
                    break;
                case Operator.Is:
                    ops = " IS ";
                    break;
                case Operator.Add:
                    ops = " + ";
                    break;
                case Operator.Subtract:
                    ops = " - ";
                    break;
                case Operator.Multiply:
                    ops = " * ";
                    break;
                case Operator.Divide:
                    ops = " / ";
                    break;
                case Operator.BitShiftLeft:
                    ops = " << ";
                    break;
                case Operator.BitShiftRight:
                    ops = " >> ";
                    break;
                case Operator.Modulo:
                    ops = " % ";
                    break;
                case Operator.Not:
                    ops = " NOT ";
                    break;
                case Operator.Exists:
                    ops = " EXISTS ";
                    break;
                case Operator.Between:
                    ops = " BETWEEN ";
                    break;
                case Operator.And:
                    ops = " AND ";
                    break;
                case Operator.Or:
                    ops = " OR ";
                    break;
                case Operator.XOr:
                    ops = " XOR ";
                    break;
                case Operator.BitwiseAnd:
                    ops = " & ";
                    break;
                case Operator.BitwiseOr:
                    ops = " | ";
                    break;
                case Operator.Concat:
                    ops = " & ";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(" The operation '" + operation + "' is not a known operator");
            }
            this.Writer.Write(ops);
        }

        #endregion

        #region public virtual void WriteLiteral(DbType dbType, object value)

        public virtual void WriteLiteral(DbType dbType, object value)
        {
            if (null == value)
                this.WriteNull();
            else
            {
                switch (dbType)
                {
                    case DbType.Binary:
                        throw new NotSupportedException("Cannot write a binary literal");

                    case DbType.Date:
                    case DbType.Time:
                    case DbType.DateTime:
                    case DbType.DateTime2:
                    case DbType.DateTimeOffset:
                        this.BeginDateLiteral();
                        this.Writer.Write(((DateTime)value).ToString(this.DateFormatString));
                        this.EndDateLiteral();
                        break;
                    case DbType.Guid:
                        break;
                    case DbType.Xml:
                    case DbType.AnsiString:
                    case DbType.AnsiStringFixedLength:
                    case DbType.String:
                    case DbType.StringFixedLength:
                        this.BeingStringLiteral();
                        this.Writer.Write(this.EscapeString((string)value));
                        this.EndStringLiteral();
                        break;
                    default:
                        if (value is DBNull)
                            this.Writer.Write("NULL");
                        else
                            this.Writer.Write(value);
                        break;
                }
            }
        }

        #endregion

        #region public virtual void WriteNull()

        public virtual void WriteNull()
        {
            this.Writer.Write("NULL");
        }

        #endregion

        #region public virtual void WriteAllFieldIdentifier(string schemaOwner, string sourceTable)

        public virtual void WriteAllFieldIdentifier(string schemaOwner, string sourceTable)
        {
            if (string.IsNullOrEmpty(sourceTable) && !string.IsNullOrEmpty(schemaOwner))
                throw new ArgumentNullException("sourceTable", Errors.CannotSpecifySchemaOwnerAndNotTable);

            if (string.IsNullOrEmpty(schemaOwner) == false)
            {
                this.BeginIdentifier();
                this.WriteRaw(schemaOwner);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            if (string.IsNullOrEmpty(sourceTable) == false)
            {
                this.BeginIdentifier();
                this.WriteRaw(sourceTable);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            this.Writer.Write(this.GetAllFieldIdentifier());
        }

        #endregion

        #region public virtual void WriteSourceField(string schemaOwner, string sourceTable, string columnName, string alias)

        /// <summary>
        /// Appends the owner (optional), table (optional unless owner is specified) and column (required) to the statement and
        /// sets the alias name (optional). All the identifers are wrapped within the identifier delimiter characters
        /// </summary>
        /// <param name="schemaOwner"></param>
        /// <param name="sourceTable"></param>
        /// <param name="columnName"></param>
        /// <param name="alias"></param>
        public virtual void WriteSourceField(string schemaOwner, string sourceTable, string columnName, string alias)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException("columnName", Errors.NoColumnNameSpecifiedForAField);

            if (string.IsNullOrEmpty(sourceTable) && !string.IsNullOrEmpty(schemaOwner))
                throw new ArgumentNullException("sourceTable", Errors.CannotSpecifySchemaOwnerAndNotTable);

            if (string.IsNullOrEmpty(schemaOwner) == false)
            {
                this.BeginIdentifier();
                this.WriteRaw(schemaOwner);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            if (string.IsNullOrEmpty(sourceTable) == false)
            {
                this.BeginIdentifier();
                this.WriteRaw(sourceTable);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            this.BeginIdentifier();
            this.WriteRaw(columnName);
            this.EndIdentifier();

            if (string.IsNullOrEmpty(alias) == false)
            {
                this.WriteAlias(alias);
            }
        }

        #endregion

        #region public virtual void WriteSourceTable(string schemaOwner, string sourceTable, string alias)

        /// <summary>
        /// Appends the owner (optional) and table (required) to the statement and sets the alias name (optional).
        /// All the identifiers are wrapped with the delimiter characters
        /// </summary>
        /// <param name="schemaOwner"></param>
        /// <param name="sourceTable"></param>
        /// <param name="alias"></param>
        public virtual void WriteSourceTable(string schemaOwner, string sourceTable, string alias)
        {

            if (string.IsNullOrEmpty(sourceTable))
                throw new ArgumentNullException("sourceTable", Errors.NoTableNameSpecifiedForATable);

            if (string.IsNullOrEmpty(schemaOwner) == false)
            {
                this.BeginIdentifier();
                this.WriteRaw(schemaOwner);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            this.BeginIdentifier();
            this.WriteRaw(sourceTable);
            this.EndIdentifier();

            if (string.IsNullOrEmpty(alias) == false)
            {
                WriteAlias(alias);
            }
        }

        #endregion

        #region public virtual void WriteSourceTable(string schemaOwner, string sourceTable, string alias)

        /// <summary>
        /// Appends the owner (optional) and table (required) to the statement and sets the alias name (optional).
        /// All the identifiers are wrapped with the delimiter characters
        /// </summary>
        /// <param name="schemaOwner"></param>
        /// <param name="sourceTable"></param>
        /// <param name="alias"></param>
        public virtual void WriteSourceTable(string catalog, string schemaOwner, string sourceTable, string alias)
        {

            if (string.IsNullOrEmpty(sourceTable))
                throw new ArgumentNullException("sourceTable", Errors.NoTableNameSpecifiedForATable);

            if (string.IsNullOrEmpty(catalog) == false)
            {
                this.BeginIdentifier();
                this.WriteRaw(catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            if (string.IsNullOrEmpty(schemaOwner) == false)
            {
                this.BeginIdentifier();
                this.WriteRaw(schemaOwner);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            this.BeginIdentifier();
            this.WriteRaw(sourceTable);
            this.EndIdentifier();

            if (string.IsNullOrEmpty(alias) == false)
            {
                WriteAlias(alias);
            }
        }

        #endregion

        #region public virtual void WriteSourceTable(string schemaOwner, string sourceTable)

        /// <summary>
        /// Appends the catalog (optional),  owner (optional) and source (required) to the statement.
        /// All the identifiers are wrapped with the delimiter characters
        /// </summary>
        /// <param name="schemaOwner"></param>
        /// <param name="sourceTable"></param>
        /// <param name="alias"></param>
        public virtual void WriteSource(string catalog, string schemaOwner, string source)
        {

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source", Errors.NoTableNameSpecifiedForATable);

            if (string.IsNullOrEmpty(catalog) == false)
            {
                this.BeginIdentifier();
                this.WriteRaw(catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            if (string.IsNullOrEmpty(schemaOwner) == false)
            {
                this.BeginIdentifier();
                this.WriteRaw(schemaOwner);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            this.BeginIdentifier();
            this.WriteRaw(source);
            this.EndIdentifier();

        }

        #endregion

        #region public virtual void WriteAlias(string alias)

        public virtual void WriteAlias(string alias)
        {
            this.BeginAlias();
            this.BeginIdentifier();
            this.WriteRaw(alias);
            this.EndIdentifier();
            this.EndAlias();
        }

        #endregion


        //
        // dispose and finalize
        //

        #region protected virtual void Dispose(bool disposing)

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.OwnsWriter)
                this.Writer.Dispose();
        }
        
        #endregion

        #region public void Dispose()

        public void Dispose()
        {
            this.Dispose(true);
        }

        #endregion

        #region ~DBStatementBuilder()

        ~DBStatementBuilder()
        {
            this.Dispose(false);
        }

        #endregion

        //
        // object overrides
        //

        #region public override string ToString()

        public override string ToString()
        {
            return this.Writer.ToString();
        }

        #endregion


        //
        // Generate CREATE XXX Statements
        //

        public virtual void GenerateCreateTableScript(Perceiveit.Data.Schema.DBSchemaTable schemaTable)
        {
            throw new NotSupportedException("CREATE TABLE");
        }

        public virtual void GenerateCreateViewScript(Perceiveit.Data.Schema.DBSchemaView schemaView, DBQuery script)
        {
            throw new NotSupportedException("CREATE VIEW");
        }

        public virtual void GenerateCreateProcedureScript(Perceiveit.Data.Schema.DBSchemaSproc schemaSproc, DBScript script)
        {
            throw new NotSupportedException("CREATE PROCEDURE");
        }

        public virtual void GenerateCreateFunctionScript(Perceiveit.Data.Schema.DBSchemaFunction schemaSproc, DBScript script)
        {
            throw new NotSupportedException("CREATE FUNCTION");
        }

        public virtual void GenerateCreateIndexScript(DBSchemaIndex schemaIndex)
        {
            throw new NotSupportedException("CREATE INDEX");
        }


        //
        // GenerateDropXXX methods
        //

        #region public virtual void GenerateDropTableScript(DBSchemaItemRef itemRef)

        public virtual void GenerateDropTableScript(DBSchemaItemRef itemRef)
        {
            if (null == itemRef)
                throw new ArgumentNullException("itemRef");

            if (string.IsNullOrEmpty(itemRef.Name))
                throw new ArgumentNullException("itemRef.Name");

            if (itemRef.Type != DBSchemaTypes.Table)
                throw new ArgumentOutOfRangeException("itemRef.Type");


            this.BeginDrop(itemRef.Type);

            this.BeginIdentifier();
            if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.WriteRaw(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            if (string.IsNullOrEmpty(itemRef.Schema) == false)
            {
                this.WriteRaw(itemRef.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            this.WriteRaw(itemRef.Name);
            this.EndIdentifier();
            this.EndDrop(itemRef.Type);
        }

        #endregion

        #region public virtual void GenerateDropIndexScript(DBSchemaItemRef itemRef)

        public virtual void GenerateDropIndexScript(DBSchemaItemRef itemRef)
        {
            //DROP INDEX 'indexname' ON 'table name'

            if (null == itemRef)
                throw new ArgumentNullException("itemRef");

            if (string.IsNullOrEmpty(itemRef.Name))
                throw new ArgumentNullException("itemRef.Name");

            if (itemRef.Type != DBSchemaTypes.Index)
                throw new ArgumentOutOfRangeException("itemRef.Type");

            if (itemRef.Container == null || string.IsNullOrEmpty(itemRef.Container.Name) || itemRef.Container.Type != DBSchemaTypes.Table)
                throw new ArgumentNullException("itemRef.Container", Errors.ContainerNotSetOnIndexReference);

            this.BeginDrop(DBSchemaTypes.Index);
            this.BeginIdentifier();
            if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.WriteRaw(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            if (string.IsNullOrEmpty(itemRef.Schema) == false)
            {
                this.WriteRaw(itemRef.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            this.WriteRaw(itemRef.Name);
            this.EndIdentifier();
            this.WriteRaw(" ON ");

            //write the table reference
            itemRef = itemRef.Container;
            this.BeginIdentifier();
            if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.WriteRaw(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            if (string.IsNullOrEmpty(itemRef.Schema) == false)
            {
                this.WriteRaw(itemRef.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            this.WriteRaw(itemRef.Name);
            this.EndIdentifier();
        }


        #endregion

        #region public virtual void GenerateDropViewScript(DBSchemaItemRef itemRef)

        public virtual void GenerateDropViewScript(DBSchemaItemRef itemRef)
        {
            if (null == itemRef)
                throw new ArgumentNullException("itemRef");

            if (string.IsNullOrEmpty(itemRef.Name))
                throw new ArgumentNullException("itemRef.Name");

            if (itemRef.Type != DBSchemaTypes.View)
                throw new ArgumentOutOfRangeException("itemRef.Type");


            this.BeginDrop(itemRef.Type);

            this.BeginIdentifier();
            if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.WriteRaw(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            if (string.IsNullOrEmpty(itemRef.Schema) == false)
            {
                this.WriteRaw(itemRef.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            this.WriteRaw(itemRef.Name);
            this.EndIdentifier();
            this.EndDrop(itemRef.Type);
        }

        #endregion

        #region public virtual void GenerateDropProcedureScript(DBSchemaItemRef itemRef)

        public virtual void GenerateDropProcedureScript(DBSchemaItemRef itemRef)
        {
            if (null == itemRef)
                throw new ArgumentNullException("itemRef");

            if (string.IsNullOrEmpty(itemRef.Name))
                throw new ArgumentNullException("itemRef.Name");

            if (itemRef.Type != DBSchemaTypes.StoredProcedure)
                throw new ArgumentOutOfRangeException("itemRef.Type");


            this.BeginDrop(itemRef.Type);

            this.BeginIdentifier();
            if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.WriteRaw(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            if (string.IsNullOrEmpty(itemRef.Schema) == false)
            {
                this.WriteRaw(itemRef.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            this.WriteRaw(itemRef.Name);
            this.EndIdentifier();
            this.EndDrop(itemRef.Type);
        }

        #endregion

        #region public virtual void GenerateDropFunctionScript(DBSchemaItemRef itemRef)

        public virtual void GenerateDropFunctionScript(DBSchemaItemRef itemRef)
        {
            if (null == itemRef)
                throw new ArgumentNullException("itemRef");

            if (string.IsNullOrEmpty(itemRef.Name))
                throw new ArgumentNullException("itemRef.Name");

            if (itemRef.Type != DBSchemaTypes.Function)
                throw new ArgumentOutOfRangeException("itemRef.Type");


            this.BeginDrop(itemRef.Type);

            this.BeginIdentifier();
            if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.WriteRaw(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            if (string.IsNullOrEmpty(itemRef.Schema) == false)
            {
                this.WriteRaw(itemRef.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            this.WriteRaw(itemRef.Name);
            this.EndIdentifier();
            this.EndDrop(itemRef.Type);
        }

        #endregion

        #region protected virtual List<DBSchemaColumn> SortColumnsByOrdinal(IEnumerable<DBSchemaColumn> columns)

        protected virtual List<DBSchemaColumn> SortColumnsByOrdinal(IEnumerable<DBSchemaColumn> columns)
        {
            List<DBSchemaColumn> remain = new List<DBSchemaColumn>(columns);
            List<DBSchemaColumn> sorted = new List<DBSchemaColumn>(remain.Count);
            int maxOrd = -1;

            foreach (DBSchemaColumn col in remain)
            {
                if (col.OrdinalPosition > -1)
                    maxOrd = Math.Max(maxOrd, col.OrdinalPosition);
            }

            if (maxOrd < 0)// there are no ordinals set so just return the default order
                return remain;

            int currOrd = 0;
            while (currOrd <= maxOrd)
            {
                int lastcount = sorted.Count;

                foreach (DBSchemaColumn col in remain)
                {
                    //if the columns ordinal position matches our current ordinal - add it
                    if (col.OrdinalPosition == currOrd)
                        sorted.Add(col);
                }

                //remove the colums that were added to sorted from the remain collection
                while (lastcount < sorted.Count)
                {
                    remain.Remove(sorted[lastcount]);
                    lastcount++;
                    if (remain.Count < 1)
                        break;
                }

                currOrd++;
            }

            if (remain.Count > 0)
                //we have left over without ordinals set to add them 
                //to the end in the order that they were inserted into the collection
                sorted.AddRange(remain);

            return sorted;


        }


        #endregion

        #region protected virtual List<DBSchemaParameter> SortColumnsByOrdinal(IEnumerable<DBSchemaParameter> columns)

        protected virtual List<DBSchemaParameter> SortColumnsByOrdinal(IEnumerable<DBSchemaParameter> parameters)
        {
            List<DBSchemaParameter> remain = new List<DBSchemaParameter>(parameters);
            List<DBSchemaParameter> sorted = new List<DBSchemaParameter>(remain.Count);
            int maxOrd = -1;

            foreach (DBSchemaParameter col in remain)
            {
                if (col.ParameterIndex > -1)
                    maxOrd = Math.Max(maxOrd, col.ParameterIndex);
            }

            if (maxOrd < 0)// there are no ordinals set so just return the default order
                return remain;

            int currOrd = 0;
            while (currOrd <= maxOrd)
            {
                int lastcount = sorted.Count;

                foreach (DBSchemaParameter col in remain)
                {
                    //if the columns ordinal position matches our current ordinal - add it
                    if (col.ParameterIndex == currOrd)
                        sorted.Add(col);
                }

                //remove the colums that were added to sorted from the remain collection
                while (lastcount < sorted.Count)
                {
                    remain.Remove(sorted[lastcount]);
                    lastcount++;
                    if (remain.Count < 1)
                        break;
                }

                currOrd++;
            }

            if (remain.Count > 0)
                //we have left over without ordinals set to add them 
                //to the end in the order that they were inserted into the collection
                sorted.AddRange(remain);

            return sorted;


        }


        #endregion

        #region protected virtual string GetNativeTypeForDbType(System.Data.DbType dbType, int setSize, out string options)

        protected virtual string GetNativeTypeForDbType(System.Data.DbType dbType, int setSize, out string options)
        {
            string type;
            options = string.Empty;

            switch (dbType)
            {
                case System.Data.DbType.AnsiStringFixedLength:
                case System.Data.DbType.StringFixedLength:
                    if (setSize < 1)
                        setSize = 255;
                    else if (setSize > 255)
                        throw new ArgumentOutOfRangeException("CHAR size", "The maximum supported fixed length charater string is 255");

                    type = "CHAR";
                    options = "(" + setSize + ")";
                    break;

                case System.Data.DbType.String:
                case System.Data.DbType.AnsiString:
                    if (setSize < 0)
                        setSize = 255;
                    if (setSize < 256)
                    {
                        type = "VARCHAR";
                        options = "(" + setSize.ToString() + ")";
                    }
                    else if (setSize < 65536)
                        type = "TEXT";
                    else if (setSize < 16777215)
                        type = "MEDIUMTEXT";
                    else
                        type = "LONGTEXT";
                    break;


                case System.Data.DbType.Binary:
                case System.Data.DbType.Object:

                    if (setSize > 0)
                    {
                        if (setSize < 256)
                            type = "TINYBLOB";
                        else if (setSize < 65536)
                            type = "BLOB";
                        else if (setSize < 16777216)
                            type = "MEDIUMBLOB";
                        else
                            type = "LONGBLOB";
                    }
                    else
                        type = "MEDIUMBLOB";
                    break;


                case System.Data.DbType.Boolean:
                    type = "BIT";
                    break;

                case System.Data.DbType.Byte:
                    type = "TINYINT";
                    options = " UNSIGNED";
                    break;

                case System.Data.DbType.Date:
                    type = "DATE";
                    break;

                case System.Data.DbType.DateTime:
                    type = "DATETIME";
                    break;

                case System.Data.DbType.DateTime2:
                    type = "DATETIME";
                    break;

                case System.Data.DbType.DateTimeOffset:
                    type = "TIMESTAMP";
                    break;

                case System.Data.DbType.Currency:
                case System.Data.DbType.Decimal:
                    type = "DECIMAL";
                    break;

                case System.Data.DbType.Double:
                    type = "DOUBLE";
                    break;

                case System.Data.DbType.Guid:
                    type = "BINARY";
                    options = "(16)";
                    break;

                case System.Data.DbType.Int16:
                    type = "SMALLINT";
                    break;

                case System.Data.DbType.Int32:
                    type = "INT";
                    break;

                case System.Data.DbType.Int64:
                    type = "BIGINT";
                    break;

                case System.Data.DbType.SByte:
                    type = "TINYINT";
                    break;

                case System.Data.DbType.Single:
                    type = "FLOAT";
                    break;

                case System.Data.DbType.Time:
                    type = "TIME";
                    break;

                case System.Data.DbType.UInt16:
                    type = "SMALLINT";
                    options = " UNSIGNED";
                    break;

                case System.Data.DbType.UInt32:
                    type = "INT";
                    options = " UNSIGNED";
                    break;
                case System.Data.DbType.UInt64:
                    type = "BIGINT";
                    options = " UNSIGNED";
                    break;
                case System.Data.DbType.VarNumeric:
                    type = "DECIMAL";
                    break;
                case System.Data.DbType.Xml:
                    type = "LONGTEXT";
                    break;
                default:
                    throw new NotSupportedException("The DbType '" + dbType.ToString() + "'is not supported");

            }

            return type;
        }

        #endregion

        #region public virtual void Begin/End DeclareStatement(DBParam param)

        public virtual void BeginDeclareStatement(DBParam param)
        {
            this.BeginNewLine();

            this.WriteRaw("DECLARE ");
            if (!param.HasName)
                param.Name = this.GetUniqueParameterName(this.ParameterNamePrefix);

            this.WriteParameterReference(param.Name);
            this.WriteRaw(" ");
            string options;
            string type = this.GetNativeTypeForDbType(param.DbType, param.Size, out options);

            this.WriteRaw(type);
            if (!string.IsNullOrEmpty(options))
            {
                this.WriteRaw(options);
                if (!options.EndsWith(" "))
                    this.WriteRaw(" ");
            }
            
        }

        public virtual void EndDeclareStatement()
        {
            if (this.StatementDepth < 1)
                this.WriteRaw(";");
            this.BeginNewLine();
        }

        #endregion

        #region public virtual void Begin/End SetStatement()

        public virtual void BeginSetStatement()
        {
            this.BeginNewLine();

            this.WriteRaw("SET ");
        }

        public virtual void EndSetStatement()
        {
            if (this.StatementDepth < 1)
                this.WriteRaw(";");
            this.BeginNewLine();
        }

        #endregion

        #region public virtual void Begin/End Returns Statement

        public virtual void BeginReturnsStatement()
        {
            this.BeginNewLine();

            this.WriteRaw("RETURN ");
        }

        public virtual void EndReturnsStatement()
        {
            if (this.StatementDepth < 1)
                this.WriteRaw(";");
            this.BeginNewLine();
        }

        #endregion
    }
}
