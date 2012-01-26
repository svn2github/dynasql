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
using System.Text;
using System.Threading;

namespace Scryber
{
    /// <summary>
    /// The PDFTraceContext is a static class used to access the current PDFTraceLog
    /// </summary>
    /// <remarks>The log is thread specific and stored in the thread data</remarks>
    public static class PDFTraceContext
    {
        private const string TraceContextName = "Scryber.TraceContext";

        public static PDFTraceLog GetLog()
        {
            LocalDataStoreSlot slot = Thread.GetNamedDataSlot(TraceContextName);

            PDFTraceLog log = Thread.GetData(slot) as PDFTraceLog;
            if (log == null)
            {
                log = CreateTraceLog();
                Thread.SetData(slot, log);
            }
            
            return log;
        }

        private static PDFTraceLog CreateTraceLog()
        {
            return Configuration.ScryberConfiguration.GetLog();
        }

        public static PDFTraceLog InitTraceLog()
        {
            LocalDataStoreSlot slot = Thread.GetNamedDataSlot(TraceContextName);
            object data = Thread.GetData(slot);
            if (data is IDisposable)
                ((IDisposable)data).Dispose();

            PDFTraceLog log = CreateTraceLog();
            Thread.SetData(slot, log);
            

            return log;
        }

        public static void ClearTraceLog()
        {
            LocalDataStoreSlot slot = Thread.GetNamedDataSlot(TraceContextName);
            object data = Thread.GetData(slot);
            if (data is IDisposable)
            {
                ((IDisposable)data).Dispose();
            }
            Thread.FreeNamedDataSlot(TraceContextName);
            
        }
    }
}
