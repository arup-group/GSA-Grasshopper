using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {

  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaBucklingLengthFactorsGoo"/> type.
  /// </summary>
  public class GsaBucklingLengthFactorsParameter : GH_OasysPersistentParam<GsaBucklingLengthFactorsGoo> {

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("e2349b4f-1ebb-4661-99d9-07c6a3ef22b9");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription => m_data.DataCount == 0
              ? "Empty " + GsaBucklingLengthFactorsGoo.Name + " parameter"
      : base.InstanceDescription;

    public override string TypeName => SourceCount == 0
      ? GsaBucklingLengthFactorsGoo.Name
      : base.TypeName;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.BucklingFactorsParam;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaBucklingLengthFactorsParameter() : base(new GH_InstanceDescription(
      GsaBucklingLengthFactorsGoo.Name,
      GsaBucklingLengthFactorsGoo.NickName,
      GsaBucklingLengthFactorsGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    #endregion Public Constructors

    #region Protected Methods
    protected override GsaBucklingLengthFactorsGoo PreferredCast(object data) {
      return data.GetType() == typeof(GsaBucklingLengthFactors)
        ? new GsaBucklingLengthFactorsGoo((GsaBucklingLengthFactors)data)
        : base.PreferredCast(data);
    }

    #endregion Protected Methods
  }
}
