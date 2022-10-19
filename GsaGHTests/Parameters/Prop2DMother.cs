using GsaAPI;

namespace GsaGHTests.Parameters
{
  internal class Prop2DMother
  {

    // this is wrong!
    internal static Prop2D exampleProp1 = new Prop2D
    {
      AxisProperty = 1,
      MaterialGradeProperty = 4,
      MaterialAnalysisProperty = 42,
      MaterialType = MaterialType.GENERIC,
      Name = "mariam",
      Description = "awesome property",
      Type = Property2D_Type.LOAD
    };

    internal static Prop2D exampleProp2 = new Prop2D
    {
      AxisProperty = 0,
      MaterialGradeProperty = 2,
      MaterialAnalysisProperty = 13,
      MaterialType = MaterialType.UNDEF,
      Name = "mariam",
      Description = "awesome property",
      Type = Property2D_Type.SHELL
    };
  }
}
