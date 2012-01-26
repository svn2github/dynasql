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
using Scryber;
using Scryber.Drawing;

namespace Scryber.Caching
{
    public class PDFStaticCacheProvider : IPDFCacheProvider
    {
        private class CacheEntry
        {
            internal object Data;
            internal DateTime Expires;
            internal bool IsExpired() { return DateTime.Now > Expires; }
        }

        private static Dictionary<string, CacheEntry> _cache = new Dictionary<string,CacheEntry>(StringComparer.OrdinalIgnoreCase);
        private static object _lock = new object();
        
        public bool TryRetrieveFromCache(string type, string key, out object data)
        {
            key = CombineKey(type, key);
            CacheEntry entry;
            lock (_lock)
            {
                _cache.TryGetValue(key, out entry);
                if (null == entry)
                    data = null;
                else if (entry.IsExpired())
                {
                    _cache.Remove(key);
                    data = null;
                }
                else
                    data = entry.Data;

            }
            return null != data;
        }

        public void AddToCache(string type, string key, object data)
        {
            this.AddToCache(type, key, data, DateTime.MaxValue);
        }

        public void AddToCache(string type, string key, object data, TimeSpan duration)
        {
            if (duration == TimeSpan.Zero)
                return;

            this.AddToCache(type, key, data, DateTime.Now.Add(duration));
        }

        public void AddToCache(string type, string key, object data, DateTime expires)
        {
            if (expires < DateTime.Now)
                return;
            key = CombineKey(type, key);
            CacheEntry entry = new CacheEntry() { Expires = expires, Data = data };
            lock (_lock)
            {
                _cache[key] = entry;
            }
        }

        private static string CombineKey(string type, string key)
        {
            if (null == type)
                throw RecordAndRaise.ArgumentNull("type");
            else if (null == key)
                throw RecordAndRaise.ArgumentNull("key");
            else
                return "pdf:" + type + ":" + key;
        }

    }
}
