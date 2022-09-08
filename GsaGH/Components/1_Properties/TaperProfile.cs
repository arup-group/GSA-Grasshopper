using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Section
  /// </summary>
  public class TaperProfile : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("fd6dd254-c16f-4970-a447-a9b258d116ef");
    public TaperProfile()
      : base("Taper Profile", "Taper", "Create a Profile that tapers along its length from start and end profiles",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.TaperProfile;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

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
      //start
      GH_String gh_profile = new GH_String();
      DA.GetData(0, ref gh_profile);
      GH_Convert.ToString(gh_profile, out string start, GH_Conversion.Both);

      //end
      gh_profile = new GH_String();
      DA.GetData(1, ref gh_profile);
      GH_Convert.ToString(gh_profile, out string end, GH_Conversion.Both);

      string[] startParts = start.Split(' ');
      string[] endParts = end.Split(' ');

      if (startParts[0] == "STD" & endParts[0] == "STD")
      {
        if (startParts[1] == endParts[1])
        {
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

