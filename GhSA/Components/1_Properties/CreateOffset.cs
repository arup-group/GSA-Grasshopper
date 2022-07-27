﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Parameters;
using UnitsNet;
using System.Linq;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Offset
  /// </summary>
  public class CreateOffset : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("ba73abd3-cd48-4dd2-9cd1-d89c921dd108");
    public CreateOffset()
      : base("Create Offset", "Offset", "Create GSA Offset",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateOffset;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // length
        //dropdownitems.Add(Enum.GetNames(typeof(UnitsNet.Units.LengthUnit)).ToList());
        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        IQuantity quantity = new Length(0, lengthUnit);
        unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

        first = false;
      }

      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }

    private void UpdateUIFromSelectedItems()
    {
      lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[0]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    #endregion

    #region Input and output
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Measure"
    });
    private bool first = true;
    private UnitsNet.Units.LengthUnit lengthUnit = GsaGH.Units.LengthUnitGeometry;
    string unitAbbreviation;
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity quantity = new Length(0, lengthUnit);
      unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]", "X1", "X1 - Start axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]", "X2", "X2 - End axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]", "Y", "Y Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]", "Z", "Z Offset", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Offset", "Of", "GSA Offset", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaOffset offset = new GsaOffset
      {
        X1 = GetInput.Length(this, DA, 0, lengthUnit, true),
        X2 = GetInput.Length(this, DA, 1, lengthUnit, true),
        Y = GetInput.Length(this, DA, 2, lengthUnit, true),
        Z = GetInput.Length(this, DA, 3, lengthUnit, true)
      };

      DA.SetData(0, new GsaOffsetGoo(offset));
    }
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      GsaGH.Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);

      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      GsaGH.Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
      UpdateUIFromSelectedItems();
      first = false;
      return base.Read(reader);
    }
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    #endregion

    #region IGH_VariableParameterComponent null implementation
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      IQuantity quantity = new Length(0, lengthUnit);
      unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));
      Params.Input[0].Name = "Offset X1 [" + unitAbbreviation + "]";
      Params.Input[1].Name = "Offset X2 [" + unitAbbreviation + "]";
      Params.Input[2].Name = "Offset Y [" + unitAbbreviation + "]";
      Params.Input[3].Name = "Offset Z [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

