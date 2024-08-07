using System;
using System.Collections.Generic;
using System.Drawing;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Parameters;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaDiagramParameterTests {
    [Fact]
    public void GsaDiagramArrowheadParameterBakeTests() {
      GraphicDrawResult graphic =
        GsaArrowheadDiagramTests.Element1dUniformLoadDiagramResults();
      var goo = new GsaDiagramGoo(new GsaArrowheadDiagram(graphic.Triangles, 1, Color.Empty));
      var param = new GsaDiagramParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, goo);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Single(doc.Objects);
      doc.Dispose();
    }

    [Fact]
    public void GsaDiagramLineParameterBakeTests() {
      GraphicDrawResult graphic = GsaLineDiagramTests.Element1dMyyDiagramResults();
      var goo = new GsaDiagramGoo(new GsaLineDiagram(graphic.Lines[1], 1, Color.Empty));
      var param = new GsaDiagramParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, goo);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Single(doc.Objects);
      doc.Dispose();
    }

    [Fact]
    public void GsaDiagramVectorParameterBakeTests() {
      var vec = new GsaVectorDiagram(new Point3d(0, 0, 0), new Vector3d(0, 0, -1), true, Color.Teal);
      var goo = new GsaDiagramGoo(vec);
      var param = new GsaDiagramParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, goo);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Single(doc.Objects);
      doc.Dispose();
    }
  }
}
