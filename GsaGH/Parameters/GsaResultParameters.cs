﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a Parameter interface for the Data_GsaSection type.
  /// </summary>
  public class GsaResultsParameter : GH_PersistentParam<GsaResultGoo>
  {
    public GsaResultsParameter()
      : base(new GH_InstanceDescription("Result", "Res", "GSA Result", GsaGH.Components.Ribbon.CategoryName.Name(),
          GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    {
    }

    public override Guid ComponentGuid => new Guid("81f6f103-cb53-414c-908b-6adf46c3260d");

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ResultParam;

    protected override GH_GetterResult Prompt_Plural(ref List<GsaResultGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref GsaResultGoo value)
    {
      return GH_GetterResult.cancel;
    }
    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
    {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Not available",
        Visible = false
      };
      return item;
    }
    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
    {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Not available",
        Visible = false
      };
      return item;
    }

    #region preview methods

    public bool Hidden
    {
      get { return true; }
      //set { m_hidden = value; }
    }
    public bool IsPreviewCapable
    {
      get { return false; }
    }
    #endregion
  }

}
