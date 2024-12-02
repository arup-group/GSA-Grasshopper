using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Grasshopper.GUI;
using Grasshopper.Kernel;

using OasysGH.Components;

using Rhino;
using Rhino.Display;
using Rhino.Geometry;

namespace GsaGH.Helpers.GH {
  internal class ContourLegend {
    public ContourLegendConfiguration Configuration { get; private set; }

    private const int DefaultTextHeight = 12;
    private const int DefaultBitmapWidth = 110;

    private int _textHeight = DefaultTextHeight;
    private int _leftBitmapEdge;
    private bool _isDrawLegendCalled = false;

    private string _scaleLegendTxt;

    public ContourLegend() {
      Configuration = new ContourLegendConfiguration();
    }

    public ToolStripMenuItem CreateLegendToolStripMenuItem(GH_OasysDropDownComponent component, Action updateUI) {
      var legendScale = new ToolStripTextBox {
        Text = Configuration.Scale.ToString(),
      };
      legendScale.TextChanged += (s, e) => MaintainScaleLegendText(legendScale);
      var legendScaleMenu = new ToolStripMenuItem("Scale Legend") {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      var menu2 = new GH_MenuCustomControl(legendScaleMenu.DropDown, legendScale.Control, true, 200);
      legendScaleMenu.DropDownItems[1].MouseUp += (s, e) => {
        UpdateLegendScale(component, updateUI);
        (component as IGH_VariableParameterComponent).VariableParameterMaintenance();
        component.ExpireSolution(true);
      };

      return legendScaleMenu;
    }

    /// <summary>
    ///   Draws the complete legend including the rectangle, title, values, and bottom text.
    /// </summary>
    public void DrawLegendRectangle(IGH_PreviewArgs args, string title, string bottomText) {
      if (!IsDrawable()) {
        return;
      }

      _isDrawLegendCalled = true;
      InitializeDimensions(args.Viewport.Bounds.Right);

      DrawBitmap(args);
      DrawTitle(args, title);
      DrawValues(args);
      DrawBottomText(args, bottomText);

      _isDrawLegendCalled = false;
    }

    public void DrawBitmap(IGH_PreviewArgs args) {
      EnsureDimensionsInitialized(args);
      const int TopOffset = 20;
      int topPosition = CalculateScaledOffset(TopOffset);

      args.Display.DrawBitmap(new DisplayBitmap(Configuration.Bitmap), _leftBitmapEdge, topPosition);
    }

    public void DrawTitle(IGH_PreviewArgs args, string title) {
      EnsureDimensionsInitialized(args);
      const int TopOffset = 7;
      int topPosition = CalculateScaledOffset(TopOffset);

      args.Display.Draw2dText(title, Color.Black, new Point2d(_leftBitmapEdge, topPosition), false, _textHeight);
    }

    public void DrawValues(IGH_PreviewArgs args) {
      EnsureDimensionsInitialized(args);
      const int LeftOffset = 25;
      int leftEdge = _leftBitmapEdge + CalculateScaledOffset(LeftOffset);
      var zippedLists = Configuration.Values.Zip(Configuration.ValuePositionsY, (value, positionY) => new {
        value,
        positionY,
      });
      foreach (var pair in zippedLists) {
        args.Display.Draw2dText(pair.value, Color.Black, new Point2d(leftEdge, pair.positionY), false, _textHeight);
      }
    }

    public void DrawBottomText(IGH_PreviewArgs args, string bottomText) {
      EnsureDimensionsInitialized(args);
      const int BottomOffset = 145;
      const int ExtraOffset = 20;
      int bitmapWidth = CalculateScaledOffset(DefaultBitmapWidth);
      int bitmapWidthWithOffset = bitmapWidth + CalculateScaledOffset(ExtraOffset);
      int topPosition = CalculateScaledOffset(BottomOffset);

      string wrappedText = WrapText(bottomText, bitmapWidthWithOffset);
      args.Display.Draw2dText(wrappedText, Color.Black, new Point2d(_leftBitmapEdge, topPosition), false, _textHeight);
    }

    private bool IsDrawable() {
      return Configuration != null && Configuration.IsLegendDisplayable();
    }

    private void InitializeDimensions(int viewportEdge) {
      _textHeight = CalculateScaledOffset(DefaultTextHeight);
      _leftBitmapEdge = viewportEdge - CalculateScaledOffset(DefaultBitmapWidth);
    }

    /// <summary>
    ///   Ensures dimensions are initialized for individual drawing calls.
    /// </summary>
    private void EnsureDimensionsInitialized(IGH_PreviewArgs args) {
      if (!_isDrawLegendCalled) {
        InitializeDimensions(args.Viewport.Bounds.Right);
      }
    }

    private string WrapText(string bottomText, int width) {
      var font = new Font(RhinoDoc.ActiveDoc.DimStyles.Current.Font.LogfontName, _textHeight);
      return TextWrapper.WrapText(bottomText, width, font);
    }

    private int CalculateScaledOffset(int value) {
      return (int)(value * Configuration.Scale);
    }

    #region Legend toolstrip menu helpers

    private void UpdateLegendScale(GH_OasysDropDownComponent component, Action updateUI) {
      try {
        Configuration.SetLegendScale(double.Parse(_scaleLegendTxt));
      } catch (Exception e) {
        component.AddRuntimeWarning("Invalid scale value. Please enter a valid number.");
      }

      Configuration.ScaleBitmap();
      component.ExpirePreview(true);
      updateUI();
    }

    private void MaintainScaleLegendText(ToolStripItem menuitem) {
      _scaleLegendTxt = menuitem.Text;
    }

    #endregion

  }
}
