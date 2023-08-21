using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsGeneration.Parameters {
  public class ParametersMarkDown {
    public static string ParametersOverview = 
      "# Parameters\n" +
      "\n" +
      "::: warning" +
      "\nGSA-Grasshopper plugin [GsaGH] is pre-release and under active development, including further testing to be undertaken. It is provided \\\"as-is\\\" and you bear the risk of using it. Future versions may contain breaking changes. Any files, results, or other types of output information created using GsaGH should not be relied upon without thorough and independent checking.\n:::\n" +
      "\n" +
      "GsaGH introduces a new set of Grasshopper parameters. Parameters are what is passed from one component's output to another component's input.\n" +
      "![Parameters](https://developer.rhino3d.com/api/grasshopper/media/ParameterKinds.png)\n" +
      "\n" +
      "## Parameters";

    public static string MaterialsText = 
      "## Description\n" +
      "\n" +
      "In GSA, all elements that refer to beam sections, 2D properties or 3D properties require an analysis material. This can either be an explicitly defined material or an implied material from a material grade. The latter are always assume to be elastic isotropic materials.\n" +
      "\n" +
      "With GsaGH you can refer to standard (code defined) materials by referencing the *type* (steel, concrete, etc), the *material number* (ID) and *grade*. \n" +
      "\n" +
      "> **Note**: At present, it is not possible to create a standard material within GsaGH. You can set this up in an empty GSA model and reference this model with GsaGH. Alternatively, it is possible to create custom analysis material with GsaGH.\n" +
      "\n" +
      "### GSA reference\n" +
      "\n" +
      "Standard materials:\n" +
      "\n" +
      "- [Steel](/references/hidr-data-mat-steel.html)\n" +
      "- [Concrete](/references/hidr-data-mat-concrete.html)\n" +
      "- [FRP](/references/hidr-data-mat-frp.html)\n" +
      "- [Aluminium](/references/hidr-data-mat-aluminium.html)\n" +
      "- [Timber](/references/hidr-data-mat-timber.html)\n" +
      "- [Glass](/references/hidr-data-mat-glass.html)\n" +
      "- [Fabric](/references/hidr-data-mat-fabric.html)\n" +
      "\n" +
      "Custom analysis material:\n" +
      "\n" +
      "- [Custom](/references/hidr-data-mat-anal.html)\n";
  }
}
