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
using Scryber.Components;
using Scryber.Native;

namespace Scryber
{
    public class PDFNameDictionary : IArtefactCollection
    {

        private string _name;
        private Dictionary<string, PDFDestination> _dests = new Dictionary<string,PDFDestination>();

        public string CollectionName
        {
            get { return _name; }
        }

        public PDFNameDictionary(string name)
        {
            _name = name;
        }

        object IArtefactCollection.Register(IArtefactEntry entry)
        {
            PDFDestination dest = (PDFDestination)entry;
            return this.RegisterDestination(dest);
        }

        internal PDFDestination RegisterDestination(PDFDestination dest)
        {
            _dests[dest.FullName] = dest;
            return dest;
        }

        void IArtefactCollection.Close(object result)
        {
        }

        public void Clear()
        {
            _dests.Clear();
        }

        public PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            PDFObjectRef names = writer.BeginObject();
            writer.BeginDictionary();

            if (_dests.Count > 0)
            {
                List<string> all = new List<string>(_dests.Keys);

                all.Sort();
                PDFObjectRef dests = WriteDestinationNames(context, writer, all);

                writer.WriteDictionaryObjectRefEntry("Dests", dests);
            }
            writer.EndDictionary();

            writer.EndObject();

            return names;
        }


        private PDFObjectRef WriteDestinationNames(PDFRenderContext context, PDFWriter writer, IEnumerable<string> all)
        {
            PDFObjectRef dests = writer.BeginObject();
            writer.BeginDictionary();

            //Write the names array
            writer.BeginDictionaryEntry("Names");
            writer.BeginArray();

            string firstname = string.Empty;
            string lastname = string.Empty;

            foreach (string name in all)
            {

                if (WriteDestination(context, writer, name))
                {
                    if (string.IsNullOrEmpty(firstname))
                        firstname = name;
                    lastname = name;
                }
            }
            writer.EndArray();
            writer.EndDictionaryEntry();

            //Write limits
            writer.BeginDictionaryEntry("Limits");
            writer.WriteArrayStringEntries(firstname, lastname);
            writer.EndDictionaryEntry();

            writer.EndDictionary();
            writer.EndObject();
            return dests;
        }

        internal bool WriteDestination(PDFRenderContext context, PDFWriter writer, string name)
        {
            PDFDestination dest = _dests[name];

            writer.BeginArrayEntry();
            writer.WriteStringLiteral(name);
            writer.EndArrayEntry();

            writer.BeginArrayEntry();
            dest.RenderToPDF(context, writer);
            writer.EndArrayEntry();
            writer.WriteLine();
            return true;
        }

        
    }
}
