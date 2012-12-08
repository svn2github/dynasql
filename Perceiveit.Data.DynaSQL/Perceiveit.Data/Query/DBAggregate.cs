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
    /// Defines an aggregate function to be generated for a clause
    /// </summary>
    public abstract class DBAggregate : DBCalculableClause
    {
        //
        // public properties
        //

        #region public AggregateFunction Function {get;set;}

        private AggregateFunction _func;
        /// <summary>
        /// Gets or sets the aggregate function for this instance
        /// </summary>
        public AggregateFunction Function
        {
            get { return _func; }
            set { _func = value; }
        }

        #endregion

        #region public DBClause InnerReference {get;set;}

        private DBClause _innerref;
        /// <summary>
        /// Gets or sets the inner clause for the aggregation
        /// </summary>
        public DBClause InnerReference
        {
            get { return _innerref; }
            set { _innerref = value; }
        }

        #endregion

        #region public string Alias {get;set;}

        private string _alias;

        /// <summary>
        /// Gets the alias name for the resultant clause
        /// </summary>
        public string Alias
        {
            get { return _alias; }
            protected set { _alias = value; }
        }

        #endregion


        //
        // static factory methods
        //

        #region public static DBAggregate Count(DBClause reference)
        /// <summary>
        /// Creates a new Count aggregation for this passed clause.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static DBAggregate Count(DBClause reference)
        {
            return Aggregate(AggregateFunction.Count, reference);
        }

        #endregion

        #region public static DBAggregate CountAll()

        /// <summary>
        /// Creates a new Count(*) aggregation
        /// </summary>
        /// <returns></returns>
        public static DBAggregate CountAll()
        {
            
            return Aggregate(AggregateFunction.Count, DBField.AllFields());
        }

        #endregion

        #region public static DBAggregate Count(string field)
        /// <summary>
        /// Creates a new Count('field') aggregation
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static DBAggregate Count(string field)
        {
            return Aggregate(AggregateFunction.Count, DBField.Field(field));
        }

        #endregion

        #region public static DBAggregate Aggregate(AggregateFunction func, DBClause reference)
        /// <summary>
        /// Creates a new Aggregation for the provided clause.
        /// </summary>
        /// <param name="func"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static DBAggregate Aggregate(AggregateFunction func, DBClause reference)
        {
            if (null == reference)
                throw new ArgumentNullException("reference");

            DBAggregateRef cref = new DBAggregateRef();
            cref.Function = func;
            cref.InnerReference = reference;
            return cref;
        }

        #endregion

        #region internal static DBAggregate Aggregate()

        internal static DBAggregate Aggregate()
        {
            return new DBAggregateRef();
        }

        #endregion

    }

    internal class DBAggregateRef : DBAggregate, IDBAlias
    {

#if SILVERLIGHT
        // no statement building
#else
        //
        // SQL Statement building
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)
        
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginAggregateFunction(this.Function,this.Function.ToString());
            builder.BeginFunctionParameterList();

            if(this.InnerReference != null)
                this.InnerReference.BuildStatement(builder);

            builder.EndFunctionParameterList();
            builder.EndAggregateFunction(this.Function, this.Function.ToString());

            if (string.IsNullOrEmpty(this.Alias) == false)
            {
                builder.WriteAlias(this.Alias);
            }
            return true;
        }

        #endregion

#endif

        //
        // XML Serialization
        //

        #region protected override string XmlElementName {get;}

        protected override string XmlElementName
        {
            get { return XmlHelper.Aggregate; }
        }

        #endregion

        #region protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.FunctionName, this.Function.ToString(), context);

            if (string.IsNullOrEmpty(this.Alias) == false)
                this.WriteAttribute(writer, XmlHelper.Alias, this.Alias, context);

            return base.WriteAllAttributes(writer, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        
        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.InnerReference != null)
                this.InnerReference.WriteXml(writer, context);
            return true;
        }

        #endregion

        #region protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            bool b;
            if (this.IsAttributeMatch(XmlHelper.FunctionName, reader, context))
            {
                this.Function = XmlHelper.ParseEnum <AggregateFunction>(reader.Value);
                b = true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Alias, reader, context))
            {
                this.Alias = reader.Value;
                b = true;
            }
            else
                b = base.ReadAnAttribute(reader, context);

            return b;
        }

        #endregion

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        
        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            this.InnerReference = this.ReadNextInnerClause(reader.Name, reader, context);
            return true;
        }

        #endregion
        
        //
        // Interface Implementations
        //

        #region void IDBAlias.As(string alias)

        void IDBAlias.As(string alias)
        {
            this.Alias = alias;
        }

        #endregion



    }
}
