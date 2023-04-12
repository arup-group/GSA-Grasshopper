using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaResultGoo" /> type.
  /// </summary>
  public class GsaResultsParameter : GH_OasysPersistentParam<GsaResultGoo> {
    public override Guid ComponentGuid => new Guid("81f6f103-cb53-414c-908b-6adf46c3260d");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0
        ? "Empty " + GsaResultGoo.Name + " parameter"
        : base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0
        ? GsaResultGoo.Name
        : base.TypeName;
    public GsaResultsParameter() : base(new GH_InstanceDescription(GsaResultGoo.Name,
                      GsaResultGoo.NickName,
      GsaResultGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override Bitmap Icon => Resources.ResultParam;

    protected override GsaResultGoo PreferredCast(object data)
      => data.GetType() == typeof(GsaResult)
        ? new GsaResultGoo((GsaResult)data)
        : base.PreferredCast(data);
  }
}
