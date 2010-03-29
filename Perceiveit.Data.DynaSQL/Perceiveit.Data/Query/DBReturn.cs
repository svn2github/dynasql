using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.Query
{
    public abstract class DBReturn : DBStatement
    {
        private DBClause _toreturn;

        public DBClause ToReturn
        {
            get { return _toreturn; }
            protected set { _toreturn = value; }
        }

        public static DBReturn Return()
        {
            return new DBReturnRef();
        }

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
