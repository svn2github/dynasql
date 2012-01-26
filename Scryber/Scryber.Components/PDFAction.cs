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
using Scryber.Drawing;
using Scryber.Native;

namespace Scryber
{
    public abstract class PDFAction
    {
        public PDFComponent Component { get; private set; }

        public LinkAction ActionType { get; private set; }

        

        

        protected PDFAction(PDFComponent component, LinkAction action)
        {
            this.Component = component;
            this.ActionType = action;
        }

        public abstract PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer);

        
    }

    internal class PDFNamedAction : PDFAction
    {

        

        public PDFNamedAction(PDFComponent component, LinkAction action)
            : base(component, action)
        {
            
        }


        public override PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            writer.BeginDictionary();
            writer.WriteDictionaryNameEntry("Type", "Action");
            writer.WriteDictionaryNameEntry("S", "Named");
            writer.WriteDictionaryNameEntry("N", this.GetNameForAction(this.ActionType));
            writer.EndDictionary();
            return null;
        }

        private string GetNameForAction(LinkAction linkAction)
        {
            string name;
            switch (linkAction)
            {
                case LinkAction.Undefined:
                case LinkAction.Uri:
                case LinkAction.Destination:
                case LinkAction.ExternalDestination:
                case LinkAction.Launch:
                    throw RecordAndRaise.ArgumentOutOfRange("linkAction");

                case LinkAction.NextPage:
                    name = "NextPage";
                    break;
                case LinkAction.PrevPage:
                    name = "PrevPage";
                    break;
                case LinkAction.FirstPage:
                    name = "FirstPage";
                    break;
                case LinkAction.LastPage:
                    name = "LastPage";
                    break;
                default:
                    throw RecordAndRaise.ArgumentOutOfRange("linkAction");
            }
            return name;
        }
    }

    public class PDFDestinationAction : PDFAction
    {
        public PDFDestination Destination { get; set; }

        public PDFDestinationAction(PDFComponent owner, LinkAction action, PDFDestination destination)
            : base(owner, action)
        {
            this.Destination = destination;
        }

        public override PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            writer.BeginDictionary();
            writer.WriteDictionaryNameEntry("Type", "Action");
            writer.WriteDictionaryNameEntry("S", "GoTo");

            //The destination should be registered with the Name Dictionary
            //so we use the full name here to refer to it.
            writer.WriteDictionaryStringEntry("D", this.Destination.FullName);
            writer.EndDictionaryEntry();

            writer.EndDictionary();
            return null;
        }
    }

    public class PDFRemoteDestinationAction : PDFAction
    {
        public string File { get; set; }

        public string DestinationName { get; set; }

        public bool NewWindow { get; set; }

        public PDFRemoteDestinationAction(PDFComponent owner, LinkAction action, string file, string name)
            : base(owner, action)
        {
            this.File = file;
            this.DestinationName = name;
        }

        public override PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            writer.BeginDictionary();
            writer.WriteDictionaryNameEntry("Type", "Action");
            writer.WriteDictionaryNameEntry("S", "GoToR");
            writer.WriteDictionaryStringEntry("F", this.File);
            if (!string.IsNullOrEmpty(this.DestinationName))
                writer.WriteDictionaryStringEntry("D", this.DestinationName);
            if (this.NewWindow)
                writer.WriteDictionaryBooleanEntry("NewWindow", this.NewWindow);
            writer.EndDictionary();
            return null;
        }
    }
}
