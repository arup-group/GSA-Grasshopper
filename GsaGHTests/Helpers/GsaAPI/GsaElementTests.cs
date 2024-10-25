using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.InteropServices;

using GsaAPI;

using Xunit;

namespace GsaGHTests.Helpers.GsaAPITests {
  [Collection("GrasshopperFixture collection")]
  public class GsaElementTests {
    [Fact]
    public void LoadPanelTypeIsSetToMinusThousand() {
      Assert.Equal(-1000, GSAElement.LoadPanelType);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void IsLoadPanelShouldBeSetCorrectlyIfLoadPanelIsSet(bool isLoadPanel) {
      GSAElement gsaElement = null;
      gsaElement = isLoadPanel ? new GSAElement(new LoadPanelElement()) : new GSAElement(new Element());

      Assert.Equal(isLoadPanel, gsaElement.IsLoadPanel);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GroupsShouldDefaultToOneIfLoadPanelIsSet(bool isLoadPanel) {
      int expectedValue = 1;
      GSAElement gsaElement = null;
      gsaElement = isLoadPanel ? new GSAElement(new LoadPanelElement()) : new GSAElement(new Element());

      Assert.Equal(expectedValue, gsaElement.Group);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NameShouldBeSetCorrectlyIfLoadPanelIsSet(bool isLoadPanel) {
      string expectedValue = $"example1 + {isLoadPanel}";
      GSAElement gsaElement = isLoadPanel ? new GSAElement(new LoadPanelElement() {
        Name = expectedValue,
      }) : new GSAElement(new Element() {
        Name = expectedValue,
      });

      Assert.Equal(expectedValue, isLoadPanel ? gsaElement.LoadPanelElement.Name : gsaElement.Element.Name);
      Assert.Equal(expectedValue, gsaElement.Name);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PropertyShouldBeSetCorrectlyIfLoadPanelIsSet(bool isLoadPanel) {
      int expectedValue1 = isLoadPanel ? 1 : 0;
      GSAElement gsaElement = isLoadPanel ? new GSAElement(new LoadPanelElement() {
        Property = expectedValue1,
      }) : new GSAElement(new Element() {
        Property = expectedValue1,
      });

      Assert.Equal(expectedValue1, isLoadPanel ? gsaElement.LoadPanelElement.Property : gsaElement.Element.Property);
      Assert.Equal(expectedValue1, gsaElement.Property);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GroupShouldBeSetCorrectlyIfLoadPanelIsSet(bool isLoadPanel) {
      int expectedValue = isLoadPanel ? 11 : 22;
      GSAElement gsaElement = isLoadPanel ? new GSAElement(new LoadPanelElement() {
        Group = expectedValue,
      }) : new GSAElement(new Element() {
        Group = expectedValue,
      });

      Assert.Equal(expectedValue, isLoadPanel ? gsaElement.LoadPanelElement.Group : gsaElement.Element.Group);
      Assert.Equal(expectedValue, gsaElement.Group);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ParentMemberShouldBeSetCorrectlyIfLoadPanelIsSet(bool isLoadPanel) {
      ParentMember expectedValue = isLoadPanel ? new ParentMember(1, 1) : new ParentMember(2, 2);
      GSAElement gsaElement = isLoadPanel ? new GSAElement(new LoadPanelElement() {
        ParentMember = expectedValue,
      }) : new GSAElement(new Element() {
        ParentMember = expectedValue,
      });

      Assert.Equal(expectedValue.Replica,
        isLoadPanel ? gsaElement.LoadPanelElement.ParentMember.Replica : gsaElement.Element.ParentMember.Replica);
      Assert.Equal(expectedValue.Member,
        isLoadPanel ? gsaElement.LoadPanelElement.ParentMember.Member : gsaElement.Element.ParentMember.Member);
      Assert.Equal(expectedValue.Member, gsaElement.ParentMember.Member);
      Assert.Equal(expectedValue.Replica, gsaElement.ParentMember.Replica);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void IsDummyShouldBeSetCorrectlyIfLoadPanelIsSet(bool isLoadPanel) {
      bool expectedValue = isLoadPanel;
      GSAElement gsaElement = isLoadPanel ? new GSAElement(new LoadPanelElement() {
        IsDummy = expectedValue,
      }) : new GSAElement(new Element() {
        IsDummy = expectedValue,
      });

      Assert.Equal(expectedValue, isLoadPanel ? gsaElement.LoadPanelElement.IsDummy : gsaElement.Element.IsDummy);
      Assert.Equal(expectedValue, gsaElement.IsDummy);
    }

    [Fact]
    public void OffsetShouldSetTheValueCorrectlyIfElement() {
      var expectedValue = new {
        X1 = 1,
        X2 = 1,
        Y = 1,
        Z = 1,
      };
      var element = new Element() {
        Offset = {
          X1 = expectedValue.X1,
          X2 = expectedValue.X2,
          Y = expectedValue.Y,
          Z = expectedValue.Z,
        },
      };
      var gsaElement = new GSAElement(element);

      Assert.Equal(expectedValue.X1, gsaElement.Element.Offset.X1);
      Assert.Equal(expectedValue.X2, gsaElement.Element.Offset.X2);
      Assert.Equal(expectedValue.Y, gsaElement.Element.Offset.Y);
      Assert.Equal(expectedValue.Z, gsaElement.Element.Offset.Z);
      Assert.Equal(expectedValue.X1, gsaElement.Offset.X1);
      Assert.Equal(expectedValue.X2, gsaElement.Offset.X2);
      Assert.Equal(expectedValue.Y, gsaElement.Offset.Y);
      Assert.Equal(expectedValue.Z, gsaElement.Offset.Z);
    }

    [Fact]
    public void OffsetShouldThrowErrorIfLoadPanel() {
      var element = new LoadPanelElement() { };
      var gsaElement = new GSAElement(element);
      Assert.Throws<ArgumentException>(() => gsaElement.Offset = null);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OrientationAngleShouldBeSetCorrectlyIfLoadPanelIsSet(bool isLoadPanel) {
      double expectedValue = isLoadPanel ? 1.1d : 2.2d;
      GSAElement gsaElement = isLoadPanel ? new GSAElement(new LoadPanelElement() {
        OrientationAngle = expectedValue,
      }) : new GSAElement(new Element() {
        OrientationAngle = expectedValue,
      });

      Assert.Equal(expectedValue,
        isLoadPanel ? gsaElement.LoadPanelElement.OrientationAngle : gsaElement.Element.OrientationAngle);
      Assert.Equal(expectedValue, gsaElement.OrientationAngle);
    }

    [Fact]
    public void OrientationNodeShouldSetValueCorrectlyIfElement() {
      var element = new Element();
      var gsaElement = new GSAElement(element) {
        OrientationNode = 1,
      };

      Assert.Equal(1, gsaElement.Element.OrientationNode);
      Assert.Equal(1, gsaElement.OrientationNode);
    }

    [Fact]
    public void OrientationNodeShouldThrowExcepionIfLoadPanel() {
      var element = new LoadPanelElement();
      var gsaElement = new GSAElement(element);
      Assert.Throws<ArgumentException>(() => gsaElement.OrientationNode = 1);
    }

    [Fact]
    public void TopologyShouldBeSetCorrectlyIfElement() {
      var expectedValue = new ReadOnlyCollection<int>(new List<int>() {
        111,
      });
      var gsaElement = new GSAElement(new Element() {
        Topology = expectedValue,
      });

      Assert.Equal(expectedValue[0], gsaElement.Element.Topology[0]);
      Assert.Equal(expectedValue[0], gsaElement.Topology[0]);
    }

    [Fact]
    public void SettingTopologyShouldThrowAnErrorIfLoadPanel() {
      Assert.ThrowsAny<SEHException>(() => new GSAElement(new LoadPanelElement() {
        Topology = new ReadOnlyCollection<int>(new List<int>(1)),
      }));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TypeShouldBeSetCorrectlyIfLoadPanelIsSet(bool isLoadPanel) {
      ElementType expectedValue = isLoadPanel ? (ElementType)GSAElement.LoadPanelType : ElementType.BEAM;
      GSAElement gsaElement = isLoadPanel ? new GSAElement(
        new LoadPanelElement() { // there is no type for loadPanel - we need to convert our static fileld into type!!!
        }) : new GSAElement(new Element()) {
        Type = expectedValue,
      };

      Assert.Equal(expectedValue, gsaElement.Type);
      Assert.Equal(expectedValue, isLoadPanel ? gsaElement.Type : gsaElement.Element.Type);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ColorShouldBeSetCorrectlyIfLoadPanelIsSet(bool isLoadPanel) {
      ValueType expectedValue = isLoadPanel ? Color.FromArgb(255, 240, 248, 255) : Color.FromArgb(255, 0, 255, 255);
      GSAElement gsaElement = isLoadPanel ? new GSAElement(new LoadPanelElement() {
        Colour = expectedValue,
      }) : new GSAElement(new Element() {
        Colour = expectedValue,
      });

      Assert.Equal(expectedValue, isLoadPanel ? gsaElement.LoadPanelElement.Colour : gsaElement.Element.Colour);
      Assert.Equal(expectedValue, gsaElement.Colour);
    }

    [Fact]
    public void SetAndGetReleaseShouldReturnValidValuesIfElement() {
      int topology = 1;
      var gsaElement = new GSAElement(new Element());
      gsaElement.SetRelease(topology, new Bool6(true, false, true, false, true, true));
      Bool6 actualResult = gsaElement.Release(topology);
      Assert.True(actualResult.X);
      Assert.False(actualResult.XX);
      Assert.False(actualResult.Y);
      Assert.True(actualResult.YY);
      Assert.True(actualResult.Z);
      Assert.True(actualResult.ZZ);
    }

    [Fact]
    public void SetReleaseShouldThrowAnErrorIfLoadPanel() {
      var gsaElement = new GSAElement(new LoadPanelElement());
      Assert.Throws<ArgumentException>(() => gsaElement.SetRelease(1, new Bool6(true, true, true, true, true, true)));
    }

    [Fact]
    public void GetReleaseShouldThrowAnErrorIfLoadPanel() {
      var gsaElement = new GSAElement(new LoadPanelElement());
      Assert.Throws<ArgumentException>(() => gsaElement.Release(1));
    }

    [Fact]
    public void SetAndGetEndReleaseShouldReturnValidValuesIfElement() {
      int topology = 1;
      var gsaElement = new GSAElement(new Element());
      gsaElement.SetEndRelease(topology, new EndRelease(new Bool6(true, false, true, false, false, true)));
      EndRelease actualResult = gsaElement.GetEndRelease(topology);
      Assert.True(actualResult.Releases.X);
      Assert.False(actualResult.Releases.XX);
      Assert.False(actualResult.Releases.Y);
      Assert.False(actualResult.Releases.YY);
      Assert.True(actualResult.Releases.Z);
      Assert.True(actualResult.Releases.ZZ);
    }

    [Fact]
    public void SetEndReleaseShouldThrowAnErrorIfLoadPanel() {
      var gsaElement = new GSAElement(new LoadPanelElement());
      Assert.Throws<ArgumentException>(()
        => gsaElement.SetEndRelease(1, new EndRelease(new Bool6(true, false, true, false, false, true))));
    }

    [Fact]
    public void GetEndReleaseShouldThrowAnErrorIfLoadPanel() {
      var gsaElement = new GSAElement(new LoadPanelElement());
      Assert.Throws<ArgumentException>(() => gsaElement.GetEndRelease(1));
    }

    [Fact]
    public void TypeAsStringShouldReturnCorrectStringIfElement() {
      var gsaElement = new GSAElement(new Element());
      Assert.Equal(gsaElement.Element.TypeAsString(), gsaElement.TypeAsString());
    }

    [Fact]
    public void TypeAsStringShouldReturnCorrectStringIfLoadPanel() {
      var gsaElement = new GSAElement(new LoadPanelElement());
      Assert.Equal("Load Panel", gsaElement.TypeAsString());
    }
  }
}
