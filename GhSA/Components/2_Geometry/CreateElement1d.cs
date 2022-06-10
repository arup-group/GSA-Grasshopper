using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create new 1D Element
  /// </summary>
  public class CreateElement1d : GH_Component, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("88c58aae-4cd8-4d37-b63f-d828571e6941");
    public CreateElement1d()
      : base("Create 1D Element", "Elem1D", "Create GSA 1D Element",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    {
    }

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateElem1d;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddCurveParameter("Line", "L", "Line to create GSA Element", GH_ParamAccess.item);
      pManager.AddGenericParameter("Section", "PB", "GSA Section Property. Input either a GSA Section or an Integer to use a Section already defined in model", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("1D Element", "E1D", "GSA 1D Element", GH_ParamAccess.item);

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

          // 1 section
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
                elem.Section.ID = idd;
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

