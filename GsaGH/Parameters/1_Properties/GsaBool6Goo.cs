using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaBool6" /> can be used in Grasshopper.
  /// </summary>
  public class GsaBool6Goo : GH_OasysGoo<GsaBool6> {
    public static string Description => 
      "A Bool6 contains six booleans to set releases in \n" +
      "Element1Ds and Member1Ds, or restraints in Nodes.\n" +
      "You can create a new Bool6 from a 6-character string \n" +
      "describing the restraint condition (F = Fixed, R = \n" +
      "Released) for each degree of freedom)";
    public static string Name => "Bool6";
    public static string NickName => "B6";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaBool6Goo(GsaBool6 item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaBool6Goo(Value);
    }
  }
}
