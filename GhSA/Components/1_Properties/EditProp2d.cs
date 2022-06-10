using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using UnitsNet;
using System.Linq;
using UnitsNet.GH;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Prop2d and ouput the information
  /// </summary>
  public class EditProp2d : GH_Component, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("4cfdee19-451b-4ee3-878b-93a86767ffef");
    public EditProp2d()
      : base("Edit 2D Property", "Prop2dEdit", "Modify GSA 2D Property",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.tertiary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditProp2d;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout


    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity quantity = new Length(0, Units.LengthUnitSection);
      string unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property to get or set information for", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID", "Set 2D Property Number. If ID is set it will replace any existing 2D Property in the model", GH_ParamAccess.item);
      pManager.AddGenericParameter("Material", "Ma", "Set GSA Material or reference existing material by ID", GH_ParamAccess.item);
      pManager.AddGenericParameter("Thickness [" + unitAbbreviation + "]", "Th", "Set Property Thickness", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax", "Set Axis as integer: Global (0) or Topological (1)", GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Set Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "Set 2D Property Colour", GH_ParamAccess.item);

      for (int i = 1; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property with changes", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Prop2d Number", "ID", "2D Property Number", GH_ParamAccess.item);
      pManager.AddGenericParameter("Material", "Ma", "Get GSA Material", GH_ParamAccess.item);
      pManager.AddGenericParameter("Thickness", "Th", "Get Property Thickness", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax", "Get Axis: Global (0) or Topological (1)", GH_ParamAccess.item);
      pManager.AddTextParameter("Prop2d Name", "Na", "Name of 2D Proerty", GH_ParamAccess.item);
      pManager.AddColourParameter("Prop2d Colour", "Co", "2D Property Colour", GH_ParamAccess.item);
      pManager.AddTextParameter("Type", "Ty", "2D Property Type", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaProp2d gsaProp2d = new GsaProp2d();
      GsaProp2d prop = new GsaProp2d();
      if (DA.GetData(0, ref gsaProp2d))
      {
        prop = gsaProp2d.Duplicate();
      }
      else
      {
        return;
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

      // 3 Thickness
      if (this.Params.Input[3].SourceCount > 0)
        prop.Thickness = GetInput.Length(this, DA, 3, Units.LengthUnitSection, true);

      // 4 Axis
      GH_Integer ghax = new GH_Integer();
      if (DA.GetData(4, ref ghax))
      {
        if (GH_Convert.ToInt32(ghax, out int axis, GH_Conversion.Both))
        {
          prop.AxisProperty = axis;
        }
      }

      // 5 name
      GH_String ghnm = new GH_String();
      if (DA.GetData(5, ref ghnm))
      {
        if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
          prop.Name = name;
      }

      // 6 Colour
      GH_Colour ghcol = new GH_Colour();
      if (DA.GetData(6, ref ghcol))
      {
        if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
          prop.Colour = col;
      }

      //#### outputs ####
      int ax = (prop.API_Prop2d == null) ? 0 : prop.AxisProperty;
      string nm = (prop.API_Prop2d == null) ? "--" : prop.Name;
      ValueType colour = (prop.API_Prop2d == null) ? null : prop.API_Prop2d.Colour;

      DA.SetData(0, new GsaProp2dGoo(prop));
      DA.SetData(1, prop.ID);
      DA.SetData(2, new GsaMaterialGoo(new GsaMaterial(prop)));
      DA.SetData(3, new GH_UnitNumber(prop.Thickness));
      DA.SetData(4, ax);
      DA.SetData(5, nm);
      DA.SetData(6, colour);

      string str = (prop.API_Prop2d == null) ? "--" : prop.Type.ToString();
      if (prop.API_Prop2d == null)
        str = Char.ToUpper(str[0]) + str.Substring(1).ToLower().Replace("_", " ");
      DA.SetData(7, str);
    }
  }
}

