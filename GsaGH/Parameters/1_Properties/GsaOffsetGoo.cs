using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaOffset" /> can be used in Grasshopper.
  /// </summary>
  public class GsaOffsetGoo : GH_OasysGoo<GsaOffset> {
    public static string Description => 
      "An Offset will move the Section or 2D Property away \n" +
      "from the centre of the local axis in Element1D, Member1D, \n" +
      "Element2D, or Member2D.";
    public static string Name => "Offset";
    public static string NickName => "Off";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaOffsetGoo(GsaOffset item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaOffsetGoo(Value);
    }
  }
}
