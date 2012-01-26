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
using Scryber.Text;
using Scryber.Native;
using System.Drawing;
using Scryber.Resources;

namespace Scryber.Drawing
{
    public delegate PDFTextRenderOptions ResolveStyleCallback(string styleClass);

    public partial class PDFGraphics
    {
        #region protected SetCurrentFont(PDFFont font)

        public void SetCurrentFont(PDFFont font)
        {
            PDFResource rsrc = this.Container.Document.GetResource(PDFResource.FontDefnResourceType, font.FullName, true);
            if (null == rsrc)
                throw new NullReferenceException(String.Format(Errors.FontNotFound, font.FullName));

            PDFName name = this.Container.Register(rsrc);

            this.Writer.WriteOpCodeS(PDFOpCode.TxtFont, name, font.Size.RealValue);
        }

        #endregion

        #region protected RenderCurrentTextMode(TextRenderMode mode)

        protected void RenderCurrentTextMode(TextRenderMode mode)
        {
            this.Writer.WriteOpCodeS(PDFOpCode.TxtRenderMode, new PDFNumber((int)mode));
        }

        #endregion


        #region MeasureBlock(PDFTextReader reader, Scryber.Text.PDFTextRenderOptions options, PDFSize size) + 1 Overload

        public PDFTextBlock MeasureBlock(string text, PDFBrush brush, PDFFont font)
        {
            return MeasureBlock(text, brush, font, new PDFSize(double.MaxValue, double.MaxValue));
        }

        public PDFTextBlock MeasureBlock(string text, PDFBrush brush, PDFFont font, PDFSize size)
        {
            PDFTextRenderOptions options = new PDFTextRenderOptions();
            options.Font = font;
            options.Brush = brush;
            return MeasureBlock(text, options, size);
        }

        public PDFTextBlock MeasureBlock(string text, PDFTextRenderOptions options, PDFSize size)
        {
            using (PDFTextReader reader = PDFTextReader.Create(text,TextFormat.Plain))
            {
                return MeasureBlock(reader, options, size);
            }
        }
        public PDFTextBlock MeasureBlock(PDFTextReader reader, PDFTextRenderOptions options, PDFSize size)
        {
            PDFTextBlock block = PDFTextBlock.Create(reader, size, this, options);
            block.MeasuredSize = size;
            return block;
        }

        internal PDFTextBlock MeasureBlock(PDFTextReader reader, PDFTextRenderOptions options, PDFSize size, ResolveStyleCallback callback)
        {
            PDFTextBlock block = PDFTextBlock.Create(reader, size, this, options);
            block.MeasuredSize = size;
            return block;
        }


        #endregion






        StringFormat _defaultformat = InitFormat();

        private static StringFormat InitFormat()
        {
            StringFormat format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces);
            format.Trimming = StringTrimming.Word;
            return format;
        }
        

        private double GetLineLeft(PDFSize size, PDFTextRenderOptions options)
        {
            if (options.Layout != null && options.Layout.FirstLineInset != PDFUnit.Empty)
                return options.Layout.FirstLineInset.ToPoints().Value;
            else
                return 0.0;
        }

        private System.Drawing.Font GetSystemFont(PDFTextRenderOptions options)
        {
            if (options == null || options.Font == null)
                return null;
            else
                return options.Font.GetSystemFont();
            
        }

        private PDFTextRenderOptions ResolveTextClass(string classname)
        {
            throw new NotImplementedException("ResolveTextClass");
        }

        private void ReleaseTextRenderOptions(PDFTextRenderOptions options, PDFRect bounds)
        {
            if (options.Brush != null)
                options.Brush.ReleaseGraphics(this, bounds);
            if (options.Stroke != null)
                options.Stroke.ReleaseGraphics(this, bounds);
        }

        private void SetTextRenderOptions(PDFTextRenderOptions options, PDFRect bounds)
        {
            
            TextRenderMode mode = TextRenderMode.NoOp;
            if (options == null)
                throw new ArgumentNullException("options");
            
            if (options.Brush != null)
            {
                mode = TextRenderMode.Fill;
                options.Brush.SetUpGraphics(this, bounds);
            }
            if (options.Stroke != null)
            {
                if (mode == TextRenderMode.Fill)
                    mode = TextRenderMode.FillAndStroke;
                else
                    mode = TextRenderMode.Stroke;

                options.Stroke.SetUpGraphics(this, bounds);
            }
            if (options.Font != null)
                this.SetCurrentFont(options.Font);

            if (options.Layout != null)
            {
                if (options.Layout.Leading != PDFUnit.Empty)
                    this.Writer.WriteOpCodeS(PDFOpCode.TxtLeading, options.Layout.Leading.RealValue);
            }
            this.Writer.WriteOpCodeS(PDFOpCode.TxtRenderMode, (PDFNumber)(int)mode);
        }

        public void FillText(PDFTextBlock block, PDFPoint location)
        {
            this.FillText(block, new PDFRect(location, block.Size));
        }

        public void FillText(PDFTextBlock block, PDFRect bounds)
        {
            ResolveStyleCallback callback = new ResolveStyleCallback(this.ResolveTextClass);
            this.FillText(block, bounds, callback);
        }

        public void FillText(PDFTextBlock block, PDFRect bounds, ResolveStyleCallback callback)
        {
            this.DoFillText(block.InnerBlock, bounds, callback);
        }




        private void DoFillText(TextBlock block, PDFRect bounds, ResolveStyleCallback callback)
        {
            PDFPoint pt = bounds.Location;

            this.SaveGraphicsState();
            this.Writer.WriteOpCodeS(PDFOpCode.TxtBegin);

            try
            {
                
                this.SetTextRenderOptions(block.RootRenderOptions, bounds);

                List<int> linecounts = new List<int>();

                
                PDFUnit inset;
                int totallinecount = 0;
                int lastparalinecount = 0;
                int paracount = 0;
                int paralinecount = 0;
                PDFReal lastlinewidth = PDFReal.Zero;

                HorizontalAlignment halign = HorizontalAlignment.Left;
                if (block.RootRenderOptions != null && block.RootRenderOptions.Layout != null)
                    halign = block.RootRenderOptions.Layout.HAlign;

                foreach (Text.TextParagraph para in block.Paragraphs)
                {
                    
                    //TODO:take account of text alignment
                    if(linecounts.Count == 0)
                        inset = para.FirstLineInset;
                    else if (linecounts[linecounts.Count-1] == 1)
                        inset = PDFUnit.Zero;
                    else
                        inset = para.FirstLineInset;
                    
                    
                    if (para.LineCount > 0)
                    {
                        paralinecount = 0;
                        
                        

                        foreach (Text.TextLine line in para.Lines)
                        {
                            
                            PDFUnit offset = PDFUnit.Zero;
                            //TODO: Text: Change the offset to the base line offset rather thant the total height
                            
                            if (totallinecount == 0)
                                pt.Y += (PDFUnit)line.AscentHeight;

                            this.BeginNewLine(totallinecount, paralinecount, lastparalinecount, bounds, inset.RealValue, pt, (PDFReal)line.Height, (PDFReal)line.Width, lastlinewidth, halign);

                            foreach (Text.TextSpan span in line.TextSpans)
                            {
                                if (span.Font != null)
                                {
                                    this.SetCurrentFont(span.Font);
                                }
                                this.FillSpan(span);
                                offset += (PDFUnit)span.Width;
                            }
                            paralinecount++;
                            totallinecount++;
                            lastlinewidth = (PDFReal)line.Width;

                        }
                        lastparalinecount = paralinecount;
                    }
                    paracount++;

                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(String.Format(Errors.CouldNotFillText, ex.Message), ex);
            }
            finally
            {
                this.Writer.WriteOpCodeS(PDFOpCode.TxtEnd);
                this.ReleaseTextRenderOptions(block.RootRenderOptions, bounds);
                this.RestoreGraphicsState();
            }
        }

        private void BeginNewLine(int totallinecount, int paralinecount, int lastparalinecount, PDFRect bounds, PDFReal inset, PDFPoint pt, PDFReal lineheight, PDFReal linewidth, PDFReal lastlinewidth, HorizontalAlignment halign)
        {
            if (halign == HorizontalAlignment.Left)
            {
                if (totallinecount == 0)
                    this.Writer.WriteOpCodeS(PDFOpCode.TxtMoveNextOffset, GetXPosition(pt.X.RealValue + inset), GetYPosition(pt.Y));

                else if (lastparalinecount > 1 && paralinecount == 0)
                    this.Writer.WriteOpCodeS(PDFOpCode.TxtMoveNextOffset, GetXOffset(inset), GetYOffset(lineheight));

                else if (paralinecount == 1)
                    this.Writer.WriteOpCodeS(PDFOpCode.TxtMoveNextOffset, GetXOffset(PDFReal.Zero - inset), GetYOffset(lineheight));

                else
                    this.Writer.WriteOpCodeS(PDFOpCode.TxtMoveNextOffset, GetXOffset(PDFReal.Zero), GetYOffset(lineheight));
            }
            else if (halign == HorizontalAlignment.Right)
            {
                if (totallinecount == 0)
                    this.Writer.WriteOpCodeS(PDFOpCode.TxtMoveNextOffset, GetXPosition((pt.X.RealValue + bounds.Width.RealValue) - linewidth), GetYPosition(pt.Y));
                else
                    this.Writer.WriteOpCodeS(PDFOpCode.TxtMoveNextOffset, GetXPosition(lastlinewidth - linewidth), GetYOffset(lineheight));
            }
            else if (halign == HorizontalAlignment.Center)
            {
                double middle = pt.X.RealValue.Value + (bounds.Width.RealValue.Value/2);
                double start = middle - (linewidth.Value/2);
                this.Writer.WriteOpCodeS(PDFOpCode.TxtMoveNextOffset, GetXPosition(start),GetYPosition(pt.Y));
            }

        }

        
        
        private void FillSpan(TextSpan span)
        {
            string chars = span.Characters;
            
            if (chars.IndexOfAny(TextConst.EscapeChars) > -1)
            {
                for (int i = 0; i < TextConst.EscapeStrings.Length; i++)
                {
                    chars = chars.Replace(TextConst.EscapeStrings[i], TextConst.ReplaceStrings[i]);
                }
            }
            
            this.Writer.WriteOpCodeS(PDFOpCode.TxtPaint, (PDFString)chars);
        }

        

    }
}
