using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaSectionGoo"/> type.
  /// </summary>
  public class GsaSectionParameter : GH_PersistentParam<GsaSectionGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaSectionGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaSectionGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("8500f335-fad7-46a0-b1be-bdad22ab1474");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.SectionParam;

    public GsaSectionParameter() : base(new GH_InstanceDescription(
      GsaSectionGoo.Name,
      GsaSectionGoo.NickName,
      GsaSectionGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    protected override GH_GetterResult Prompt_Plural(ref List<GsaSectionGoo> values)
    {
      return GH_GetterResult.cancel;
    }

    protected override GH_GetterResult Prompt_Singular(ref GsaSectionGoo value)
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
