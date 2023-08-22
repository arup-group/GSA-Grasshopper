﻿using GsaGhDocs.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using static Rhino.Render.RenderEnvironment;
using static System.Collections.Specialized.BitVector32;

namespace GsaGhDocs.Helpers {
  public class StringHelper {
    public static string Description(string str) {
      string markdown = ConvertSummaryToMarkup(str);
      return $"## Description\n\n{markdown}\n\n";
    }

    private static string ConvertSummaryToMarkup(string str) {

      // <see href="https://docs.oasys-software.com/structural/gsa/references/listsandembeddedlists.html">syntax</see>
      if (str.Contains("<see href=\"")) {
        StringSplitOptions opt = StringSplitOptions.None;
        string[] split = str.Split(new string[] { "<see href=\"" }, opt);
        str = split[0];
        for (int i = 1; i < split.Length; i++) {
          string[] htmlLinkAndRest = split[i].Split(new string[] { "\">" }, opt);
          string htmlAddress = htmlLinkAndRest[0];
          string[] htmlNameAndRest = htmlLinkAndRest[1].Split(new string[] { "</see>" }, opt);
          string htmlName = htmlNameAndRest[0];
          string markdownLink = $"[{htmlName}]({htmlAddress})";
          str += markdownLink + htmlNameAndRest[1];
        }
      }

      // <see cref="GsaAPI.Bool6" />
      if (str.Contains("<see cref=\"")) {
        StringSplitOptions opt = StringSplitOptions.None;
        string[] split = str.Split(new string[] { "<see cref=\"" }, opt);
        str = split[0];
        for (int i = 1; i < split.Length; i++) {
          string[] refAndRest = split[i].Split(new string[] { "/>" }, opt);
          string reference = refAndRest[0];
          switch (reference[0]) {
            case 'T':
              reference = reference.Replace("T:", string.Empty)
                .Replace("\"", string.Empty).Replace(" ", string.Empty); ;
              string[] typeSplit = reference.Split('.');
              string markdownLink = SortReference(typeSplit[0], typeSplit[1], typeSplit.Last());
              str += markdownLink + refAndRest[1];
              break;

            case 'M':
              reference = reference.Replace("M:", string.Empty)
                .Replace("\"", string.Empty).Replace(" ", string.Empty); ;
              string[] typeSplit2 = reference.Split('.');
              str += typeSplit2.Last() + refAndRest[1];
              break;
          }
        }
      }

      return str;
    }

    private static string SortReference(string @namespace, string type, string name) {
      switch (@namespace) {
        case "GsaAPI":
          string link = "https://docs.oasys-software.com/structural/gsa/references/dotnet-api/data-classes.html#"
          + name.ToLower();
          if (name == "Bool6" 
            || name == "Annotation"
            || name == "AutomaticOffset"
            || name == "SectionModifierAttribute"
            || name == "Prop2DModifierAttribute"
            || name == "EndRelease") {
            link = "https://docs.oasys-software.com/structural/gsa/references/dotnet-api/types.html#"
          + name.ToLower();
          }
          return $"[GsaAPI {name}]({link})";

        case "GsaGH":
          if (type == "Parameters") {
            return $"[{name}](gsagh-{name.ToLower()}-parameter.html)";
          }
          if (type == "Components") {
            return $"[{name}](gsagh-{name.ToLower()}-component.html)";
          }
          break;
      }

      return string.Empty;
    }
    
    public static string Icon(string iconName, string postfix = "") {
      if (iconName.StartsWith("UnitNumber")) {
        return $"![UnitNumber](./images/gsagh/UnitParam.png)";
      }

      iconName = iconName.Replace(" (List)", string.Empty);
      iconName = iconName.Replace(" (Tree)", string.Empty);
      
      // ![Material](./images/gsagh/MaterialParam.png)
      string name = iconName.Replace(" ", string.Empty);
      return $"![{name}](./images/gsagh/{name}{postfix}.png)";
    }

    public static string Link(string linkName, string postfix) {
      // [Material](gsagh-material-parameter.html)
      string name = linkName.Replace(" ", "-");
      return $"[{linkName}](gsagh-{name.ToLower()}-{postfix.ToLower()}.html)";
    }

    public static string ParameterLink(string parameterType, List<string> parameterNames) {
      string parameterName = parameterType;
      parameterName = parameterName.Replace(" (List)", string.Empty);
      parameterName = parameterName.Replace(" (Tree)", string.Empty);
      string list = parameterType.Contains(" (List)") ? " (List)" : string.Empty;
      string tree = parameterType.Contains(" (Tree)") ? " (Tree)" : string.Empty;

      if (parameterNames.Contains(parameterName.ToUpper())) {
        string name = parameterName.Replace(" ", "-");
        return $"[{parameterName}](gsagh-{name.ToLower()}-parameter.html)" + list + tree;
      }

      return parameterType;
    }

    public static string Bold(string text) {
      return $"**{text}**";
    }

    public static string Italic(string text) {
      return $"_{text}_";
    }

    public static string FileName(string text, string postfix) {
      string name = text.Replace(" ", "-");
      return $@"Output\gsagh-{name.ToLower()}-{postfix.ToLower()}.md";
    }

    public static string Warning(string text) {
      // ::: warning
      // GSA - Grasshopper plugin[GsaGH] is pre - release and under active development, including further testing to be undertaken. It is provided \"as-is\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using GsaGH should not be relied upon without thorough and independent checking.
      // :::
      return $"::: warning\n{text}\n:::\n\n";
    }

    public static string BetaWarning() {
      string txt = "GSA-Grasshopper plugin [GsaGH] is pre-release and under active development, including further testing to be undertaken. It is provided \"as-is\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using GsaGH should not be relied upon without thorough and independent checking.";
      return Warning(txt);
    }

    public static string Tip(string headline, string text) {
      // ::: tip Did you know?
      // The `Bool6` icon takes inspiration from the central pin / hinge / charnier connection[Ove Arup's Kingsgate footbridge](https://www.arup.com/projects/kingsgate-footbridge).
      // ![Kingsgate Footbridge Durham](./ images / gsagh / Kingsgate - Footbridge - Durham.jpg)
      // * (c)Giles Rocholl / Arup *
      // :::
      return $"::: tip {headline}\n{text}\n:::\n\n";
    }
  }
}
