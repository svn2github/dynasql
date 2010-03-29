using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.Query
{
    public abstract class DBDeclaration : DBStatement
    {
        private DBParam _param;
        public DBParam Parameter { get { return _param; } private set { _param = value; } }


        public static DBDeclaration Declare(DBParam param)
        {
            DBDelcarationRef dref = new DBDelcarationRef();
            dref.Parameter = param;
            return dref;
        }
    }

    internal class DBDelcarationRef : DBDeclaration
    {
        protected override string XmlElementName
        {
            get { return XmlHelper.Declare; }
        }

        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginDeclareStatement(this.Parameter);
            builder.EndDeclareStatement();
            return true;
        }

        protected override bool WriteInnerElements(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            bool b = base.WriteInnerElements(writer, context);
            this.Parameter.WriteXml(writer, context);
            return b;
        }
    }
}
