using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create new 3d Member
  /// </summary>
  public class CreateMember3d : GH_OasysDropDownComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
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

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGeometryParameter("Solid", "S", "Solid Geometry - Closed Brep or Mesh", GH_ParamAccess.item);
      pManager.AddParameter(new GsaProp3dParameter());
      pManager.AddGenericParameter("Mesh Size [" + unitAbbreviation + "]", "Ms", "Targe mesh size", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMember3dParameter());
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
          mem.MeshSize = (Length)Input.UnitNumber(this, DA, 2, this.LengthUnit, true);

        DA.SetData(0, new GsaMember3dGoo(mem));
      }
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Length
      this.DropDownItems.Add(FilteredUnits.FilteredLengthUnits);
      this.SelectedItems.Add(this.LengthUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance()
    {
      Params.Input[2].Name = "Mesh Size [" + Length.GetAbbreviation(this.LengthUnit) + "]";
    }
    #endregion
  }
}

