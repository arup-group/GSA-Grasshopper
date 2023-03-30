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
using OasysGH.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  ///   Component to create new 1D Member
  /// </summary>
  public class CreateMember1d : GH_OasysDropDownComponent
  {
    protected override void SolveInstance(IGH_DataAccess da)
    {
      var ghcrv = new GH_Curve();
      if (!da.GetData(0, ref ghcrv))
        return;

      if (ghcrv == null)
        this.AddRuntimeWarning("Curve input is null");
      Curve crv = null;
      if (!GH_Convert.ToCurve(ghcrv, ref crv, GH_Conversion.Both))
        return;

      var mem = new GsaMember1d(crv);
      if (mem.PolyCurve.GetLength() < DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry))
        this.AddRuntimeRemark(
          "Service message from you favourite Oasys dev team: Based on your Default Unit Settings (changed in the Oasys Menu), one or more input curves have relatively short length less than the set tolerance ("
          + DefaultUnits.Tolerance.ToString()
            .Replace(" ", string.Empty)
          + ". This may convert into a zero-length line when assembling the GSA Model, thus creating invalid topology that cannot be analysed. You can ignore this message if you are creating your model in another unit (set on 'Analyse' or 'CreateModel' components) than "
          + DefaultUnits.LengthUnitGeometry.ToString()
          + ".");

      var rel1 = new GsaBool6
      {
        X = _restraints,
        Y = _y1,
        Z = _z1,
        Xx = _xx1,
        Yy = _yy1,
        Zz = _zz1,
      };

      mem.ReleaseStart = rel1;

      var rel2 = new GsaBool6
      {
        X = _x2,
        Y = _y2,
        Z = _z2,
        Xx = _xx2,
        Yy = _yy2,
        Zz = _zz2,
      };
      mem.ReleaseEnd = rel2;

      var ghTyp = new GH_ObjectWrapper();
      var section = new GsaSection();
      if (da.GetData(1, ref ghTyp))
      {
        if (ghTyp.Value is GsaSectionGoo)
        {
          ghTyp.CastTo(ref section);
          mem.Section = section;
        }
        else
        {
          if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both))
            mem.Section = new GsaSection(id);
          else
            this.AddRuntimeError(
              "Unable to convert PB input to a Section Property of reference integer");
        }
      }

      double meshSize = 0;
      if (da.GetData(2, ref meshSize))
        mem.MeshSize = meshSize;

      da.SetData(0, new GsaMember1dGoo(mem));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("8278b67c-425a-4220-b759-79ecdd6aba55");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateMem1d;

    public CreateMember1d() : base("Create 1D Member",
      "Mem1D",
      "Create GSA 1D Member",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddCurveParameter("Curve",
        "C",
        "Curve (a NURBS curve will automatically be converted in to a Polyline of Arc and Line segments)",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter());
      pManager.AddNumberParameter("Mesh Size in model units",
        "Ms",
        "Target mesh size",
        GH_ParamAccess.item);
      pManager[1]
        .Optional = true;
      pManager[2]
        .Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaMember1dParameter());

    #endregion

    #region Custom UI
    private List<bool> _startRestraints;
    private List<bool> _endRestraints;

    public override void SetSelected(int i, int j) { }

    public override void InitialiseDropdowns() { }

    public override void CreateAttributes() => m_attributes = new OasysGH.UI.Foo.ReleasesComponentAttributes(this, SetReleases, "Start Release", "End Release", _restraints);

    public void SetReleases(List<bool> startRestraints, List<bool> endRestraints)
    {
      _restraints = restraints;
      base.UpdateUI();
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IWriter writer)
    {
      writer.SetBoolean("x1", _restraints[0]);
      writer.SetBoolean("y1", _restraints[1]);
      writer.SetBoolean("z1", _restraints[2]);
      writer.SetBoolean("xx1", _restraints[3]);
      writer.SetBoolean("yy1", _restraints[4]);
      writer.SetBoolean("zz1", _restraints[5]);
      writer.SetBoolean("x2", _restraints[6]);
      writer.SetBoolean("y2", _restraints[7]);
      writer.SetBoolean("z2", _restraints[8]);
      writer.SetBoolean("xx2", _restraints[9]);
      writer.SetBoolean("yy2", _restraints[10]);
      writer.SetBoolean("zz2", _restraints[11]);
      return base.Write(writer);
    }

    public override bool Read(GH_IReader reader)
    {
      _restraints[0] = reader.GetBoolean("x1");
      _restraints[1] = reader.GetBoolean("y1");
      _restraints[2] = reader.GetBoolean("z1");
      _restraints[3] = reader.GetBoolean("xx1");
      _restraints[4] = reader.GetBoolean("yy1");
      _restraints[5] = reader.GetBoolean("zz1");
      _restraints[6] = reader.GetBoolean("x2");
      _restraints[7] = reader.GetBoolean("y2");
      _restraints[8] = reader.GetBoolean("z2");
      _restraints[9] = reader.GetBoolean("xx2");
      _restraints[10] = reader.GetBoolean("yy2");
      _restraints[11] = reader.GetBoolean("zz2");
      return base.Read(reader);
    }
    #endregion
  }
}
