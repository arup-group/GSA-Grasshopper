using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaProp2dGoo"/> type.
  /// </summary>
  public class GsaProp2dParameter : GH_PersistentParam<GsaProp2dGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaProp2dGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaProp2dGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("05a034ad-683d-479b-9768-5c04379c0606");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Prop2dParam;

    public GsaProp2dParameter() : base(new GH_InstanceDescription(
      GsaProp2dGoo.Name,
      GsaProp2dGoo.NickName,
      GsaProp2dGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    protected override GH_GetterResult Prompt_Plural(ref List<GsaProp2dGoo> values)
    {
      return GH_GetterResult.cancel;
    }

    protected override GH_GetterResult Prompt_Singular(ref GsaProp2dGoo value)
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
