using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create new 1D Member
  /// </summary>
  public class CreateMember1d : GH_OasysComponent, IGH_PreviewObject, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("5c5b9efa-cdae-4be5-af40-ff2b590801dd");
    public CreateMember1d()
      : base("Create 1D Member", "Mem1D", "Create GSA 1D Member",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat2())
    {
    }

    public override GH_Exposure Exposure => GH_Exposure.primary;

    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateMem1d;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      m_attributes = new UI.ReleasesComponentUI(this, SetReleases, "Start Release", "End Release", x1, y1, z1, xx1, yy1, zz1, x2, y2, z2, xx2, yy2, zz2);
    }

    public void SetReleases(bool resx1, bool resy1, bool resz1, bool resxx1, bool resyy1, bool reszz1,
        bool resx2, bool resy2, bool resz2, bool resxx2, bool resyy2, bool reszz2)
    {
      x1 = resx1;
      y1 = resy1;
      z1 = resz1;
      xx1 = resxx1;
      yy1 = resyy1;
      zz1 = reszz1;
      x2 = resx2;
      y2 = resy2;
      z2 = resz2;
      xx2 = resxx2;
      yy2 = resyy2;
      zz2 = reszz2;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    #endregion

    #region Input and output
    bool x1;
    bool y1;
    bool z1;
    bool xx1;
    bool yy1;
    bool zz1;
    bool x2;
    bool y2;
    bool z2;
    bool xx2;
    bool yy2;
    bool zz2;

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      // we need to save all the items that we want to reappear when a GH file is saved and re-opened
      writer.SetBoolean("x1", (bool)x1);
      writer.SetBoolean("y1", (bool)y1);
      writer.SetBoolean("z1", (bool)z1);
      writer.SetBoolean("xx1", (bool)xx1);
      writer.SetBoolean("yy1", (bool)yy1);
      writer.SetBoolean("zz1", (bool)zz1);
      writer.SetBoolean("x2", (bool)x2);
      writer.SetBoolean("y2", (bool)y2);
      writer.SetBoolean("z2", (bool)z2);
      writer.SetBoolean("xx2", (bool)xx2);
      writer.SetBoolean("yy2", (bool)yy2);
      writer.SetBoolean("zz2", (bool)zz2);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      // when a GH file is opened we need to read in the data that was previously set by user
      x1 = (bool)reader.GetBoolean("x1");
      y1 = (bool)reader.GetBoolean("y1");
      z1 = (bool)reader.GetBoolean("z1");
      xx1 = (bool)reader.GetBoolean("xx1");
      yy1 = (bool)reader.GetBoolean("yy1");
      zz1 = (bool)reader.GetBoolean("zz1");
      x2 = (bool)reader.GetBoolean("x2");
      y2 = (bool)reader.GetBoolean("y2");
      z2 = (bool)reader.GetBoolean("z2");
      xx2 = (bool)reader.GetBoolean("xx2");
      yy2 = (bool)reader.GetBoolean("yy2");
      zz2 = (bool)reader.GetBoolean("zz2");
      // we need to recreate the custom UI again as this is created before this read IO is called
      // otherwise the component will not display the selected item on the canvas
      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);

      return base.Read(reader);
    }
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddCurveParameter("Curve", "C", "Curve (NURBS curve will be converted to a Polyline)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Section", "PB", "GSA Section Property. Input either a GSA Section or an Integer to use a Section already defined in model", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("1D Member", "M1D", "GSA 1D Member", GH_ParamAccess.item);

    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Curve ghcrv = new GH_Curve();
      if (DA.GetData(0, ref ghcrv))
      {
        if (ghcrv == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Curve input is null"); }
        Curve crv = null;
        if (GH_Convert.ToCurve(ghcrv, ref crv, GH_Conversion.Both))
        {
          GsaMember1d mem = new GsaMember1d(crv);
          if (mem.PolyCurve.GetLength() < Units.Tolerance.As(Units.LengthUnitGeometry))
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "One or more input curves have relatively short length and may convert into a zero-length line in GSA thus creating invalid topology that cannot be analysed.");

          GsaBool6 rel1 = new GsaBool6
          {
            X = x1,
            Y = y1,
            Z = z1,
            XX = xx1,
            YY = yy1,
            ZZ = zz1
          };

          mem.ReleaseStart = rel1;

          GsaBool6 rel2 = new GsaBool6
          {
            X = x2,
            Y = y2,
            Z = z2,
            XX = xx2,
            YY = yy2,
            ZZ = zz2
          };
          mem.ReleaseEnd = rel2;

          // 1 section
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          GsaSection section = new GsaSection();
          if (DA.GetData(1, ref gh_typ))
          {
            if (gh_typ.Value is GsaSectionGoo)
            {
              gh_typ.CastTo(ref section);
              mem.Section = section;
            }
            else
            {
              if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                mem.Section = new GsaSection(idd);
              else
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
                return;
              }
            }
          }

          DA.SetData(0, new GsaMember1dGoo(mem));
        }
      }
    }
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

    }
    #endregion

  }
}

