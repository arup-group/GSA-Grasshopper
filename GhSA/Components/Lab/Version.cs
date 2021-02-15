using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GhSA.Parameters;
using System.Resources;
using Grasshopper.Documentation;
using System.IO;


namespace GhSA.Components
{
    public class GsaVersion : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("0d64cc30-f24b-4940-97e8-2fb1eb9fef95");
        public GsaVersion()
          : base("GSA Plugin Version", "Version", "Get the version of this plugin.",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { this.Hidden = true; } // sets the initial state of the component to hidden

        public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GsaVersion;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("GSA Plugin Version", "Plugin", "Plugin version", GH_ParamAccess.item);
            pManager.AddTextParameter("Location", "File", "Plugin File Location", GH_ParamAccess.item);
            pManager.AddTextParameter("GSA Location", "GSA", "GSA Folder Location", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_AssemblyInfo gsaplugin = Grasshopper.Instances.ComponentServer.FindAssembly(new Guid("a3b08c32-f7de-4b00-b415-f8b466f05e9f"));

            DA.SetData(0, gsaplugin.Version);
            DA.SetData(1, gsaplugin.Location);
            DA.SetData(2, Util.Gsa.InstallationFolderPath.GetPath);
        }
    }
}

