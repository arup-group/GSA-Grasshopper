using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;

namespace GsaGH.Parameters {

  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaProp2d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaProp2dGoo : GH_OasysGoo<GsaProp2d> {

    #region Properties + Fields
    public static string Description => "GSA Area Property";
    public static string Name => "Prop2D";
    public static string NickName => "PA";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaProp2dGoo(GsaProp2d item) : base(item) {
    }

    #endregion Public Constructors

    #region Public Methods
    public override bool CastFrom(object source) {
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;

      if (GH_Convert.ToDouble(source, out double thk, GH_Conversion.Both)) {
        Value = new GsaProp2d(new Length(thk, DefaultUnits.LengthUnitSection));
      }
      return false;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      if (base.CastTo(ref target))
        return true;

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value == null)
          target = default;
        else {
          var ghint = new GH_Integer();
          if (GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint))
            target = (TQ)(object)ghint;
          else
            target = default;
        }
        return true;
      }

      target = default;
      return false;
    }

    public override IGH_Goo Duplicate() => new GsaProp2dGoo(Value);

    #endregion Public Methods
  }
}
