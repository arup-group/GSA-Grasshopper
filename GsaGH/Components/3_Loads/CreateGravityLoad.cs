using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components
{
  public class CreateGravityLoad : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("f9099874-92fa-4608-b4ed-a788df85a407");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GravityLoad;

    public CreateGravityLoad() : base("Create Gravity Load",
      "GravityLoad",
      "Create GSA Gravity Load",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    #endregion

    #region input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddIntegerParameter("Load case", "LC", "Load case number (by default 1)", GH_ParamAccess.item, 1);
      pManager.AddTextParameter("Element list", "El", "List of Elements to apply load to (by default 'All')." + System.Environment.NewLine +
         "Element list should take the form:" + System.Environment.NewLine +
         " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
         "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddVectorParameter("Gravity factor", "G", "Gravity vector factor (default z = -1)", GH_ParamAccess.item, new Vector3d(0, 0, -1));
      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Gravity load", "Ld", "GSA Gravity Load", GH_ParamAccess.item);
    }
    #endregion
    protected override void SolveInstance(IGH_DataAccess DA)
    {

      GsaGravityLoad gravityLoad = new GsaGravityLoad();

      //Load case
      int lc = 1;
      GH_Integer gh_lc = new GH_Integer();
      if (DA.GetData(0, ref gh_lc))
        GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
      gravityLoad.GravityLoad.Case = lc;

      //element/beam list
      // check that user has not inputted Gsa geometry elements here
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(1, ref gh_typ))
      {
        string type = gh_typ.Value.ToString().ToUpper();
        if (type.StartsWith("GSA "))
        {
          Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
              "You cannot input a Node/Element/Member in ElementList input!" + System.Environment.NewLine +
              "Element list should take the form:" + System.Environment.NewLine +
              "'1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)'" + System.Environment.NewLine +
              "Refer to GSA help file for definition of lists and full vocabulary.");
          return;
        }
      }
      string beamList = "all";
      GH_String gh_bl = new GH_String();
      if (DA.GetData(1, ref gh_bl))
        GH_Convert.ToString(gh_bl, out beamList, GH_Conversion.Both);
      gravityLoad.GravityLoad.Elements = beamList;

      // 2 Name
      string name = "";
      GH_String gh_name = new GH_String();
      if (DA.GetData(2, ref gh_name))
      {
        if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
          gravityLoad.GravityLoad.Name = name;
      }

      //factor
      Vector3 factor = new Vector3();
      Vector3d vect = new Vector3d(0, 0, -1);
      GH_Vector gh_factor = new GH_Vector();
      if (DA.GetData(3, ref gh_factor))
        GH_Convert.ToVector3d(gh_factor, ref vect, GH_Conversion.Both);
      factor.X = vect.X; factor.Y = vect.Y; factor.Z = vect.Z;

      if (vect.Z > 0)
        AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Just a friendly note that your gravity vector is pointing upwards and that is not normal.");

      gravityLoad.GravityLoad.Factor = factor;

      GsaLoad gsaLoad = new GsaLoad(gravityLoad);
      DA.SetData(0, new GsaLoadGoo(gsaLoad));
    }
  }
}
