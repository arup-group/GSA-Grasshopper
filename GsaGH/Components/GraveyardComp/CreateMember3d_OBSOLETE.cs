using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create new 3d Member
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class CreateMember3d_OBSOLETE : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("df0c7608-9e46-4500-ab63-0c4162a580d4");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateMem3d;

    public CreateMember3d_OBSOLETE() : base("Create 3D Member",
      "Mem3D",
      "Create GSA Member 3D",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddGeometryParameter("Solid", "S", "Solid Geometry - Closed Brep or Mesh", GH_ParamAccess.item);
      pManager.AddParameter(new GsaProp3dParameter());
      pManager.AddGenericParameter("Mesh Size [" + unitAbbreviation + "]", "Ms", "Targe mesh size", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember3dParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      if (ghTyp == null) {
        this.AddRuntimeWarning("Solid input is null");
      }
      var mem = new GsaMember3d();
      var brep = new Brep();
      var mesh = new Mesh();
      if (GH_Convert.ToBrep(ghTyp.Value, ref brep, GH_Conversion.Both)) {
        if (brep.IsValid)
          mem = new GsaMember3d(brep);
        else {
          this.AddRuntimeError("S input is not a valid Brep geometry");
          return;
        }
      }
      else if (GH_Convert.ToMesh(ghTyp.Value, ref mesh, GH_Conversion.Both))
        mem = new GsaMember3d(mesh);
      else {
        this.AddRuntimeError("Unable to convert Geometry input to a 3D Member");
        return;
      }

      ghTyp = new GH_ObjectWrapper();
      var prop3d = new GsaProp3d();
      if (da.GetData(1, ref ghTyp)) {
        switch (ghTyp.Value)
        {
          case GsaProp3dGoo _:
            ghTyp.CastTo(ref prop3d);
            mem.Prop3d = prop3d;
            break;
          case GsaMaterialGoo _:
          {
            var mat = new GsaMaterial();
            ghTyp.CastTo(ref mat);
            prop3d = new GsaProp3d(mat);
            mem.Prop3d = prop3d;
            break;
          }
          default: {
            if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both))
              mem.Prop3d = new GsaProp3d(id);
            else {
              this.AddRuntimeError("Unable to convert PA input to a 2D Property of reference integer");
              return;
            }

            break;
          }
        }
      }

      if (Params.Input[2].SourceCount > 0)
        mem.MeshSize = ((Length)Input.UnitNumber(this, da, 2, _lengthUnit, true)).Meters;

      da.SetData(0, new GsaMember3dGoo(mem));
    }

    #region Custom UI
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new []
        {
          "Unit",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      // Length
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance() {
      Params.Input[2].Name = "Mesh Size [" + Length.GetAbbreviation(_lengthUnit) + "]";
    }
    #endregion
  }
}

