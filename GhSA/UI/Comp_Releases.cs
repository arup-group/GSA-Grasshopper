using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Attributes;
using GhSA.Components;

namespace GhSA.UI
{
    /// <summary>
    /// Class to create custom component UI with 2 x 6 check box toggles
    /// 
    /// This class is made for the CreateMember1d component
    /// 
    /// To use this method override CreateAttributes() in component class and set m_attributes = new ReleasesComponentUI(...
    /// </summary>
    public class ReleasesComponentUI : GH_ComponentAttributes
    {
        public ReleasesComponentUI(GH_Component owner, Action<bool,bool,bool,bool,bool,bool, bool, bool, bool, bool, bool, bool> updateHandle, 
            string spacerText1, string spacerText2, 
            bool resx1, bool resy1, bool resz1, bool resxx1, bool resyy1, bool reszz1,
            bool resx2, bool resy2, bool resz2, bool resxx2, bool resyy2, bool reszz2) : base(owner)
        {
            x1 = resx1;
            y1 = resy1;
            z1 = resz1;
            xx1 = resxx1;
            yy1 = resyy1;
            zz1 = reszz1;
            x2 = resx2;
            y2 = resy2;
            z2 = resz2;
            xx2 = resxx2;
            yy2 = resyy2;
            zz2 = reszz2;
            update = updateHandle;
            SpacerTxt1 = spacerText1;
            SpacerTxt2 = spacerText2;
        }

        // function that sends back the user input to the component
        readonly Action<bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool, bool> update;

        #region Custom layout logic
        // restraints set by component
        bool x1;
        bool y1;
        bool z1;
        bool xx1;
        bool yy1;
        bool zz1;

        bool x2;
        bool y2;
        bool z2;
        bool xx2;
        bool yy2;
        bool zz2;

        // text boxes bounds for pre-set restraints
        RectangleF SpacerBounds1;
        RectangleF SpacerBounds2;
        readonly string SpacerTxt1;
        readonly string SpacerTxt2;

        // annotation text bounds
        RectangleF xTxtBounds1;
        RectangleF yTxtBounds1;
        RectangleF zTxtBounds1;
        RectangleF xxTxtBounds1;
        RectangleF yyTxtBounds1;
        RectangleF zzTxtBounds1;
        RectangleF xTxtBounds2;
        RectangleF yTxtBounds2;
        RectangleF zTxtBounds2;
        RectangleF xxTxtBounds2;
        RectangleF yyTxtBounds2;
        RectangleF zzTxtBounds2;

        // bounds for check boxes
        RectangleF xBounds1;
        RectangleF yBounds1;
        RectangleF zBounds1;
        RectangleF xxBounds1;
        RectangleF yyBounds1;
        RectangleF zzBounds1;
        RectangleF xBounds2;
        RectangleF yBounds2;
        RectangleF zBounds2;
        RectangleF xxBounds2;
        RectangleF yyBounds2;
        RectangleF zzBounds2;

        float MinWidth
        {
            get
            {
                return 90;
            }
            set
            {
                MinWidth = value;
            }
        }

        protected override void Layout()
        {
            base.Layout();
            
            // Set the actual layout of the component here:

            // first change the width to suit; using max to determine component visualisation style
            FixLayout();

            int s = 3; //spacing to edges and internal between boxes

            //spacer and title
            int h0 = 0;
            if (SpacerTxt1 != "")
            {
                h0 = 10;
                SpacerBounds1 = new RectangleF(Bounds.X, Bounds.Bottom + s / 2, Bounds.Width, h0);
            }

            // create annotation (x, y, z, xx, yy, zz) placeholders
            float w = (Bounds.Width - 6 * s) / 6; // width of each check box
            int h2 = 15; // height
            xTxtBounds1 = new RectangleF(Bounds.X + s, Bounds.Bottom + h0, w, h2);
            yTxtBounds1 = new RectangleF(Bounds.X + 1.5f * s + w, Bounds.Bottom + h0, w, h2);
            zTxtBounds1 = new RectangleF(Bounds.X + 2.5f * s + 2 * w, Bounds.Bottom + h0, w, h2);
            xxTxtBounds1 = new RectangleF(Bounds.X + 3f * s + 3 * w, Bounds.Bottom + h0, w + s, h2);
            yyTxtBounds1 = new RectangleF(Bounds.X + 4f * s + 4 * w, Bounds.Bottom + h0, w + s, h2);
            zzTxtBounds1 = new RectangleF(Bounds.X + 5f * s - 1 + 5 * w, Bounds.Bottom + h0, w + s, h2);

            // create check box placeholders
            int h3 = 15;
            xBounds1 = new RectangleF(Bounds.X + s / 2, Bounds.Bottom + h0 + h2, w, h3);
            yBounds1 = new RectangleF(Bounds.X + 1.5f * s + w, Bounds.Bottom + h0 + h2, w, h3);
            zBounds1 = new RectangleF(Bounds.X + 2.5f * s + 2 * w, Bounds.Bottom + h0 + h2, w, h3);
            xxBounds1 = new RectangleF(Bounds.X + 3.5f * s + 3 * w, Bounds.Bottom + h0 + h2, w, h3);
            yyBounds1 = new RectangleF(Bounds.X + 4.5f * s + 4 * w, Bounds.Bottom + h0 + h2, w, h3);
            zzBounds1 = new RectangleF(Bounds.X + 5.5f * s - 1 + 5 * w, Bounds.Bottom + h0 + h2, w, h3);

            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + h2 + h3 + s);

            h0 = 0;
            if (SpacerTxt2 != "")
            {
                h0 = 10;
                SpacerBounds2 = new RectangleF(Bounds.X, Bounds.Bottom + s / 2, Bounds.Width, h0);
            }

            // create annotation (x, y, z, xx, yy, zz) placeholders
            xTxtBounds2 = new RectangleF(Bounds.X + s, Bounds.Bottom + h0, w, h2);
            yTxtBounds2 = new RectangleF(Bounds.X + 1.5f * s + w, Bounds.Bottom + h0, w, h2);
            zTxtBounds2 = new RectangleF(Bounds.X + 2.5f * s + 2 * w, Bounds.Bottom + h0, w, h2);
            xxTxtBounds2 = new RectangleF(Bounds.X + 3f * s + 3 * w, Bounds.Bottom + h0, w + s, h2);
            yyTxtBounds2 = new RectangleF(Bounds.X + 4f * s + 4 * w, Bounds.Bottom + h0, w + s, h2);
            zzTxtBounds2 = new RectangleF(Bounds.X + 5f * s - 1 + 5 * w, Bounds.Bottom + h0, w + s, h2);

            // create check box placeholders
            xBounds2 = new RectangleF(Bounds.X + s / 2 + 1, Bounds.Bottom + h0 + h2, w, h3);
            yBounds2 = new RectangleF(Bounds.X + 1.5f * s + w, Bounds.Bottom + h0 + h2, w, h3);
            zBounds2 = new RectangleF(Bounds.X + 2.5f * s + 2 * w, Bounds.Bottom + h0 + h2, w, h3);
            xxBounds2 = new RectangleF(Bounds.X + 3.5f * s + 3 * w, Bounds.Bottom + h0 + h2, w, h3);
            yyBounds2 = new RectangleF(Bounds.X + 4.5f * s + 4 * w, Bounds.Bottom + h0 + h2, w, h3);
            zzBounds2 = new RectangleF(Bounds.X + 5.5f * s - 1 + 5 * w, Bounds.Bottom + h0 + h2, w, h3);

            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + h2 + h3 + s);
        }

        protected void FixLayout()
        {
            float width = this.Bounds.Width; // initial component width before UI overrides
            float num = Math.Max(width, MinWidth); // number for new width
            float num2 = 0f; // value for increased width (if any)

            // first check if original component must be widened
            if (num > width)
            {
                num2 = num - width; // change in width
                // update component bounds to new width
                this.Bounds = new RectangleF(
                    this.Bounds.X - num2 / 2f,
                    this.Bounds.Y,
                    num,
                    this.Bounds.Height);
            }

            // secondly update position of input and output parameter text
            // first find the maximum text width of parameters

            foreach (IGH_Param item in base.Owner.Params.Output)
            {
                PointF pivot = item.Attributes.Pivot; // original anchor location of output
                RectangleF bounds = item.Attributes.Bounds; // text box itself
                item.Attributes.Pivot = new PointF(
                    pivot.X + num2 / 2f, // move anchor to the right
                    pivot.Y);
                item.Attributes.Bounds = new RectangleF(
                    bounds.Location.X + num2 / 2f,  // move text box to the right
                    bounds.Location.Y,
                    bounds.Width,
                    bounds.Height);
            }
            // for input params first find the widest input text box as these are right-aligned
            float inputwidth = 0f;
            foreach (IGH_Param item in base.Owner.Params.Input)
            {
                if (inputwidth < item.Attributes.Bounds.Width)
                    inputwidth = item.Attributes.Bounds.Width;
            }
            foreach (IGH_Param item2 in base.Owner.Params.Input)
            {
                PointF pivot2 = item2.Attributes.Pivot; // original anchor location of input
                RectangleF bounds2 = item2.Attributes.Bounds;
                item2.Attributes.Pivot = new PointF(
                    pivot2.X - num2 / 2f + inputwidth, // move to the left, move back by max input width
                    pivot2.Y);
                item2.Attributes.Bounds = new RectangleF(
                     bounds2.Location.X - num2 / 2f,
                     bounds2.Location.Y,
                     bounds2.Width,
                     bounds2.Height);
            }
        }

        #endregion

        #region Custom Mouse handling
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                GH_Component comp = Owner as GH_Component;

                if (xTxtBounds1.Contains(e.CanvasLocation) | xBounds1.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle X1");
                    x1 = !x1;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (yTxtBounds1.Contains(e.CanvasLocation) | yBounds1.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle Y1");
                    y1 = !y1;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (zTxtBounds1.Contains(e.CanvasLocation) | zBounds1.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle Z1");
                    z1 = !z1;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (xxTxtBounds1.Contains(e.CanvasLocation) | xxBounds1.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle XX1");
                    xx1 = !xx1;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (yyTxtBounds1.Contains(e.CanvasLocation) | yyBounds1.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle YY1");
                    yy1 = !yy1;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (zzTxtBounds1.Contains(e.CanvasLocation) | zzBounds1.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle ZZ1");
                    zz1 = !zz1;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

                if (xTxtBounds2.Contains(e.CanvasLocation) | xBounds2.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle X2");
                    x2 = !x2;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (yTxtBounds2.Contains(e.CanvasLocation) | yBounds2.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle Y2");
                    y2 = !y2;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (zTxtBounds2.Contains(e.CanvasLocation) | zBounds2.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle Z2");
                    z2 = !z2;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (xxTxtBounds2.Contains(e.CanvasLocation) | xxBounds2.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle XX2");
                    xx2 = !xx2;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (yyTxtBounds2.Contains(e.CanvasLocation) | yyBounds2.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle YY2");
                    yy2 = !yy2;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (zzTxtBounds2.Contains(e.CanvasLocation) | zzBounds2.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle ZZ2");
                    zz2 = !zz2;
                    update(x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

            }
            return base.RespondToMouseDown(sender, e);
        }
        bool mouseOver;
        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (xBounds1.Contains(e.CanvasLocation) | 
                yBounds1.Contains(e.CanvasLocation) | 
                zBounds1.Contains(e.CanvasLocation) | 
                xxBounds1.Contains(e.CanvasLocation) | 
                yyBounds1.Contains(e.CanvasLocation) | 
                zzBounds1.Contains(e.CanvasLocation) | 
                xBounds2.Contains(e.CanvasLocation) | 
                yBounds2.Contains(e.CanvasLocation) | 
                zBounds2.Contains(e.CanvasLocation) | 
                xxBounds2.Contains(e.CanvasLocation) | 
                yyBounds2.Contains(e.CanvasLocation) | 
                zzBounds2.Contains(e.CanvasLocation))
            {
                mouseOver = true;
                sender.Cursor = System.Windows.Forms.Cursors.Hand;
                return GH_ObjectResponse.Capture;
            }

            if (mouseOver)
            {
                mouseOver = false;
                Grasshopper.Instances.CursorServer.ResetCursor(sender);
                return GH_ObjectResponse.Release; 
            }

            return base.RespondToMouseMove(sender, e);
        }
        #endregion

        #region Custom Render logic
        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                //We need to draw everything outselves.

                Color myColour = UI.Colour.GsaDarkBlue;
                Brush myBrush = new SolidBrush(myColour);

                //Text boxes
                Brush activeFillBrush = myBrush;
                Brush passiveFillBrush = Brushes.LightGray;
                Color borderColour = myColour;
                Color passiveBorder = Color.DarkGray;
                Brush annoText = Brushes.Black;

                Font font = GH_FontServer.Standard;
                int s = 8;
                if (Grasshopper.CentralSettings.CanvasFullNames)
                {
                    s = 10;
                    font = GH_FontServer.Standard;
                }

                // adjust fontsize to high resolution displays
                font = new Font(font.FontFamily, font.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);

                Pen pen = new Pen(borderColour);

                graphics.DrawString(SpacerTxt1, GH_FontServer.Small, annoText, SpacerBounds1, GH_TextRenderingConstants.CenterCenter);
                graphics.DrawLine(pen, SpacerBounds1.X, SpacerBounds1.Y + SpacerBounds1.Height / 2, SpacerBounds1.X + (SpacerBounds1.Width - GH_FontServer.StringWidth(SpacerTxt1, GH_FontServer.Small)) / 2 - 4, SpacerBounds1.Y + SpacerBounds1.Height / 2);
                graphics.DrawLine(pen, SpacerBounds1.X + (SpacerBounds1.Width - GH_FontServer.StringWidth(SpacerTxt1, GH_FontServer.Small)) / 2 + GH_FontServer.StringWidth(SpacerTxt1, GH_FontServer.Small) + 4, SpacerBounds1.Y + SpacerBounds1.Height / 2, SpacerBounds1.X + SpacerBounds1.Width, SpacerBounds1.Y + SpacerBounds1.Height / 2);

                graphics.DrawString("x", font, annoText, xTxtBounds1, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(xBounds1.X + xBounds1.Width / 2, xBounds1.Y + xBounds1.Height / 2), x1, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("y", font, annoText, yTxtBounds1, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(yBounds1.X + yBounds1.Width / 2, yBounds1.Y + yBounds1.Height / 2), y1, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("z", font, annoText, zTxtBounds1, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(zBounds1.X + zBounds1.Width / 2, zBounds1.Y + zBounds1.Height / 2), z1, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("xx", font, annoText, xxTxtBounds1, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(xxBounds1.X + xxBounds1.Width / 2, xxBounds1.Y + xxBounds1.Height / 2), xx1, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("yy", font, annoText, yyTxtBounds1, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(yyBounds1.X + yyBounds1.Width / 2, yyBounds1.Y + yyBounds1.Height / 2), yy1, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("zz", font, annoText, zzTxtBounds1, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(zzBounds1.X + zzBounds1.Width / 2, zzBounds1.Y + zzBounds1.Height / 2), zz1, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString(SpacerTxt2, GH_FontServer.Small, annoText, SpacerBounds2, GH_TextRenderingConstants.CenterCenter);
                graphics.DrawLine(pen, SpacerBounds2.X, SpacerBounds2.Y + SpacerBounds2.Height / 2, SpacerBounds2.X + (SpacerBounds2.Width - GH_FontServer.StringWidth(SpacerTxt2, GH_FontServer.Small)) / 2 - 4, SpacerBounds2.Y + SpacerBounds2.Height / 2);
                graphics.DrawLine(pen, SpacerBounds2.X + (SpacerBounds2.Width - GH_FontServer.StringWidth(SpacerTxt2, GH_FontServer.Small)) / 2 + GH_FontServer.StringWidth(SpacerTxt2, GH_FontServer.Small) + 4, SpacerBounds2.Y + SpacerBounds2.Height / 2, SpacerBounds2.X + SpacerBounds2.Width, SpacerBounds2.Y + SpacerBounds2.Height / 2);

                graphics.DrawString("x", font, annoText, xTxtBounds2, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(xBounds2.X + xBounds2.Width / 2, xBounds2.Y + xBounds2.Height / 2), x2, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("y", font, annoText, yTxtBounds2, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(yBounds2.X + yBounds2.Width / 2, yBounds2.Y + yBounds2.Height / 2), y2, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("z", font, annoText, zTxtBounds2, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(zBounds2.X + zBounds2.Width / 2, zBounds2.Y + zBounds2.Height / 2), z2, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("xx", font, annoText, xxTxtBounds2, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(xxBounds2.X + xxBounds2.Width / 2, xxBounds2.Y + xxBounds2.Height / 2), xx2, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("yy", font, annoText, yyTxtBounds2, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(yyBounds2.X + yyBounds2.Width / 2, yyBounds2.Y + yyBounds2.Height / 2), yy2, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("zz", font, annoText, zzTxtBounds2, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(zzBounds2.X + zzBounds2.Width / 2, zzBounds2.Y + zzBounds2.Height / 2), zz2, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

            }
        }
        #endregion
    }
}
