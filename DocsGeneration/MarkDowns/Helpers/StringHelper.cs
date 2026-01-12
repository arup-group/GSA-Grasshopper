using System;
using System.Collections.Generic;
using System.Linq;

namespace DocsGeneration.MarkDowns.Helpers {
  public enum AdmonitionType {
    Note,
    Tip,
    Info,
    Caution,
    Danger
  }

  public class StringHelper {
    public const string PrefixBetweenTypes = "GSA ";

    public static string AddBetaWarning() {
      string txt
        = "GSA-Grasshopper plugin is pre-release and under active development, including further testing to be undertaken. It is provided \"as-is\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using the plugin should not be relied upon without thorough and independent checking.";
      return Admonition(txt, AdmonitionType.Caution);
    }

    public static string Admonition(string txt, AdmonitionType type, string headline = "") {
      return $":::{type.ToString().ToLower()} {headline}\n\n{txt}\n\n:::\n\n";
    }

    public static string MakeBold(string text) {
      return $"**{text}**";
    }

    public static string MakeItalic(string text) {
      return $"_{text}_";
    }

    public static string SummaryDescription(string str, Configuration config) {
      string markdown = ConvertSummaryToMarkup(str, config);
      return $"## Description\n\n{markdown}\n\n";
    }

    public static string ComponentDescription(string description, List<string> parameterNames) {
      if (description.Contains(PrefixBetweenTypes)) {
        StringSplitOptions opt = StringSplitOptions.None;
        string[] split = description.Split(new[] { PrefixBetweenTypes }, opt);
        if (split.Length > 1) {
          description = ApplyCorrectArticle(split[0], split[1]);
        }

        for (int i = 1; i < split.Length; i++) {
          string[] parameterNameAndRest = split[i].Split(' ');
          string parameterName = parameterNameAndRest.First();
          if (parameterNames.Contains(parameterName.ToUpper())) {
            description += CreateParameterLink(parameterName);
            for (int j = 1; j < parameterNameAndRest.Length; j++) {
              description += $"{parameterNameAndRest[j]} ";
            }
          } else {
            description += split[i];
          }
        }
      }

      return description;
    }

    private static string CreateParameterLink(string parameterName)
    {
      string linkName = CapitalizeFirstLetter(parameterName);
      linkName = FormatDimensions(linkName);
      return ParameterLink(linkName, parameterName);
    }

    private static string ParameterLink(string linkName, string parameterName)
    {
      return $"{MarkdownLink(linkName, $"gsagh-{parameterName.ToLower()}-parameter.md")} ";
    }

    private static string MarkdownLink(string linkName, string parameterName)
    {
      return $"[{linkName}]({parameterName})";
    }

    private static string FormatDimensions(string name)
    {
      return name.Replace(" 3d", " 3D").Replace(" 2d", " 2D").Replace(" 1d", " 1D");
    }

    private static string CapitalizeFirstLetter(string parameterName)
    {
      return parameterName[0].ToString().ToUpper() + parameterName.Substring(1);
    }

    public static string ApplyCorrectArticle(string left, string right) {
      right = right.ToLower();
      if (StartsWithVowel(right) && EndsWithArticleAndSpace(left)) {
        left = RemoveArticle(left);
        left += "an ";
      }

      return left;
    }

    public static string Replace(string oldValue) {
      return oldValue.Replace(Environment.NewLine, "<br />").Replace("|", "&#124;").Replace("{ `", @"\{ `")
       .Replace("` }", @"` \}").Replace("|*", @"| \*").Replace("_*", @"_\*");
    }

    private static bool EndsWithArticleAndSpace(string word)
    {
      return word.EndsWith("a ");
    }

    private static string RemoveArticle(string left)
    {
      return left.TrimEnd().TrimEnd('a');
    }

    private static bool StartsWithVowel(string word) {
      return word.StartsWith("a") || word.StartsWith("e") || word.StartsWith("i") || word.StartsWith("o")
        || word.StartsWith("u");
    }

    private static string ConvertSummaryToMarkup(string str, Configuration config) {
      str = str.Replace("</para>", "\n\n").Replace("<para>", string.Empty);
      str = str.Replace("IGsaLoad", "GsaLoad");
      str = str.Replace("IGsaResult", "GsaResult");
      str = str.Replace("IGsaGridLoad", "GsaGridLoad");
      str = str.Replace("IGsaStandardMaterial", "GsaStandardMaterial");
      str = str.Replace("IGsaAnnotation", "GsaAnnotation");
      str = str.Replace("IGsaDiagram", "GsaDiagram");

      // For example:
      // <see href="https://docs.oasys-software.com/structural/gsa/references/listsandembeddedlists.html">syntax</see>
      if (str.Contains("<see href=\"")) {
        StringSplitOptions opt = StringSplitOptions.None;
        string[] split = str.Split(new string[] { "<see href=\"" }, opt);
        str = split[0];
        for (int i = 1; i < split.Length; i++) {
          string[] htmlLinkAndRest = split[i].Split(new string[] { "\">" }, opt);
          string htmlAddress = htmlLinkAndRest[0].Replace(".html", ".md");
          htmlAddress = htmlAddress.Replace("https://docs.oasys-software.com/structural/gsa/", "/");
          string[] htmlNameAndRest = htmlLinkAndRest[1].Split(new string[] { "</see>" }, opt);
          string htmlName = htmlNameAndRest[0];
          str += $"[{htmlName}]({htmlAddress})";
          if (htmlNameAndRest.Length > 1) {
            str += htmlNameAndRest[1];
          }
        }
      }

      // For example:
      // <see cref="GsaSection"/>
      // <see cref="GsaAPI.Bool6" />
      // <see cref="Components.CreateModel"/>
      // xml summary will come through with prefix T for type or M for method
      if (str.Contains("<see cref=\"")) {
        StringSplitOptions opt = StringSplitOptions.None;
        string[] split = str.Split(new string[] { "<see cref=\"" }, opt);
        str = split[0];
        for (int i = 1; i < split.Length; i++) {
          string[] refAndRest = split[i].Split(new string[] { "/>" }, opt);
          string reference = refAndRest[0];
          switch (reference[0]) {
            case 'T':
              reference = reference.Replace("T:", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty);
              ;
              string[] typeSplit = reference.Split('.');
              string markdownLink = SortReference(typeSplit[0], typeSplit[1], typeSplit.Last(), config);
              markdownLink = markdownLink.Replace(" 3d", " 3D").Replace(" 2d", " 2D").Replace(" 1d", " 1D");
              str += markdownLink;
              if (refAndRest.Length > 1) {
                str += refAndRest[1];
              }

              break;

            case 'M':
              reference = reference.Replace("M:", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty);
              ;
              string[] typeSplit2 = reference.Split('.');
              str += typeSplit2.Last();
              if (refAndRest.Length > 1) {
                str += refAndRest[1];
              }

              break;

            default:
              str += refAndRest.Last();
              break;
          }
        }
      }

      // For example:
      // <list type="bullet">
      // <item><description>Item 1.</description></item>
      // <item><description>Item 2.</description></item>
      // </list>
      if (str.Contains("<list type=\"bullet\">")) {
        StringSplitOptions opt = StringSplitOptions.RemoveEmptyEntries;
        string[] split = str.Split(new string[] { "<list type=\"bullet\">" }, opt);
        str = split[0];
        for (int i = 1; i < split.Length; i++) {
          string[] listsAndRest = split[i].Split(new string[] { "<item><description>" }, opt);
          for (int j = 0; j < listsAndRest.Length - 1; j++) {
            str += "\n- " + listsAndRest[j].Replace("</description></item>", string.Empty);
          }

          string[] lastItemAndRest = listsAndRest.Last().Split(new string[] { "</list>" }, opt);
          str += "\n- " + lastItemAndRest[0].Replace("</description></item>", "\n\n");
          if (lastItemAndRest.Length > 1) {
            str += lastItemAndRest[1].TrimStart();
          }
        }
      }

      return str;
    }

    private static string SortReference(string nameSpace, string type, string name, Configuration config) {
      switch (nameSpace) {
        case "GsaAPI":
          string apiLink = "https://docs.oasys-software.com/structural/gsa/references/dotnet-api/data-classes.html#"
            + name.ToLower();
          if (name == "Bool6" || name == "Annotation" || name == "AutomaticOffset" || name == "SectionModifierAttribute"
            || name == "Prop2DModifierAttribute" || name == "EndRelease") {
            apiLink = "https://docs.oasys-software.com/structural/gsa/references/dotnet-api/types.html#"
              + name.ToLower();
          }

          return $"[GsaAPI {name}]({apiLink})";

        case "GsaGH":
          name = name.Replace("Gsa", string.Empty);
          string link = FileHelper.CreateFileName(FileHelper.SplitCamelCase(name, "-"), type.TrimEnd('s'), config.ProjectName);
          name = FileHelper.SplitCamelCase(name, " ");
          return $"[{name}]({link}.md)";
      }

      return string.Empty;
    }
  }
}
