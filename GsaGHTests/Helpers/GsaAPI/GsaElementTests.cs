using System;

using Xunit;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.InteropServices;
using GsaAPI;
using GsaGH.Helpers;

namespace GsaGHTests.Helpers.GsaAPITests {

  [Collection("GrasshopperFixture collection")]
  public class GsaElementTests_LoadPanel {
    private readonly GSAElement gsaElement;

    public GsaElementTests_LoadPanel() {
      gsaElement = new GSAElement(new LoadPanelElement()) {
        Name = "name",
        Group = 11,
        Property = 1,
        ParentMember = new ParentMember(11, 11),
        IsDummy = true,
        OrientationAngle = 1.1d,
        Type = (ElementType)GSAElement.LoadPanelType,
        Colour = Color.White,
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
      Assert.Equal(11, gsaElement.Group);
    }

    [Fact]
    public void NameShouldBeSetCorrectlyForParent() {
      Assert.Equal("name", gsaElement.Name);
    }

    [Fact]
    public void NameShouldBeSetCorrectlyForChild() {
      Assert.Equal("name", gsaElement.LoadPanelElement.Name);
    }

    [Fact]
    public void PropertyShouldBeSetCorrectlyForParent() {
      Assert.Equal(1, gsaElement.Property);
    }

    [Fact]
    public void PropertyShouldBeSetCorrectlyForChild() {
      Assert.Equal(1, gsaElement.LoadPanelElement.Property);
    }

    [Fact]
    public void GroupShouldBeSetCorrectlyForParent() {
      Assert.Equal(11, gsaElement.Group);
    }

    [Fact]
    public void GroupShouldBeSetCorrectlyForChild() {
      Assert.Equal(11, gsaElement.LoadPanelElement.Group);
    }

    [Fact]
    public void ParentMemberReplicaShouldBeSetCorrectlyForParent() {
      Assert.Equal(11, gsaElement.ParentMember.Replica);
    }

    [Fact]
    public void ParentMemberMemberShouldBeSetCorrectlyForParent() {
      Assert.Equal(11, gsaElement.ParentMember.Member);
    }

    [Fact]
    public void ParentMemberReplicaShouldBeSetCorrectlyForChild() {
      Assert.Equal(11, gsaElement.LoadPanelElement.ParentMember.Replica);
    }

    [Fact]
    public void ParentMemberMemberShouldBeSetCorrectlyForChild() {
      Assert.Equal(11, gsaElement.LoadPanelElement.ParentMember.Member);
    }

    [Fact]
    public void IsDummyShouldBeSetCorrectlyForParent() {
      Assert.True(gsaElement.IsDummy);
    }

    [Fact]
    public void IsDummyShouldBeSetCorrectlyForChild() {
      Assert.True(gsaElement.LoadPanelElement.IsDummy);
    }

    [Fact]
    public void OffsetShouldThrowError() {
      Assert.Throws<ArgumentException>(() => gsaElement.Offset = null);
    }

    [Fact]
    public void OrientationAngleShouldBeSetCorrectlyForParent() {
      Assert.Equal(1.1d, gsaElement.OrientationAngle);
    }

    [Fact]
    public void OrientationAngleShouldBeSetCorrectlyForChild() {
      Assert.Equal(1.1d, gsaElement.LoadPanelElement.OrientationAngle);
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
      Assert.Equal((ElementType)GSAElement.LoadPanelType, gsaElement.Type);
    }

    [Fact]
    public void ColorShouldBeSetCorrectlyForParent() {
      Assert.Equal(Color.FromArgb(255, 255, 255, 255), gsaElement.Colour);
    }

    [Fact]
    public void ColorShouldBeSetCorrectlyForChild() {
      Assert.Equal(Color.FromArgb(255, 255, 255, 255), gsaElement.LoadPanelElement.Colour);
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

  [Collection("GrasshopperFixture collection")]
  public class GsaElementTests_Element {
    private readonly GSAElement gsaElement;

    public GsaElementTests_Element() {
      gsaElement = new GSAElement(new Element()) {
        Name = "expectedName",
        Group = 22,
        Property = 2,
        ParentMember = new ParentMember(22, 22),
        IsDummy = false,
        OrientationAngle = 2.2d,
        Type = ElementType.BEAM,
        Colour = Color.Aqua,
        Offset = {
          X1 = 1,
          X2 = 1,
          Y = 1,
          Z = 1,
        },
        OrientationNode = 1,
        Topology = new ReadOnlyCollection<int>(new List<int>() {
          111,
        }),
      };
      gsaElement.SetRelease(1, new Bool6(true, false, true, false, true, true));
      gsaElement.SetEndRelease(1, new EndRelease(new Bool6(true, false, true, false, false, true)));
    }

    [Fact]
    public void GroupsShouldDefaultToOne() {
      Assert.Equal(22, gsaElement.Group);
    }

    [Fact]
    public void NameShouldBeSetCorrectlyForParent() {
      Assert.Equal("expectedName", gsaElement.Name);
    }

    [Fact]
    public void NameShouldBeSetCorrectlyForChild() {
      Assert.Equal("expectedName", gsaElement.Element.Name);
    }

    [Fact]
    public void PropertyShouldBeSetCorrectlyForParent() {
      Assert.Equal(2, gsaElement.Property);
    }

    [Fact]
    public void PropertyShouldBeSetCorrectlyForChild() {
      Assert.Equal(2, gsaElement.Element.Property);
    }

    [Fact]
    public void GroupShouldBeSetCorrectlyForParent() {
      Assert.Equal(22, gsaElement.Group);
    }

    [Fact]
    public void GroupShouldBeSetCorrectlyForChild() {
      Assert.Equal(22, gsaElement.Element.Group);
    }

    [Fact]
    public void ParentMemberReplicaShouldBeSetCorrectlyForParent() {
      Assert.Equal(22, gsaElement.ParentMember.Replica);
    }

    [Fact]
    public void ParentMemberMemberShouldBeSetCorrectlyForParent() {
      Assert.Equal(22, gsaElement.ParentMember.Member);
    }

    [Fact]
    public void ParentMemberReplicaShouldBeSetCorrectlyForChild() {
      Assert.Equal(22, gsaElement.Element.ParentMember.Replica);
    }

    [Fact]
    public void ParentMemberMemberShouldBeSetCorrectlyForChild() {
      Assert.Equal(22, gsaElement.Element.ParentMember.Member);
    }

    [Fact]
    public void IsDummyShouldBeSetCorrectlyForParent() {
      Assert.False(gsaElement.IsDummy);
    }

    [Fact]
    public void IsDummyShouldBeSetCorrectlyForChild() {
      Assert.False(gsaElement.Element.IsDummy);
    }

    [Fact]
    public void OffsetShouldSetX1ValueCorrectlyForParent() {
      Assert.Equal(1, gsaElement.Offset.X1);
    }

    [Fact]
    public void OffsetShouldSetX2ValueCorrectlyForParent() {
      Assert.Equal(1, gsaElement.Offset.X2);
    }

    [Fact]
    public void OffsetShouldSetYValueCorrectlyForParent() {
      Assert.Equal(1, gsaElement.Offset.Y, DoubleComparer.Default);
    }

    [Fact]
    public void OffsetShouldSetZValueCorrectlyForParent() {
      Assert.Equal(1, gsaElement.Offset.Z, DoubleComparer.Default);
    }

    [Fact]
    public void OffsetShouldSetX1ValueCorrectlyForChild() {
      Assert.Equal(1, gsaElement.Element.Offset.X1);
    }

    [Fact]
    public void OffsetShouldSetX2ValueCorrectlyForChild() {
      Assert.Equal(1, gsaElement.Element.Offset.X2);
    }

    [Fact]
    public void OffsetShouldSetYValueCorrectlyForChild() {
      Assert.Equal(1, gsaElement.Element.Offset.Y, DoubleComparer.Default);
    }

    [Fact]
    public void OffsetShouldSetZValueCorrectlyForChild() {
      Assert.Equal(1, gsaElement.Element.Offset.Z, DoubleComparer.Default);
    }

    [Fact]
    public void OrientationAngleShouldBeSetCorrectlyForParent() {
      Assert.Equal(2.2d, gsaElement.OrientationAngle);
    }

    [Fact]
    public void OrientationAngleShouldBeSetCorrectlyForchild() {
      Assert.Equal(2.2d, gsaElement.Element.OrientationAngle);
    }

    [Fact]
    public void OrientationNodeShouldSetValueCorrectlyForParent() {
      Assert.Equal(1, gsaElement.OrientationNode);
    }

    [Fact]
    public void OrientationNodeShouldSetValueCorrectlyForChild() {
      Assert.Equal(1, gsaElement.Element.OrientationNode);
    }

    [Fact]
    public void TopologyShouldBeSetCorrectlyForParent() {
      Assert.Equal(111, gsaElement.Topology[0]);
    }

    [Fact]
    public void TopologyShouldBeSetCorrectlyForChild() {
      Assert.Equal(111, gsaElement.Element.Topology[0]);
    }

    [Fact]
    public void TypeShouldBeSetCorrectlyForParent() {
      Assert.Equal(ElementType.BEAM, gsaElement.Type);
    }

    [Fact]
    public void TypeShouldBeSetCorrectlyForChild() {
      Assert.Equal(ElementType.BEAM, gsaElement.Element.Type);
    }

    [Fact]
    public void ColorShouldBeSetCorrectlyForParent() {
      Assert.Equal(Color.FromArgb(255, 0, 255, 255), gsaElement.Colour);
    }

    [Fact]
    public void ColorShouldBeSetCorrectlyForChild() {
      Assert.Equal(Color.FromArgb(255, 0, 255, 255), gsaElement.Element.Colour);
    }

    [Fact]
    public void SetAndGetReleaseShouldReturnValidX() {
      Bool6 actualResult = gsaElement.Release(1);
      Assert.True(actualResult.X);
    }

    [Fact]
    public void SetAndGetReleaseShouldReturnValidY() {
      Bool6 actualResult = gsaElement.Release(1);
      Assert.False(actualResult.Y);
    }

    [Fact]
    public void SetAndGetReleaseShouldReturnValidZ() {
      Bool6 actualResult = gsaElement.Release(1);
      Assert.True(actualResult.Z);
    }

    [Fact]
    public void SetAndGetReleaseShouldReturnValidXx() {
      Bool6 actualResult = gsaElement.Release(1);
      Assert.False(actualResult.XX);
    }

    [Fact]
    public void SetAndGetReleaseShouldReturnValidYy() {
      Bool6 actualResult = gsaElement.Release(1);
      Assert.False(actualResult.YY);
    }

    [Fact]
    public void SetAndGetReleaseShouldReturnValidZz() {
      Bool6 actualResult = gsaElement.Release(1);
      Assert.True(actualResult.ZZ);
    }

    [Fact]
    public void SetAndGetEndReleaseShouldReturnValidX() {
      EndRelease actualResult = gsaElement.GetEndRelease(1);
      Assert.True(actualResult.Releases.X);
    }

    [Fact]
    public void SetAndGetEndReleaseShouldReturnValidY() {
      EndRelease actualResult = gsaElement.GetEndRelease(1);
      Assert.False(actualResult.Releases.Y);
    }

    [Fact]
    public void SetAndGetEndReleaseShouldReturnValidZ() {
      EndRelease actualResult = gsaElement.GetEndRelease(1);
      Assert.True(actualResult.Releases.Z);
    }

    [Fact]
    public void SetAndGetEndReleaseShouldReturnValidXx() {
      EndRelease actualResult = gsaElement.GetEndRelease(1);
      Assert.False(actualResult.Releases.XX);
    }

    [Fact]
    public void SetAndGetEndReleaseShouldReturnValidYy() {
      EndRelease actualResult = gsaElement.GetEndRelease(1);
      Assert.False(actualResult.Releases.YY);
    }

    [Fact]
    public void SetAndGetEndReleaseShouldReturnValidZz() {
      EndRelease actualResult = gsaElement.GetEndRelease(1);
      Assert.True(actualResult.Releases.ZZ);
    }

    [Fact]
    public void TypeAsStringShouldReturnCorrectString() {
      Assert.Equal(gsaElement.Element.TypeAsString(), gsaElement.TypeAsString());
    }
  }
}
