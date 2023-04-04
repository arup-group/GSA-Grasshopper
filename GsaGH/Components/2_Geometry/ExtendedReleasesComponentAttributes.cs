//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using Grasshopper;
//using Grasshopper.GUI;
//using Grasshopper.GUI.Canvas;
//using Grasshopper.Kernel;
//using Grasshopper.Kernel.Attributes;

//namespace OasysGH.UI.Foo
//{
//  public class ExtendedReleasesComponentAttributes : ReleasesComponentAttributes
//  {
//    public ExtendedReleasesComponentAttributes(GH_Component owner, Action<List<bool>> updateHandle, string spacerText1, string spacerText2, string spacerText3,
//        List<bool> restraints, List<bool> setting) : base(owner, updateHandle, spacerText1, spacerText2, restraints)
//    {
//      _spacerTxt3 = spacerText3;
//      _setting = setting;
//    }

//    #region Custom layout logic
//    List<bool> _setting = new List<bool>();

//    // text boxes bounds for pre-set restraints
//    RectangleF _spacerBounds3;
//    readonly string _spacerTxt3;

//    // annotation text bounds
//    List<RectangleF> _textBounds;

//    // bounds for check boxes
//    List<RectangleF> _bounds = new List<RectangleF>();

//    protected override void Layout()
//    {
//      base.Layout();

//      // Set the actual layout of the component here:

//      // first change the width to suit; using max to determine component visualisation style
//      FixLayout();

//      // spacer and title
//      int h0 = 0;
//      if (_spacerTxt3 != "")
//      {
//        Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height - (CentralSettings.CanvasObjectIcons ? 5 : 0));

//        h0 = 10;
//        _spacerBounds3 = new RectangleF(Bounds.X, Bounds.Bottom + _spacing / 2, Bounds.Width, h0);
//      }

//      float w = (Bounds.Width - 6 * _spacing) / 6; // width of each check box
//      int h2 = 15; // height
//      int h3 = 15;
//      // create annotation (x, y, z, xx, yy, zz) placeholders
//      _textBounds = new List<RectangleF>()
//      {
//        new RectangleF(Bounds.X + _spacing, Bounds.Bottom + h0, w, h2),
//        new RectangleF(Bounds.X + 1.5f * _spacing + w, Bounds.Bottom + h0, w, h2),
//        new RectangleF(Bounds.X + 2.5f * _spacing + 2 * w, Bounds.Bottom + h0, w, h2),
//        new RectangleF(Bounds.X + 3f * _spacing + 3 * w, Bounds.Bottom + h0, w + _spacing, h2),
//        new RectangleF(Bounds.X + 4f * _spacing + 4 * w, Bounds.Bottom + h0, w + _spacing, h2),
//        new RectangleF(Bounds.X + 5f * _spacing - 1 + 5 * w, Bounds.Bottom + h0, w + _spacing, h2)
//      };
//      // create check box placeholders
//      _bounds = new List<RectangleF>()
//      {
//        new RectangleF(Bounds.X + _spacing / 2, Bounds.Bottom + h0 + h2, w, h3),
//        new RectangleF(Bounds.X + 1.5f * _spacing + w, Bounds.Bottom + h0 + h2, w, h3),
//        new RectangleF(Bounds.X + 2.5f * _spacing + 2 * w, Bounds.Bottom + h0 + h2, w, h3),
//        new RectangleF(Bounds.X + 3.5f * _spacing + 3 * w, Bounds.Bottom + h0 + h2, w, h3),
//        new RectangleF(Bounds.X + 4.5f * _spacing + 4 * w, Bounds.Bottom + h0 + h2, w, h3),
//        new RectangleF(Bounds.X + 5.5f * _spacing - 1 + 5 * w, Bounds.Bottom + h0 + h2, w, h3)
//      };

//      Bounds = new RectangleF(Bounds.X, Bounds.Y, Bounds.Width, Bounds.Height + h0 + h2 + h3 + _spacing);
//    }
//    #endregion

//    #region Custom Mouse handling
//    public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
//    {
//      if (e.Button == System.Windows.Forms.MouseButtons.Left)
//      {
//        var comp = Owner as GH_Component;

//        if (_textBounds[0].Contains(e.CanvasLocation) | _bounds[0].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle X1");
//          _setting[0] = !_setting[0];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }
//        if (_textBounds[1].Contains(e.CanvasLocation) | _bounds[1].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle Y1");
//          _setting[1] = !_setting[1];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }
//        if (_textBounds[2].Contains(e.CanvasLocation) | _bounds[2].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle Z1");
//          _setting[2] = !_setting[2];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }
//        if (_textBounds[3].Contains(e.CanvasLocation) | _bounds[3].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle XX1");
//          _setting[3] = !_setting[3];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }
//        if (_textBounds[4].Contains(e.CanvasLocation) | _bounds[4].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle YY1");
//          _setting[4] = !_setting[4];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }
//        if (_textBounds[5].Contains(e.CanvasLocation) | _bounds[5].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle ZZ1");
//          _setting[5] = !_setting[5];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }

//        if (_textBounds[6].Contains(e.CanvasLocation) | _bounds[6].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle X2");
//          _setting[6] = !_setting[6];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }
//        if (_textBounds[7].Contains(e.CanvasLocation) | _bounds[7].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle Y2");
//          _setting[7] = !_setting[7];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }
//        if (_textBounds[8].Contains(e.CanvasLocation) | _bounds[8].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle Z2");
//          _setting[8] = !_setting[8];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }
//        if (_textBounds[9].Contains(e.CanvasLocation) | _bounds[9].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle XX2");
//          _setting[9] = !_setting[9];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }
//        if (_textBounds[10].Contains(e.CanvasLocation) | _bounds[10].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle YY2");
//          _setting[10] = !_setting[10];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }
//        if (_textBounds[11].Contains(e.CanvasLocation) | _bounds[11].Contains(e.CanvasLocation))
//        {
//          comp.RecordUndoEvent("Toggle ZZ2");
//          _setting[11] = !_setting[11];
//          _update(_setting);
//          comp.ExpireSolution(true);
//          return GH_ObjectResponse.Handled;
//        }
//      }
//      return base.RespondToMouseDown(sender, e);
//    }
//    bool _mouseOver;

//    public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
//    {
//      foreach (RectangleF bounds in _bounds)
//      {
//        if (bounds.Contains(e.CanvasLocation))
//        {
//          _mouseOver = true;
//          sender.Cursor = System.Windows.Forms.Cursors.Hand;
//          return GH_ObjectResponse.Capture;
//        }
//      }

//      if (_mouseOver)
//      {
//        _mouseOver = false;
//        Instances.CursorServer.ResetCursor(sender);
//        return GH_ObjectResponse.Release;
//      }

//      return base.RespondToMouseMove(sender, e);
//    }
//    #endregion

//    #region Custom Render logic
//    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
//    {
//      base.Render(canvas, graphics, channel);

//      if (channel == GH_CanvasChannel.Objects)
//      {
//        // We need to draw everything outselves.

//        Color myColour = Colour.OasysDarkBlue;
//        Brush myBrush = new SolidBrush(myColour);

//        // Text boxes
//        Brush activeFillBrush = myBrush;
//        Brush passiveFillBrush = Brushes.LightGray;
//        Color borderColour = myColour;
//        Color passiveBorder = Color.DarkGray;
//        Brush annoText = Brushes.Black;

//        Font font = GH_FontServer.Standard;
//        int s = 8;
//        if (CentralSettings.CanvasFullNames)
//        {
//          _spacing = 10;
//          font = GH_FontServer.Standard;
//        }

//        // adjust fontsize to high resolution displays
//        font = new Font(font.FontFamily, font.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);
//        Font sml = GH_FontServer.Small;
//        sml = new Font(sml.FontFamily, sml.Size / GH_GraphicsUtil.UiScale, FontStyle.Regular);
//        var pen = new Pen(borderColour);

//        //graphics.DrawString(_spacerTxt1, sml, annoText, _spacerBounds1, GH_TextRenderingConstants.CenterCenter);
//        //graphics.DrawLine(pen, _spacerBounds1.X, _spacerBounds1.Y + _spacerBounds1.Height / 2, _spacerBounds1.X + (_spacerBounds1.Width - GH_FontServer.StringWidth(_spacerTxt1, sml)) / 2 - 4, _spacerBounds1.Y + _spacerBounds1.Height / 2);
//        //graphics.DrawLine(pen, _spacerBounds1.X + (_spacerBounds1.Width - GH_FontServer.StringWidth(_spacerTxt1, sml)) / 2 + GH_FontServer.StringWidth(_spacerTxt1, sml) + 4, _spacerBounds1.Y + _spacerBounds1.Height / 2, _spacerBounds1.X + _spacerBounds1.Width, _spacerBounds1.Y + _spacerBounds1.Height / 2);

//        //var text = new List<string>() { "x", "y", "z", "xx", "yy", "zz" };

//        //for (int i = 0; i < 6; i++)
//        //{
//        //  graphics.DrawString(text[i], font, annoText, _textBounds[i], GH_TextRenderingConstants.CenterCenter);
//        //  CheckBox.DrawCheckButton(graphics, new PointF(_bounds[i].X + _bounds[i].Width / 2, _bounds[i].Y + _bounds[i].Height / 2), _setting[i], activeFillBrush, borderColour, passiveFillBrush, passiveBorder, s);
//        //}
//      }
//    }
//    #endregion
//  }
//}
