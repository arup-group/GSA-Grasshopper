using System;
using NUnit.Framework;
using GsaGH;
using GsaGH.Parameters;
using Rhino.Geometry;
using Rhino;
using Grasshopper;
using GsaAPI;

namespace ComponentsTest
{
  public class OffsetTests
  {
    [TestCase]
    public void CreateOffsetComponentTest()
    {
      // create the component
      var comp = new GsaGH.Components.CreateOffset();
      comp.CreateAttributes();

      Component.SetInput(comp, "0.5");
      Component.SetInput(comp, -0.75, 1);
      Component.SetInput(comp, 1.99, 2);
      Component.SetInput(comp, 0.99, 3);

      // set output data to Gsa-goo type
      GsaOffsetGoo output = (GsaOffsetGoo)Component.GetOutput(comp);

      // cast from -goo to Gsa-GH data type
      GsaOffset offset = new GsaOffset();
      output.CastTo(ref offset);

      Assert.AreEqual(0.5, offset.X1);
      Assert.AreEqual(-0.75, offset.X2);
      Assert.AreEqual(1.99, offset.Y);
      Assert.AreEqual(0.99, offset.Z);
    }
  }
}