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

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// DBQuery.Select, Update, Delete, Insert, Exec and Begin all return specific DBQuery 
    /// instances that enable the building of SQL Statements to be executed on a Relational Database.
    /// </summary>
    public abstract class DBQuery : DBStatement, System.Xml.Serialization.IXmlSerializable
    {

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

        public static DBSelectQuery SelectCount()
        {
            DBSelectSet sel = DBSelectSet.SelectCount();
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = sel;
            q.Last = sel;
            return q;
        }

        public static DBSelectQuery SelectCount(string field)
        {
            DBSelectSet sel = DBSelectSet.SelectCount(field);
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = sel;
            return q;
        }

        public static DBSelectQuery SelectCount(string table, string field)
        {
            DBSelectSet sel = DBSelectSet.SelectCount(table, field);
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = sel;
            return q;
        }

        public static DBSelectQuery SelectCount(string owner, string table, string field)
        {
            DBSelectSet sel = DBSelectSet.SelectCount(owner, table, field);
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = sel;
            return q;
        }



        #endregion

        #region public static DBQuery SelectFields(params string[] fields)

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

        public static DBSelectQuery Select()
        {
            DBSelectQuery q = new DBSelectQuery();
            return q;
        }

        #endregion

        #region public static DBSelectQuery SelectTopN(int count)

        public static DBSelectQuery SelectTopN(int count)
        {
            DBSelectQuery q = DBQuery.Select();
            return q.TopN(count);
        }

        #endregion

        #region public static DBSelectQuery SelectTopPercent(double percent)

        public static DBSelectQuery SelectTopPercent(double percent)
        {
            DBSelectQuery q = DBQuery.Select();
            return q.TopPercent(percent);
        }

        #endregion

        #region public static DBSelectQuery SelectDistinct()

        public static DBSelectQuery SelectDistinct()
        {
            DBSelectQuery q = DBQuery.Select();
            return q.Distinct();
        }

        #endregion

        #region public static DBSelectQuery Select(DBClause fields)

        public static DBSelectQuery Select(DBClause field)
        {
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = DBSelectSet.Select(field);
            q.Last = q.SelectSet;
            return q;
        }

        #endregion

        #region public static DBSelectQuery SelectAll()

        public static DBSelectQuery SelectAll()
        {
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = DBSelectSet.SelectAll();
            q.Last = q.SelectSet;
            return q;
        }

        #endregion

        #region public static DBSelectQuery SelectAll(string table)

        public static DBSelectQuery SelectAll(string table)
        {
            DBSelectQuery q = new DBSelectQuery();
            q.SelectSet = DBSelectSet.SelectAll(table);
            q.Last = q.SelectSet;
            return q;
        }

        #endregion

        #region public static DBSelectQuery SelectAll(string owner, string table)

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

        #region public static DBExecQuery Exec(string name)

        public static DBExecQuery Exec(string name)
        {
            DBExecQuery exec = new DBExecQuery();
            exec.SprocName = name;
            return exec;
        }

        #endregion


        //
        // DBUpdateQuery factory methods
        //

        #region public static DBUpdateQuery Update() + 3 overloads

        public static DBUpdateQuery Update()
        {
            return new DBUpdateQuery();
        }

        public static DBUpdateQuery Update(string table)
        {
            DBUpdateQuery upd = Update();
            upd.TableSet = DBTableSet.From(table);
            upd.Last = upd.TableSet;
            return upd;
        }

        public static DBUpdateQuery Update(string owner, string table)
        {
            DBUpdateQuery upd = Update();
            upd.TableSet = DBTableSet.From(owner, table);
            upd.Last = upd.TableSet;
            return upd;
        }

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

        public static DBInsertQuery InsertInto(string intoTable)
        {
            DBTable ts = DBTable.Table(intoTable);
            DBInsertQuery q = new DBInsertQuery();
            q.Into = ts;
            q.Last = ts;
            return q;
        }

        public static DBInsertQuery InsertInto(string owner, string table)
        {
            DBTable ts = DBTable.Table(owner, table);
            DBInsertQuery q = new DBInsertQuery();
            q.Into = ts;
            q.Last = ts;
            return q;
        }


        public static DBInsertQuery InsertInto(DBTable tbl)
        {
            DBInsertQuery q = new DBInsertQuery();
            q.Into = tbl;
            q.Last = tbl;
            return q;
        }

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

        public static DBDeleteQuery DeleteFrom(string intoTable)
        {
            DBTable ts = DBTable.Table(intoTable);
            DBDeleteQuery q = new DBDeleteQuery();
            q.FromTable = ts;
            q.Last = ts;
            return q;
        }

        public static DBDeleteQuery DeleteFrom(string owner, string table)
        {
            DBTable ts = DBTable.Table(owner, table);
            DBDeleteQuery q = new DBDeleteQuery();
            q.FromTable = ts;
            q.Last = ts;
            return q;
        }


        public static DBDeleteQuery DeleteFrom(DBTable tbl)
        {
            DBDeleteQuery q = new DBDeleteQuery();
            q.FromTable = tbl;
            q.Last = tbl;
            return q;
        }

        #endregion

        //
        // DBScriptQuery factory methods
        //

        #region public static DBScript Begin() + 2 overloads

        public static DBScript Begin()
        {
            DBScript s = new DBScript();
            return s;
        }

        public static DBScript Begin(DBQuery query)
        {
            DBScript s = Begin();
            s.Then(query);
            return s;
        }

        public static DBScript Begin(params DBQuery[] querys)
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

        public static DBScript Script(params DBStatement[] statements)
        {
            DBScript s = Begin();

            if (statements != null && statements.Length > 0)
                s.Inner.AddRange(statements);

            return s;
        }

        #endregion


        //
        // Build statement methods
        //

        #region protected internal virtual System.Data.CommandType GetCommandType()

        protected internal virtual System.Data.CommandType GetCommandType()
        {
            return System.Data.CommandType.Text;
        }

        #endregion


        //
        // writeXml methods
        //

        #region public void WriteXml(System.Xml.XmlWriter writer)

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            this.WriteXml(writer, "http://schemas.perceiveit.co.uk/Query", "");
        }

        #endregion

        #region public void WriteXml(System.Xml.XmlWriter writer, string ns, string prefix)

        public void WriteXml(System.Xml.XmlWriter writer, string ns, string prefix)
        {
            XmlWriterContext context = new XmlWriterContext(ns, prefix);
            writer.WriteStartElement(XmlHelper.Statement, ns);
            this.WriteXml(writer, context);
            if (context.Parameters.Count > 0)
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

        public static DBQuery ReadXml(System.Xml.XmlReader reader)
        {
            return DBQuery.ReadXml(reader, "http://schemas.perceiveit.co.uk/Query", "");
        }

        #endregion

        #region public static DBQuery ReadXml(System.Xml.XmlReader reader, string ns, string pref)

        public static DBQuery ReadXml(System.Xml.XmlReader reader, string ns, string pref)
        {
            XmlReaderContext context = new XmlReaderContext(ns, pref);
            return DBQuery.sReadXml(reader, context);
        }

        #endregion

        #region public static DBQuery ReadXml(System.Xml.XmlReader reader, string ns, string pref, XmlFactory factory)

        public static DBQuery ReadXml(System.Xml.XmlReader reader, string ns, string pref, XmlFactory factory)
        {
            XmlReaderContext context = new XmlReaderContext(ns, pref, factory);
            return DBQuery.sReadXml(reader, context);
        }

        #endregion

        #region private static DBQuery sReadXml(System.Xml.XmlReader reader, XmlReaderContext context)

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
             XmlReaderContext context = new XmlReaderContext("http://schemas.perceiveit.co.uk/Query", string.Empty);
             this.ReadXml(reader, context);
        }

        #endregion
    }
}
