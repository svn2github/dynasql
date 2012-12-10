using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perceiveit.Data.Query
{
    public class DBUseQuery : DBStatement
    {

        private string _dbname;


        #region public string DBName {get;set;}

        /// <summary>
        /// Gets or sets the name of the database this use statement will invoke.
        /// </summary>
        public string DBName
        {
            get { return _dbname; }
            set { _dbname = value; }
        }

        #endregion

        #region protected override string XmlElementName {get;}

        /// <summary>
        /// returns the XmlElement name of this DBUse
        /// </summary>
        protected override string XmlElementName
        {
            get { return XmlHelper.Use; }
        }

        #endregion


        //
        // ctor
        //

        #region protected DBUse(string name)

        /// <summary>
        /// Creates a new 'use' statement with the specified name
        /// </summary>
        /// <param name="name"></param>
        protected DBUseQuery(string name)
        {
            this._dbname = name;
        }

        #endregion


#if SILVERLIGHT
        //No Statement building
#else

        /// <summary>
        /// Writes this use statement to the builder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public override bool BuildStatement(DBStatementBuilder builder)
        {
            builder.BeginUseStatement();
            builder.BeginIdentifier();
            builder.WriteObjectName(this.DBName);
            builder.EndIdentifier();
            builder.EndUseStatement();
            return true;
        }

#endif

        //
        //xml serialization
        //

        protected override bool WriteAllAttributes(System.Xml.XmlWriter writer, XmlWriterContext context)
        {
            if (string.IsNullOrEmpty(this.DBName) == false)
                this.WriteAttribute(writer, XmlHelper.Name, this.DBName, context);
            return base.WriteAllAttributes(writer, context);
        }

        protected override bool ReadAnAttribute(System.Xml.XmlReader reader, XmlReaderContext context)
        {
            if (this.IsAttributeMatch(XmlHelper.Name, reader, context))
            {
                this.DBName = reader.Value;
                return true;
            }
            else
                return base.ReadAnAttribute(reader, context);
        }

        //
        // static factory methods
        //


        public static DBUseQuery Use()
        {
            return DBUseQuery.Use(string.Empty);
        }

        public static DBUseQuery Use(string databaseName)
        {
            return new DBUseQuery(databaseName);
        }


    }
}
