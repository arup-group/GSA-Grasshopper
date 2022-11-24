using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to get GSA reaction forces
  /// </summary>
  public class ReactionForce : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("4f06d674-c736-4d9c-89d9-377bc424c547");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ReactionForces;

    public ReactionForce() : base("Reaction Forces",
      "ReacForce",
      "Reaction Force result values",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + System.Environment.NewLine +
          "Node list should take the form:" + System.Environment.NewLine +
          " 1 11 to 72 step 2 not (XY3 31 to 45)" + System.Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string forceunitAbbreviation = Force.GetAbbreviation(this.ForceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(this.MomentUnit);

      string note = System.Environment.NewLine + "DataTree organised as { CaseID ; Permutation } " +
                    System.Environment.NewLine + "fx. {1;2} is Case 1, Permutation 2, where each branch " +
          System.Environment.NewLine + "branch contains a list matching the NodeIDs in the ID output.";

      pManager.AddGenericParameter("Force X [" + forceunitAbbreviation + "]", "Fx", "Reaction Forces in X-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Y [" + forceunitAbbreviation + "]", "Fy", "Reaction Forces in Y-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Z [" + forceunitAbbreviation + "]", "Fz", "Reaction Forces in Z-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force |XYZ| [" + forceunitAbbreviation + "]", "|F|", "Combined |XYZ| Reaction Forces in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment XX [" + momentunitAbbreviation + "]", "Mxx", "Reaction Moments around X-axis in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment YY [" + momentunitAbbreviation + "]", "Myy", "Reaction Moments around Y-axis in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment ZZ [" + momentunitAbbreviation + "]", "Mzz", "Reaction Moments around Z-axis in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment |XYZ| [" + momentunitAbbreviation + "]", "|M|", "Combined |XXYYZZ| Reaction Moments in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddTextParameter("Nodes IDs", "ID", "Node IDs for each result value", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();

      // Get filer case
      string nodeList = "All";
      GH_String gh_Type = new GH_String();
      if (DA.GetData(1, ref gh_Type))
        GH_Convert.ToString(gh_Type, out nodeList, GH_Conversion.Both);

      if (nodeList.ToLower() == "all" || nodeList == "")
        nodeList = "All";

      // data trees to output
      DataTree<GH_UnitNumber> out_transX = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_transY = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_transZ = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_transXYZ = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_rotX = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_rotY = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_rotZ = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_rotXYZ = new DataTree<GH_UnitNumber>();
      DataTree<int> outIDs = new DataTree<int>();

      // Get Model
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(0, gh_types))
      {
        List<GsaResult> results = new List<GsaResult>();

        for (int i = 0; i < gh_types.Count; i++)
        {
          GH_ObjectWrapper gh_typ = gh_types[i];
          if (gh_typ == null || gh_typ.Value == null)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input is null");
            return;
          }
          if (gh_typ.Value is GsaResultGoo)
          {
            result = ((GsaResultGoo)gh_typ.Value).Value;
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
            return;
          }
          Tuple<List<GsaResultsValues>, List<int>> resultgetter = result.NodeReactionForceValues(nodeList, ForceUnit, MomentUnit);
          List<GsaResultsValues> vals = resultgetter.Item1;
          List<int> sortedIDs = resultgetter.Item2;

          List<int> permutations = (result.SelectedPermutationIDs == null ? new List<int>() { 0 } : result.SelectedPermutationIDs);

          for (int index = 0; index < vals.Count; index++)
          {
            GH_Path p = p = new GH_Path(result.CaseID, permutations[index]);

            List<GH_UnitNumber> transX = new List<GH_UnitNumber>();
            List<GH_UnitNumber> transY = new List<GH_UnitNumber>();
            List<GH_UnitNumber> transZ = new List<GH_UnitNumber>();
            List<GH_UnitNumber> transXYZ = new List<GH_UnitNumber>();
            List<GH_UnitNumber> rotX = new List<GH_UnitNumber>();
            List<GH_UnitNumber> rotY = new List<GH_UnitNumber>();
            List<GH_UnitNumber> rotZ = new List<GH_UnitNumber>();
            List<GH_UnitNumber> rotXYZ = new List<GH_UnitNumber>();
            List<int> ids = new List<int>();

            Parallel.For(0, 2, item => // split into two tasks
            {
              if (item == 0)
              {
                for (int j = 0; j < sortedIDs.Count; j++)
                {
                  int ID = sortedIDs[j];
                  ids.Add(ID);
                  ConcurrentDictionary<int, GsaResultQuantity> res = vals[index].xyzResults[ID];
                  GsaResultQuantity values = res[0]; // there is only one result per node
                  transX.Add(new GH_UnitNumber(values.X.ToUnit(ForceUnit))); // use ToUnit to capture changes in dropdown
                  transY.Add(new GH_UnitNumber(values.Y.ToUnit(ForceUnit)));
                  transZ.Add(new GH_UnitNumber(values.Z.ToUnit(ForceUnit)));
                  transXYZ.Add(new GH_UnitNumber(values.XYZ));
                }
              }
              if (item == 1)
              {
                for (int j = 0; j < sortedIDs.Count; j++)
                {
                  int ID = sortedIDs[j];
                  ConcurrentDictionary<int, GsaResultQuantity> res = vals[index].xxyyzzResults[ID];
                  GsaResultQuantity values = res[0]; // there is only one result per node
                  rotX.Add(new GH_UnitNumber(values.X.ToUnit(MomentUnit))); // use ToUnit to capture changes in dropdown
                  rotY.Add(new GH_UnitNumber(values.Y.ToUnit(MomentUnit)));
                  rotZ.Add(new GH_UnitNumber(values.Z.ToUnit(MomentUnit)));
                  rotXYZ.Add(new GH_UnitNumber(values.XYZ));
                }
              }
            });

            out_transX.AddRange(transX, p);
            out_transY.AddRange(transY, p);
            out_transZ.AddRange(transZ, p);
            out_transXYZ.AddRange(transXYZ, p);
            out_rotX.AddRange(rotX, p);
            out_rotY.AddRange(rotY, p);
            out_rotZ.AddRange(rotZ, p);
            out_rotXYZ.AddRange(rotXYZ, p);
            outIDs.AddRange(ids, p);
          }
        }

        DA.SetDataTree(0, out_transX);
        DA.SetDataTree(1, out_transY);
        DA.SetDataTree(2, out_transZ);
        DA.SetDataTree(3, out_transXYZ);
        DA.SetDataTree(4, out_rotX);
        DA.SetDataTree(5, out_rotY);
        DA.SetDataTree(6, out_rotZ);
        DA.SetDataTree(7, out_rotXYZ);
        DA.SetDataTree(8, outIDs);
      }
    }

    #region Custom UI
    private ForceUnit ForceUnit = DefaultUnits.ForceUnit;
    private MomentUnit MomentUnit = DefaultUnits.MomentUnit;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Force Unit", "Moment Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // force
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      this.SelectedItems.Add(Force.GetAbbreviation(this.ForceUnit));

      // moment
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment));
      this.SelectedItems.Add(Moment.GetAbbreviation(this.MomentUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (i == 0)
        this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), this.SelectedItems[i]);
      else if (i == 1)
        this.MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), this.SelectedItems[0]);
      this.MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), this.SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string forceunitAbbreviation = Force.GetAbbreviation(this.ForceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(this.MomentUnit);
      int i = 0;
      Params.Output[i++].Name = "Force X [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Y [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Z [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force |XYZ| [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment XX [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment YY [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment ZZ [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment |XXYYZZ| [" + momentunitAbbreviation + "]";
    }
    #endregion
  }
}

