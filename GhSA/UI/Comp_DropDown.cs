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
        RectangleF scrollBar;// surrounding bound for vertical scroll element
        float scrollStartY; // location of scroll element at drag start
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
                if (dropdownlist.Count > maxNoRows)
                    bt += 15; // add room for vertical scroll bar
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

            int s = 2; // spacing to edges and internal between boxes

            // create bound for spacer and title
            int h0 = 0; // height of spacer bound
            if (SpacerTxt != "")
            {
                Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height - (CentralSettings.CanvasObjectIcons ? 5 : 0));
                h0 = 10;
                SpacerBounds = new RectangleF(Bounds.X, Bounds.Bottom + s / 2, Bounds.Width, h0);
            }

            int h1 = 15; // height of bound for one line of text / text box
            int bw = h1; // dropdown button/arrow width

            // create top/selected border and text box
            BorderBound = new RectangleF(Bounds.X + 2 * s, Bounds.Bottom + h0 + 2 * s, Bounds.Width - 2 - 4 * s, h1);
            // text box inside border
            float textWidth = (dropdownlist.Count > maxNoRows) ? BorderBound.Width - bw : BorderBound.Width;
            TextBound = new RectangleF(BorderBound.X, BorderBound.Y, textWidth, BorderBound.Height); 
            // dropdown button/arrow area inside border
            ButtonBound = new RectangleF(BorderBound.X + BorderBound.Width - bw, BorderBound.Y, bw, BorderBound.Height);

            //update component bounds
            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + h1 + 4 * s);

            // create list of bounds for dropdownlist if dropdownmenu is unfolded
            if (unfolded)
            {
                dropdownBound = new RectangleF(BorderBound.X, BorderBound.Y + h1 + s, BorderBound.Width, Math.Min(dropdownlist.Count, maxNoRows) * h1);

                //update component size if dropdown is unfolded to be able to capture mouseclicks
                Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + dropdownBound.Height + s);

                // additional move for the content (moves more than the scroll bar)
                float contentScroll = 0;

                // vertical scroll bar if number of items in dropdown list is bigger than max rows allowed
                if (dropdownlist.Count > maxNoRows)
                {
                    if (scrollBar == null)
                        scrollBar = new RectangleF();

                    // setup size of scroll bar
                    scrollBar.X = dropdownBound.X + dropdownBound.Width - 8; // locate from right-side of dropdown area
                    // compute height based on number of items in list, but with a minimum size of 2 rows
                    scrollBar.Height = (float)Math.Max(2 * h1, dropdownBound.Height * ((double)maxNoRows / ((double)dropdownlist.Count)));
                    scrollBar.Width = 8; // width of mouse-grab area (actual scroll bar drawn later)

                    // vertical position (.Y)
                    if (deltaY + scrollStartY >= 0) // handle if user drags above starting point
                    {
                        // dragging downwards:
                        if (dropdownBound.Height - scrollBar.Height >= deltaY + scrollStartY) // handles if user drags below bottom point
                        {
                            // update scroll bar position for normal scroll event within bounds
                            scrollBar.Y = dropdownBound.Y + deltaY + scrollStartY;
                        }
                        else
                        {
                            // scroll reached bottom
                            scrollStartY = dropdownBound.Height - scrollBar.Height;
                            deltaY = 0;
                        }
                    }
                    else
                    {
                        // scroll reached top
                        scrollStartY = 0;
                        deltaY = 0;
                    }

                    // calculate moved position of content
                    float scrollBarMovedPercentage = (dropdownBound.Y - scrollBar.Y) / (dropdownBound.Height - scrollBar.Height);
                    float scrollContentHeight = dropdownlist.Count * h1 - dropdownBound.Height;
                    contentScroll = scrollBarMovedPercentage * scrollContentHeight;
                }

                // create list of text boxes (we will only draw the visible ones later)
                dropdownBounds = new List<RectangleF>();
                for (int i = 0; i < dropdownlist.Count; i++)
                {
                    dropdownBounds.Add(new RectangleF(BorderBound.X, BorderBound.Y + (i + 1) * h1 + s + contentScroll, BorderBound.Width, h1));
                }
            }
            else
            {
                if (dropdownBounds != null)
                    dropdownBounds.Clear();
                dropdownBound = new RectangleF();
                scrollBar = new RectangleF();
                scrollStartY = 0;
            }
        }

        protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
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
                    Brush scrollbar = new SolidBrush(Color.FromArgb(drag? 160 : 120, Color.Black));
                    Pen scrollPen = new Pen(scrollbar);
                    scrollPen.Width = scrollBar.Width - 2;
                    scrollPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    scrollPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    graphics.DrawLine(scrollPen, scrollBar.X + 4, scrollBar.Y + 4, scrollBar.X + 4, scrollBar.Y + scrollBar.Height - 4);

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
                    scrollStartY += deltaY;
                    deltaY = 0;
                    drag = false;
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Release;
                }

                RectangleF rec = BorderBound;
                if (rec.Contains(e.CanvasLocation))
                {
                    unfolded = !unfolded;
                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }

                if (unfolded)
                {
                    RectangleF rec3 = dropdownBound;
                    if (rec3.Contains(e.CanvasLocation))
                    {
                        for (int i = 0; i < dropdownlist.Count; i++)
                        {
                            RectangleF rec4 = dropdownBounds[i];
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
                    RectangleF rec = scrollBar;
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

            if (ButtonBound.Contains(e.CanvasLocation))
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
