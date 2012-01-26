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
using Scryber.Styles;
using Scryber.Resources;
using Scryber.Text;
using Scryber.Drawing;
using Scryber.Native;

namespace Scryber.Components
{
    public abstract class PDFTextBase : PDFVisualComponent, IPDFTextComponent
    {

        #region protected TextFormat BaseFormat {get;set}

        private TextFormat _format;

        protected TextFormat BaseFormat
        {
            get { return _format; }
            set { _format = value; this.ResetTextBlock(); }
        }

        #endregion

        #region protected virtual string BaseText {get;set;}

        private string _text;

        protected virtual string BaseText
        {
            get { return _text; }
            set { _text = value; this.ResetTextBlock(); }
        }

        #endregion

        #region protected PDFTextBlock TextBlock {get;set;} + Interface implementation

        private PDFTextBlock _textops = null;

        protected PDFTextBlock TextBlock
        {
            get { return this._textops; }
            set { this._textops = value; }
        }

        PDFTextBlock IPDFTextComponent.TextBlock
        {
            get { return this.TextBlock; }
            set { this.TextBlock = value; }
        }

        #endregion

        #region protected .ctor(type)

        protected PDFTextBase(PDFObjectType type)
            : base(type)
        {
        }

        #endregion

        #region protected virtual void ResetTextBlock() + Interface implmentation

        protected virtual void ResetTextBlock()
        {
            _textops = null;
        }

        void IPDFTextComponent.ResetTextBlock()
        {
            this.ResetTextBlock();
        }

        #endregion

        #region protected virtual PDFTextReader CreateReader() + Interface implementation

        protected virtual PDFTextReader CreateReader()
        {
            return PDFTextReader.Create(this.BaseText, this.BaseFormat);
        }

        PDFTextReader IPDFTextComponent.CreateReader()
        {
            return this.CreateReader();
        }

        #endregion

        public override PDFObjectRef RenderToPDF(PDFRenderContext context, PDFWriter writer)
        {
            return base.RenderToPDF(context, writer);
        }

        protected override PDFObjectRef DoRenderToPDF(PDFRenderContext context, PDFStyle fullstyle, PDFGraphics graphics, PDFWriter writer)
        {
            PDFTextRenderOptions options = fullstyle.CreateTextStyle();

            if (this.TextBlock == null)
            {
                using (PDFTextReader reader = PDFTextReader.Create(this.BaseText, this.BaseFormat))
                {
                    this.TextBlock = PDFTextBlock.Create(reader, context.Space, graphics, options);
                }
            }

            PDFRect rect = new PDFRect(context.Offset, context.Space);
            if (options.Brush != null)
                graphics.FillText(this.TextBlock, rect);
            if (options.Stroke != null)
                graphics.FillText(this.TextBlock, rect);


            return base.DoRenderToPDF(context, fullstyle, graphics, writer);

        }

        /// <summary>
        /// Removes the extra white-space from an Xml text string due to charriage returns and indents.
        /// </summary>
        /// <param name="text">The text to trim</param>
        /// <returns>The trimmed text</returns>
        public static string GetTrimmedXMLText(string text)
        {
            string[] lines = text.Split('\r','\n');
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrEmpty(line))
                    continue;

                //if this is a second xml line remove the white space and optionally add 
                // a single space to the end of the current string.
                if (i > 0)
                {
                    line = line.TrimStart();

                    if (string.IsNullOrEmpty(line))
                        continue;

                    if(sb.Length > 0 && char.IsWhiteSpace(sb[sb.Length - 1]) == false)
                        sb.Append(" ");
                }
               

                
                sb.Append(line);
            }
            return sb.ToString();
        }

        
    }
}
