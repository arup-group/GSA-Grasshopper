using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaLoadGoo" /> type.
  /// </summary>
  public class GsaLoadParameter : GH_OasysPersistentParam<GsaLoadGoo> {
    public override Guid ComponentGuid => new Guid("2833ef04-c595-4b05-8db3-622c75fa9a25");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaLoadGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaLoadGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.LoadParam;

    public GsaLoadParameter() : base(new GH_InstanceDescription(GsaLoadGoo.Name,
      GsaLoadGoo.NickName, GsaLoadGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaLoadGoo PreferredCast(object data) {
      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Load");
      return new GsaLoadGoo(null);
    }
  }
}
