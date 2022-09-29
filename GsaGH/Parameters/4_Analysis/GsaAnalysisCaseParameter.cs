using System;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaAnalysisCaseGoo"/> type.
  /// </summary>
  public class GsaAnalysisCaseParameter : GH_PersistentParam<GsaAnalysisCaseGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaAnalysisCaseGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaAnalysisCaseGoo.Name : base.TypeName;

    public override Guid ComponentGuid => new Guid("6b99a192-bdbd-41bf-8efa-1bc146d3c224");

    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.AnalysisCaseParam;

    public GsaAnalysisCaseParameter() : base(new GH_InstanceDescription(
     GsaAnalysisCaseGoo.Name,
     GsaAnalysisCaseGoo.NickName,
     GsaAnalysisCaseGoo.Description + " parameter",
     GsaGH.Components.Ribbon.CategoryName.Name(),
     GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    protected override GH_GetterResult Prompt_Plural(ref List<GsaAnalysisCaseGoo> values)
    {
      return GH_GetterResult.cancel;
    }

    protected override GH_GetterResult Prompt_Singular(ref GsaAnalysisCaseGoo value)
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
