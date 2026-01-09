using System.Collections.Generic;
using System.Collections.ObjectModel;

using GsaAPI;

using GsaGH.Parameters;

using Xunit;

using EntityType = GsaGH.Parameters.EntityType;
using LengthUnit = OasysUnits.Units.LengthUnit;
using GsaGH.Helpers;
namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaListTest {
    [Fact]
    public void TestUndefinedList() {
      // Arrange
      var m = new GsaAPI.Model();

      var apilist = new EntityList {
        Name = "undef List",
        Definition = "1 to 5 not 3",
        Type = GsaAPI.EntityType.Undefined
      };
      // Act
      m.AddList(apilist);

      var model = new GsaModel() { ApiModel = m };
      GsaList list = model.GetLists()[0];

      Assert.Equal(1, list.Id);
      Assert.Equal("undef List", list.Name);
      Assert.Equal("1 to 5 not 3", list.Definition);
      Assert.Equal(EntityType.Undefined, list.EntityType);
      Assert.Equal("1 to 5 not 3", (string)list.GetListObjects(LengthUnit.Meter)[0]);
    }

    [Fact]
    public void TestNodeList() {
      // Arrange
      var n1 = new Node() { Position = new Vector3() { X = 0, Y = 0, Z = 0 } };
      var n2 = new Node() { Position = new Vector3() { X = 10, Y = 10, Z = 0 } };
      var n3 = new Node() { Position = new Vector3() { X = 0, Y = 0, Z = 3.5 } };
      var m = new GsaAPI.Model();
      m.AddNode(n1);
      m.AddNode(n2);
      m.AddNode(n3);
      var apilist = new EntityList {
        Name = "node List",
        Definition = "All NOT 2",
        Type = GsaAPI.EntityType.Node
      };

      // Act
      m.AddList(apilist);
      var model = new GsaModel() { ApiModel = m };
      GsaList list = model.GetLists()[0];

      // Assert
      Assert.Equal(1, list.Id);
      Assert.Equal("node List", list.Name);
      Assert.Equal("all not 2", list.Definition);
      Assert.Equal(EntityType.Node, list.EntityType);
      List<object> nodes = list.GetListObjects(LengthUnit.Meter);
      Assert.Equal(2, nodes.Count);
      Assert.Equal(1, ((GsaNodeGoo)nodes[0]).Value.Id);
      Assert.Equal(3, ((GsaNodeGoo)nodes[1]).Value.Id);
      Assert.Equal(3.5, ((GsaNodeGoo)nodes[1]).Value.Point.Z, DoubleComparer.Default);
    }

    [Fact]
    public void TestElementList() {
      // Arrange
      var n1 = new Node() { Position = new Vector3() { X = 0, Y = 0, Z = 0 } };
      var n2 = new Node() { Position = new Vector3() { X = 10, Y = 0, Z = 0 } };
      var n3 = new Node() { Position = new Vector3() { X = 10, Y = 10, Z = 3.5 } };
      var n4 = new Node() { Position = new Vector3() { X = 0, Y = 10, Z = 3.5 } };
      var e1 = new Element() { Type = ElementType.BEAM, Topology = new ReadOnlyCollection<int>(new List<int>() { 1, 2 }) };
      var e2 = new Element() { Type = ElementType.BEAM, Topology = new ReadOnlyCollection<int>(new List<int>() { 2, 3 }) };
      var e3 = new Element() { Type = ElementType.BEAM, Topology = new ReadOnlyCollection<int>(new List<int>() { 3, 4 }) };
      var e4 = new Element() { Type = ElementType.QUAD4, Topology = new ReadOnlyCollection<int>(new List<int>() { 1, 2, 3, 4 }) };
      var m = new GsaAPI.Model();
      m.AddNode(n1);
      m.AddNode(n2);
      m.AddNode(n3);
      m.AddNode(n4);
      m.AddElement(e1);
      m.AddElement(e2);
      m.AddElement(e3);
      m.AddElement(e4);

      var apilist = new EntityList {
        Name = "elem List",
        Definition = "1 2 4",
        Type = GsaAPI.EntityType.Element
      };

      // Act
      m.AddList(apilist);
      var model = new GsaModel() { ApiModel = m };
      GsaList list = model.GetLists()[0];

      // Assert
      Assert.Equal(1, list.Id);
      Assert.Equal("elem List", list.Name);
      Assert.Equal("1 2 4", list.Definition);
      Assert.Equal(EntityType.Element, list.EntityType);
      List<object> elems = list.GetListObjects(LengthUnit.Meter);
      Assert.Equal(3, elems.Count);
      Assert.Equal(1, ((GsaElement1dGoo)elems[0]).Value.Id);
      Assert.Equal(2, ((GsaElement1dGoo)elems[1]).Value.Id);
      Assert.Equal(4, ((GsaElement2dGoo)elems[2]).Value.Ids[0]);
      Assert.Equal(10, ((GsaElement1dGoo)elems[1]).Value.Line.PointAtStart.X, DoubleComparer.Default);
    }

    [Fact]
    public void TestMemberList() {
      // Arrange
      var n1 = new Node() { Position = new Vector3() { X = 0, Y = 0, Z = 0 } };
      var n2 = new Node() { Position = new Vector3() { X = 10, Y = 0, Z = 0 } };
      var n3 = new Node() { Position = new Vector3() { X = 10, Y = 10, Z = 3.5 } };
      var n4 = new Node() { Position = new Vector3() { X = 0, Y = 10, Z = 3.5 } };
      var m1 = new Member() { Type = MemberType.GENERIC_1D, Topology = "1 2" };
      var m2 = new Member() { Type = MemberType.GENERIC_2D, Topology = "1 2 3 4" };
      var m3 = new Member() { Type = MemberType.GENERIC_1D, Topology = "2 3" };
      var m = new GsaAPI.Model();
      m.AddNode(n1);
      m.AddNode(n2);
      m.AddNode(n3);
      m.AddNode(n4);
      m.AddMember(m1);
      m.AddMember(m2);
      m.AddMember(m3);

      var apilist = new EntityList {
        Name = "mem List",
        Definition = "all not 1",
        Type = GsaAPI.EntityType.Member
      };

      // Act
      m.AddList(apilist);
      var model = new GsaModel() { ApiModel = m };
      GsaList list = model.GetLists()[0];

      // Assert
      Assert.Equal(1, list.Id);
      Assert.Equal("mem List", list.Name);
      Assert.Equal("all not 1", list.Definition);
      Assert.Equal(EntityType.Member, list.EntityType);
      List<object> mems = list.GetListObjects(LengthUnit.Meter);
      Assert.Equal(2, mems.Count);
      Assert.Equal(3, ((GsaMember1dGoo)mems[0]).Value.Id);
      Assert.Equal(10, ((GsaMember1dGoo)mems[0]).Value.Topology[0].X, DoubleComparer.Default);
      Assert.Equal(2, ((GsaMember2dGoo)mems[1]).Value.Id);
      Assert.Equal(3.5, ((GsaMember2dGoo)mems[1]).Value.Topology[3].Z, DoubleComparer.Default);
    }
  }
}
