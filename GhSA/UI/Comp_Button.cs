using Grasshopper;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace GhSA.UI
{
    /// <summary>
    /// Class to create custom component UI with a button
    /// 
    /// This class is made for the open and save components
    /// 
    /// To use this method override CreateAttributes() in component class and set m_attributes = new ButtonComponentUI(...
    /// </summary>
    public class ButtonComponentUI : GH_ComponentAttributes
    {
        public ButtonComponentUI(GH_Component owner, string displayText, Action clickHandle, string spacerText = "") : base(owner) { buttonText = displayText; SpacerTxt = spacerText; action = clickHandle; }

        readonly string buttonText; // text to be displayed
        RectangleF ButtonBounds; // area for button to be displayed
        readonly Action action;

        RectangleF SpacerBounds; // spacer between standard component and button
        readonly string SpacerTxt; // text to be displayed on spacer

        bool mouseDown;
        float MinWidth {
            get 
            {
                List<string> spacers = new List<string>();
                spacers.Add(SpacerTxt);
                float sp = GhSA.UI.ComponentUI.MaxTextWidth(spacers, GH_FontServer.Small);
                List<string> buttons = new List<string>();
                buttons.Add(buttonText);
                float bt = GhSA.UI.ComponentUI.MaxTextWidth(buttons, GH_FontServer.Standard);

                float num = Math.Max(Math.Max(sp, bt), 90);
                return num;
            } 
            set {MinWidth = value;}}
        protected override void Layout()
        {
            base.Layout();

            // first change the width to suit; using max to determine component visualisation style
            FixLayout();

            int s = 2; //spacing to edges and internal between boxes

            int h0 = 0;
            //spacer and title
            if (SpacerTxt != "")
            {
                Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height - (CentralSettings.CanvasObjectIcons ? 5 : 0));
                h0 = 10;
                SpacerBounds = new RectangleF(Bounds.X, Bounds.Bottom + s/2, Bounds.Width, h0);
            }

            int h1 = 20; // height of button
            // create text box placeholders
            ButtonBounds = new RectangleF(Bounds.X + 2*s, Bounds.Bottom + h0 + 2*s, Bounds.Width - 4*s, h1);

            //update component bounds
            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + h1 + 4 * s);
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                Pen spacer = new Pen(UI.Colour.SpacerColour);
                
                Font font = GH_FontServer.Standard;
                // adjust fontsize to high resolution displays
                font = new Font(font.FontFamily, font.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);

                Font sml = GH_FontServer.Small;
                // adjust fontsize to high resolution displays
                sml = new Font(sml.FontFamily, sml.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);

                //Draw divider line
                if (SpacerTxt != "")
                {
                    graphics.DrawString(SpacerTxt, sml, UI.Colour.AnnotationTextDark, SpacerBounds, GH_TextRenderingConstants.CenterCenter);
                    graphics.DrawLine(spacer, SpacerBounds.X, SpacerBounds.Y + SpacerBounds.Height / 2, SpacerBounds.X + (SpacerBounds.Width - GH_FontServer.StringWidth(SpacerTxt, sml)) / 2 - 4, SpacerBounds.Y + SpacerBounds.Height / 2);
                    graphics.DrawLine(spacer, SpacerBounds.X + (SpacerBounds.Width - GH_FontServer.StringWidth(SpacerTxt, sml)) / 2 + GH_FontServer.StringWidth(SpacerTxt, sml) + 4, SpacerBounds.Y + SpacerBounds.Height / 2, SpacerBounds.X + SpacerBounds.Width, SpacerBounds.Y + SpacerBounds.Height / 2);
                }

                // Draw button box
                System.Drawing.Drawing2D.GraphicsPath button = UI.ButtonsUI.Button.RoundedRect(ButtonBounds, 2);

                Brush normal_colour = UI.Colour.ButtonColour;
                Brush hover_colour = UI.Colour.HoverButtonColour; 
                Brush clicked_colour = UI.Colour.ClickedButtonColour; 
                
                Brush butCol = (mouseOver) ? hover_colour : normal_colour;
                graphics.FillPath(mouseDown ? clicked_colour : butCol, button);

                // draw button edge
                Color edgeColor = UI.Colour.ButtonBorderColour;
                Color edgeHover = UI.Colour.HoverBorderColour;
                Color edgeClick = UI.Colour.ClickedBorderColour;
                Color edgeCol = (mouseOver) ? edgeHover : edgeColor;
                Pen pen = new Pen(mouseDown ? edgeClick : edgeCol)
                {
                    Width = (mouseDown) ? 0.8f : 0.5f
                };
                graphics.DrawPath(pen, button);

                // draw button glow
                System.Drawing.Drawing2D.GraphicsPath overlay = UI.ButtonsUI.Button.RoundedRect(ButtonBounds, 2, true);
                graphics.FillPath(new SolidBrush(Color.FromArgb(mouseDown ? 0 : mouseOver ? 40 : 60, 255, 255, 255)), overlay);

                // draw button text
                graphics.DrawString(buttonText, font, UI.Colour.AnnotationTextBright, ButtonBounds, GH_TextRenderingConstants.CenterCenter);
            }
        }
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                RectangleF rec = ButtonBounds;
                if (rec.Contains(e.CanvasLocation))
                {
                    mouseDown = true;
                    Owner.OnDisplayExpired(false);
                    return GH_ObjectResponse.Capture;
                }
            }
            return base.RespondToMouseDown(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                RectangleF rec = ButtonBounds;
                if (rec.Contains(e.CanvasLocation))
                {
                    if (mouseDown)
                    {
                        mouseDown = false;
                        mouseOver = false;
                        Owner.OnDisplayExpired(false);
                        action();
//                        Owner.ExpireSolution(true);
                        return GH_ObjectResponse.Release;
                    }
                }
            }
            return base.RespondToMouseUp(sender, e);
        }
        bool mouseOver;
        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (ButtonBounds.Contains(e.CanvasLocation))
            {
                mouseOver = true;
                Owner.OnDisplayExpired(false);
                sender.Cursor = System.Windows.Forms.Cursors.Hand;
                return GH_ObjectResponse.Capture;
            }

            if (mouseOver)
            {
                mouseOver = false;
                Owner.OnDisplayExpired(false);
                Grasshopper.Instances.CursorServer.ResetCursor(sender);
                return GH_ObjectResponse.Release;
            }

            return base.RespondToMouseMove(sender, e);
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
    }
}
