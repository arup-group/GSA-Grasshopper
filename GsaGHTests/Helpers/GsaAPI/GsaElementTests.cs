using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.InteropServices;

using GsaAPI;

using Xunit;

namespace GsaGHTests.Helpers.GsaAPITests {

  [Collection("GrasshopperFixture collection")]
  public class GsaElementTests_LoadPanel {
    private readonly GSAElement gsaElement;
    private const string expectedName = "name";
    private const int expectedPropertyValue = 1;
    private const int expectedGroupValue = 11;
    private readonly ParentMember expectedParentValue = new ParentMember(11, 11);
    private const bool expectedIsDummyValue = true;
    private const double expectedOrientationAngle = 1.1d;
    private readonly ElementType expectedElementType = (ElementType)GSAElement.LoadPanelType;
    private readonly ValueType expectedColor = Color.FromArgb(255, 240, 248, 255);

    public GsaElementTests_LoadPanel() {
      gsaElement = new GSAElement(new LoadPanelElement()) {
        Name = expectedName,
        Group = expectedGroupValue,
        Property = expectedPropertyValue,
        ParentMember = expectedParentValue,
        IsDummy = expectedIsDummyValue,
        OrientationAngle = expectedOrientationAngle,
        Type = expectedElementType,
        Colour = expectedColor,
      };
    }

    [Fact]
    public void LoadPanelTypeIsSetToMinusThousand() {
      Assert.Equal(-1000, GSAElement.LoadPanelType);
    }

    [Fact]
    public void IsLoadPanelShouldBeSetCorrectly() {
      Assert.True(gsaElement.IsLoadPanel);
    }

    [Fact]
    public void GroupsShouldDefaultToOne() {
      Assert.Equal(1, gsaElement.Group);
    }

    [Fact]
    public void NameShouldBeSetCorrectlyForParent() {
      Assert.Equal(expectedName, gsaElement.Name);
    }

    [Fact]
    public void NameShouldBeSetCorrectlyForChild() {
      Assert.Equal(expectedName, gsaElement.LoadPanelElement.Name);
    }

    [Fact]
    public void PropertyShouldBeSetCorrectlyForParent() {
      Assert.Equal(expectedPropertyValue, gsaElement.Property);
    }

    [Fact]
    public void PropertyShouldBeSetCorrectlyForChild() {
      Assert.Equal(expectedPropertyValue, gsaElement.LoadPanelElement.Property);
    }

    [Fact]
    public void GroupShouldBeSetCorrectlyForParent() {
      Assert.Equal(expectedGroupValue, gsaElement.Group);
    }

    [Fact]
    public void GroupShouldBeSetCorrectlyForChild() {
      Assert.Equal(expectedGroupValue, gsaElement.LoadPanelElement.Group);
    }

    [Fact]
    public void ParentMemberReplicaShouldBeSetCorrectlyForParent() {
      Assert.Equal(expectedParentValue.Replica, gsaElement.ParentMember.Replica);
    }

    [Fact]
    public void ParentMemberMemberShouldBeSetCorrectlyForParent() {
      Assert.Equal(expectedParentValue.Member, gsaElement.ParentMember.Member);
    }

    [Fact]
    public void ParentMemberReplicaShouldBeSetCorrectlyForChild() {
      Assert.Equal(expectedParentValue.Replica, gsaElement.LoadPanelElement.ParentMember.Replica);
    }

    [Fact]
    public void ParentMemberMemberShouldBeSetCorrectlyForChild() {
      Assert.Equal(expectedParentValue.Member, gsaElement.LoadPanelElement.ParentMember.Member);
    }

    [Fact]
    public void IsDummyShouldBeSetCorrectlyForParent() {
      Assert.Equal(expectedIsDummyValue, gsaElement.IsDummy);
    }

    [Fact]
    public void IsDummyShouldBeSetCorrectlyForChild() {
      Assert.Equal(expectedIsDummyValue, gsaElement.LoadPanelElement.IsDummy);
    }

    [Fact]
    public void OffsetShouldThrowError() {
      Assert.Throws<ArgumentException>(() => gsaElement.Offset = null);
    }

    [Fact]
    public void OrientationAngleShouldBeSetCorrectlyForParent() {
      Assert.Equal(expectedOrientationAngle, gsaElement.OrientationAngle);
    }

    [Fact]
    public void OrientationAngleShouldBeSetCorrectlyForChild() {
      Assert.Equal(expectedOrientationAngle, gsaElement.LoadPanelElement.OrientationAngle);
    }

    [Fact]
    public void OrientationNodeShouldThrowExcepion() {
      Assert.Throws<ArgumentException>(() => gsaElement.OrientationNode = 1);
    }

    [Fact]
    public void SettingTopologyShouldThrowAnError() {
      Assert.ThrowsAny<SEHException>(() => gsaElement.Topology = new ReadOnlyCollection<int>(new List<int>(1)));
    }

    [Fact]
    public void TypeShouldBeSetCorrectly() {
      Assert.Equal(expectedElementType, gsaElement.Type);
    }

    [Fact]
    public void ColorShouldBeSetCorrectlyForParent() {
      Assert.Equal(expectedColor, gsaElement.Colour);
    }

    [Fact]
    public void ColorShouldBeSetCorrectlyForChild() {
      Assert.Equal(expectedColor, gsaElement.LoadPanelElement.Colour);
    }

    [Fact]
    public void SetReleaseShouldThrowAnError() {
      Assert.Throws<ArgumentException>(() => gsaElement.SetRelease(1, new Bool6(true, true, true, true, true, true)));
    }

    [Fact]
    public void GetReleaseShouldThrowAnError() {
      Assert.Throws<ArgumentException>(() => gsaElement.Release(1));
    }

    [Fact]
    public void SetEndReleaseShouldThrowAnError() {
      Assert.Throws<ArgumentException>(()
        => gsaElement.SetEndRelease(1, new EndRelease(new Bool6(true, false, true, false, false, true))));
    }

    [Fact]
    public void GetEndReleaseShouldThrowAnError() {
      Assert.Throws<ArgumentException>(() => gsaElement.GetEndRelease(1));
    }

    [Fact]
    public void TypeAsStringShouldReturnCorrectString() {
      Assert.Equal("Load Panel", gsaElement.TypeAsString());
    }
  }

  //[Collection("GrasshopperFixture collection")]
  //public class GsaElementTests_Element {
  //  private readonly GSAElement gsaElement;
  //  private const string expectedName = "name";
  //  private const int expectedPropertyValue = 2;
  //  private const int expectedGroupValue = 22;
  //  private readonly ParentMember expectedParentValue = new ParentMember(22, 22);
  //  private const bool expectedIsDummyValue = false;
  //  private const double expectedOrientationAngle = 2.2d;
  //  private readonly ElementType expectedElementType = ElementType.BEAM;
  //  private readonly ValueType expectedColor = Color.FromArgb(255, 0, 255, 255);
  //  private readonly int expectedOffsetValue = 1;
  //  private readonly int expectedOrientationNode = 1;
  //  private readonly ReadOnlyCollection<int> expectedTopologyValue = new ReadOnlyCollection<int>(new List<int>() {
  //    111,
  //  });
  //  private const int topology = 1;

  //  public GsaElementTests_Element() {
  //    gsaElement = new GSAElement(new Element()) {
  //      Name = expectedName,
  //      Group = expectedGroupValue,
  //      Property = expectedPropertyValue,
  //      ParentMember = expectedParentValue,
  //      IsDummy = expectedIsDummyValue,
  //      OrientationAngle = expectedOrientationAngle,
  //      Type = expectedElementType,
  //      Colour = expectedColor,
  //      Offset = {
  //        X1 = expectedOffsetValue,
  //        X2 = expectedOffsetValue,
  //        Y = expectedOffsetValue,
  //        Z = expectedOffsetValue,
  //      },
  //      OrientationNode = 1,
  //      Topology = expectedTopologyValue,
  //    };
  //    gsaElement.SetRelease(topology, new Bool6(true, false, true, false, true, true));
  //    gsaElement.SetEndRelease(topology, new EndRelease(new Bool6(true, false, true, false, false, true)));
  //  }

  //  [Fact]
  //  public void GroupsShouldDefaultToOne() {
  //    Assert.Equal(expectedGroupValue, gsaElement.Group);
  //  }

  //  [Fact]
  //  public void NameShouldBeSetCorrectlyForParent() {
  //    Assert.Equal(expectedName, gsaElement.Name);
  //  }

  //  [Fact]
  //  public void NameShouldBeSetCorrectlyForChild() {
  //    Assert.Equal(expectedName, gsaElement.Element.Name);
  //  }

  //  [Fact]
  //  public void PropertyShouldBeSetCorrectlyForParent() {
  //    Assert.Equal(expectedPropertyValue, gsaElement.Property);
  //  }

  //  [Fact]
  //  public void PropertyShouldBeSetCorrectlyForChild() {
  //    Assert.Equal(expectedPropertyValue, gsaElement.Element.Property);
  //  }

  //  [Fact]
  //  public void GroupShouldBeSetCorrectlyForParent() {
  //    Assert.Equal(expectedGroupValue, gsaElement.Group);
  //  }

  //  [Fact]
  //  public void GroupShouldBeSetCorrectlyForChild() {
  //    Assert.Equal(expectedPropertyValue, gsaElement.Element.Group);
  //  }

  //  [Fact]
  //  public void ParentMemberReplicaShouldBeSetCorrectlyForParent() {
  //    Assert.Equal(expectedParentValue.Replica, gsaElement.ParentMember.Replica);
  //  }

  //  [Fact]
  //  public void ParentMemberMemberShouldBeSetCorrectlyForParent() {
  //    Assert.Equal(expectedParentValue.Member, gsaElement.ParentMember.Member);
  //  }

  //  [Fact]
  //  public void ParentMemberReplicaShouldBeSetCorrectlyForChild() {
  //    Assert.Equal(expectedParentValue.Replica, gsaElement.Element.ParentMember.Replica);
  //  }

  //  [Fact]
  //  public void ParentMemberMemberShouldBeSetCorrectlyForChild() {
  //    Assert.Equal(expectedParentValue.Member, gsaElement.Element.ParentMember.Member);
  //  }

  //  [Fact]
  //  public void IsDummyShouldBeSetCorrectlyForParent() {
  //    Assert.Equal(expectedIsDummyValue, gsaElement.IsDummy);
  //  }

  //  [Fact]
  //  public void IsDummyShouldBeSetCorrectlyForChild() {
  //    Assert.Equal(expectedIsDummyValue, gsaElement.Element.IsDummy);
  //  }

  //  [Fact]
  //  public void OffsetShouldSetX1ValueCorrectlyForParent() {
  //    Assert.Equal(expectedOffsetValue, gsaElement.Offset.X1);
  //  }

  //  [Fact]
  //  public void OffsetShouldSetX2ValueCorrectlyForParent() {
  //    Assert.Equal(expectedOffsetValue, gsaElement.Offset.X2);
  //  }

  //  [Fact]
  //  public void OffsetShouldSetYValueCorrectlyForParent() {
  //    Assert.Equal(expectedOffsetValue, gsaElement.Offset.Y);
  //  }

  //  [Fact]
  //  public void OffsetShouldSetZValueCorrectlyForParent() {
  //    Assert.Equal(expectedOffsetValue, gsaElement.Offset.Z);
  //  }

  //  [Fact]
  //  public void OffsetShouldSetX1ValueCorrectlyForChild() {
  //    Assert.Equal(expectedOffsetValue, gsaElement.Element.Offset.X1);
  //  }

  //  [Fact]
  //  public void OffsetShouldSetX2ValueCorrectlyForChild() {
  //    Assert.Equal(expectedOffsetValue, gsaElement.Element.Offset.X2);
  //  }

  //  [Fact]
  //  public void OffsetShouldSetYValueCorrectlyForChild() {
  //    Assert.Equal(expectedOffsetValue, gsaElement.Element.Offset.Y);
  //  }

  //  [Fact]
  //  public void OffsetShouldSetZValueCorrectlyForChild() {
  //    Assert.Equal(expectedOffsetValue, gsaElement.Element.Offset.Z);
  //  }

  //  [Fact]
  //  public void OrientationAngleShouldBeSetCorrectlyForParent() {
  //    Assert.Equal(expectedOrientationAngle, gsaElement.OrientationAngle);
  //  }

  //  [Fact]
  //  public void OrientationAngleShouldBeSetCorrectlyForchild() {
  //    Assert.Equal(expectedOrientationAngle, gsaElement.Element.OrientationAngle);
  //  }

  //  [Fact]
  //  public void OrientationNodeShouldSetValueCorrectlyForParent() {
  //    Assert.Equal(expectedOrientationNode, gsaElement.OrientationNode);
  //  }

  //  [Fact]
  //  public void OrientationNodeShouldSetValueCorrectlyForChild() {
  //    Assert.Equal(expectedOrientationNode, gsaElement.Element.OrientationNode);
  //  }

  //  [Fact]
  //  public void TopologyShouldBeSetCorrectlyForParent() {
  //    Assert.Equal(expectedTopologyValue[0], gsaElement.Topology[0]);
  //  }

  //  [Fact]
  //  public void TopologyShouldBeSetCorrectlyForChild() {
  //    Assert.Equal(expectedTopologyValue[0], gsaElement.Element.Topology[0]);
  //  }

  //  [Fact]
  //  public void TypeShouldBeSetCorrectlyForParent() {
  //    Assert.Equal(expectedElementType, gsaElement.Type);
  //  }

  //  [Fact]
  //  public void TypeShouldBeSetCorrectlyForChild() {
  //    Assert.Equal(expectedElementType, gsaElement.Element.Type);
  //  }

  //  [Fact]
  //  public void ColorShouldBeSetCorrectlyForParent() {
  //    Assert.Equal(expectedColor, gsaElement.Colour);
  //  }

  //  [Fact]
  //  public void ColorShouldBeSetCorrectlyForChild() {
  //    Assert.Equal(expectedColor, gsaElement.Element.Colour);
  //  }

  //  [Fact]
  //  public void SetAndGetReleaseShouldReturnValidX() {
  //    Bool6 actualResult = gsaElement.Release(topology);
  //    Assert.True(actualResult.X);
  //  }

  //  [Fact]
  //  public void SetAndGetReleaseShouldReturnValidY() {
  //    Bool6 actualResult = gsaElement.Release(topology);
  //    Assert.False(actualResult.Y);
  //  }

  //  [Fact]
  //  public void SetAndGetReleaseShouldReturnValidZ() {
  //    Bool6 actualResult = gsaElement.Release(topology);
  //    Assert.True(actualResult.Z);
  //  }

  //  [Fact]
  //  public void SetAndGetReleaseShouldReturnValidXx() {
  //    Bool6 actualResult = gsaElement.Release(topology);
  //    Assert.False(actualResult.XX);
  //  }

  //  [Fact]
  //  public void SetAndGetReleaseShouldReturnValidYy() {
  //    Bool6 actualResult = gsaElement.Release(topology);
  //    Assert.True(actualResult.YY);
  //  }

  //  [Fact]
  //  public void SetAndGetReleaseShouldReturnValidZz() {
  //    Bool6 actualResult = gsaElement.Release(topology);
  //    Assert.True(actualResult.ZZ);
  //  }

  //  [Fact]
  //  public void SetAndGetEndReleaseShouldReturnValidX() {
  //    EndRelease actualResult = gsaElement.GetEndRelease(topology);
  //    Assert.True(actualResult.Releases.X);
  //  }

  //  [Fact]
  //  public void SetAndGetEndReleaseShouldReturnValidY() {
  //    EndRelease actualResult = gsaElement.GetEndRelease(topology);
  //    Assert.False(actualResult.Releases.Y);
  //  }

  //  [Fact]
  //  public void SetAndGetEndReleaseShouldReturnValidZ() {
  //    EndRelease actualResult = gsaElement.GetEndRelease(topology);
  //    Assert.True(actualResult.Releases.Z);
  //  }

  //  [Fact]
  //  public void SetAndGetEndReleaseShouldReturnValidXx() {
  //    EndRelease actualResult = gsaElement.GetEndRelease(topology);
  //    Assert.False(actualResult.Releases.XX);
  //  }

  //  [Fact]
  //  public void SetAndGetEndReleaseShouldReturnValidYy() {
  //    EndRelease actualResult = gsaElement.GetEndRelease(topology);
  //    Assert.False(actualResult.Releases.YY);
  //  }

  //  [Fact]
  //  public void SetAndGetEndReleaseShouldReturnValidZz() {
  //    EndRelease actualResult = gsaElement.GetEndRelease(topology);
  //    Assert.True(actualResult.Releases.ZZ);
  //  }

  //  [Fact]
  //  public void TypeAsStringShouldReturnCorrectString() {
  //    Assert.Equal(gsaElement.Element.TypeAsString(), gsaElement.TypeAsString());
  //  }
  //}
}
