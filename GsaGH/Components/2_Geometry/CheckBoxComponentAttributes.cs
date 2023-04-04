using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using OasysGH.UI.Helpers;

namespace OasysGH.UI {
  public class CheckBoxComponentComponentAttributes : GH_ComponentAttributes {

    public float MinWidth {
      get => 90;
      set => MinWidth = value;
    }

    public int _h0 = 10;
    public int _h2 = 15;
    public int _h3 = 15;
    public int _spacing = 3;

    private readonly List<List<bool>> _bools = new List<List<bool>>();

    // bounds for check boxes
    private List<List<RectangleF>> _bounds = new List<List<RectangleF>>();
    private readonly List<int> _columns;
    private readonly int _rows;
    private readonly List<string> _spacerTxts;
    private readonly List<List<string>> _text;

    // function that sends back the user input to the component
    private readonly Action<List<List<bool>>> _update;
    private bool _mouseOver;

    // text boxes bounds for pre-set restraints
    private List<RectangleF> _spacerBounds = new List<RectangleF>();

    // annotation text bounds
    private List<List<RectangleF>> _textBounds = new List<List<RectangleF>>();

    public CheckBoxComponentComponentAttributes(GH_Component owner, Action<List<List<bool>>> updateHandle, List<string> spacerTexts, List<List<bool>> restraints, List<List<string>> text) : base(owner) {
      _bools = restraints;
      _update = updateHandle;
      _spacerTxts = spacerTexts;
      _text = text;

      _rows = restraints.Count;
      _columns = new List<int>();
      foreach (List<bool> row in restraints) {
        _columns.Add(row.Count);
      }
      if (spacerTexts.Count != _rows || text.Count != _rows) {
        throw new ArgumentException("Input parameters must have the same size!");
      }
    }

    public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e) {
      if (e.Button == System.Windows.Forms.MouseButtons.Left) {
        var comp = Owner as GH_Component;

        for (int r = 0; r < _rows; r++) {
          for (int c = 0; c < _columns[r]; c++) {

            if (_textBounds[r][c].Contains(e.CanvasLocation) | _bounds[r][c].Contains(e.CanvasLocation)) {
              comp.RecordUndoEvent("Toggle X1");
              _bools[r][c] = !_bools[r][c];
              _update(_bools);
              comp.ExpireSolution(true);
              return GH_ObjectResponse.Handled;
            }
          }
        }
      }
      return base.RespondToMouseDown(sender, e);
    }

    public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e) {
      for (int r = 0; r < _rows; r++) {
        foreach (RectangleF bounds in _bounds[r]) {
          if (bounds.Contains(e.CanvasLocation)) {
            _mouseOver = true;
            sender.Cursor = System.Windows.Forms.Cursors.Hand;
            return GH_ObjectResponse.Capture;
          }
        }
      }

      if (_mouseOver) {
        _mouseOver = false;
        Instances.CursorServer.ResetCursor(sender);
        return GH_ObjectResponse.Release;
      }

      return base.RespondToMouseMove(sender, e);
    }

    protected void FixLayout() {
      float width = Bounds.Width; // initial component width before UI overrides
      float num = Math.Max(width, MinWidth); // number for new width
      float num2 = 0f; // value for increased width (if any)

      // first check if original component must be widened
      if (num > width) {
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
      foreach (IGH_Param item in Owner.Params.Output) {
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
      foreach (IGH_Param item in Owner.Params.Input) {
        if (inputwidth < item.Attributes.Bounds.Width)
          inputwidth = item.Attributes.Bounds.Width;
      }
      foreach (IGH_Param item2 in Owner.Params.Input) {
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

    // spacing to edges and internal between boxes
    protected override void Layout() {
      base.Layout();

      // first change the width to suit; using max to determine component visualisation style
      FixLayout();


      // spacer and title
      _spacerBounds = new List<RectangleF>();
      _bounds = new List<List<RectangleF>>();
      _textBounds = new List<List<RectangleF>>();

      for (int r = 0; r < _rows; r++) {
        // width of each check box
        float w = (Bounds.Width - _columns[r] * _spacing) / _columns[r];

        int h0 = 0;
        if (_spacerTxts[r] != "") {
          if (r == 0) { // ?
            Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height - (CentralSettings.CanvasObjectIcons ? 5 : 0));
          }
          h0 = _h0;
          _spacerBounds.Add(new RectangleF(Bounds.X, Bounds.Bottom + _spacing / 2, Bounds.Width, h0));
        }

        // create annotation (x, y, z, xx, yy, zz) placeholders
        _textBounds.Add(new List<RectangleF>() {
          new RectangleF(Bounds.X + _spacing, Bounds.Bottom + h0, w, _h2),
          new RectangleF(Bounds.X + 1.5f * _spacing + w, Bounds.Bottom + h0, w, _h2),
          new RectangleF(Bounds.X + 2.5f * _spacing + 2 * w, Bounds.Bottom + h0, w, _h2),
          new RectangleF(Bounds.X + 3f * _spacing + 3 * w, Bounds.Bottom + h0, w + _spacing, _h2),
          new RectangleF(Bounds.X + 4f * _spacing + 4 * w, Bounds.Bottom + h0, w + _spacing, _h2),
          new RectangleF(Bounds.X + 5f * _spacing - 1 + 5 * w, Bounds.Bottom + h0, w + _spacing, _h2)
        });

        // create check box placeholders
        _bounds.Add(new List<RectangleF>() {
          new RectangleF(Bounds.X + _spacing / 2, Bounds.Bottom + h0 + _h2, w, _h3),
          new RectangleF(Bounds.X + 1.5f * _spacing + w, Bounds.Bottom + h0 + _h2, w, _h3),
          new RectangleF(Bounds.X + 2.5f * _spacing + 2 * w, Bounds.Bottom + h0 + _h2, w, _h3),
          new RectangleF(Bounds.X + 3.5f * _spacing + 3 * w, Bounds.Bottom + h0 + _h2, w, _h3),
          new RectangleF(Bounds.X + 4.5f * _spacing + 4 * w, Bounds.Bottom + h0 + _h2, w, _h3),
          new RectangleF(Bounds.X + 5.5f * _spacing - 1 + 5 * w, Bounds.Bottom + h0 + _h2, w, _h3)
        });

        Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + _h2 + _h3 + _spacing);
      }
    }

    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel) {
      base.Render(canvas, graphics, channel);

      if (channel == GH_CanvasChannel.Objects) {
        // Text boxes
        Brush activeFillBrush = new SolidBrush(Colour.OasysDarkBlue);
        Brush passiveFillBrush = Brushes.LightGray;
        Color borderColour = Colour.OasysDarkBlue;
        Color passiveBorder = Color.DarkGray;
        Brush annoText = Brushes.Black;

        Font font = GH_FontServer.Standard;
        int s = 8;
        if (CentralSettings.CanvasFullNames) {
          s = 10;
          font = GH_FontServer.Standard;
        }

        // adjust fontsize to high resolution displays
        font = new Font(font.FontFamily, font.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);
        Font sml = GH_FontServer.Small;
        sml = new Font(sml.FontFamily, sml.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);
        var pen = new Pen(borderColour);

        for (int r = 0; r < _rows; r++) {

          graphics.DrawString(_spacerTxts[0], sml, annoText, _spacerBounds[r], GH_TextRenderingConstants.CenterCenter);
          graphics.DrawLine(pen, _spacerBounds[r].X, _spacerBounds[r].Y + _spacerBounds[r].Height / 2, _spacerBounds[r].X + (_spacerBounds[r].Width - GH_FontServer.StringWidth(_spacerTxts[0], sml)) / 2 - 4, _spacerBounds[r].Y + _spacerBounds[r].Height / 2);
          graphics.DrawLine(pen, _spacerBounds[r].X + (_spacerBounds[r].Width - GH_FontServer.StringWidth(_spacerTxts[0], sml)) / 2 + GH_FontServer.StringWidth(_spacerTxts[0], sml) + 4, _spacerBounds[r].Y + _spacerBounds[r].Height / 2, _spacerBounds[r].X + _spacerBounds[r].Width, _spacerBounds[r].Y + _spacerBounds[r].Height / 2);

          for (int c = 0; c < _columns[r]; c++) {
            graphics.DrawString(_text[r][c], font, annoText, _textBounds[r][c], GH_TextRenderingConstants.CenterCenter);
            CheckBox.DrawCheckButton(graphics, new PointF(_bounds[r][c].X + _bounds[r][c].Width / 2, _bounds[r][c].Y + _bounds[r][c].Height / 2), _bools[r][c], activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);
          }
        }
      }
    }
  }
}
