/*  Copyright 2009 PerceiveIT Limited
 *  This file is part of the DynaSQL library.
 *
*  DynaSQL is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  DynaSQL is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Query in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.OleDb
{
    /// <summary>
    /// Implements the DBStatementBuilder for the OleDb provider - specifically Ms Access
    /// </summary>
    public class DBOleDbStatementBuilder : DBStatementBuilder
    {

        public DBOleDbStatementBuilder(DBDatabase forDatabase, DBDatabaseProperties properties, System.IO.TextWriter tw, bool ownswriter)
            : base(forDatabase, properties, tw, ownswriter)
        {
        }


        public override void BeginDateLiteral()
        {
            this.WriteRaw("#");
            //base.BeginDateLiteral();
        }

        public override void EndDateLiteral()
        {
            this.WriteRaw("#");
            //base.EndDateLiteral();
        }

        public override void BeginScript()
        {
            //this.Writer.Write("BEGIN");
            this.IncrementStatementBlock();
            //this.BeginNewLine();
        }

        public override void EndScript()
        {
            this.DecrementStatementBlock();
            //this.BeginNewLine();
            //this.Writer.WriteLine("END");
        }

        public override void WriteStatementTerminator()
        {
            base.WriteStatementTerminator();
        }

        public override void BeginFunction(Function function, string name)
        {
            if (function == Function.LastID)
                this.WriteRaw("@@Identity");//don't like this, but it looks like the only way
                                            //any other options then I'd go with them happily
            else
                base.BeginFunction(function, name);
        }
        protected override object ConvertParamValueToNativeValue(System.Data.DbType type, object value)
        {
            object converted;
            if (value is DateTime)
            {
                DateTime dtvalue = (DateTime)value;

                if (type == System.Data.DbType.DateTime)
                {
                    converted = new DateTime(dtvalue.Year, dtvalue.Month, dtvalue.Day, dtvalue.Hour, dtvalue.Minute, dtvalue.Second);
                }
                else if (type == System.Data.DbType.Date)
                {
                    converted = new DateTime(dtvalue.Year, dtvalue.Month, dtvalue.Day);
                }
                else if (type == System.Data.DbType.DateTime2)
                {
                    converted = new DateTime(dtvalue.Year, dtvalue.Month, dtvalue.Day, dtvalue.Hour, dtvalue.Minute, dtvalue.Second);
                }
                else
                    converted = base.ConvertParamValueToNativeValue(type, value);
            }
            else
                converted = base.ConvertParamValueToNativeValue(type, value);

            return converted;
        }

    }
}
