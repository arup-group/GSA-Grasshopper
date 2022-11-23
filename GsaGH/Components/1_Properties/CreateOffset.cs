using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Offset
  /// </summary>
  public class CreateOffset : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("ba73abd3-cd48-4dd2-9cd1-d89c921dd108");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateOffset;

    public CreateOffset() : base("Create Offset",
      "Offset",
      "Create GSA Offset",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]", "X1", "X1 - Start axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]", "X2", "X2 - End axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]", "Y", "Y Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]", "Z", "Z Offset", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaOffsetParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaOffset offset = new GsaOffset
      {
        X1 = (Length)Input.UnitNumber(this, DA, 0, LengthUnit, true),
        X2 = (Length)Input.UnitNumber(this, DA, 1, LengthUnit, true),
        Y = (Length)Input.UnitNumber(this, DA, 2, LengthUnit, true),
        Z = (Length)Input.UnitNumber(this, DA, 3, LengthUnit, true)
      };

      DA.SetData(0, new GsaOffsetGoo(offset));
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Measure"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);

      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      Params.Input[0].Name = "Offset X1 [" + unitAbbreviation + "]";
      Params.Input[1].Name = "Offset X2 [" + unitAbbreviation + "]";
      Params.Input[2].Name = "Offset Y [" + unitAbbreviation + "]";
      Params.Input[3].Name = "Offset Z [" + unitAbbreviation + "]";
    }
    #endregion
  }
}
