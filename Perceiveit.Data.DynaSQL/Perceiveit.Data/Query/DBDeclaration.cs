using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// Represents a DECLARE statement in a script
    /// </summary>
    public abstract class DBDeclaration : DBStatement
    {
        private DBParam _param;

        /// <summary>
        /// Gets the Parameter reference this instance is declaring
        /// </summary>
        public DBParam Parameter { get { return _param; } private set { _param = value; } }

        /// <summary>
        /// Creates and returns a new declaration of the parameter
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
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
