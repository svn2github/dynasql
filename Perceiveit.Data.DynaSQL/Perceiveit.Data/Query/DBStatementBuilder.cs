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
        }

        #endregion

        //
        // properties
        // 

        #region public List<string> ParameterExclusions {get;}

        private List<string> _exclusions;

        /// <summary>
        /// Gets the list of all the names of parameters that should not be generated.
        /// (i.e. Those that have been declared within the statement)
        /// </summary>
        public List<string> ParameterExclusions
        {
            get
            {
                if (null == _exclusions)
                    _exclusions = new List<string>();
                return _exclusions;
            }
        }

        /// <summary>
        /// Returns true if there are registered parameters that should be excluded from passing on the command
        /// </summary>
        public bool HasExclusionParameters
        {
            get { return null != _exclusions && _exclusions.Count > 0; }
        }

        #endregion

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

        #region protected virtual StatementParameterList Parameters {get;}

        private StatementParameterList _params;

        /// <summary>
        /// Gets the list of parameters in the built statement
        /// </summary>
        protected StatementParameterList Parameters
        {
            get
            {
                if (_params == null)
                    _params = new StatementParameterList();
                return _params;
            }
        }

        #endregion

        #region public virtual bool HasParameters {get;}

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

        /// <summary>
        /// Gets the format string to convert dates to string representations.
        /// Inheritors can overwrite this to return their own formatting
        /// </summary>
        protected virtual string DateFormatString
        {
            get
            {
                return "yyyy-MM-dd HH:mm:ss";
            }
        }

        #endregion

        #region public int StatementDepth {get;}

        private int _depth;

        public int StatementDepth
        {
            get { return _depth; }
            protected set { _depth = value; }
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
        public DBDatabaseProperties DatabaseProperties
        {
            get { return this._dbprops; }
        }

        #endregion

        #region protected DBSchemaTypes CurrentlyCreating {get;set;}

        private DBSchemaTypes _currentCreation = DBSchemaTypes.None; //Not true but not used.

        /// <summary>
        /// Defines the type of object this builder is generating the CREATE statement for
        /// </summary>
        protected DBSchemaTypes CurrentlyCreating
        {
            get { return _currentCreation; }
            set { _currentCreation = value; }
        }

        #endregion

        //
        // .ctor(s)
        //

        #region protected DBStatementBuilder(DBDatabase database, DBDatabaseProperties properties, System.IO.TextWriter writer, bool ownsWriter)

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
        // checking support
        //

        #region public bool SupportsStatement(DBSchemaTypes type, DBSchemaOperation operation)

        /// <summary>
        /// Checks if a particular operation is supported
        /// </summary>
        /// <param name="type"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        public bool SupportsStatement(DBSchemaTypes type, DBSchemaOperation operation)
        {
            return this.DatabaseProperties.CheckSupports(type, operation);
        }

        #endregion

        #region public StatementParameterList GetPassingParameters()

        /// <summary>
        /// Gets the collection of parameters that should be passed by the DbCommand
        /// to the database engine. Rather than any declared parameters within the statement
        /// </summary>
        /// <returns></returns>
        public StatementParameterList GetPassingParameters()
        {
            if (this.HasParameters == false)
                return null;
            else if (this.HasExclusionParameters == false)
                return this.Parameters;
            else
            {
                StatementParameterList topass = new StatementParameterList();
                foreach (StatementParameter p in this.Parameters)
                {
                    if (!this.ParameterExclusions.Contains(p.ParameterName))
                        topass.Add(p);
                }
                return topass;
            }

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

        public virtual void BeginFunctionParameter(int index)
        {
            if (index > 0)
                this.WriteReferenceSeparator();
        }

        public virtual void EndFunctionParameter(int index)
        {
        }

        /// <summary>
        /// Appends an individual separator to the statement ', '
        /// </summary>
        public virtual void WriteReferenceSeparator()
        {
            this.Writer.Write(", ");
        }

        #endregion

        #region Query Hints

        public virtual void BeginQueryHints()
        {
            this.WriteRawSQLString(" OPTIONS (");
        }

        public virtual void EndQueryHints()
        {
            this.EndBlock();
        }

        public virtual void BeginAQueryHint(DBQueryOption hint)
        {
            string s = this.GetQueryHintWord(hint);
            this.WriteRawSQLString(s);
        }

        public virtual void EndAQueryHint(DBQueryOption hint)
        {
        }

        protected virtual string GetQueryHintWord(DBQueryOption option)
        {
            
            string value;
            switch (option)
            {
                case DBQueryOption.HashGroup:
                    value = "HASH GROUP";
                    break;
                case DBQueryOption.OrderGroup:
                    value = "ORDER GROUP";
                    break;
                case DBQueryOption.ConcatUnion:
                    value = "CONCAT UNION";
                    break;
                case DBQueryOption.HashUnion:
                    value = "HASH UNION";
                    break;
                case DBQueryOption.MergeUnion:
                    value = "MERGE UNION";
                    break;
                case DBQueryOption.LoopJoin:
                    value = "LOOP JOIN";
                    break;
                case DBQueryOption.MergeJoin:
                    value = "MERGE JOIN";
                    break;
                case DBQueryOption.HashJoin:
                    value = "HASH JOIN";
                    break;
                case DBQueryOption.ExpandViews:
                    value = "EXPAND VIEWS";
                    break;
                case DBQueryOption.Fast:
                    value = "FAST";
                    break;
                case DBQueryOption.ForceOrder:
                    value = "FORCE ORDER";
                    break;
                case DBQueryOption.KeepPlan:
                    value = "KEEP PLAN";
                    break;
                case DBQueryOption.KeepFixedPlan:
                    value = "KEEPFIXED PLAN";
                    break;
                case DBQueryOption.MaxDOP:
                    value = "MAXDOP";
                    break;
                case DBQueryOption.MaxRecursion:
                    value = "MAXRECURSION";
                    break;
                case DBQueryOption.OptimizeFor:
                    value = "OPTIMIZE FOR";
                    break;
                case DBQueryOption.OptimiseForUnknown:
                    value = "OPTIMIZE FOR UNKNOWN";
                    break;
                case DBQueryOption.ParametrizationSimple:
                    value = "PARAMETERIZATION SIMPLE";
                    break;
                case DBQueryOption.ParameterizationForced:
                    value = "PARAMETERIZATION FORCED";
                    break;
                case DBQueryOption.Recompile:
                    value = "RECOMPLIE";
                    break;
                case DBQueryOption.RobustPlan:
                    value = "ROBUST PLAN";
                    break;
                case DBQueryOption.UsePlan:
                    value = "USE PLAN";
                    break;
                case DBQueryOption.Raw:
                default:
                    value = "";
                    break;
            }
            return value;
        }

        #endregion

        #region Table Hints

        public virtual void BeginTableHints()
        {
            this.WriteRawSQLString(" WITH ");
            this.BeginBlock();
        }

        public virtual void EndTableHints()
        {
            this.EndBlock();
        }

        public virtual void BeginTableHint(DBTableHint hint)
        {
            string s = this.GetTableHintWord(hint);
            this.WriteRawSQLString(s);
        }

        public virtual void EndTableHint(DBTableHint hint)
        {
        }

        public virtual void BeginHintParameterList()
        {
            this.BeginBlock();
        }

        public virtual void EndHintParameterList()
        {
            this.EndBlock();
        }

        public virtual void WriteHintParameter(int index, string parameter)
        {
            if (index > 0)
                this.WriteReferenceSeparator();
            this.WriteObjectName(parameter);
        }

        public virtual string GetTableHintWord(DBTableHint hint)
        {
            if (hint == DBTableHint.Raw)
                return "";
            else
                return hint.ToString().ToUpper();
        }

        #endregion

        #region public virtual void BeginSubSelect() + EndSubSelect()

        public virtual void BeginSubStatement()
        {
            //We need to check that we are not in a CREATE PROCEDURE ... state
            if (this.StatementDepth <= 1 && this.CurrentlyCreating == DBSchemaTypes.StoredProcedure)
                return;
            else
                this.Writer.Write(" (");
        }

        public virtual void EndSubStatement()
        {
            //We need to check that we are not in a CREATE PROCEDURE ... state
            if (this.StatementDepth <= 2 && this.CurrentlyCreating == DBSchemaTypes.StoredProcedure)
                return;
            else
            {
                this.Writer.Write(") ");
                this.BeginNewLine();
            }
        }

        #endregion

        #region public virtual char GetStartIdentifier() + GetEndIdentifier() + GetIdentifierSeparator()

        /// <summary>
        /// returns the character that marks the start of an object identifier (default '[')
        /// </summary>
        /// <returns></returns>
        public virtual char GetStartIdentifier()
        {
            return '[';
        }

        /// <summary>
        /// returns the character that marks the end of an object identifier (default ']')
        /// </summary>
        /// <returns></returns>
        public virtual char GetEndIdentifier()
        {
            return ']';
        }

        /// <summary>
        /// returns the character that separates object identifiers (default '.')
        /// </summary>
        /// <returns></returns>
        public virtual char GetIdentifierSeparator()
        {
            return '.';
        }

        #endregion

        #region public void BeginIdentifier() + EndIdentifier() + AppendIdSeparator()

        public void BeginIdentifier()
        {
            char start = GetStartIdentifier();
            this.Writer.Write(start);
        }

        public void EndIdentifier()
        {
            char end = GetEndIdentifier();
            this.Writer.Write(end);
        }

        public void AppendIdSeparator()
        {
            char id = GetIdentifierSeparator();
            this.Writer.Write(id);
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

            this.WriteRawSQLString("SELECT ");
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
            this.Writer.Write(";");
        }

        #endregion

        #region public virtual void BeginSelectList() + EndSelectList()

        /// <summary>
        /// Begins the list of select fields and clauses
        /// </summary>
        public virtual void BeginSelectList()
        {
        }

        /// <summary>
        /// Ends the list of selected fields and clauses
        /// </summary>
        public virtual void EndSelectList()
        {
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

        public void BeginBlock(bool indent)
        {
            this.BeginBlock();
            if (indent)
                this.IncrementStatementDepth();
        }

        public virtual void BeginBlock()
        {
            this.Writer.Write(" (");
        }

        public virtual void EndBlock()
        {
            this.Writer.Write(") ");
        }

        public void EndBlock(bool outdent)
        {
            this.EndBlock();
            if (outdent)
                this.DecrementStatementDepth();

        }

        #endregion

        #region public virtual void BeginEntityDefinition() + EndEntityDefinition()
        /// <summary>
        /// Starts the actual definition of an entity such as a View or Stored Procedure after declaring the name
        /// </summary>
        public virtual void BeginEntityDefinition()
        {
            this.WriteRawSQLString(" AS "); 
            this.BeginNewLine();
        }

        /// <summary>
        /// Ends the definition of an entity such as a View or Stored Procedure
        /// </summary>
        public virtual void EndEntityDefinition()
        {
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

        public virtual void BeginOrderClause(Order order)
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
            if(!string.IsNullOrEmpty(o))
                this.WriteRawSQLString(o);
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

        //
        // update
        //

        #region public void BeginUpdateStatement() + EndUpdateStatement()

        public virtual void BeginUpdateStatement()
        {
            this.BeginNewLine();
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
            this.BeginNewLine();
            this.WriteRawSQLString(" SET ");
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

        #region public virtual void WriteAssignValue(DBClause receiver, DBClause value)

        /// <summary>
        /// Writes an assignment (e.g. receiver = value )
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="value"></param>
        public virtual void WriteAssignValue(DBClause receiver, DBClause value)
        {
            receiver.BuildStatement(this);
            this.WriteAssignmentOperator();
            value.BuildStatement(this);
        }

        #endregion

        #region protected virtual void WriteAssignmentOperator()

        /// <summary>
        /// Writes the asignment operator (=)
        /// </summary>
        protected virtual void WriteAssignmentOperator()
        {
            this.WriteOperator(Operator.Equals);
        }

        #endregion

        //
        // insert
        //

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

        //
        // delete
        //

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
                case Function.Concatenate:
                    name = "CONCAT";
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
            this.BeginNewLine();
        }

        public virtual void EndScript()
        {
            this.BeginNewLine();
            this.Writer.WriteLine("END");
            if (this.StatementDepth <= 1)
                this.WriteStatementTerminator();
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

        //
        // create / drop
        //

        #region public virtual void BeginDrop(DBSchemaTypes type, string owner, string name, bool checkExists) + EndDrop(type)

        public virtual void BeginDropStatement(DBSchemaTypes type, string owner, string name, bool checkExists)
        {
            
            if (checkExists)
            {
                this.BeginCheckExists(type, owner, name);
            }
            this.BeginDropStatement(type);
            this.WriteSourceTable(owner, name, string.Empty);
            if (checkExists)
            {
                this.EndCheckExists(type, owner, name);
            }

            
        }

        public virtual void BeginDropStatement(DBSchemaTypes type)
        {
            this.BeginNewLine();
            this.IncrementStatementDepth();

            this.WriteRawSQLString("DROP ");
            WriteDropType(type);
        }

        protected virtual void WriteDropType(DBSchemaTypes type)
        {
            switch (type)
            {
                case DBSchemaTypes.Table:
                    this.WriteDropType("TABLE");
                    break;
                case DBSchemaTypes.View:
                    this.WriteDropType("VIEW");
                    break;
                case DBSchemaTypes.StoredProcedure:
                    this.WriteDropType("PROCEDURE");
                    break;
                case DBSchemaTypes.Function:
                    this.WriteDropType("FUNCTION");
                    break;
                case DBSchemaTypes.Index:
                    this.WriteDropType("INDEX");
                    break;
                case (DBSchemaTypes)0:
                default:
                    throw new ArgumentOutOfRangeException("type");

            }
            this.WriteSpace();
        }

        protected void WriteDropType(string type)
        {
            this.WriteRawSQLString(type);
        }

        public virtual void EndDrop(DBSchemaTypes type, bool checkExists)
        {
            //this.WriteStatementTerminator(); //Not sure why - validate removal against unit tests
            this.DecrementStatementDepth();
        }


        #endregion

        #region public virtual void BeginReferenceOn() + EndReferenceOn()
        /// <summary>
        /// Starts the ON section of a SQL statement e.g CREATE INDEX [name] ON .....
        /// </summary>
        public virtual void BeginReferenceOn()
        {
            this.WriteRawSQLString(" ON ");
        }

        /// <summary>
        /// Ends the ON section of a SQL statement after all the ON parts have been added.
        /// </summary>
        public virtual void EndReferenceOn()
        { }

        #endregion

        #region public virtual string GetCreateOption(CreateOptions option)

        /// <summary>
        /// Gets the implementation specific string for the CREATE ... options including identifiers for unique contraints and temportary tables
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public virtual string GetCreateOption(CreateOptions option)
        {
            List<string> all = new List<string>();
            if ((option & CreateOptions.Unique) > 0)
                all.Add("UNIQUE");
            if ((option & CreateOptions.Temporary) > 0)
                all.Add("TEMP");
            if ((option & CreateOptions.Clustered) > 0)
                all.Add("CLUSTERED");
            if ((option & CreateOptions.NonClustered) > 0)
                all.Add("NONCLUSTERED");

            return string.Join(" ", all.ToArray());

        }

        #endregion

        #region protected virtual void BeginCreate(DBSchemaTypes type) + EndCreate(type)

       
        /// <summary>
        /// Starts a new CREATE ... statement with the owner.name any options (from the GetCreateOptions) 
        /// and a flag to identifiy if this should be checked for non existance first
        /// </summary>
        /// <param name="type">The type of schema object to create</param>
        /// <param name="owner">The shema owner for the database object that will be created</param>
        /// <param name="name">The name for the database object that will ber created</param>
        /// <param name="options"></param>
        /// <param name="checknotexists"></param>
        public virtual void BeginCreate(DBSchemaTypes type, string owner, string name, string options, bool checknotexists)
        {
            this.CurrentlyCreating = type;

            if (checknotexists)
                this.BeginCheckNotExists(type, owner, name);

            //If the Foreign Key has a name it generally  comes before the FOREIGN KEY declaration but behind a CONSTRAINT marker.
            //CONSTRAINT is not required otherwise.
            bool isconstraint = IsConstraintType(type);
            if (isconstraint)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    this.WriteRawSQLString("CONSTRAINT ");
                    this.BeginIdentifier();
                    this.WriteObjectName(name);
                    this.EndIdentifier();
                    this.WriteSpace();
                }
            }
            else
                this.BeginNewLine();
            this.IncrementStatementDepth();
            this.WriteCreateType(type, options, isconstraint);
            
            if(!isconstraint)
                this.WriteSourceTable(owner, name, string.Empty);

            if (checknotexists)
                this.EndCheckExists(type, owner, name);
        }

        
        /// <summary>
        /// Starts a new CREATE ... statement
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options"></param>
        public virtual void BeginCreate(DBSchemaTypes type, string options)
        {
            this.CurrentlyCreating = type;
            bool constraint = this.IsConstraintType(type);
            if(!constraint)
                this.BeginNewLine();

            this.IncrementStatementDepth();
            WriteCreateType(type, options, constraint);
            
        }

        /// <summary>
        /// Outputs the CREATE XXX part
        /// </summary>
        /// <param name="type"></param>
        /// <param name="options"></param>
        protected virtual void WriteCreateType(DBSchemaTypes type, string options, bool isconstraint)
        {
            string nativetype;
            switch (type)
            {
                case DBSchemaTypes.Table:
                    nativetype = "TABLE";
                    break;
                case DBSchemaTypes.View:
                    nativetype = "VIEW";
                    break;
                case DBSchemaTypes.StoredProcedure:
                    nativetype = "PROCEDURE";
                    break;
                case DBSchemaTypes.Function:
                    nativetype = "FUNCTION";
                    break;
                case DBSchemaTypes.Index:
                    nativetype = "INDEX";
                    break;
                case DBSchemaTypes.ForeignKey:
                    nativetype = "FOREIGN KEY";
                    break;
                case DBSchemaTypes.PrimaryKey:
                    nativetype = "PRIMARY KEY";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(type.ToString(),String.Format(Errors.CannotBuildCreateType,type.ToString()));
            }

            WriteCreateType(nativetype, options, isconstraint);
            this.WriteSpace();
        }

        protected virtual void WriteCreateType(string nativeType, string options, bool isconstraint)
        {
            if (!isconstraint)
                this.WriteRawSQLString("CREATE ");

            if (string.IsNullOrEmpty(options) == false)
            {
                if (options.EndsWith(" ") == false)
                    options = options + " ";

                this.WriteRawSQLString(options);
            }

            this.WriteRawSQLString(nativeType);
        }

        public virtual void EndCreate(DBSchemaTypes type, bool checknotexists)
        {
            bool constraint = this.IsConstraintType(type);
            //if (!constraint && type != DBSchemaTypes.StoredProcedure)
            //{
            //    if (this.StatementDepth > 1)
            //        this.EndSubStatement();
            //    else
            //        this.WriteStatementTerminator();
            //}
            this.DecrementStatementDepth();

            this.CurrentlyCreating = DBSchemaTypes.None;
        }

        protected virtual bool IsConstraintType(DBSchemaTypes type)
        {
            return (type == DBSchemaTypes.ForeignKey) || (type == DBSchemaTypes.PrimaryKey);
        }

        #endregion

        #region public virtual void BeginTableConstraints() + EndTableConstraints()

        /// <summary>
        /// Begins the list of CreateTable constraints (e.g. Foreign Keys).
        /// </summary>
        public virtual void BeginTableConstraints()
        {
            //We must have columns on the table, so we append a comma and start the constraints.
            this.WriteReferenceSeparator();
            //this.BeginNewLine();
            //this.IncrementStatementDepth();
        }

        /// <summary>
        /// Ends the list of CreateTable constraints (e.g Foreign Keys).
        /// </summary>
        public virtual void EndTableConstraints()
        {
            //this.DecrementStatementDepth();
        }

        #endregion 

        //
        // foreign key on update, on delete and references statements.
        // 

        #region public virtual void BeginForeignKeyUpdateActions()

        /// <summary>
        /// Begins the list of update actions
        /// </summary>
        public virtual void BeginForeignKeyUpdateActions()
        {

        }

        #endregion

        #region public void WriteDeleteAction(DBFKAction perform)

        /// <summary>
        /// Writes the ON DELETE action
        /// </summary>
        /// <param name="perform"></param>
        public void WriteDeleteAction(DBFKAction perform)
        {
            this.WriteForeignKeyAction("DELETE", perform);
        }

        #endregion

        #region public void WriteUpdateAction(DBFKAction perform)

        /// <summary>
        /// Writes the ON UPDATE action
        /// </summary>
        /// <param name="perform"></param>
        public void WriteUpdateAction(DBFKAction perform)
        {
            this.WriteForeignKeyAction("UPDATE", perform);
        }

        #endregion

        #region protected virtual void WriteForeignKeyAction(string actionOn, DBFKAction perform)

        /// <summary>
        /// Outputs the ON [action] [perform] for a foreign key
        /// </summary>
        /// <param name="actionOn"></param>
        /// <param name="perform"></param>
        protected virtual void WriteForeignKeyAction(string actionOn, DBFKAction perform)
        {
            switch (perform)
            {
                case DBFKAction.Cascade:
                    this.WriteRawSQLString(" ON ");
                    this.WriteRawSQLString(actionOn);
                    this.WriteRawSQLString(" CASCADE");

                    break;
                case DBFKAction.NoAction:
                    this.WriteRawSQLString(" ON ");
                    this.WriteRawSQLString(actionOn);
                    this.WriteRawSQLString(" NO ACTION");

                    break;
                case DBFKAction.Undefined:
                default:
                    break;
            }
        }
        #endregion

        #region public void EndForeignKeyUpdateActions()

        /// <summary>
        /// Ends the list of foreign key update actions
        /// </summary>
        public virtual void EndForeignKeyUpdateActions()
        {

        }

        #endregion

        #region public void BeginReferences(string owner, string name)

        /// <summary>
        /// Begins a REFERENCES ... section
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        public void BeginReferences(string owner, string name)
        {
            this.WriteRawSQLString(" REFERENCES ");
            this.WriteSourceTable(owner, name, string.Empty);
        }

        #endregion

        #region public void EndReferences(string owner, string name)

        /// <summary>
        /// Ends the REFERENCES ... section
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        public void EndReferences(string owner, string name)
        {

        }

        #endregion

        //
        // check exists
        //

        #region public virtual void BeginCheckExists(DBSchemaTypes type, string owner, string name) + EndCheckExists()


        /// <summary>
        /// Starts the IF EXISTS ... block - usually called from the BeginDrop method
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        public virtual void BeginCheckExists(DBSchemaTypes type, string owner, string name)
        {
            this.WriteRawSQLString("IF EXISTS");
            this.IncrementStatementDepth();
            BuildInfoSchemaLookup(type, owner, name);

        }

        /// <summary>
        /// Ends the IF EXISTS ... block usually called from the BeginDrop method.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        public virtual void EndCheckExists(DBSchemaTypes type, string owner, string name)
        {
            this.DecrementStatementDepth();
        }

        #endregion

        #region public virtual void BeginCheckNotExists(DBSchemaTypes type, string owner, string name) + EndCheckNotExisits()

        /// <summary>
        /// Starts the IF NOT EXISTS block  - usually called by the create statement
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        public virtual void BeginCheckNotExists(DBSchemaTypes type, string owner, string name)
        {
            this.WriteRawSQLString("IF NOT EXISTS");
            this.IncrementStatementDepth();
            BuildInfoSchemaLookup(type, owner, name);
        }

        /// <summary>
        /// Ends the IF NOT EXISTS block
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        public virtual void EndCheckNotExists(DBSchemaTypes type, string owner, string name)
        {
            this.DecrementStatementDepth();
        }

        #endregion

        #region  private void BuildInfoSchemaLookup(DBSchemaTypes type, string owner, string name)

        /// <summary>
        /// Builds the lookup for the INFORMATION_SCHEMA.XXX view to check for existance of particular entries.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <remarks>Called by the BeginCheckExists and BeginCheckNotExists. The SchemaInformation is loaded from the 
        /// database properties and the lookups are references to the columns in the view for the schema</remarks>
        private void BuildInfoSchemaLookup(DBSchemaTypes type, string owner, string name)
        {

            DBSchemaInformation info = this.DatabaseProperties.SchemaInformation;
            DBSchemaInfoLookup lookup = info.GetListLookup(type);

            //Generate a sub select statement to look at the INFORMATION_SCHEMA to check it exists
            DBSelectQuery sel = DBQuery.SelectAll().From(info.SchemaName, lookup.TableName)
                                                   .WhereFieldEquals(lookup.LookupNameColumn, DBConst.String(name));
            if (!string.IsNullOrEmpty(owner))
                sel = sel.AndWhere(lookup.LookupSchemaColumn, Compare.Equals, DBConst.String(owner));

            //Add this statement to self
            sel.BuildStatement(this);
        }

        #endregion

        //
        // script
        //

        #region public virtual void Begin/End DeclareStatement(DBParam param)

        public virtual void BeginDeclareStatement(DBParam param)
        {
            this.WriteRawSQLString("DECLARE ");
            this.WriteParameter(param, false);
            this.ParameterExclusions.Add(param.Name);
        }

        

        public virtual void EndDeclareStatement()
        {
            if (this.StatementDepth < 1)
                this.WriteStatementTerminator();
            this.BeginNewLine();
        }

        #endregion

        #region public virtual void BeginProcedureParameters() + EndProcedureParameters()

        private bool _isDeclaringParameters = false;

        /// <summary>
        /// Returns true if we a currently registering parameters
        /// </summary>
        protected bool IsDeclaringParameters
        {
            get { return _isDeclaringParameters; }
            set { _isDeclaringParameters = value; }
        }

        /// <summary>
        /// Starts the list of parameters for a stored procedure
        /// </summary>
        public virtual void BeginProcedureParameters()
        {
            this.IsDeclaringParameters = true;
            this.BeginBlock(true);
        }

        /// <summary>
        /// Ends the list of parameters for a stored procedure
        /// </summary>
        public virtual void EndProcedureParameters()
        {
            this.EndBlock(true);
            this.IsDeclaringParameters = false;
        }

        #endregion

        #region public virtual void WriteParameter(DBParam param, bool includeDirection) + 1 overload

        public virtual void WriteParameter(DBParam param, bool includeDirection)
        {
            bool writeType = true;
            this.WriteParameter(param, writeType, includeDirection);
        }

        public virtual void WriteParameter(DBParam param, bool writetype, bool includeDirection)
        {
            if (!param.HasName)
                param.Name = this.GetUniqueParameterName();

            this.WriteNativeParameterReference(param.Name);

            if (writetype)
            {
                this.WriteSpace();
                string options;
                string type = this.GetNativeTypeForDbType(param.DbType, param.Size, param.Precision, DBColumnFlags.Nullable, out options);

                this.WriteRawSQLString(type);
                if (!string.IsNullOrEmpty(options))
                {
                    this.WriteRawSQLString(options);
                    if (!options.EndsWith(" "))
                        this.WriteSpace();
                }
            }

            if (includeDirection && param.Direction != ParameterDirection.Input)
            {
                switch (param.Direction)
                {
                    case ParameterDirection.InputOutput:
                    case ParameterDirection.Output:
                        this.WriteRawSQLString("OUTPUT ");
                        break;
                    case ParameterDirection.ReturnValue:
                        break;
                    default:
                        break;
                }
            }

        }

        #endregion

        #region public virtual void Begin/End SetStatement()

        public virtual void BeginSetStatement()
        {
            this.BeginNewLine();

            this.WriteRawSQLString("SET ");
        }

        public virtual void EndSetStatement()
        {
            if (this.StatementDepth < 1)
                this.WriteStatementTerminator();
            this.BeginNewLine();
        }

        #endregion

        #region public virtual void Begin/End Returns Statement

        public virtual void BeginReturnsStatement()
        {
            this.BeginNewLine();

            this.WriteRawSQLString("RETURN ");
        }

        public virtual void EndReturnsStatement()
        {
            if (this.StatementDepth < 1)
                this.WriteStatementTerminator();
            this.BeginNewLine();
        }

        #endregion

        #region public virtual void BeginUseStatement() + EndUseStatement()

        public virtual void BeginUseStatement()
        {
            this.WriteRawSQLString("USE ");
        }

        public virtual void EndUseStatement()
        {
            if(this.StatementDepth < 1)
                this.WriteStatementTerminator();

            this.BeginNewLine();

        }

        #endregion

        //
        // parameter addition, conversion and naming
        //

        #region Stop/Resume/Should Register(ing) Paramters

        private int _monitoringParams = 0;

        /// <summary>
        /// Stops automatically registering the parameter references when they are registered
        /// </summary>
        public virtual void StopRegisteringParameters()
        {
            _monitoringParams = _monitoringParams - 1;
        }

        /// <summary>
        /// Resumes the atomatic adding of parameter references to this builders collection when they are registered.
        /// </summary>
        public virtual void ResumeRegisteringParameters()
        {
            _monitoringParams = _monitoringParams + 1;
        }

        /// <summary>
        /// Checks the monitoring flag to see if the parameters should register themseleves with the builder.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ShouldRegisterParameters()
        {
            return _monitoringParams >= 0;
        }

        #endregion

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
                paramName = this.GetUniqueParameterName();
                declared = false;
            }
            else if (this.Parameters.Contains(paramName))
            {
                if (this.DatabaseProperties.ParameterLayout == DBParameterLayout.Named)
                    //already registered
                    return paramName;
                else
                {
                    paramName = this.GetUniqueParameterName();
                }
            }
            else
                declared = true;

            if (this.ShouldRegisterParameters())
            {
                StatementParameter sp = new StatementParameter();
                sp.ParameterName = paramName;
                sp.ValueSource = source;
                sp.ParameterIndex = this.Parameters.Count;
                if (!declared)
                    sp.ValueSource.Name = paramName;

                this.Parameters.Add(sp);
            }

            return paramName;
        }

        #endregion

        #region public virtual void WriteParameterReference(string paramname)

        /// <summary>
        /// Appends a parameter reference to the statement in database native format
        /// </summary>
        /// <param name="paramname">The coded parameter name</param>
        /// <exception cref="ArgumentNullException" >Thown if the paramname value is null or empty</exception>
        public virtual void WriteNativeParameterReference(string paramname)
        {
            if (string.IsNullOrEmpty(paramname))
                throw new ArgumentNullException("Cannot append a null or empty parameter reference - implementors: Use the GetUniqueID to support unnamed parameters");
            
            string native = this.GetNativeParameterName(paramname, true);
            if (this.IsDeclaringParameters)
            {
                if (!this.ParameterExclusions.Contains(native))
                    this.ParameterExclusions.Add(native);
            }

            this.Writer.Write(native);
        }

        #endregion

        #region public virtual string GetUniqueParameterName()

        /// <summary>
        /// Returns a new unique name of a parameter.
        /// </summary>
        /// <returns></returns>
        public virtual string GetUniqueParameterName()
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

        public virtual void PopulateParameter(DbParameter p, StatementParameter sparam)
        {
            string name = sparam.ParameterName;
            //if (string.IsNullOrEmpty(name))
            //    p.SourceColumn = this.GetUniqueParameterName(DBParam.ParameterNamePrefix);
            //else
            //    p.SourceColumn = name;
            bool inline = false;
            p.ParameterName = this.GetNativeParameterName(name, inline);

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

        #region public virtual string GetNativeParameterName(string paramName, bool forstatement)

        /// <summary>
        /// Returns a native name of a paramerter including any required prefix e.g @ or :
        /// If the name will be part of the SQL statement then forstatement will be true.
        /// </summary>
        /// <param name="paramName">The name stem</param>
        /// <param name="forstatement">True if this is inline as part of the statement, 
        /// otherwise false for a parameter that will be added as one of the command parameters collection.</param>
        /// <returns></returns>
        public virtual string GetNativeParameterName(string paramName, bool forstatement)
        {
            return string.Format(this.DatabaseProperties.ParameterFormat, paramName);
        }

        #endregion

        //
        // write methods
        //

        #region public virtual void WriteSpace()

        /// <summary>
        /// Writes a blank space
        /// </summary>
        public virtual void WriteSpace()
        {
            this.Writer.Write(" ");
        }

        #endregion

        #region public virtual void WriteTop(double value, double offset, TopType topType)

        public virtual void WriteTop(double count, double offset, TopType topType)
        {
            if (Array.IndexOf<TopType>(this.DatabaseProperties.SupportedTopTypes, topType) < 0)
                throw new NotSupportedException("The top type '" + topType.ToString() + "' is not supported by this database");

            this.WriteRawSQLString(" TOP ");
            if (topType == TopType.Percent)
            {
                this.Write(count);
                this.WriteRawSQLString(" PERCENT ");
            }
            else if(topType == TopType.Count)
            {
                this.Writer.Write((int)count);
                this.WriteSpace();
            }
            else if (topType == TopType.Range)
            {
                this.Write((int)count);
                this.WriteReferenceSeparator();
                this.Write((int)offset);
            }

        }

        #endregion

        #region public virtual void WriteDistinct()

        public virtual void WriteDistinct()
        {
            this.Writer.Write(" DISTINCT ");
        }

        #endregion

        #region public virtual void WriteRawSQLString(string value)

        /// <summary>
        /// Writes a SQL string that is not validated - as it is expected to contain cotrol characters
        /// </summary>
        /// <param name="value"></param>
        public virtual void WriteRawSQLString(string value)
        {
            this.Writer.Write(value);
        }

        #endregion

        #region public virtual void WriteObjectName(string name)

        /// <summary>
        /// Writes the name of an object in the database - Does not include the Identifier separators
        /// </summary>
        /// <param name="name">The name of the object</param>
        /// <exception cref="System.ArgumentException" >Thrown if the name contains one of the start or end object identifier characters</exception>
        public virtual void WriteObjectName(string name)
        {
            char start = this.GetStartIdentifier();
            char end = this.GetEndIdentifier();
            if (name.IndexOf(start) > -1 || name.IndexOf(end) > -1)
                throw new ArgumentException(String.Format(Errors.ObjectNamesCannotContainIdentifierCharacters, name, start, end));

            this.Writer.Write(name);
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
                            this.WriteNull();
                        else
                            this.Writer.Write(value);
                        break;
                }
            }
        }

        #endregion

        #region public virtual void WriteNull() + WriteNotNull()

        public virtual void WriteNull()
        {
            this.Writer.Write("NULL");
        }

        public virtual void WriteNotNull()
        {
            this.Writer.Write("NOT NULL");
        }

        #endregion

        #region public virtual void WriteAllFieldIdentifier(string catalogOwner,string schemaOwner, string sourceTable)

        public virtual void WriteAllFieldIdentifier(string catalogOwner, string schemaOwner, string sourceTable)
        {
            if (string.IsNullOrEmpty(sourceTable) && !string.IsNullOrEmpty(schemaOwner))
                throw new ArgumentNullException("sourceTable", Errors.CannotSpecifySchemaOwnerAndNotTable);

            if (string.IsNullOrEmpty(catalogOwner) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(catalogOwner);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            if (string.IsNullOrEmpty(schemaOwner) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(schemaOwner);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }
            else if (!string.IsNullOrEmpty(catalogOwner))
                this.AppendIdSeparator();

            if (string.IsNullOrEmpty(sourceTable) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(sourceTable);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }
            else if (!string.IsNullOrEmpty(catalogOwner) || !string.IsNullOrEmpty(sourceTable))
                this.AppendIdSeparator(); //catalog...* or catalog.dbo..*

            this.Writer.Write(this.GetAllFieldIdentifier());
        }

        #endregion

        #region public virtual void WriteSourceField(string catalog, string schemaOwner, string sourceTable, string columnName, string alias)

        /// <summary>
        /// Appends the owner (optional), table (optional unless owner is specified) and column (required) to the statement and
        /// sets the alias name (optional). All the identifers are wrapped within the identifier delimiter characters
        /// </summary>
        /// <param name="schemaOwner"></param>
        /// <param name="sourceTable"></param>
        /// <param name="columnName"></param>
        /// <param name="alias"></param>
        public virtual void WriteSourceField(string catalog, string schemaOwner, string sourceTable, string columnName, string alias)
        {
            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentNullException("columnName", Errors.NoColumnNameSpecifiedForAField);

            if (string.IsNullOrEmpty(sourceTable) && !string.IsNullOrEmpty(schemaOwner))
                throw new ArgumentNullException("sourceTable", Errors.CannotSpecifySchemaOwnerAndNotTable);

            if (string.IsNullOrEmpty(catalog) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            if (string.IsNullOrEmpty(schemaOwner) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(schemaOwner);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }
            else if (!string.IsNullOrEmpty(catalog))
                this.AppendIdSeparator();

            if (string.IsNullOrEmpty(sourceTable) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(sourceTable);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }
            else if (!string.IsNullOrEmpty(schemaOwner) || !string.IsNullOrEmpty(catalog))
                this.AppendIdSeparator(); //catalog...field || catalog.dbo..field

            this.BeginIdentifier();
            this.WriteObjectName(columnName);
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
        public void WriteSourceTable(string schemaOwner, string sourceTable, string alias)
        {
            this.WriteSourceTable(string.Empty, schemaOwner, sourceTable, alias);
        }

        #endregion


        #region public virtual void WriteSourceTable(string catalog, string schemaOwner, string sourceTable, string alias)

        /// <summary>
        /// Appends the catalog (optional) owner (optional) and table (required) to the statement and sets the alias name (optional).
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
                this.WriteObjectName(catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            if (string.IsNullOrEmpty(schemaOwner) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(schemaOwner);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }
            else if (string.IsNullOrEmpty(catalog) == false)
                this.AppendIdSeparator(); //catalog..table

            this.BeginIdentifier();
            this.WriteObjectName(sourceTable);
            this.EndIdentifier();

            if (string.IsNullOrEmpty(alias) == false)
            {
                WriteAlias(alias);
            }
        }

        #endregion

        #region public virtual void WriteSource(string schemaOwner, string sourceTable)

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
                this.WriteObjectName(catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            if (string.IsNullOrEmpty(schemaOwner) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(schemaOwner);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }
            else if (!string.IsNullOrEmpty(catalog))
                this.AppendIdSeparator(); // catalog..source

            if (string.IsNullOrEmpty(source) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(source);
                this.EndIdentifier();
            }

        }

        #endregion

        #region public virtual void WriteAlias(string alias)

        public virtual void WriteAlias(string alias)
        {
            this.BeginAlias();
            this.BeginIdentifier();
            this.WriteObjectName(alias);
            this.EndIdentifier();
            this.EndAlias();
        }

        #endregion

        #region public virtual void WriteSubQueryAlias(string alias)

        /// <summary>
        /// Writes the alias name of a sub query
        /// </summary>
        /// <param name="alias"></param>
        public virtual void WriteSubQueryAlias(string alias)
        {
            this.WriteAlias(alias);
        }

        #endregion

        #region public virtual void WriteDataType(DbType type, int length, int accuracy)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="length"></param>
        /// <param name="accuracy"></param>
        /// <param name="flags"></param>
        public virtual void WriteColumnDataType(DbType type, int length, int accuracy, DBColumnFlags flags)
        {
            string opts;
            string sqltype = GetNativeTypeForDbType(type, length, accuracy, flags, out opts);
            this.WriteRawSQLString(sqltype);
            if (!string.IsNullOrEmpty(opts))
                this.WriteRawSQLString(opts);
        }

        #endregion

        #region public virtual void WriteColumnFlags(DBColumnFlags flags, DBClause defaultValue)

        /// <summary>
        /// Writes the flags associated with a colums data type and identity
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="defaultValue"></param>
        public virtual void WriteColumnFlags(DBColumnFlags flags, DBClause defaultValue)
        {
            if ((flags & DBColumnFlags.PrimaryKey) > 0)
                this.WriteRawSQLString(" PRIMARY KEY");

            this.WriteSpace();
            if ((flags & DBColumnFlags.Nullable) > 0)
                this.WriteNull();
            else
                this.WriteNotNull();

            if ((flags & DBColumnFlags.AutoAssign) > 0)
                this.WriteRawSQLString(" IDENTITY");

            if ((flags & DBColumnFlags.HasDefault) > 0)
            {
                this.WriteRawSQLString(" DEFAULT ");
                defaultValue.BuildStatement(this);
            }
        }

        #endregion

 
        public virtual void WriteSequenceOption(DBSequenceBuilderOption option)
        {
            throw new NotSupportedException(Errors.SequenceCreationIsNotSupportedOnThisEngine);
        }

        public virtual void WriteSequenceOption(DBSequenceBuilderOption option, int value)
        {
            throw new NotSupportedException(Errors.SequenceCreationIsNotSupportedOnThisEngine);
        }

       

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

        #region public virtual void GenerateCreateXXXScript - Not Supported

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

        #endregion

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


            this.BeginDropStatement(itemRef.Type);

            this.BeginIdentifier();
            if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.WriteObjectName(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }

            if (string.IsNullOrEmpty(itemRef.Schema) == false)
            {
                this.WriteObjectName(itemRef.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
                this.BeginIdentifier();
            }
            else if (!string.IsNullOrEmpty(itemRef.Catalog))
                this.AppendIdSeparator();

            this.WriteObjectName(itemRef.Name);
            this.EndIdentifier();
            this.EndDrop(itemRef.Type, false);
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

            this.BeginDropStatement(DBSchemaTypes.Index);
            if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
                
            }

            if (string.IsNullOrEmpty(itemRef.Schema) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(itemRef.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }
            else if (!string.IsNullOrEmpty(itemRef.Catalog))
                this.AppendIdSeparator();

            this.BeginIdentifier();
            this.WriteObjectName(itemRef.Name);
            this.EndIdentifier();
            this.WriteRawSQLString(" ON ");

            //write the table reference
            itemRef = itemRef.Container;
            this.BeginIdentifier();
            if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            if (string.IsNullOrEmpty(itemRef.Schema) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(itemRef.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }
            else if (!string.IsNullOrEmpty(itemRef.Catalog))
                this.AppendIdSeparator();

            this.BeginIdentifier();
            this.WriteObjectName(itemRef.Name);
            this.EndIdentifier();
            this.EndDrop(DBSchemaTypes.Index, false);
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


            this.BeginDropStatement(itemRef.Type);

            if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
                }

            if (string.IsNullOrEmpty(itemRef.Schema) == false)
            {
                
                this.BeginIdentifier();
                this.WriteObjectName(itemRef.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }
            else if (!string.IsNullOrEmpty(itemRef.Catalog))
                this.AppendIdSeparator();

            this.BeginIdentifier();
            this.WriteObjectName(itemRef.Name);
            this.EndIdentifier();
            this.EndDrop(itemRef.Type, false);
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


            this.BeginDropStatement(itemRef.Type);

           if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

           if (string.IsNullOrEmpty(itemRef.Schema) == false)
           {
               this.BeginIdentifier();
               this.WriteObjectName(itemRef.Schema);
               this.EndIdentifier();
               this.AppendIdSeparator();
           }
           else if (!string.IsNullOrEmpty(itemRef.Catalog))
               this.AppendIdSeparator();

            this.BeginIdentifier();
            this.WriteObjectName(itemRef.Name);
            this.EndIdentifier();
            this.EndDrop(itemRef.Type, false);
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


            this.BeginDropStatement(itemRef.Type);

            if (string.IsNullOrEmpty(itemRef.Catalog) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(itemRef.Catalog);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }

            if (string.IsNullOrEmpty(itemRef.Schema) == false)
            {
                this.BeginIdentifier();
                this.WriteObjectName(itemRef.Schema);
                this.EndIdentifier();
                this.AppendIdSeparator();
            }
            else if (!string.IsNullOrEmpty(itemRef.Catalog))
                this.AppendIdSeparator();

            this.BeginIdentifier();
            this.WriteObjectName(itemRef.Name);
            this.EndIdentifier();
            this.EndDrop(itemRef.Type, false);
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

        protected virtual string GetNativeTypeForDbType(System.Data.DbType dbType, int setSize, int accuracy, DBColumnFlags flags, out string options)
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
                    options = "";
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

        #region public virtual void GenerateCheckExists(DBSchemaTypes types, string owner, string name)

        /// <summary>
        /// Creates a check exists statement for a specific type with owner.name and the lookups for the table
        /// </summary>
        /// <param name="types"></param>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="info"></param>
        /// <param name="lookups"></param>
        public virtual void GenerateCheckExists(DBSchemaTypes types, string owner, string name)
        {
            DBSchemaInformation info = this.DatabaseProperties.SchemaInformation;
            DBSchemaInfoLookup lookups = info.GetListLookup(types);
            if (null == lookups)
                throw new ArgumentNullException("type", String.Format(Errors.DatabaseEngineDoesntSupportType, types));
            
            DBSelectQuery sel = DBQuery.Select(DBConst.Int32(1))
                                  .From(info.SchemaName, lookups.TableName);
            if (!string.IsNullOrEmpty(owner))
                sel = sel.WhereField(lookups.LookupSchemaColumn, Compare.Like, DBConst.String(owner))
                         .AndWhere(lookups.LookupNameColumn, Compare.Like, DBConst.String(name));
            else
                sel = sel.WhereField(lookups.LookupNameColumn, Compare.Like, DBConst.String(name));
            if (null != lookups.Restrictions)
                sel = sel.And(lookups.Restrictions);

            sel.IsInnerQuery = true;
            sel.BuildStatement(this);

        }

        #endregion

    }
}
