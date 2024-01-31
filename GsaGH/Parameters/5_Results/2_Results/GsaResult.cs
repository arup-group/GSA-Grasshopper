using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers;

namespace GsaGH.Parameters.Results {
  /// <summary>
  /// <para>A Result is used to select Cases from an analysed <see cref="GsaModel"/> and extract the values for post-processing or visualisation.</para>
  /// <para>The following result types can be extracted if they are present in the model:
  /// <list type="bullet">
  /// <item><description><see href="https://docs.oasys-software.com/structural/gsa/references/dotnet-api/result-classes.html#noderesult">Node Results</see>: `Displacement` and `Reaction`.</description></item>
  /// <item><description><see href="https://docs.oasys-software.com/structural/gsa/references/dotnet-api/result-classes.html#element1dresult">1D Element Results</see>: `Displacement`, `Force` and `StrainEnergyDensity`.</description></item>
  /// <item><description><see href="https://docs.oasys-software.com/structural/gsa/references/dotnet-api/result-classes.html#element2dresult">2D Element Results</see>: `Displacement`, `Force`, `Moment`, `Shear` and `Stress`.</description></item>
  /// <item><description><see href="https://docs.oasys-software.com/structural/gsa/references/dotnet-api/result-classes.html#element3dresult">3D Element Results</see>: `Displacement` and `Stress`.</description></item>
  /// <item><description><see href="https://docs.oasys-software.com/structural/gsa/references/dotnet-api/result-classes.html#globalresult">Global Results</see>: `Frequency`, `LoadFactor`, `ModalGeometricStiffness`, `ModalMass`, `ModalStiffness`, `TotalLoad`, `TotalReaction`, `Mode`, `EffectiveInertia`, `EffectiveMass` and `Eigenvalue`.</description></item>
  /// </list></para>
  /// <para>All result values from the <see href="https://docs.oasys-software.com/structural/gsa/references/dotnet-api/introduction.html">.NET API</see> has been wrapped in <see href="https://docs.oasys-software.com/structural/gsa/references/gsagh/gsagh-unitnumber-parameter.html">Unit Number</see> and can be converted into different measures on the fly. The Result parameter caches the result values</para>
  /// </summary>
  public class GsaResult : IGsaResult {
    // Caches
    public IEntity0dResultCache<IEnergyDensity, Entity0dExtremaKey> Element1dAverageStrainEnergyDensities {
      get;
      private set;
    }
    public IEntity1dResultCache<IDisplacement, ResultVector6<Entity1dExtremaKey>> Element1dDisplacements {
      get;
      private set;
    }
    public IEntity1dResultCache<IInternalForce, ResultVector6<Entity1dExtremaKey>> Element1dInternalForces {
      get;
      private set;
    }
    public IEntity1dResultCache<IEnergyDensity, Entity1dExtremaKey> Element1dStrainEnergyDensities {
      get;
      private set;
    }
    public IEntity1dResultCache<IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>> Element1dDerivedStresses {
      get;
      private set;
    }
    public IEntity1dResultCache<IStress1d, ResultStress1d<Entity1dExtremaKey>> Element1dStresses {
      get;
      private set;
    }
    public IMeshResultCache<IMeshQuantity<IDisplacement>, IDisplacement, ResultVector6<Entity2dExtremaKey>> Element2dDisplacements {
      get;
      private set;
    }
    public IEntity2dLayeredResultCache<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> Element2dStresses {
      get;
      private set;
    }
    public IMeshResultCache<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> Element2dForces {
      get;
      private set;
    }
    public IMeshResultCache<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> Element2dMoments {
      get;
      private set;
    }
    public IMeshResultCache<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> Element2dShearForces {
      get;
      private set;
    }
    public IMeshResultCache<IMeshQuantity<ITranslation>, ITranslation, ResultVector3InAxis<Entity2dExtremaKey>> Element3dDisplacements {
      get;
      private set;
    }
    public IMeshResultCache<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> Element3dStresses {
      get;
      private set;
    }
    public IEntity0dResultCache<IDisplacement, ResultVector6<Entity0dExtremaKey>> NodeDisplacements {
      get;
      private set;
    }
    public IEntity0dResultCache<IFootfall, ResultFootfall<Entity0dExtremaKey>> NodeResonantFootfalls {
      get;
      private set;
    }
    public IEntity0dResultCache<IReactionForce, ResultVector6<Entity0dExtremaKey>> NodeReactionForces {
      get;
      private set;
    }
    public IEntity0dResultCache<IReactionForce, ResultVector6<Entity0dExtremaKey>> NodeSpringForces {
      get;
      private set;
    }
    public IEntity0dResultCache<IFootfall, ResultFootfall<Entity0dExtremaKey>> NodeTransientFootfalls {
      get;
      private set;
    }
    public IGlobalResultsCache GlobalResults {
      get;
      private set;
    }
    public IEntity1dResultCache<IDisplacement, ResultVector6<Entity1dExtremaKey>>
      Member1dDisplacements {
      get;
      private set;
    }
    public IEntity1dResultCache<IInternalForce, ResultVector6<Entity1dExtremaKey>>
      Member1dInternalForces {
      get;
      private set;
    }
    public IEntity0dResultCache<ISteelUtilisation, SteelUtilisationExtremaKeys> SteelUtilisations {
      get;
      private set;
    }

    public SteelDesignEffectiveLengthCache SteelDesignEffectiveLengths {
      get;
      private set;
    }

    internal GsaResult(GsaModel model, AnalysisCaseResult result, int caseId) {
      if (model == null || result == null) {
        return;
      }

      InitialiseAnalysisCaseResults(model, result, caseId);
    }

    internal GsaResult(
      GsaModel model, CombinationCaseResult result, int caseId, IEnumerable<int> permutations) {
      if (model == null || result == null) {
        return;
      }

      InitialiseCombinationsCaseResults(model, result, caseId, permutations?.OrderBy(x => x));
    }

    // Other members
    public int CaseId { get; set; }
    public int Permutations { get; set; } = 0;
    public string CaseName { get; set; }
    public GsaModel Model { get; set; }
    public List<int> SelectedPermutationIds { get; set; }
    public CaseType CaseType { get; set; }

    public override string ToString() {
      string txt = string.Empty;
      switch (CaseType) {
        case CaseType.AnalysisCase:
          txt = "A" + CaseId;
          break;

        case CaseType.CombinationCase:
          txt = "C" + CaseId;
          if (SelectedPermutationIds.Count > 0) {
            txt = SelectedPermutationIds.Count > 1
              ? $"{txt} ({SelectedPermutationIds.Count} permutations)" :
              txt + " p" + SelectedPermutationIds[0];
          }

          break;
      }

      if (!string.IsNullOrEmpty(CaseName)) {
        txt = $"{txt} '{CaseName}'";
      }

      return txt.TrimSpaces();
    }

    internal ReadOnlyCollection<int> NodeIds(string nodeList) {
      var entityList = new EntityList() {
        Definition = nodeList,
        Type = GsaAPI.EntityType.Node,
        Name = "node",
      };
      return Model.Model.ExpandList(entityList);
    }

    internal ReadOnlyCollection<int> ElementIds(string elementList, int dimension) {
      string filter = string.Empty;
      switch (dimension) {
        case 1:
          filter = " not (PA PV)";
          break;

        case 2:
          filter = " not (PB PV)";
          break;

        case 3:
          filter = " not (PB PA)";
          break;
      }

      var entityList = new EntityList() {
        Definition = elementList + filter,
        Type = GsaAPI.EntityType.Element,
        Name = "elem",
      };
      return Model.Model.ExpandList(entityList);
    }

    internal ReadOnlyCollection<int> MemberIds(string memberList) {
      var entityList = new EntityList() {
        Definition = memberList,
        Type = GsaAPI.EntityType.Member,
        Name = "mem",
      };
      return Model.Model.ExpandList(entityList);
    }

    private void InitialiseAnalysisCaseResults(
      GsaModel model, AnalysisCaseResult result, int caseId) {

      Element1dAverageStrainEnergyDensities = new Element1dAverageStrainEnergyDensityCache(result);
      Element1dDisplacements = new Element1dDisplacementCache(result);
      Element1dInternalForces = new Element1dInternalForceCache(result);
      Element1dDerivedStresses = new Element1dDerivedStressCache(result);
      Element1dStrainEnergyDensities = new Element1dStrainEnergyDensityCache(result);
      Element1dStresses = new Element1dStressCache(result);

      Element2dDisplacements = new Element2dDisplacementCache(result);
      Element2dForces = new Element2dForceCache(result);
      Element2dMoments = new Element2dMomentCache(result);
      Element2dShearForces = new Element2dShearForceCache(result);
      Element2dStresses = new Element2dStressCache(result);

      Element3dDisplacements = new Element3dDisplacementCache(result);
      Element3dStresses = new Element3dStressCache(result);

      NodeDisplacements = new NodeDisplacementCache(result);
      NodeReactionForces = new NodeReactionForceCache(result, model?.Model);
      NodeSpringForces = new NodeSpringForceCache(result);
      NodeResonantFootfalls = new NodeResonantFootfallCache(result);
      NodeTransientFootfalls = new NodeTransientFootfallCache(result);

      Member1dInternalForces = new Member1dInternalForceCache(result);
      Member1dDisplacements = new Member1dDisplacementCache(result);

      SteelUtilisations = new SteelUtilisationCache(result);
      SteelDesignEffectiveLengths = new SteelDesignEffectiveLengthCache(result);

      GlobalResults = new GlobalResultsCache(result);

      Model = model;
      CaseType = CaseType.AnalysisCase;
      CaseId = caseId;
      if (model?.Model?.Results()?.ContainsKey(caseId) != true) {
        return;
      }
      CaseName = model.Model.AnalysisCaseName(CaseId);
    }

    private void InitialiseCombinationsCaseResults(
      GsaModel model, CombinationCaseResult result, int caseId, IEnumerable<int> permutations) {
      ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> temp = result.NodeDisplacement(model.Model.Nodes().Keys.First().ToString());
      Permutations = temp[temp.Keys.First()].Count;

      Element1dAverageStrainEnergyDensities = new Element1dAverageStrainEnergyDensityCache(result);
      Element1dDisplacements = new Element1dDisplacementCache(result);
      Element1dInternalForces = new Element1dInternalForceCache(result);
      Element1dDerivedStresses = new Element1dDerivedStressCache(result);
      Element1dStrainEnergyDensities = new Element1dStrainEnergyDensityCache(result);
      Element1dStresses = new Element1dStressCache(result);

      Element2dDisplacements = new Element2dDisplacementCache(result);
      Element2dForces = new Element2dForceCache(result);
      Element2dMoments = new Element2dMomentCache(result);
      Element2dShearForces = new Element2dShearForceCache(result);
      Element2dStresses = new Element2dStressCache(result);

      Element3dDisplacements = new Element3dDisplacementCache(result);
      Element3dStresses = new Element3dStressCache(result);

      NodeDisplacements = new NodeDisplacementCache(result);
      NodeReactionForces = new NodeReactionForceCache(result, model?.Model);
      NodeSpringForces = new NodeSpringForceCache(result);

      Member1dDisplacements = new Member1dDisplacementCache(result);
      Member1dInternalForces = new Member1dInternalForceCache(result);

      SteelUtilisations = new SteelUtilisationCache(result);
      SteelDesignEffectiveLengths = new SteelDesignEffectiveLengthCache(result);

      Model = model;
      CaseType = CaseType.CombinationCase;
      CaseId = caseId;
      SelectedPermutationIds = permutations?.ToList();
      if (model?.Model?.CombinationCases()?.ContainsKey(caseId) == true) {
        CaseName = model.Model.CombinationCases()[CaseId].Name;
      }
    }
  }
}
