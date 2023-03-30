using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {

  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaOffsetGoo"/> type.
  /// </summary>
  public class GsaOffsetParameter : GH_OasysPersistentParam<GsaOffsetGoo> {

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("0b14f16e-bd6a-4da7-991a-359f64aa28fd");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription => m_data.DataCount == 0
              ? "Empty " + GsaOffsetGoo.Name + " parameter"
      : base.InstanceDescription;

    public override string TypeName => SourceCount == 0
      ? GsaOffsetGoo.Name
      : base.TypeName;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.OffsetParam;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaOffsetParameter() : base(new GH_InstanceDescription(
      GsaOffsetGoo.Name,
      GsaOffsetGoo.NickName,
      GsaOffsetGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    #endregion Public Constructors

    #region Protected Methods
    protected override GsaOffsetGoo PreferredCast(object data) {
      return data.GetType() == typeof(GsaOffset)
        ? new GsaOffsetGoo((GsaOffset)data)
        : base.PreferredCast(data);
    }

    #endregion Protected Methods
  }
}
