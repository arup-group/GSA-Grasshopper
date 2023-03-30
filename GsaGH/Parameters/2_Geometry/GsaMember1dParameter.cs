using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {

  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaMember1dGoo" /> type.
  /// </summary>
  public class GsaMember1dParameter : GH_OasysPersistentGeometryParam<GsaMember1dGoo> {

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("0392a5a0-7762-4214-8c30-fb395365056e");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override string InstanceDescription
      => m_data.DataCount == 0
        ? "Empty " + GsaMember1dGoo.Name + " parameter"
        : base.InstanceDescription;

    public override string TypeName
      => SourceCount == 0
        ? GsaMember1dGoo.Name
        : base.TypeName;

    protected override Bitmap Icon => Resources.Mem1dParam;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaMember1dParameter() : base(new GH_InstanceDescription(GsaMember1dGoo.Name,
                          GsaMember1dGoo.NickName,
      GsaMember1dGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    #endregion Public Constructors

    #region Public Methods
    public override void DrawViewportMeshes(IGH_PreviewArgs args) {
    }

    #endregion Public Methods

    #region Protected Methods
    protected override GsaMember1dGoo PreferredCast(object data)
          => data.GetType() == typeof(GsaMember1d)
        ? new GsaMember1dGoo((GsaMember1d)data)
        : base.PreferredCast(data);

    #endregion Protected Methods
  }
}
