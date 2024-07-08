using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using AngleUnit = OasysUnits.Units.AngleUnit;
namespace GsaGH.Helpers {

  public static class ElementHelper {
    public static List<int> Group(List<object> elements) {
      var group = new List<int>();
      foreach (object element in elements) {
        if ((element as Element) != null) {
          var element2d = element as Element;
          group.Add(element2d.Group);
        }
        else {
          var element2d = element as LoadPanelElement;
          group.Add(element2d.Group);
        }
      }
      return group;
    }

    public static List<int> Property(List<object> elements) {
      var group = new List<int>();
      foreach (object element in elements) {
        if ((element as Element) != null) {
          var element2d = element as Element;
          group.Add(element2d.Property);
        }
        else {
          var element2d = element as LoadPanelElement;
          group.Add(element2d.Property);
        }
      }
      return group;
    }

    public static List<int> ParentMember(List<object> elements) {
      var group = new List<int>();
      foreach (object element in elements) {
        if ((element as Element) != null) {
          var element2d = element as Element;
          group.Add(element2d.ParentMember.Member);
        }
        else {
          var element2d = element as LoadPanelElement;
          group.Add(element2d.ParentMember.Member);
        }
      }
      return group;
    }

    public static List<int> OrientationNode(List<object> elements) {
      var group = new List<int>();
      foreach (object element in elements) {
        if ((element as Element) != null) {
          var element2d = element as Element;
          group.Add(element2d.OrientationNode);
        }
      }
      return group;
    }

    public static List<GsaOffset> GsaGhOffset(List<object> elements) {
      var group = new List<GsaOffset>();
      foreach (object element in elements) {
        if ((element as Element) != null) {
          var element2d = element as Element;
          group.Add(new GsaOffset(element2d.Offset.X1, element2d.Offset.X2, element2d.Offset.Y, element2d.Offset.Z));
        }
      }
      return group;
    }

    public static List<ElementType> Type(List<object> elements) {
      var group = new List<ElementType>();
      foreach (object element in elements) {
        if ((element as Element) != null) {
          var element2d = element as Element;
          group.Add(element2d.Type);
        }
      }
      return group;
    }

    public static List<double> OrientationAngle(List<object> elements) {
      var group = new List<double>();
      foreach (object element in elements) {
        if ((element as Element) != null) {
          var element2d = element as Element;
          group.Add(element2d.OrientationAngle);
        }
        else {
          var element2d = element as LoadPanelElement;
          group.Add(element2d.OrientationAngle);
        }
      }
      return group;
    }

    public static List<Angle> OasysOrientationAngle(List<object> elements) {
      var group = new List<Angle>();
      foreach (object element in elements) {
        if ((element as Element) != null) {
          var element2d = element as Element;
          group.Add(new Angle(element2d.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian));
        }
        else {
          var element2d = element as LoadPanelElement;
          group.Add(new Angle(element2d.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian));
        }
      }
      return group;
    }

    public static List<bool> IsDummy(List<object> elements) {
      var group = new List<bool>();
      foreach (object element in elements) {
        if ((element as Element) != null) {
          var element2d = element as Element;
          group.Add(element2d.IsDummy);
        }
        else {
          var element2d = element as LoadPanelElement;
          group.Add(element2d.IsDummy);
        }
      }
      return group;
    }

    public static List<Color> Colour(List<object> elements) {
      var group = new List<Color>();
      foreach (object element in elements) {
        if ((element as Element) != null) {
          var element2d = element as Element;
          group.Add((Color)element2d.Colour);
        }
        else {
          var element2d = element as LoadPanelElement;
          group.Add((Color)element2d.Colour);
        }
      }
      return group;
    }

    public static List<string> Name(List<object> elements) {
      var group = new List<string>();
      foreach (object element in elements) {
        if ((element as Element) != null) {
          var element2d = element as Element;
          group.Add(element2d.Name);
        }
        else {
          var element2d = element as LoadPanelElement;
          group.Add(element2d.Name);
        }
      }
      return group;
    }
  }
}