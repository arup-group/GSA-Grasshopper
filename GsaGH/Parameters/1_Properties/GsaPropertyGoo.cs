using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaSection" /> and <see cref="GsaSpringProperty" />can be used in Grasshopper.
  /// </summary>
  public class GsaPropertyGoo : GH_OasysGoo<IGsaProperty> {
    public static string Description => "GSA Section Property (Beam) or Spring Property";
    public static string Name => "Property";
    public static string NickName => "PB";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaPropertyGoo(IGsaProperty item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaPropertyGoo(Value);
    }

    public override bool CastTo<Q>(ref Q target) {
      switch (target) {
        case GH_Integer integer:
          if (Value is GsaSection section) {
            integer.Value = section.Id;
            return true;
          }
          break;
      }
      return base.CastTo(ref target);
    }

    public override string ToString() {
      if (Value == null) {
        return "Null";
      }

      string typeName = Value.GetType().Name.TrimStart('I').Replace("Gsa", string.Empty);
      return PluginInfo.ProductName + " " + typeName + " (" + Value.ToString() + ")";
    }
  }
}
