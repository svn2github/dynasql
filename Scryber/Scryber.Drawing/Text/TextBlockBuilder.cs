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
using Scryber.Drawing;
using Scryber;
using System.Drawing;

namespace Scryber.Text
{

    class TextBlockBuilder : IDisposable
    {

        //
        // Inner Classes
        //

        #region private class TextState

        /// <summary>
        /// Encapsulates the current state of the Text Builder
        /// </summary>
        private class TextState
        {
            private PDFFont _nextspanfont;

            private TextBlock _block;
            public TextBlock Block
            {
                get { return _block; }
            }

            private TextLine _currentline;

            public TextLine CurrentLine
            {
                get { return this._currentline; }
                set { this._currentline = value; }
            }

            private TextParagraph _currPara;

            public TextParagraph CurrentParagraph
            {
                get { return this._currPara; }
                set { this._currPara = value; }
            }

            private TextSpan _currSpan;

            public TextSpan CurrentSpan
            {
                get { return this._currSpan; }
                set { this._currSpan = value; }
            }

            public TextState(TextBlock block)
            {
                this._block = block;
            }

            public void CloseCurrentLine(double leading, double assent)
            {
                if (CurrentLine == null)
                    this.BeginLine(leading, assent);

                if (this.CurrentParagraph == null)
                    throw new NullReferenceException("There is no current paragraph");

                this.CurrentParagraph.AddLine(this.CurrentLine);
                this.CurrentLine = null;
            }

            public void BeginLine(double leading, double ascent)
            {
                TextLine line = new TextLine();
                line.Height = leading;
                line.AscentHeight = ascent;
                line.Width = 0.0;
                this.CurrentLine = line;
            }

            public void BeginParagraph(PDFUnit firstlineinset)
            {
                TextParagraph para = new TextParagraph();
                para.FirstLineInset = firstlineinset;
                this.CurrentParagraph = para;
            }


            internal void AppendSpan(TextSpan span)
            {
                if (this.CurrentLine == null)
                    throw new ArgumentNullException("There is no current line to append a span to");
                if (null != _nextspanfont)
                {
                    span.Font = _nextspanfont;
                    _nextspanfont = null;
                }
                this.CurrentLine.AddSpan(span);
            }

            internal void AppendFont(PDFFont font)
            {
                this._nextspanfont = font;
            }

            internal bool HasCurrentLine()
            {
                return this.CurrentLine != null;
            }

            internal bool HasCurrentParagraph()
            {
                return this.CurrentParagraph != null;
            }

            internal void CloseBlock(double leading, double ascent)
            {
                if (this.CurrentParagraph != null)
                    this.CloseCurrentParagraph(leading, ascent);
            }

            internal void CloseCurrentParagraph(double leading, double ascent)
            {
                if (this.CurrentParagraph == null)
                    return;

                if (this.HasCurrentLine())
                    this.CloseCurrentLine(leading, ascent);
                this.Block.AddParagraph(this.CurrentParagraph);
                this.CurrentParagraph = null;
            }

            internal void BeginBlock()
            {
                
            }
        }

        #endregion

        //
        // Reader Properties
        //

        #region protected PDFTextReader Reader {get;}

        private PDFTextReader _reader;
        /// <summary>
        /// Gets the Reader this TextBlockBuilder uses to extract the path data
        /// </summary>
        protected PDFTextReader Reader
        {
            get { return _reader; }
        }

        #endregion

        #region protected PDFGraphics Graphics {get;}

        private PDFGraphics _g;

        /// <summary>
        /// Gets the current graphics associated with the TextBlockBuilder
        /// </summary>
        protected PDFGraphics Graphics
        {
            get { return _g; }
        }

        #endregion

        #region protected PDFSize AvailableSize {get;}

        private PDFSize _size;

        /// <summary>
        /// Gets the available size for the ultimate TextBlock
        /// </summary>
        protected PDFSize AvailableSize
        {
            get { return _size; }
            set { this._size = value; }
        }

        #endregion

        #region public PDFSize OriginalSpace {get;}

        private PDFSize _origspace;
        /// <summary>
        /// Gets the Original space available in the block
        /// </summary>
        public PDFSize OriginalSpace
        {
            get { return _origspace; }
        }

        #endregion

        #region protected bool Continue {get;}

        private bool _continue;

        /// <summary>
        /// Gets a flag to identify if the building process should continue
        /// </summary>
        public bool Continue
        {
            get { return _continue; }
            private set { this._continue = value; }
        }

        #endregion

        #region public bool OwnsReader {get;}

        private bool _ownsreader;

        /// <summary>
        /// Gets the flag to identify if this TextBlockBuilder owns the PDFTextReader
        /// </summary>
        public bool OwnsReader
        {
            get { return _ownsreader; }
        }

        #endregion

        #region protected virtual PDFFont CurrentFont {get;}

        /// <summary>
        /// Gets the current font
        /// </summary>
        protected virtual PDFFont CurrentFont
        {
            get
            {
                return this.RenderStack.Peek().Font;
            }
        }

        #endregion

        #region private Graphics SystemGrapics {get;}

        /// <summary>
        /// Gets the current System.Drawing.Graphics instance
        /// </summary>
        private Graphics SystemGrapics
        {
            get
            {
                return this.Graphics.CreateWinGraphics();
            }
        }

        #endregion

        #region private Font SystemFont {get;}

        /// <summary>
        /// Gets the System.Font for the CurrentFont
        /// </summary>
        private Font SystemFont
        {
            get
            {
                return this.CurrentFont.GetSystemFont();
            }
        }

        #endregion

        #region private PDFFontMetrics CurrentFontMetrics {get;}

        /// <summary>
        /// Gets the current font metrics for the CurrentFont
        /// </summary>
        private PDFFontMetrics CurrentFontMetrics
        {
            get
            {
                return this.CurrentFont.FontMetrics;
            }
        }

        #endregion

        #region public TextState CurrentState {get;}

        private TextState _currstate;

        /// <summary>
        /// Gets the Current TextState for the Builder
        /// </summary>
        private TextState CurrentState
        {
            get { return _currstate; }
        }

        #endregion

        #region protected Stack<PDFTextRenderOptions> RenderStack

        private Stack<PDFTextRenderOptions> _renderstack;
        
        /// <summary>
        /// Gets the stack of PDFTextRenderOptions 
        /// </summary>
        protected Stack<PDFTextRenderOptions> RenderStack
        {
            get { return this._renderstack; }
        }

        #endregion

        #region protected PDFUnit ParaStartInset {get;}

        /// <summary>
        /// Gets the inset for the first line of a paragraph
        /// </summary>
        protected PDFUnit ParaStartInset
        {
            get
            {
                PDFUnit inset = (PDFUnit)0;
                PDFTextLayout layout = this.RenderStack.Peek().Layout;
                if (layout != null)
                    inset = layout.FirstLineInset;
                return inset;
            }
        }
        #endregion

        #region protected bool WrapText {get;}

        /// <summary>
        /// Gets a flag to identify if the text should be wrapped or truncated.
        /// </summary>
        protected WordWrap WrapText
        {
            get
            {
                WordWrap wrap = Scryber.Text.PDFTextLayout.DefaultWrapText;
                PDFTextLayout layout = this.RenderStack.Peek().Layout;
                if (layout != null)
                    wrap = layout.WrapText;
                return wrap;
            }
        }

        #endregion

        #region protected double LineLeading {get;}
        /// <summary>
        /// Gets the current Leading for the font (or the leading explicitly defined on the style)
        /// </summary>
        protected double LineLeading
        {
            get
            {
                double leading = this.CurrentFontMetrics.LineHeight;
                PDFTextLayout layout = this.RenderStack.Peek().Layout;
                if (layout != null && layout.Leading > PDFUnit.Empty)
                    leading = layout.Leading.RealValue.Value;
                return leading;
            }
        }

        #endregion


        #region protected StringFormat SystemFormat {get;}

        private StringFormat _sysformat;
        /// <summary>
        /// Gets the standard string format for the measurement of text
        /// </summary>
        protected StringFormat SystemFormat
        {
            get
            {
                return this._sysformat;
            }
        }

        #endregion

        //
        // constructor
        //

        #region .ctor

        public TextBlockBuilder(PDFTextReader reader, PDFSize size, PDFGraphics graphics)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            

            //this._currstate = new TextState(forblock);
            
            this._ownsreader = false;
            this._reader = reader;

            this._g = graphics;

            this._renderstack = new Stack<PDFTextRenderOptions>();
            //this._renderstack.Push(forblock.RootRenderOptions);
            this._size = size;
            this._origspace = size;

            this._sysformat = (StringFormat)StringFormat.GenericTypographic.Clone();
            this._sysformat.Trimming = StringTrimming.Word;
            this._sysformat.FormatFlags |= StringFormatFlags.NoWrap;
            this._continue = true;

        }

        #endregion

        //
        // protected and public methods
        //

        #region protected virtual bool ReadNext()

        protected virtual bool ReadNext()
        {
            if (this.Continue == false)
                throw new InvalidOperationException("Cannot read more when continue is false");
            this.Continue = this.Reader.Read();
            return this.Continue;
        }

        #endregion

        #region public void BuildBlock(TextBlock block)

        /// <summary>
        /// Builds the block structure based upon the current Reader TextOps
        /// </summary>
        public void BuildBlock(TextBlock block)
        {
            if (block == null)
                throw new ArgumentNullException("block");
            if (block.RootRenderOptions == null)
                throw new ArgumentNullException("block.RootRenderOptions");
            
            this._currstate = new TextState(block);
            this.RenderStack.Push(block.RootRenderOptions);
            this.CurrentState.BeginBlock();
            while (this.ReadNext())
            {
                PDFTextOpType type = this.Reader.OpType;
                PDFTextFontOp fop;
                switch (type)
                {
                    case PDFTextOpType.LineBreak:
                        this.EndCurrentPara();
                        break;

                    case PDFTextOpType.StyleStart:
                        fop = (PDFTextFontOp)this.Reader.Value;
                        ApplyFontStyle(fop.FontStyle);  
                        break;
                    case PDFTextOpType.StyleEnd:
                        fop = (PDFTextFontOp)this.Reader.Value;
                        RetractFontStyle(fop.FontStyle); 
                        break;
                    case PDFTextOpType.TextContent:
                        this.AppendTextContent(this.Reader.Value);
                        break;
                    case PDFTextOpType.BeginBlock:

                        break;
                    case PDFTextOpType.EndBlock:

                        break;
                    case PDFTextOpType.ClassStart:

                        break;
                    case PDFTextOpType.ClassEnd:

                        break;
                    case PDFTextOpType.None:
                    case PDFTextOpType.Unknown:
                    default:
                        break;
                }
            }
            this.CurrentState.CloseBlock(this.LineLeading,this.CurrentFontMetrics.Ascent);
        }

        #endregion

        private void ApplyFontStyle(string style)
        {
            Scryber.Drawing.FontStyle fstyle = GetFontStyleForName(style);
            PDFTextRenderOptions opts = this.RenderStack.Peek();
            opts = opts.Clone();
            opts.Font = new PDFFont(opts.Font, opts.Font.FontStyle | fstyle);
            this.CurrentState.AppendFont(opts.Font);
            this.RenderStack.Push(opts);
        }

        private Scryber.Drawing.FontStyle GetFontStyleForName(string name)
        {
            name = name.ToLower();
            switch (name)
            {
                case("b"):
                    return Scryber.Drawing.FontStyle.Bold;

                case("i"):
                    return Scryber.Drawing.FontStyle.Italic;

                case("sup"):
                    return Scryber.Drawing.FontStyle.Superscript;

                case("sub"):
                    return Scryber.Drawing.FontStyle.Subscript;

                default:
                    return Scryber.Drawing.FontStyle.Regular;
            }
        }

        private void RetractFontStyle(string style)
        {
            PDFTextRenderOptions opts = this.RenderStack.Pop();
            this.CurrentState.AppendFont(this.RenderStack.Peek().Font);
        }

        private void EndCurrentPara()
        {
            if (this.CurrentState.HasCurrentLine())
                this.EndCurrentLine();
            this.CurrentState.CloseCurrentParagraph(this.LineLeading, this.CurrentFontMetrics.Ascent);

        }

        private void AppendTextContent(PDFTextOp op)
        {
            string s = (op as PDFTextDrawOp).Characters;
            AppendTextConent(s);
        }

        private void AppendTextConent(string s)
        {
            if (this.CurrentState.HasCurrentParagraph() == false)
                this.CurrentState.BeginParagraph(this.ParaStartInset);
            if (this.CurrentState.HasCurrentLine() == false)
                this.CurrentState.BeginLine(this.LineLeading, this.CurrentFontMetrics.Ascent);
            
            while (s.Length > 0)
            {
                int charsfitted;
                SizeF sf = this.MeasureString(s, this.CurrentState.CurrentParagraph.LineCount == 0 && this.CurrentState.CurrentLine.SpanCount == 0, out charsfitted);
                string fitted;
                TextSpan span;

                if (charsfitted >= s.Length)
                {
                    fitted = s;
                    s = string.Empty;
                    span = new TextSpan(fitted);
                }
                //TODO:Word wrapping options rather than just no-wrap
                else if (this.WrapText != WordWrap.NoWrap)
                {
                    fitted = s.Substring(0, charsfitted).TrimEnd(' ');
                    s = s.Substring(charsfitted);
                    s = s.TrimStart(' ');
                    span = new TextSpan(fitted);
                }
                else
                {
                    fitted = s.Substring(0, charsfitted);
                    s = s.Substring(charsfitted);
                    span = new TextSpan(fitted);
                }

                span.Width = sf.Width;
                span.Height = sf.Height;
                this.CurrentState.AppendSpan(span);

                
                if (s != string.Empty)
                {
                    double h = this.CurrentState.CurrentLine.Height;
                    this.CurrentState.CloseCurrentLine(this.LineLeading,this.CurrentFontMetrics.Ascent);
                    this.CurrentState.BeginLine(this.LineLeading, this.CurrentFontMetrics.Ascent);
                    this.AvailableSize =  new PDFSize(this.OriginalSpace.Width,this.AvailableSize.Height - (PDFUnit)this.LineLeading);
                }
                else
                    this.AvailableSize = new PDFSize(this.AvailableSize.Width - (PDFUnit)span.Width, this.AvailableSize.Height);

            }
        }

        private SizeF MeasureString(string s, bool isfirst, out int charsfitted)
        {
            SizeF sf = new SizeF((float)this.AvailableSize.Width.RealValue.Value, (float)this.AvailableSize.Height.RealValue.Value);
            if (isfirst)
                sf.Width -= (float)this.CurrentState.CurrentParagraph.FirstLineInset.RealValue.Value;
            int linesfitted;
            sf = this.SystemGrapics.MeasureString(s, this.SystemFont, sf, this.SystemFormat, out charsfitted, out linesfitted);
            return sf;
        }

        private void EndCurrentLine()
        {
            this.CurrentState.CloseCurrentLine(this.LineLeading, this.CurrentFontMetrics.Ascent);
            this.AvailableSize = new PDFSize(this.OriginalSpace.Width, this.AvailableSize.Height);
        }

        private static bool OpIsSupported(PDFTextOpType optype)
        {
            return optype != PDFTextOpType.Unknown && optype != PDFTextOpType.None;
        }

        #region IDisposable

        public void Dispose()
        {
            if (this.Reader != null && this.OwnsReader)
                this.Reader.Dispose();

            this._reader = null;
        }

        #endregion
    }
}
