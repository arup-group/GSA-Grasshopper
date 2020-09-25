using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using System.Windows.Forms;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace GhSA.UI
{
    /// <summary>
    /// Class to create custom component UI with a single dropdown menu
    /// 
    /// Look at gsaDropDownSingle.cs for an example of how to call this method.
    /// 
    /// To use this method override CreateAttributes() in component class and set m_attributes = new DropDownComponentUI(...
    /// </summary>
    public class DropDownComponentUI : GH_ComponentAttributes
    {
        public DropDownComponentUI(GH_Component owner, Action<string> clickHandle, List<string> dropdownContent, string selected, string spacerText = "", string initialdescription = "") : base(owner) 
        {
            initialTxt = (initialdescription == "") ? null : initialdescription; // if no description is inputted then null initialTxt
            displayText = (selected == null) ? initialdescription : selected; // if selected input is null (eg first/initial draw of component) then set displaytext to initial description
            displayText = (displayText == "") ? dropdownContent[0] : displayText; // in case no initial description is set and we are in first/inital run then set displaytext to first dropdowncontent item
            SpacerTxt = spacerText; 
            action = clickHandle;
            dropdownlist = dropdownContent;
        }

        string displayText; // the selected item text
        List<string> dropdownlist;//list of elements for dropdown
        RectangleF BorderBound;// area where the selected item is displayed
        RectangleF TextBound;// leftern-most part of the selected/displayed item
        RectangleF ButtonBound;// right side bit where we place the button to unfold the dropdown list
        List<RectangleF> dropdownBounds;// list of bounds for each item in dropdown list
        RectangleF dropdownBound;// surrounding bound for the entire dropdown list
        bool unfolded;
        Action<string> action;
        string initialTxt;
        RectangleF SpacerBounds;
        string SpacerTxt;

        float minWidth
        {
            get
            {
                float num = Math.Max(Math.Max(
                    GH_FontServer.StringWidth(SpacerTxt, GH_FontServer.Small) + 8,
                    GH_FontServer.StringWidth(displayText, GH_FontServer.Standard)) + 8,
                    90);

                return num;
            }
            set { minWidth = value; }
        }
        protected override void Layout()
        {
            base.Layout();

            // first change the width to suit; using max to determine component visualisation style
            FixLayout();

            int s = 2; //spacing to edges and internal between boxes

            int h0 = 0;
            int txtwidth = 0;
            //spacer and title
            if (SpacerTxt != "")
            {
                h0 = 10;
                txtwidth = GH_FontServer.StringWidth(SpacerTxt, GH_FontServer.Small);
                SpacerBounds = new RectangleF(Bounds.X, Bounds.Bottom + s / 2, Bounds.Width, h0);
            }

            int h1 = 15; // height border
            int bw = h1; // button width
            // create text box border
            BorderBound = new RectangleF(Bounds.X + 2 * s, Bounds.Bottom + h0 + 2 * s, Bounds.Width - 2 - 4 * s, h1);
            // text box inside border
            TextBound = new RectangleF(BorderBound.X, BorderBound.Y, BorderBound.Width - bw, BorderBound.Height);
            // button area inside border
            ButtonBound = new RectangleF(BorderBound.X + BorderBound.Width - bw, BorderBound.Y, bw, BorderBound.Height);

            //update component bounds
            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + h1 + 4 * s);

            // create list of bounds for dropdown if dropdown is unfolded
            if (unfolded)
            {
                if (dropdownBounds == null)
                dropdownBounds = new List<RectangleF>();
                for (int i = 0; i < dropdownlist.Count; i++)
                    dropdownBounds.Add(new RectangleF(BorderBound.X, BorderBound.Y + (i + 1) * h1 + s, BorderBound.Width, BorderBound.Height));
                dropdownBound = new RectangleF(BorderBound.X, BorderBound.Y + h1 + s, BorderBound.Width, dropdownBounds.Count * BorderBound.Height);
                //update component size if dropdown is unfolded to be able to capture mouseclicks
                Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + dropdownBound.Height + s); 
            }
            else
            {
                if (dropdownBounds != null)
                    dropdownBounds.Clear();
                dropdownBound = new RectangleF();
            }
        }

        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                Pen spacer = new Pen(UI.Colour.SpacerColour);
                Pen pen = new Pen(UI.Colour.GsaDarkBlue);
                if (displayText == initialTxt)
                    pen = new Pen(UI.Colour.BorderColour);
                pen.Width = 0.5f;
                //Draw divider line
                if (SpacerTxt != "")
                {
                    graphics.DrawString(SpacerTxt, GH_FontServer.Small, UI.Colour.AnnotationTextDark, SpacerBounds, GH_TextRenderingConstants.CenterCenter);
                    graphics.DrawLine(spacer, SpacerBounds.X, SpacerBounds.Y + SpacerBounds.Height / 2, SpacerBounds.X + (SpacerBounds.Width - GH_FontServer.StringWidth(SpacerTxt, GH_FontServer.Small)) / 2 - 4, SpacerBounds.Y + SpacerBounds.Height / 2);
                    graphics.DrawLine(spacer, SpacerBounds.X + (SpacerBounds.Width - GH_FontServer.StringWidth(SpacerTxt, GH_FontServer.Small)) / 2 + GH_FontServer.StringWidth(SpacerTxt, GH_FontServer.Small) + 4, SpacerBounds.Y + SpacerBounds.Height / 2, SpacerBounds.X + SpacerBounds.Width, SpacerBounds.Y + SpacerBounds.Height / 2);
                }

                // Draw selected item
                Font font = new Font(GH_FontServer.FamilyStandard, 7);
                Brush fontColour = UI.Colour.AnnotationTextDark;
                // background
                Brush background = new SolidBrush(UI.Colour.GsaLightGrey);
                graphics.FillRectangle(background, BorderBound); // background
                // border
                graphics.DrawRectangle(pen, BorderBound.X, BorderBound.Y, BorderBound.Width, BorderBound.Height);
                // text
                graphics.DrawString(displayText, (displayText == initialTxt) ? GH_FontServer.Small : font, (displayText == initialTxt) ? Brushes.Gray : fontColour, TextBound, GH_TextRenderingConstants.NearCenter);
                // draw dropdown arrow
                ButtonsUI.DropDownArrow.DrawDropDownButton(graphics, new PointF(ButtonBound.X + ButtonBound.Width / 2, ButtonBound.Y + ButtonBound.Height / 2), UI.Colour.GsaDarkBlue, 15);

                // draw dropdown list
                if (unfolded)
                {
                    Pen penborder = new Pen(Brushes.Gray);
                    Brush dropdownbackground = new SolidBrush(UI.Colour.GsaLightGrey);
                    penborder.Width = 0.3f;
                    for (int i = 0; i < dropdownlist.Count; i++)
                    {
                        // background
                        graphics.FillRectangle(dropdownbackground, dropdownBounds[i]);
                        // border
                        graphics.DrawRectangle(penborder, dropdownBounds[i].X, dropdownBounds[i].Y, dropdownBounds[i].Width, dropdownBounds[i].Height);
                        // text
                        graphics.DrawString(dropdownlist[i], font, fontColour, dropdownBounds[i], GH_TextRenderingConstants.NearCenter);
                    }
                   
                    // border
                    graphics.DrawRectangle(pen, dropdownBound.X, dropdownBound.Y, dropdownBound.Width, dropdownBound.Height);
                }
            }
        }
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                System.Drawing.RectangleF rec = BorderBound;
                GH_Component comp = Owner as GH_Component;
                if (rec.Contains(e.CanvasLocation))
                {
                    unfolded = !unfolded;
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

                if (unfolded)
                {
                    System.Drawing.RectangleF rec2 = dropdownBound;
                    if (rec2.Contains(e.CanvasLocation))
                    {
                        for (int i = 0; i < dropdownlist.Count; i++)
                        {
                            System.Drawing.RectangleF rec3 = dropdownBounds[i];
                            if (rec3.Contains(e.CanvasLocation))
                            {
                                comp.RecordUndoEvent("Selected " + dropdownlist[i]);
                                displayText = dropdownlist[i]; //change what user sees to selected item
                                action(dropdownlist[i]); // send selected item back to component
                                unfolded = !unfolded; // close dropdown
                                comp.ExpireSolution(true);
                                return GH_ObjectResponse.Handled;
                            }
                        }
                    }
                    else
                    {
                        unfolded = !unfolded;
                        comp.ExpireSolution(true);
                        return GH_ObjectResponse.Handled;
                    }
                }
            }
            return base.RespondToMouseDown(sender, e);
        }

        // consider adding a new cursor type when mouse is on top of clickable dropdown?

        //public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        //{
        //    if (BorderBound.Contains(e.ControlLocation))
        //    {
        //        Rectangle rect = GH_Convert.ToRectangle(BorderBound);
        //        Control ctrl = new Control(displayText, rect.X, rect.Y, rect.Width, rect.Height);
        //        GH_Component comp = Owner as GH_Component;
        //        Grasshopper.Instances.CursorServer.AttachCursor(ctrl, "GH_AddObject");
        //        ctrl.Refresh();
        //        return GH_ObjectResponse.Handled;
        //    }
        //    return GH_ObjectResponse.Ignore;
        //}

        protected void FixLayout()
        {
            float width = this.Bounds.Width;
            float num = Math.Max(width, minWidth);
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
