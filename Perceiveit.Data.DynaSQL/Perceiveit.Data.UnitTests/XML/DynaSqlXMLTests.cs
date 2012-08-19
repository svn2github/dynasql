using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using Perceiveit.Data;
using Perceiveit.Data.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Perceiveit.Data.DynaSql.Tests
{
    /// <summary>
    /// 
    /// All these tests are designed to test the XML Serialization.
    /// Create a SQL statement using DBQuery, then 
    /// serialize the DBQuery to XML. From this string back to a DBQuery and 
    /// finally to SQL which is compared to the original to ensure there is no loss
    /// 
    /// </summary>
    [TestClass]
    public class DynaSqlXMLTests
    {



        [TestMethod]
        public void Test_01_SelectOneCategoryTest()
        {
            int id = 1;

            DBSelectQuery select = DBQuery.SelectAll()
                                    .From("Categories").As("C")
                                    .WhereField("CategoryID", Compare.Equals, DBParam.ParamWithDelegate(DbType.Int32, delegate { return id; }))
                                    .GroupBy("CategoryName")
                                    .OrderBy("ProductCount", Order.Descending);

            string sql = this.OutputSql(select, " SELECT ONE CATEGORY", true);
            string xml = this.OutputXML(select, " SELECT ONE CATEGORY");
            this.ValidateXml(xml, sql, " SELECT ONE CATEGORY");

        }

        [TestMethod]
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

            string sql = this.OutputSql(select," # PRODUCTS IN CATEGORIES", true);
            string xml = this.OutputXML(select," # PRODUCTS IN CATEGORIES");
            this.ValidateXml(xml, sql, " # PRODUCTS IN CATEGORIES");
        }


        [TestMethod]
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

            string sql = this.OutputSql(q, "SelectCustomersWithOrdersOver500", true);
            string xml = this.OutputXML(q, "SelectCustomersWithOrdersOver500");
            ValidateXml(xml, sql, "SelectCustomersWithOrdersOver500");
        }




        [TestMethod]
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
             
            string sql = this.OutputSql(script, " Insert and Return last ID script", true);
            string xml = this.OutputXML(script, " Insert and Return last ID script");
            this.ValidateXml(xml, sql, " Insert and Return last ID script");
        }





        [TestMethod]
        public void Test_05_InsertWithSqlSelect()
        {
            DBInsertQuery ins = DBQuery.InsertInto("Categories")
                                       .Fields("CategoryName","Description","Picture")
                                       .Select(
                                            DBQuery.SelectDistinct().TopN(10)
                                                   .Fields("Country", "Country")
                                                   .Const(DbType.Object,DBNull.Value)
                                                   .From("Customers"));

            string sql = this.OutputSql(ins, " Insert with Sql Select statement", true);
            string xml = this.OutputXML(ins, " Insert with Sql Select statement");
            ValidateXml(xml, sql, " Insert with Sql Select statement");
        }





        [TestMethod]
        public void Test_06_DeleteWithoutPictureAndCast()
        {
            DBQuery del = DBQuery.DeleteFrom("Categories")
                                .WhereField("Picture", Compare.Is, DBConst.Null())
                                .OrWhereField("CategoryID",Compare.GreaterThan,(DBConst)8);

            string sql = this.OutputSql(del, " Delete with Null and Cast statement", true);
            string xml = this.OutputXML(del, " Delete with Null and Cast statement");
            ValidateXml(xml, sql, " Delete with Null and Cast statement");
        }




        [TestMethod()]
        public void Test_07_CreateTable()
        {
            DBQuery create = DBQuery.Create.Table("dbo", "MoreCategories")
                                           .Add("mc_id", DbType.Int32, DBColumnFlags.AutoAssign)
                                           .Add("mc_name", DbType.String, 50)
                                           .Add("mc_desc", DbType.String, 255, DBColumnFlags.Nullable)
                                           .Add("mc_ot_id", DbType.Int32, DBColumnFlags.Nullable)
                                           .Add("mc_ot_name", DbType.AnsiString, 50, DBColumnFlags.Nullable)
                                           .Constraints(
                                                        DBConstraint.PrimaryKey("pk_Id_and_name").Columns("mc_id", "mc_name")
                                                        );
            this.OutputAndValidate(create, " Create Table with primary key constraint");
        }





        [TestMethod()]
        public void Test_08_CreateTableWithForeignKey()
        {
            DBQuery create = DBQuery.Create.Table("dbo", "MoreCategories")
                                           .Add("mc_id", DbType.Int32, DBColumnFlags.AutoAssign | DBColumnFlags.PrimaryKey)
                                           .Add("mc_name", DbType.String, 50)
                                           .Add("mc_desc", DbType.String, 255, DBColumnFlags.Nullable)
                                           .Add("mc_ot_id", DbType.Int32, DBColumnFlags.Nullable)
                                           .Add("mc_ot_name", DbType.AnsiString, 50, DBColumnFlags.Nullable)
                                           .Constraints(
                                           DBConstraint.ForeignKey("fk_to_OtherTable").Columns("mc_ot_id", "mc_ot_name")
                                                                         .References("OtherTable").Columns("ot_id", "ot_name")
                                                                         );
            this.OutputAndValidate(create, " Create Table with foreign key constraint");

        }



        [TestMethod()]
        public void Test_09_CreateIndex()
        {
            DBQuery create = DBQuery.Create.Index("dbo", "ix_MoreCategoriesName")
                                            .Unique().NonClustered()
                                            .On("dbo", "MoreCategories")
                                            .Add("mc_name", Order.Ascending)
                                            .IfNotExists();
            
            this.OutputAndValidate(create, " Create unique non-clustered index on table");

        }



        [TestMethod()]
        public void Test_10_CreateView()
        {
            DBParam startswith = DBParam.Param("startswith", DbType.AnsiString, 50);

            DBQuery create = DBQuery.Create.View("dbo", "CategoriesByName")
                                           .As(
                                                DBQuery.Select()
                                                        .Field("mc_id").As("id")
                                                        .Field("mc_name").As("name")
                                                        .Field("mc_desc").As("desc")
                                                        .From("dbo", "MoreCategories")
                                                        .Where("mc_name", Compare.Like, startswith)
                                                        .OrderBy("mc_name")
                                             );
            this.OutputAndValidate(create, " Create a view with a select statement");
        }



        [TestMethod()]
        public void Test_11_CreateProcedure()
        {
            DBParam custid = DBParam.Param("cust",DbType.Int32);
            DBParam orderid = DBParam.Param("id", DbType.Int32, ParameterDirection.Input);
            DBParam name = DBParam.Param("name", DbType.String, 50, ParameterDirection.Input);

            DBQuery create = DBQuery.Create.StoredProcedure("dbo", "ChangeOrderItemCustomer")
                                           .WithParams(orderid, name)
                                           .As(
                                                DBQuery.Declare(custid),

                                                DBQuery.Select().TopN(1)
                                                              .Field(DBAssign.Set(custid, DBField.Field("customerid")))
                                                                    .From("Orders")
                                                                    .WhereFieldEquals("order_id", orderid),

                                                DBQuery.Update("OrderItems")
                                                        .Set("customerid", custid)
                                                        .Set("customername",name)
                                                        .WhereFieldEquals("orderid", orderid)
                                            );

            this.OutputAndValidate(create, " Create Procedure ChangeOrderItemCustomer");        
        }

        [TestMethod()]
        public void Test_12_CreateSequence()
        {
            DBDatabase db = GetOracleDB();


            DBQuery create = DBQuery.Create.Sequence("orcle", "TwoInAMillion")
                                           .Minimum(1).Maximum(1000000)
                                           .StartWith(1).IncrementBy(2)
                                           .NoCache().NoCycle();

            this.OutputAndValidate(db, create, " Create Sequence TwoInAMillion");
        }

        

        [TestMethod()]
        public void Test_13_DropTable()
        {
            DBQuery drop = DBQuery.Drop.Table("dbo", "MoreCategories").IfExists();

            this.OutputAndValidate(drop, " Drop Table MoreCategories If Exists");
        }

        [TestMethod()]
        public void Test_15_DropIndex()
        {
            DBQuery drop = DBQuery.Drop.Index("dbo", "ix_MoreCategoriesName").On("dbo","MoreCategories").IfExists();

            this.OutputAndValidate(drop, "Drop index ix_MoreCategoriesName");
        }

        [TestMethod()]
        public void Test_16_DropView()
        {
            DBQuery drop = DBQuery.Drop.View("dbo", "CategoriesByName").IfExists();

            this.OutputAndValidate(drop, " Drop view CategoriesByName");
        }

        [TestMethod()]
        public void Test_17_DropProcedure()
        {
            DBQuery drop = DBQuery.Drop.StoredProcedure("dbo", "ChangeOrderItemCustomer").IfExists();

            this.OutputAndValidate(drop, " Drop procedure ChangeOrderItemCustomer");
        }

        [TestMethod()]
        public void Test_18_DropSequence()
        {
            DBDatabase db = GetOracleDB();

            DBQuery drop = DBQuery.Drop.Sequence("orcle", "TwoInAMillion");
            this.OutputAndValidate(db, drop, " Drop Sequence TwoInAMillion");
        }

        [TestMethod()]
        public void Test_19_TableHints()
        {
            DBQuery hinted = DBQuery.SelectAll()
                                    .From("dbo", "Categories").WithHint(DBTableHint.NoLock).WithHint(DBTableHint.Index, "pk_Categories")
                                    .InnerJoin("dbo", "Customers").WithHint(DBTableHint.ReadUncommitted)
                                            .On("Categories", "category_id", Compare.Equals, "Customers", "category_id")
                                    .WhereIn(DBField.Field("Categroies", "category_id"), DBConst.Int32(1), DBConst.Int32(2), DBConst.Int32(3))
                                    .WithQueryOption(DBQueryOption.Fast, 10).WithQueryOption(DBQueryOption.HashJoin);

            this.OutputAndValidate(hinted, " Hinted joined select query");
        }

        


        //
        // support methods
        //

        #region public byte[] GetLocalImage(string imgpath)

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

        #endregion

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

        #region private static DBDatabase GetOracleDB()

        /// <summary>
        /// Creates and returns a reference to a (non-existant) Oracle Database
        /// </summary>
        /// <returns></returns>
        private static DBDatabase GetOracleDB()
        {
            DBDatabase db = CreateDB();
            if (db.ProviderName != Oracle.DBOracleImplementation.OracleProviderName)
            {
                Console.WriteLine("Running with NON Oracle Provider - so everything will fail. Only oracle provider supports the Squence generation");
                Console.WriteLine("Switching to oracle provider");
                //Doesn't need to exist to run the tests
                db = DBDatabase.Create("DATA SOURCE=0.0.0.0;PERSIST SECURITY INFO=True;USER ID=NONE;PASSWORD=NONE", Oracle.DBOracleImplementation.OracleProviderName);

            }
            return db;
        }

        #endregion

        /// <summary>
        /// Outputs the generated SQL and generated XML, 
        /// then converts XML back to SQL and makes sure it matches the previous SQL.
        /// This validated the conversion
        /// </summary>
        /// <param name="query"></param>
        /// <param name="name"></param>
        private void OutputAndValidate(DBQuery query, string name)
        {
            this.OutputAndValidate(CreateDB(), query, name);
        }

        private void OutputAndValidate(DBDatabase db, DBQuery query, string name)
        {
            string sql = this.OutputSql(db, query, name, true);
            string xml = this.OutputXML(query, name);
            this.ValidateXml(db, xml, sql, name);
        }

        #region private void OutputSql(DBQuery query, string name)

        /// <summary>
        /// Outputs the specified query as a SQL statement onto the console. User the DaBDatabase from CreateDB()
        /// </summary>
        /// <param name="query"></param>
        /// <param name="name"></param>
        private string OutputSql(DBQuery query, string name, bool output)
        {
            DBDatabase db = CreateDB();
            return OutputSql(db, query, name, output);
        }

        private string OutputSql(DBDatabase db, DBQuery query, string name, bool output)
        {
            DbCommand cmd = db.CreateCommand(query);
            string all = cmd.CommandText;
            
            if (cmd.Parameters.Count > 0)
            {
                foreach (DbParameter p in cmd.Parameters)
                {
                    string param = string.Format("Parameter '{0}': sourcename='{1}', type='{2}', size='{3}', direction='{4}', hasvalue='{5}';",
                                            p.ParameterName, p.SourceColumn, p.DbType, p.Size, p.Direction, (null != p.Value && ((p.Value is DBNull) == false)));
                    all += "\r\n" + param;
                }
            }
            if (output)
            {
                Console.WriteLine("SQL statement for :{0} ", name);
                Console.WriteLine();
                Console.WriteLine(all);
            }
            return all;
        }

        #endregion

        #region private void OutputXML(DBQuery query, string name)

        /// <summary>
        /// Outputs the specified query as an xml document onto the console
        /// </summary>
        /// <param name="query"></param>
        /// <param name="name"></param>
        private string OutputXML(DBQuery query, string name)
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
            string all = sw.ToString();

            Console.WriteLine(all);
            ((IDisposable)writer).Dispose();
            sw.Dispose();

            Console.WriteLine();
            return all;
        }

        #endregion

        #region private void ValidateXml(string xml, string sql, string name)

        /// <summary>
        /// Takes the XML string and converts it back to a DBQuery. 
        /// It then checks the generated sql statement from the DBQuery and compares to the provided sql string
        /// and makes sure they are the same (equal case sensitive).
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="sql"></param>
        /// <param name="name"></param>
        private void ValidateXml(string xml, string sql, string name)
        {
            DBDatabase db = CreateDB();
            ValidateXml(db, xml, sql, name);
        }

        private void ValidateXml(DBDatabase db, string xml, string sql, string name)
        {
            DBQuery q;
            using (System.IO.StringReader sw = new System.IO.StringReader(xml))
            {
                using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(sw))
                {
                    q = DBQuery.ReadXml(reader);
                }
            }

            string sqlFromXml = this.OutputSql(db, q, name, true);

            Assert.AreEqual(sqlFromXml, sql, "The deserialized XML did not generate the same SQL statement for '" + name + "'");
            Console.WriteLine("Deserialized XML generated same SQL for " + name);
        }

        #endregion

    }
}
