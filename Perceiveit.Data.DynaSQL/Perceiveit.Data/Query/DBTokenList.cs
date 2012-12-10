/*  Copyright 2009 PerceiveIT Limited
 *  This file is part of the DynaSQL library.
 *
*  DynaSQL is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  DynaSQL is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Query in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Perceiveit.Data.Query
{
    /// <summary>
    /// A list of DBTokens along with methods to support statement building
    /// </summary>
    public class DBTokenList<T> : List<T> where T : DBToken
    {
        /// <summary>
        /// Gets the flag that identifies if this list should use the standard builder ReferenceSeparator
        /// </summary>
        public virtual bool UseBuilderSeparator { get { return true; } }

        /// <summary>
        /// Gets the separator for each token when the statements are built
        /// </summary>
        public virtual string TokenSeparator { get { return ", "; } }

        /// <summary>
        /// Gets the characters that should prepend the list when built
        /// </summary>
        public virtual string ListStart { get { return ""; } }

        /// <summary>
        /// Gets the characters that should be appended to the list after being built
        /// </summary>
        public virtual string ListEnd { get { return ""; } }


#if SILVERLIGHT
        // no statement building
#else
        /// <summary>
        /// Builds this list of tokens onto the statement
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual bool BuildStatement(DBStatementBuilder builder)
        {
            return this.BuildStatement(builder, false, false);
        }

        /// <summary>
        /// Builds the list of tokens 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="eachOnNewLine"></param>
        /// <param name="indent"></param>
        /// <returns></returns>
        public virtual bool BuildStatement(DBStatementBuilder builder, bool eachOnNewLine, bool indent)
        {
            int count = 0;

            bool outputSeparator = false; //initially false

            string start = this.ListStart;
            if (!string.IsNullOrEmpty(start))
                builder.WriteRawSQLString(this.ListStart);
            else if (eachOnNewLine)
                builder.BeginNewLine();

            for (int i = 0; i < this.Count; i++)
            {
                DBToken clause = this[i];

                if (outputSeparator)
                {
                    if (this.UseBuilderSeparator)
                        builder.WriteReferenceSeparator();

                    else if (!string.IsNullOrEmpty(this.TokenSeparator))
                        builder.WriteRawSQLString(this.TokenSeparator);

                    if (eachOnNewLine)
                        builder.BeginNewLine();
                }

                //if the clause outputs then it should return true - so the next item has a separator written
                outputSeparator = clause.BuildStatement(builder);

                if (outputSeparator) //if we were output then increment the count
                    count++;
            }

            string end = this.ListEnd;
            if (!string.IsNullOrEmpty(end))
                builder.WriteRawSQLString(end);

            //did we write anything
            if (count > 0 || !string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
                return true;
            else
                return false;

        }

#endif

    }
}
