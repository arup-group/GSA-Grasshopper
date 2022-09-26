using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create new 2D Element
  /// </summary>
  public class CreateElement2d : GH_OasysComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("8f83d32a-c2df-4f47-9cfc-d2d4253703e1");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateElem2d;

    public CreateElement2d() : base("Create 2D Element",
      "Elem2D",
      "Create GSA 2D Element",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat2())
    { }
    #endregion

    #region Custom UI
    //This region overrides the typical component layout


    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddMeshParameter("Mesh", "M", "Mesh to create GSA Element", GH_ParamAccess.item);
      pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property. Input either a GSA 2D Property or an Integer to use a Section already defined in model", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("2D Element", "E2D", "GSA 2D Element", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Mesh ghmesh = new GH_Mesh();
      if (DA.GetData(0, ref ghmesh))
      {
        if (ghmesh == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Mesh input is null"); }
        Mesh mesh = new Mesh();
        if (GH_Convert.ToMesh(ghmesh, ref mesh, GH_Conversion.Both))
        {
          GsaElement2d elem = new GsaElement2d(mesh);

          // 1 section
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          GsaProp2d prop2d = new GsaProp2d();
          if (DA.GetData(1, ref gh_typ))
          {
            if (gh_typ.Value is GsaProp2dGoo)
              gh_typ.CastTo(ref prop2d);
            else
            {
              if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
              {
                prop2d.ID = idd;
              }
              else
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
                return;
              }
            }
          }

          List<GsaProp2d> prop2Ds = new List<GsaProp2d>();
          for (int i = 0; i < elem.API_Elements.Count; i++)
            prop2Ds.Add(prop2d);
          elem.Properties = prop2Ds;

          DA.SetData(0, new GsaElement2dGoo(elem));
        }
      }
    }
  }
}

