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
      if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)) && Value != null) {
        target = (Q)(object)new GH_Integer(Value.Id);
        return true;
      }

      target = default;
      return false;
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
