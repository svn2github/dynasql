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

namespace Scryber.Data
{
    [PDFParsableComponent("XMLDataSource")]
    public class PDFXMLDataSource : PDFDataSourceBase
    {
        private string _source;

        [PDFAttribute("source-path")]
        public string SourcePath
        {
            get { return _source; }
            set { _source = value; }
        }

        private int _cachedur = -1;

        /// <summary>
        /// The total number of seconds to cache the data source
        /// </summary>
        [PDFAttribute("cache-duration")]
        public int CacheDuration
        {
            get { return this._cachedur; }
            set { this._cachedur = value; }
        }

        private string _trannypath;

        [PDFAttribute("transform-path")]
        public string TransformPath
        {
            get { return _trannypath; }
            set { this._trannypath = value; }
        }

        public PDFXMLDataSource()
            : this(PDFObjectTypes.XmlData)
        {
        }

        protected PDFXMLDataSource(PDFObjectType type)
            : base(type)
        {
        }

        private System.Xml.XmlDocument _doc = null;

        protected override object DoSelectData(string path)
        {

            if (_doc == null)
            {
                _doc = this.LoadXMLDocument(this.SourcePath);
            }

            if (string.IsNullOrEmpty(path))
                return _doc.CreateNavigator().Select("*");
            else
                return _doc.CreateNavigator().Select(path);
        }

        private System.Xml.XmlDocument LoadXMLDocument(string sourcepath)
        {
            PDFDocument pdf = this.Document;
            string path = this.MapPath(sourcepath);
            
            object obj;

            System.Xml.XmlDocument doc;
            if (this.CacheDuration > 0)
            {
                if (!pdf.CacheProvider.TryRetrieveFromCache(PDFObjectTypes.XmlData.ToString(), path, out obj))
                {
                    doc = new System.Xml.XmlDocument();
                    doc.Load(path);
                    pdf.CacheProvider.AddToCache(PDFObjectTypes.XmlData.ToString(), path, doc, new TimeSpan(0, 0, this.CacheDuration));
                }
                else
                    doc = (System.Xml.XmlDocument)obj;
            }
            else //not using cacheing
            {
                doc = new System.Xml.XmlDocument();
                doc.Load(path);
            }
            return doc;
        }
    }
}
