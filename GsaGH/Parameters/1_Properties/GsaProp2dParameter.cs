using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaProp2dGoo" /> type.
  /// </summary>
  public class GsaProp2dParameter : GH_OasysPersistentParam<GsaProp2dGoo> {
    public override Guid ComponentGuid => new Guid("05a034ad-683d-479b-9768-5c04379c0606");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaProp2dGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaProp2dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Prop2dParam;

    public GsaProp2dParameter() : base(new GH_InstanceDescription(GsaProp2dGoo.Name,
      GsaProp2dGoo.NickName, GsaProp2dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaProp2dGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaProp2d)) {
        return new GsaProp2dGoo((GsaProp2d)data);
      }

      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        return new GsaProp2dGoo(new GsaProp2d(id));
      }

      if (GH_Convert.ToDouble(data, out double val1, GH_Conversion.Both)) {
        if (Quantity.TryFrom(val1, DefaultUnits.LengthUnitSection, out IQuantity length1)) {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Number converted to thickness in " +
          DefaultUnits.LengthUnitSection.ToString());
          return new GsaProp2dGoo(new GsaProp2d((Length)length1));
        }
      }

      if (GH_Convert.ToString(data, out string val2, GH_Conversion.Both)) {
        Quantity.TryFrom(0.0, DefaultUnits.LengthUnitSection, out IQuantity length2);
        Type type = length2.QuantityInfo.ValueType;
        if (val2.EndsWith("m") && type == typeof(Duration)) {
          type = typeof(Length);
        }

        if (Quantity.TryParse(type, val2, out IQuantity length3)) {
          return new GsaProp2dGoo(new GsaProp2d((Length)length3));
        }
      }

      AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
      $"Data conversion failed from {data.GetTypeName()} to Prop2d");
      return new GsaProp2dGoo(null);
    }
  }
}
