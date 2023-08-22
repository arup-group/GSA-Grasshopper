using Eto.Drawing;
using System.Collections.Generic;

namespace GsaGhDocs.Helpers {
  public class StringHelper {
    public static string Icon(string iconName, string postfix = "") {
      if (iconName.StartsWith("UnitNumber")) {
        return $"![UnitNumber](./images/gsagh/UnitParam.png)";
      }
      
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
      if (parameterNames.Contains(parameterType)) {
        return Link(parameterType, "parameter");
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
