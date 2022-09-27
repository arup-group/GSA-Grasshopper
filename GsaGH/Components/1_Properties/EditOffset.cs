using System;
using System.Linq;
using Grasshopper.Kernel;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit an Offset and ouput the information
  /// </summary>
  public class EditOffset : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("1e094fcd-8f5f-4047-983c-e0e57a83ae52");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditOffset;

    public EditOffset() : base("Edit Offset",
      "OffsetEdit",
      "Modify GSA Offset or just get information about existing",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Custom UI
    //This region overrides the typical component layout

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      IQuantity quantity = new Length(0, DefaultUnits.LengthUnitGeometry);
      string unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Offset", "Of", "GSA Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]", "X1", "X1 - Start axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]", "X2", "X2 - End axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]", "Y", "Y Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]", "Z", "Z Offset", GH_ParamAccess.item);
      for (int i = 1; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity quantity = new Length(0, DefaultUnits.LengthUnitGeometry);
      string unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Offset", "Of", "GSA Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X1 [" + unitAbbreviation + "]", "X1", "X1 - Start axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset X2 [" + unitAbbreviation + "]", "X2", "X2 - End axial offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Y [" + unitAbbreviation + "]", "Y", "Y Offset", GH_ParamAccess.item);
      pManager.AddGenericParameter("Offset Z [" + unitAbbreviation + "]", "Z", "Z Offset", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaOffset offset = new GsaOffset();
      GsaOffset gsaoffset = new GsaOffset();
      if (DA.GetData(0, ref gsaoffset))
      {
        if (offset != null)
        {
          int inp = 1;
          if (this.Params.Input[inp].SourceCount != 0)
            offset.X1 = (Length)Input.LengthOrRatio(this, DA, inp++, DefaultUnits.LengthUnitGeometry, true);

          if (this.Params.Input[inp].SourceCount != 0)
            offset.X2 = (Length)Input.LengthOrRatio(this, DA, inp++, DefaultUnits.LengthUnitGeometry, true);

          if (this.Params.Input[inp].SourceCount != 0)
            offset.Y = (Length)Input.LengthOrRatio(this, DA, inp++, DefaultUnits.LengthUnitGeometry, true);

          if (this.Params.Input[inp].SourceCount != 0)
            offset.Z = (Length)Input.LengthOrRatio(this, DA, inp++, DefaultUnits.LengthUnitGeometry, true);

          //outputs
          int outp = 0;
          DA.SetData(outp++, new GsaOffsetGoo(offset));

          DA.SetData(outp++, new GH_UnitNumber(offset.X1));
          DA.SetData(outp++, new GH_UnitNumber(offset.X2));
          DA.SetData(outp++, new GH_UnitNumber(offset.Y));
          DA.SetData(outp++, new GH_UnitNumber(offset.Z));
        }
      }
    }
  }
}
