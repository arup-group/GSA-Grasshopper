using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {

  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaModelGoo"/> type.
  /// </summary>
  public class GsaModelParameter : GH_OasysPersistentParam<GsaModelGoo> {

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("43eb8fb6-d469-4c3b-ab3c-e8d6ad378d9a");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override string InstanceDescription => m_data.DataCount == 0
              ? "Empty " + GsaModelGoo.Name + " parameter"
      : base.InstanceDescription;

    public override string TypeName => SourceCount == 0
      ? GsaModelGoo.Name
      : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.ModelParam;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaModelParameter() : base(new GH_InstanceDescription(
      GsaModelGoo.Name,
      GsaModelGoo.NickName,
      GsaModelGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    #endregion Public Constructors

    #region Protected Methods
    protected override GsaModelGoo PreferredCast(object data) {
      return data.GetType() == typeof(GsaModel)
        ? new GsaModelGoo((GsaModel)data)
        : base.PreferredCast(data);
    }

    #endregion Protected Methods
  }
}
