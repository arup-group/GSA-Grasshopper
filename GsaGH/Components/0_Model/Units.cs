﻿using System;
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
  public class Units : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("2eb77ab2-6ea1-4899-ab95-6ae8a36d9d23");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Units;

    public Units() : base("Model Units", "Units",
      "Get or set the units used by GSA when opening this Model", CategoryName.Name(),
      SubCategoryName.Cat0()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "Existing GSA model to get or set units for.", GH_ParamAccess.item);
      pManager.AddTextParameter("Acceleration", "Acc", "Set Acceleration Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Angle", "Ang", "Set Angle Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Energy", "E", "Set Energy Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Force", "F", "Set Force Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Geometry", "LL",
        "Set 'Length - large' Unit used for Node Geometry for this GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Property Dimension", "LP",
        "Set 'Length - sections' Unit used for Property Dimensions for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Displacement", "LS",
        "Set 'Length - small' Unit used for Displacement Results for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Mass", "M", "Set Mass Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Stress", "S", "Set Stress Unit for this GSA Model",
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
      pManager.AddTextParameter("Property Dimension", "LP",
        "Get 'Length - sections' Unit for this GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Displacement", "LS",
        "Get 'Length - small' Unit for this GSA Model", GH_ParamAccess.item);
      pManager.AddTextParameter("Mass", "M", "Get Mass Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Stress", "S", "Get Stress Unit for this GSA Model",
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
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.Acceleration = (AccelerationUnit)Enum.Parse(typeof(AccelerationUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(AngleUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Acceleration Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.Angle = (AngleUnit)Enum.Parse(typeof(AngleUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(AngleUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Angle Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.Energy = (EnergyUnit)Enum.Parse(typeof(EnergyUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(EnergyUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Energy Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.Force = (ForceUnit)Enum.Parse(typeof(ForceUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(ForceUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Force Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.LengthLarge = (LengthUnit)Enum.Parse(typeof(LengthUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(LengthUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Length Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.LengthSections = (LengthUnit)Enum.Parse(typeof(LengthUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(LengthUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Length Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.LengthSmall = (LengthUnit)Enum.Parse(typeof(LengthUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(LengthUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Length Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.Mass = (MassUnit)Enum.Parse(typeof(MassUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(MassUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Mass Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.Stress = (StressUnit)Enum.Parse(typeof(StressUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(StressUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Stress Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.TimeLong = (TimeUnit)Enum.Parse(typeof(TimeUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(TimeUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Time Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.TimeMedium = (TimeUnit)Enum.Parse(typeof(TimeUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(TimeUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Time Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.TimeShort = (TimeUnit)Enum.Parse(typeof(TimeUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(TimeUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Time Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          try {
            units.Velocity = (VelocityUnit)Enum.Parse(typeof(VelocityUnit), txt, true);
          }
          catch (ArgumentException) {
            string[] names = Enum.GetNames(typeof(VelocityUnit));
            this.AddRuntimeError(
              $"Unable to convert '{txt}' to a known Velocity Unit. Accepted inputs are:"
              + Environment.NewLine + string.Join(Environment.NewLine, names));
          }
        }
      }

      i = 0;

      da.SetData(i++, new GsaModelGoo(new GsaModel(model.Model)));
      da.SetData(i++, units.Acceleration.ToString());
      da.SetData(i++, units.Angle.ToString());
      da.SetData(i++, units.Energy.ToString());
      da.SetData(i++, units.Force.ToString());
      da.SetData(i++, units.LengthLarge.ToString());
      da.SetData(i++, units.LengthSections.ToString());
      da.SetData(i++, units.LengthSmall.ToString());
      da.SetData(i++, units.Mass.ToString());
      da.SetData(i++, units.Stress.ToString());
      da.SetData(i++, units.TimeLong.ToString());
      da.SetData(i++, units.TimeMedium.ToString());
      da.SetData(i++, units.TimeShort.ToString());
      da.SetData(i++, units.Velocity.ToString());

      this.AddRuntimeRemark(
        "This component can be used to set the default units when the model is saved and opening in GSA. "
        + Environment.NewLine
        + "To change the default units used inside Grasshopper go to the Oasys menu -> Oasys Units.");
    }
  }
}
