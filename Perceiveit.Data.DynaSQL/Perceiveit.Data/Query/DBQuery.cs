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

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// DBQuery.Select, Update, Delete, Insert, Exec and Begin all return specific DBQuery 
    /// instances that enable the building of SQL Statements to be executed on a Relational Database.
    /// </summary>
    public abstract partial class DBQuery : DBStatement, System.Xml.Serialization.IXmlSerializable
    {

        public const string XMLSCHEMA = "http://schemas.perceiveit.co.uk/Query";

        #region internal bool IsInnerQuery {get;set;}

        private bool _innerq = false;

        internal bool IsInnerQuery
        {
            get { return this._innerq; }
            set { this._innerq = value; }
        }

        #endregion

        //
        // DBSelectQuery factory methods
        //

        #region public static DBQuery SelectCount() + 3 overloads
        
        /// <summary>
        /// Begins a new SELECT statement with a COUNT(*) as the first result
        /// </summary>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery SelectCount()
        {
            DBSelectSet sel = DBSelectSet.SelectCount();
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = sel;
            q.Last = sel;
            return q;
        }

        /// <summary>
        /// Begins a new SELECT statement with a COUNT([field]) as the first result
        /// </summary>
        /// <param name="field">The name of the field to count</param>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery SelectCount(string field)
        {
            DBSelectSet sel = DBSelectSet.SelectCount(field);
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = sel;
            return q;
        }


        /// <summary>
        /// Begins a new SELECT statement with a COUNT([table].[field]) as the first result
        /// </summary>
        /// <param name="field">The name of the field to count</param>
        /// <param name="table">The name of the table (or alias) containing the field to count</param>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery SelectCount(string table, string field)
        {
            DBSelectSet sel = DBSelectSet.SelectCount(table, field);
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = sel;
            return q;
        }

        /// <summary>
        /// Begins a new SELECT statement with a COUNT([owner].[table].[field]) as the first result
        /// </summary>
        /// <param name="field">The name of the field to count</param>
        /// <param name="table">The name of the table (or alias) containing the field to count</param>
        /// <param name="owner">The schema owner of the table</param>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery SelectCount(string owner, string table, string field)
        {
            DBSelectSet sel = DBSelectSet.SelectCount(owner, table, field);
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = sel;
            return q;
        }



        #endregion

        #region public static DBQuery SelectFields(params string[] fields)

        /// <summary>
        /// Begins a new SELECT statement with a set of fields as the result
        /// </summary>
        /// <param name="fields">The name of the fields to select</param>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery SelectFields(params string[] fields)
        {
            DBSelectSet sel = DBSelectSet.SelectFields(fields);
            DBSelectQuery query = new DBSelectQuery();
            query.SelectSet = sel;
            query.Last = sel;

            return query;
        }

        #endregion

        #region public static DBSelectQuery Select()
        /// <summary>
        /// Begins a new SELECT statement. Add fields and tables by using the methods on the instance returned
        /// </summary>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery Select()
        {
            DBSelectQuery q = new DBSelectQuery();
            return q;
        }

        #endregion

        #region public static DBSelectQuery SelectTopN(int count)
        /// <summary>
        /// Begins a new SELECT statement with a limit of [count] items returned
        /// </summary>
        /// <param name="count">The total number of items to return</param>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery SelectTopN(int count)
        {
            DBSelectQuery q = DBQuery.Select();
            return q.TopN(count);
        }

        #endregion

        #region public static DBSelectQuery SelectDistinct()

        /// <summary>
        /// Begins a new SELECT DISTINCT statement. Add fields and tables by using the methods on the instance returned
        /// </summary>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery SelectDistinct()
        {
            DBSelectQuery q = DBQuery.Select();
            return q.Distinct();
        }

        #endregion

        #region public static DBSelectQuery Select(DBClause field) + 1 Overload
        /// <summary>
        /// Begins a new SELECT statement with the clause as the first value returned.
        /// </summary>
        /// <param name="clause">Any valid SQL clause (DBConst, DBParam, etc.) </param>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery Select(DBClause clause)
        {
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = DBSelectSet.Select(clause);
            q.Last = q.SelectSet;
            return q;
        }

        /// <summary>
        /// Begins a new SELECT statement with the clauses as the first values returned
        /// </summary>
        /// <param name="clauses">Any valid clause (DBConst, DBParam, etc)</param>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery Select(params DBClause[] clauses)
        {
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = DBSelectSet.Select();
            foreach (DBClause c in clauses)
            {
                q.SelectSet.And(c);
            }
            q.Last = q.SelectSet;
            return q;            
        }

        #endregion

        #region public static DBSelectQuery SelectAll()
        /// <summary>
        /// Begins a new SELECT * statement.
        /// </summary>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery SelectAll()
        {
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = DBSelectSet.SelectAll();
            q.Last = q.SelectSet;
            return q;
        }

        #endregion

        #region public static DBSelectQuery SelectAll(string table)
        /// <summary>
        /// Begins a new SELECT [table].* statment
        /// </summary>
        /// <param name="table">The name (or alias) of the table</param>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery SelectAll(string table)
        {
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = DBSelectSet.SelectAll(table);
            q.Last = q.SelectSet;
            return q;
        }

        #endregion

        #region public static DBSelectQuery SelectAll(string owner, string table)

        /// <summary>
        /// Begins a new SELECT [owner].[table].* statment
        /// </summary>
        /// <param name="table">The name (or alias) of the table</param>
        /// <param name="owner">The schema owner of the table</param>
        /// <returns>A new DBSelectQuery to support statement chaining</returns>
        public static DBSelectQuery SelectAll(string owner, string table)
        {
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = DBSelectSet.SelectAll(owner, table);
            q.Last = q.SelectSet;
            return q;
        }

        #endregion


        //
        // DBExecQuery factory methods
        //

        #region public static DBExecQuery Exec(string name) + 1 overload

        /// <summary>
        /// Begins a new 'EXEC [procedure name]' statement
        /// </summary>
        /// <param name="name">The name of the procedure</param>
        /// <returns>A new DBExecQuery to support statement chaining</returns>
        public static DBExecQuery Exec(string name)
        {
            DBExecQuery exec = new DBExecQuery();
            exec.SprocName = name;
            return exec;
        }

        public static DBExecQuery Exec(string owner, string name)
        {
            DBExecQuery exec = new DBExecQuery();
            exec.SprocName = name;
            exec.Owner = owner;
            return exec;
        }

        #endregion


        //
        // DBUpdateQuery factory methods
        //

        #region public static DBUpdateQuery Update() + 3 overloads

        /// <summary>
        /// Begins a new UPDATE statement
        /// </summary>
        /// <returns>A new DBUpdateQuery to support statement chaining</returns>
        public static DBUpdateQuery Update()
        {
            return new DBUpdateQuery();
        }

        /// <summary>
        /// Begins a new UPDATE [table] statement
        /// </summary>
        /// <param name="table">The name of the table to update the rows on</param>
        /// <returns>A new DBUpdateQuery to support statement chaining</returns>
        public static DBUpdateQuery Update(string table)
        {
            DBUpdateQuery upd = Update();
            upd.TableSet = DBTableSet.From(table);
            upd.Last = upd.TableSet;
            return upd;
        }

        /// <summary>
        /// Begins a new UPDATE [table] statement
        /// </summary>
        /// <param name="table">The name of the table to update the rows on</param>
        /// <param name="owner">The schema owner of the table</param>
        /// <returns>A new DBUpdateQuery to support statement chaining</returns>
        public static DBUpdateQuery Update(string owner, string table)
        {
            DBUpdateQuery upd = Update();
            upd.TableSet = DBTableSet.From(owner, table);
            upd.Last = upd.TableSet;
            return upd;
        }

        /// <summary>
        /// Begins a new UPDATE [source] statement
        /// </summary>
        /// <param name="source">The table to update the rows on</param>
        /// <returns>A new DBUpdateQuery to support statement chaining</returns>
        public static DBUpdateQuery Update(DBTable source)
        {
            DBUpdateQuery upd = DBUpdateQuery.Update();
            upd.TableSet = DBTableSet.From(source);
            upd.Last = upd.TableSet;
            return upd;
        }

        #endregion


        //
        // DBInsertQuery factory methods
        //

        #region public static DBInsertQuery InsertInto(string intoTable) + 3 overloads

        /// <summary>
        /// Begins a new INSERT INTO [table] statement
        /// </summary>
        /// <param name="intoTable">The table to insert the rows on</param>
        /// <returns>A new DBInsertQuery to support statement chaining</returns>
        public static DBInsertQuery InsertInto(string intoTable)
        {
            DBTable ts = DBTable.Table(intoTable);
            DBInsertQuery q = new DBInsertQuery();
            q.Into = ts;
            q.Last = ts;
            return q;
        }

        /// <summary>
        /// Begins a new INSERT INTO [owner].[table] statement
        /// </summary>
        /// <param name="table">The table to insert the rows on</param>
        /// <param name="owner">The schema owner of the table</param>
        /// <returns>A new DBInsertQuery to support statement chaining</returns>
        public static DBInsertQuery InsertInto(string owner, string table)
        {
            DBTable ts = DBTable.Table(owner, table);
            DBInsertQuery q = new DBInsertQuery();
            q.Into = ts;
            q.Last = ts;
            return q;
        }


        /// <summary>
        /// Begins a new INSERT INTO [table] statement
        /// </summary>
        /// <param name="tbl">The table to insert the rows on</param>
        /// <returns>A new DBInsertQuery to support statement chaining</returns>
        public static DBInsertQuery InsertInto(DBTable tbl)
        {
            DBInsertQuery q = new DBInsertQuery();
            q.Into = tbl;
            q.Last = tbl;
            return q;
        }

        /// <summary>
        /// Begins a new INSERT INTO statement
        /// </summary>
        /// <returns>A new DBInsertQuery to support statement chaining</returns>
        internal static DBInsertQuery InsertInto()
        {
            DBInsertQuery q = new DBInsertQuery();
            return q;
        }

        #endregion


        //
        // DBDeleteQuery factory methods
        //

        #region public static DBDeleteQuery DeleteFrom(string intoTable) + 2 overloads

        /// <summary>
        /// begins a new DELETE FROM [table] statement
        /// </summary>
        /// <param name="table">The name of the table to delete from</param>
        /// <returns>A new DBDeleteQuery to support statement chaining</returns>
        public static DBDeleteQuery DeleteFrom(string table)
        {
            DBTable ts = DBTable.Table(table);
            DBDeleteQuery q = new DBDeleteQuery();
            q.FromTable = ts;
            q.Last = ts;
            return q;
        }

        /// <summary>
        /// begins a new DELETE FROM [owner].[table] statement
        /// </summary>
        /// <param name="table">The name of the table to delete from</param>
        /// <param name="owner">The schema owner of the table</param>
        /// <returns>A new DBDeleteQuery to support statement chaining</returns>
        public static DBDeleteQuery DeleteFrom(string owner, string table)
        {
            DBTable ts = DBTable.Table(owner, table);
            DBDeleteQuery q = new DBDeleteQuery();
            q.FromTable = ts;
            q.Last = ts;
            return q;
        }


        /// <summary>
        /// begins a new DELETE FROM [table] statement
        /// </summary>
        /// <param name="table">The table to delete from</param>
        /// <returns>A new DBDeleteQuery to support statement chaining</returns>
        public static DBDeleteQuery DeleteFrom(DBTable table)
        {
            DBDeleteQuery q = new DBDeleteQuery();
            q.FromTable = table;
            q.Last = table;
            return q;
        }

        #endregion

        //
        // DBScriptQuery factory methods
        //

        #region public static DBScript Begin() + 2 overloads

        /// <summary>
        /// Starts a new script where multiple statements can be executed within the same command
        /// </summary>
        /// <returns>A new DBScript for statement chaining</returns>
        public static DBScript Begin()
        {
            DBScript s = new DBScriptRef();
            return s;
        }

        /// <summary>
        /// Starts a new script where multiple statements can be executed within the same command
        /// and sets the first statement to the specified query
        /// </summary>
        /// <param name="query">The first statement in the script</param>
        /// <returns>A new DBScript for statement chaining</returns>
        public static DBScript Begin(DBStatement query)
        {
            DBScript s = Begin();
            s.Then(query);
            return s;
        }

        /// <summary>
        /// Starts a new script where the provided multiple statements can be executed within the same command
        /// </summary>
        /// <param name="querys">The statements in the script</param>
        /// <returns>A new DBScript for statement chaining</returns>
        public static DBScript Begin(params DBStatement[] querys)
        {
            DBScript s = Begin();

            if (null != querys && querys.Length > 0)
            {
                foreach (DBQuery q in querys)
                {
                    s.Then(q);
                }
            }

            return s;
        }

        #endregion

        #region public static DBScript Script(params DBQuery[] statements)

        /// <summary>
        /// Starts a new script where the provided multiple statements can be executed within the same command
        /// </summary>
        /// <param name="statements">The statements in the script</param>
        /// <returns>A new DBScript for statement chaining</returns>
        public static DBScript Script(params DBStatement[] statements)
        {
            DBScript s = Begin();

            if (statements != null && statements.Length > 0)
                s.Inner.AddRange(statements);

            return s;
        }

        #endregion

        #region public static DBDeclaration Declare(DBParam param)

        /// <summary>
        /// Creates a declaration statement e.g. DECLARE @p1 INT;
        /// </summary>
        /// <param name="param">The parameter to declare</param>
        /// <returns>The declaration</returns>
        public static DBDeclaration Declare(DBParam param)
        {
            return DBDeclaration.Declare(param);
        }

        #endregion

        #if SILVERLIGHT
        // no statement building
#else
        //
        // Build statement methods
        //

        #region protected internal virtual System.Data.CommandType GetCommandType()
        /// <summary>
        /// Gets the command type for this query (default is Text).
        /// Inheritors can override to return their own type of command
        /// </summary>
        /// <returns></returns>
        protected internal virtual System.Data.CommandType GetCommandType()
        {
            return System.Data.CommandType.Text;
        }

        #endregion

#endif

        //
        // writeXml methods
        //

        #region public void WriteXml(System.Xml.XmlWriter writer)
        /// <summary>
        /// Outputs this query as an xml stream onto the specifed writer
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            this.WriteXml(writer, "http://schemas.perceiveit.co.uk/Query", "");
        }

        #endregion

        #region public void WriteXml(System.Xml.XmlWriter writer, string ns, string prefix)

        /// <summary>
        /// Outputs this query as an xml stream onto the specified writer with the specified namespace and prefix
        /// </summary>
        /// <param name="writer">The XmlWriter to output to</param>
        /// <param name="ns">The xml namespace to use</param>
        /// <param name="prefix">The specifed prefix</param>
        public void WriteXml(System.Xml.XmlWriter writer, string ns, string prefix)
        {
            XmlWriterContext context = new XmlWriterContext(ns, prefix);
            writer.WriteStartElement(XmlHelper.Statement, ns);
            this.WriteXml(writer, context);
            if (this.IsInnerQuery == false && context.Parameters.Count > 0)
            {
                writer.WriteStartElement(XmlHelper.Parameters);
                foreach (DBParam p in context.Parameters)
                {
                    p.WriteFullParameterXml(writer, context);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();//statement
        }

        #endregion


        //
        // readXml methods
        //

        #region public static DBQuery ReadXml(System.Xml.XmlReader reader)
        /// <summary>
        /// Inputs the XmlReader and attempts to parse as a valid DbQuery
        /// </summary>
        /// <param name="reader">The reader to load the DbQuery from</param>
        /// <returns>The parsed DbQuery</returns>
        public static DBQuery ReadXml(System.Xml.XmlReader reader)
        {
            return DBQuery.ReadXml(reader, "http://schemas.perceiveit.co.uk/Query", "");
        }

        #endregion

        #region public static DBQuery ReadXml(System.Xml.XmlReader reader, string ns, string pref)

        /// <summary>
        /// Inputs the XmlReader and attempts to parse as a valid DbQuery with the specified element namespace and prefix
        /// </summary>
        /// <param name="reader">The reader to load the DbQuery from</param>
        /// <param name="ns">The namespace of the DBQuery elements</param>
        /// <param name="prefix">The prefix</param>
        /// <returns>The parsed DbQuery</returns>
        public static DBQuery ReadXml(System.Xml.XmlReader reader, string ns, string prefix)
        {
            XmlReaderContext context = new XmlReaderContext(ns, prefix);
            return DBQuery.sReadXml(reader, context);
        }

        #endregion

        #region public static DBQuery ReadXml(System.Xml.XmlReader reader, string ns, string pref, XmlFactory factory)

        /// <summary>
        /// Inputs the XmlReader and attempts to parse as a valid DbQuery with the 
        /// specified element namespace and prefix
        /// </summary>
        /// <param name="reader">The reader to load the DbQuery from</param>
        /// <param name="ns">The namespace of the DBQuery elements</param>
        /// <param name="prefix">The prefix</param>
        /// <param name="factory">An XmlFactory that creates new DBClause instances from their known names</param>
        /// <returns>The parsed DbQuery</returns>
        public static DBQuery ReadXml(System.Xml.XmlReader reader, string ns, string prefix, XmlFactory factory)
        {
            XmlReaderContext context = new XmlReaderContext(ns, prefix, factory);
            return DBQuery.sReadXml(reader, context);
        }

        #endregion

        #region private static DBQuery sReadXml(System.Xml.XmlReader reader, XmlReaderContext context)
        /// <summary>
        /// Read implementation
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static DBQuery sReadXml(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            DBQuery q = null;
            DBClause c = null;

            while (reader.EOF == false && reader.NodeType != System.Xml.XmlNodeType.Element)
            {
                if (!reader.Read())
                    break;
            }

            if (reader.NodeType == System.Xml.XmlNodeType.Element && XmlHelper.IsElementMatch(XmlHelper.Statement, reader, context))
            {
                while(reader.Read())
                    if(reader.NodeType == System.Xml.XmlNodeType.Element)
                        break;

                if (!reader.EOF)
                {
                    c = context.Factory.Read(reader.LocalName, reader, context);
                    if (c is DBQuery)
                        q = (DBQuery)c;
                    else
                        throw XmlHelper.CreateException("The Xml stream did not begin with a valid DBQuery node, and cannot be deserialized", reader, null);
                }
                else
                    throw XmlHelper.CreateException("The Xml stream did not begin with a valid DBQuery node, and cannot be deserialized", reader, null);
            }
            if (!reader.EOF)
            {
                while (reader.Read())
                {
                    if (reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name == XmlHelper.Parameters)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == System.Xml.XmlNodeType.Element && XmlHelper.IsElementMatch(XmlHelper.Parameter, reader, context))
                            {
                                string name = reader.GetAttribute(XmlHelper.Name);
                                DBParam param;
                                if (context.Parameters.TryGetParameter(name, out param))
                                    param.ReadXml(reader, context);
                            }
                        }
                    }
                }
            }

            return q;
        }

        #endregion



        #region IXmlSerializable Members

        System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
        {
            throw new NotSupportedException("IXmlSerializable.GetSchema");
        }

        void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
             XmlReaderContext context = new XmlReaderContext(XMLSCHEMA, string.Empty);
             this.ReadXml(reader, context);
        }

        #endregion
    }
}
