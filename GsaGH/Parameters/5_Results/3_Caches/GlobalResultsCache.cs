using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using MassUnit = OasysUnits.Units.MassUnit;

namespace GsaGH.Parameters.Results {
  public class GlobalResultsCache : IGlobalResultsCache {
    public IApiResult ApiResult { get; private set; }
    public IEffectiveInertia EffectiveInertia {
      get {
        if (_effectiveInertia == null) {
          Calculate();
        }

        return _effectiveInertia;
      }
    }
    public IEffectiveMass EffectiveMass {
      get {
        if (_effectiveMass == null) {
          Calculate();
        }

        return _effectiveMass;
      }
    }

    public double? Eigenvalue {
      get {
        if (_eigenvalue == null) {
          Calculate();
        }

        return _eigenvalue;
      }
    }

    public Frequency Frequency {
      get {
        if (_frequency == null) {
          Calculate();
        }

        return _frequency;
      }
    }
    public Ratio LoadFactor {
      get {
        if (_loadFactor == null) {
          Calculate();
        }

        return _loadFactor;
      }
    }
    public ForcePerLength ModalGeometricStiffness {
      get {
        if (_modalGeometricStiffness == null) {
          Calculate();
        }

        return _modalGeometricStiffness;
      }
    }
    public Mass ModalMass {
      get {
        if (_modalMass == null) {
          Calculate();
        }

        return _modalMass;
      }
    }
    public ForcePerLength ModalStiffness {
      get {
        if (_modalStiffness == null) {
          Calculate();
        }

        return _modalStiffness;
      }
    }
    public int? Mode {
      get {
        if (_mode == null) {
          Calculate();
        }

        return _mode;
      }
    }
    public IInternalForce TotalLoad {
      get {
        if (_totalLoad == null) {
          Calculate();
        }

        return _totalLoad;
      }
    }
    public IInternalForce TotalReaction {
      get {
        if (_totalReaction == null) {
          Calculate();
        }

        return _totalReaction;
      }
    }

    private IEffectiveInertia _effectiveInertia;
    private IEffectiveMass _effectiveMass;
    private double? _eigenvalue;
    private Frequency _frequency;
    private Ratio _loadFactor;
    private ForcePerLength _modalGeometricStiffness;
    private Mass _modalMass;
    private ForcePerLength _modalStiffness;
    private int? _mode;
    private IInternalForce _totalLoad;
    private IInternalForce _totalReaction;

    internal GlobalResultsCache(AnalysisCaseResult result) {
      ApiResult = new ApiResult(result);
    }

    // GSA-6747 CombinationCaseResult does not contain global results
    //internal GlobalResultsCache(CombinationCaseResult result) {
    //  ApiResult = new ApiResult(result);
    //}

    private void Calculate() {
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
        _modalGeometricStiffness = new ForcePerLength(res.ModalGeometricStiffness,
          ForcePerLengthUnit.NewtonPerMeter);
      }

      if (res.Mode != 0) {
        _mode = res.Mode;
      }

      _totalLoad = new ReactionForce(res.TotalLoad);
      _totalReaction = new ReactionForce(res.TotalReaction);
    }
  }
}
