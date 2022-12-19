using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CreateFaceLoads : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("c4ad7a1e-350b-48b2-b636-24b6ef7bd0f3");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.FaceLoad;

    public CreateFaceLoads() : base("Create Face Load",
      "FaceLoad",
      "Create GSA Face Load",
      CategoryName.Name(),
      SubCategoryName.Cat3())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Pressure.GetAbbreviation(this.ForcePerAreaUnit);

      pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
      pManager.AddGenericParameter("Element list", "G2D", "Property, 2D Elements or 2D Members to apply load to; either input Prop2d, Element2d, or Member2d, or a text string." + Environment.NewLine +
          "Text string with Element list should take the form:" + Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item);
      pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Axis", "Ax", "Load axis (default Local). " +
              Environment.NewLine + "Accepted inputs are:" +
              Environment.NewLine + "0 : Global" +
              Environment.NewLine + "-1 : Local", GH_ParamAccess.item, -1);
      pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
              Environment.NewLine + "Accepted inputs are:" +
              Environment.NewLine + "x" +
              Environment.NewLine + "y" +
              Environment.NewLine + "z", GH_ParamAccess.item, "z");
      pManager.AddBooleanParameter("Projected", "Pj", "Projected (default not)", GH_ParamAccess.item, false);
      pManager.AddNumberParameter("Value [" + unitAbbreviation + "]", "V", "Load Value", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaLoadParameter(), "Face Load", "Ld", "GSA Face Load", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaFaceLoad faceLoad = new GsaFaceLoad();

      // 0 Load case
      int lc = 1;
      GH_Integer gh_lc = new GH_Integer();
      if (DA.GetData(0, ref gh_lc))
        GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
      faceLoad.FaceLoad.Case = lc;

      // element/member list
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(1, ref gh_typ))
      {
        if (gh_typ.Value is GsaElement2dGoo)
        {
          GsaElement2dGoo goo = (GsaElement2dGoo)gh_typ.Value;
          faceLoad.RefObjectGuid = goo.Value.Guid;
          faceLoad.ReferenceType = ReferenceType.Element;
        }
        else if (gh_typ.Value is GsaMember2dGoo)
        {
          GsaMember2dGoo goo = (GsaMember2dGoo)gh_typ.Value;
          faceLoad.RefObjectGuid = goo.Value.Guid;
          faceLoad.ReferenceType = ReferenceType.Member;
          if (_mode != FoldMode.Uniform)
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member loading will not automatically redistribute non-linear loading to child elements. Any non-uniform loading made from Members is likely not what you are after. Please check the load in GSA.");
          else
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Member loading in GsaGH will automatically find child elements created from parent member with the load still being applied to elements. If you save the file and continue working in GSA please note that the member-loading relationship will be lost.");
        }
        else if (gh_typ.Value is GsaProp2dGoo)
        {
          GsaProp2dGoo goo = (GsaProp2dGoo)gh_typ.Value;
          faceLoad.RefObjectGuid = goo.Value.Guid;
          faceLoad.ReferenceType = ReferenceType.Prop2d;
        }
        else if (GH_Convert.ToString(gh_typ.Value, out string elemList, GH_Conversion.Both))
          faceLoad.FaceLoad.Elements = elemList;
      }

      // 2 Name
      string name = "";
      GH_String gh_name = new GH_String();
      if (DA.GetData(2, ref gh_name))
      {
        if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
          faceLoad.FaceLoad.Name = name;
      }

      // 3 axis
      int axis = -1;
      faceLoad.FaceLoad.AxisProperty = 0; //Note there is currently a bug/undocumented in GsaAPI that cannot translate an integer into axis type (Global, Local or edformed local)
      GH_Integer gh_ax = new GH_Integer();
      if (DA.GetData(3, ref gh_ax))
      {
        GH_Convert.ToInt32(gh_ax, out axis, GH_Conversion.Both);
        if (axis == 0 || axis == -1)
          faceLoad.FaceLoad.AxisProperty = axis;
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

      faceLoad.FaceLoad.Direction = direc;

      switch (_mode)
      {
        case FoldMode.Uniform:
          if (_mode == FoldMode.Uniform)
          {
            faceLoad.FaceLoad.Type = FaceLoadType.CONSTANT;

            //projection
            bool prj = false;
            GH_Boolean gh_prj = new GH_Boolean();
            if (DA.GetData(5, ref gh_prj))
              GH_Convert.ToBoolean(gh_prj, out prj, GH_Conversion.Both);
            faceLoad.FaceLoad.IsProjected = prj;

            Pressure load1 = (Pressure)Input.UnitNumber(this, DA, 6, ForcePerAreaUnit);

            // set position and value
            faceLoad.FaceLoad.SetValue(0, load1.NewtonsPerSquareMeter);
          }
          break;

        case FoldMode.Variable:
          if (_mode == FoldMode.Variable)
          {
            faceLoad.FaceLoad.Type = FaceLoadType.GENERAL;

            //projection
            bool prj = false;
            GH_Boolean gh_prj = new GH_Boolean();
            if (DA.GetData(5, ref gh_prj))
              GH_Convert.ToBoolean(gh_prj, out prj, GH_Conversion.Both);
            faceLoad.FaceLoad.IsProjected = prj;

            // set value
            faceLoad.FaceLoad.SetValue(0, ((Pressure)Input.UnitNumber(this, DA, 6, ForcePerAreaUnit, true)).NewtonsPerSquareMeter);
            faceLoad.FaceLoad.SetValue(1, ((Pressure)Input.UnitNumber(this, DA, 7, ForcePerAreaUnit, true)).NewtonsPerSquareMeter);
            faceLoad.FaceLoad.SetValue(2, ((Pressure)Input.UnitNumber(this, DA, 8, ForcePerAreaUnit, true)).NewtonsPerSquareMeter);
            faceLoad.FaceLoad.SetValue(3, ((Pressure)Input.UnitNumber(this, DA, 9, ForcePerAreaUnit, true)).NewtonsPerSquareMeter);
          }
          break;

        case FoldMode.Point:
          if (_mode == FoldMode.Point)
          {
            faceLoad.FaceLoad.Type = FaceLoadType.POINT;

            //projection
            bool prj = false;
            GH_Boolean gh_prj = new GH_Boolean();
            if (DA.GetData(5, ref gh_prj))
              GH_Convert.ToBoolean(gh_prj, out prj, GH_Conversion.Both);
            faceLoad.FaceLoad.IsProjected = prj;

            double r = 0;
            DA.GetData(7, ref r);

            double s = 0;
            DA.GetData(8, ref s);

            // set position and value
            faceLoad.FaceLoad.SetValue(0, ((Pressure)Input.UnitNumber(this, DA, 6, ForcePerAreaUnit)).NewtonsPerSquareMeter);
            //faceLoad.Position.X = r; //note Vector2 currently only get in GsaAPI
            //faceLoad.Position.Y = s;
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Warning: the position cannot be set in GsaAPI at the moment");
          }
          break;

        case FoldMode.Edge:
          if (_mode == FoldMode.Edge)
          {
            //faceLoad.Type = BeamLoadType.EDGE; GsaAPI implementation missing

            // get data
            int edge = 1;
            DA.GetData(5, ref edge);

            faceLoad.FaceLoad.SetValue(0, ((Pressure)Input.UnitNumber(this, DA, 6, ForcePerAreaUnit)).NewtonsPerSquareMeter);
            if (this.Params.Input[7].SourceCount != 0)
              faceLoad.FaceLoad.SetValue(1, ((Pressure)Input.UnitNumber(this, DA, 7, ForcePerAreaUnit)).NewtonsPerSquareMeter);
            else
              faceLoad.FaceLoad.SetValue(1, faceLoad.FaceLoad.Value(0));

            //faceLoad.FaceLoad. = edge; //note implementation of edge-load is not yet supported in GsaAPI

            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Warning: edge-load is not yet supported in GsaAPI");
          }
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }

      GsaLoad gsaLoad = new GsaLoad(faceLoad);
      DA.SetData(0, new GsaLoadGoo(gsaLoad));
    }

    #region Custom UI
    private enum FoldMode
    {
      Uniform,
      Variable,
      Point,
      Edge //note implementation of edge-load is not yet supported in GsaAPI
    }

    readonly List<string> _loadTypeOptions = new List<string>(new string[]
    {
      "Uniform",
      "Variable",
      "Point",
      //"Edge" note implementation of edge-load is not yet supported in GsaAPI
    });

    private FoldMode _mode = FoldMode.Uniform;
    PressureUnit ForcePerAreaUnit = DefaultUnits.ForcePerAreaUnit;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Type", "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Type
      this.DropDownItems.Add(_loadTypeOptions);
      this.SelectedItems.Add(_mode.ToString());

      // ForcePerArea
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.ForcePerArea)));
      this.SelectedItems.Add(Pressure.GetAbbreviation((this.ForcePerAreaUnit)));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) // change is made to the first dropdown list
      {
        switch (SelectedItems[0])
        {
          case "Uniform":
            Mode1Clicked();
            break;
          case "Variable":
            Mode2Clicked();
            break;
          case "Point":
            Mode3Clicked();
            break;
          case "Edge":
            Mode4Clicked();
            break;
        }
      }
      else
        this.ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), this.SelectedItems[1]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this._mode = (FoldMode)Enum.Parse(typeof(FoldMode), this.SelectedItems[0]);
      this.DuringLoad = true;
      switch (SelectedItems[0])
      {
        case "Uniform":
          Mode1Clicked();
          break;
        case "Variable":
          Mode2Clicked();
          break;
        case "Point":
          Mode3Clicked();
          break;
        case "Edge":
          Mode4Clicked();
          break;
      }
      this.DuringLoad = false;
      this.ForcePerAreaUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), this.SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Pressure.GetAbbreviation(this.ForcePerAreaUnit);

      if (_mode == FoldMode.Uniform)
      {
        Params.Input[5].NickName = "Pj";
        Params.Input[5].Name = "Projected";
        Params.Input[5].Description = "Projected (default not)";
        Params.Input[5].Access = GH_ParamAccess.item;
        Params.Input[5].Optional = true;

        Params.Input[6].NickName = "V";
        Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
        Params.Input[6].Description = "Load Value";
        Params.Input[6].Access = GH_ParamAccess.item;
        Params.Input[6].Optional = false;
      }

      if (_mode == FoldMode.Variable)
      {
        Params.Input[5].NickName = "Pj";
        Params.Input[5].Name = "Projected";
        Params.Input[5].Description = "Projected (default not)";
        Params.Input[5].Access = GH_ParamAccess.item;
        Params.Input[5].Optional = true;

        Params.Input[6].NickName = "V1";
        Params.Input[6].Name = "Value 1 [" + unitAbbreviation + "]";
        Params.Input[6].Description = "Load Value Corner 1";
        Params.Input[6].Access = GH_ParamAccess.item;
        Params.Input[6].Optional = true;

        Params.Input[7].NickName = "V2";
        Params.Input[7].Name = "Value 2 [" + unitAbbreviation + "]";
        Params.Input[7].Description = "Load Value Corner 2";
        Params.Input[7].Access = GH_ParamAccess.item;
        Params.Input[7].Optional = true;

        Params.Input[8].NickName = "V3";
        Params.Input[8].Name = "Value 3 [" + unitAbbreviation + "]";
        Params.Input[8].Description = "Load Value Corner 3";
        Params.Input[8].Access = GH_ParamAccess.item;
        Params.Input[8].Optional = true;

        Params.Input[9].NickName = "V4";
        Params.Input[9].Name = "Value 4 [" + unitAbbreviation + "]";
        Params.Input[9].Description = "Load Value Corner 4";
        Params.Input[9].Access = GH_ParamAccess.item;
        Params.Input[9].Optional = true;
      }

      if (_mode == FoldMode.Point)
      {
        Params.Input[5].NickName = "Pj";
        Params.Input[5].Name = "Projected";
        Params.Input[5].Description = "Projected (default not)";
        Params.Input[5].Access = GH_ParamAccess.item;
        Params.Input[5].Optional = true;

        Params.Input[6].NickName = "V";
        Params.Input[6].Name = "Value [" + unitAbbreviation + "]";
        Params.Input[6].Description = "Load Value Corner 1";
        Params.Input[6].Access = GH_ParamAccess.item;
        Params.Input[6].Optional = false;

        Params.Input[7].NickName = "r";
        Params.Input[7].Name = "Position r";
        Params.Input[7].Description = "The position r of the point load to be specified in ( r , s )" +
            Environment.NewLine + "coordinates based on two-dimensional shape function." +
            Environment.NewLine + " • Coordinates vary from −1 to 1 for Quad 4 and Quad 8." +
            Environment.NewLine + " • Coordinates vary from 0 to 1 for Triangle 3 and Triangle 6";
        Params.Input[7].Access = GH_ParamAccess.item;
        Params.Input[7].Optional = true;

        Params.Input[8].NickName = "s";
        Params.Input[8].Name = "Position s";
        Params.Input[8].Description = "The position s of the point load to be specified in ( r , s )" +
            Environment.NewLine + "coordinates based on two-dimensional shape function." +
            Environment.NewLine + " • Coordinates vary from −1 to 1 for Quad 4 and Quad 8." +
            Environment.NewLine + " • Coordinates vary from 0 to 1 for Triangle 3 and Triangle 6";
        Params.Input[8].Access = GH_ParamAccess.item;
        Params.Input[8].Optional = true;
      }

      if (_mode == FoldMode.Edge)
      {
        Params.Input[5].NickName = "Ed";
        Params.Input[5].Name = "Edge";
        Params.Input[5].Description = "Edge (1, 2, 3 or 4)";
        Params.Input[5].Access = GH_ParamAccess.item;
        Params.Input[5].Optional = false;

        Params.Input[6].NickName = "V1";
        Params.Input[6].Name = "Value 1 [" + unitAbbreviation + "]";
        Params.Input[6].Description = "Load Value Corner 1";
        Params.Input[6].Access = GH_ParamAccess.item;
        Params.Input[6].Optional = false;

        Params.Input[7].NickName = "V2";
        Params.Input[7].Name = "Value 2 [" + unitAbbreviation + "]";
        Params.Input[7].Description = "Load Value Corner 2";
        Params.Input[7].Access = GH_ParamAccess.item;
        Params.Input[7].Optional = false;
      }
    }
    #endregion

    #region menu override
    bool DuringLoad = false;
    private void Mode1Clicked()
    {
      if (!this.DuringLoad && _mode == FoldMode.Uniform)
        return;

      RecordUndoEvent("Uniform Parameters");
      _mode = FoldMode.Uniform;

      //remove input parameters
      if (_mode == FoldMode.Edge)
      {
        while (Params.Input.Count > 5)
          Params.UnregisterInputParameter(Params.Input[5], true);
        //add input parameters
        Params.RegisterInputParam(new Param_Boolean());
        Params.RegisterInputParam(new Param_Number());
      }
      else
      {
        while (Params.Input.Count > 6)
          Params.UnregisterInputParameter(Params.Input[6], true);
        //add input parameters
        Params.RegisterInputParam(new Param_Number());
      }
    }
    private void Mode2Clicked()
    {
      if (!this.DuringLoad && _mode == FoldMode.Variable)
        return;

      RecordUndoEvent("Variable Parameters");
      _mode = FoldMode.Variable;

      //remove input parameters
      if (_mode == FoldMode.Edge)
      {
        while (Params.Input.Count > 5)
          Params.UnregisterInputParameter(Params.Input[5], true);
        //add input parameters
        Params.RegisterInputParam(new Param_Boolean());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
      }
      else
      {
        while (Params.Input.Count > 6)
          Params.UnregisterInputParameter(Params.Input[6], true);
        //add input parameters
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
      }
    }

    private void Mode3Clicked()
    {
      if (!this.DuringLoad && _mode == FoldMode.Point)
        return;

      RecordUndoEvent("Point Parameters");
      _mode = FoldMode.Point;

      //remove input parameters
      if (_mode == FoldMode.Edge)
      {
        while (Params.Input.Count > 5)
          Params.UnregisterInputParameter(Params.Input[5], true);
        //add input parameters
        Params.RegisterInputParam(new Param_Boolean());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
      }
      else
      {
        while (Params.Input.Count > 6)
          Params.UnregisterInputParameter(Params.Input[6], true);
        //add input parameters
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
        Params.RegisterInputParam(new Param_Number());
      }
    }
    private void Mode4Clicked()
    {
      if (!this.DuringLoad && _mode == FoldMode.Edge)
        return;

      RecordUndoEvent("Edge Parameters");
      _mode = FoldMode.Edge;

      //remove input parameters
      while (Params.Input.Count > 5)
        Params.UnregisterInputParameter(Params.Input[5], true);

      //add input parameters
      Params.RegisterInputParam(new Param_Number());
      Params.RegisterInputParam(new Param_Number());
      Params.RegisterInputParam(new Param_Number());
    }
    #endregion
  }
}
