using System;
using System.Collections.Generic;

using GsaAPI;

using GsaGH.Helpers;

namespace GsaGH.Parameters {
  public class MemberEndRestraintFactory {
    internal static MemberEndRestraint CreateFromStrings(string f1, string f2, string t,
      string maj, string min) {
      RotationalRestraint topRot = RotationalRestraint(f1);
      RotationalRestraint botRot = RotationalRestraint(f2);
      RotationalRestraint majRot = RotationalRestraint(maj);
      RotationalRestraint minRot = RotationalRestraint(min);
      TorsionalRestraint tor = TorsionalRestraint(t);
      TranslationalRestraint topTrans = TranslationalRestraint(f1);
      TranslationalRestraint botTrans = TranslationalRestraint(f2);
      TranslationalRestraint majTrans = TranslationalRestraint(maj);
      TranslationalRestraint minTrans = TranslationalRestraint(min);
      return new MemberEndRestraint(topRot, botRot, majRot, minRot, tor,
        topTrans, botTrans, majTrans, minTrans);
    }

    internal static MemberEndRestraint CreateFromStrings(string s) {
      if (Enum.TryParse(s, true, out StandardRestraint std)) {
        return new MemberEndRestraint(std);
      }

      string f1 = string.Empty;
      string f2 = string.Empty;
      string maj = string.Empty;
      string min = string.Empty;
      string t = string.Empty;
      string[] split = s.ToLower().Split(' ', ',');
      foreach (string sp in split) {
        if (sp.Contains("f1")) {
          f1 = sp;
        }

        if (sp.Contains("f2")) {
          f2 = sp;
        }

        if (sp.Contains("maj")) {
          maj = sp;
        }

        if (sp.Contains("min")) {
          min = sp;
        }

        if (sp.Contains("t")) {
          t = sp;
        }
      }

      return new MemberEndRestraint(
        RotationalRestraint(f1), RotationalRestraint(f2),
        RotationalRestraint(maj), RotationalRestraint(min),
        TorsionalRestraint(t),
        TranslationalRestraint(f1), TranslationalRestraint(f2),
        TranslationalRestraint(maj), TranslationalRestraint(min)
        );
    }

    internal static string MemberEndRestraintToString(MemberEndRestraint r) {
      string f1 = "F1";
      string f2 = "F2";
      string t = "T";
      string maj = "MAJ";
      string min = "MIN";

      if (r.TopFlangeLateralRestraint != GsaAPI.TranslationalRestraint.None) {
        f1 += "L";
      }

      if (r.BottomFlangeLateralRestraint != GsaAPI.TranslationalRestraint.None) {
        f2 += "L";
      }

      if (r.MajorAxisTranslationalRestraint != GsaAPI.TranslationalRestraint.None) {
        maj += "V";
      }

      if (r.MinorAxisTranslationalRestraint != GsaAPI.TranslationalRestraint.None) {
        min += "V";
      }

      if (r.TopFlangeWarpingRestraint != GsaAPI.RotationalRestraint.None) {
        f1 += r.TopFlangeWarpingRestraint == GsaAPI.RotationalRestraint.Partial ? "P" : "W";
      }

      if (r.BottomFlangeWarpingRestraint != GsaAPI.RotationalRestraint.None) {
        f2 += r.BottomFlangeWarpingRestraint == GsaAPI.RotationalRestraint.Partial ? "P" : "W";
      }

      switch (r.TorsionalRestraint) {
        case GsaAPI.TorsionalRestraint.Friction:
          t += "F";
          break;

        case GsaAPI.TorsionalRestraint.Partial:
          t += "P";
          break;

        case GsaAPI.TorsionalRestraint.Full:
          t += "R";
          break;

        case GsaAPI.TorsionalRestraint.None:
          t = string.Empty;
          break;
      }

      if (r.MajorAxisRotationalRestraint != GsaAPI.RotationalRestraint.None) {
        maj += r.MajorAxisRotationalRestraint == GsaAPI.RotationalRestraint.Partial ? "P" : "W";
      }

      if (r.MinorAxisRotationalRestraint != GsaAPI.RotationalRestraint.None) {
        min += r.MinorAxisRotationalRestraint == GsaAPI.RotationalRestraint.Partial ? "P" : "W";
      }

      var output = new List<string>();
      if (f1 != "F1") {
        output.Add(f1);
      }

      if (f2 != "F2") {
        output.Add(f2);
      }

      if (t != "T") {
        output.Add(t);
      }

      if (maj != "MAJ") {
        output.Add(maj);
      }

      if (min != "MIN") {
        output.Add(min);
      }

      string s = string.Join(" ", output).TrimSpaces();
      switch (s) {
        case "F1L F2L TR MAJV MINV":
          return "Pinned";

        case "F1LW F2LW TR MAJVW MINVW":
          return "Fixed";

        case "F1LW F2W TR MAJVW MINV":
          return "FullRotational";

        case "F1LP F2P TR MAJVP MINV":
          return "PartialRotational";

        case "F1L":
          return "TopFlangeLateral";
      }

      if (string.IsNullOrEmpty(s)) {
        return "Free";
      }

      return s;
    }

    internal static RotationalRestraint RotationalRestraint(string s) {
      if (string.IsNullOrEmpty(s)) {
        return GsaAPI.RotationalRestraint.None;
      }

      s = s.ToLower().Replace("f1", string.Empty).Replace("f2", string.Empty)
        .Replace("maj", string.Empty).Replace("min", string.Empty);
      s = s.TrimStart('l').TrimStart('v');
      if (s.StartsWith("n") || s.StartsWith("0") || s.Contains("false") || s.Contains("none")) {
        return GsaAPI.RotationalRestraint.None;
      }

      if (s.StartsWith("p") || s.StartsWith("1") || s.Contains("partial")) {
        return GsaAPI.RotationalRestraint.Partial;
      }

      if (s.StartsWith("w") || s.StartsWith("f") || s.StartsWith("r") ||
        s.StartsWith("2") || s.Contains("true") || s.Contains("full")) {
        return GsaAPI.RotationalRestraint.Full;
      }

      return GsaAPI.RotationalRestraint.None;
    }

    internal static TranslationalRestraint TranslationalRestraint(string s) {
      if (string.IsNullOrEmpty(s)) {
        return GsaAPI.TranslationalRestraint.None;
      }

      s = s.ToLower().Replace("f1", string.Empty).Replace("f2", string.Empty)
        .Replace("maj", string.Empty).Replace("min", string.Empty);
      s = s.TrimStart('w').TrimStart('p');
      if (s.StartsWith("n") || s.StartsWith("0") || s.Contains("false") || s.Contains("none")) {
        return GsaAPI.TranslationalRestraint.None;
      }

      if (s.StartsWith("l") || s.StartsWith("v") || s.StartsWith("r")
        || s.StartsWith("1") || s.Contains("true") || s.Contains("full")) {
        return GsaAPI.TranslationalRestraint.Full;
      }

      return GsaAPI.TranslationalRestraint.None;
    }

    internal static TorsionalRestraint TorsionalRestraint(string s) {
      if (string.IsNullOrEmpty(s)) {
        return GsaAPI.TorsionalRestraint.None;
      }

      s = s.ToLower().Replace("t", string.Empty);
      if (s.StartsWith("n") || s.StartsWith("0") || s.Contains("false") || s.Contains("none")) {
        return GsaAPI.TorsionalRestraint.None;
      }

      if (s.StartsWith("p") || s.StartsWith("2") || s.Contains("partial")) {
        return GsaAPI.TorsionalRestraint.Partial;
      }

      if (s.StartsWith("fu") || s.StartsWith("r") || s.StartsWith("3")
        || s.Contains("true") || s.Contains("full")) {
        return GsaAPI.TorsionalRestraint.Full;
      }

      if (s.StartsWith("f") || s.StartsWith("1") || s.Contains("frict")) {
        return GsaAPI.TorsionalRestraint.Friction;
      }

      return GsaAPI.TorsionalRestraint.None;
    }

    internal static string RestraintSyntax() {
      return "Restraint Description Syntax:" +
        "\n\nParts of the description are separated by spaces or commas." +
        "\nThe parts that can be restrained are:" +
        "\nF1, F2: flanges" +
        "\nwith suffices:" +
        "\n    L: lateral" +
        "\n    W: full warping" +
        "\n    P: part warping" +
        "\nT: torsion" +
        "\nwith suffices" +
        "\n    R: full restraint" +
        "\n    P: part restraint" +
        "\n    F: friction only" +
        "\nMAJ, MIN major and minor axis bending and shear" +
        "\nwith suffices" +
        "\n    R: full rotational restraint" +
        "\n    P: part rotational restraint" +
        "\n    V: Translational restraint" +
        "\nExamples:" +
        "\n    F1LP, TP, MAJRV:" +
        "\tLateral and partial warping restraint to flange 1" +
        "\t    F12W:\t\tFlanges 1&2 full warping restraint...\n";
    }
  }
}
