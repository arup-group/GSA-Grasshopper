using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using System.Windows.Forms;
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
    public class Button3ComponentUI : GH_ComponentAttributes
    {
        public Button3ComponentUI(GH_Component owner, string display1Text, string display2Text, string display3Text, Action clickHandle1, Action clickHandle2, Action clickHandle3, bool canOpen, string spacerText = "") : base(owner) 
        { 
            button1Text = display1Text;
            button2Text = display2Text;
            button3Text = display3Text;
            SpacerTxt = spacerText; 
            action1 = clickHandle1;
            action2 = clickHandle2;
            action3 = clickHandle3;
            greyoutButton3 = !canOpen;
        }

        readonly string button1Text; // text to be displayed button 1
        readonly string button2Text; // text to be displayed button 2
        readonly string button3Text; // text to be displayed button 3
        System.Drawing.RectangleF Button1Bounds; // area for button1 to be displayed
        System.Drawing.RectangleF Button2Bounds; // area for button2 to be displayed
        System.Drawing.RectangleF Button3Bounds; // area for button3 to be displayed
        readonly Action action1;
        readonly Action action2;
        readonly Action action3;
        readonly bool greyoutButton3;

        bool mouseDown1;
        bool mouseDown2;
        bool mouseDown3;

        RectangleF SpacerBounds; // spacer between standard component and button
        readonly string SpacerTxt; // text to be displayed on spacer

        float MinWidth {
            get 
            {
                List<string> spacers = new List<string>();
                spacers.Add(SpacerTxt);
                float sp = GhSA.UI.ComponentUI.MaxTextWidth(spacers, GH_FontServer.Small);
                List<string> buttons = new List<string>();
                buttons.Add(button1Text);
                buttons.Add(button2Text);
                buttons.Add(button3Text);
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
                h0 = 10;
                SpacerBounds = new RectangleF(Bounds.X, Bounds.Bottom + s/2, Bounds.Width, h0);
            }

            int h1 = 20; // height of button
            // create text box placeholders
            Button1Bounds = new RectangleF(Bounds.X + 2 * s, Bounds.Bottom + h0 + 2 * s, Bounds.Width - 4 * s, h1);
            Button2Bounds = new RectangleF(Bounds.X + 2 * s, Button1Bounds.Bottom + 2 * s, Bounds.Width - 4 * s, h1);
            Button3Bounds = new RectangleF(Bounds.X + 2 * s, Button2Bounds.Bottom + 2 * s, Bounds.Width - 4 * s, h1);
            //update component bounds
            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + 3 * h1 + 8 * s);
        }

        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
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

                // ### button 1 ###
                // Draw button box
                System.Drawing.Drawing2D.GraphicsPath button1 = UI.ButtonsUI.Button.RoundedRect(Button1Bounds, 2);

                Brush normal_colour1 = UI.Colour.ButtonColour;
                Brush hover_colour1 = UI.Colour.HoverButtonColour;
                Brush clicked_colour1 = UI.Colour.ClickedButtonColour;

                Brush butCol1 = (mouseOver1) ? hover_colour1 : normal_colour1;
                graphics.FillPath(mouseDown1 ? clicked_colour1 : butCol1, button1);

                // draw button edge
                Color edgeColor1 = UI.Colour.ButtonBorderColour;
                Color edgeHover1 = UI.Colour.HoverBorderColour;
                Color edgeClick1 = UI.Colour.ClickedBorderColour;
                Color edgeCol1 = (mouseOver1) ? edgeHover1 : edgeColor1;
                Pen pen1 = new Pen(mouseDown1 ? edgeClick1 : edgeCol1)
                {
                    Width = (mouseDown1) ? 0.8f : 0.5f
                };
                graphics.DrawPath(pen1, button1);

                // draw button text
                graphics.DrawString(button1Text, font, UI.Colour.AnnotationTextBright, Button1Bounds, GH_TextRenderingConstants.CenterCenter);


                // ### button 2 ###
                // Draw button box
                System.Drawing.Drawing2D.GraphicsPath button2 = UI.ButtonsUI.Button.RoundedRect(Button2Bounds, 2);

                Brush normal_colour2 = UI.Colour.ButtonColour;
                Brush hover_colour2 = UI.Colour.HoverButtonColour;
                Brush clicked_colour2 = UI.Colour.ClickedButtonColour;

                Brush butCol2 = (mouseOver2) ? hover_colour2 : normal_colour2;
                graphics.FillPath(mouseDown2 ? clicked_colour2 : butCol2, button2);

                // draw button edge
                Color edgeColor2 = UI.Colour.ButtonBorderColour;
                Color edgeHover2 = UI.Colour.HoverBorderColour;
                Color edgeClick2 = UI.Colour.ClickedBorderColour;
                Color edgeCol2 = (mouseOver2) ? edgeHover2 : edgeColor2;
                Pen pen2 = new Pen(mouseDown2 ? edgeClick2 : edgeCol2)
                {
                    Width = (mouseDown2) ? 0.8f : 0.5f
                };
                graphics.DrawPath(pen2, button2);

                // draw button text
                graphics.DrawString(button2Text, font, UI.Colour.AnnotationTextBright, Button2Bounds, GH_TextRenderingConstants.CenterCenter);

                // ### button 3 ###
                // Draw button box
                System.Drawing.Drawing2D.GraphicsPath button3 = UI.ButtonsUI.Button.RoundedRect(Button3Bounds, 2);

                Brush normal_colour3 = UI.Colour.ButtonColour;
                Brush hover_colour3 = UI.Colour.HoverButtonColour;
                Brush clicked_colour3 = UI.Colour.ClickedButtonColour;

                Brush butCol3 = (mouseOver3) ? hover_colour3 : normal_colour3;
                graphics.FillPath(mouseDown3 ? clicked_colour3 : butCol3, button3);

                // draw button edge
                Color edgeColor3 = UI.Colour.ButtonBorderColour;
                Color edgeHover3 = UI.Colour.HoverBorderColour;
                Color edgeClick3 = UI.Colour.ClickedBorderColour;
                Color edgeCol3 = (mouseOver3) ? edgeHover3 : edgeColor3;
                Pen pen3 = new Pen(mouseDown3 ? edgeClick3 : edgeCol3)
                {
                    Width = (mouseDown3) ? 0.8f : 0.5f
                };
                graphics.DrawPath(pen3, button3);

                // draw button text
                graphics.DrawString(button3Text, font, UI.Colour.AnnotationTextBright, Button3Bounds, GH_TextRenderingConstants.CenterCenter);

            }
        }
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.RectangleF rec1 = Button1Bounds;
                if (rec1.Contains(e.CanvasLocation))
                {
                    mouseDown1 = true;
                    mouseDown2 = false;
                    mouseDown3 = false;
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Capture;
                }
                System.Drawing.RectangleF rec2 = Button2Bounds;
                if (rec2.Contains(e.CanvasLocation))
                {
                    mouseDown1 = false;
                    mouseDown2 = true;
                    mouseDown3 = false;
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Capture;
                }
                System.Drawing.RectangleF rec3 = Button3Bounds;
                if (rec3.Contains(e.CanvasLocation))
                {
                    mouseDown1 = false;
                    mouseDown2 = false;
                    mouseDown3 = true; ;
                    Owner.ExpireSolution(true);
                    return GH_ObjectResponse.Capture;
                }
            }
            return base.RespondToMouseDown(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.RectangleF rec1 = Button1Bounds;
                if (rec1.Contains(e.CanvasLocation))
                {
                    if (mouseDown1)
                    {
                        mouseDown1 = false;
                        mouseDown2 = false;
                        mouseDown3 = false;
                        action1();
                        Owner.ExpireSolution(true);
                        return GH_ObjectResponse.Release;
                    }
                }
                System.Drawing.RectangleF rec2 = Button2Bounds;
                if (rec2.Contains(e.CanvasLocation))
                {
                    if (mouseDown2)
                    {
                        mouseDown1 = false;
                        mouseDown2 = false;
                        mouseDown3 = false;
                        action2();
                        Owner.ExpireSolution(true);
                        return GH_ObjectResponse.Release;
                    }
                }
                System.Drawing.RectangleF rec3 = Button3Bounds;
                if (rec3.Contains(e.CanvasLocation))
                {
                    if (mouseDown3)
                    {
                        mouseDown1 = false;
                        mouseDown2 = false;
                        mouseDown3 = false;
                        action3();
                        Owner.ExpireSolution(true);
                        return GH_ObjectResponse.Release;
                    }
                }
            }
            return base.RespondToMouseUp(sender, e);
        }
        bool mouseOver1;
        bool mouseOver2;
        bool mouseOver3;
        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (Button1Bounds.Contains(e.CanvasLocation))
            {
                mouseOver1 = true;
                mouseOver2 = false;
                mouseOver3 = false;
                Owner.OnDisplayExpired(false);
                sender.Cursor = System.Windows.Forms.Cursors.Hand;
                return GH_ObjectResponse.Capture;
            }
            if (Button2Bounds.Contains(e.CanvasLocation))
            {
                mouseOver2 = true;
                mouseOver1 = false;
                mouseOver3 = false;
                Owner.OnDisplayExpired(false);
                sender.Cursor = System.Windows.Forms.Cursors.Hand;
                return GH_ObjectResponse.Capture;
            }
            if (Button3Bounds.Contains(e.CanvasLocation))
            {
                mouseOver3 = true;
                mouseOver1 = false;
                mouseOver2 = false;
                Owner.OnDisplayExpired(false);
                sender.Cursor = System.Windows.Forms.Cursors.Hand;
                return GH_ObjectResponse.Capture;
            }

            if (mouseOver1 | mouseOver2 | mouseOver3)
            {
                mouseOver1 = false;
                mouseOver2 = false;
                mouseOver3 = false;
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
