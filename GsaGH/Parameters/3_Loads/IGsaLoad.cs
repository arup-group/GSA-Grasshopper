using System;

using GsaGH.Parameters.Enums;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>A Load parameter can contain
  /// <see href="https://docs.oasys-software.com/structural/gsa/references/nodalloading-data.html">Node Loads</see>,
  /// <see href="https://docs.oasys-software.com/structural/gsa/references/beamloading-data.html">Beam Loads</see>,
  /// <see href="https://docs.oasys-software.com/structural/gsa/references/2delementloading-data.html">2D Element Loads</see>,
  /// <see href="https://docs.oasys-software.com/structural/gsa/references/gridloading-data.html">Grid Loads</see>, and
  /// <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-gravity.html">Gravity Loads</see>,
  /// </para>
  /// <para>GSA provides a number of different ways to apply loads to a model.</para>
  /// <para>The simplest option is use the <see cref="Components.CreateNodeLoad"/> component to create nodal loading where forces are applied directly to nodes. This is not recommended for 2D and 3D elements. </para>
  /// <para>The next level of loading applies loads to the elements, either use the <see cref="Components.CreateBeamLoad"/> component, or <see cref="Components.CreateFaceLoad"/> component. In the solver these use shape functions to give loading on the nodes compatible with the elements to which the load is applied. </para>
  /// <para>Grid loading is a different type of loading which is applied to a <see cref="GsaGridPlaneSurface"/>. An algorithm then distributes this loading from the grid surface to the surrounding elements. This can be useful for models where floor slabs are not modelled explicitly. </para>
  /// <para>Gravity is the final load type create with the <see cref="Components.CreateGravityLoad"/> component. This is different from the other load types as it is specified as an acceleration (in g). This is normally used to model the dead weight of the structure by specifying a gravity load of −1 × g in the z direction.</para>
  ///
  /// </summary>
  public interface IGsaLoad {
    GsaList ReferenceList { get; }
    ReferenceType ReferenceType { get; }
    Guid RefObjectGuid { get; }
    GsaLoadCase LoadCase { get; }
    int CaseId { get; set; }
    string Name { get; set; }
    IGsaLoad Duplicate();
  }
}
