using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perceiveit.Data.Query
{

    /// <summary>
    /// Defines a single table hint
    /// </summary>
    public abstract class DBTableHintOption : DBHintOption
    {
        private DBTableHint _option;
        private string[] _params;

        public DBTableHint Option { get { return _option; } set { _option = value; } }

        public string[] Parameters { get { return _params; } set { _params = value; } }

        public static DBTableHintOption WithHint(DBTableHint hint)
        {
            DBTableHintOption with = new DBTableHintOptionRef();
            with.Option = hint;
            return with;
        }

        public static DBTableHintOption WithHint(DBTableHint hint, string[] parameters)
        {
            DBTableHintOption with = WithHint(hint);
            with.Parameters = parameters;
            return with;
        }

        internal static DBTableHintOption Empty()
        {
            return new DBTableHintOptionRef();
        }
    }

    /// <summary>
    /// Implementation class
    /// </summary>
    internal class DBTableHintOptionRef : DBTableHintOption
    {
        protected override string XmlElementName
        {
            get { return XmlHelper.TableHint; }
        }

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginTableHint(this.Option);
            if (this.Parameters != null && this.Parameters.Length > 0)
            {
                string[] all = this.Parameters;
                builder.BeginHintParameterList();
                for (int i = 0; i < all.Length; i++)
                {
                    builder.WriteHintParameter(i, all[i]);
                }
                builder.EndHintParameterList();
            }
            builder.EndTableHint(this.Option);
            return true;
        }

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            this.WriteAttribute(writer, XmlHelper.HintOption, this.Option.ToString(), context);
            return base.WriteAllAttributes(writer, context);
        }

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (this.Parameters != null && this.Parameters.Length > 0)
            {
                for (int i = 0; i < Parameters.Length; i++)
                {
                    writer.WriteStartElement(XmlHelper.HintParameter);
                    writer.WriteValue(this.Parameters[i]);
                    writer.WriteEndElement();
                }
            }
            return base.WriteInnerElements(writer, context);
        }

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (IsAttributeMatch(XmlHelper.HintOption, reader, context))
            {
                object val = Enum.Parse(typeof(DBTableHint), reader.Value);
                this.Option = (DBTableHint)val;
                return true;
            }
            return base.ReadAnAttribute(reader, context);
        }

        protected override bool ReadAnInnerElement(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (IsElementMatch(XmlHelper.HintParameter, reader, context))
            {
                string value = reader.ReadElementContentAsString();
                if (null == this.Parameters)
                    this.Parameters = new string[] { value };
                else
                {
                    string[] all = this.Parameters;
                    Array.Resize<string>(ref all, all.Length + 1);
                    all[all.Length - 1] = value;
                    this.Parameters = all;
                }
                return true;
            }
            else
                return base.ReadAnInnerElement(reader, context);
        }
    }



    /// <summary>
    /// A list of Table hints keyed by DBTableHint
    /// </summary>
    public class DBTableHintOptionList : System.Collections.ObjectModel.KeyedCollection<DBTableHint, DBTableHintOption>
    {
        protected override DBTableHint GetKeyForItem(DBTableHintOption item)
        {
            return item.Option;
        }
    }

}
