using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.DynaSql.Tests
{
    public class CustomDBSqlClientImplementation : Perceiveit.Data.SqlClient.DBSqlClientImplementation
    {

        public CustomDBSqlClientImplementation()
            : base()
        {
            Console.WriteLine("***** Custom SqlClient implementation created *****");
        }

    }
}
