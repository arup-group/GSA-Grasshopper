using System.Collections.Generic;
using System.ComponentModel;
using Rhino.Geometry;

namespace GsaGH.Helpers.GsaAPI {
  /// <summary>
  /// Profile class holds information about a profile
  /: Standard, Catalogue or Geometric
  /// ShapeOptions for Standard type
  /// Isection units
  /// </summary>
  public class ProfileHelper {
    public enum ProfileTypes {
      [Description("Standard")] Standard,
      [Description("Catalogue")] Catalogue,
      [Description("Geometric")] Geometric,
    }
    public enum GeoTypes {
      [Description("Perimeter")] Perim,
    }

    public enum StdShapeOptions {
      [Description("Rectangle")] Rectangle,
      [Description("Circle")] Circle,
      [Description("I section")] Isection,
      [Description("Tee")] Tee,
      [Description("Channel")] Channel,
      [Description("Angle")] Angle,
    }

    public enum SectUnitOptions {
      [Description("mm")] UMm,
      [Description("cm")] UCm,
      [Description("m")] Um,
      [Description("ft")] UFt,
      [Description("in")] UIn,
    }

    public ProfileTypes ProfileType;
    public StdShapeOptions StdShape;
    public GeoTypes GeoType;
    public SectUnitOptions SectUnit = SectUnitOptions.UMm;

    public string CatalogueProfileName = "";

    public bool IsTapered;
    public bool IsHollow;
    public bool IsElliptical;
    public bool IsGeneral;
    public bool IsB2B;

    public double D;
    public double B1;
    public double B2;
    public double Tf1;
    public double Tf2;
    public double Tw1;
    public double Tw2;

    public List<Point2d> PerimeterPoints;
    public List<List<Point2d>> VoidPoints;
  }
  /// <summary>
  /// Helper class for Profile/Isection conversions
  /// </summary>
  public class ConvertSection {
    /// <summary>
    /// Method to convert a GsaProfile to a string that can be read by GSA
    /// (in GsaAPI.Isection.Profile or GsaGH.Parameters.GsaSection.Isection.Profile)
    /// 
    /// NOTE: 
    /// - Does not cover all profile types available in GSA (but all available in GsaProfile)
    /// - Geometric can handle custom profiles with voids. Origin/anchor to be implemented.
    /// - Catalogue profiles yet to be implemented
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
                return "STD TR" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.B2.ToString("0.############");
              case ProfileHelper.StdShapeOptions.Rectangle when gsaProfile.IsHollow:
                return "STD RHS" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.Tw1.ToString("0.############") + " " + gsaProfile.Tf1.ToString("0.############");
              case ProfileHelper.StdShapeOptions.Rectangle:
                return "STD R" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############");
              case ProfileHelper.StdShapeOptions.Circle when gsaProfile.IsHollow: {
                  if (gsaProfile.IsElliptical) {
                    return "STD OVAL" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.Tw1.ToString("0.############");
                  }
                  else {
                    return "STD CHS" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.Tw1.ToString("0.############");
                  }
                }
              case ProfileHelper.StdShapeOptions.Circle when gsaProfile.IsElliptical:
                return "STD E" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " 2";
              case ProfileHelper.StdShapeOptions.Circle:
                return "STD C" + unit + gsaProfile.D.ToString("0.############");
              case ProfileHelper.StdShapeOptions.Isection when gsaProfile.IsGeneral: {
                  if (gsaProfile.IsTapered) {
                    return "STD TI" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.B2.ToString("0.############") + " "
                           + gsaProfile.Tw1.ToString("0.############") + " " + gsaProfile.Tw2.ToString("0.############") + " " + gsaProfile.Tf1.ToString("0.############") + " " + gsaProfile.Tf2.ToString("0.############");
                  }
                  else {
                    return "STD GI" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.B2.ToString("0.############") + " "
                           + gsaProfile.Tw1.ToString("0.############") + " " + gsaProfile.Tf1.ToString("0.############") + " " + gsaProfile.Tf2.ToString("0.############");
                  }
                }
              case ProfileHelper.StdShapeOptions.Isection:
                return "STD I" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.Tw1.ToString("0.############") + " " + gsaProfile.Tf1.ToString("0.############");
              case ProfileHelper.StdShapeOptions.Tee when gsaProfile.IsTapered:
                return "STD TT" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.Tw1.ToString("0.############") + " "
                       + gsaProfile.Tw2.ToString("0.############") + " " + gsaProfile.Tf1.ToString("0.############");
              case ProfileHelper.StdShapeOptions.Tee:
                return "STD T" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.Tw1.ToString("0.############") + " " + gsaProfile.Tf1.ToString("0.############");
              case ProfileHelper.StdShapeOptions.Channel when gsaProfile.IsB2B:
                return "STD DCH" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.Tw1.ToString("0.############") + " " + gsaProfile.Tf1.ToString("0.############");
              case ProfileHelper.StdShapeOptions.Channel:
                return "STD CH" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.Tw1.ToString("0.############") + " " + gsaProfile.Tf1.ToString("0.############");
              case ProfileHelper.StdShapeOptions.Angle when gsaProfile.IsB2B:
                return "STD D" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.Tw1.ToString("0.############") + " " + gsaProfile.Tf1.ToString("0.############");
              case ProfileHelper.StdShapeOptions.Angle:
                return "STD A" + unit + gsaProfile.D.ToString("0.############") + " " + gsaProfile.B1.ToString("0.############") + " " + gsaProfile.Tw1.ToString("0.############") + " " + gsaProfile.Tf1.ToString("0.############");
              default:
                return "STD something else";
            }
          }
        case ProfileHelper.ProfileTypes.Catalogue: {
            string outputSectionString = "";

            if (gsaProfile.CatalogueProfileName != null) {
              outputSectionString = gsaProfile.CatalogueProfileName;
            }

            return $"CAT {outputSectionString}";
          }
        case ProfileHelper.ProfileTypes.Geometric when gsaProfile.GeoType == ProfileHelper.GeoTypes.Perim: {
            string unit = "";

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

            string profile = "GEO P" + unit;
            int iPoint = 0;
            foreach (Point2d point in gsaProfile.PerimeterPoints) {
              if ((iPoint > 0))
                profile += " L";
              else
                profile += " M";

              profile += ("("
                          + (point.X + ("|"
                                        + (point.Y + ")"))));
              iPoint++;
            }

            if (gsaProfile.VoidPoints == null || !(gsaProfile.VoidPoints.Count > 0)) {
              return profile;
            }

            {
              foreach (List<Point2d> voidPoint in gsaProfile.VoidPoints) {
                iPoint = 0;
                foreach (Point2d point in voidPoint) {
                  if (iPoint > 0)
                    profile += " L";
                  else
                    profile += " M";

                  profile += ("("
                              + (point.X + ("|"
                                            + (point.Y + ")"))));
                  iPoint++;
                }
              }
            }

            return profile;
          }
        case ProfileHelper.ProfileTypes.Geometric:
          return "GEO";
        default:
          return null;
      }
    }
  }
}
