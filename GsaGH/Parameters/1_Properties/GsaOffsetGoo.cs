using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaOffset" /> can be used in Grasshopper.
  /// </summary>
  public class GsaOffsetGoo : GH_OasysGoo<GsaOffset> {
    public static string Description => "GSA Offset";
    public static string Name => "Offset";
    public static string NickName => "Off";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaOffsetGoo(GsaOffset item) : base(item) { }

    public override bool CastFrom(object source) {
      if (source == null) {
        return false;
      }

      if (base.CastFrom(source)) {
        return true;
      }

      if (!GH_Convert.ToDouble(source, out double myval, GH_Conversion.Both)) {
        return false;
      }

      Value.Z = new Length(myval, DefaultUnits.LengthUnitGeometry);
      // if input to parameter is a single number convert it to the most common Z-offset
      return true;
    }

    public override IGH_Goo Duplicate() {
      return new GsaOffsetGoo(Value);
    }
  }
}
