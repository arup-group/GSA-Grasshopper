using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using System.Windows.Forms;
using System;
using System.Drawing;

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
        System.Drawing.RectangleF ButtonBounds; // area for button to be displayed
        readonly Action action;

        RectangleF SpacerBounds; // spacer between standard component and button
        readonly string SpacerTxt; // text to be displayed on spacer

        float MinWidth {
            get 
            {
                float num = Math.Max(Math.Max(
                    GH_FontServer.StringWidth(SpacerTxt, GH_FontServer.Small) + 8,
                    GH_FontServer.StringWidth(buttonText, GH_FontServer.Standard))+ 8,
                    90);
                
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
            ButtonBounds = new RectangleF(Bounds.X + 2*s, Bounds.Bottom + h0 + 2*s, Bounds.Width - 2 - 4*s, h1);

            //update component bounds
            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + h1 + 4 * s);
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
                graphics.FillRectangle(UI.Colour.ButtonColor, ButtonBounds);
                graphics.DrawRectangle(pen, ButtonBounds.X, ButtonBounds.Y, ButtonBounds.Width, ButtonBounds.Height);
                graphics.DrawString(buttonText, font, UI.Colour.AnnotationTextBright, ButtonBounds, GH_TextRenderingConstants.CenterCenter);
            }
        }
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.RectangleF rec = ButtonBounds;
                if (rec.Contains(e.CanvasLocation))
                {
                    action();
                    return GH_ObjectResponse.Handled;
                }
            }
            return base.RespondToMouseDown(sender, e);
        }

        protected void FixLayout()
        {
            float width = this.Bounds.Width;
            float num = Math.Max(width, MinWidth);
            float num2 = 0f;
            if (num > this.Bounds.Width)
            {
                num2 = num - this.Bounds.Width;
                this.Bounds = new RectangleF(this.Bounds.X - num2 / 2f, this.Bounds.Y, num, this.Bounds.Height);
            }
            foreach (IGH_Param item in base.Owner.Params.Output)
            {
                PointF pivot = item.Attributes.Pivot;
                RectangleF bounds = item.Attributes.Bounds;
                item.Attributes.Pivot = new PointF(pivot.X, pivot.Y);
                item.Attributes.Bounds = new RectangleF(bounds.Location.X, bounds.Location.Y, bounds.Width + num2 / 2f, bounds.Height);
            }
            foreach (IGH_Param item2 in base.Owner.Params.Input)
            {
                PointF pivot2 = item2.Attributes.Pivot;
                RectangleF bounds2 = item2.Attributes.Bounds;
                item2.Attributes.Pivot = new PointF(pivot2.X - num2 / 2f, pivot2.Y);
                item2.Attributes.Bounds = new RectangleF(bounds2.Location.X - num2 / 2f, bounds2.Location.Y, bounds2.Width + num2 / 2f, bounds2.Height);
            }
        }
    }
}
