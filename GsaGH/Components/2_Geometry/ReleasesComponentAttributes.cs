using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using OasysGH.UI.Helpers;

namespace OasysGH.UI.Foo
{
  /// <summary>
  /// Class to create custom component UI with 2 x 6 check box toggles
  /// 
  /// This class is made for the CreateMember1d component
  /// 
  /// To use this method override CreateAttributes() in component class and set m_attributes = new ReleasesComponentAttributes(...
  /// </summary>
  public class ReleasesComponentAttributes : GH_ComponentAttributes
  {
    public ReleasesComponentAttributes(GH_Component owner, Action<List<bool>> updateHandle, string spacerText1, string spacerText2,
        List<bool> restraints) : base(owner)
    {
      _restraints = restraints;
      _update = updateHandle;
      _spacerTxt1 = spacerText1;
      _spacerTxt2 = spacerText2;
    }

    // function that sends back the user input to the component
    protected readonly Action<List<bool>> _update;

    #region Custom layout logic
    // restraints set by component
    List<bool> _restraints = new List<bool>();

    // text boxes bounds for pre-set restraints
    RectangleF _spacerBounds1;
    RectangleF _spacerBounds2;
    readonly string _spacerTxt1;
    readonly string _spacerTxt2;

    // annotation text bounds
    List<RectangleF> _textBounds;

    // bounds for check boxes
    List<RectangleF> _bounds = new List<RectangleF>();

    protected int _spacing = 3; // spacing to edges and internal between boxes

    protected float MinWidth
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

      // first change the width to suit; using max to determine component visualisation style
      FixLayout();


      // spacer and title
      int h0 = 0;
      if (_spacerTxt1 != "")
      {
        Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height - (CentralSettings.CanvasObjectIcons ? 5 : 0));

        h0 = 10;
        _spacerBounds1 = new RectangleF(Bounds.X, Bounds.Bottom + _spacing / 2, Bounds.Width, h0);
      }

      float w = (Bounds.Width - 6 * _spacing) / 6; // width of each check box
      int h2 = 15; // height
      int h3 = 15;
      // create annotation (x, y, z, xx, yy, zz) placeholders
      _textBounds = new List<RectangleF>()
      {
        new RectangleF(Bounds.X + _spacing, Bounds.Bottom + h0, w, h2),
        new RectangleF(Bounds.X + 1.5f * _spacing + w, Bounds.Bottom + h0, w, h2),
        new RectangleF(Bounds.X + 2.5f * _spacing + 2 * w, Bounds.Bottom + h0, w, h2),
        new RectangleF(Bounds.X + 3f * _spacing + 3 * w, Bounds.Bottom + h0, w + _spacing, h2),
        new RectangleF(Bounds.X + 4f * _spacing + 4 * w, Bounds.Bottom + h0, w + _spacing, h2),
        new RectangleF(Bounds.X + 5f * _spacing - 1 + 5 * w, Bounds.Bottom + h0, w + _spacing, h2)
      };
      // create check box placeholders
      _bounds = new List<RectangleF>()
      {
        new RectangleF(Bounds.X + _spacing / 2, Bounds.Bottom + h0 + h2, w, h3),
        new RectangleF(Bounds.X + 1.5f * _spacing + w, Bounds.Bottom + h0 + h2, w, h3),
        new RectangleF(Bounds.X + 2.5f * _spacing + 2 * w, Bounds.Bottom + h0 + h2, w, h3),
        new RectangleF(Bounds.X + 3.5f * _spacing + 3 * w, Bounds.Bottom + h0 + h2, w, h3),
        new RectangleF(Bounds.X + 4.5f * _spacing + 4 * w, Bounds.Bottom + h0 + h2, w, h3),
        new RectangleF(Bounds.X + 5.5f * _spacing - 1 + 5 * w, Bounds.Bottom + h0 + h2, w, h3)
      };

      Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + h2 + h3 + _spacing);

      h0 = 0;
      if (_spacerTxt2 != "")
      {
        h0 = 10;
        _spacerBounds2 = new RectangleF(Bounds.X, Bounds.Bottom + _spacing / 2, Bounds.Width, h0);
      }

      // create annotation (x, y, z, xx, yy, zz) placeholders
      _textBounds.AddRange(new List<RectangleF>()
      {
        new RectangleF(Bounds.X + _spacing, Bounds.Bottom + h0, w, h2),
        new RectangleF(Bounds.X + 1.5f * _spacing + w, Bounds.Bottom + h0, w, h2),
        new RectangleF(Bounds.X + 2.5f * _spacing + 2 * w, Bounds.Bottom + h0, w, h2),
        new RectangleF(Bounds.X + 3f * _spacing + 3 * w, Bounds.Bottom + h0, w + _spacing, h2),
        new RectangleF(Bounds.X + 4f * _spacing + 4 * w, Bounds.Bottom + h0, w + _spacing, h2),
        new RectangleF(Bounds.X + 5f * _spacing - 1 + 5 * w, Bounds.Bottom + h0, w + _spacing, h2)
      });

      // create check box placeholders
      _bounds.AddRange(new List<RectangleF>()
      {
        new RectangleF(Bounds.X + _spacing / 2 + 1, Bounds.Bottom + h0 + h2, w, h3),
        new RectangleF(Bounds.X + 1.5f * _spacing + w, Bounds.Bottom + h0 + h2, w, h3),
        new RectangleF(Bounds.X + 2.5f * _spacing + 2 * w, Bounds.Bottom + h0 + h2, w, h3),
        new RectangleF(Bounds.X + 3.5f * _spacing + 3 * w, Bounds.Bottom + h0 + h2, w, h3),
        new RectangleF(Bounds.X + 4.5f * _spacing + 4 * w, Bounds.Bottom + h0 + h2, w, h3),
        new RectangleF(Bounds.X + 5.5f * _spacing - 1 + 5 * w, Bounds.Bottom + h0 + h2, w, h3)
      });
      Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + h2 + h3 + _spacing);
    }

    protected void FixLayout()
    {
      float width = Bounds.Width; // initial component width before UI overrides
      float num = Math.Max(width, MinWidth); // number for new width
      float num2 = 0f; // value for increased width (if any)

      // first check if original component must be widened
      if (num > width)
      {
        num2 = num - width; // change in width
                            // update component bounds to new width
        Bounds = new RectangleF(
            Bounds.X - num2 / 2f,
            Bounds.Y,
            num,
            Bounds.Height);
      }

      // secondly update position of input and output parameter text
      // first find the maximum text width of parameters
      foreach (IGH_Param item in Owner.Params.Output)
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
      foreach (IGH_Param item in Owner.Params.Input)
      {
        if (inputwidth < item.Attributes.Bounds.Width)
          inputwidth = item.Attributes.Bounds.Width;
      }
      foreach (IGH_Param item2 in Owner.Params.Input)
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
        var comp = Owner as GH_Component;

        if (_textBounds[0].Contains(e.CanvasLocation) | _bounds[0].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle X1");
          _restraints[0] = !_restraints[0];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }
        if (_textBounds[1].Contains(e.CanvasLocation) | _bounds[1].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle Y1");
          _restraints[1] = !_restraints[1];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }
        if (_textBounds[2].Contains(e.CanvasLocation) | _bounds[2].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle Z1");
          _restraints[2] = !_restraints[2];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }
        if (_textBounds[3].Contains(e.CanvasLocation) | _bounds[3].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle XX1");
          _restraints[3] = !_restraints[3];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }
        if (_textBounds[4].Contains(e.CanvasLocation) | _bounds[4].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle YY1");
          _restraints[4] = !_restraints[4];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }
        if (_textBounds[5].Contains(e.CanvasLocation) | _bounds[5].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle ZZ1");
          _restraints[5] = !_restraints[5];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }

        if (_textBounds[6].Contains(e.CanvasLocation) | _bounds[6].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle X2");
          _restraints[6] = !_restraints[6];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }
        if (_textBounds[7].Contains(e.CanvasLocation) | _bounds[7].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle Y2");
          _restraints[7] = !_restraints[7];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }
        if (_textBounds[8].Contains(e.CanvasLocation) | _bounds[8].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle Z2");
          _restraints[8] = !_restraints[8];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }
        if (_textBounds[9].Contains(e.CanvasLocation) | _bounds[9].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle XX2");
          _restraints[9] = !_restraints[9];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }
        if (_textBounds[10].Contains(e.CanvasLocation) | _bounds[10].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle YY2");
          _restraints[10] = !_restraints[10];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }
        if (_textBounds[11].Contains(e.CanvasLocation) | _bounds[11].Contains(e.CanvasLocation))
        {
          comp.RecordUndoEvent("Toggle ZZ2");
          _restraints[11] = !_restraints[11];
          _update(_restraints);
          comp.ExpireSolution(true);
          return GH_ObjectResponse.Handled;
        }
      }
      return base.RespondToMouseDown(sender, e);
    }
    bool _mouseOver;

    public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
      foreach (RectangleF bounds in _bounds)
      {
        if (bounds.Contains(e.CanvasLocation))
        {
          _mouseOver = true;
          sender.Cursor = System.Windows.Forms.Cursors.Hand;
          return GH_ObjectResponse.Capture;
        }
      }

      if (_mouseOver)
      {
        _mouseOver = false;
        Instances.CursorServer.ResetCursor(sender);
        return GH_ObjectResponse.Release;
      }

      return base.RespondToMouseMove(sender, e);
    }
    #endregion

    #region Custom Render logic
    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
    {
      base.Render(canvas, graphics, channel);

      if (channel == GH_CanvasChannel.Objects)
      {
        // We need to draw everything outselves.

        Color myColour = Colour.OasysDarkBlue;
        Brush myBrush = new SolidBrush(myColour);

        // Text boxes
        Brush activeFillBrush = myBrush;
        Brush passiveFillBrush = Brushes.LightGray;
        Color borderColour = myColour;
        Color passiveBorder = Color.DarkGray;
        Brush annoText = Brushes.Black;

        Font font = GH_FontServer.Standard;
        int s = 8;
        if (CentralSettings.CanvasFullNames)
        {
          s = 10;
          font = GH_FontServer.Standard;
        }

        // adjust fontsize to high resolution displays
        font = new Font(font.FontFamily, font.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);
        Font sml = GH_FontServer.Small;
        sml = new Font(sml.FontFamily, sml.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);
        var pen = new Pen(borderColour);

        graphics.DrawString(_spacerTxt1, sml, annoText, _spacerBounds1, GH_TextRenderingConstants.CenterCenter);
        graphics.DrawLine(pen, _spacerBounds1.X, _spacerBounds1.Y + _spacerBounds1.Height / 2, _spacerBounds1.X + (_spacerBounds1.Width - GH_FontServer.StringWidth(_spacerTxt1, sml)) / 2 - 4, _spacerBounds1.Y + _spacerBounds1.Height / 2);
        graphics.DrawLine(pen, _spacerBounds1.X + (_spacerBounds1.Width - GH_FontServer.StringWidth(_spacerTxt1, sml)) / 2 + GH_FontServer.StringWidth(_spacerTxt1, sml) + 4, _spacerBounds1.Y + _spacerBounds1.Height / 2, _spacerBounds1.X + _spacerBounds1.Width, _spacerBounds1.Y + _spacerBounds1.Height / 2);

        var text = new List<string>() { "x", "y", "z", "xx", "yy", "zz" };

        for (int i = 0; i < 6; i++)
        {
          graphics.DrawString(text[i], font, annoText, _textBounds[i], GH_TextRenderingConstants.CenterCenter);
          CheckBox.DrawCheckButton(graphics, new PointF(_bounds[i].X + _bounds[i].Width / 2, _bounds[i].Y + _bounds[i].Height / 2), _restraints[i], activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);
        }

        graphics.DrawString(_spacerTxt2, sml, annoText, _spacerBounds2, GH_TextRenderingConstants.CenterCenter);
        graphics.DrawLine(pen, _spacerBounds2.X, _spacerBounds2.Y + _spacerBounds2.Height / 2, _spacerBounds2.X + (_spacerBounds2.Width - GH_FontServer.StringWidth(_spacerTxt2, sml)) / 2 - 4, _spacerBounds2.Y + _spacerBounds2.Height / 2);
        graphics.DrawLine(pen, _spacerBounds2.X + (_spacerBounds2.Width - GH_FontServer.StringWidth(_spacerTxt2, sml)) / 2 + GH_FontServer.StringWidth(_spacerTxt2, sml) + 4, _spacerBounds2.Y + _spacerBounds2.Height / 2, _spacerBounds2.X + _spacerBounds2.Width, _spacerBounds2.Y + _spacerBounds2.Height / 2);

        for (int i = 6; i < 12; i++)
        {
          graphics.DrawString(text[i - 6], font, annoText, _textBounds[i], GH_TextRenderingConstants.CenterCenter);
          CheckBox.DrawCheckButton(graphics, new PointF(_bounds[i].X + _bounds[i].Width / 2, _bounds[i].Y + _bounds[i].Height / 2), _restraints[i], activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);
        }
      }
    }
    #endregion
  }
}
