using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
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
  ///   Component to create new 3d Member
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class CreateMember3d_OBSOLETE : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("df0c7608-9e46-4500-ab63-0c4162a580d4");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateMem3d;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public CreateMember3d_OBSOLETE() : base("Create 3D Member", "Mem3D", "Create GSA Member 3D",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      Params.Input[2].Name = "Mesh Size [" + Length.GetAbbreviation(_lengthUnit) + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddGeometryParameter("Solid", "S", "Solid Geometry - Closed Brep or Mesh",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaProp3dParameter());
      pManager.AddGenericParameter("Mesh Size [" + unitAbbreviation + "]", "Ms", "Targe mesh size",
        GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember3dParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      if (ghTyp == null) {
        this.AddRuntimeWarning("Solid input is null");
      }

      GsaMember3d member;
      var brep = new Brep();
      var mesh = new Mesh();
      if (GH_Convert.ToBrep(ghTyp.Value, ref brep, GH_Conversion.Both)) {
        if (brep.IsValid) {
          member = new GsaMember3d(brep);
        } else {
          this.AddRuntimeError("S input is not a valid Brep geometry");
          return;
        }
      } else if (GH_Convert.ToMesh(ghTyp.Value, ref mesh, GH_Conversion.Both)) {
        member = new GsaMember3d(mesh);
      } else {
        this.AddRuntimeError("Unable to convert Geometry input to a 3D Member");
        return;
      }

      ghTyp = new GH_ObjectWrapper();
      if (da.GetData(1, ref ghTyp)) {
        switch (ghTyp.Value) {
          case GsaProp3dGoo value:
            member.Prop3d = value.Value;
            break;

          case GsaMaterialGoo value: {
            member.Prop3d = new GsaProp3d(value.Value);
            break;
          }
          default: {
            if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both)) {
              member.Prop3d = new GsaProp3d(id);
            } else {
              this.AddRuntimeError(
                "Unable to convert PA input to a 2D Property of reference integer");
              return;
            }

            break;
          }
        }
      }

      if (Params.Input[2].SourceCount > 0) {
        member.MeshSize = ((Length)Input.UnitNumber(this, da, 2, _lengthUnit, true)).Meters;
      }

      da.SetData(0, new GsaMember3dGoo(member));
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
