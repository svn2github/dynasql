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
using System.Data.Common;
using Perceiveit.Data.Query;

namespace Perceiveit.Data.SqLite
{
    /// <summary>
    /// Builds statements for the SQLite database engine
    /// </summary>
    public class DBStatementSQLiteBuilder : DBStatementBuilder
    {
        #region ivars

        /// <summary>
        /// Defines the value for the TopN function that is later output on the sql string as Limits
        /// </summary>
        private int _limits = -1;

        /// <summary>
        /// defines the value for the offset at which to start outputting the rows and appended to the LIMIT value
        /// </summary>
        private int _offset = -1;


        #endregion


        public DBStatementSQLiteBuilder(DBDatabase forDatabase, DBDatabaseProperties properties, System.IO.TextWriter tw, bool ownswriter)
            : base(forDatabase, properties, tw, ownswriter)
        {
        }


        public override void BeginFunction(Function function, string name)
        {
            if (function == Function.LastID)
                this.WriteRaw("last_insert_rowid");
            else
                base.BeginFunction(function, name);
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

        public override void WriteTop(double limit, double offset, TopType topType)
        {
            

            if (this.StatementDepth == 1)
            {
                if (topType == TopType.Percent)
                    throw new NotSupportedException("This provider does not support the top percent syntax");

                this._limits = (int)limit;
                this._offset = (int)offset;
            }
        }

        public override void EndSelectStatement()
        {
            if (this.StatementDepth == 1 && this._limits > 0)
            {
                this.WriteRaw(" LIMIT ");
                this.WriteRaw(_limits.ToString());
                if (this._offset > 0)
                {
                    this.WriteRaw(" OFFSET ");
                    this.WriteRaw(_offset.ToString());
                }
            }
            base.EndSelectStatement();
        }
        
    }
}
