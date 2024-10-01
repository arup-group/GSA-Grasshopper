using GsaAPI;

using OasysUnits;
using OasysUnits.Units;

using MassUnit = OasysUnits.Units.MassUnit;

namespace GsaGH.Parameters.Results {
  public class GlobalResultsCache : IGlobalResultsCache {

    private IEffectiveInertia _effectiveInertia;
    private IEffectiveMass _effectiveMass;
    private double? _eigenvalue;
    private Frequency _frequency;
    private Ratio _loadFactor;
    private ForcePerLength _modalGeometricStiffness;
    private Mass _modalMass;
    private ForcePerLength _modalStiffness;
    private int? _mode;
    private IReactionForce _totalLoad;
    private IReactionForce _totalReaction;

    private bool _calculated = false;

    internal GlobalResultsCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    public IApiResult ApiResult { get; private set; }
    public IEffectiveInertia EffectiveInertia {
      get {
        Calculate();
        return _effectiveInertia;
      }
    }
    public IEffectiveMass EffectiveMass {
      get {
        Calculate();
        return _effectiveMass;
      }
    }

    public double? Eigenvalue {
      get {
        Calculate();
        return _eigenvalue;
      }
    }

    public Frequency Frequency {
      get {
        Calculate();
        return _frequency;
      }
    }
    public Ratio LoadFactor {
      get {
        Calculate();
        return _loadFactor;
      }
    }
    public ForcePerLength ModalGeometricStiffness {
      get {
        Calculate();
        return _modalGeometricStiffness;
      }
    }
    public Mass ModalMass {
      get {
        Calculate();
        return _modalMass;
      }
    }
    public ForcePerLength ModalStiffness {
      get {
        Calculate();
        return _modalStiffness;
      }
    }
    public int? Mode {
      get {
        Calculate();
        return _mode;
      }
    }
    public IReactionForce TotalLoad {
      get {
        Calculate();
        return _totalLoad;
      }
    }
    public IReactionForce TotalReaction {
      get {
        Calculate();
        return _totalReaction;
      }
    }

    // GSA-6747 CombinationCaseResult does not contain global results
    //internal GlobalResultsCache(CombinationCaseResult result) {
    //  ApiResult = new ApiResult(result);
    //}

    private void Calculate() {
      if (_calculated) {
        return;
      }

      GlobalResult res = ((AnalysisCaseResult)ApiResult.Result).Global;
      if (res.EffectiveInertia != null) {
        _effectiveInertia = new EffectiveInertia(res.EffectiveInertia);
      }

      _effectiveMass = new EffectiveMass(res.EffectiveMass);

      if (res.Frequency == 0 && res.LoadFactor == 0 && res.ModalStiffness != 0) {
        _eigenvalue = res.ModalStiffness;
      }

      if (res.Frequency != 0) {
        _frequency = new Frequency(res.Frequency, FrequencyUnit.Hertz);
      }

      if (res.LoadFactor != 0) {
        _loadFactor = new Ratio(res.LoadFactor, RatioUnit.DecimalFraction);
      }

      if (res.ModalGeometricStiffness != 0) {
        _modalGeometricStiffness = new ForcePerLength(res.ModalGeometricStiffness,
          ForcePerLengthUnit.NewtonPerMeter);
      }

      if (res.ModalMass != 0) {
        _modalMass = new Mass(res.ModalMass, MassUnit.Kilogram);
      }

      if (res.ModalStiffness != 0) {
        _modalStiffness = new ForcePerLength(res.ModalStiffness, ForcePerLengthUnit.NewtonPerMeter);
      }

      if (res.Mode != 0) {
        _mode = res.Mode;
      }

      _totalLoad = new ReactionForce(res.TotalLoad);
      _totalReaction = new ReactionForce(res.TotalReaction);
      _calculated = true;
    }
  }
}
