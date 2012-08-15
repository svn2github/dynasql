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
using System.Linq;
using System.Text;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Represents a query to create a sequence for generating numbers. Only supported on Oracle
    /// </summary>
    public abstract class DBCreateSequenceQuery : DBCreateQuery
    {
        protected const int DEF_MIN = -1;
        protected const int DEF_MAX = -1;
        protected const int DEF_START = 1;
        protected const int DEF_INCREMENT = 1;
        protected const int DEF_CACHE = -1;
        protected const DBSequenceOrdering DEF_ORDERED = DBSequenceOrdering.None;
        protected const DBSequenceCycling DEF_CYCLE = DBSequenceCycling.None;
        protected const int NO_CACHE_VALUE = 0;

        protected const int NO_MAX_VALUE = int.MaxValue;



        #region internal string SequenceName { get; set; }

        /// <summary>
        /// Gets or sets the name of the sequence this query should create
        /// </summary>
        internal string SequenceName { get; set; }

        #endregion

        #region internal string Owner { get; set; }

        /// <summary>
        /// Gets or sets the name of the sequence owner
        /// </summary>
        internal string Owner { get; set; }

        #endregion

        //
        // Sequence properties
        //


        internal int MinValue { get; set; }

        internal int MaxValue { get; set; }

        internal int StartWithValue { get; set; }

        internal int IncrementValue { get; set; }

        internal int CacheSize { get; set; }

        internal DBSequenceOrdering Order { get; set; }

        internal DBSequenceCycling Cycling { get; set; }

        //
        // ctors
        //

        #region protected DBCreateSequenceQuery(string owner, string name)

        /// <summary>
        /// Protected constructor - use the static factory methods to instantiate
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        protected DBCreateSequenceQuery(string owner, string name)
        {
            this.MinValue = DEF_MIN;
            this.MaxValue = DEF_MAX;
            this.StartWithValue = DEF_START;
            this.IncrementValue = DEF_INCREMENT;
            this.Order = DEF_ORDERED;
            this.CacheSize = DEF_CACHE;
            this.Cycling = DEF_CYCLE;

            this.Owner = owner;
            this.SequenceName = name;
        }

        #endregion


        //
        // public methods
        //

        #region public DBCreateSequenceQuery Maximum(int maxvalue)

        /// <summary>
        /// Specifies the maximum value of the sequence. Optional and if not specified default will be 999999999999999999999999999999 (or the engine's default)
        /// </summary>
        /// <param name="maxvalue"></param>
        /// <returns></returns>
        public DBCreateSequenceQuery Maximum(int maxvalue)
        {
            this.MaxValue = maxvalue;
            return this;
        }

        #endregion

        #region public DBCreateSequenceQuery Minimum(int minvalue)

        /// <summary>
        /// Specifies the minimum value of the sequence. Optional and if not specified default will be 1
        /// </summary>
        /// <param name="minvalue"></param>
        /// <returns></returns>
        public DBCreateSequenceQuery Minimum(int minvalue)
        {
            this.MinValue = minvalue;
            return this;
        }

        #endregion

        #region public DBCreateSequenceQuery StartWith(int start)

        /// <summary>
        /// Specifies the first value in the sequence. Optional and if not specified default will be 1 for ascending 
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public DBCreateSequenceQuery StartWith(int start)
        {
            this.StartWithValue = start;
            return this;
        }

        #endregion

        #region public DBCreateSequenceQuery Increment() + IncrementBy(int)

        /// <summary>
        /// Specifies that a sequence should increment by 1 for each value
        /// </summary>
        /// <returns></returns>
        public DBCreateSequenceQuery Increment()
        {
            return this.IncrementBy(DEF_INCREMENT);
        }

        /// <summary>
        /// Specifies the amount to increment (or decrement) each sequence value by.
        /// </summary>
        /// <param name="increment">The amount (use negative for decrementing)</param>
        /// <returns></returns>
        public DBCreateSequenceQuery IncrementBy(int increment)
        {
            this.IncrementValue = increment;
            return this;
        }

        #endregion

        #region public DBCreateSequenceQuery Decrement() + DecrementBy(int)

        /// <summary>
        /// Specifes that a sequence should decrement by 1 for each value
        /// </summary>
        /// <returns></returns>
        public DBCreateSequenceQuery Decrement()
        {
            return this.DecrementBy(1);
        }

        /// <summary>
        /// Specifies the amount to decrement the sequence by each time
        /// </summary>
        /// <param name="decrement"></param>
        /// <returns></returns>
        public DBCreateSequenceQuery DecrementBy(int decrement)
        {
            return this.IncrementBy(-decrement);
        }

        #endregion

        #region public DBCreateSequenceQuery Cache(int size) + NoCache()

        /// <summary>
        /// Specifes the number of cached values to allocate and hold in memory. Optional and if not specified uses the engines default (usually 20)
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public DBCreateSequenceQuery Cache(int size)
        {
            this.CacheSize = size;
            return this;
        }


        /// <summary>
        /// Specifies that sequence numbers should not be cached, and should be looked up each time
        /// </summary>
        /// <returns></returns>
        public DBCreateSequenceQuery NoCache()
        {
            return this.Cache(NO_CACHE_VALUE);
        }

        #endregion

        #region public DBCreateSequenceQuery Ordered() + NotOrdered()

        /// <summary>
        /// Specifies that this sequence should be ordered - values will be assigned in order. Optional
        /// </summary>
        /// <returns></returns>
        public DBCreateSequenceQuery Ordered()
        {
            this.Order = DBSequenceOrdering.Ordered;
            return this;
        }

        /// <summary>
        /// Specifies that the order of numbers allocated does not matter - values will be assigned in any order of the cache
        /// </summary>
        /// <returns></returns>
        public DBCreateSequenceQuery NotOrdered()
        {
            this.Order = DBSequenceOrdering.NotOrdered;
            return this;
        }

        #endregion

        #region public DBCreateSequenceQuery Cycle() + NoCycle()

        /// <summary>
        /// Specifies that this sequence should cylce the values - restart numbering when it reaches the end. Optional, and defaults to the engines choice.
        /// </summary>
        /// <returns></returns>
        public DBCreateSequenceQuery Cycle()
        {
            this.Cycling = DBSequenceCycling.Cycle;
            return this;
        }

        /// <summary>
        /// Specifes that this sequence should NOT cycle the values - error when the max (or min) value is exceeded. Optional, and defaults to the engines choice.
        /// </summary>
        /// <returns></returns>
        public DBCreateSequenceQuery NoCycle()
        {
            this.Cycling = DBSequenceCycling.NoCycle;
            return this;
        }

        #endregion

        #region public DBCreateSequenceQuery IfNotExists()

        /// <summary>
        /// Checks the database schema before creation to ensure that it is only created if it does not already exist
        /// </summary>
        /// <returns></returns>
        public DBCreateSequenceQuery IfNotExists()
        {
            this.CheckExists = DBExistState.NotExists;
            return this;
        }

        #endregion

        //
        // static factory methods
        //

        #region public static DBCreateSequenceQuery Sequence(string name) + 2 overloads

        /// <summary>
        /// Creates a new Sequence reference
        /// </summary>
        /// <returns></returns>
        public static DBCreateSequenceQuery Sequence()
        {
            return Sequence(string.Empty, string.Empty);
        }

        /// <summary>
        /// Instantiates a new Create Sequence reference with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBCreateSequenceQuery Sequence(string name)
        {
            return Sequence(string.Empty, name);
        }

        /// <summary>
        ///  Instantiates a new Create Sequence reference with the specified name and owner
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DBCreateSequenceQuery Sequence(string owner, string name)
        {
            DBCreateSequenceQueryRef seq = new DBCreateSequenceQueryRef(owner, name);
            return seq;
        }

        #endregion

    }


    internal class DBCreateSequenceQueryRef : DBCreateSequenceQuery
    {
        protected override string XmlElementName
        {
            get { return XmlHelper.CreateSequence; }
        }

        public DBCreateSequenceQueryRef(string owner, string name)
            : base(owner, name)
        {
        }

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginCreate(DBSchemaTypes.Sequence, this.Owner, this.SequenceName, string.Empty, this.CheckExists == DBExistState.NotExists);

            if (this.MinValue != DEF_MIN)
                builder.WriteSequenceOption(DBSequenceBuilderOption.Minimum, this.MinValue);

            if (this.MaxValue != DEF_MAX)
                builder.WriteSequenceOption(DBSequenceBuilderOption.Maximim, this.MaxValue);

            if (this.StartWithValue != DEF_START)
                builder.WriteSequenceOption(DBSequenceBuilderOption.StartValue, this.StartWithValue);

            if (this.IncrementValue != DEF_INCREMENT)
                builder.WriteSequenceOption(DBSequenceBuilderOption.Increment, this.IncrementValue);

            if (this.CacheSize == NO_CACHE_VALUE)
                builder.WriteSequenceOption(DBSequenceBuilderOption.NoCaching);
            else if (this.CacheSize != DEF_CACHE)
                builder.WriteSequenceOption(DBSequenceBuilderOption.Cache, this.CacheSize);

            if (this.Cycling == DBSequenceCycling.Cycle)
                builder.WriteSequenceOption(DBSequenceBuilderOption.Cycling);
            else if (this.Cycling == DBSequenceCycling.NoCycle)
                builder.WriteSequenceOption(DBSequenceBuilderOption.NoCycling);

            if (this.Order == DBSequenceOrdering.NotOrdered)
                builder.WriteSequenceOption(DBSequenceBuilderOption.NotOrdered);
            else if (this.Order == DBSequenceOrdering.Ordered)
                builder.WriteSequenceOption(DBSequenceBuilderOption.Ordered);


            builder.EndCreate(DBSchemaTypes.Sequence, this.CheckExists == DBExistState.NotExists);

            return true;

        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (!string.IsNullOrEmpty(this.SequenceName))
                this.WriteAttribute(writer, XmlHelper.Name, this.SequenceName, context);

            if (!string.IsNullOrEmpty(this.Owner))
                this.WriteAttribute(writer, XmlHelper.Owner, this.Owner, context);

            if (this.MinValue != DEF_MIN)
                this.WriteAttribute(writer, XmlHelper.SequenceMin, this.MinValue.ToString(), context);

            if (this.MaxValue != DEF_MAX)
                this.WriteAttribute(writer, XmlHelper.SequenceMax, this.MaxValue.ToString(), context);

            if (this.StartWithValue != DEF_START)
                this.WriteAttribute(writer, XmlHelper.SequenceStart, this.StartWithValue.ToString(), context);

            if (this.IncrementValue != DEF_INCREMENT)
                this.WriteAttribute(writer, XmlHelper.SequenceIncrement, this.IncrementValue.ToString(), context);

            if(this.CacheSize == NO_CACHE_VALUE)
                this.WriteAttribute(writer, XmlHelper.SequenceNoCache, true.ToString(), context);
            else if(this.CacheSize != DEF_CACHE)
                this.WriteAttribute(writer, XmlHelper.SequenceCache, this.CacheSize.ToString(), context);

            if (this.Cycling != DBSequenceCycling.None)
                this.WriteAttribute(writer, XmlHelper.SequenceCycling, this.Cycling.ToString(), context);
            
            if (this.Order != DBSequenceOrdering.None)
                this.WriteAttribute(writer,XmlHelper.SequenceOrdering, this.Order.ToString(), context);
            
            return base.WriteAllAttributes(writer, context);
        }

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (this.IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.SequenceName = reader.Value;
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.Owner, reader, context))
            {
                this.Owner = reader.Value;
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.SequenceMin, reader, context))
            {
                this.MinValue = int.Parse(reader.Value);
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.SequenceMax, reader, context))
            {
                this.MaxValue = int.Parse(reader.Value);
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.SequenceStart, reader, context))
            {
                this.StartWithValue = int.Parse(reader.Value);
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.SequenceIncrement, reader, context))
            {
                this.IncrementValue = int.Parse(reader.Value);
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.SequenceNoCache, reader, context))
            {
                this.CacheSize = NO_CACHE_VALUE;
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.SequenceCache, reader, context))
            {
                this.CacheSize = int.Parse(reader.Value);
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.SequenceCycling, reader, context))
            {
                this.Cycling = (DBSequenceCycling)Enum.Parse(typeof(DBSequenceCycling), reader.Value);
                return true;
            }
            else if (this.IsAttributeMatch(XmlHelper.SequenceOrdering, reader, context))
            {
                this.Order = (DBSequenceOrdering)Enum.Parse(typeof(DBSequenceOrdering), reader.Value);
                return true;
            }
            else
                return base.ReadAnAttribute(reader, context);
        }
    }


}
