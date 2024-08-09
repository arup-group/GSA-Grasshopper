using System;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  public class ModelUnits : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("82d4a7c0-802e-4ad6-ba4b-b0a68f7191bb");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ModelUnits;

    public ModelUnits() : base("Model Units", "Units",
      "Get or set the units used by GSA when opening this GSA Model",
      CategoryName.Name(), SubCategoryName.Cat0()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "Existing GSA model to get or set units for.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Acceleration", "Acc", "Set Acceleration Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Angle", "Ang", "Set Angle Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Energy", "E", "Set Energy Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Force", "F", "Set Force Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Geometry", "LL", "Set 'Length - large' Unit used for Node Geometry for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Property Dimension", "LP", "Set 'Length - sections' Unit used for Property Dimensions for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Displacement", "LS", "Set 'Length - small' Unit used for Displacement Results for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Mass", "M", "Set Mass Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Stress", "S", "Set Stress Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Strain", "Sn", "Set Strain Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Temperature", "T", "Set Temperature Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Long", "TL", "Set Time Long Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Medium", "TM", "Set Time Medium Unitfor this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Short", "TS", "Set Time Short Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Velocity", "V", "Set Velocity Unit for this GSA Model",
        GH_ParamAccess.item);
      for (int i = 1; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
      pManager.AddTextParameter("Acceleration", "Acc", "Get Acceleration Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Angle", "Ang", "Get Angle Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Energy", "E", "Get Energy Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Force", "F", "Get Force Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Geometry", "LL", "Get 'Length - large' Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Property Dimension", "LP", "Get 'Length - sections' Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Displacement", "LS", "Get 'Length - small' Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Mass", "M", "Get Mass Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Stress", "S", "Get Stress Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Strain", "Sn", "Get Strain Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Temperature", "T", "Get Temperature Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Long", "TL", "Get Time Long Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Medium", "TM", "Get Time Medium Unitfor this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Short", "TS", "Get Time Short Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Velocity", "V", "Get Velocity Unit for this GSA Model",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo gooModel = null;
      GsaModel model = null;
      int i = 0;
      if (da.GetData(i++, ref gooModel)) {
        model = gooModel.Value.Clone();
      }
      UiUnits units = model.Units;

      var ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(AccelerationUnit));
        if (result.Item1) {
          units.Acceleration = (AccelerationUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(AngleUnit));
        if (result.Item1) {
          units.Angle = (AngleUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(EnergyUnit));
        if (result.Item1) {
          units.Energy = (EnergyUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(ForceUnit));
        if (result.Item1) {
          units.Force = (ForceUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(LengthUnit));
        if (result.Item1) {
          units.LengthLarge = (LengthUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(LengthUnit));
        if (result.Item1) {
          units.LengthSections = (LengthUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(LengthUnit));
        if (result.Item1) {
          units.LengthSmall = (LengthUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(MassUnit));
        if (result.Item1) {
          units.Mass = (MassUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(StressUnit));
        if (result.Item1) {
          units.Stress = (StressUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(StrainUnit));
        if (result.Item1) {
          units.Strain = (StrainUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(TemperatureUnit));
        if (result.Item1) {
          units.Temperature = (TemperatureUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(TimeUnit));
        if (result.Item1) {
          units.TimeLong = (TimeUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(TimeUnit));
        if (result.Item1) {
          units.TimeMedium = (TimeUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(TimeUnit));
        if (result.Item1) {
          units.TimeShort = (TimeUnit)result.Item2;
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        Tuple<bool, Enum> result = ParseUnit(ghString, typeof(VelocityUnit));
        if (result.Item1) {
          units.Velocity = (VelocityUnit)result.Item2;
        }
      }

      i = 0;

      da.SetData(i++, new GsaModelGoo(new GsaModel(model.ApiModel)));
      da.SetData(i++, units.Acceleration.ToString());
      da.SetData(i++, units.Angle.ToString());
      da.SetData(i++, units.Energy.ToString());
      da.SetData(i++, units.Force.ToString());
      da.SetData(i++, units.LengthLarge.ToString());
      da.SetData(i++, units.LengthSections.ToString());
      da.SetData(i++, units.LengthSmall.ToString());
      da.SetData(i++, units.Mass.ToString());
      da.SetData(i++, units.Stress.ToString());
      da.SetData(i++, units.Strain.ToString());
      da.SetData(i++, units.Temperature.ToString());
      da.SetData(i++, units.TimeLong.ToString());
      da.SetData(i++, units.TimeMedium.ToString());
      da.SetData(i++, units.TimeShort.ToString());
      da.SetData(i++, units.Velocity.ToString());

      this.AddRuntimeRemark(
        "This component can be used to set the default units when the model is saved and opening in GSA. " +
        Environment.NewLine + "To change the default units used inside Grasshopper go to the Oasys menu -> Oasys Units.");
    }

    private Tuple<bool, Enum> ParseUnit(GH_String ghString, Type type) {
      if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
        try {
          var unit = (Enum)Enum.Parse(type, txt, ignoreCase: true);
          return Tuple.Create(true, unit);
        } catch (ArgumentException) {
          string[] names = Enum.GetNames(type);
          this.AddRuntimeError($"Unable to convert '{txt}' to a known unit. Accepted inputs are:"
            + Environment.NewLine + string.Join(Environment.NewLine, names));
        }
      }
      return Tuple.Create<bool, Enum>(false, null);
    }
  }
}
