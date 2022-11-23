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
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class Elem2dStress : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("b5eb8a78-e0dd-442b-bbd7-0384d6c944cb");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Stress2D;

    public Elem2dStress() : base("2D Stresses",
      "Stress2D",
      "2D Projected Stress result values",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddTextParameter("Element filter list", "El", "Filter results by list." + System.Environment.NewLine +
          "Element list should take the form:" + System.Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddNumberParameter("Stress Layer", "σL", "Layer within the cross-section to get results." +
                           System.Environment.NewLine + "Input a number between -1 and 1, representing the normalised thickness," +
                           System.Environment.NewLine + "default value is zero => middle of the element.", GH_ParamAccess.item, 0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string unitAbbreviation = Pressure.GetAbbreviation(this.StresshUnit);

      string note = System.Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    System.Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
          System.Environment.NewLine + "branch contains a list of results in the following order:" +
          System.Environment.NewLine + "Vertex(1), Vertex(2), ..., Vertex(i), Centre" +
          System.Environment.NewLine + "+ve in-plane stresses: tensile(ie. + ve direct strain)." +
          System.Environment.NewLine + "+ve bending stress gives rise to tension on the top surface." +
          System.Environment.NewLine + "+ve shear stresses: +ve shear strain.";

      pManager.AddGenericParameter("Stress XX [" + unitAbbreviation + "]", "xx", "Stress in XX-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress YY [" + unitAbbreviation + "]", "yy", "Stress in YY-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress ZZ [" + unitAbbreviation + "]", "zz", "Stress in ZZ-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress XY [" + unitAbbreviation + "]", "xy", "Stress in XY-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress YZ [" + unitAbbreviation + "]", "yz", "Stress in YZ-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress ZX [" + unitAbbreviation + "]", "zx", "Stress in ZX-direction in Global Axis." + note, GH_ParamAccess.tree);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();

      // Get filter case
      string elementlist = "All";
      GH_String gh_Type = new GH_String();
      if (DA.GetData(1, ref gh_Type))
        GH_Convert.ToString(gh_Type, out elementlist, GH_Conversion.Both);

      // Get layer
      double layer = 0;
      GH_String gh_Type1 = new GH_String();
      if (DA.GetData(2, ref gh_Type1))
        GH_Convert.ToDouble(gh_Type1, out layer, GH_Conversion.Both);

      // data trees to output
      DataTree<GH_UnitNumber> out_XX = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_YY = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_ZZ = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_XY = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_YZ = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_ZX = new DataTree<GH_UnitNumber>();

      // Get Model
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(0, gh_types))
      {
        List<GsaResult> results = new List<GsaResult>();

        for (int i = 0; i < gh_types.Count; i++) // loop through all case/combinations
        {
          GH_ObjectWrapper gh_typ = gh_types[i];
          if (gh_typ.Value is GsaResultGoo)
          {
            result = ((GsaResultGoo)gh_typ.Value).Value;
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
            return;
          }

          List<GsaResultsValues> vals = result.Element2DStressValues(elementlist, layer, StresshUnit);

          List<int> permutations = (result.SelectedPermutationIDs == null ? new List<int>() { 0 } : result.SelectedPermutationIDs);

          // loop through all permutations (analysis case will just have one)
          for (int index = 0; index < vals.Count; index++)
          {
            if (vals[index].xyzResults.Count == 0 & vals[index].xxyyzzResults.Count == 0)
            {
              string[] typ = result.ToString().Split('{');
              string acase = typ[1].Replace('}', ' ');
              AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Case " + acase + "contains no Element2D results.");
              continue;
            }
            Parallel.For(0, 2, thread => // split computation in two for xyz and xxyyzz
            {
              if (thread == 0)
              {
                //do xyz part of results
                // loop through all elements
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[index].xyzResults)
                {
                  int elementID = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                  if (res.Count == 0) { continue; }

                  GH_Path p = new GH_Path(result.CaseID, permutations[index], elementID);

                  out_XX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(StresshUnit))), p); // use ToUnit to capture changes in dropdown
                  out_YY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(StresshUnit))), p);
                  out_ZZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(StresshUnit))), p);
                }
              }
              if (thread == 1)
              {
                //do xxyyzz
                // loop through all elements
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[index].xxyyzzResults)
                {
                  int elementID = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                  if (res.Count == 0) { continue; }

                  GH_Path p = new GH_Path(result.CaseID, permutations[index], elementID);

                  out_XY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X)), p); // always use [rad] units
                  out_YZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y)), p);
                  out_ZX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z)), p);
                }
              }
            });
          }
        }

        DA.SetDataTree(0, out_XX);
        DA.SetDataTree(1, out_YY);
        DA.SetDataTree(2, out_ZZ);
        DA.SetDataTree(3, out_XY);
        DA.SetDataTree(4, out_YZ);
        DA.SetDataTree(5, out_ZX);
      }
    }

    #region Custom UI
    private PressureUnit StresshUnit = DefaultUnits.StressUnitResult;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Unit",
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Stress
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      this.SelectedItems.Add(this.StresshUnit.ToString());

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.StresshUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.StresshUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Pressure.GetAbbreviation(this.StresshUnit);
      int i = 0;
      Params.Output[i++].Name = "Stress XX [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress YY [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress ZZ [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress XY [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress YZ [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress ZX [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

