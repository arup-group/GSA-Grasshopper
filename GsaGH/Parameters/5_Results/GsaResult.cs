using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Helpers.GsaApi;
using OasysUnits.Units;
using Rhino.Geometry;
using EnergyUnit = OasysUnits.Units.EnergyUnit;
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  public class GsaResult {
    /// <summary>
    ///   Analysis Case 1DElement Displacement Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, numberOfDivisions, axisId)
    /// </summary>
    internal Dictionary<Tuple<string, int, int>, GsaResultsValues>
      ACaseElement1DDisplacementValues { get; set; }
      = new Dictionary<Tuple<string, int, int>, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case 1DElement Footfall Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, numberOfDivisions)
    /// </summary>
    internal Dictionary<Tuple<string, FootfallResultType>, GsaResultsValues>
      ACaseElement1DFootfallValues { get; set; }
      = new Dictionary<Tuple<string, FootfallResultType>, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case 1DElement Force Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, numberOfDivisions, axisId)
    /// </summary>
    internal Dictionary<Tuple<string, int, int>, GsaResultsValues>
      ACaseElement1DForceValues { get; set; }
      = new Dictionary<Tuple<string, int, int>, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case 1DElement API Result Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, numberOfDivisions, axisId)
    /// </summary>
    internal Dictionary<Tuple<string, int, int>, ReadOnlyDictionary<int, Element1DResult>>
      ACaseElement1DResults { get; set; }
      = new Dictionary<Tuple<string, int, int>, ReadOnlyDictionary<int, Element1DResult>>();
    /// <summary>
    ///   Analysis Case 1DElement Strain Energy Density Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, numberOfDivisions, axisId)
    /// </summary>
    internal Dictionary<Tuple<string, int, int>, GsaResultsValues>
      ACaseElement1DStrainEnergyDensityValues { get; set; }
      = new Dictionary<Tuple<string, int, int>, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case 2DElement Displacement Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseElement2DDisplacementValues { get; set; }
      = new Dictionary<string, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case 1DElement Footfall Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, numberOfDivisions)
    /// </summary>
    internal Dictionary<Tuple<string, FootfallResultType>, GsaResultsValues>
      ACaseElement2DFootfallValues { get; set; }
      = new Dictionary<Tuple<string, FootfallResultType>, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case 2DElement Force Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseElement2DForceValues { get; set; }
      = new Dictionary<string, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case 2DElement API Result Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, layer)
    /// </summary>
    internal Dictionary<Tuple<string, double>, ReadOnlyDictionary<int, Element2DResult>>
      ACaseElement2DResults { get; set; }
      = new Dictionary<Tuple<string, double>, ReadOnlyDictionary<int, Element2DResult>>();
    /// <summary>
    ///   Analysis Case 2DElement Shear Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseElement2DShearValues { get; set; }
      = new Dictionary<string, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case 2DElement Stress Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, layer)
    /// </summary>
    internal Dictionary<Tuple<string, double>, GsaResultsValues>
      ACaseElement2DStressValues { get; set; }
      = new Dictionary<Tuple<string, double>, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case 3DElement Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseElement3DDisplacementValues { get; set; }
      = new Dictionary<string, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case 3DElement API Result Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    /// </summary>
    internal Dictionary<string, ReadOnlyDictionary<int, Element3DResult>>
      ACaseElement3DResults { get; set; }
      = new Dictionary<string, ReadOnlyDictionary<int, Element3DResult>>();
    /// <summary>
    ///   Analysis Case 3DElement Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseElement3DStressValues { get; set; }
      = new Dictionary<string, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case Node Displacement Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseNodeDisplacementValues { get; set; }
      = new Dictionary<string, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case Node Footfall Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    /// </summary>
    internal Dictionary<Tuple<string, FootfallResultType>, GsaResultsValues>
      ACaseNodeFootfallValues { get; set; }
      = new Dictionary<Tuple<string, FootfallResultType>, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case Node Reaction Force Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    /// </summary>
    internal Dictionary<string, GsaResultsValues> ACaseNodeReactionForceValues { get; set; }
      = new Dictionary<string, GsaResultsValues>();
    /// <summary>
    ///   Analysis Case Node API Result Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    /// </summary>
    internal Dictionary<string, ReadOnlyDictionary<int, NodeResult>> ACaseNodeResults { get; set; }
      = new Dictionary<string, ReadOnlyDictionary<int, NodeResult>>();
    /// <summary>
    ///   Analysis Case API Result
    /// </summary>
    internal AnalysisCaseResult AnalysisCaseResult { get; set; }
    internal int CaseId { get; set; }
    internal string CaseName { get; set; }
    /// <summary>
    ///   Combination Case API Result
    /// </summary>
    internal CombinationCaseResult CombinationCaseResult { get; set; }
    /// <summary>
    ///   Combination Case 1DElement Displacement Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, permutations, axisId)
    ///   value = Dictionary(elementID, Dictionary(numberOfDivisions, results))
    /// </summary>
    internal Dictionary<Tuple<string, int, int>, ConcurrentDictionary<int, GsaResultsValues>>
      ComboElement1DDisplacementValues { get; set; }
      = new Dictionary<Tuple<string, int, int>, ConcurrentDictionary<int, GsaResultsValues>>();
    /// <summary>
    ///   Combination Case 1DElement Forces Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, permutations, axisId)
    ///   value = Dictionary(elementID, Dictionary(numberOfDivisions, results))
    /// </summary>
    internal Dictionary<Tuple<string, int, int>, ConcurrentDictionary<int, GsaResultsValues>>
      ComboElement1DForceValues { get; set; }
      = new Dictionary<Tuple<string, int, int>, ConcurrentDictionary<int, GsaResultsValues>>();
    /// <summary>
    ///   Combination Case 1DElement API Result Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, permutations, axisId)
    ///   value = Dictionary(elementID, Dictionary(numberOfDivisions, results))
    /// </summary>
    internal
      Dictionary<Tuple<string, int, int>, ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>>>
      ComboElement1DResults { get; set; }
      = new Dictionary<Tuple<string, int, int>,
        ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>>>();
    /// <summary>
    ///   Combination Case 1DElement API Result Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, permutations, axisId)
    ///   value = Dictionary(elementID, Dictionary(numberOfDivisions, results))
    /// </summary>
    internal
      Dictionary<Tuple<string, int, int>, ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>>>
      ComboElement1DResultsInclStrainEnergyDensity { get; set; }
      = new Dictionary<Tuple<string, int, int>,
        ReadOnlyDictionary<int, ReadOnlyCollection<Element1DResult>>>();
    /// <summary>
    ///   Combination Case 1DElement Strain Energy Density Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, permutations, axisId)
    ///   value = Dictionary(elementID, Dictionary(numberOfDivisions, results))
    /// </summary>
    internal Dictionary<Tuple<string, int, int>, ConcurrentDictionary<int, GsaResultsValues>>
      ComboElement1DStrainEnergyDensityValues { get; set; }
      = new Dictionary<Tuple<string, int, int>, ConcurrentDictionary<int, GsaResultsValues>>();
    /// <summary>
    ///   Combination Case 2DElement Displacement Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    ///   value = (elementID, collection(permutationResult))
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>
      ComboElement2DDisplacementValues { get; set; }
      = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();
    /// <summary>
    ///   Combination Case 2DElement Force/Moment Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    ///   value = Dictionary(elementID, Dictionary(permutationID, permutationsResults))
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>
      ComboElement2DForceValues { get; set; }
      = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();
    /// <summary>
    ///   Combination Case 2DElement API Result Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, layer)
    ///   value = Dictionary(elementID, Dictionary(permutationID, permutationsResults))
    /// </summary>
    internal Dictionary<Tuple<string, double>,
        ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>>>
      ComboElement2DResults { get; set; }
      = new Dictionary<Tuple<string, double>,
        ReadOnlyDictionary<int, ReadOnlyCollection<Element2DResult>>>();
    /// <summary>
    ///   Combination Case 2DElement Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    ///   value = Dictionary(elementID, Dictionary(numberOfDivisions, results))
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>
      ComboElement2DShearValues { get; set; }
      = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();
    /// <summary>
    ///   Combination Case 2DElement Stress Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = Tuple(elementList, layer)
    ///   value = Dictionary(elementID, Dictionary(permutationID, permutationsResults))
    /// </summary>
    internal Dictionary<Tuple<string, double>, ConcurrentDictionary<int, GsaResultsValues>>
      ComboElement2DStressValues { get; set; }
      = new Dictionary<Tuple<string, double>, ConcurrentDictionary<int, GsaResultsValues>>();
    /// <summary>
    ///   Combination Case 3DElement Displacement Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    ///   value = Dictionary(elementID, Dictionary(permutationID, permutationsResults))
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>
      ComboElement3DDisplacementValues { get; set; }
      = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();
    /// <summary>
    ///   Combination Case 3DElement API Result Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    ///   value = (elementID, collection(permutationResult))
    /// </summary>
    internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>>>
      ComboElement3DResults { get; set; }
      = new Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<Element3DResult>>>();
    /// <summary>
    ///   Combination Case 3DElement Stress Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    ///   value = Dictionary(elementID, Dictionary(permutationID, permutationsResults))
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>
      ComboElement3DStressValues { get; set; }
      = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();
    /// <summary>
    ///   Combination Case Node Displacement Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    ///   value = Dictionary(permutationID, permutationsResults)
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>
      ComboNodeDisplacementValues { get; set; }
      = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();
    /// <summary>
    ///   Combination Case Node Reaction Force Result VALUES Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    ///   value = Dictionary(permutationID, permutationsResults)
    /// </summary>
    internal Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>
      ComboNodeReactionForceValues { get; set; }
      = new Dictionary<string, ConcurrentDictionary<int, GsaResultsValues>>();
    /// <summary>
    ///   Combination Case Node API Result Dictionary
    ///   Append to this dictionary to chache results
    ///   key = elementList
    ///   value = (elementID, collection(permutationResult))
    /// </summary>
    internal Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>>>
      ComboNodeResults { get; set; }
      = new Dictionary<string, ReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>>>();
    internal GsaModel Model { get; set; }
    /// <summary>
    ///   User set permutation ID. If -1 => return all.
    /// </summary>
    internal List<int> SelectedPermutationIds { get; set; }
    internal CaseType Type { get; set; }

    public GsaResult() { }

    internal GsaResult(GsaModel model, AnalysisCaseResult result, int caseId) {
      Model = model;
      AnalysisCaseResult = result;
      Type = CaseType.AnalysisCase;
      CaseId = caseId;
      CaseName = model.Model.AnalysisCaseName(CaseId);
    }

    internal GsaResult(
      GsaModel model, CombinationCaseResult result, int caseId, IEnumerable<int> permutations) {
      Model = model;
      CombinationCaseResult = result;
      Type = CaseType.Combination;
      CaseId = caseId;
      SelectedPermutationIds = permutations.OrderBy(x => x).ToList();
    }

    internal GsaResult(GsaModel model, CombinationCaseResult result, int caseId, int permutation) {
      Model = model;
      CombinationCaseResult = result;
      Type = CaseType.Combination;
      CaseId = caseId;
      SelectedPermutationIds = new List<int>() {
        permutation,
      };
    }

    public GsaResult Duplicate() {
      return this;
    }

    public override string ToString() {
      string txt = string.Empty;
      switch (Type) {
        case CaseType.AnalysisCase:
          txt = "A" + CaseId;
          break;

        case CaseType.Combination: {
            txt = "C" + CaseId;
            if (SelectedPermutationIds.Count > 0) {
              txt = SelectedPermutationIds.Count > 1 ? txt + " P:" + SelectedPermutationIds.Count :
                txt + " p" + SelectedPermutationIds[0];
            }

            break;
          }
      }

      return txt.Trim().Replace("  ", " ");
    }

    /// <summary>
    ///   Get beam average strain energy density values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="axisId"></param>
    /// <param name="energyUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element1DAverageStrainEnergyDensityValues(
      string elementlist, int axisId, EnergyUnit energyUnit) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }

      var key = new Tuple<string, int, int>(elementlist, 1, axisId);
      if (Type == CaseType.AnalysisCase) {
        if (!ACaseElement1DStrainEnergyDensityValues.ContainsKey(key)) {
          if (!ACaseElement1DResults.ContainsKey(key)) {
            ACaseElement1DResults.Add(key, AnalysisCaseResult.Element1DResults(elementlist, 1));
          }

          ACaseElement1DStrainEnergyDensityValues.Add(key,
            ResultHelper.GetElement1DResultValues(ACaseElement1DResults[key], energyUnit, true));
        }

        return new List<GsaResultsValues> {
          ACaseElement1DStrainEnergyDensityValues[key],
        };
      }

      if (ComboElement1DStrainEnergyDensityValues.ContainsKey(key)) {
        return new List<GsaResultsValues>(ComboElement1DStrainEnergyDensityValues[key].Values);
      }

      if (!ComboElement1DResults.ContainsKey(key)) {
        ComboElement1DResults.Add(key,
          CombinationCaseResult.Element1DResults(elementlist, 1, true));
      }

      ComboElement1DStrainEnergyDensityValues.Add(key,
        ResultHelper.GetElement1DResultValues(ComboElement1DResults[key], energyUnit,
          SelectedPermutationIds, true));

      return new List<GsaResultsValues>(ComboElement1DStrainEnergyDensityValues[key].Values);
    }

    /// <summary>
    ///   Get beam displacement values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cached data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="positionsCount"></param>
    /// <param name="axisId"></param>
    /// <param name="lengthUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element1DDisplacementValues(
      string elementlist, int positionsCount, int axisId, LengthUnit lengthUnit) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }
      Plane global = Plane.WorldXY;

      var key = new Tuple<string, int, int>(elementlist, positionsCount, axisId);
      if (Type == CaseType.AnalysisCase) {
        if (!ACaseElement1DDisplacementValues.ContainsKey(key)) {
          if (!ACaseElement1DResults.ContainsKey(key)) {
            ACaseElement1DResults.Add(key,
              AnalysisCaseResult.Element1DResults(elementlist, positionsCount));
          }

          GsaResultsValues res = ResultHelper.GetElement1DResultValues(ACaseElement1DResults[key], lengthUnit);
          if (axisId == 0) {
            res.CoordinateTransformationTo(global, Model.Model);
          }

          ACaseElement1DDisplacementValues.Add(key, res);
        }

        return new List<GsaResultsValues> {
          ACaseElement1DDisplacementValues[key]
        };
      }

      if (!ComboElement1DDisplacementValues.ContainsKey(key)) {
        if (!ComboElement1DResults.ContainsKey(key)) {
          ComboElement1DResults.Add(key,
            CombinationCaseResult.Element1DResults(elementlist, positionsCount, false));
        }

        ComboElement1DDisplacementValues.Add(key,
          ResultHelper.GetElement1DResultValues(ComboElement1DResults[key], lengthUnit,
            SelectedPermutationIds));
      }

      return new List<GsaResultsValues>(ComboElement1DDisplacementValues[key].Values);
    }

    /// <summary>
    ///   Get beam footfall values
    ///   For analysis case results only
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element1DFootfallValues(
      string elementlist, FootfallResultType type) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }

      var key = new Tuple<string, FootfallResultType>(elementlist, type);
      if (Type != CaseType.AnalysisCase) {
        throw new Exception("Cannot get Footfall results for a Combination Case.");
      }

      if (!ACaseElement1DFootfallValues.ContainsKey(key)) {
        GsaResultsValues nodeFootfallResultValues = NodeFootfallValues("All", type);
        ACaseElement1DFootfallValues.Add(key,
          ResultHelper.GetElement1DFootfallResultValues(elementlist, Model,
            nodeFootfallResultValues));
      }

      return new List<GsaResultsValues> {
        ACaseElement1DFootfallValues[key],
      };
    }

    /// <summary>
    ///   Get beam force values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="positionsCount"></param>
    /// <param name="axisId"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element1DForceValues(
      string elementlist, int positionsCount, int axisId, ForceUnit forceUnit, MomentUnit momentUnit) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }

      var key = new Tuple<string, int, int>(elementlist, positionsCount, axisId);
      if (Type == CaseType.AnalysisCase) {
        if (!ACaseElement1DForceValues.ContainsKey(key)) {
          if (!ACaseElement1DResults.ContainsKey(key)) {
            ACaseElement1DResults.Add(key,
              AnalysisCaseResult.Element1DResults(elementlist, positionsCount));
          }

          ACaseElement1DForceValues.Add(key,
            ResultHelper.GetElement1DResultValues(ACaseElement1DResults[key], forceUnit,
              momentUnit));
        }

        return new List<GsaResultsValues> {
          ACaseElement1DForceValues[key],
        };
      }

      if (!ComboElement1DForceValues.ContainsKey(key)) {
        if (!ComboElement1DResults.ContainsKey(key)) {
          ComboElement1DResults.Add(key,
            CombinationCaseResult.Element1DResults(elementlist, positionsCount, false));
        }

        ComboElement1DForceValues.Add(key,
          ResultHelper.GetElement1DResultValues(ComboElement1DResults[key], forceUnit, momentUnit,
            SelectedPermutationIds));
      }

      return new List<GsaResultsValues>(ComboElement1DForceValues[key].Values);
    }

    /// <summary>
    ///   Get beam strain energy density values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="positionsCount"></param>
    /// <param name="axisId"></param>
    /// <param name="energyUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element1DStrainEnergyDensityValues(
      string elementlist, int positionsCount, int axisId, EnergyUnit energyUnit) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }

      var key = new Tuple<string, int, int>(elementlist, positionsCount, axisId);
      if (Type == CaseType.AnalysisCase) {
        if (!ACaseElement1DStrainEnergyDensityValues.ContainsKey(key)) {
          if (!ACaseElement1DResults.ContainsKey(key)) {
            ACaseElement1DResults.Add(key,
              AnalysisCaseResult.Element1DResults(elementlist, positionsCount));
          }

          ACaseElement1DStrainEnergyDensityValues.Add(key,
            ResultHelper.GetElement1DResultValues(ACaseElement1DResults[key], energyUnit));
        }

        return new List<GsaResultsValues> {
          ACaseElement1DStrainEnergyDensityValues[key],
        };
      }

      if (ComboElement1DStrainEnergyDensityValues.ContainsKey(key)) {
        return new List<GsaResultsValues>(ComboElement1DStrainEnergyDensityValues[key].Values);
      }

      if (!ComboElement1DResultsInclStrainEnergyDensity.ContainsKey(key)) {
        ComboElement1DResultsInclStrainEnergyDensity.Add(key,
          CombinationCaseResult.Element1DResults(elementlist, positionsCount, true));
      }

      ComboElement1DStrainEnergyDensityValues.Add(key,
        ResultHelper.GetElement1DResultValues(ComboElement1DResultsInclStrainEnergyDensity[key],
          energyUnit, SelectedPermutationIds));

      return new List<GsaResultsValues>(ComboElement1DStrainEnergyDensityValues[key].Values);
    }

    /// <summary>
    ///   Get 2D displacement values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="lengthUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element2DDisplacementValues(
      string elementlist, LengthUnit lengthUnit) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }

      if (Type == CaseType.AnalysisCase) {
        if (!ACaseElement2DDisplacementValues.ContainsKey(elementlist)) {
          if (!ACaseElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) {
            ACaseElement2DResults.Add(new Tuple<string, double>(elementlist, 0),
              AnalysisCaseResult.Element2DResults(elementlist, 0));
          }

          ACaseElement2DDisplacementValues.Add(elementlist,
            ResultHelper.GetElement2DResultValues(
              ACaseElement2DResults[new Tuple<string, double>(elementlist, 0)], lengthUnit));
        }

        return new List<GsaResultsValues> {
          ACaseElement2DDisplacementValues[elementlist],
        };
      }

      if (ComboElement2DDisplacementValues.ContainsKey(elementlist)) {
        return new List<GsaResultsValues>(ComboElement2DDisplacementValues[elementlist].Values);
      }

      if (!ComboElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) {
        ComboElement2DResults.Add(new Tuple<string, double>(elementlist, 0),
          CombinationCaseResult.Element2DResults(elementlist, 0));
      }

      ComboElement2DDisplacementValues.Add(elementlist,
        ResultHelper.GetElement2DResultValues(
          ComboElement2DResults[new Tuple<string, double>(elementlist, 0)], lengthUnit,
          SelectedPermutationIds));

      return new List<GsaResultsValues>(ComboElement2DDisplacementValues[elementlist].Values);
    }

    /// <summary>
    ///   Get beam footfall values
    ///   For analysis case results only
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element2DFootfallValues(
      string elementlist, FootfallResultType type) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }

      var key = new Tuple<string, FootfallResultType>(elementlist, type);
      if (Type == CaseType.AnalysisCase) {
        if (!ACaseElement2DFootfallValues.ContainsKey(key)) {
          GsaResultsValues nodeFootfallResultValues = NodeFootfallValues("All", type);
          ACaseElement2DFootfallValues.Add(key,
            ResultHelper.GetElement2DFootfallResultValues(elementlist, Model,
              nodeFootfallResultValues));
        }

        return new List<GsaResultsValues>() {
          ACaseElement2DFootfallValues[key],
        };
      } else {
        throw new Exception("Cannot get Footfall results for a Combination Case.");
      }
    }

    /// <summary>
    ///   Get 2D force values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element2DForceValues(
      string elementlist, ForcePerLengthUnit forceUnit, ForceUnit momentUnit) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }

      if (Type == CaseType.AnalysisCase) {
        if (!ACaseElement2DForceValues.ContainsKey(elementlist)) {
          if (!ACaseElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) {
            ACaseElement2DResults.Add(new Tuple<string, double>(elementlist, 0),
              AnalysisCaseResult.Element2DResults(elementlist, 0));
          }

          ACaseElement2DForceValues.Add(elementlist,
            ResultHelper.GetElement2DResultValues(
              ACaseElement2DResults[new Tuple<string, double>(elementlist, 0)], forceUnit,
              momentUnit));
        }

        return new List<GsaResultsValues> {
          ACaseElement2DForceValues[elementlist],
        };
      }

      if (ComboElement2DForceValues.ContainsKey(elementlist)) {
        return new List<GsaResultsValues>(ComboElement2DForceValues[elementlist].Values);
      }

      if (!ComboElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) {
        ComboElement2DResults.Add(new Tuple<string, double>(elementlist, 0),
          CombinationCaseResult.Element2DResults(elementlist, 0));
      }

      ComboElement2DForceValues.Add(elementlist,
        ResultHelper.GetElement2DResultValues(
          ComboElement2DResults[new Tuple<string, double>(elementlist, 0)], forceUnit, momentUnit,
          SelectedPermutationIds));

      return new List<GsaResultsValues>(ComboElement2DForceValues[elementlist].Values);
    }

    /// <summary>
    ///   Get 2D shear force values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// ///
    /// <param name="forceUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element2DShearValues(
      string elementlist, ForcePerLengthUnit forceUnit) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }

      if (Type == CaseType.AnalysisCase) {
        if (!ACaseElement2DShearValues.ContainsKey(elementlist)) {
          if (!ACaseElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) {
            ACaseElement2DResults.Add(new Tuple<string, double>(elementlist, 0),
              AnalysisCaseResult.Element2DResults(elementlist, 0));
          }

          ACaseElement2DShearValues.Add(elementlist,
            ResultHelper.GetElement2DResultValues(
              ACaseElement2DResults[new Tuple<string, double>(elementlist, 0)], forceUnit));
        }

        return new List<GsaResultsValues> {
          ACaseElement2DShearValues[elementlist],
        };
      }

      if (ComboElement2DShearValues.ContainsKey(elementlist)) {
        return new List<GsaResultsValues>(ComboElement2DShearValues[elementlist].Values);
      }

      if (!ComboElement2DResults.ContainsKey(new Tuple<string, double>(elementlist, 0))) {
        ComboElement2DResults.Add(new Tuple<string, double>(elementlist, 0),
          CombinationCaseResult.Element2DResults(elementlist, 0));
      }

      ComboElement2DShearValues.Add(elementlist,
        ResultHelper.GetElement2DResultValues(
          ComboElement2DResults[new Tuple<string, double>(elementlist, 0)], forceUnit,
          SelectedPermutationIds));

      return new List<GsaResultsValues>(ComboElement2DShearValues[elementlist].Values);
    }

    /// <summary>
    ///   Get 2D stress values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="layer"></param>
    /// <param name="stressUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element2DStressValues(
      string elementlist, double layer, PressureUnit stressUnit) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }

      var key = new Tuple<string, double>(elementlist, layer);
      if (Type == CaseType.AnalysisCase) {
        if (!ACaseElement2DStressValues.ContainsKey(key)) {
          if (!ACaseElement2DResults.ContainsKey(key)) {
            ACaseElement2DResults.Add(key, AnalysisCaseResult.Element2DResults(elementlist, layer));
          }

          ACaseElement2DStressValues.Add(key,
            ResultHelper.GetElement2DResultValues(ACaseElement2DResults[key], stressUnit));
        }

        return new List<GsaResultsValues> {
          ACaseElement2DStressValues[key],
        };
      }

      if (ComboElement2DStressValues.ContainsKey(key)) {
        return new List<GsaResultsValues>(ComboElement2DStressValues[key].Values);
      }

      if (!ComboElement2DResults.ContainsKey(key)) {
        ComboElement2DResults.Add(key, CombinationCaseResult.Element2DResults(elementlist, layer));
      }

      ComboElement2DStressValues.Add(key,
        ResultHelper.GetElement2DResultValues(ComboElement2DResults[key], stressUnit,
          SelectedPermutationIds));

      return new List<GsaResultsValues>(ComboElement2DStressValues[key].Values);
    }

    /// <summary>
    ///   Get 3D displacement values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="lengthUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element3DDisplacementValues(
      string elementlist, LengthUnit lengthUnit) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }

      if (Type == CaseType.AnalysisCase) {
        if (!ACaseElement3DDisplacementValues.ContainsKey(elementlist)) {
          if (!ACaseElement3DResults.ContainsKey(elementlist)) {
            ACaseElement3DResults.Add(elementlist,
              AnalysisCaseResult.Element3DResults(elementlist));
          }

          ACaseElement3DDisplacementValues.Add(elementlist,
            ResultHelper.GetElement3DResultValues(ACaseElement3DResults[elementlist], lengthUnit));
        }

        return new List<GsaResultsValues> {
          ACaseElement3DDisplacementValues[elementlist],
        };
      }

      if (ComboElement3DDisplacementValues.ContainsKey(elementlist)) {
        return new List<GsaResultsValues>(ComboElement3DDisplacementValues[elementlist].Values);
      }

      if (!ComboElement3DResults.ContainsKey(elementlist)) {
        ComboElement3DResults.Add(elementlist, CombinationCaseResult.Element3DResults(elementlist));
      }

      ComboElement3DDisplacementValues.Add(elementlist,
        ResultHelper.GetElement3DResultValues(ComboElement3DResults[elementlist], lengthUnit,
          SelectedPermutationIds));

      return new List<GsaResultsValues>(ComboElement3DDisplacementValues[elementlist].Values);
    }

    /// <summary>
    ///   Get 2D stress values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="elementlist"></param>
    /// <param name="stressUnit"></param>
    /// <returns></returns>
    internal List<GsaResultsValues> Element3DStressValues(
      string elementlist, PressureUnit stressUnit) {
      if (elementlist.ToLower() == "all" || elementlist == string.Empty) {
        elementlist = "All";
      }

      if (Type == CaseType.AnalysisCase) {
        if (!ACaseElement3DStressValues.ContainsKey(elementlist)) {
          if (!ACaseElement3DResults.ContainsKey(elementlist)) {
            ACaseElement3DResults.Add(elementlist,
              AnalysisCaseResult.Element3DResults(elementlist));
          }

          ACaseElement3DStressValues.Add(elementlist,
            ResultHelper.GetElement3DResultValues(ACaseElement3DResults[elementlist], stressUnit));
        }

        return new List<GsaResultsValues> {
          ACaseElement3DStressValues[elementlist],
        };
      }

      if (ComboElement3DStressValues.ContainsKey(elementlist)) {
        return new List<GsaResultsValues>(ComboElement3DStressValues[elementlist].Values);
      }

      if (!ComboElement3DResults.ContainsKey(elementlist)) {
        ComboElement3DResults.Add(elementlist, CombinationCaseResult.Element3DResults(elementlist));
      }

      ComboElement3DStressValues.Add(elementlist,
        ResultHelper.GetElement3DResultValues(ComboElement3DResults[elementlist], stressUnit,
          SelectedPermutationIds));

      return new List<GsaResultsValues>(ComboElement3DStressValues[elementlist].Values);
    }

    /// <summary>
    ///   Get node displacement values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="nodelist"></param>
    /// <param name="lengthUnit"></param>
    /// <returns></returns>
    internal Tuple<List<GsaResultsValues>, List<int>> NodeDisplacementValues(
      string nodelist, LengthUnit lengthUnit) {
      if (nodelist.ToLower() == "all" || nodelist == string.Empty) {
        nodelist = "All";
      }

      if (Type == CaseType.AnalysisCase) {
        if (!ACaseNodeDisplacementValues.ContainsKey(nodelist)) {
          if (!ACaseNodeResults.ContainsKey(nodelist)) {
            ACaseNodeResults.Add(nodelist, AnalysisCaseResult.NodeResults(nodelist));
          }

          ACaseNodeDisplacementValues.Add(nodelist,
            ResultHelper.GetNodeResultValues(ACaseNodeResults[nodelist], lengthUnit));
        }

        return new Tuple<List<GsaResultsValues>, List<int>>(new List<GsaResultsValues> {
          ACaseNodeDisplacementValues[nodelist],
        }, Model.Model.Nodes(nodelist).Keys.ToList());
      }

      if (!ComboNodeDisplacementValues.ContainsKey(nodelist)) {
        if (!ComboNodeResults.ContainsKey(nodelist)) {
          ComboNodeResults.Add(nodelist, CombinationCaseResult.NodeResults(nodelist));
        }

        ComboNodeDisplacementValues.Add(nodelist,
          ResultHelper.GetNodeResultValues(ComboNodeResults[nodelist], lengthUnit,
            SelectedPermutationIds));
      }

      return new Tuple<List<GsaResultsValues>, List<int>>(
        new List<GsaResultsValues>(ComboNodeDisplacementValues[nodelist].Values),
        Model.Model.Nodes(nodelist).Keys.ToList());
    }

    /// <summary>
    ///   Get node footfall result values
    ///   For analysis case results only
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="nodelist"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    internal GsaResultsValues NodeFootfallValues(string nodelist, FootfallResultType type) {
      if (nodelist.ToLower() == "all" || nodelist == string.Empty) {
        nodelist = "All";
      }

      if (Type == CaseType.AnalysisCase) {
        var key = new Tuple<string, FootfallResultType>(nodelist, type);
        if (!ACaseNodeFootfallValues.ContainsKey(key)) {
          ACaseNodeFootfallValues.Add(key,
            ResultHelper.GetNodeFootfallResultValues(nodelist, Model, type, CaseId));
        }

        return ACaseNodeFootfallValues[key];
      }

      throw new Exception("Cannot get Footfall results for a Combination Case.");
    }

    /// <summary>
    ///   Get node displacement values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="nodelist"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <returns></returns>
    internal Tuple<List<GsaResultsValues>, List<int>> NodeReactionForceValues(
      string nodelist, ForceUnit forceUnit, MomentUnit momentUnit) {
      if (nodelist.ToLower() == "all" || nodelist == string.Empty) {
        nodelist = "All";
      }

      ConcurrentBag<int> supportnodeIDs = null;
      if (nodelist.ToLower() == "all" || nodelist == string.Empty) {
        supportnodeIDs = new ConcurrentBag<int>();
        ReadOnlyDictionary<int, Node> nodes = Model.Model.Nodes();
        Parallel.ForEach(nodes, node => {
          NodalRestraint rest = node.Value.Restraint;
          if (rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ) {
            supportnodeIDs.Add(node.Key);
          }
        });
        nodelist = "All";
      }

      if (Type == CaseType.AnalysisCase) {
        if (!ACaseNodeReactionForceValues.ContainsKey(nodelist)) {
          if (!ACaseNodeResults.ContainsKey(nodelist)) {
            ACaseNodeResults.Add(nodelist, AnalysisCaseResult.NodeResults(nodelist));
          }

          ACaseNodeReactionForceValues.Add(nodelist,
            ResultHelper.GetNodeReactionForceResultValues(ACaseNodeResults[nodelist], forceUnit,
              momentUnit, supportnodeIDs));
        }

        return new Tuple<List<GsaResultsValues>, List<int>>(new List<GsaResultsValues> {
          ACaseNodeReactionForceValues[nodelist],
        }, ACaseNodeReactionForceValues[nodelist].XyzResults.Keys.OrderBy(x => x).ToList());
      }

      if (!ComboNodeReactionForceValues.ContainsKey(nodelist)) {
        if (!ComboNodeResults.ContainsKey(nodelist)) {
          ComboNodeResults.Add(nodelist, CombinationCaseResult.NodeResults(nodelist));
        }

        ComboNodeReactionForceValues.Add(nodelist,
          ResultHelper.GetNodeReactionForceResultValues(ComboNodeResults[nodelist], forceUnit,
            momentUnit, SelectedPermutationIds, supportnodeIDs));
      }

      return new Tuple<List<GsaResultsValues>, List<int>>(
        new List<GsaResultsValues>(ComboNodeReactionForceValues[nodelist].Values),
        ComboNodeReactionForceValues[nodelist].Values.First().XyzResults.Keys.OrderBy(x => x)
         .ToList());
    }

    /// <summary>
    ///   Get node displacement values
    ///   For analysis case the length of the list will be 1
    ///   This method will use cache data if it exists
    /// </summary>
    /// <param name="nodelist"></param>
    /// <param name="forceUnit"></param>
    /// <param name="momentUnit"></param>
    /// <returns></returns>
    internal Tuple<List<GsaResultsValues>, List<int>> SpringReactionForceValues(
      string nodelist, ForceUnit forceUnit, MomentUnit momentUnit) {
      if (nodelist.ToLower() == "all" || nodelist == string.Empty) {
        nodelist = "All";
      }

      ConcurrentBag<int> supportnodeIDs = null;
      if (nodelist.ToLower() == "all" || nodelist == string.Empty) {
        supportnodeIDs = new ConcurrentBag<int>();
        ReadOnlyDictionary<int, Node> nodes = Model.Model.Nodes();
        Parallel.ForEach(nodes, node => {
          NodalRestraint rest = node.Value.Restraint;
          if (rest.X || rest.Y || rest.Z || rest.XX || rest.YY || rest.ZZ) {
            supportnodeIDs.Add(node.Key);
          }
        });
        nodelist = "All";
      }

      if (Type == CaseType.AnalysisCase) {
        if (!ACaseNodeReactionForceValues.ContainsKey(nodelist)) {
          if (!ACaseNodeResults.ContainsKey(nodelist)) {
            ACaseNodeResults.Add(nodelist, AnalysisCaseResult.NodeResults(nodelist));
          }

          ACaseNodeReactionForceValues.Add(nodelist,
            ResultHelper.GetNodeSpringForceResultValues(ACaseNodeResults[nodelist], forceUnit,
              momentUnit, supportnodeIDs));
        }

        return new Tuple<List<GsaResultsValues>, List<int>>(new List<GsaResultsValues> {
          ACaseNodeReactionForceValues[nodelist],
        }, ACaseNodeReactionForceValues[nodelist].XyzResults.Keys.OrderBy(x => x).ToList());
      }

      if (!ComboNodeReactionForceValues.ContainsKey(nodelist)) {
        if (!ComboNodeResults.ContainsKey(nodelist)) {
          ComboNodeResults.Add(nodelist, CombinationCaseResult.NodeResults(nodelist));
        }

        ComboNodeReactionForceValues.Add(nodelist,
          ResultHelper.GetNodeSpringForceResultValues(ComboNodeResults[nodelist], forceUnit,
            momentUnit, SelectedPermutationIds, supportnodeIDs));
      }

      return new Tuple<List<GsaResultsValues>, List<int>>(
        new List<GsaResultsValues>(ComboNodeReactionForceValues[nodelist].Values),
        ComboNodeReactionForceValues[nodelist].Values.First().XyzResults.Keys.OrderBy(x => x)
         .ToList());
    }
  }
}
