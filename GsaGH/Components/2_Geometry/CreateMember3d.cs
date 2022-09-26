using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create new 3d Member
  /// </summary>
  public class CreateMember3d : GH_OasysComponent, IGH_PreviewObject, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("df0c7608-9e46-4500-ab63-0c4162a580d4");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateMem3d;

    public CreateMember3d() : base("Create 3D Member",
      "Mem3D",
      "Create GSA Member 3D",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat2())
    { }
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // length
        //dropdownitems.Add(Enum.GetNames(typeof(Units.LengthUnit)).ToList());
        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        IQuantity quantity = new Length(0, lengthUnit);
        unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    private void UpdateUIFromSelectedItems()
    {
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[0]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Unit"
    });
    private bool first = true;
    private LengthUnit lengthUnit = Units.LengthUnitGeometry;
    string unitAbbreviation;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity length = new Length(0, lengthUnit);
      unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGeometryParameter("Solid", "S", "Solid Geometry - Closed Brep or Mesh", GH_ParamAccess.item);
      pManager.AddGenericParameter("3D Prop", "PV", "GSA 3D Property. Input either a GSA 3D Property, a GSA Material or an Integer to use a 3D Property already defined in model", GH_ParamAccess.item);
      pManager.AddGenericParameter("Mesh Size [" + unitAbbreviation + "]", "Ms", "Targe mesh size", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("3D Member", "M3D", "GSA 3D Member", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Solid input is null"); }
        GsaMember3d mem = new GsaMember3d();
        Brep brep = new Brep();
        Mesh mesh = new Mesh();
        if (GH_Convert.ToBrep(gh_typ.Value, ref brep, GH_Conversion.Both))
        {
          if (brep.IsValid)
            mem = new GsaMember3d(brep);
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "S input is not a valid Brep geometry");
            return;
          }
        }
        else if (GH_Convert.ToMesh(gh_typ.Value, ref mesh, GH_Conversion.Both))
          mem = new GsaMember3d(mesh);
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Geometry input to a 3D Member");
          return;
        }

        // 1 prop3d to be implemented GsaAPI
        gh_typ = new GH_ObjectWrapper();
        GsaProp3d prop3d = new GsaProp3d();
        if (DA.GetData(1, ref gh_typ))
        {
          if (gh_typ.Value is GsaProp3dGoo)
          {
            gh_typ.CastTo(ref prop3d);
            mem.Property = prop3d;
          }
          else if (gh_typ.Value is GsaMaterialGoo)
          {
            GsaMaterial mat = new GsaMaterial();
            gh_typ.CastTo(ref mat);
            prop3d = new GsaProp3d(mat);
            mem.Property = prop3d;
          }
          else
          {
            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
              mem.PropertyID = idd; //new GsaProp3d(idd);
            else
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
              return;
            }
          }
        }

        // 2 mesh size
        if (this.Params.Input[2].SourceCount > 0)
        {
          mem.MeshSize = GetInput.GetLength(this, DA, 2, lengthUnit, true);
        }

        DA.SetData(0, new GsaMember3dGoo(mem));
      }
    }
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      try // if users has an old versopm of this component then dropdown menu wont read
      {
        Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
      }
      catch (Exception) // we create the dropdown menu with our chosen default
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // set length to meters as this was the only option for old components
        lengthUnit = LengthUnit.Meter;

        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        IQuantity quantity = new Length(0, lengthUnit);
        unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));
      }

      UpdateUIFromSelectedItems();

      first = false;

      return base.Read(reader);
    }
    #endregion

    #region IGH_VariableParameterComponent null implementation
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      IQuantity length = new Length(0, lengthUnit);
      unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
      Params.Input[2].Name = "Mesh Size [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

