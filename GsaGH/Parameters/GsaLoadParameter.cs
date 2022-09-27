﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a Parameter interface for the Data_GsaLoad type.
  /// </summary>
  public class GsaLoadParameter : GH_PersistentParam<GsaLoadGoo>
  {
    public GsaLoadParameter()
      : base(new GH_InstanceDescription("Load", "Ld", "GSA Load", GsaGH.Components.Ribbon.CategoryName.Name(), GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    {
    }

    public override Guid ComponentGuid => new Guid("2833ef04-c595-4b05-8db3-622c75fa9a25");

    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.LoadParam;

    protected override GH_GetterResult Prompt_Plural(ref List<GsaLoadGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref GsaLoadGoo value)
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
