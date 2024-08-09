using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaAssemblyGoo" /> type.
  /// </summary>
  public class GsaAssemblyParameter : GH_OasysPersistentParam<GsaAssemblyGoo> {
    public override Guid ComponentGuid => new Guid("4b39f91c-4558-4432-96d1-0bbec98a3447");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaAssemblyGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaAssemblyGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.AssemblyParam;

    public GsaAssemblyParameter() : base(new GH_InstanceDescription(GsaAssemblyGoo.Name,
      GsaAssemblyGoo.NickName, GsaAssemblyGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }
  }
}
