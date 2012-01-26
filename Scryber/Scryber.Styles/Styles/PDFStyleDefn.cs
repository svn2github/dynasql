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
using System.Drawing;

namespace Scryber.Styles
{
    /// <summary>
    /// Defines a single style that has the capabilities to be applied to multiple page Components
    /// based upon their ID, Type, or class
    /// </summary>
    [PDFParsableComponent("Style")]
    public class PDFStyleDefn : PDFStyle
    {

        private Type _type;

        /// <summary>
        /// Gets or sets the type of components this definition is applied to.
        /// </summary>
        [PDFAttribute("applied-type")]
        public Type AppliedType
        {
            get { return _type; }
            set { _type = value; }
        }

        private string _class;

        /// <summary>
        /// Gets or sets the style-class that this definition is applied to.
        /// </summary>
        [PDFAttribute("applied-class")]
        public string AppliedClass
        {
            get { return _class; }
            set { _class = value; }
        }

        private string _id;
    
        /// <summary>
        /// Gets or sets the id of teh components this definition is applied to.
        /// </summary>
        [PDFAttribute("applied-id")]
        public string AppliedID
        {
            get { return _id; }
            set { _id = value; }
        }

        private ComponentState _state;

        [PDFAttribute("applied-state")]
        public ComponentState AppliedState
        {
            get { return _state; }
            set { _state = value; }
        }



        public PDFStyleDefn()
            : base(PDFObjectTypes.Style)
        {
        }

        public PDFStyleDefn(Type appliedtype, string appliedid, string appliedclassname)
            : this()
        {
            this._class = appliedclassname;
            this._id = appliedid;
            this._type = appliedtype;
        }

        public override PDFStyle MatchClass(string classname)
        {
            if (this.IsClassNameMatch(classname))
                return this;
            else
                return null;
        }

        public bool IsCatchAllStyle()
        {
            //We are catch all if we have no specific applied options
            return null == this.AppliedType
                        && string.IsNullOrEmpty(this.AppliedClass)
                        && string.IsNullOrEmpty(this.AppliedID);
        }

        public const char ClassNameSeparator = ' ';
        public const StringComparison ClassNameComparer = StringComparison.Ordinal; //CaseSensitive

        /// <summary>
        /// Returns true if the specified class name is considered a match for this definitions class name
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public bool IsClassNameMatch(string classname)
        {
            if (string.IsNullOrEmpty(classname))
                return false;// string.IsNullOrEmpty(this.AppliedClass);

            if (classname.IndexOf(ClassNameSeparator) > 0)
            {
                string[] all = classname.Split(ClassNameSeparator);
                foreach (string name in all)
                {
                    if (!string.IsNullOrEmpty(name) && IsClassNameMatch(name))
                        return true;
                }
                return false;
            }
            else
                return string.Equals(this.AppliedClass, classname, ClassNameComparer);
        }

        public virtual bool IsMatchedTo(IPDFComponent component)
        {
            if (null == component)
                return false;

            if (this.IsCatchAllStyle())
                return true;

            bool match = false;

            if (string.IsNullOrEmpty(this.AppliedID) == false && this.AppliedID.Equals(component.ID))
                match = true;

            if (null != this.AppliedType)
            {
                if (this.AppliedType.IsAssignableFrom(component.GetType()))
                    match = true;
                else
                    match = false;
            }
            if (string.IsNullOrEmpty(this.AppliedClass) == false)
            {
                if ((component is IPDFStyledComponent) && (this.IsClassNameMatch((component as IPDFStyledComponent).StyleClass)))
                    match = true;
                else
                    match = false;
            }

            return match;
        }

        public override void MergeInto(PDFStyle style, IPDFComponent forComponent, ComponentState state)
        {
            if (this.IsMatchedTo(forComponent))
                base.MergeInto(style, forComponent, state);
        }

        
    }

    
}
