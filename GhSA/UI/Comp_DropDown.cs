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
            displayText = selected ?? initialdescription; // if selected input is null (eg first/initial draw of component) then set displaytext to initial description
            displayText = (displayText == "") ? dropdownContent[0] : displayText; // in case no initial description is set and we are in first/inital run then set displaytext to first dropdowncontent item
            SpacerTxt = spacerText; 
            action = clickHandle;
            dropdownlist = dropdownContent;
        }

        string displayText; // the selected item text
        readonly List<string> dropdownlist;//list of elements for dropdown
        RectangleF BorderBound;// area where the selected item is displayed
        RectangleF TextBound;// leftern-most part of the selected/displayed item
        RectangleF ButtonBound;// right side bit where we place the button to unfold the dropdown list
        List<RectangleF> dropdownBounds;// list of bounds for each item in dropdown list
        RectangleF dropdownBound;// surrounding bound for the entire dropdown list
        RectangleF dropdownScroller;// surrounding bound for vertical scroll element
        float scrollY; // location of scroll element at drag start
        float dragMouseStartY; // location of mouse at drag start
        float deltaY; // moved Y-location of scroll element
        int maxNoRows = 10;
        bool drag;
        bool unfolded;
        readonly Action<string> action;
        readonly string initialTxt;
        RectangleF SpacerBounds;
        readonly string SpacerTxt;

        float MinWidth
        {
            get
            {
                List<string> spacers = new List<string>();
                spacers.Add(SpacerTxt);
                float sp = GhSA.UI.ComponentUI.MaxTextWidth(spacers, GH_FontServer.Small);
                List<string> buttons = new List<string>();
                float bt = GhSA.UI.ComponentUI.MaxTextWidth(dropdownlist, new Font(GH_FontServer.FamilyStandard, 7));
                float num = Math.Max(Math.Max(sp, bt), 90);
                return num;
            }
            set { MinWidth = value; }
        }
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
                dropdownBound = new RectangleF(BorderBound.X, BorderBound.Y + h1 + s, BorderBound.Width, Math.Min(dropdownlist.Count, maxNoRows) * BorderBound.Height);
                
                //update component size if dropdown is unfolded to be able to capture mouseclicks
                Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + dropdownBound.Height + s);

                // vertical scroll bar if no of items in dropdown list is bigger than max rows allowed
                if (dropdownlist.Count > maxNoRows)
                {
                    if (dropdownScroller == null)
                        dropdownScroller = new RectangleF();

                    dropdownScroller.X = dropdownBound.X + dropdownBound.Width - 7;
                    dropdownScroller.Height = (float)Math.Max(2 * h1, h1 * maxNoRows * ((double)maxNoRows / ((double)dropdownlist.Count + 1)) - s);
                    dropdownScroller.Width = 8;

                    if (deltaY + scrollY >= 0) // handle if user drags above starting point
                    {
                        if (dropdownBound.Height - dropdownScroller.Height >= deltaY + scrollY) // handles if user drags below bottom point
                        {
                            dropdownScroller.Y = dropdownBound.Y + s + deltaY + scrollY;
                        }
                        else
                        {
                            scrollY = dropdownBound.Height - dropdownScroller.Height;
                            deltaY = 0;
                        }
                    }
                    else
                    {
                        scrollY = 0;
                        deltaY = 0;
                    }
                }

                dropdownBounds = new List<RectangleF>();
                for (int i = 0; i < dropdownlist.Count; i++)
                {
                    dropdownBounds.Add(new RectangleF(BorderBound.X, BorderBound.Y + (i + 1) * h1 + s - deltaY - scrollY, BorderBound.Width, BorderBound.Height));
                }
            }
            else
            {
                if (dropdownBounds != null)
                    dropdownBounds.Clear();
                dropdownBound = new RectangleF();
                dropdownScroller = new RectangleF();
                scrollY = 0;
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

                // Draw selected item
                Font font = new Font(GH_FontServer.FamilyStandard, 7);
                // adjust fontsize to high resolution displays
                font = new Font(font.FontFamily, font.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);
                Brush fontColour = UI.Colour.AnnotationTextDark;
                // background
                Brush background = new SolidBrush(UI.Colour.GsaLightGrey);
                graphics.FillRectangle(background, BorderBound); // background
                // border
                graphics.DrawRectangle(pen, BorderBound.X, BorderBound.Y, BorderBound.Width, BorderBound.Height);
                // text
                graphics.DrawString(displayText, (displayText == initialTxt) ? sml : font, (displayText == initialTxt) ? Brushes.Gray : fontColour, TextBound, GH_TextRenderingConstants.NearCenter);
                // draw dropdown arrow
                ButtonsUI.DropDownArrow.DrawDropDownButton(graphics, new PointF(ButtonBound.X + ButtonBound.Width / 2, ButtonBound.Y + ButtonBound.Height / 2), UI.Colour.GsaDarkBlue, 15);

                // draw dropdown list
                if (unfolded)
                {
                    Pen penborder = new Pen(Brushes.Gray);
                    Brush dropdownbackground = new SolidBrush(UI.Colour.GsaLightGrey);
                    penborder.Width = 0.3f;
                    for (int i = 0; i < dropdownBounds.Count; i++)
                    {
                        RectangleF listItem = dropdownBounds[i];
                        if (listItem.Y < dropdownBound.Y)
                        {
                            if (listItem.Y + listItem.Height < dropdownBound.Y)
                            {
                                dropdownBounds[i] = new RectangleF();
                                continue;
                            }
                            else
                            {
                                listItem.Height = listItem.Height - (dropdownBound.Y - listItem.Y);
                                listItem.Y = dropdownBound.Y;
                                dropdownBounds[i] = listItem;
                            }
                        }
                        else if (listItem.Y + listItem.Height > dropdownBound.Y + dropdownBound.Height)
                        {
                            if (listItem.Y > dropdownBound.Y + dropdownBound.Height)
                            {
                                dropdownBounds[i] = new RectangleF();
                                continue;
                            }
                            else
                            {
                                listItem.Height = dropdownBound.Y + dropdownBound.Height - listItem.Y;
                                dropdownBounds[i] = listItem;
                            }
                        }
                        // background
                        graphics.FillRectangle(dropdownbackground, dropdownBounds[i]);
                        // border
                        graphics.DrawRectangle(penborder, dropdownBounds[i].X, dropdownBounds[i].Y, dropdownBounds[i].Width, dropdownBounds[i].Height);
                        // text
                        if (dropdownBounds[i].Height > 2)
                            graphics.DrawString(dropdownlist[i], font, fontColour, dropdownBounds[i], GH_TextRenderingConstants.NearCenter);
                    }
                   
                    // border
                    graphics.DrawRectangle(pen, dropdownBound.X, dropdownBound.Y, dropdownBound.Width, dropdownBound.Height);

                    // draw vertical scroll bar
                    Brush scrollbar = new SolidBrush(Color.FromArgb(drag? 220 : 160, Color.Black));
                    RectangleF scrollbarrectangle = new RectangleF(dropdownScroller.X + 2, dropdownScroller.Y, dropdownScroller.Width - 4, dropdownScroller.Height - 4);
                    graphics.FillRectangle(scrollbar, scrollbarrectangle);

                }
            }
        }
        public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                GH_Component comp = Owner as GH_Component;
                if (drag)
                {
                    // if drag was true then we release it here:
                    scrollY += deltaY;
                    deltaY = 0;
                    drag = false;
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Release;
                }

                System.Drawing.RectangleF rec = BorderBound;
                if (rec.Contains(e.CanvasLocation))
                {
                    unfolded = !unfolded;
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

                if (unfolded)
                {
                    System.Drawing.RectangleF rec3 = dropdownBound;
                    if (rec3.Contains(e.CanvasLocation))
                    {
                        for (int i = 0; i < dropdownlist.Count; i++)
                        {
                            System.Drawing.RectangleF rec4 = dropdownBounds[i];
                            if (rec4.Contains(e.CanvasLocation))
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
            return base.RespondToMouseUp(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (unfolded)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    System.Drawing.RectangleF rec = dropdownScroller;
                    GH_Component comp = Owner as GH_Component;
                    if (rec.Contains(e.CanvasLocation))
                    {
                        dragMouseStartY = e.CanvasLocation.Y;
                        drag = true;
                        comp.ExpireSolution(true);
                        return GH_ObjectResponse.Capture;
                    }
                }
            }
            return base.RespondToMouseDown(sender, e);
        }

        public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (drag)
            {
                GH_Component comp = Owner as GH_Component;

                deltaY = e.CanvasLocation.Y - dragMouseStartY;
                
                comp.ExpireSolution(true);
                return GH_ObjectResponse.Ignore;
            }

            return base.RespondToMouseMove(sender, e);
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
