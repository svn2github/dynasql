using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perceiveit.Data.Query
{
    internal class DBTableHintSet : DBClause
    {

        #region public DBTable Table {get;set;}

        private DBTable _owner;

        /// <summary>
        /// Gets or sets the table these hints belong to
        /// </summary>
        public DBTable Owner { get { return _owner; } set { _owner = value; } }

        #endregion

        #region public DBTableHintOptionList Hints {get;}

        private DBTableHintOptionList _hints;

        /// <summary>
        /// Gets the list of hints on this set
        /// </summary>
        public DBTableHintOptionList Hints
        {
            get
            {
                return _hints;
            }
        }

        #endregion

        #region public bool HasHints {get;}

        /// <summary>
        /// Returns true if there is at least one hint specified in this set
        /// </summary>
        public bool HasHints
        {
            get { return this._hints != null && this._hints.Count > 0; }
        }

        #endregion

        #region protected override string XmlElementName {get;}

        /// <summary>
        /// Gets the element name for this table hint set
        /// </summary>
        protected override string XmlElementName
        {
            get { return XmlHelper.TableHintSet; }
        }

        #endregion

        //
        // ctor
        //

        #region public DBTableHintSet(DBTable owner)

        public DBTableHintSet(DBTable owner)
        {
            this._owner = owner;
            this._hints = new DBTableHintOptionList();
        }

        #endregion

        //
        // Instance methods - each adds a new hint to the collection and returns the owner table
        //


        public DBTableHintSet WithHint(DBTableHint hint)
        {
            DBTableHintOption opt = DBTableHintOption.WithHint(hint);
            this.WithHint(opt);
            return this;
        }

        public DBTableHintSet WithHint(DBTableHint hint, params string[] options)
        {
            DBTableHintOption opt = DBTableHintOption.WithHint(hint, options);
            this.WithHint(opt);
            return this;
        }


        public DBTableHintSet WithHints(params DBTableHint[] hints)
        {
            if (null != hints && hints.Length > 0)
            {
                foreach (DBTableHint hint in hints)
                {
                    this.WithHint(hint);
                }
            }
            return this;
        }

        public DBTableHintSet WithHint(DBTableHintOption hint)
        {
            this.Hints.Add(hint);
            return this;
        }

        public DBTableHintSet Clear()
        {
            this.Hints.Clear();
            return this;
        }

#if SILVERLIGHT
        // no statement building
#else

        //
        // statement building
        //

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            if (this.HasHints)
            {
                builder.BeginTableHints();
                int count = 0;
                foreach (DBTableHintOption hint in this.Hints)
                {
                    if (count > 0)
                        builder.AppendReferenceSeparator();
                    
                    hint.BuildStatement(builder);
                    count++;
                }
                builder.EndTableHints();
                return true;
            }
            else
                return false;
        }

#endif

        //
        // XML Serialization
        //

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (this.IsElementMatch(XmlHelper.TableHint, reader, context))
            {
                DBTableHintOption inner = DBTableHintOption.Empty();
                inner.ReadXml(reader, context);
                this.Hints.Add(inner);
                return true;
            }
            else
                return base.ReadAnInnerElement(reader, context);
        }

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.HasHints)
            {
                foreach (DBTableHintOption option in this.Hints)
                {
                    option.WriteXml(writer, context);
                }
            }
            return base.WriteInnerElements(writer, context);
        }

    }
}
