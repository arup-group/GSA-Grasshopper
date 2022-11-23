using System;
using System.Collections.Generic;
using System.Linq;
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

namespace GsaGH.Components
{
    /// <summary>
    /// Component to create new 2D Member
    /// </summary>
    public class CreateMember2d_OBSOLETE : GH_OasysDropDownComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("df0c2786-9e46-4500-ab63-0c4162a580d4");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateMem2d;

    public CreateMember2d_OBSOLETE() : base("Create 2D Member",
      "Mem2D",
      "Create GSA Member 2D",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddBrepParameter("Brep", "B", "Planar Brep (non-planar geometry will be automatically converted to an average plane of exterior boundary control points))", GH_ParamAccess.item);
      pManager.AddPointParameter("Incl. Points", "(P)", "Inclusion points (will automatically be projected onto Brep)", GH_ParamAccess.list);
      pManager.AddCurveParameter("Incl. Curves", "(C)", "Inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines)", GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter());
      pManager.AddGenericParameter("Mesh Size [" + unitAbbreviation + "]", "Ms", "Target mesh size", GH_ParamAccess.item);

      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMember2dParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Brep ghbrep = new GH_Brep();
      if (DA.GetData(0, ref ghbrep))
      {
        if (ghbrep == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Brep input is null"); }
        Brep brep = new Brep();
        if (GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both))
        {
          // 1 Points
          List<Point3d> pts = new List<Point3d>();
          List<GH_Point> ghpts = new List<GH_Point>();
          if (DA.GetDataList(1, ghpts))
          {
            for (int i = 0; i < ghpts.Count; i++)
            {
              Point3d pt = new Point3d();
              if (GH_Convert.ToPoint3d(ghpts[i], ref pt, GH_Conversion.Both))
                pts.Add(pt);
            }
          }

          // 2 Curves
          List<Curve> crvs = new List<Curve>();
          List<GH_Curve> ghcrvs = new List<GH_Curve>();
          if (DA.GetDataList(2, ghcrvs))
          {
            for (int i = 0; i < ghcrvs.Count; i++)
            {
              Curve crv = null;
              if (GH_Convert.ToCurve(ghcrvs[i], ref crv, GH_Conversion.Both))
                crvs.Add(crv);
            }
          }

          // build new member with brep, crv and pts
          GsaMember2d mem = new GsaMember2d(brep, crvs, pts);

          // 3 section
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          GsaProp2d prop2d = new GsaProp2d();
          if (DA.GetData(3, ref gh_typ))
          {
            if (gh_typ.Value is GsaProp2dGoo)
            {
              gh_typ.CastTo(ref prop2d);
              mem.Property = prop2d;
            }
            else
            {
              if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                mem.Property = new GsaProp2d(idd);
              else
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
                return;
              }
            }
          }

          // 4 mesh size
          if (this.Params.Input[4].SourceCount > 0)
            mem.MeshSize = ((Length)Input.UnitNumber(this, DA, 4, this.LengthUnit, true)).Meters;

          DA.SetData(0, new GsaMember2dGoo(mem));
        }
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
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance()
    {
      Params.Input[4].Name = "Mesh Size [" + Length.GetAbbreviation(this.LengthUnit) + "]";
    }
    #endregion
  }
}

