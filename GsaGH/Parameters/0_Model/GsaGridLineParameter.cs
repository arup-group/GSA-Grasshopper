using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaGridLineGoo" /> type.
  /// </summary>
  public class GsaGridLineParameter : GH_OasysPersistentParam<GsaGridLineGoo> {
    public override Guid ComponentGuid => new Guid("e5e50621-12c9-4ac4-abb0-926d60414ea7");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaGridLineGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaGridLineGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.ListParam;

    public GsaGridLineParameter() : base(new GH_InstanceDescription(GsaGridLineGoo.Name,
      GsaGridLineGoo.NickName, GsaGridLineGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaGridLineGoo PreferredCast(object data) {
      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Grid Line");
      return new GsaGridLineGoo(null);
    }
  }
}
