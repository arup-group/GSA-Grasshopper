using System;
using NUnit.Framework;
using GhSA;
using GhSA.Parameters;
using Rhino.Geometry;
using Rhino;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;

namespace ComponentsTest
{
    public class Component
    {
        public static void SetInput(GH_Component component, string text_input, int index = 0)
        {
            // instantiate new GH_String param
            var input = new Param_String();
            input.CreateAttributes();
            // set persistent data
            input.PersistentData.Append(new GH_String(text_input));
            // add num to component input
            component.Params.Input[index].AddSource(input);
        }

        public static void SetInput(GH_Component component, double number_input, int index = 0)
        {
            // instantiate new GH_Number param
            var input = new Param_Number();
            input.CreateAttributes();
            // set persistent data
            input.PersistentData.Append(new GH_Number(number_input));
            // add num to component input
            component.Params.Input[index].AddSource(input);
        }

        public static void SetInput(GH_Component component, object generic_input, int index = 0)
        {
            // instantiate new GH_Generic param
            var input = new Param_GenericObject();
            input.CreateAttributes();
            // set persistent data
            input.PersistentData.Append(new GH_ObjectWrapper(generic_input));
            // add num to component input
            component.Params.Input[index].AddSource(input);
        }

        public static void SetInput(GH_Component component, Point3d point_input, int index = 0)
        {
            // instantiate new GH_Number param
            var input = new Param_Point();
            input.CreateAttributes();

            // set persistent data
            input.PersistentData.Append(new GH_Point(point_input));
            // add num to component input
            component.Params.Input[index].AddSource(input);
        }

        public static void SetInput(GH_Component component, Curve curve_input, int index = 0)
        {
            // instantiate new GH_Number param
            var input = new Param_Curve();
            input.CreateAttributes();

            // set persistent data
            input.PersistentData.Append(new GH_Curve(curve_input));
            // add num to component input
            component.Params.Input[index].AddSource(input);
        }

        public static void SetInput(GH_Component component, Brep brep_input, int index = 0)
        {
            // instantiate new GH_Number param
            var input = new Param_Brep();
            input.CreateAttributes();

            // set persistent data
            input.PersistentData.Append(new GH_Brep(brep_input));
            // add num to component input
            component.Params.Input[index].AddSource(input);
        }

        public static void SetInput(GH_Component component, Mesh mesh_input, int index = 0)
        {
            // instantiate new GH_Number param
            var input = new Param_Mesh();
            input.CreateAttributes();

            // set persistent data
            input.PersistentData.Append(new GH_Mesh(mesh_input));
            // add num to component input
            component.Params.Input[index].AddSource(input);
        }

        public static object GetOutput(GH_Component component, int index = 0)
        {
            if (component.Params.Output[index].VolatileDataCount == 0)
            {
                // run component calculation
                component.ExpireSolution(true);

                // read output data
                component.Params.Output[index].CollectData();
            }

            // set output data to Gsa-goo type
            return component.Params.Output[index].VolatileData.get_Branch(0)[0];
        }

        public static object GetOutput(GH_Component component, int index, int branch = 0, int item = 0)
        {
            if(component.Params.Output[index].VolatileDataCount == 0)
            {
                // run component calculation
                component.ExpireSolution(true);

                // read output data
                component.Params.Output[index].CollectData();
            }

            // set output data to Gsa-goo type
            return component.Params.Output[index].VolatileData.get_Branch(branch)[item];
        }
    }
}