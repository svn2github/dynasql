using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Perceiveit.Data;
using Perceiveit.Data.Query;
using System.Data.Common;

namespace Perceiveit.Data.DynaSql.Tests
{
    [TestFixture()]
    public class DynaSQLXmlReadTests
    {
        


        private static string DBConnection = Nw.DbConnection;
        private static string DBProvider = Nw.DbProvider;



        [Test()]
        public void _01_SimpleXmlTest()
        {
            DBQuery q = DBQuery.SelectAll().From("Categories");

            q = SerializeAndDeserialzeQueryToMatch(q, "Simple Test");
        }

        [Test()]
        public void _02_WhereFieldXmlTest()
        {
            DBQuery q = DBQuery.SelectAll().From("Categories")
                                           .WhereFieldEquals("category_id", DBConst.Int32(10));

            q = SerializeAndDeserialzeQueryToMatch(q, "Where Test");
        }

        [Test()]
        public void _03_FieldTypesXmlTest()
        {
            DBQuery q = DBQuery.SelectAll().Field("category_id").As("A Cateogry").Sum("count")
                                           .From("Categories")
                                           .WhereFieldEquals("category_id", DBConst.Int32(10));

            q = SerializeAndDeserialzeQueryToMatch(q, "Field Test");
        }

        [Test()]
        public void _04_JoinTypesXmlTest()
        {
            DBQuery q = DBQuery.SelectAll()
                               .From("Categories").As("C")
                               .InnerJoin("Products").As("P").On("C","category_id",Compare.Equals,"P","product_category")
                               .WhereFieldEquals("category_id", DBConst.Int32(10));

            q = SerializeAndDeserialzeQueryToMatch(q, "Join Test");
        }

        [Test()]
        public void _05_AggregateXmlTest()
        {
            DBQuery q = DBQuery.Select().Field(DBField.Field("UnitPrice") * DBField.Field("Quantity") * (DBConst.Const(1.0) - DBField.Field("Discount"))).As("OrderValue") 
                                        .From("Orders").As("0");

            q = SerializeAndDeserialzeQueryToMatch(q, "Aggregate test");
        }

        [Test()]
        public void _06_FullComplexInnerSelectXmlTest()
        {
            DBSelectQuery orderValue = DBQuery.Select()
                                                .Sum(DBField.Field("UnitPrice") * DBField.Field("Quantity") * (DBConst.Const(1.0) - DBField.Field("Discount"))).As("OrderValue")
                                                .And(DBFunction.IsNull("OrderID", DBConst.Const("No ID")))
                                                .Field("O", "customerID")
                                             .From("Orders").As("O")
                                                 .InnerJoin("Order Details").As("OD")
                                                    .On("OD", "OrderID", Compare.Equals, "O", "OrderID")
                                             .GroupBy("O", "CustomerID");

            DBQuery q = DBQuery.SelectTopN(10)
                                    .Field("CUST", "CompanyName").As("Company Name")
                                    .Field("CUST", "CustomerID").As("Customer ID")
                                    .And(DBFunction.IsNull("CUST", "ContactName", DBConst.Const("No Contact"))).As("Contact")
                                    .Sum("ORD", "OrderValue").As("Total Value")
                                .From("Customers").As("CUST")
                                    .InnerJoin(orderValue).As("ORD")
                                        .On("ORD", "CustomerID", Compare.Equals, "CUST", "CustomerID")
                                .WhereField("ORD", "OrderValue", Compare.GreaterThanEqual, DBConst.Const(500.0))
                                .GroupBy("CUST", "CompanyName").And("CUST", "CustomerID")
                                .OrderBy("Total Value", Order.Descending);

            q = SerializeAndDeserialzeQueryToMatch(q, "Complex Inner Select Test");
        }


        [Test()]
        public void _07_SimpleDeleteXmlTest()
        {
            DBQuery q = DBQuery.DeleteFrom("Customers").WhereField("customer_id", Compare.Equals, DBConst.Int32(2));
            q = SerializeAndDeserialzeQueryToMatch(q, "Simple Delete");
        }

        [Test()]
        public void _08_DeleteExistsXmlTest()
        {
            DBQuery q = DBQuery.DeleteFrom("Customers")
                               .WhereField("customer_id", Compare.In, DBValueGroup.All(1, 2, 3, 4, 5, 6))
                               .OrWhereField("picture", Compare.Is, DBConst.Null());
            q = SerializeAndDeserialzeQueryToMatch(q, "Exists with Or Delete");
        }

        [Test()]
        public void _09_DeleteParameterXmlTest()
        {
            DBParam p1 = DBParam.ParamWithValue("first", 1);
            DBParam p2 = DBParam.ParamWithValue("second", 2);
            DBParam p3 = DBParam.ParamWithValue("third", 3);

            DBQuery q = DBQuery.DeleteFrom("Customers")
                               .WhereField("customer_id", Compare.In, DBValueGroup.All(p1,p2,p3))
                               .OrWhereField("picture", Compare.Is, DBConst.Null());
            q = SerializeAndDeserialzeQueryToMatch(q, "Parameterized Delete");
        }

        [Test()]
        public void _10_UpdateSimpleXmlTest()
        {
            DBQuery q = DBQuery.Update("CUSTOMERS").Set("customer_name", DBConst.String("new name"))
                                                   .AndSet("customer_address", DBConst.String("new address"))
                                                   .WhereFieldEquals("customer_id", DBConst.Int32(10));
            q = SerializeAndDeserialzeQueryToMatch(q, "Simple Update");
        }

        [Test()]
        public void _11_UpdateSubSelectAndParameterXmlTest()
        {
            DBParam cid = DBParam.ParamWithValue("cid", 10);

            DBQuery thirtyDaysOrders = DBQuery.Select()
                                                    .Sum(DBField.Field("order_value")).From("ORDER").As("O")
                                              .WhereField("order_date", Compare.GreaterThan, DBConst.DateTime(DateTime.Today.AddDays(-30)))
                                              .AndWhere("customer_id", Compare.Equals, cid);

            DBQuery q = DBQuery.Update("CUSTOMERS").Set("customer_name", DBConst.String("new name"))
                                                   .AndSet("customer_address", DBConst.String("new address"))
                                                   .AndSet("order_value",thirtyDaysOrders)
                                                   .WhereField("customer_id",Compare.Equals, cid);
            q = SerializeAndDeserialzeQueryToMatch(q, "Sub Select Update");
        }

        [Test()]
        public void _12_InsertSimpleXmlTest()
        {
            DBParam pname = DBParam.ParamWithDelegate(delegate { return "My Customer Name"; });
            DBParam paddress = DBParam.ParamWithDelegate(delegate { return "My Customer Address"; });

            DBQuery q = DBQuery.InsertInto("CUSTOMERS").Fields("customer_name", "customer_address")
                                                       .Values(pname, paddress);

            q = SerializeAndDeserialzeQueryToMatch(q, "Simple Insert");

        }

        [Test()]
        public void _13_InsertWithSubSelectXmlTest()
        {
            DBParam pduplicate = DBParam.ParamWithDelegate("dupid", System.Data.DbType.Int32, delegate { return 10; });

            DBQuery q = DBQuery.InsertInto("CUSTOMERS").Fields("customer_name", "customer_address")
                                                       .Select(DBQuery.Select().Fields("customer_name", "customer_address")
                                                                               .From("CUSTOMERS").As("DUP")
                                                                               .WhereFieldEquals("customer_id", pduplicate));

            q = SerializeAndDeserialzeQueryToMatch(q, "Sub Select Insert");

        }

        [Test()]
        public void _14_ScriptTest()
        {
            DBParam pname = DBParam.ParamWithDelegate(delegate { return "My Customer Name"; });
            DBParam paddress = DBParam.ParamWithDelegate(delegate { return "My Customer Address"; });

            DBQuery q = DBQuery.Begin(
                        DBQuery.InsertInto("CUSOMERS").Fields("customer_name", "customer_address")
                                                      .Values(pname, paddress),
                        DBQuery.Select(DBFunction.LastID()))
                        .End();

            q = SerializeAndDeserialzeQueryToMatch(q, "Simple Script");
        }



        private DBQuery SerializeAndDeserialzeQueryToMatch(DBQuery q, string name)
        {
            string statement = this.OutputSql(q, name);

            string result = this.OutputXml(q, name);

            q = this.ReadXmlQuery(result);

            string parsed = this.OutputSql(q, name);
            Assert.IsTrue(parsed.Equals(statement, StringComparison.CurrentCultureIgnoreCase), "Compared SQL statements are not the same");
            Console.WriteLine("Confirmed the generated SQL matches for query :" + name);
            return q;
        }

        private DBQuery ReadXmlQuery(string xml)
        {
            using (System.IO.StringReader sr = new System.IO.StringReader(xml))
            {
                using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(sr))
                {
                    return DBQuery.ReadXml(reader);
                }
            }
        }

        private string OutputXml(DBQuery q, string name)
        {
            string xml;
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineHandling = System.Xml.NewLineHandling.Entitize;
                settings.NewLineOnAttributes = false;
                settings.ConformanceLevel = System.Xml.ConformanceLevel.Document;
                settings.CheckCharacters = true;

                using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sw,settings))
                {
                    q.WriteXml(writer);
                }
                sw.Flush();
                xml = sw.ToString();
            }
            Console.WriteLine("XML text for :{0} ", name);
            Console.WriteLine();
            Console.WriteLine(xml);
            Console.WriteLine();

            return xml;
        }

        private void OutputXml(string xml, string name)
        {
            Console.WriteLine("XML text for :{0} ", name);
            Console.WriteLine();
            Console.WriteLine(xml);
            Console.WriteLine();
        }


        #region public static DBDatabase CreateDB()

        /// <summary>
        /// Creates a new DBDatabase referring to the constants DBConnection and DBProvider
        /// </summary>
        /// <returns></returns>
        public static DBDatabase CreateDB()
        {
            DBDatabase db = DBDatabase.Create(DBConnection, DBProvider);

            //requires valid connection
            //Console.WriteLine("DataBase '" + db.ProviderName + "' properties: " + db.DBProperties.ToString());

            return db;
        }

        #endregion

        #region private void OutputSql(DBQuery query, string name)

        /// <summary>
        /// Outputs the specified query as a SQL statement onto the console. User the DaBDatabase from CreateDB()
        /// </summary>
        /// <param name="query"></param>
        /// <param name="name"></param>
        private string OutputSql(DBQuery query, string name)
        {
            DBDatabase db = CreateDB();
            Console.WriteLine("SQL statement for :{0} ", name);
            Console.WriteLine();
            DbCommand cmd = db.CreateCommand(query);
            Console.WriteLine(cmd.CommandText);
            if (cmd.Parameters.Count > 0)
            {
                foreach (DbParameter p in cmd.Parameters)
                {
                    Console.WriteLine("Parameter '{0}': sourcename='{1}', type='{2}', size='{3}', direction='{4}', hasvalue='{5}';",
                                            p.ParameterName, p.SourceColumn, p.DbType, p.Size, p.Direction, (null != p.Value && ((p.Value is DBNull) == false)));
                }
            }
            Console.WriteLine();
            string text = cmd.CommandText;
            cmd.Dispose();

            return text;
        }

        #endregion

    }
}
