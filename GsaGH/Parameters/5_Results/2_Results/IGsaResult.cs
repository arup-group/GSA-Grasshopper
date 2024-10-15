using System.Collections.Generic;

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
  /// <para>All result values from the <see href="https://docs.oasys-software.com/structural/gsa/references/dotnet-api/introduction.html">.NET API</see> have been wrapped in <see href="https://docs.oasys-software.com/structural/gsa/references/gsagh/gsagh-unitnumber-parameter.html">Unit Number</see> and can be converted into different measures as you work. The Result parameter caches the result values.</para>
  /// </summary>
  public interface IGsaResult {
    public int CaseId { get; }
    public string CaseName { get; }
    public GsaModel Model { get; }
    public List<int> SelectedPermutationIds { get; }
    public CaseType CaseType { get; }
  }
}
