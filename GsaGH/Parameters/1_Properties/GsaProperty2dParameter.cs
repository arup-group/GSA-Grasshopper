using System;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;
using OasysGH.Units;

using OasysUnits;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaProperty2dGoo" /> type.
  /// </summary>
  public class GsaProperty2dParameter : GH_OasysPersistentParam<GsaProperty2dGoo> {
    public override Guid ComponentGuid => new Guid("05a034ad-683d-479b-9768-5c04379c0606");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaProperty2dGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaProperty2dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Property2dParam;

    public GsaProperty2dParameter() : base(new GH_InstanceDescription(GsaProperty2dGoo.Name,
      GsaProperty2dGoo.NickName, GsaProperty2dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaProperty2dGoo PreferredCast(object data) {
      string mes = string.Empty;
      string defaultText = $"{data.GetTypeName()} does not contain a 2D Property";
      switch (data) {
        case GsaElement2dGoo elem2d:
          if (elem2d.Value.Prop2ds.IsNullOrEmpty()) {
            mes = defaultText;
            break;
          }

          return new GsaProperty2dGoo(elem2d.Value.Prop2ds[0]);

        case GsaMember2dGoo mem2d:
          if (mem2d.Value.Prop2d == null) {
            mes = defaultText;
            break;
          }

          return new GsaProperty2dGoo(mem2d.Value.Prop2d);
      }

      if (data.GetType() != typeof(GH_Number) && // assume GH_Number is double and we do that below
        GH_Convert.ToInt32(data, out int id, GH_Conversion.Both)) {
        GH_Convert.ToString(data, out string val, GH_Conversion.Both);
        if (!val.Contains(".")) {
          return new GsaProperty2dGoo(new GsaProperty2d(id));
        }
      }

      if (GH_Convert.ToDouble(data, out double val1, GH_Conversion.Both)) {
        if (Quantity.TryFrom(val1, DefaultUnits.LengthUnitSection, out IQuantity length1)) {
          this.AddRuntimeWarning($"Number converted to thickness in {DefaultUnits.LengthUnitSection}");
          return new GsaProperty2dGoo(new GsaProperty2d((Length)length1));
        }
      }

      if (GH_Convert.ToString(data, out string val2, GH_Conversion.Both)) {
        Quantity.TryFrom(0.0, DefaultUnits.LengthUnitSection, out IQuantity length2);
        Type type = length2.QuantityInfo.ValueType;
        if (val2.EndsWith("m") && type == typeof(Duration)) {
          type = typeof(Length);
        }

        if (Quantity.TryParse(type, val2, out IQuantity length3)) {
          return new GsaProperty2dGoo(new GsaProperty2d((Length)length3));
        }
      }

      if (!string.IsNullOrEmpty(mes)) {
        mes = "." + Environment.NewLine + mes;
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Prop2d" + mes);
      return new GsaProperty2dGoo(null);
    }
  }
}
