using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  /// Model class, this class defines the basic properties and methods for any Gsa Model
  /// </summary>
  [Serializable]
  public class GsaModel {
    #region properties
    public Model Model { get; set; } = new Model();
    public string FileNameAndPath { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public LengthUnit ModelUnit { get; set; } = LengthUnit.Undefined;
    public BoundingBox BoundingBox {
      get {
        if (!_boundingBox.IsValid)
          _boundingBox = GetBoundingBox();
        return _boundingBox;
      }
    }
    private BoundingBox _boundingBox = BoundingBox.Empty;

    internal GsaAPI.Titles Titles {
      get {
        return Model.Titles();
      }
    }
    #endregion

    #region constructors
    public GsaModel() {
    }
    #endregion

    #region methods
    /// <summary>
    /// Clones this model so we can make changes safely
    /// </summary>
    /// <returns>Returns a clone of this model with a new GUID</returns>
    public GsaModel Clone() {
      var clone = new GsaModel {
        Model = Model.Clone(),
        FileNameAndPath = FileNameAndPath,
        ModelUnit = ModelUnit,
        Guid = Guid.NewGuid(),
        _boundingBox = _boundingBox,
      };
      return clone;
    }

    public GsaModel Duplicate(bool copy = false) {
      if (copy)
        return Clone();

      // create shallow copy
      var dup = new GsaModel { Model = Model };
      if (FileNameAndPath != null)
        dup.FileNameAndPath = FileNameAndPath;
      dup.Guid = new Guid(Guid.ToString());
      dup.ModelUnit = ModelUnit;
      dup._boundingBox = _boundingBox;
      return dup;
    }

    public override string ToString() {
      string s = "New GsaGH Model";
      if (Model != null && Titles != null) {
        if (!string.IsNullOrEmpty(FileNameAndPath))
          s = Path.GetFileName(FileNameAndPath).Replace(".gwb", string.Empty);

        if (Titles?.Title != null && Titles.Title != "") {
          if (s == "" || s == "Invalid")
            s = Titles.Title;
          else
            s += " {" + Titles.Title + "}";
        }
      }
      if (ModelUnit != LengthUnit.Undefined)
        s += " [" + Length.GetAbbreviation(ModelUnit) + "]";
      return s;
    }

    private BoundingBox GetBoundingBox() {
      var outNodes = new ConcurrentDictionary<int, Node>(Model.Nodes());
      var pts = new ConcurrentBag<Point3d>();
      Parallel.ForEach(outNodes, node => {
        pts.Add(Helpers.Import.Nodes.Point3dFromNode(node.Value, LengthUnit.Meter));
      });

      if (ModelUnit == LengthUnit.Undefined || ModelUnit == LengthUnit.Meter) {
        return new BoundingBox(pts);
      }

      double factor = 1 / new Length(1, ModelUnit).Meters;
      var scale = Transform.Scale(new Point3d(0, 0, 0), factor);
      return new BoundingBox(pts, scale);

    }
    #endregion
  }
}
