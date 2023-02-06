using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Section
  /// </summary>
  public class TaperProfile : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("fd6dd254-c16f-4970-a447-a9b258d116ef");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.TaperProfile;

    public TaperProfile() : base("Taper Profile",
      "Taper",
      "Create a Profile that tapers along its length from start and end profiles",
      CategoryName.Name(),
      SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddTextParameter("Profile Start", "Pf1", "Profile at start of element", GH_ParamAccess.item);
      pManager.AddTextParameter("Profile End", "Pf2", "Profile at end of element", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("Tapered Profile", "Pf", "Profile tapering along the length of its element", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // start
      GH_String gh_profile = new GH_String();
      DA.GetData(0, ref gh_profile);
      GH_Convert.ToString(gh_profile, out string start, GH_Conversion.Both);

      // end
      gh_profile = new GH_String();
      DA.GetData(1, ref gh_profile);
      GH_Convert.ToString(gh_profile, out string end, GH_Conversion.Both);

      string[] startParts = start.Split(' ');
      string[] endParts = end.Split(' ');

      if (startParts[0] != endParts[0])
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Start and End Profile types must be similar");
        return;
      }

      if (startParts[0] == "STD" || startParts[0] == "GEO")
      {
        if (startParts[1] == endParts[1])
        {
          if (startParts[0] == "GEO") 
          {
            if (!startParts[1].StartsWith("P"))
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "It is only possible to taper between two Perimeter / bridge profiles");
              return;
            }
            if (startParts.Length != endParts.Length)
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Start and End Profile must contain similar number of points");
              return;
            }
          }

          string taper = startParts[0] + " " + startParts[1];
          for (int i = 2; i < startParts.Length; i++)
          {
            taper = taper + " " + startParts[i];
          }
          taper = taper + " :";
          for (int i = 2; i < endParts.Length; i++)
          {
            taper = taper + " " + endParts[i];
          }
          DA.SetData(0, taper);
          return;
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Start and End Profile types must be similar");
        }
      }
      else
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Profile type must be 'STD'");
      }
    }
  }
}
