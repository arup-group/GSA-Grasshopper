using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to create new 1D Element
    /// </summary>
    public class CreateElement1d : GH_OasysComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("88c58aae-4cd8-4d37-b63f-d828571e6941");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateElem1d;

    public CreateElement1d() : base("Create 1D Element",
      "Elem1D",
      "Create GSA 1D Element",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddLineParameter("Line", "L", "Line to create GSA Element", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter());
      pManager[1].Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaElement1dParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Line ghln = new GH_Line();
      if (DA.GetData(0, ref ghln))
      {
        if (ghln == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Line input is null"); }
        Line ln = new Line();
        if (GH_Convert.ToLine(ghln, ref ln, GH_Conversion.Both))
        {
          GsaElement1d elem = new GsaElement1d(new LineCurve(ln));

          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          GsaSection section = new GsaSection();
          if (DA.GetData(1, ref gh_typ))
          {
            if (gh_typ.Value is GsaSectionGoo)
            {
              gh_typ.CastTo(ref section);
              elem.Section = section;
            }
            else
            {
              if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                elem.Section.Id = idd;
              else
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
                return;
              }
            }
          }

          DA.SetData(0, new GsaElement1dGoo(elem));
        }
      }
    }
  }
}
