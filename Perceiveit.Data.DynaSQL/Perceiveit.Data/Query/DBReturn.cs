﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Defines a Return ... statement
    /// </summary>
    public abstract class DBReturn : DBStatement
    {
        private DBClause _toreturn;
        /// <summary>
        /// Get the clause that will be returned by the database engine when this statement is executed. 
        /// Inheritors can set the value
        /// </summary>
        public DBClause ToReturn
        {
            get { return _toreturn; }
            protected set { _toreturn = value; }
        }

        /// <summary>
        /// Creates and returns a new RETURN empty statement
        /// </summary>
        /// <returns></returns>
        public static DBReturn Return()
        {
            return new DBReturnRef();
        }

        /// <summary>
        /// Creates and returns a new RETURN xxx statement
        /// </summary>
        /// <param name="toreturn">The clause that will be executed by the database and its resultant value returned</param>
        /// <returns></returns>
        public static DBReturn Return(DBClause toreturn)
        {
            DBReturnRef ret = new DBReturnRef();
            ret.ToReturn = toreturn;
            return ret;
        }
    }

    internal class DBReturnRef : DBReturn
    {
        protected override string XmlElementName
        {
            get { return XmlHelper.Returns; }
        }

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginReturnsStatement();
            if (null != this.ToReturn)
                this.ToReturn.BuildStatement(builder);
            builder.EndReturnsStatement();
            return true;
        }

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (null != this.ToReturn)
            {
                this.WriteStartElement("to-return", writer, context);
                this.ToReturn.WriteXml(writer, context);
                this.WriteEndElement("to-return", writer, context);
            }
            return base.WriteInnerElements(writer, context);
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (this.IsElementMatch("to-return", reader, context))
            {
                DBClause ret = this.ReadNextInnerClause("to-return", reader, context);
                this.ToReturn = ret;
                return true;
            }
            else
                return base.ReadAnInnerElement(reader, context);
        }

    }
}