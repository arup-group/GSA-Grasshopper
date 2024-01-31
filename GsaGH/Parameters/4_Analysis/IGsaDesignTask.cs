namespace GsaGH.Parameters {
  /// <summary>
  /// <para>A Design Task is collection of specifications that guide the automated, iterative design or checking of members. A Design Task is analogous to an Analysis Task in that there can be multiple Design Tasks all of which are saved with the model. Design Tasks must be executed to carry out either a design or a check based on the parameters defined in the task. </para>
  /// <para>In Grasshopper, it is only possible to create steel design tasks.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/sbs-steeldesign/">Design Tasks</see> to read more.</para>
  /// </summary>
  public interface IGsaDesignTask {
    GsaList List { get; }
    int Id { get; }
    string Name { get; }
  }
}
