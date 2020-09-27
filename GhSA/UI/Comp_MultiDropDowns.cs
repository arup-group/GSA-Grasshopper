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
    /// Class to create custom component UI with multiple dropdowns
    /// 
    /// Note that it is the component's responsibility to dynamically update lists, this class is only displaying what it gets.
    /// Look at gsaDropDownMulti.cs for an example of how to call this method.
    /// 
    /// To use this method override CreateAttributes() in component class and set m_attributes = new MultiDropDownComponentUI(...
    /// </summary>
    public class MultiDropDownComponentUI : GH_ComponentAttributes
    {
        public MultiDropDownComponentUI(GH_Component owner, Action<int, int> clickHandle, List<List<string>> dropdownContents, List<string> selections, List<string> spacerTexts = null, List<string> initialdescriptions = null) : base(owner)
        {
            dropdownlists = dropdownContents;
            spacerTxts = spacerTexts;
            action = clickHandle;
            initialTxts = (initialdescriptions == null) ? null : initialdescriptions; // if no description is inputted then null initialTxt
            if (selections == null)
            {
                List<string> tempDisplaytxt = new List<string>();
                for (int i = 0; i < dropdownlists.Count; i++)
                    tempDisplaytxt.Add((initialdescriptions == null) ? dropdownlists[i][0] : initialdescriptions[i]);
                displayTexts = tempDisplaytxt;
            }
            else
                displayTexts = selections;
        }

        readonly List<string> spacerTxts; // list of descriptive texts above each dropdown
        List<RectangleF> SpacerBounds;

        List<RectangleF> BorderBound;// area where the selected item is displayed
        List<RectangleF> TextBound;// lefternmost part of the selected/displayed item
        List<RectangleF> ButtonBound;// right side bit where we place the button to unfold the dropdown list

        readonly List<string> displayTexts; // the selected item text
        readonly List<string> initialTxts; // initial text to be able to display a hint

        readonly List<List<string>> dropdownlists; // content lists of items for dropdown

        List<List<RectangleF>> dropdownBounds;// list of bounds for each item in dropdown list
        List<RectangleF> dropdownBound;// surrounding bound for the entire dropdown list

        readonly Action<int, int> action; //function sending back the selection to component (i = dropdowncontentlist, j = selected item in that list)
        
        List<bool> unfolded; // list of bools for unfolded or closed dropdown
        

        float minWidth
        {
            get
            {
                float sp = new float();
                for (int i = 0; i < spacerTxts.Count; i++)
                {
                    if (GH_FontServer.StringWidth(spacerTxts[i], GH_FontServer.Small) + 8 > sp)
                        sp = GH_FontServer.StringWidth(spacerTxts[i], GH_FontServer.Small) + 8;
                }
                float di = new float();
                for (int i = 0; i < displayTexts.Count; i++)
                {
                    if (GH_FontServer.StringWidth(displayTexts[i], GH_FontServer.Small) + 8 > sp)
                        di = GH_FontServer.StringWidth(displayTexts[i], GH_FontServer.Small) + 8;
                }
                float num = Math.Max(Math.Max(sp, di), 90);
                return num;
            }
            set { minWidth = value; }
        }
        protected override void Layout()
        {
            base.Layout();

            // first change the width to suit; using max to determine component visualisation style
            FixLayout();

            if (SpacerBounds == null)
                SpacerBounds = new List<RectangleF>();
            if (BorderBound == null)
                BorderBound = new List<RectangleF>();
            if (TextBound == null)
                TextBound = new List<RectangleF>();
            if (ButtonBound == null)
                ButtonBound = new List<RectangleF>();
            if (dropdownBound == null)
                dropdownBound = new List<RectangleF>();
            if (dropdownBounds == null)
                dropdownBounds = new List<List<RectangleF>>();
            if (unfolded == null)
                unfolded = new List<bool>();

            int s = 2; //spacing to edges and internal between boxes

            int h0 = 0;
            for (int i = 0; i < dropdownlists.Count; i++) 
            {
                //spacer and title
                if (spacerTxts[i] != "")
                {
                    h0 = 10;
                    RectangleF tempSpacer = new RectangleF(Bounds.X, Bounds.Bottom + s / 2, Bounds.Width, h0);
                    if (SpacerBounds.Count == i || SpacerBounds[i] == null)
                        SpacerBounds.Add(tempSpacer);
                    else
                        SpacerBounds[i] = tempSpacer;
                }

                int h1 = 15; // height border
                int bw = h1; // button width

                // create text box border
                RectangleF tempBorder = new RectangleF(Bounds.X + 2 * s, Bounds.Bottom + h0 + 2 * s, Bounds.Width - 2 - 4 * s, h1);
                if (BorderBound.Count == i || BorderBound[i] == null)
                    BorderBound.Add(tempBorder);
                else
                    BorderBound[i] = tempBorder;

                // text box inside border
                RectangleF tempText = new RectangleF(BorderBound[i].X, BorderBound[i].Y, BorderBound[i].Width - bw, BorderBound[i].Height);
                if (TextBound.Count == i || TextBound[i] == null)
                    TextBound.Add(tempText);
                else
                    TextBound[i] = tempText;

                // button area inside border
                RectangleF tempButton = new RectangleF(BorderBound[i].X + BorderBound[i].Width - bw, BorderBound[i].Y, bw, BorderBound[i].Height);
                if (ButtonBound.Count == i || ButtonBound[i] == null)
                    ButtonBound.Add(tempButton);
                else
                    ButtonBound[i] = tempButton;

                //update component bounds
                Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + h1 + 4 * s);

                // create list of bounds for dropdown if dropdown is unfolded
                if (unfolded.Count == i)
                    unfolded.Add(new bool()); //ensure we have a bool for every list

                if (unfolded[i]) // if unfolded checked create dropdown list
                {
                    if (dropdownBounds[i] == null)
                        dropdownBounds[i] = new List<RectangleF>(); // if first time clicked create new list
                    else
                        dropdownBounds[i].Clear(); // if previously created make sure to clear existing if content has changed
                    for (int j = 0; j < dropdownlists[i].Count; j++)
                    {
                        dropdownBounds[i].Add(new RectangleF(BorderBound[i].X, BorderBound[i].Y + (j + 1) * h1 + s, BorderBound[i].Width, BorderBound[i].Height));
                    }
                    dropdownBound[i] = new RectangleF(BorderBound[i].X, BorderBound[i].Y + h1 + s, BorderBound[i].Width, dropdownBounds[i].Count * BorderBound[i].Height);
                    //update component size if dropdown is unfolded to be able to capture mouseclicks
                    Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + dropdownBound[i].Height + s);
                }
                else
                {
                    if (dropdownBounds != null)
                    {
                        if (dropdownBounds.Count == i)
                            dropdownBounds.Add(new List<RectangleF>());
                        if (dropdownBounds[i] != null)
                            dropdownBounds[i].Clear();
                        if (dropdownBound.Count == i)
                            dropdownBound.Add(new RectangleF());
                        else
                            dropdownBound[i] = new RectangleF();
                    }
                }
            }
        }
        

        protected override void Render(GH_Canvas canvas, System.Drawing.Graphics graphics, GH_CanvasChannel channel)
        {
            base.Render(canvas, graphics, channel);

            if (channel == GH_CanvasChannel.Objects)
            {
                Pen spacer = new Pen(UI.Colour.SpacerColour);
                Pen pen = new Pen(UI.Colour.GsaDarkBlue);
                pen.Width = 0.5f;

                for (int i = 0; i < dropdownlists.Count; i++)
                {
                    
                    //Draw divider line
                    if (spacerTxts[i] != "")
                    {
                        graphics.DrawString(spacerTxts[i], GH_FontServer.Small, UI.Colour.AnnotationTextDark, SpacerBounds[i], GH_TextRenderingConstants.CenterCenter);
                        graphics.DrawLine(spacer, SpacerBounds[i].X, SpacerBounds[i].Y + SpacerBounds[i].Height / 2, SpacerBounds[i].X + (SpacerBounds[i].Width - GH_FontServer.StringWidth(spacerTxts[i], GH_FontServer.Small)) / 2 - 4, SpacerBounds[i].Y + SpacerBounds[i].Height / 2);
                        graphics.DrawLine(spacer, SpacerBounds[i].X + (SpacerBounds[i].Width - GH_FontServer.StringWidth(spacerTxts[i], GH_FontServer.Small)) / 2 + GH_FontServer.StringWidth(spacerTxts[i], GH_FontServer.Small) + 4, SpacerBounds[i].Y + SpacerBounds[i].Height / 2, SpacerBounds[i].X + SpacerBounds[i].Width, SpacerBounds[i].Y + SpacerBounds[i].Height / 2);
                    }

                    // Draw selected item
                    // set font and colour depending on inital or selected text
                    Font font = new Font(GH_FontServer.FamilyStandard, 7);
                    Brush fontColour = UI.Colour.AnnotationTextDark;
                    if (initialTxts != null)
                    {
                        if (displayTexts[i] == initialTxts[i])
                        {
                            pen = new Pen(UI.Colour.BorderColour);
                            font = GH_FontServer.Small;
                            fontColour = Brushes.Gray;
                        }
                    }
                    
                    // background
                    Brush background = new SolidBrush(UI.Colour.GsaLightGrey);
                    graphics.FillRectangle(background, BorderBound[i]); // background
                                                                     // border
                    graphics.DrawRectangle(pen, BorderBound[i].X, BorderBound[i].Y, BorderBound[i].Width, BorderBound[i].Height);
                    // text
                    graphics.DrawString(displayTexts[i], font, fontColour, TextBound[i], GH_TextRenderingConstants.NearCenter);
                    // draw dropdown arrow460a2412-ce15-49a6-b8da-e512ba92eeec
                    ButtonsUI.DropDownArrow.DrawDropDownButton(graphics, new PointF(ButtonBound[i].X + ButtonBound[i].Width / 2, ButtonBound[i].Y + ButtonBound[i].Height / 2), UI.Colour.GsaDarkBlue, 15);

                    // draw dropdown list
                    font = new Font(GH_FontServer.FamilyStandard, 7);
                    fontColour = UI.Colour.AnnotationTextDark;
                    if (unfolded[i])
                    {
                        Pen penborder = new Pen(Brushes.Gray);
                        Brush dropdownbackground = new SolidBrush(UI.Colour.GsaLightGrey);
                        penborder.Width = 0.3f;
                        for (int j = 0; j < dropdownBounds[i].Count; j++)
                        {
                            // background
                            graphics.FillRectangle(dropdownbackground, dropdownBounds[i][j]);
                            // border
                            graphics.DrawRectangle(penborder, dropdownBounds[i][j].X, dropdownBounds[i][j].Y, dropdownBounds[i][j].Width, dropdownBounds[i][j].Height);
                            // text
                            graphics.DrawString(dropdownlists[i][j], font, fontColour, dropdownBounds[i][j], GH_TextRenderingConstants.NearCenter);
                        }
                        // border
                        graphics.DrawRectangle(pen, dropdownBound[i].X, dropdownBound[i].Y, dropdownBound[i].Width, dropdownBound[i].Height);
                    }
                }
            }
        }
        public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                for (int i = 0; i < dropdownlists.Count; i++)
                {
                    System.Drawing.RectangleF rec = BorderBound[i];
                    GH_Component comp = Owner as GH_Component;
                    if (rec.Contains(e.CanvasLocation))
                    {
                        unfolded[i] = !unfolded[i];
                        comp.ExpireSolution(true);
                        return GH_ObjectResponse.Handled;
                    }

                    if (unfolded[i])
                    {
                        System.Drawing.RectangleF rec2 = dropdownBound[i];
                        if (rec2.Contains(e.CanvasLocation))
                        {
                            for (int j = 0; j < dropdownBounds[i].Count; j++)
                            {
                                System.Drawing.RectangleF rec3 = dropdownBounds[i][j];
                                if (rec3.Contains(e.CanvasLocation))
                                {
                                    if (displayTexts[i] != dropdownlists[i][j])
                                    {
                                        // record an undo event so that user can ctrl + z
                                        comp.RecordUndoEvent("Selected " + dropdownlists[i][j]);
                                        
                                        // change the displayed text on canvas
                                        displayTexts[i] = dropdownlists[i][j];
                                        
                                        // if initial texts exists then change all dropdowns below this one to the initial description
                                        if (initialTxts != null)
                                        {
                                            for (int k = i + 1; k < dropdownlists.Count; k++)
                                                displayTexts[k] = initialTxts[k];
                                        }

                                        // send the selected item back to component (i = dropdownlist index, j = selected item in that list)
                                        action(i, j);

                                        // close the dropdown
                                        unfolded[i] = !unfolded[i];
                                        
                                        // recalculate component
                                        comp.ExpireSolution(true);
                                    }
                                    else
                                    {
                                        unfolded[i] = !unfolded[i];
                                        comp.ExpireSolution(true);
                                    }
                                    return GH_ObjectResponse.Handled;
                                }
                            }
                        }
                        else
                        {
                            unfolded[i] = !unfolded[i];
                            comp.ExpireSolution(true);
                            return GH_ObjectResponse.Handled;
                        }
                    }
                }
            }
            return base.RespondToMouseDown(sender, e);
        }
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
