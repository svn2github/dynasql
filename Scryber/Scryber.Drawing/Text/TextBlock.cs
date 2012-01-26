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

namespace Scryber.Text
{
    internal class TextBlock : TextData
    {
        public override double Width
        {
            get
            {
                if (this._paras == null || this._paras.Count == 0)
                    return 0.0;
                double w = base.Width;
                if (w < 0.0)
                {
                    w = 0.0;
                    foreach (TextParagraph para in this._paras)
                    {
                        w = Math.Max(w, para.Width);
                    }
                    base.Width = w;
                }
                return w;
            }
            set
            {
                base.Width = value;
            }
        }

        public override double Height
        {
            get
            {
                if (this._paras == null || this._paras.Count == 0)
                    return 0.0;
                double h = base.Height;
                if (h < 0.0)
                {
                    h = 0.0;
                    foreach (TextParagraph para in this._paras)
                    {
                        h += para.Height;
                    }
                    base.Height = h;
                }
                return h;
            }
            set
            {
                base.Height = value;
            }
        }

        private PDFTextRenderOptions _renderoptions;

        public PDFTextRenderOptions RootRenderOptions
        {
            get { return _renderoptions; }
        }

        private List<TextParagraph> _paras;

        internal TextParagraph[] Paragraphs
        {
            get { return (_paras == null) ? (new TextParagraph[] { }) : _paras.ToArray(); }
        }

        internal TextBlock(PDFTextRenderOptions renderoptions)
        {
            if (renderoptions == null)
                throw new ArgumentNullException("renderoptions");

            this._renderoptions = renderoptions;
        }

        internal void AddParagraph(TextParagraph para)
        {
            if (this._paras == null)
                this._paras = new List<TextParagraph>(1);
            this._paras.Add(para);
            base.Width = -1;
            base.Height = -1;
        }

    }
}
