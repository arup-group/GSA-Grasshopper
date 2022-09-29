using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaElement3dGoo"/> type.
  /// </summary>
  public class GsaElement3dParameter : GH_PersistentGeometryParam<GsaElement3dGoo>, IGH_PreviewObject
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaElement3dGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaElement3dGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("e7326f8e-c8e5-40d9-b8e4-6912ccf80b92");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Elem3dParam;

    public GsaElement3dParameter() : base(new GH_InstanceDescription(
      GsaElement3dGoo.Name,
      GsaElement3dGoo.NickName,
      GsaElement3dGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    //We do not allow users to pick parameter, 
    //therefore the following 4 methods disable all this ui.
    protected override GH_GetterResult Prompt_Plural(ref List<GsaElement3dGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref GsaElement3dGoo value)
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
    public BoundingBox ClippingBox
    {
      get
      {
        return Preview_ComputeClippingBox();
      }
    }
    public void DrawViewportMeshes(IGH_PreviewArgs args)
    {
      //Use a standard method to draw gunk, you don't have to specifically implement this.
      Preview_DrawMeshes(args);
    }
    public void DrawViewportWires(IGH_PreviewArgs args)
    {
      //Use a standard method to draw gunk, you don't have to specifically implement this.
      Preview_DrawWires(args);
    }

    private bool m_hidden = false;
    public bool Hidden
    {
      get { return m_hidden; }
      set { m_hidden = value; }
    }
    public bool IsPreviewCapable
    {
      get { return true; }
    }
    #endregion
  }
}
