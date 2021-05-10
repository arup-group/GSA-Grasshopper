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
    /// Class to create custom component UI with 6 check box toggles
    /// 
    /// This class is made for the CreateBool6 component
    /// 
    /// To use this method override CreateAttributes() in component class and set m_attributes = new Bool6ComponentUI(...
    /// </summary>
    public class Bool6ComponentUI : GH_ComponentAttributes
    {
        public Bool6ComponentUI(GH_Component owner, Action<bool,bool,bool,bool,bool,bool> updateHandle, string spacerText, bool resx, bool resy, bool resz, bool resxx, bool resyy, bool reszz) : base(owner)
        {
            x = resx;
            y = resy;
            z = resz;
            xx = resxx;
            yy = resyy;
            zz = reszz;
            update = updateHandle;
            SpacerTxt = spacerText;
        }

        private readonly Action<bool, bool, bool, bool, bool, bool> update;

        #region Custom layout logic
        // restraints set by component
        bool x;
        bool y;
        bool z;
        bool xx;
        bool yy;
        bool zz;

        // text boxes bounds for pre-set restraints
        RectangleF SpacerBounds;
        readonly string SpacerTxt;

        // annotation text bounds
        RectangleF xTxtBounds;
        RectangleF yTxtBounds;
        RectangleF zTxtBounds;
        RectangleF xxTxtBounds;
        RectangleF yyTxtBounds;
        RectangleF zzTxtBounds;

        // bounds for check boxes
        RectangleF xBounds;
        RectangleF yBounds;
        RectangleF zBounds;
        RectangleF xxBounds;
        RectangleF yyBounds;
        RectangleF zzBounds;

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
            if (SpacerTxt != "")
            {
                h0 = 10;
                SpacerBounds = new RectangleF(Bounds.X, Bounds.Bottom + s / 2, Bounds.Width, h0);
            }

            // create annotation (x, y, z, xx, yy, zz) placeholders
            float w = (Bounds.Width - 6 * s) / 6; // width of each check box
            int h2 = 15; // height
            xTxtBounds = new RectangleF(Bounds.X + s, Bounds.Bottom + h0, w, h2);
            yTxtBounds = new RectangleF(Bounds.X + 1.5f * s + w, Bounds.Bottom + h0, w, h2);
            zTxtBounds = new RectangleF(Bounds.X + 2.5f * s + 2 * w, Bounds.Bottom + h0, w, h2);
            xxTxtBounds = new RectangleF(Bounds.X + 3f * s + 3 * w, Bounds.Bottom + h0, w + s, h2);
            yyTxtBounds = new RectangleF(Bounds.X + 4f * s + 4 * w, Bounds.Bottom + h0, w + s, h2);
            zzTxtBounds = new RectangleF(Bounds.X + 5f * s - 1 + 5 * w, Bounds.Bottom + h0, w + s, h2);

            // create check box placeholders
            int h3 = 15;
            xBounds = new RectangleF(Bounds.X + s / 2 + 1, Bounds.Bottom + h0 + h2, w, h3);
            yBounds = new RectangleF(Bounds.X + 1.5f * s + w, Bounds.Bottom + h0 + h2, w, h3);
            zBounds = new RectangleF(Bounds.X + 2.5f * s + 2 * w, Bounds.Bottom + h0 + h2, w, h3);
            xxBounds = new RectangleF(Bounds.X + 3.5f * s + 3 * w, Bounds.Bottom + h0 + h2, w, h3);
            yyBounds = new RectangleF(Bounds.X + 4.5f * s + 4 * w, Bounds.Bottom + h0 + h2, w, h3);
            zzBounds = new RectangleF(Bounds.X + 5.5f * s - 1 + 5 * w, Bounds.Bottom + h0 + h2, w, h3);

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

                if (xTxtBounds.Contains(e.CanvasLocation) | xBounds.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle X");
                    x = !x;
                    update(x, y, z, xx, yy, zz);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (yTxtBounds.Contains(e.CanvasLocation) | yBounds.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle Y");
                    y = !y;
                    update(x, y, z, xx, yy, zz);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (zTxtBounds.Contains(e.CanvasLocation) | zBounds.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle Z");
                    z = !z;
                    update(x, y, z, xx, yy, zz);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (xxTxtBounds.Contains(e.CanvasLocation) | xxBounds.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle XX");
                    xx = !xx;
                    update(x, y, z, xx, yy, zz);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (yyTxtBounds.Contains(e.CanvasLocation) | yyBounds.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle YY");
                    yy = !yy;
                    update(x, y, z, xx, yy, zz);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
                if (zzTxtBounds.Contains(e.CanvasLocation) | zzBounds.Contains(e.CanvasLocation))
                {
                    comp.RecordUndoEvent("Toggle ZZ");
                    zz = !zz;
                    update(x, y, z, xx, yy, zz);
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

            }
            return base.RespondToMouseDown(sender, e);
        }
        bool mouseOver;
        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (xBounds.Contains(e.CanvasLocation) |
                yBounds.Contains(e.CanvasLocation) |
                zBounds.Contains(e.CanvasLocation) |
                xxBounds.Contains(e.CanvasLocation) | 
                yyBounds.Contains(e.CanvasLocation) | 
                zzBounds.Contains(e.CanvasLocation))
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

                Font sml = GH_FontServer.Small;
                // adjust fontsize to high resolution displays
                sml = new Font(sml.FontFamily, sml.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);

                //draw the component
                base.RenderComponentCapsule(canvas, graphics, true, true, true, true, true, true);

                Pen pen = new Pen(borderColour);

                graphics.DrawString(SpacerTxt, sml, annoText, SpacerBounds, GH_TextRenderingConstants.CenterCenter);
                graphics.DrawLine(pen, SpacerBounds.X, SpacerBounds.Y + SpacerBounds.Height / 2, SpacerBounds.X + (SpacerBounds.Width - GH_FontServer.StringWidth(SpacerTxt, sml)) / 2 - 4, SpacerBounds.Y + SpacerBounds.Height / 2);
                graphics.DrawLine(pen, SpacerBounds.X + (SpacerBounds.Width - GH_FontServer.StringWidth(SpacerTxt, sml)) / 2 + GH_FontServer.StringWidth(SpacerTxt, sml) + 4, SpacerBounds.Y + SpacerBounds.Height / 2, SpacerBounds.X + SpacerBounds.Width, SpacerBounds.Y + SpacerBounds.Height / 2);

                graphics.DrawString("x", font, annoText, xTxtBounds, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(xBounds.X + xBounds.Width / 2, xBounds.Y + xBounds.Height / 2), x, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("y", font, annoText, yTxtBounds, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(yBounds.X + yBounds.Width / 2, yBounds.Y + yBounds.Height / 2), y, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("z", font, annoText, zTxtBounds, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(zBounds.X + zBounds.Width / 2, zBounds.Y + zBounds.Height / 2), z, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("xx", font, annoText, xxTxtBounds, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(xxBounds.X + xxBounds.Width / 2, xxBounds.Y + xxBounds.Height / 2), xx, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("yy", font, annoText, yyTxtBounds, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(yyBounds.X + yyBounds.Width / 2, yyBounds.Y + yyBounds.Height / 2), yy, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

                graphics.DrawString("zz", font, annoText, zzTxtBounds, GH_TextRenderingConstants.CenterCenter);
                ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(zzBounds.X + zzBounds.Width / 2, zzBounds.Y + zzBounds.Height / 2), zz, activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);

            }
        }
        #endregion
    }
}
