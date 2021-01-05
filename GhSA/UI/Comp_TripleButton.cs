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
                Pen pen = new Pen(UI.Colour.BorderColour)
                {
                    Width = 0.5f
                };

                Font reg = GH_FontServer.Standard;
                // adjust fontsize to high resolution displays
                reg = new Font(reg.FontFamily, reg.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);

                Font ita = GH_FontServer.StandardItalic;
                // adjust fontsize to high resolution displays
                ita = new Font(ita.FontFamily, ita.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);

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

                // Draw button 1 box
                graphics.FillRectangle(UI.Colour.ButtonColor, Button1Bounds);
                graphics.DrawRectangle(pen, Button1Bounds.X, Button1Bounds.Y, Button1Bounds.Width, Button1Bounds.Height);
                graphics.DrawString(button1Text, reg, UI.Colour.AnnotationTextBright, Button1Bounds, GH_TextRenderingConstants.CenterCenter);

                // Draw button 2 box
                graphics.FillRectangle(UI.Colour.ButtonColor, Button2Bounds);
                graphics.DrawRectangle(pen, Button2Bounds.X, Button2Bounds.Y, Button2Bounds.Width, Button2Bounds.Height);
                graphics.DrawString(button2Text, reg, UI.Colour.AnnotationTextBright, Button2Bounds, GH_TextRenderingConstants.CenterCenter);

                // Draw button 3 box
                Brush button3color = (greyoutButton3) ? UI.Colour.InactiveButtonColor : UI.Colour.ButtonColor;
                graphics.FillRectangle(button3color, Button3Bounds);
                graphics.DrawRectangle(pen, Button3Bounds.X, Button3Bounds.Y, Button3Bounds.Width, Button3Bounds.Height);
                graphics.DrawString(button3Text, 
                    (greyoutButton3) ? ita : reg,
                    (greyoutButton3) ? UI.Colour.AnnotationTextDarkGrey : UI.Colour.AnnotationTextBright, 
                    Button3Bounds, 
                    GH_TextRenderingConstants.CenterCenter);
            }
        }
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.RectangleF rec1 = Button1Bounds;
                if (rec1.Contains(e.CanvasLocation))
                {
                    action1();
                    return GH_ObjectResponse.Handled;
                }
                System.Drawing.RectangleF rec2 = Button2Bounds;
                if (rec2.Contains(e.CanvasLocation))
                {
                    action2();
                    return GH_ObjectResponse.Handled;
                }
                System.Drawing.RectangleF rec3 = Button3Bounds;
                if (rec3.Contains(e.CanvasLocation))
                {
                    action3();
                    return GH_ObjectResponse.Handled;
                }
            }
            return base.RespondToMouseDown(sender, e);
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
