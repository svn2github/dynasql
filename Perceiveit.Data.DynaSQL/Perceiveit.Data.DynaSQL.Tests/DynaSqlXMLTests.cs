using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using NUnit.Framework;
using Perceiveit.Data;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.DynaSql.Tests
{
    [TestFixture()]
    public class DynaSqlXMLTests
    {

        

        [Test()]
        public void Test_01_SelectOneCategoryTest()
        {
            int id = 1;

            DBSelectQuery select = DBQuery.SelectAll()
                                    .From("Categories").As("C")
                                    .WhereField("CategoryID", Compare.Equals, DBParam.ParamWithDelegate(DbType.Int32, delegate { return id; }))
                                    .GroupBy("CategoryName")
                                    .OrderBy("ProductCount", Order.Descending);

            this.OutputSql(select, " SELECT ONE CATEGORY");
            this.OutputXML(select, " SELECT ONE CATEGORY");

        }

        [Test()]
        public void Test_02_SelectProductsInCategoriesTest()
        {
            DBSelectQuery select = DBQuery.Select()
                                          .Count("ProductID").As("ProductCount")
                                          .Field("CategoryName")
                                    .From("Categories").As("C")
                                        .InnerJoin("Products").As("P")
                                            .On("C", "CategoryID", Compare.Equals, "P", "CategoryID")
                                    .GroupBy("CategoryName")
                                    .OrderBy("ProductCount", Order.Descending);

            this.OutputSql(select," # PRODUCTS IN CATEGORIES");
            this.OutputXML(select," # PRODUCTS IN CATEGORIES");

        }


        [Test()]
        public void Test_03_ScriptOfSelectCustomersWithOrdersOver500()
        {
            DBSelectQuery orderValue = DBQuery.Select()
                                            .Sum(DBField.Field("UnitPrice") * DBField.Field("Quantity") * (DBConst.Const(1.0) - DBField.Field("Discount"))).As("OrderValue")
                                            .And(DBFunction.IsNull("OrderID",DBConst.Const("No ID")))
                                            .Field("O","customerID")
                                        .From("Orders").As("O")
                                             .InnerJoin("Order Details").As("OD")
                                                .On("OD","OrderID",Compare.Equals,"O","OrderID")
                                        .GroupBy("O","CustomerID");

            DBQuery q = DBQuery.SelectTopN(10)
                                    .Field("CUST", "CompanyName").As("Company Name")
                                    .Field("CUST","CustomerID").As("Customer ID")
                                    .And(DBFunction.IsNull("CUST","ContactName", DBConst.Const("No Contact"))).As("Contact")
                                    .Sum("ORD", "OrderValue").As("Total Value")
                                .From("Customers").As("CUST")
                                    .InnerJoin(orderValue).As("ORD")
                                        .On("ORD", "CustomerID", Compare.Equals, "CUST", "CustomerID")
                                .WhereField("ORD", "OrderValue", Compare.GreaterThanEqual, DBConst.Const(500.0))
                                .GroupBy("CUST","CompanyName").And("CUST","CustomerID")
                                .OrderBy("Total Value", Order.Descending);

            this.OutputSql(q, "SelectCustomersWithOrdersOver500");
            this.OutputXML(q, "SelectCustomersWithOrdersOver500");
        }




        [Test()]
        public void Test_04_InsertScript()
        {
            byte[] imgdata = this.GetLocalImage("bomb.gif");
            
            DBParam cName = DBParam.ParamWithValue("name", DbType.String,(object)"newType_1");
            DBParam cDesc = DBParam.ParamWithValue("desc", DbType.String, (object)"newDescrption_1");
            DBParam cPic = DBParam.ParamWithValue("pic", DbType.Binary, (object)imgdata);
            
            DBInsertQuery ins = DBQuery.InsertInto("Categories")
                                       .Fields("CategoryName", "Description", "Picture")
                                       .Values(cName, cDesc, cPic);
            DBSelectQuery sel = DBQuery.Select(DBFunction.LastID());

            DBScript script = DBQuery.Begin(ins)
                                     .Then(sel)
                                     .End();

            this.OutputSql(script, "Insert and Return last ID script");
            this.OutputXML(script, "Insert and Return last ID script");
        }





        [Test()]
        public void Test_05_InsertWithSqlSelect()
        {
            DBInsertQuery ins = DBQuery.InsertInto("Categories")
                                       .Fields("CategoryName","Description","Picture")
                                       .Select(
                                            DBQuery.SelectDistinct().TopN(10)
                                                   .Fields("Country", "Country")
                                                   .Const(DbType.Object,DBNull.Value)
                                                   .From("Customers"));

            this.OutputSql(ins, "Insert with Sql Select statement");
            this.OutputXML(ins, "Insert with Sql Select statement");
        }





        [Test()]
        public void Test_06_DeleteWithoutPictureAndCast()
        {
            DBQuery del = DBQuery.DeleteFrom("Categories")
                                .WhereField("Picture", Compare.Is, DBConst.Null())
                                .OrWhereField("CategoryID",Compare.GreaterThan,(DBConst)8);

            this.OutputSql(del, "Insert with Sql Select statement");
            this.OutputXML(del, "Insert with Sql Select statement");
        }

        //
        // support methods
        //

        public byte[] GetLocalImage(string imgpath)
        {
            imgpath = System.IO.Path.Combine(System.Environment.CurrentDirectory, imgpath);


            byte[] imgdata = null;

            if (System.IO.File.Exists(imgpath))
            {
                Console.WriteLine("Using Image at path:{0}", imgpath);
                imgdata = System.IO.File.ReadAllBytes(imgpath);
            }

            return imgdata;

        }

        #region public static DBDatabase CreateDB()

        /// <summary>
        /// Creates a new DBDatabase referring to the constants DBConnection and DBProvider
        /// </summary>
        /// <returns></returns>
        public static DBDatabase CreateDB()
        {
            DBDatabase db = DBDatabase.Create(Nw.DbConnection, Nw.DbProvider);

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
        private void OutputSql(DBQuery query, string name)
        {
            DBDatabase db = CreateDB();
            Console.WriteLine("SQL statement for :{0} ",name);
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
        }

        #endregion

        #region private void OutputXML(DBQuery query, string name)

        /// <summary>
        /// Outputs the specified query as an xml document onto the console
        /// </summary>
        /// <param name="query"></param>
        /// <param name="name"></param>
        private void OutputXML(DBQuery query, string name)
        {
            Console.WriteLine("XML statement for :{0} ", name);
            Console.WriteLine();
            
            System.IO.StringWriter sw = new System.IO.StringWriter();

            System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineHandling = System.Xml.NewLineHandling.Entitize;
            settings.NewLineOnAttributes = false;
            settings.ConformanceLevel = System.Xml.ConformanceLevel.Document;
            settings.CheckCharacters = true;

            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(sw, settings);
            query.WriteXml(writer);
            writer.Close();
            sw.Close();
            Console.WriteLine(sw.ToString());
            ((IDisposable)writer).Dispose();
            sw.Dispose();

            Console.WriteLine();

        }

        #endregion
    }
}
