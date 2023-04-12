using System.Collections.Generic;
using System.Drawing;
using Rhino.Display;

namespace GsaGH.Helpers.Graphics {
  /// <summary>
  /// Colour class holding the main colours used in colour scheme.
  /// Make calls to this class to be able to easy update colours.
  ///
  /// </summary>
  public class Colours {
    public static Brush ActiveBrush {
      get { return new SolidBrush(ActiveColour); }
    }
    public static Color ActiveColour {
      get { return GsaDarkBlue; }
    }
    public static Brush AnnotationTextBright {
      get { return Brushes.White; }
    }
    public static Brush AnnotationTextDark {
      get { return Brushes.Black; }
    }
    public static Brush AnnotationTextDarkGrey {
      get { return new SolidBrush(GsaDarkGrey); }
    }
    public static Color BorderColour {
      get { return GsaLightGrey; }
    }
    public static Color ButtonBorderColour {
      get { return GsaLightGrey; }
    }
    public static Brush ButtonColour {
      get { return new SolidBrush(GsaDarkBlue); }
    }
    public static Color ClickedBorderColour {
      get { return Color.White; }
    }
    public static Brush ClickedButtonColour {
      get { return new SolidBrush(Graphics.Colours.WhiteOverlay(GsaDarkBlue, 0.32)); }
    }
    public static Color Dummy1D {
      get { return Color.FromArgb(255, 143, 143, 143); }
    }
    public static DisplayMaterial Dummy2D {
      get {
        var material = new DisplayMaterial {
          Diffuse = Color.FromArgb(1, 143, 143, 143),
          Emission = Color.White,
          Transparency = 0.9,
        };
        return material;
      }
    }
    public static Color Element1d {
      get { return Color.FromArgb(255, 95, 190, 180); }
    }
    public static Color Element1dNode {
      get { return GsaDarkGreen; }
    }
    public static Color Element1dNodeSelected {
      get { return GsaDarkGreen; }
    }
    public static Color Element1dSelected {
      get { return GsaDarkPurple; }
    }
    public static Color Element2dEdge {
      get { return GsaBlue; }
    }
    public static Color Element2dEdgeSelected {
      get { return GsaDarkPurple; }
    }
    public static DisplayMaterial Element2dFace {
      get {
        var material = new DisplayMaterial {
          Diffuse = Color.FromArgb(50, 150, 150, 150),
          Emission = Color.FromArgb(50, 190, 190, 190),
          Transparency = 0.1,
        };
        return material;
      }
    }
    public static DisplayMaterial Element2dFaceSelected {
      get {
        var material = new DisplayMaterial {
          Diffuse = Color.FromArgb(5, 150, 150, 150),
          Emission = Color.FromArgb(5, 150, 150, 150),
          Transparency = 0.2,
        };
        return material;
      }
    }
    public static DisplayMaterial Element3dFace {
      get {
        var material = new DisplayMaterial {
          Diffuse = Color.FromArgb(50, 150, 150, 150),
          Emission = Color.FromArgb(50, 190, 190, 190),
          Transparency = 0.3,
        };
        return material;
      }
    }
    public static Color GsaBlue {
      get { return Color.FromArgb(255, 99, 148, 237); }
    }
    public static Color GsaDarkBlue {
      get { return Color.FromArgb(255, 0, 92, 175); }
    }
    public static Color GsaDarkGreen {
      get { return Color.FromArgb(255, 27, 141, 133); }
    }
    public static Color GsaDarkGrey {
      get { return Color.FromArgb(255, 164, 164, 164); }
    }
    public static Color GsaDarkPurple {
      get { return Color.FromArgb(255, 136, 0, 136); }
    }
    public static Color GsaGold {
      get { return Color.FromArgb(255, 255, 183, 0); }
    }
    public static Color GsaGreen {
      get { return Color.FromArgb(255, 48, 170, 159); }
    }
    public static Color GsaLightBlue {
      get { return Color.FromArgb(255, 130, 169, 241); }
    }

    public static Color GsaLightGrey {
      get { return Color.FromArgb(255, 244, 244, 244); }
    }
    public static Color HoverBorderColour {
      get { return Color.White; }
    }
    public static Brush HoverButtonColour {
      get { return new SolidBrush(Graphics.Colours.WhiteOverlay(GsaDarkBlue, 0.16)); }
    }
    public static Brush HoverInactiveButtonColour {
      get { return new SolidBrush(Color.FromArgb(255, 216, 216, 216)); }
    }
    public static Brush InactiveBorderColor {
      get { return new SolidBrush(Color.FromArgb(255, 216, 216, 216)); }
    }
    public static Brush InactiveButtonColour {
      get { return new SolidBrush(GsaLightGrey); }
    }
    public static Color Member1d {
      get { return GsaGreen; }
    }
    public static Color Member1dNode {
      get { return GsaDarkGreen; }
    }
    public static Color Member1dNodeSelected {
      get { return GsaGold; }
    }
    public static Color Member1dSelected {
      get { return GsaDarkPurple; }
    }
    public static Color Member2dEdge {
      get { return GsaBlue; }
    }
    public static Color Member2dEdgeSelected {
      get { return GsaDarkPurple; }
    }
    public static DisplayMaterial Member2dFace {
      get {
        var material = new DisplayMaterial {
          Diffuse = Color.FromArgb(50, 150, 150, 150),
          Emission = Color.FromArgb(50, 45, 45, 45),
          Transparency = 0.1,
        };
        return material;
      }
    }
    public static DisplayMaterial Member2dFaceSelected {
      get {
        var material = new DisplayMaterial {
          Diffuse = Color.FromArgb(5, 150, 150, 150),
          Emission = Color.FromArgb(5, 5, 5, 5),
          Transparency = 0.2,
        };
        return material;
      }
    }
    public static Color Member2dInclLn {
      get { return GsaGold; }
    }
    public static Color Member2dInclPt {
      get { return GsaGold; }
    }
    public static DisplayMaterial Member2dVoidCutterFace {
      get {
        var material = new DisplayMaterial {
          Diffuse = Color.FromArgb(50, 200, 0, 0),
          Emission = Color.FromArgb(50, 45, 45, 45),
          Transparency = 0.6,
        };
        return material;
      }
    }
    public static Color Node {
      get { return GsaGreen; }
    }
    public static Color NodeSelected {
      get { return GsaDarkPurple; }
    }
    public static Color Release {
      get { return Color.FromArgb(255, 153, 32, 32); }
    }
    public static Color SpacerColour {
      get { return GsaDarkBlue; }
    }
    public static Color Support {
      get { return Color.FromArgb(255, 0, 100, 0); }
    }
    public static DisplayMaterial SupportSymbol {
      get {
        var material = new DisplayMaterial() {
          Diffuse = Color.FromArgb(255, Support.R, Support.G, Support.B),
          Emission = Color.FromArgb(255, 50, 50, 50),
          Transparency = 0.2,
        };
        return material;
      }
    }
    public static DisplayMaterial SupportSymbolSelected {
      get {
        var material = new DisplayMaterial() {
          Diffuse = Color.FromArgb(255, NodeSelected.R, NodeSelected.G, NodeSelected.B),
          Emission = Color.FromArgb(255, 50, 50, 50),
          Transparency = 0.2,
        };
        return material;
      }
    }
    public static Color VoidCutter {
      get { return Color.FromArgb(255, 200, 0, 0); }
    }
    public static Color ElementType(global::GsaAPI.ElementType elementType) {
      switch ((int)elementType) {
        case 1:
          return Color.FromArgb(255, 72, 99, 254);

        case 2:
          return Color.FromArgb(255, 95, 190, 180);

        case 23:
          return Color.FromArgb(255, 39, 52, 147);

        case 3:
          return Color.FromArgb(255, 73, 101, 101);

        case 21:
          return Color.FromArgb(255, 200, 81, 45);

        case 20:
          return Color.FromArgb(255, 192, 67, 255);

        case 9:
          return Color.FromArgb(255, 178, 178, 178);

        case 10:
          return Color.FromArgb(255, 32, 32, 32);

        case 24:
          return Color.FromArgb(255, 51, 82, 82);

        case 19:
          return Color.FromArgb(255, 155, 18, 214);

        default:
          return Color.FromArgb(255, 95, 190, 180);
      }
    }

    public static DisplayMaterial FaceCustom(Color colour) {
      var material = new DisplayMaterial() {
        Diffuse = Color.FromArgb(50, colour.R, colour.G, colour.B),
        Emission = Color.White,
        Transparency = 0.1,
      };
      return material;
    }

    public static Color Overlay(Color original, Color overlay, double ratio) {
      return Color.FromArgb(255,
          (int)(ratio * overlay.R + (1 - ratio) * original.R),
          (int)(ratio * overlay.G + (1 - ratio) * original.G),
          (int)(ratio * overlay.B + (1 - ratio) * original.B));
    }

    public static Grasshopper.GUI.Gradient.GH_Gradient Stress_Gradient(List<Color> colours = null) {
      var gHGradient = new Grasshopper.GUI.Gradient.GH_Gradient();

      if (colours == null || colours.Count < 2) {
        gHGradient.AddGrip(-1, Color.FromArgb(0, 0, 206));
        gHGradient.AddGrip(-0.666, Color.FromArgb(0, 127, 229));
        gHGradient.AddGrip(-0.333, Color.FromArgb(90, 220, 186));
        gHGradient.AddGrip(0, Color.FromArgb(205, 254, 114));
        gHGradient.AddGrip(0.333, Color.FromArgb(255, 220, 71));
        gHGradient.AddGrip(0.666, Color.FromArgb(255, 127, 71));
        gHGradient.AddGrip(1, Color.FromArgb(205, 0, 71));
      }
      else {
        for (int i = 0; i < colours.Count; i++) {
          double t = 1.0 - 2.0 / (colours.Count - 1.0) * i;
          gHGradient.AddGrip(t, colours[i]);
        }
      }

      return gHGradient;
    }

    public static Color WhiteOverlay(Color original, double ratio) {
      Color white = Color.White;
      return Color.FromArgb(255,
          (int)(ratio * white.R + (1 - ratio) * original.R),
          (int)(ratio * white.G + (1 - ratio) * original.G),
          (int)(ratio * white.B + (1 - ratio) * original.B));
    }
  }
}
