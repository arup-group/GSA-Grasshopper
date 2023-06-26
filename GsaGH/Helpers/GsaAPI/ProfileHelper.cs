using System.Collections.Generic;
using System.ComponentModel;
using Rhino.Geometry;

namespace GsaGH.Helpers.GsaApi {
  /// <summary>
  ///   Helper class for Profile/Section conversions
  /// </summary>
  public class ConvertSection {

    /// <summary>
    ///   Method to convert a GsaProfile to a string that can be read by GSA
    ///   (in GsaAPI.Section.Profile or GsaGH.Parameters.GsaSection.Section.Profile)
    ///   NOTE:
    ///   - Does not cover all profile types available in GSA (but all available in GsaProfile)
    ///   - Geometric can handle custom profiles with voids. Origin/anchor to be implemented.
    ///   - Catalogue profiles yet to be implemented
    /// </summary>
    /// <param name="gsaProfile"></param>
    /// <returns></returns>
    public static string ProfileConversion(ProfileHelper gsaProfile) {
      switch (gsaProfile.ProfileType) {
        case ProfileHelper.ProfileTypes.Standard: {
          string unit = " ";

          switch (gsaProfile.SectUnit) {
            case ProfileHelper.SectUnitOptions.UCm:
              unit = "(cm) ";
              break;

            case ProfileHelper.SectUnitOptions.Um:
              unit = "(m) ";
              break;

            case ProfileHelper.SectUnitOptions.UIn:
              unit = "(in) ";
              break;

            case ProfileHelper.SectUnitOptions.UFt:
              unit = "(ft) ";
              break;
          }

          switch (gsaProfile.StdShape) {
            case ProfileHelper.StdShapeOptions.Rectangle when gsaProfile.IsTapered:
              return
                $"STD TR{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.B2:0.############}";

            case ProfileHelper.StdShapeOptions.Rectangle when gsaProfile.IsHollow:
              return
                $"STD RHS{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.Tw1:0.############} {gsaProfile.Tf1:0.############}";

            case ProfileHelper.StdShapeOptions.Rectangle:
              return $"STD R{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############}";

            case ProfileHelper.StdShapeOptions.Circle when gsaProfile.IsHollow: {
              return gsaProfile.IsElliptical ?
                $"STD OVAL{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.Tw1:0.############}" :
                $"STD CHS{unit}{gsaProfile.D:0.############} {gsaProfile.Tw1:0.############}";
            }
            case ProfileHelper.StdShapeOptions.Circle when gsaProfile.IsElliptical:
              return $"STD E{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} 2";

            case ProfileHelper.StdShapeOptions.Circle:
              return $"STD C{unit}{gsaProfile.D:0.############}";

            case ProfileHelper.StdShapeOptions.Section when gsaProfile.IsGeneral: {
              return gsaProfile.IsTapered ?
                $"STD TI{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.B2:0.############} {gsaProfile.Tw1:0.############} {gsaProfile.Tw2:0.############} {gsaProfile.Tf1:0.############} {gsaProfile.Tf2:0.############}" :
                $"STD GI{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.B2:0.############} {gsaProfile.Tw1:0.############} {gsaProfile.Tf1:0.############} {gsaProfile.Tf2:0.############}";
            }
            case ProfileHelper.StdShapeOptions.Section:
              return
                $"STD I{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.Tw1:0.############} {gsaProfile.Tf1:0.############}";

            case ProfileHelper.StdShapeOptions.Tee when gsaProfile.IsTapered:
              return
                $"STD TT{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.Tw1:0.############} {gsaProfile.Tw2:0.############} {gsaProfile.Tf1:0.############}";

            case ProfileHelper.StdShapeOptions.Tee:
              return
                $"STD T{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.Tw1:0.############} {gsaProfile.Tf1:0.############}";

            case ProfileHelper.StdShapeOptions.Channel when gsaProfile.IsB2B:
              return
                $"STD DCH{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.Tw1:0.############} {gsaProfile.Tf1:0.############}";

            case ProfileHelper.StdShapeOptions.Channel:
              return
                $"STD CH{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.Tw1:0.############} {gsaProfile.Tf1:0.############}";

            case ProfileHelper.StdShapeOptions.Angle when gsaProfile.IsB2B:
              return
                $"STD D{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.Tw1:0.############} {gsaProfile.Tf1:0.############}";

            case ProfileHelper.StdShapeOptions.Angle:
              return
                $"STD A{unit}{gsaProfile.D:0.############} {gsaProfile.B1:0.############} {gsaProfile.Tw1:0.############} {gsaProfile.Tf1:0.############}";

            default: return "STD something else";
          }
        }
        case ProfileHelper.ProfileTypes.Catalogue: {
          string outputSectionString = string.Empty;

          if (gsaProfile.CatalogueProfileName != null) {
            outputSectionString = gsaProfile.CatalogueProfileName;
          }

          return $"CAT {outputSectionString}";
        }
        case ProfileHelper.ProfileTypes.Geometric
          when gsaProfile.GeoType == ProfileHelper.GeoTypes.Perim: {
          string unit = string.Empty;

          switch (gsaProfile.SectUnit) {
            case ProfileHelper.SectUnitOptions.UMm:
              unit = "(mm)";
              break;

            case ProfileHelper.SectUnitOptions.UCm:
              unit = "(cm)";
              break;

            case ProfileHelper.SectUnitOptions.Um:
              unit = "(m)";
              break;

            case ProfileHelper.SectUnitOptions.UIn:
              unit = "(in)";
              break;

            case ProfileHelper.SectUnitOptions.UFt:
              unit = "(ft)";
              break;
          }

          string profile = $"GEO P{unit}";
          int iPoint = 0;
          foreach (Point2d point in gsaProfile.PerimeterPoints) {
            if (iPoint > 0) {
              profile += " L";
            } else {
              profile += " M";
            }

            profile += $"({point.X}|{point.Y})";
            iPoint++;
          }

          if (gsaProfile.VoidPoints == null || !(gsaProfile.VoidPoints.Count > 0)) {
            return profile;
          }

          {
            foreach (List<Point2d> voidPoint in gsaProfile.VoidPoints) {
              iPoint = 0;
              foreach (Point2d point in voidPoint) {
                if (iPoint > 0) {
                  profile += " L";
                } else {
                  profile += " M";
                }

                profile += $"({point.X}|{point.Y})";
                iPoint++;
              }
            }
          }

          return profile;
        }
        case ProfileHelper.ProfileTypes.Geometric: return "GEO";

        default: return null;
      }
    }
  }

  /// <summary>
  ///   Profile class holds information about a profile
  ///   : Standard, Catalogue or Geometric
  ///   ShapeOptions for Standard type
  ///   Section units
  /// </summary>
  public class ProfileHelper {
    public enum GeoTypes {
      [Description("Perimeter")] Perim,
    }

    public enum ProfileTypes {
      [Description("Standard")] Standard,
      [Description("Catalogue")] Catalogue,
      [Description("Geometric")] Geometric,
    }

    public enum SectUnitOptions {
      [Description("mm")] UMm,
      [Description("cm")] UCm,
      [Description("m")] Um,
      [Description("ft")] UFt,
      [Description("in")] UIn,
    }

    public enum StdShapeOptions {
      [Description("Rectangle")] Rectangle,
      [Description("Circle")] Circle,
      [Description("Section")] Section,
      [Description("Tee")] Tee,
      [Description("Channel")] Channel,
      [Description("Angle")] Angle,
    }

    public double B1;
    public double B2;
    public string CatalogueProfileName = string.Empty;
    public double D;
    public GeoTypes GeoType;
    public bool IsB2B;
    public bool IsElliptical;
    public bool IsGeneral;
    public bool IsHollow;
    public bool IsTapered;
    public List<Point2d> PerimeterPoints;
    public ProfileTypes ProfileType;
    public SectUnitOptions SectUnit = SectUnitOptions.UMm;
    public StdShapeOptions StdShape;
    public double Tf1;
    public double Tf2;
    public double Tw1;
    public double Tw2;
    public List<List<Point2d>> VoidPoints;
  }
}
