﻿using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {

  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaAnalysisTaskGoo" /> type.
  /// </summary>
  public class GsaAnalysisTaskParameter : GH_OasysPersistentParam<GsaAnalysisTaskGoo> {

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("51048d67-3652-45d0-9eec-0f9ef339c1a5");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0
        ? "Empty " + GsaAnalysisTaskGoo.Name + " parameter"
        : base.InstanceDescription;

    public override string TypeName
      => SourceCount == 0
        ? GsaAnalysisTaskGoo.Name
        : base.TypeName;

    protected override Bitmap Icon => Resources.AnalysisTaskParam;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaAnalysisTaskParameter() : base(new GH_InstanceDescription(GsaAnalysisTaskGoo.Name,
                          GsaAnalysisTaskGoo.NickName,
      GsaAnalysisTaskGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    #endregion Public Constructors

    #region Protected Methods
    protected override GsaAnalysisTaskGoo PreferredCast(object data) => data.GetType() == typeof(GsaAnalysisTask)
      ? new GsaAnalysisTaskGoo((GsaAnalysisTask)data)
      : base.PreferredCast(data);

    #endregion Protected Methods
  }
}
