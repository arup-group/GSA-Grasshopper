using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
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
    /// Component to get GSA node displacement values
    /// </summary>
    public class NodeDisplacement : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("83844063-3da9-4d96-95d3-ea39f96f3e2a");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.NodeDisplacement;

    public NodeDisplacement() : base("Node Displacements",
      "NodeDisp",
      "Node Translation and Rotation result values",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddTextParameter("Node filter list", "No", "Filter results by list." + Environment.NewLine +
          "Node list should take the form:" + Environment.NewLine +
          " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      string note = Environment.NewLine + "DataTree organised as { CaseID ; Permutation } " +
                    Environment.NewLine + "fx. {1;2} is Case 1, Permutation 2, where each branch " +
                      Environment.NewLine + "contains a list matching the NodeIDs in the ID output.";

      pManager.AddGenericParameter("Translations X [" + unitAbbreviation + "]", "Ux", "Translations in X-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Y [" + unitAbbreviation + "]", "Uy", "Translations in Y-direction in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Z [" + unitAbbreviation + "]", "Uz", "Translations in Z-direction in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations |XYZ| [" + unitAbbreviation + "]", "|U|", "Combined |XYZ| Translations in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations XX [rad]", "Rxx", "Rotations around X-axis in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations YY [rad]", "Ryy", "Rotations around Y-axis in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations ZZ [rad]", "Rzz", "Rotations around Z-axis in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations |XYZ| [rad]", "|R|", "Combined |XXYYZZ| Rotations in Global Axis" + note, GH_ParamAccess.tree);
      pManager.AddTextParameter("Nodes IDs", "ID", "Node IDs for each result value", GH_ParamAccess.tree);
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
            this.AddRuntimeWarning("Input is null");
            return;
          }
          if (gh_typ.Value is GsaResultGoo)
          {
            result = ((GsaResultGoo)gh_typ.Value).Value;
          }
          else
          {
            this.AddRuntimeError("Error converting input to GSA Result");
            return;
          }

          Tuple<List<GsaResultsValues>, List<int>> nodedisp = result.NodeDisplacementValues(nodeList, this.LengthUnit);
          List<GsaResultsValues> vals = nodedisp.Item1;
          List<int> sortedIDs = nodedisp.Item2;

          List<int> permutations = (result.SelectedPermutationIDs == null ? new List<int>() { 1 } : result.SelectedPermutationIDs);
          if (permutations.Count == 1 && permutations[0] == -1)
            permutations = Enumerable.Range(1, vals.Count).ToList();

          // loop through all permutations (analysis case will just have one)
          foreach (int perm in permutations)
          {
            GH_Path path = new GH_Path(result.CaseId, result.SelectedPermutationIDs == null ? 0 : perm);

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
                  ConcurrentDictionary<int, GsaResultQuantity> res = vals[perm - 1].xyzResults[ID];
                  GsaResultQuantity values = res[0]; // there is only one result per node
                  transX.Add(new GH_UnitNumber(values.X.ToUnit(this.LengthUnit))); // use ToUnit to capture changes in dropdown
                  transY.Add(new GH_UnitNumber(values.Y.ToUnit(this.LengthUnit)));
                  transZ.Add(new GH_UnitNumber(values.Z.ToUnit(this.LengthUnit)));
                  transXYZ.Add(new GH_UnitNumber(values.XYZ.ToUnit(this.LengthUnit)));
                }
              }
              if (item == 1)
              {
                for (int j = 0; j < sortedIDs.Count; j++)
                {
                  int ID = sortedIDs[j];
                  ConcurrentDictionary<int, GsaResultQuantity> res = vals[perm - 1].xxyyzzResults[ID];
                  GsaResultQuantity values = res[0]; // there is only one result per node
                  rotX.Add(new GH_UnitNumber(values.X));
                  rotY.Add(new GH_UnitNumber(values.Y));
                  rotZ.Add(new GH_UnitNumber(values.Z));
                  rotXYZ.Add(new GH_UnitNumber(values.XYZ));
                }
              }
            });

            out_transX.AddRange(transX, path);
            out_transY.AddRange(transY, path);
            out_transZ.AddRange(transZ, path);
            out_transXYZ.AddRange(transXYZ, path);
            out_rotX.AddRange(rotX, path);
            out_rotY.AddRange(rotY, path);
            out_rotZ.AddRange(rotZ, path);
            out_rotXYZ.AddRange(rotXYZ, path);
            outIDs.AddRange(ids, path);
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

        Helpers.PostHog.Result(result.Type, 0, GsaResultsValues.ResultType.Displacement);
      }
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitResult;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Unit",
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
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      int i = 0;
      Params.Output[i++].Name = "Translations X [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Y [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Z [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations |XYZ| [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

