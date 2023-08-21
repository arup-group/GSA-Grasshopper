using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaList" /> can be used in Grasshopper.
  /// </summary>
  public class GsaListGoo : GH_OasysGoo<GsaList> {
    public static string Description => 
      "A list is expressed as a string of text in a specific syntax. \n" +
      "Lists (of nodes, elements, members and cases) are used, \n" +
      "for example, when a particular load is to be applied to \n" +
      "one or several elements. To define a series of items the \n" +
      "list can either specify each individually or, if applicable, \n" +
      "use a more concise syntax.";
    public static string Name => "List";
    public static string NickName => "L";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public GsaListGoo() { }
    public GsaListGoo(GsaList item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaListGoo(Value);
    }

    public override bool CastFrom(object source) {
      if (source == null) {
        return false;
      }
      
      if (GH_Convert.ToString(source, out string text, GH_Conversion.Both)) {
        Value = new GsaList() {
          EntityType = EntityType.Undefined,
          Definition = text
        };
        return true;
      }

      return false;
    }
  }
}
