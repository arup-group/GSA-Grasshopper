using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;
using OasysGH.Units;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaOffsetGoo" /> type.
  /// </summary>
  public class GsaOffsetParameter : GH_OasysPersistentParam<GsaOffsetGoo> {
    public override Guid ComponentGuid => new Guid("0b14f16e-bd6a-4da7-991a-359f64aa28fd");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaOffsetGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaOffsetGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.OffsetParam;

    public GsaOffsetParameter() : base(new GH_InstanceDescription(GsaOffsetGoo.Name,
      GsaOffsetGoo.NickName, GsaOffsetGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaOffsetGoo PreferredCast(object data) {
      if (GH_Convert.ToDouble(data, out double myval, GH_Conversion.Both)) {
        this.AddRuntimeWarning("Number converted to Z-offset in " +
          DefaultUnits.LengthUnitSection.ToString());
        return new GsaOffsetGoo(new GsaOffset(0, 0, 0, myval, DefaultUnits.LengthUnitSection));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Offset");
      return new GsaOffsetGoo(null);
    }
  }
}
