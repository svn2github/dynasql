using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perceiveit.Data.Query
{
    internal class DBQueryHintOptionSet : DBExpressionSet
    {

        #region protected DBQueryHintList Items

        private DBQueryHintList _items;

        /// <summary>
        /// Gets or sets the inner list of DBQueryHintOption(s)
        /// </summary>
        protected DBQueryHintList Items
        {
            get { return _items; }
            set { _items = value; }
        }

        #endregion

        protected override string XmlElementName
        {
            get
            {
                return XmlHelper.QueryOptionSet;
            }
        }

        //
        // ctor
        //

        #region internal DBQueryHintOptionSet()

        /// <summary>
        /// Creates a new DBQueryHintOptionSet
        /// </summary>
        internal DBQueryHintOptionSet()
        {
        }

        #endregion

        //
        // public methods
        //
         
        #region public DBQueryHintOptionSet Add(DBQueryHintOption options)

        /// <summary>
        /// Adds a DBQueryHintOption to this set of hints
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public DBQueryHintOptionSet Add(DBQueryHintOption options)
        {
            if (this.Items == null)
                this.Items = new DBQueryHintList();
            this.Items.Add(options);

            return this;

        }

        #endregion

#if SILVERLIGHT
        // no statement building
#else
        //
        // sql statement building
        //

        #region public override bool BuildStatement(DBStatementBuilder builder)

        /// <summary>
        /// Builds the SQL for the query hints in this set
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (null != this.Items && this.Items.Count > 0)
            {
                int count = 0;
                foreach (DBQueryHintOption hint in this.Items)
                {
                    if (count > 0)
                        builder.WriteReferenceSeparator();

                    hint.BuildStatement(builder);
                    count++;
                }
                return true;
            }
            else
                return false;
        }

        #endregion

#endif

        //
        // XML Serialization
        //

        #region protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (IsElementMatch(XmlHelper.QueryOption, reader, context))
            {
                DBClause c = this.ReadNextInnerClause(reader.Name, reader, context);
                if (null == this.Items)
                    this.Items = new DBQueryHintList();

                this.Items.Add(c as DBQueryHintOption);

                return true;
            }
            else
                return base.ReadAnInnerElement(reader, context);
        }

        #endregion

        #region protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this._items != null && this._items.Count > 0)
            {
                foreach (DBQueryHintOption hint in this._items)
                {
                    hint.WriteXml(writer, context);
                }
            }
            return base.WriteInnerElements(writer, context);
        }

        #endregion

    }
}
