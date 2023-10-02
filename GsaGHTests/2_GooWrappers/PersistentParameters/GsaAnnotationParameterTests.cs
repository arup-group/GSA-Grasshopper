﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;
using GsaGHTests.Parameters;
using OasysGH.Components;
using OasysGH.Parameters;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.GooWrappers {
  [Collection("GrasshopperFixture collection")]
  public class GsaAnnotationParameterTests {
    [Fact]
    public void GsaAnnotationDotParameterBakeTests() {
      var annoDot = new GsaAnnotationGoo(GsaAnnotationDotTests.AnnotationDotMother());
      var param = new GsaAnnotationParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, annoDot);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Single(doc.Objects);
      doc.Dispose();
    }

    [Fact]
    public void GsaAnnotation3dParameterBakeTests() {
      var anno3d = new GsaAnnotationGoo(GsaAnnotation3dTests.Annotation3dMother());
      var param = new GsaAnnotationParameter();
      param.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, anno3d);

      var doc = Rhino.RhinoDoc.CreateHeadless(null);
      var guids = new List<Guid>();
      param.BakeGeometry(doc, guids);
      Assert.NotEmpty(guids);
      Assert.Single(doc.Objects);
      doc.Dispose();
    }
  }
}
