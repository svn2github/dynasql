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

namespace Scryber.Styles
{
    [PDFParsableComponent("Styles")]
    [PDFRemoteParsableComponent("Styles-Ref")]
    public class PDFStylesDocument : PDFStyleBase, IPDFComponent, IPDFRemoteComponent
    {

        public event EventHandler Initialized;


        protected virtual void OnInit(PDFInitContext context)
        {
            if(null != this.Initialized)
                this.Initialized(this,EventArgs.Empty);
        }


        //
        // properties
        //
        

        private PDFStyleCollection _styles;

        [PDFElement("")]
        [PDFArray(typeof(PDFStyle))]
        public PDFStyleCollection Styles
        {
            get 
            {
                if (null == _styles)
                    _styles = new PDFStyleCollection();
                return _styles;
            }
            set { _styles = value; }
        }

        public IPDFDocument Document
        {
            get { return _parent.Document; }
        }

        private IPDFComponent _parent;

        public IPDFComponent Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }

        private string _id;

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }


        #region public string LoadedSource {get;set;}

        private string _source;
        /// <summary>
        /// Gets or sets the full path to the source this document was loaded from
        /// </summary>
        public string LoadedSource
        {
            get { return _source; }
            set 
            { 
                this._source = value;
            }
        }

        #endregion

        #region public string ReferenceSource {get;set;}

        private string _refsrc;
        public const string FilePathAttributeName = "source";

        /// <summary>
        /// Gets or sets the reference source path this style document was loaded from
        /// </summary>
        [PDFAttribute(FilePathAttributeName)]
        public string ReferenceSource
        {
            get { return _refsrc; }
            set { _refsrc = value; }
        }

        #endregion

        //
        // ctor(s)
        //


        public PDFStylesDocument()
            : this(PDFObjectTypes.StyleDocument)
        {
        }

        public PDFStylesDocument(PDFObjectType type)
            : base(type)
        {
        }

        //
        // methods
        //

        /// <summary>
        /// Returns a full path to a resource based upon the 
        /// provided path and the root path of the document. If the 
        /// path cannot be determined returns the original path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public virtual string MapPath(string path)
        {
            if (System.Web.VirtualPathUtility.IsAbsolute(path))
                return path;

            if (System.IO.Path.IsPathRooted(path))
                return path;

            if (!string.IsNullOrEmpty(this.LoadedSource))
            {
                bool isfile;
                path = this.MapPath(path, out isfile);
                return path;
            }
            else if (null == this.Parent)
                return path;
            else
                return this.Parent.MapPath(path);
        }

        public virtual string MapPath(string source, out bool isfile)
        {
            string result;
            string path;

            if (string.IsNullOrEmpty(source))
                throw new ArgumentNullException("source");

            if (Uri.IsWellFormedUriString(source, UriKind.RelativeOrAbsolute))
            {
                if (System.Web.VirtualPathUtility.IsAppRelative(source))
                {
                    result = System.Web.HttpContext.Current.Server.MapPath(source);
                    isfile = true;
                }
                else if (Uri.IsWellFormedUriString(source, UriKind.Absolute))
                {
                    result = source;
                    isfile = false;
                }
                else if (!GetLocalPath(out path, out isfile))
                    throw new System.IO.FileNotFoundException(source);
                else if (isfile)
                {
                    result = System.IO.Path.Combine(path, source.Replace('/', '\\'));
                    result = System.IO.Path.GetFullPath(result);
                }
                else
                    result = System.Web.VirtualPathUtility.Combine(path, source);

            }
            else if (System.IO.Path.IsPathRooted(source))
            {
                isfile = true;
                result = System.IO.Path.GetFullPath(source); //Normalize
            }
            else if (!GetLocalPath(out path, out isfile))
                throw new System.IO.FileNotFoundException(source);

            else if (isfile)
            {
                result = System.IO.Path.Combine(path, source.Replace('/', '\\'));
                result = System.IO.Path.GetFullPath(result);
            }
            else
                result = System.Web.VirtualPathUtility.Combine(path, source);

            return result;
        }

        /// <summary>
        /// Gets the full path to this local document - setting isfile to true if the path is a file rather than a uri
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isfile"></param>
        /// <returns></returns>
        private bool GetLocalPath(out string path, out bool isfile)
        {
            path = this.LoadedSource;
            if (string.IsNullOrEmpty(path))
                path = this.GetRootDirectory();
            else
                path = System.IO.Path.GetDirectoryName(this.LoadedSource);

            if (string.IsNullOrEmpty(path))
            {
                isfile = false;
                return false;
            }
            else if (System.Uri.IsWellFormedUriString(path, UriKind.Absolute))
            {
                isfile = false;
                return true;
            }
            else if (System.IO.Path.IsPathRooted(path))
            {
                isfile = true;
                return true;
            }
            else if (path.StartsWith("~"))
            {
                string exec = this.GetRootDirectory();
                if (path.Length > 1)
                {
                    //merge current directory with the path.
                    path = path.Substring(1);
                    if (!path.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString())
                        && !exec.StartsWith(System.IO.Path.DirectorySeparatorChar.ToString()))
                        exec += System.IO.Path.DirectorySeparatorChar.ToString();
                    path = exec + path;
                }
                else
                    path = exec;
                isfile = true;

                return true;
            }
            else
            {
                throw new System.IO.FileNotFoundException(path);
            }
        }

        /// <summary>
        /// Gets the root (working) directory for this document - 
        /// root of the web application if its a web document or the current directory for executable
        /// </summary>
        /// <returns></returns>
        protected string GetRootDirectory()
        {
            if (null != System.Web.HttpContext.Current)
            {
                return System.Web.HttpContext.Current.Server.MapPath("/");
            }
            else
            {
                return System.IO.Directory.GetCurrentDirectory();
            }
        }

        public virtual PDFStyleItem CreateStyleItem(PDFObjectType type)
        {
            return PDFStyleFactory.CreateStyleItem(type);
        }

        public override void MergeInto(Scryber.Styles.PDFStyle style, IPDFComponent Component, ComponentState state)
        {
            foreach (PDFStyleBase sb in this.Styles)
            {
                sb.MergeInto(style, Component, state);
            }
        }

        public override Scryber.Styles.PDFStyle MatchClass(string classname)
        {
            PDFStyle style = new PDFStyle();

            foreach (PDFStyleBase sb in this.Styles)
            {
                PDFStyle inner = sb.MatchClass(classname);
                if (inner != null && inner.HasItems)
                    inner.MergeInto(style);
            }
            return style;
        }

        public void Init(PDFInitContext context)
        {
            this.OnInit(context);
        }

        protected virtual void Dispose(bool disposing)
        {
            
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        ~PDFStylesDocument()
        {
            this.Dispose(false);
        }

        public string GetFullPath()
        {
            return this.LoadedSource;
        }
        

    }
}
