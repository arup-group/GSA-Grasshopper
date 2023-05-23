using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using OasysGH.Units;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaBucklingLengthFactorsGoo" /> type.
  /// </summary>
  public class
    GsaBucklingLengthFactorsParameter : GH_OasysPersistentParam<GsaBucklingLengthFactorsGoo> {
    public override Guid ComponentGuid => new Guid("e2349b4f-1ebb-4661-99d9-07c6a3ef22b9");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaBucklingLengthFactorsGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName
      => SourceCount == 0 ? GsaBucklingLengthFactorsGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.BucklingFactorsParam;

    public GsaBucklingLengthFactorsParameter() : base(new GH_InstanceDescription(
      GsaBucklingLengthFactorsGoo.Name, GsaBucklingLengthFactorsGoo.NickName,
      GsaBucklingLengthFactorsGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaBucklingLengthFactorsGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaBucklingLengthFactors)) {
        return new GsaBucklingLengthFactorsGoo((GsaBucklingLengthFactors)data);
      }

      AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
        $"Data conversion failed from {data.GetTypeName()} to BucklingLengthFactors");
      return new GsaBucklingLengthFactorsGoo(null);
    }
  }
}
