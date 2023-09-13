using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaBucklingFactorsGoo" /> type.
  /// </summary>
  public class
    GsaBucklingFactorsParameter : GH_OasysPersistentParam<GsaBucklingFactorsGoo> {
    public override Guid ComponentGuid => new Guid("e2349b4f-1ebb-4661-99d9-07c6a3ef22b9");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaBucklingFactorsGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaBucklingFactorsGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.BucklingFactorsParam;

    public GsaBucklingFactorsParameter() : base(new GH_InstanceDescription(
      GsaBucklingFactorsGoo.Name, GsaBucklingFactorsGoo.NickName,
      GsaBucklingFactorsGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaBucklingFactorsGoo PreferredCast(object data) {
      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to BucklingLengthFactors");
      return new GsaBucklingFactorsGoo(null);
    }
  }
}
