using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perceiveit.Data.UnitTests
{
    /// <summary>
    /// Encapsulates read csv data from a file or text stream.
    /// </summary>
    /// <remarks>
    /// Use the static Parse methods to load a set of data. 
    /// The headings property will then contain the first row of data if parsed with headings
    /// The Items property will contain the remaining rows as an array of CSVItem(s) that cells can be accessed by index
    /// </remarks>
    public class CSVData
    {
        public const string NULLSTRING = "NULL";

        public string[] Headings { get; private set; }

        public CSVItem[] Items { get; private set; }

        private CSVData()
        {
        }

        /// <summary>
        /// Gets the offset index of the heading name in the csv data
        /// </summary>
        /// <param name="headingname">The name of the heading column (case sensitive)</param>
        /// <returns>It's offset</returns>
        /// <exception cref="ArgumentNullException" >Thrown if the heading name is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if this csv data does not have headings
        /// <exception cref="ArgumentOutOfRangeException" >Thrown if the requested heading is not in this csv data</exception>
        public int GetOffset(string headingname)
        {
            if (null == headingname)
                throw new ArgumentNullException("headingname");
            if (null == this.Headings)
                throw new InvalidOperationException("This csv data does not have any headings");
            int offset = Array.IndexOf(Headings, headingname);
            if (offset < 0)
                throw new ArgumentOutOfRangeException("The heading '" + headingname + "' does not exists in the CSV data");
            return offset;
        }

        public static CSVData Parse(string path, bool hasHeaders)
        {
            using (System.IO.TextReader reader = new System.IO.StreamReader(path))
            {
                return Parse(reader, hasHeaders);
            }
        }

        public static CSVData ParseString(string data, bool hasHeaders)
        {
            using (System.IO.StringReader sr = new System.IO.StringReader(data))
            {
                return Parse(sr, hasHeaders);
            }
        }

        public static CSVData Parse(System.IO.TextReader reader, bool hasHeaders)
        {
            int line = 0;
            string[] headers = null;
            List<CSVItem> all = new List<CSVItem>();
            string linedata = reader.ReadLine();

            while(null != linedata)
            {
                CSVItem item = ParseItem(linedata);
                if (null != item)
                {
                    if (hasHeaders && line == 0)
                        headers = item.ToArray();
                    else
                        all.Add(item);
                }
                line++;
                linedata = reader.ReadLine();

            }
            CSVData csv = new CSVData();
            csv.Headings = headers;
            csv.Items = all.ToArray();

            return csv;
        }

        /// <summary>
        /// Splits a single line into multiple cells breaking on a comma. 
        /// Trims each entry and removes any quotes.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>An initialized CSVItem containing all the cells</returns>
        private static CSVItem ParseItem(string data)
        {
            if (string.IsNullOrEmpty(data))
                return null;

            string[] all = data.Split(',');
            
            for (int i = 0; i < all.Length; i++)
            {
                string one = all[i].Trim();
                if (one.StartsWith("\""))
                    one = one.Substring(1, one.Length - 2);
                if (one == NULLSTRING)
                    one = null;
                all[i] = one;
            }
            return new CSVItem(all);
        }

    }

    public class CSVItem
    {
        private string[] _cells;

        public string this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index");
                else if (index >= this._cells.Length)
                    return null;
                else
                    return _cells[index];
            }
        }

        

        public int Count
        {
            get { return _cells.Length; }
        }

        public CSVItem(string[] cells)
        {
            this._cells = cells;
        }

        public bool IsNull(int index)
        {
            string value = _cells[index];
            return null == value;
        }

        public string[] ToArray()
        {
            return (string[])_cells.Clone();
        }
    }

    
}
