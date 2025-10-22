using System;

namespace GsaGH.Components.Helpers {
  internal static class ResultNotes {
    internal static string NoteNodeResults =
      Environment.NewLine + "DataTree organised as { `CaseID` ; `Permutation` } " +
      Environment.NewLine + "fx. `{1;2}` is Case 1, Permutation 2, where each branch " +
      Environment.NewLine + "contains a list matching the `NodeID`s in the ID output.";

    internal static string Note1dResults =
      Environment.NewLine + "DataTree organised as { `CaseID` ; `Permutation` ; `ElementID` } " +
      Environment.NewLine + "fx. `{1;2;3}` is Case 1, Permutation 2, Element 3, where each " +
      Environment.NewLine + "branch contains a list of results per element position.";

    internal static string Note2dForceResults
      = Note2dResults + Environment.NewLine + "Element results are NOT averaged at nodes";

    internal static string Note2dStressResults = Note2dResults + Environment.NewLine
      + "+ve in-plane stresses: tensile(ie. + ve direct strain)." + Environment.NewLine
      + "+ve bending stress gives rise to tension on the top surface." + Environment.NewLine
      + "+ve shear stresses: +ve shear strain.";

    internal static string Note3dStressResults
      = Note2dResults + Environment.NewLine + "+ve stresses: tensile (ie. +ve direct strain)";

    internal static string Note2dResults = Environment.NewLine
      + "DataTree organised as { `CaseID` ; `Permutation` ; `ElementID` } " + Environment.NewLine
      + "fx. `{1;2;3}` is Case 1, Permutation 2, Element 3, where each " + Environment.NewLine
      + "branch contains a list of results in the following order:" + Environment.NewLine
      + "`Vertex(1)`, `Vertex(2)`, ..., `Vertex(i)`, `Centre`";

    internal static string NoteAssemblyResults = Environment.NewLine
      + "DataTree organised as { `CaseID` ; `Permutation` ; `AssemblyID` } " + Environment.NewLine
      + "fx. `{1;2;3}` is Case 1, Permutation 2, Assembly 3, where each " + Environment.NewLine
      + "branch contains a list of results per assembly position/storey.";
  }
}
