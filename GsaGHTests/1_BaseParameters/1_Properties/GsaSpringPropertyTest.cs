using System.Collections.Generic;
using System.Drawing;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using Xunit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaSpringPropertyTest {
    [Fact]
    public void DuplicateAxialTest() {
      var original = new GsaSpringProperty {
        ApiProperty = new AxialSpringProperty() {
          Colour = Color.Yellow,
          DampingRatio = 0.1,
          Name = "Name",
          Stiffness = 1.2
        }
      };

      var duplicate = new GsaSpringProperty(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiProperty = new AxialSpringProperty() {
        Colour = Color.Red,
        DampingRatio = 0.2,
        Name = "Name2",
        Stiffness = 2.1
      };

      Assert.Equal(0, original.Id);
      Assert.False(original.IsReferencedById);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), original.ApiProperty.Colour);
      Assert.Equal(0.1, original.ApiProperty.DampingRatio, 8);
      Assert.Equal("Name", original.ApiProperty.Name);
      Assert.Equal(1.2, ((AxialSpringProperty)original.ApiProperty).Stiffness, 8);
    }

    [Fact]
    public void DuplicateTorsionalTest() {
      var original = new GsaSpringProperty {
        ApiProperty = new TorsionalSpringProperty() {
          Colour = Color.Yellow,
          DampingRatio = 0.1,
          Name = "Name",
          Stiffness = 1.2
        }
      };

      var duplicate = new GsaSpringProperty(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiProperty = new TorsionalSpringProperty() {
        Colour = Color.Red,
        DampingRatio = 0.2,
        Name = "Name2",
        Stiffness = 2.1
      };

      Assert.Equal(0, original.Id);
      Assert.False(original.IsReferencedById);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), original.ApiProperty.Colour);
      Assert.Equal(0.1, original.ApiProperty.DampingRatio, 8);
      Assert.Equal("Name", original.ApiProperty.Name);
      Assert.Equal(1.2, ((TorsionalSpringProperty)original.ApiProperty).Stiffness, 8);
    }

    [Fact]
    public void DuplicateGeneralTest1() {
      var original = new GsaSpringProperty {
        ApiProperty = new GeneralSpringProperty() {
          Colour = Color.Yellow,
          DampingRatio = 0.1,
          Name = "Name",
          StiffnessX = 1.2,
          StiffnessY = 2.2,
          StiffnessZ = 3.2,
          StiffnessXX = 4.2,
          StiffnessYY = 4.2,
          StiffnessZZ = 4.2,
        }
      };

      var duplicate = new GsaSpringProperty(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiProperty = new GeneralSpringProperty() {
        Colour = Color.Red,
        DampingRatio = 0.2,
        Name = "Name2",
        SpringCurveX = 1,
        SpringCurveY = 2,
        SpringCurveZ = 3,
        SpringCurveXX = 4,
        SpringCurveYY = 5,
        SpringCurveZZ = 6
      };

      Assert.Equal(0, original.Id);
      Assert.False(original.IsReferencedById);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), original.ApiProperty.Colour);
      Assert.Equal(0.1, original.ApiProperty.DampingRatio, 8);
      Assert.Equal("Name", original.ApiProperty.Name);
      Assert.Equal(1.2, ((GeneralSpringProperty)original.ApiProperty).StiffnessX);
      Assert.Equal(2.2, ((GeneralSpringProperty)original.ApiProperty).StiffnessY);
      Assert.Equal(3.2, ((GeneralSpringProperty)original.ApiProperty).StiffnessZ);
      Assert.Equal(4.2, ((GeneralSpringProperty)original.ApiProperty).StiffnessXX);
      Assert.Equal(4.2, ((GeneralSpringProperty)original.ApiProperty).StiffnessYY);
      Assert.Equal(4.2, ((GeneralSpringProperty)original.ApiProperty).StiffnessZZ);
    }

    [Fact]
    public void DuplicateGeneralTest2() {
      var original = new GsaSpringProperty {
        ApiProperty = new GeneralSpringProperty() {
          Colour = Color.Yellow,
          DampingRatio = 0.1,
          Name = "Name",
          SpringCurveX = 1,
          SpringCurveY = 2,
          SpringCurveZ = 3,
          SpringCurveXX = 4,
          SpringCurveYY = 5,
          SpringCurveZZ = 6
        }
      };

      var duplicate = new GsaSpringProperty(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiProperty = new GeneralSpringProperty() {
        Colour = Color.Red,
        DampingRatio = 0.2,
        Name = "Name2",
        StiffnessX = 1.2,
        StiffnessY = 2.2,
        StiffnessZ = 3.2,
        StiffnessXX = 4.2,
        StiffnessYY = 4.2,
        StiffnessZZ = 4.2,
      };

      Assert.Equal(0, original.Id);
      Assert.False(original.IsReferencedById);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), original.ApiProperty.Colour);
      Assert.Equal(0.1, original.ApiProperty.DampingRatio, 8);
      Assert.Equal("Name", original.ApiProperty.Name);
      Assert.Equal(1, ((GeneralSpringProperty)original.ApiProperty).SpringCurveX);
      Assert.Equal(2, ((GeneralSpringProperty)original.ApiProperty).SpringCurveY);
      Assert.Equal(3, ((GeneralSpringProperty)original.ApiProperty).SpringCurveZ);
      Assert.Equal(4, ((GeneralSpringProperty)original.ApiProperty).SpringCurveXX);
      Assert.Equal(5, ((GeneralSpringProperty)original.ApiProperty).SpringCurveYY);
      Assert.Equal(6, ((GeneralSpringProperty)original.ApiProperty).SpringCurveZZ);
    }

    [Fact]
    public void DuplicateMatrixTest() {
      var original = new GsaSpringProperty {
        ApiProperty = new MatrixSpringProperty() {
          Colour = Color.Yellow,
          DampingRatio = 0.1,
          Name = "Name",
          SpringMatrix = 2
        }
      };

      var duplicate = new GsaSpringProperty(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiProperty = new MatrixSpringProperty() {
        Colour = Color.Red,
        DampingRatio = 0.2,
        Name = "Name2",
        SpringMatrix = 1
      };

      Assert.Equal(0, original.Id);
      Assert.False(original.IsReferencedById);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), original.ApiProperty.Colour);
      Assert.Equal(0.1, original.ApiProperty.DampingRatio, 8);
      Assert.Equal("Name", original.ApiProperty.Name);
      Assert.Equal(2, ((MatrixSpringProperty)original.ApiProperty).SpringMatrix);
    }

    [Fact]
    public void DuplicateTensionOnlyTest() {
      var original = new GsaSpringProperty {
        ApiProperty = new TensionSpringProperty() {
          Colour = Color.Yellow,
          DampingRatio = 0.1,
          Name = "Name",
          Stiffness = 1.2
        }
      };

      var duplicate = new GsaSpringProperty(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiProperty = new TensionSpringProperty() {
        Colour = Color.Red,
        DampingRatio = 0.2,
        Name = "Name2",
        Stiffness = 2.1
      };

      Assert.Equal(0, original.Id);
      Assert.False(original.IsReferencedById);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), original.ApiProperty.Colour);
      Assert.Equal(0.1, original.ApiProperty.DampingRatio, 8);
      Assert.Equal("Name", original.ApiProperty.Name);
      Assert.Equal(1.2, ((TensionSpringProperty)original.ApiProperty).Stiffness, 8);
    }

    [Fact]
    public void DuplicateCompressionOnlyTest() {
      var original = new GsaSpringProperty {
        ApiProperty = new CompressionSpringProperty() {
          Colour = Color.Yellow,
          DampingRatio = 0.1,
          Name = "Name",
          Stiffness = 1.2
        }
      };

      var duplicate = new GsaSpringProperty(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiProperty = new CompressionSpringProperty() {
        Colour = Color.Red,
        DampingRatio = 0.2,
        Name = "Name2",
        Stiffness = 2.1
      };

      Assert.Equal(0, original.Id);
      Assert.False(original.IsReferencedById);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), original.ApiProperty.Colour);
      Assert.Equal(0.1, original.ApiProperty.DampingRatio, 8);
      Assert.Equal("Name", original.ApiProperty.Name);
      Assert.Equal(1.2, ((CompressionSpringProperty)original.ApiProperty).Stiffness, 8);
    }

    [Fact]
    public void DuplicateConnectorTest() {
      var original = new GsaSpringProperty {
        ApiProperty = new ConnectorSpringProperty() {
          Colour = Color.Yellow,
          DampingRatio = 0.1,
          Name = "Name"
        }
      };

      var duplicate = new GsaSpringProperty(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiProperty = new ConnectorSpringProperty() {
        Colour = Color.Red,
        DampingRatio = 0.2,
        Name = "Name2"
      };

      Assert.Equal(0, original.Id);
      Assert.False(original.IsReferencedById);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), original.ApiProperty.Colour);
      Assert.Equal(0.1, original.ApiProperty.DampingRatio, 8);
      Assert.Equal("Name", original.ApiProperty.Name);
    }

    [Fact]
    public void DuplicateLockupTest() {
      var original = new GsaSpringProperty {
        ApiProperty = new LockupSpringProperty() {
          Colour = Color.Yellow,
          DampingRatio = 0.1,
          Name = "Name",
          Stiffness = 1.2,
          NegativeLockup = 2.1,
          PositiveLockup = 3.1,
        }
      };

      var duplicate = new GsaSpringProperty(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiProperty = new LockupSpringProperty() {
        Colour = Color.Red,
        DampingRatio = 0.2,
        Name = "Name2",
        Stiffness = 0,
        NegativeLockup = 0,
        PositiveLockup = 0,
      };

      Assert.Equal(0, original.Id);
      Assert.False(original.IsReferencedById);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), original.ApiProperty.Colour);
      Assert.Equal(0.1, original.ApiProperty.DampingRatio, 8);
      Assert.Equal("Name", original.ApiProperty.Name);
      Assert.Equal(1.2, ((LockupSpringProperty)original.ApiProperty).Stiffness, 8);
      Assert.Equal(2.1, ((LockupSpringProperty)original.ApiProperty).NegativeLockup, 8);
      Assert.Equal(3.1, ((LockupSpringProperty)original.ApiProperty).PositiveLockup, 8);
    }

    [Fact]
    public void DuplicateGapTest() {
      var original = new GsaSpringProperty {
        ApiProperty = new GapSpringProperty() {
          Colour = Color.Yellow,
          DampingRatio = 0.1,
          Name = "Name",
          Stiffness = 1.2
        }
      };

      var duplicate = new GsaSpringProperty(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiProperty = new GapSpringProperty() {
        Colour = Color.Red,
        DampingRatio = 0.2,
        Name = "Name2",
        Stiffness = 0
      };

      Assert.Equal(0, original.Id);
      Assert.False(original.IsReferencedById);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), original.ApiProperty.Colour);
      Assert.Equal(0.1, original.ApiProperty.DampingRatio, 8);
      Assert.Equal("Name", original.ApiProperty.Name);
      Assert.Equal(1.2, ((GapSpringProperty)original.ApiProperty).Stiffness, 8);
    }

    [Fact]
    public void DuplicateFrictionTest() {
      var original = new GsaSpringProperty {
        ApiProperty = new FrictionSpringProperty() {
          Colour = Color.Yellow,
          DampingRatio = 0.1,
          Name = "Name",
          StiffnessX = 1.2,
          StiffnessY = 2.2,
          StiffnessZ = 3.2,
          FrictionCoefficient = 0.1
        }
      };

      var duplicate = new GsaSpringProperty(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiProperty = new FrictionSpringProperty() {
        Colour = Color.Red,
        DampingRatio = 0.2,
        Name = "Name2",
        StiffnessX = 0,
        StiffnessY = 0,
        StiffnessZ = 0,
        FrictionCoefficient = 0
      };

      Assert.Equal(0, original.Id);
      Assert.False(original.IsReferencedById);
      Assert.Equal(Color.FromArgb(255, 255, 255, 0), original.ApiProperty.Colour);
      Assert.Equal(0.1, original.ApiProperty.DampingRatio, 8);
      Assert.Equal("Name", original.ApiProperty.Name);
      Assert.Equal(1.2, ((FrictionSpringProperty)original.ApiProperty).StiffnessX, 8);
      Assert.Equal(2.2, ((FrictionSpringProperty)original.ApiProperty).StiffnessY, 8);
      Assert.Equal(3.2, ((FrictionSpringProperty)original.ApiProperty).StiffnessZ, 8);
      Assert.Equal(0.1, ((FrictionSpringProperty)original.ApiProperty).FrictionCoefficient, 8);
    }

    [Fact]
    public void DuplicateReferenceTest() {
      var original = new GsaSpringProperty(4);
      var duplicate = new GsaSpringProperty(original);
      Assert.Equal(4, duplicate.Id);
      Assert.True(duplicate.IsReferencedById);
    }

    [Fact]
    public void DuplicateReferenceTest2() {
      var original = new GsaSpringProperty(4);
      var duplicate = new GsaSpringProperty(original);
      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });
    }
  }
}
