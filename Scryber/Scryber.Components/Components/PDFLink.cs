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

namespace Scryber.Components
{
    [PDFParsableComponent("Link")]
    public class PDFLink : PDFVisualComponent, IPDFInvisibleContainer
    {


        public PDFLink()
            : this(PDFObjectTypes.Link)
        {
        }

        public PDFLink(PDFObjectType type)
            : base(type)
        {

        }

        #region public PDFVisualComponentList Contents {get;}

        private PDFVisualComponentList _content;

        /// <summary>
        /// Gets the content collection of page Components in this panel
        /// </summary>
        [PDFArray(typeof(PDFVisualComponent))]
        [PDFElement("Content")]
        public PDFVisualComponentList Contents
        {
            get
            {
                if (null == _content)
                {
                    _content = new PDFVisualComponentList(this.InnerContent);
                }
                return _content;
            }
        }

        #endregion


        #region public LinkAction Action {get;set;}

        private LinkAction _action = LinkAction.Undefined;
        /// <summary>
        /// Gets or sets the action type for this link. 
        /// If left undefined then the value will be (attempted to be) determined.
        /// </summary>
        [PDFAttribute("action")]
        public LinkAction Action
        {
            get { return _action; }
            set { _action = value; }
        }

        #endregion

        #region public string Destination {get;set;}

        private string _dest;

        /// <summary>
        /// Gets or sets the destination name or component (prefix with # to look for a component with the specidied ID, otherwise
        /// use the components name or unique id).
        /// </summary>
        [PDFAttribute("destination")]
        public string Destination
        {
            get { return _dest; }
            set { _dest = value; }
        }

        #endregion

        #region public string File {get;set;}

        private string _file;

        /// <summary>
        /// Gets or sets the path to the remote file
        /// </summary>
        [PDFAttribute("file")]
        public string File
        {
            get { return _file; }
            set { _file = value; }
        }

        #endregion

        #region public OutlineFit DestinationFit {get;set;}

        private OutlineFit _destfit = OutlineFit.PageWidth;
        /// <summary>
        /// Gets or sets the fit for the destination (only for local links)
        /// </summary>
        [PDFAttribute("destination-fit")]
        public OutlineFit DestinationFit
        {
            get { return _destfit; }
            set { _destfit = value; }
        }

        #endregion


        #region public bool NewWindow {get;set;}

        private bool _newWindow;

        [PDFAttribute("new-window")]
        public bool NewWindow
        {
            get { return _newWindow; }
            set { _newWindow = value; }
        }

        #endregion

        private string _alt;

        [PDFAttribute("alt")]
        public string AlternateText
        {
            get { return this._alt; }
            set { this._alt = value; }
        }

        protected override void DoRegisterArtefacts(PDFRegistrationContext context, Scryber.Styles.PDFStyle fullstyle)
        {
            LinkAction actiontype = this.Action;

            if (actiontype == LinkAction.Undefined)
                actiontype = this.ResolveActionType(Destination);
            object[] entries = null;
            PDFAction action;
            if (this.IsNamedAction(actiontype))
            {
                PDFNamedAction named = new PDFNamedAction(this, this.Action);
                action = named;
            }
            else if (actiontype == LinkAction.Destination)
            {
                //If we start with a # then we are an ID, otherwise we are a uniqueID or Name
                PDFDestination dest;
                PDFComponent comp;
                if (this.Destination.StartsWith("#"))
                {
                    string id = this.Destination.Substring(1);
                    comp = this.Document.FindAComponentById(id);
                }
                else
                {
                    comp = this.Document.FindAComponentByName(this.Destination);
                }
                if (null == comp)
                    throw RecordAndRaise.NullReference(Errors.CouldNotFindControlWithID, this.Destination);

                dest = new PDFDestination(comp, this.DestinationFit);
                dest.Extension = this.UniqueID;

                context.Document.RegisterCatalogEntry(context, PDFArtefactTypes.Names, dest);
                PDFDestinationAction destact = new PDFDestinationAction(this, actiontype, dest);
                action = destact;
            }
            else if (actiontype == LinkAction.ExternalDestination)
            {
                string name = this.Destination;
                string file = this.File;
                PDFRemoteDestinationAction remote = new PDFRemoteDestinationAction(this, actiontype, file, name);
                remote.NewWindow = this.NewWindow;

                action = remote;
            }
            else
                throw RecordAndRaise.Argument("actiontype");


            entries = this.AddActionAnnotationToChildren(context, action);

            base.DoRegisterArtefacts(context, fullstyle);

            if (null != entries)
            {
                for (int i = entries.Length - 1; i >= 0; i--)
                {
                    context.Page.CloseArtefactEntry(PDFArtefactTypes.Annotations, entries[i]);
                }
                
            }
        }

        private object[] AddActionAnnotationToChildren(PDFRegistrationContext context, PDFAction action)
        {
            List<object> entries = new List<object>();

            foreach (PDFComponent comp in this.Contents)
            {
                PDFLinkAnnotationEntry annot;
                if (comp is IPDFTextComponent)
                {
                    annot = new PDFLinkTextAnnotationEntry(comp, 0);
                }
                else
                {
                    annot = new PDFLinkAnnotationEntry(comp);
                }
                annot.Action = action;

                if (!string.IsNullOrEmpty(this.AlternateText))
                    annot.AlternateText = this.AlternateText;

                object entry = context.Page.RegisterPageEntry(context, PDFArtefactTypes.Annotations, annot);

                if (null != entry)
                    entries.Add(entry);
            }
            return entries.ToArray();
        }

        private bool IsNamedAction(LinkAction action)
        {
            switch (action)
            {
                case LinkAction.NextPage:
                case LinkAction.PrevPage:
                case LinkAction.FirstPage:
                case LinkAction.LastPage:
                    return true;

                default:
                    return false;

            }
        }

        private LinkAction ResolveActionType(string dest)
        {
            if (string.IsNullOrEmpty(this.File))
            {
                return LinkAction.Destination;
            }
            else
                return LinkAction.ExternalDestination;

        }



        //#region IPDFViewPortComponent Members

        //public IPDFLayoutEngine GetEngine(Scryber.Styles.PDFStyleStack styles, IPDFLayoutEngine parent, PDFTraceLog log)
        //{
        //    return new Support.ContainerLayoutEngine(this, styles, parent, log);
        //}

        //#endregion
    }
}
