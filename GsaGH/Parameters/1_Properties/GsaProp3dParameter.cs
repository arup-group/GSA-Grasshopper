using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaProp3dGoo"/> type.
  /// </summary>
  public class GsaProp3dParameter : GH_PersistentParam<GsaProp3dGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaProp3dGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaProp3dGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("277c96bb-8ea4-4d95-ab02-2954f14203f3");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Prop3dParam;

    public GsaProp3dParameter() : base(new GH_InstanceDescription(
      GsaProp3dGoo.Name,
      GsaProp3dGoo.NickName,
      GsaProp3dGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    protected override GH_GetterResult Prompt_Plural(ref List<GsaProp3dGoo> values)
    {
      return GH_GetterResult.cancel;
    }

    protected override GH_GetterResult Prompt_Singular(ref GsaProp3dGoo value)
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
