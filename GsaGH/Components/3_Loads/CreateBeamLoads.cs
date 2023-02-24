using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  public class CreateBeamLoads : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.BeamLoad;

    public CreateBeamLoads() : base("Create Beam Load",
      "BeamLoad",
      "Create GSA Beam Load",
      CategoryName.Name(),
      SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override Guid ComponentGuid => new Guid("e034b346-a6e8-4dd1-b12c-6104baa2586e");
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit);

      pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
      pManager.AddGenericParameter("Element list", "G1D", "Section, 1D Elements or 1D Members to apply load to; either input Section, Element1d, or Member1d, or a text string." + Environment.NewLine +
          "Text string with Element list should take the form:" + Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax", "Load axis (default Global). " +
              Environment.NewLine + "Accepted inputs are:" +
              Environment.NewLine + "0 : Global" +
              Environment.NewLine + "-1 : Local", GH_ParamAccess.item, 0);
      pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
              Environment.NewLine + "Accepted inputs are:" +
              Environment.NewLine + "x" +
              Environment.NewLine + "y" +
              Environment.NewLine + "z" +
              Environment.NewLine + "xx" +
              Environment.NewLine + "yy" +
              Environment.NewLine + "zz", GH_ParamAccess.item, "z");
      pManager.AddBooleanParameter("Projected", "Pj", "Projected (default not)", GH_ParamAccess.item, false);
      pManager.AddNumberParameter("Value [" + unitAbbreviation + "]", "V", "Load Value", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;

      _mode = FoldMode.Uniform;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaLoadParameter(), "Beam Load", "Ld", "GSA Beam Load", GH_ParamAccess.item);
    }
    #endregion
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaBeamLoad beamLoad = new GsaBeamLoad();

      // 0 Load case
      int lc = 1;
      GH_Integer gh_lc = new GH_Integer();
      if (DA.GetData(0, ref gh_lc))
        GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
      beamLoad.BeamLoad.Case = lc;

      // 1 element/beam list
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(1, ref gh_typ))
      {
        if (gh_typ.Value is GsaElement1dGoo)
        {
          GsaElement1dGoo goo = (GsaElement1dGoo)gh_typ.Value;
          beamLoad.RefObjectGuid = goo.Value.Guid;
          beamLoad.ReferenceType = ReferenceType.Element;
        }
        else if (gh_typ.Value is GsaMember1dGoo)
        {
          GsaMember1dGoo goo = (GsaMember1dGoo)gh_typ.Value;
          beamLoad.RefObjectGuid = goo.Value.Guid;
          beamLoad.ReferenceType = ReferenceType.Member;
          if (_mode != FoldMode.Uniform)
            this.AddRuntimeWarning("Member loading will not automatically redistribute non-linear loading to child elements. Any non-uniform loading made from Members is likely not what you are after. Please check the load in GSA.");
          else
            this.AddRuntimeRemarkMsg("Member loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements. If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
        }
        else if (gh_typ.Value is GsaSectionGoo)
        {
          GsaSectionGoo goo = (GsaSectionGoo)gh_typ.Value;
          beamLoad.RefObjectGuid = goo.Value.Guid;
          beamLoad.ReferenceType = ReferenceType.Section;
        }
        else if (GH_Convert.ToString(gh_typ.Value, out string beamList, GH_Conversion.Both))
          beamLoad.BeamLoad.Elements = beamList;
      }

      // 2 Name
      string name = "";
      GH_String gh_name = new GH_String();
      if (DA.GetData(2, ref gh_name))
      {
        if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
          beamLoad.BeamLoad.Name = name;
      }

      // 3 axis
      int axis = 0;
      beamLoad.BeamLoad.AxisProperty = 0; //Note there is currently a bug/undocumented in GsaAPI that cannot translate an integer into axis type (Global, Local or edformed local)
      GH_Integer gh_ax = new GH_Integer();
      if (DA.GetData(3, ref gh_ax))
      {
        GH_Convert.ToInt32(gh_ax, out axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1)
          beamLoad.BeamLoad.AxisProperty = axis;
      }

      // 4 direction
      string dir = "Z";
      Direction direc = Direction.Z;

      GH_String gh_dir = new GH_String();
      if (DA.GetData(4, ref gh_dir))
        GH_Convert.ToString(gh_dir, out dir, GH_Conversion.Both);
      dir = dir.ToUpper().Trim();
      if (dir == "X")
        direc = Direction.X;
      if (dir == "Y")
        direc = Direction.Y;
      if (dir == "XX")
        direc = Direction.XX;
      if (dir == "YY")
        direc = Direction.YY;
      if (dir == "ZZ")
        direc = Direction.ZZ;

      beamLoad.BeamLoad.Direction = direc;

      // 5 projection
      bool prj = false;
      GH_Boolean gh_prj = new GH_Boolean();
      if (DA.GetData(5, ref gh_prj))
        GH_Convert.ToBoolean(gh_prj, out prj, GH_Conversion.Both);
      beamLoad.BeamLoad.IsProjected = prj;

      // 6 value (1)
      ForcePerLength load1 = (ForcePerLength)Input.UnitNumber(this, DA, 6, ForcePerLengthUnit);

      switch (_mode)
      {
        case FoldMode.Point:
          if (_mode == FoldMode.Point)
          {
            beamLoad.BeamLoad.Type = BeamLoadType.POINT;

            // 7 pos (1)
            double pos = 0;
            if (DA.GetData(7, ref pos))
              pos *= -1;

            // set position and value
            beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
            beamLoad.BeamLoad.SetPosition(0, pos);
          }
          break;

        case FoldMode.Uniform:
          if (_mode == FoldMode.Uniform)
          {
            beamLoad.BeamLoad.Type = BeamLoadType.UNIFORM;
            // set value
            beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
          }
          break;

        case FoldMode.Linear:
          if (_mode == FoldMode.Linear)
          {
            beamLoad.BeamLoad.Type = BeamLoadType.LINEAR;

            // 7 value (2)
            ForcePerLength load2 = (ForcePerLength)Input.UnitNumber(this, DA, 7, ForcePerLengthUnit);

            // set value
            beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
            beamLoad.BeamLoad.SetValue(1, load2.NewtonsPerMeter);
          }
          break;

        case FoldMode.Patch:
          if (_mode == FoldMode.Patch)
          {
            beamLoad.BeamLoad.Type = BeamLoadType.PATCH;

            // 7 pos (1)
            double pos1 = 0;
            if (DA.GetData(7, ref pos1))
              pos1 *= -1;

            // 9 pos (2)
            double pos2 = 1;
            if (DA.GetData(9, ref pos2))
              pos2 *= -1;

            // 8 value (2)
            ForcePerLength load2 = (ForcePerLength)Input.UnitNumber(this, DA, 8, ForcePerLengthUnit);

            // set value
            beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
            beamLoad.BeamLoad.SetValue(1, load2.NewtonsPerMeter);
            beamLoad.BeamLoad.SetPosition(0, pos1);
            beamLoad.BeamLoad.SetPosition(1, pos2);
          }
          break;

        case FoldMode.Trilinear:
          if (_mode == FoldMode.Trilinear)
          {
            beamLoad.BeamLoad.Type = BeamLoadType.TRILINEAR;

            // 7 pos (1)
            double pos1 = 0;
            if (DA.GetData(7, ref pos1))
              pos1 *= -1;

            // 9 pos (2)
            double pos2 = 1;
            if (DA.GetData(9, ref pos2))
              pos2 *= -1;

            // 8 value (2)
            ForcePerLength load2 = (ForcePerLength)Input.UnitNumber(this, DA, 8, ForcePerLengthUnit);

            // set value
            beamLoad.BeamLoad.SetValue(0, load1.NewtonsPerMeter);
            beamLoad.BeamLoad.SetValue(1, load2.NewtonsPerMeter);
            beamLoad.BeamLoad.SetPosition(0, pos1);
            beamLoad.BeamLoad.SetPosition(1, pos2);
          }
          break;
      }

      GsaLoad gsaLoad = new GsaLoad(beamLoad);
      DA.SetData(0, new GsaLoadGoo(gsaLoad));
    }

    #region Custom UI
    private enum FoldMode
    {
      Point,
      Uniform,
      Linear,
      Patch,
      Trilinear
    }

    readonly List<string> _loadTypeOptions = new List<string>(new string[]
    {
      "Point",
      "Uniform",
      "Linear",
      "Patch",
      "Trilinear"
    });

    private FoldMode _mode = FoldMode.Uniform;
    private ForcePerLengthUnit ForcePerLengthUnit = DefaultUnits.ForcePerLengthUnit;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Type", "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Type
      this.DropDownItems.Add(this._loadTypeOptions);
      this.SelectedItems.Add(this._mode.ToString());

      // ForcePerLength
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength));
      this.SelectedItems.Add(ForcePerLength.GetAbbreviation((this.ForcePerLengthUnit)));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to the first dropdown list
      {
        switch (SelectedItems[0])
        {
          case "Point":
            Mode1Clicked();
            break;
          case "Uniform":
            Mode2Clicked();
            break;
          case "Linear":
            Mode3Clicked();
            break;
          case "Patch":
            Mode4Clicked();
            break;
          case "Trilinear":
            Mode5Clicked();
            break;
        }
      }
      else
        this.ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), this.SelectedItems[1]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this._mode = (FoldMode)Enum.Parse(typeof(FoldMode), this.SelectedItems[0]);
      this.DuringLoad = true;
      switch (SelectedItems[0])
      {
        case "Point":
          Mode1Clicked();
          break;
        case "Uniform":
          Mode2Clicked();
          break;
        case "Linear":
          Mode3Clicked();
          break;
        case "Patch":
          Mode4Clicked();
          break;
        case "Trilinear":
          Mode5Clicked();
          break;
      }
      this.DuringLoad = false;
      this.ForcePerLengthUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), this.SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = ForcePerLength.GetAbbreviation(this.ForcePerLengthUnit);

      if (_mode == FoldMode.Point)
      {
        Params.Input[6].NickName = "V";
        Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
        Params.Input[6].Description = "Load Value";
        Params.Input[6].Access = GH_ParamAccess.item;
        Params.Input[6].Optional = false;

        Params.Input[7].NickName = "t";
        Params.Input[7].Name = "Position (%)";
        Params.Input[7].Description = "Line parameter where point load act (between 0.0 and 1.0)";
        Params.Input[7].Access = GH_ParamAccess.item;
        Params.Input[7].Optional = true;
      }

      if (_mode == FoldMode.Uniform)
      {
        Params.Input[6].NickName = "V";
        Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
        Params.Input[6].Description = "Load Value";
        Params.Input[6].Access = GH_ParamAccess.item;
        Params.Input[6].Optional = false;
      }

      if (_mode == FoldMode.Linear)
      {
        Params.Input[6].NickName = "V1";
        Params.Input[6].Name = "Value Start [" + unitAbbreviation + "]";
        Params.Input[6].Description = "Load Value at Beam Start";
        Params.Input[6].Access = GH_ParamAccess.item;
        Params.Input[6].Optional = true;

        Params.Input[7].NickName = "V2";
        Params.Input[7].Name = "Value End [" + unitAbbreviation + "]";
        Params.Input[7].Description = "Load Value at Beam End";
        Params.Input[7].Access = GH_ParamAccess.item;
        Params.Input[7].Optional = true;
      }

      if (_mode == FoldMode.Patch)
      {
        Params.Input[6].NickName = "V1";
        Params.Input[6].Name = "Load t1 [" + unitAbbreviation + "]";
        Params.Input[6].Description = "Load Value at Position 1";
        Params.Input[6].Access = GH_ParamAccess.item;
        Params.Input[6].Optional = true;

        Params.Input[7].NickName = "t1";
        Params.Input[7].Name = "Position 1 [%]";
        Params.Input[7].Description = "Line parameter where patch load begins (between 0.0 and 1.0, but less than t2)";
        Params.Input[7].Access = GH_ParamAccess.item;
        Params.Input[7].Optional = true;

        Params.Input[8].NickName = "V2";
        Params.Input[8].Name = "Load t2 [" + unitAbbreviation + "]";
        Params.Input[8].Description = "Load Value at Position 2";
        Params.Input[8].Access = GH_ParamAccess.item;
        Params.Input[8].Optional = true;

        Params.Input[9].NickName = "t2";
        Params.Input[9].Name = "Position 2 [%]";
        Params.Input[9].Description = "Line parameter where patch load ends (between 0.0 and 1.0, but bigger than t1)";
        Params.Input[9].Access = GH_ParamAccess.item;
        Params.Input[9].Optional = true;
      }

      if (_mode == FoldMode.Trilinear)
      {
        Params.Input[6].NickName = "V1";
        Params.Input[6].Name = "Load t1 [" + unitAbbreviation + "]";
        Params.Input[6].Description = "Load Value at Position 1";
        Params.Input[6].Access = GH_ParamAccess.item;
        Params.Input[6].Optional = true;

        Params.Input[7].NickName = "t1";
        Params.Input[7].Name = "Position 1 [%]";
        Params.Input[7].Description = "Line parameter where L1 applies (between 0.0 and 1.0, but less than t2)";
        Params.Input[7].Access = GH_ParamAccess.item;
        Params.Input[7].Optional = true;

        Params.Input[8].NickName = "V2";
        Params.Input[8].Name = "Load t2 [" + unitAbbreviation + "]";
        Params.Input[8].Description = "Load Value at Position 2";
        Params.Input[8].Access = GH_ParamAccess.item;
        Params.Input[8].Optional = true;

        Params.Input[9].NickName = "t2";
        Params.Input[9].Name = "Position 2 [%]";
        Params.Input[9].Description = "Line parameter where L2 applies (between 0.0 and 1.0, but bigger than t1)";
        Params.Input[9].Access = GH_ParamAccess.item;
        Params.Input[9].Optional = true;
      }
    }
    #endregion

    #region menu override
    bool DuringLoad = false;
    private void Mode1Clicked()
    {
      if (!this.DuringLoad && _mode == FoldMode.Point)
        return;

      RecordUndoEvent("Point Parameters");
      _mode = FoldMode.Point;

      //remove input parameters
      while (Params.Input.Count > 7)
        Params.UnregisterInputParameter(Params.Input[7], true);
      Params.RegisterInputParam(new Param_GenericObject());
    }
    private void Mode2Clicked()
    {
      if (!this.DuringLoad && _mode == FoldMode.Uniform)
        return;

      RecordUndoEvent("Uniform Parameters");
      _mode = FoldMode.Uniform;

      //remove input parameters
      while (Params.Input.Count > 7)
        Params.UnregisterInputParameter(Params.Input[7], true);
    }
    private void Mode3Clicked()
    {
      if (!this.DuringLoad && _mode == FoldMode.Linear)
        return;

      RecordUndoEvent("Linear Parameters");
      _mode = FoldMode.Linear;

      //remove input parameters
      while (Params.Input.Count > 7)
        Params.UnregisterInputParameter(Params.Input[7], true);

      //add input parameters
      Params.RegisterInputParam(new Param_GenericObject());
    }
    private void Mode4Clicked()
    {
      if (!this.DuringLoad && _mode == FoldMode.Patch)
        return;

      RecordUndoEvent("Patch Parameters");
      _mode = FoldMode.Patch;

      if (_mode != FoldMode.Trilinear)
      {
        //remove input parameters
        while (Params.Input.Count > 7)
          Params.UnregisterInputParameter(Params.Input[7], true);

        //add input parameters
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_Number());
      }
    }
    private void Mode5Clicked()
    {
      if (!this.DuringLoad && _mode == FoldMode.Trilinear)
        return;

      RecordUndoEvent("Trilinear Parameters");
      _mode = FoldMode.Trilinear;

      if (_mode != FoldMode.Patch)
      {
        //remove input parameters
        while (Params.Input.Count > 7)
          Params.UnregisterInputParameter(Params.Input[7], true);

        //add input parameters
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_GenericObject());
        Params.RegisterInputParam(new Param_Number());
      }
    }
    #endregion
  }
}
