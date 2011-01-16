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
                builder.WriteRaw(this.ListStart);

            for (int i = 0; i < this.Count; i++)
            {
                DBToken clause = this[i];

                if (outputSeparator)
                {
                    if (this.UseBuilderSeparator)
                        builder.AppendReferenceSeparator();

                    else if (!string.IsNullOrEmpty(this.TokenSeparator))
                        builder.WriteRaw(this.TokenSeparator);

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
                builder.WriteRaw(end);

            //did we write anything
            if (count > 0 || !string.IsNullOrEmpty(start) || !string.IsNullOrEmpty(end))
                return true;
            else
                return false;

        }


    }
}
