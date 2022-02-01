using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace GhSA.UI
{
    /// <summary>
    /// Class to create custom component UI boolean toggle boxes
    /// 
    /// To use this method override CreateAttributes() in component class and set m_attributes = new CheckBoxComponentUI(...
    /// </summary>
    public class CheckBoxComponentUI : GH_ComponentAttributes
    {
        public CheckBoxComponentUI(GH_Component owner,
            Action<List<bool>> clickHandle, List<string> texts, List<bool> initialBools, string spacerText = null) : base(owner)
        {
            spacerTxt = (spacerText == null) ? "" : spacerText;
            action = clickHandle;
            toggleTexts = texts;
            toggles = initialBools;
        }

        readonly string spacerTxt; // list of descriptive texts above each dropdown
        RectangleF SpacerBound;

        List<RectangleF> ToggleTextBound;// righternmost part of the 
        List<RectangleF> ToggleBoxBound;// left side bit where we place the button to unfold the dropdown list

        readonly List<string> toggleTexts; // the displayed texts for each check box
        readonly List<bool> toggles; // booleans for each check box

        readonly Action<List<bool>> action; //function sending back toggles

        float MinWidth
        {
            get
            {
                List<string> texts = new List<string>(){ spacerTxt };
                float sp = GhSA.UI.ComponentUI.MaxTextWidth(texts, new Font(GH_FontServer.FamilyStandard, 7));
                float dd1 = GhSA.UI.ComponentUI.MaxTextWidth(toggleTexts, new Font(GH_FontServer.FamilyStandard, 7));
                float num = Math.Max(Math.Max(sp, dd1 + 15), 90); 
                return num;
            }
            set { MinWidth = value; }
        }
        protected override void Layout()
        {
            base.Layout();

            // first change the width
            FixLayout();

            if (SpacerBound == null)
                SpacerBound = new RectangleF();
            if (ToggleTextBound == null)
                ToggleTextBound = new List<RectangleF>();
            if (ToggleBoxBound == null)
                ToggleBoxBound = new List<RectangleF>();

            int s = 2; //spacing to edges and internal between boxes

            int h0 = 0;

            // spacer
            if (spacerTxt != "")
            {
                Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height - (CentralSettings.CanvasObjectIcons ? 5 : 0));
                h0 = 10;
                SpacerBound = new RectangleF(Bounds.X, Bounds.Bottom + s / 2, Bounds.Width, h0);
            }
            // add check boxes
            int h1 = 15; // height border
            int bw = h1; // button width

            // check boxes
            ToggleTextBound = new List<RectangleF>();
            ToggleBoxBound = new List<RectangleF>();

            for (int i = 0; i < toggleTexts.Count; i++)
            {
                // add text box 
                ToggleTextBound.Add(new RectangleF(Bounds.X + 2 * s + bw, Bounds.Bottom + s + h0 + i * h1, Bounds.Width - 4 * s - bw, h1));
                // add check box
                ToggleBoxBound.Add(new RectangleF(Bounds.X + s, Bounds.Bottom + s + h0 + i * h1, bw, h1));

                // update component bounds
            }
            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + toggleTexts.Count * h1 + 2 * s);
        }
        

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                Pen spacer = new Pen(UI.Colour.SpacerColour);
                Pen pen = new Pen(UI.Colour.GsaDarkBlue)
                {
                    Width = 0.5f
                };
                Font font = new Font(GH_FontServer.FamilyStandard, 7);
                // adjust fontsize to high resolution displays
                font = new Font(font.FontFamily, font.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);
                
                Font sml = GH_FontServer.Small;
                // adjust fontsize to high resolution displays
                sml = new Font(sml.FontFamily, sml.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);

                Brush fontColour = UI.Colour.AnnotationTextDark;


                // #### spacer ####
                if (spacerTxt != "")
                {
                    graphics.DrawString(spacerTxt, sml, UI.Colour.AnnotationTextDark, SpacerBound, GH_TextRenderingConstants.CenterCenter);
                    graphics.DrawLine(spacer, SpacerBound.X, SpacerBound.Y + SpacerBound.Height / 2, SpacerBound.X + (SpacerBound.Width - GH_FontServer.StringWidth(spacerTxt, sml)) / 2 - 4, SpacerBound.Y + SpacerBound.Height / 2);
                    graphics.DrawLine(spacer, SpacerBound.X + (SpacerBound.Width - GH_FontServer.StringWidth(spacerTxt, sml)) / 2 + GH_FontServer.StringWidth(spacerTxt, sml) + 4, SpacerBound.Y + SpacerBound.Height / 2, SpacerBound.X + SpacerBound.Width, SpacerBound.Y + SpacerBound.Height / 2);
                }

                // #### check boxes ####
                for (int i = 0; i < toggleTexts.Count; i++)
                {
                    Color myColour = UI.Colour.GsaDarkBlue;
                    Brush myBrush = new SolidBrush(myColour);
                    Brush activeFillBrush = myBrush;
                    Brush passiveFillBrush = Brushes.LightGray;
                    Color borderColour = myColour;
                    Color passiveBorder = Color.DarkGray;
                    int s = 8;
                    // draw text
                    graphics.DrawString(toggleTexts[i], font, fontColour, ToggleTextBound[i], GH_TextRenderingConstants.NearCenter);
                    // draw check box
                    ButtonsUI.CheckBox.DrawCheckButton(graphics, new PointF(ToggleBoxBound[i].X + ToggleBoxBound[i].Width / 2, ToggleBoxBound[i].Y + ToggleBoxBound[i].Height / 2), toggles[i], activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);
                    
                    // update width of text box for mouse-over event handling
                    List<string> incl = new List<string>();
                    incl.Add(toggleTexts[i]);
                    RectangleF txtBound = ToggleTextBound[i];
                    txtBound.Width = GhSA.UI.ComponentUI.MaxTextWidth(incl, font);
                    ToggleTextBound[i] = txtBound;
                }
            }
        }
        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                GH_Component comp = Owner as GH_Component;

                for (int i = 0; i < toggleTexts.Count; i++)
                {
                    if (ToggleBoxBound[i].Contains(e.CanvasLocation) || ToggleTextBound[i].Contains(e.CanvasLocation))
                    {
                        comp.RecordUndoEvent("Toggle Incl. superseeded");
                        toggles[i] = !toggles[i];
                        action(toggles);
                        comp.ExpireSolution(true);
                        return GH_ObjectResponse.Handled;
                    }
                }
            }
            return base.RespondToMouseUp(sender, e);
        }
        
        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            
            for (int i = 0; i < ToggleBoxBound.Count; i++)
            {
                if (ToggleBoxBound[i].Contains(e.CanvasLocation))
                {
                    mouseOver = true;
                    sender.Cursor = System.Windows.Forms.Cursors.Hand;
                    return GH_ObjectResponse.Capture;
                }
            }

            if (mouseOver)
            {
                mouseOver = false;
                Grasshopper.Instances.CursorServer.ResetCursor(sender);
                return GH_ObjectResponse.Release;
            }

            return base.RespondToMouseMove(sender, e);
        }
        bool mouseOver;

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
