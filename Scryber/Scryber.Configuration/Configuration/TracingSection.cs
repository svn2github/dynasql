/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Scryber.Configuration
{
    public class TracingSection : System.Configuration.ConfigurationElement
    {
        private const string TraceLevelKey = "trace-level";

        /// <summary>
        /// The configured trace level
        /// </summary>
        [ConfigurationProperty(TraceLevelKey, IsRequired = false)]
        public TraceRecordLevel TraceLevel
        {
            get
            {
                object value = this[TraceLevelKey];
                if (null == value || !(value is TraceRecordLevel))
                    return ScryberConfiguration.DefaultTraceLevel;
                else
                    return (TraceRecordLevel)value;
            }
            set
            {
                this[TraceLevelKey] = value;
            }
        }

        private const string LogCollectionKey = "";

        [ConfigurationCollection(typeof(TracingLogElement), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap,
            AddItemName = "log", RemoveItemName = "remove", ClearItemsName = "clear")]
        [ConfigurationProperty(LogCollectionKey, IsDefaultCollection = true, IsRequired = false)]
        public TracingLogCollection LogEntries
        {
            get
            {
                TracingLogCollection col = this[LogCollectionKey] as TracingLogCollection;

                return col;
            }
            set
            {
                this[LogCollectionKey] = value;
            }
        }


        public PDFTraceLog GetLog()
        {
            TraceRecordLevel level = this.TraceLevel;
            List<IPDFTraceLogFactory> all = new List<IPDFTraceLogFactory>();
            foreach (TracingLogElement ele in this.LogEntries)
            {
                if (ele.Enabled)
                {
                    all.Add(ele.GetFactory());
                }
            }
            if (all.Count == 0)
                return new DoNothingTraceLog();
            else if (all.Count == 1)
                return all[0].CreateLog(level);
            else
            {
                List<PDFTraceLog> instances = new List<PDFTraceLog>();
                foreach (IPDFTraceLogFactory factory in all)
                {
                    PDFTraceLog log = factory.CreateLog(level);
                }
                return new CompositeTraceLog(level, instances);
            }
        }
    }
}
