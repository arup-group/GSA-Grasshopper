using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to edit a Node
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class Elem2dFromBrep_OBSOLETE : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("4fa7ccd9-530e-4036-b2bf-203017b55611");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateElemsFromBreps;

    public Elem2dFromBrep_OBSOLETE() : base("Element2d from Brep",
      "Elem2dFromBrep",
      "Mesh a non-planar Brep",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      pManager.AddBrepParameter("Brep [in " + unitAbbreviation + "]", "B", "Brep (can be non-planar)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Incl. Points or Nodes [in " + unitAbbreviation + "]", "(P)", "Inclusion points or Nodes", GH_ParamAccess.list);
      pManager.AddGenericParameter("Incl. Curves or 1D Members [in " + unitAbbreviation + "]", "(C)", "Inclusion curves or 1D Members", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter());
      pManager.AddNumberParameter("Mesh Size [" + unitAbbreviation + "]", "Ms", "Targe mesh size", GH_ParamAccess.item, 0);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaElement2dParameter(), "2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.list);
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghbrep = new GH_Brep();
      if (!da.GetData(0, ref ghbrep)) {
        return;
      }

      if (ghbrep == null) {
        this.AddRuntimeWarning("Brep input is null");
      }
      var brep = new Brep();
      if (!GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both)) {
        return;
      }

      var ghTypes = new List<GH_ObjectWrapper>();
      var point3ds = new List<Point3d>();
      var nodes = new List<GsaNode>();
      if (da.GetDataList(1, ghTypes)) {
        foreach (GH_ObjectWrapper objectWrapper in ghTypes) {
          var point3d = new Point3d();
          if (objectWrapper.Value is GsaNodeGoo) {
            var gsanode = new GsaNode();
            objectWrapper.CastTo(ref gsanode);
            nodes.Add(gsanode);
          }
          else if (GH_Convert.ToPoint3d(objectWrapper.Value, ref point3d, GH_Conversion.Both)) {
            point3ds.Add(point3d);
          }
          else {
            string type = objectWrapper.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", "");
            type = type.Replace("Goo", "");
            this.AddRuntimeError("Unable to convert incl. Point/Node input parameter of type " +
                                 type + " to point or node");
          }
        }
      }

      ghTypes = new List<GH_ObjectWrapper>();
      var crvs = new List<Curve>();
      var mem1ds = new List<GsaMember1d>();
      if (da.GetDataList(2, ghTypes)) {
        foreach (GH_ObjectWrapper objectWrapper in ghTypes) {
          Curve crv = null;
          if (objectWrapper.Value is GsaMember1dGoo) {
            var gsamem1d = new GsaMember1d();
            objectWrapper.CastTo(ref gsamem1d);
            mem1ds.Add(gsamem1d);
          }
          else if (GH_Convert.ToCurve(objectWrapper.Value, ref crv, GH_Conversion.Both)) {
            crvs.Add(crv);
          }
          else {
            string type = objectWrapper.Value.GetType().ToString();
            type = type.Replace("GsaGH.Parameters.", "");
            type = type.Replace("Goo", "");
            this.AddRuntimeError("Unable to convert incl. Curve/Mem1D input parameter of type " +
                                 type + " to curve or 1D Member");
          }
        }
      }

      var ghmsz = new GH_Number();
      Length meshSize = Length.Zero;
      if (da.GetData(4, ref ghmsz)) {
        GH_Convert.ToDouble(ghmsz, out double size, GH_Conversion.Both);
        meshSize = new Length(size, _lengthUnit).ToUnit(LengthUnit.Meter);
      }

      var elem2d = new GsaElement2d(brep, crvs, point3ds, meshSize.Value, mem1ds, nodes, _lengthUnit, DefaultUnits.Tolerance);

      var ghTyp = new GH_ObjectWrapper();
      var prop2d = new GsaProp2d();
      if (da.GetData(3, ref ghTyp)) {
        if (ghTyp.Value is GsaProp2dGoo)
          ghTyp.CastTo(ref prop2d);
        else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both))
            prop2d.Id = idd;
          else {
            this.AddRuntimeError("Unable to convert PA input to a 2D Property of reference integer");
            return;
          }
        }
      }
      else
        prop2d.Id = 1;
      var prop2Ds = new List<GsaProp2d>();
      for (int i = 0; i < elem2d.ApiElements.Count; i++)
        prop2Ds.Add(prop2d);
      elem2d.Properties = prop2Ds;

      da.SetData(0, new GsaElement2dGoo(elem2d));

      this.AddRuntimeRemark("This component is work-in-progress and provided 'as-is'. It will unroll the surface, do the meshing, map the mesh back on the original surface. Only single surfaces will work. Surfaces of high curvature and not-unrollable geometries (like a sphere) is unlikely to produce good results");
    }

    #region Custom UI
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new string[]
        {
          "Unit"
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

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
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      int i = 0;
      Params.Input[i++].Name = "Brep [in " + unitAbbreviation + "]";
      Params.Input[i++].Name = "Incl. Points or Nodes [in " + unitAbbreviation + "]";
      Params.Input[i++].Name = "Incl. Curves or 1D Members [in " + unitAbbreviation + "]";
      i++;
      Params.Input[i].Name = "Mesh Size [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

