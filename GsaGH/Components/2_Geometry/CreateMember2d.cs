using System;
using System.Collections.Generic;
using System.Drawing;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create new 2D Member
  /// </summary>
  public class CreateMember2d : GH_OasysDropDownComponent {
    protected override void SolveInstance(IGH_DataAccess da) {
      var ghbrep = new GH_Brep();
      if (!da.GetData(0, ref ghbrep))
        return;

      if (ghbrep == null)
        this.AddRuntimeWarning("Brep input is null");
      var brep = new Brep();
      if (!GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both))
        return;

      var points = new List<Point3d>();
      var ghpts = new List<GH_Point>();
      if (da.GetDataList(1, ghpts))
        foreach (GH_Point point in ghpts) {
          var pt = new Point3d();
          if (GH_Convert.ToPoint3d(point, ref pt, GH_Conversion.Both))
            points.Add(pt);
        }

      var crvs = new List<Curve>();
      var ghcrvs = new List<GH_Curve>();
      if (da.GetDataList(2, ghcrvs))
        foreach (GH_Curve curve in ghcrvs) {
          Curve crv = null;
          if (GH_Convert.ToCurve(curve, ref crv, GH_Conversion.Both))
            crvs.Add(crv);
        }

      var mem = new GsaMember2d();
      try {
        mem = new GsaMember2d(brep, crvs, points);
      }
      catch (Exception e) {
        this.AddRuntimeWarning(e.Message);
      }

      var ghTyp = new GH_ObjectWrapper();
      var prop2d = new GsaProp2d();
      if (da.GetData(3, ref ghTyp)) {
        if (ghTyp.Value is GsaProp2dGoo) {
          ghTyp.CastTo(ref prop2d);
          mem.Property = prop2d;
        }
        else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both))
            mem.Property = new GsaProp2d(id);
          else
            this.AddRuntimeError(
              "Unable to convert PA input to a 2D Property of reference integer");
        }
      }

      double meshSize = 0;
      if (da.GetData(4, ref meshSize))
        mem.MeshSize = meshSize;

      var ioData = new GH_Boolean();
      if (da.GetData(5, ref ioData))
        if (GH_Convert.ToBoolean(ioData, out bool ioBool, GH_Conversion.Both))
          mem.AutomaticInternalOffset = ioBool;

      mem.AutomaticInternalOffset = _automaticInternalOffset;

      da.SetData(0, new GsaMember2dGoo(mem));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("a6333001-bbe1-49bd-873d-b925833ed159");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateMem2d;
    private bool _automaticInternalOffset;

    public CreateMember2d() : base("Create 2D Member",
      "Mem2D",
      "Create GSA Member 2D",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }
    #endregion

    public override void SetSelected(int i, int j) { }

    public override void InitialiseDropdowns() { }

    public override void CreateAttributes() {
      var internalOffset = new List<List<bool>>() { new List<bool>() { _automaticInternalOffset } };
      m_attributes = new OasysGH.UI.CheckBoxComponentComponentAttributes(this, SetReleases, new List<string>() { "Automatic offset" }, internalOffset, new List<List<string>>() { new List<string>() { "Internal" } });
    }

    public void SetReleases(List<List<bool>> internalAutomaticOffset) {
      _automaticInternalOffset = internalAutomaticOffset[0][0];

      base.UpdateUI();
    }

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddBrepParameter("Brep",
        "B",
        "Planar Brep (non-planar geometry will be automatically converted to an average plane of exterior boundary control points))",
        GH_ParamAccess.item);
      pManager.AddPointParameter("Incl. Points",
        "(P)",
        "Inclusion points (will automatically be projected onto Brep)",
        GH_ParamAccess.list);
      pManager.AddCurveParameter("Incl. Curves",
        "(C)",
        "Inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines)",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter());
      pManager.AddNumberParameter("Mesh Size in model units",
        "Ms",
        "Target mesh size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Internal Offset",
        "Io",
        "Set Automatic Internal Offset of Member",
        GH_ParamAccess.item);

      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaMember2dParameter());

    #endregion

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("automaticInternalOffset", _automaticInternalOffset);
      return base.Write(writer);
    }

    public override bool Read(GH_IReader reader) {
      _automaticInternalOffset = reader.GetBoolean("automaticInternalOffset");
      return base.Read(reader);
    }
  }
}
