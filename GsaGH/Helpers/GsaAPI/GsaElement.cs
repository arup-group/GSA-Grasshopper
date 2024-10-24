using System;
using System.Collections.ObjectModel;

namespace GsaAPI {
  public class GSAElement {
    public static int LoadPanelType = -1000;
    private GSAElement() { }
    public GSAElement(Element element) {
      Element = element;
      IsLoadPanel = false;
      Element.Group = element.Group == 0 ? 1 : element.Group;
    }
    public GSAElement(LoadPanelElement element) {
      LoadPanelElelment = element;
      IsLoadPanel = true;
      LoadPanelElelment.Group = element.Group == 0 ? 1 : element.Group;
    }

    public bool IsLoadPanel { get; }
    public LoadPanelElement LoadPanelElelment { get; }
    public Element Element { get; }

    public string Name {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.Name;
        } else {
          return Element.Name;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.Name = value;
        } else {
          Element.Name = value;
        }
      }
    }

    public int Property {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.Property;
        } else {
          return Element.Property;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.Property = value;
        } else {
          Element.Property = value;
        }
      }
    }

    public int Group {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.Group;
        } else {
          return Element.Group;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.Group = value;
        } else {
          Element.Group = value;
        }
      }
    }

    public ParentMember ParentMember {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.ParentMember;
        } else {
          return Element.ParentMember;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.ParentMember = value;
        } else {
          Element.ParentMember = value;
        }
      }
    }

    public bool IsDummy {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.IsDummy;
        } else {
          return Element.IsDummy;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.IsDummy = value;
        } else {
          Element.IsDummy = value;
        }
      }
    }

    public Offset Offset {
      get {
        if (IsLoadPanel) {
          throw new ArgumentException("Offset is not applicable for load panels");
        } else {
          return Element.Offset;
        }
      }
      set {
        if (!IsLoadPanel) {
          Element.Offset = value;
        } else { throw new ArgumentException("Offset is not applicable for load panels"); }
      }
    }

    public double OrientationAngle {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.OrientationAngle;
        } else {
          return Element.OrientationAngle;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.OrientationAngle = value;
        } else {
          Element.OrientationAngle = value;
        }
      }
    }

    public int OrientationNode {
      get {
        if (IsLoadPanel) {
          throw new ArgumentException("Orientation node is not applicable for load panels");
        } else {
          return Element.OrientationNode;
        }
      }
      set {
        if (!IsLoadPanel) {
          Element.OrientationNode = value;
        } else { throw new ArgumentException("Orientation node is not applicable for load panels"); }
      }
    }

    public ReadOnlyCollection<int> Topology {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.Topology;
        } else {
          return Element.Topology;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.Topology = value;
        } else {
          Element.Topology = value;
        }
      }
    }

    public ElementType Type {
      get {
        if (IsLoadPanel) {
          return (ElementType)LoadPanelType;
        } else {
          return Element.Type;
        }
      }
      set {
        if (!IsLoadPanel) {
          Element.Type = value;
        }
      }
    }

    public ValueType Colour {
      get {
        if (IsLoadPanel) {
          return LoadPanelElelment.Colour;
        } else {
          return Element.Colour;
        }
      }
      set {
        if (IsLoadPanel) {
          LoadPanelElelment.Colour = value;
        } else {
          Element.Colour = value;
        }
      }
    }

    public Bool6 Release(int iTopology) {
      if (IsLoadPanel) {
        throw new ArgumentException("releases is not applicable for load panels");
      } else {
        return Element.Release(iTopology);
      }
    }
    public void SetRelease(int iTopology, Bool6 release) {
      if (IsLoadPanel) {
        throw new ArgumentException("releases is not applicable for load panels");
      } else {
        Element.SetRelease(iTopology, release);
      }
    }
    public EndRelease GetEndRelease(int iTopology) {
      if (IsLoadPanel) {
        throw new ArgumentException("releases is not applicable for load panels");
      } else {
        return Element.GetEndRelease(iTopology);
      }
    }
    public void SetEndRelease(int iTopology, EndRelease endRelease) {
      if (IsLoadPanel) {
        throw new ArgumentException("releases is not applicable for load panels");
      } else {
        Element.SetEndRelease(iTopology, endRelease);
      }
    }
    public string TypeAsString() {
      if (IsLoadPanel) {
        return "Load Panel";
      } else {
        return Element.TypeAsString();
      }
    }

  }

}
