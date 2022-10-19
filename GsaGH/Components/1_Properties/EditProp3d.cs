using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Prop3d and ouput the information
  /// </summary>
  public class EditProp3d : GH_OasysComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("5e28d4d9-a0ab-46a8-8476-71781c315855");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditProp3d;

    public EditProp3d() : base("Edit 3D Property",
      "Prop3dEdit",
      "Modify GSA 3D Property",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("3D Property", "PV", "GSA 3D Property to get or set information for", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop3d Number", "ID", "Set 3D Property Number. If ID is set it will replace any existing 3D Property in the model", GH_ParamAccess.item);
      pManager.AddGenericParameter("Material", "Ma", "Set GSA Material or reference existing material by ID", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax", "Set Axis as integer: Global (0) or Topological (1)", GH_ParamAccess.item);
      pManager.AddTextParameter("Prop3d Name", "Na", "Set Name of 3D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop3d Colour", "Co", "Set 3D Property Colour", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("3D Property", "PV", "GSA 3D Property with changes", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID", "3D Property Number", GH_ParamAccess.item);
      pManager.AddGenericParameter("Material", "Ma", "Get GSA Material", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax", "Get Axis: Global (0) or Topological (1)", GH_ParamAccess.item);
      pManager.AddTextParameter("Prop3d Name", "Na", "Name of 3D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop3d Colour", "Co", "3D Property Colour", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaProp3d gsaProp3d = new GsaProp3d();
      GsaProp3d prop = new GsaProp3d();
      if (DA.GetData(0, ref gsaProp3d))
      {
        prop = gsaProp3d.Duplicate();
      }

      // #### inputs ####
      // 1 ID
      GH_Integer ghID = new GH_Integer();
      if (DA.GetData(1, ref ghID))
      {
        if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
          prop.ID = id;
      }

      // 2 Material
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(2, ref gh_typ))
      {
        GsaMaterial material = new GsaMaterial();
        if (gh_typ.Value is GsaMaterialGoo)
        {
          gh_typ.CastTo(ref material);
          prop.Material = material;
        }
        else
        {
          if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
            prop.MaterialID = idd;
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
            return;
          }
        }
      }

      // 3 Axis
      GH_Integer ghax = new GH_Integer();
      if (DA.GetData(3, ref ghax))
      {
        if (GH_Convert.ToInt32(ghax, out int axis, GH_Conversion.Both))
        {
          prop.AxisProperty = axis;
        }
      }

      // 4 name
      GH_String ghnm = new GH_String();
      if (DA.GetData(4, ref ghnm))
      {
        if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
          prop.Name = name;
      }

      // 5 Colour
      GH_Colour ghcol = new GH_Colour();
      if (DA.GetData(5, ref ghcol))
      {
        if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
          prop.Colour = col;
      }

      //#### outputs ####
      int ax = (prop.API_Prop3d == null) ? 0 : prop.AxisProperty;
      string nm = (prop.API_Prop3d == null) ? "--" : prop.Name;
      ValueType colour = (prop.API_Prop3d == null) ? null : prop.API_Prop3d.Colour;

      DA.SetData(0, new GsaProp3dGoo(prop));
      DA.SetData(1, prop.ID);
      DA.SetData(2, new GsaMaterialGoo(new GsaMaterial(prop)));
      DA.SetData(3, ax);
      DA.SetData(4, nm);
      DA.SetData(5, colour);
    }
  }
}