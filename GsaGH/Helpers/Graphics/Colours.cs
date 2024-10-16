using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

using Grasshopper.GUI.Gradient;

using GsaAPI;

using Rhino.Display;

namespace GsaGH.Helpers.Graphics {
  /// <summary>
  ///   Colour class holding the main colours used in colour scheme.
  ///   Make calls to this class to be able to easy update colours.
  /// </summary>
  [ExcludeFromCodeCoverage]
  public class Colours {
    public static Color EntityIsNotSelected => Color.FromArgb(255, 150, 0, 0);
    public static Brush ActiveBrush => new SolidBrush(ActiveColour);
    public static Color ActiveColour => GsaDarkBlue;
    public static Brush AnnotationTextBright => Brushes.White;
    public static Brush AnnotationTextDark => Brushes.Black;
    public static Brush AnnotationTextDarkGrey => new SolidBrush(GsaDarkGrey);
    public static Color Assembly => Color.FromArgb(255, 119, 158, 0);
    public static Color BorderColour => GsaLightGrey;
    public static Color ButtonBorderColour => GsaLightGrey;
    public static Brush ButtonColour => new SolidBrush(GsaDarkBlue);
    public static Color ClickedBorderColour => Color.White;
    public static Brush ClickedButtonColour => new SolidBrush(WhiteOverlay(GsaDarkBlue, 0.32));
    public static Color Dummy1D => Color.FromArgb(255, 143, 143, 143);
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
    public static Color Element1d => Color.FromArgb(255, 95, 190, 180);
    public static Color Element1dNode => GsaDarkGreen;
    public static Color Element1dNodeSelected => GsaDarkGreen;
    public static Color Element1dSelected => GsaDarkPurple;
    public static Color Element2dEdge => GsaBlue;
    public static Color Element2dEdgeSelected => GsaDarkPurple;
    public static Color Preview3dMeshDefault => Color.FromArgb(200, 220, 220, 220);
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

    private static Color Element2dFaceColorForLoadPanel = Color.FromArgb(50, 195, 218, 219);

    public static DisplayMaterial Element2dFaceLoadPanel {
      get {
        var material = new DisplayMaterial {
          Diffuse = Element2dFaceColorForLoadPanel,
          Emission = Color.FromArgb(50, 75, 218, 219),
          Transparency = 0.1,
        };
        return material;
      }
    }

    public static DisplayMaterial Element2dFaceSelectedLoadPanel {
      get {
        var material = new DisplayMaterial {
          Diffuse = Element2dFaceColorForLoadPanel,
          Transparency = 0.4,
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
    public static Color GsaBlue => Color.FromArgb(255, 99, 148, 237);
    public static Color GsaDarkBlue => Color.FromArgb(255, 0, 92, 175);
    public static Color GsaDarkGreen => Color.FromArgb(255, 27, 141, 133);
    public static Color GsaDarkGrey => Color.FromArgb(255, 164, 164, 164);
    public static Color GsaDarkPurple => Color.FromArgb(255, 136, 0, 136);
    public static Color GsaGold => Color.FromArgb(255, 255, 183, 0);
    public static Color GsaGreen => Color.FromArgb(255, 48, 170, 159);
    public static Color GsaLightBlue => Color.FromArgb(255, 130, 169, 241);

    public static Color GsaLightGrey => Color.FromArgb(255, 244, 244, 244);
    public static Color HoverBorderColour => Color.White;
    public static Brush HoverButtonColour => new SolidBrush(WhiteOverlay(GsaDarkBlue, 0.16));
    public static Brush HoverInactiveButtonColour
      => new SolidBrush(Color.FromArgb(255, 216, 216, 216));
    public static Brush InactiveBorderColor => new SolidBrush(Color.FromArgb(255, 216, 216, 216));
    public static Brush InactiveButtonColour => new SolidBrush(GsaLightGrey);
    public static Color Member1d => GsaGreen;
    public static Color Member1dNode => GsaDarkGreen;
    public static Color Member1dNodeSelected => GsaGold;
    public static Color Member1dSelected => GsaDarkPurple;
    public static Color Member2dEdge => GsaBlue;
    public static Color Member2dEdgeSelected => GsaDarkPurple;
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
    public static Color Member2dInclLn => GsaGold;
    public static Color Member2dInclPt => GsaGold;
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
    public static Color Node => GsaGreen;
    public static Color NodeSelected => GsaDarkPurple;
    public static Color Release => Color.FromArgb(255, 153, 32, 32);
    public static Color SpacerColour => GsaDarkBlue;
    public static Color Support => Color.FromArgb(255, 0, 100, 0);
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
    public static Color VoidCutter => Color.FromArgb(255, 200, 0, 0);

    public static Color ElementType(ElementType elementType) {
      return (int)elementType switch {
        1 => Color.FromArgb(255, 72, 99, 254),
        2 => Color.FromArgb(255, 95, 190, 180),
        23 => Color.FromArgb(255, 39, 52, 147),
        3 => Color.FromArgb(255, 73, 101, 101),
        21 => Color.FromArgb(255, 200, 81, 45),
        20 => Color.FromArgb(255, 192, 67, 255),
        9 => Color.FromArgb(255, 178, 178, 178),
        10 => Color.FromArgb(255, 32, 32, 32),
        24 => Color.FromArgb(255, 51, 82, 82),
        19 => Color.FromArgb(255, 155, 18, 214),
        _ => Color.FromArgb(255, 95, 190, 180),
      };
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
      return Color.FromArgb(255, (int)((ratio * overlay.R) + ((1 - ratio) * original.R)),
        (int)((ratio * overlay.G) + ((1 - ratio) * original.G)),
        (int)((ratio * overlay.B) + ((1 - ratio) * original.B)));
    }

    public static GH_Gradient Stress_Gradient(List<Color> colours = null) {
      var gHGradient = new GH_Gradient();

      if (colours == null || colours.Count < 2) {
        gHGradient.AddGrip(-1, Color.FromArgb(0, 0, 206));
        gHGradient.AddGrip(-0.666, Color.FromArgb(0, 127, 229));
        gHGradient.AddGrip(-0.333, Color.FromArgb(90, 220, 186));
        gHGradient.AddGrip(0, Color.FromArgb(205, 254, 114));
        gHGradient.AddGrip(0.333, Color.FromArgb(255, 220, 71));
        gHGradient.AddGrip(0.666, Color.FromArgb(255, 127, 71));
        gHGradient.AddGrip(1, Color.FromArgb(205, 0, 71));
      } else {
        for (int i = 0; i < colours.Count; i++) {
          double t = 1.0 - (2.0 / (colours.Count - 1.0) * i);
          gHGradient.AddGrip(t, colours[i]);
        }
      }

      return gHGradient;
    }

    public static Color WhiteOverlay(Color original, double ratio) {
      Color white = Color.White;
      return Color.FromArgb(255, (int)((ratio * white.R) + ((1 - ratio) * original.R)),
        (int)((ratio * white.G) + ((1 - ratio) * original.G)),
        (int)((ratio * white.B) + ((1 - ratio) * original.B)));
    }
  }
}
