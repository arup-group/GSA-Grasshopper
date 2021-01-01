using System;
using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
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
            var comp = new GhSA.Components.CreateOffset();
            comp.CreateAttributes();

            // set input data
            // instantiate new panel
            var panel = new Grasshopper.Kernel.Special.GH_Panel();
            panel.CreateAttributes();
            // set panel data as usertext
            panel.UserText = "0.5";
            // add panel to component input
            comp.Params.Input[0].AddSource(panel);

            // instantiate new GH_Number param
            var num = new Grasshopper.Kernel.Parameters.Param_Number();
            num.CreateAttributes();
            // set persistent data
            num.PersistentData.Append(new Grasshopper.Kernel.Types.GH_Number(-0.75));
            // add num to component input
            comp.Params.Input[1].AddSource(num);

            // instantiate new GH_Number param
            var num2 = new Grasshopper.Kernel.Parameters.Param_Number();
            num2.CreateAttributes();
            // set persistent data
            num2.PersistentData.Append(new Grasshopper.Kernel.Types.GH_Number(1.99));
            // add num to component input
            comp.Params.Input[2].AddSource(num2);

            // instantiate new GH_Number param
            var num3 = new Grasshopper.Kernel.Parameters.Param_Number();
            num3.CreateAttributes();
            // set persistent data
            num3.PersistentData.Append(new Grasshopper.Kernel.Types.GH_Number(0.99));
            // add num to component input
            comp.Params.Input[3].AddSource(num3);

            // run component calculation
            comp.ExpireSolution(true);

            // read output data
            comp.Params.Output[0].CollectData();
            // set output data to Gsa-goo type
            GsaOffsetGoo output = (GsaOffsetGoo)comp.Params.Output[0].VolatileData.get_Branch(0)[0];

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