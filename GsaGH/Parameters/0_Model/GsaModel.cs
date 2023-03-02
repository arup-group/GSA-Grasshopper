using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Model class, this class defines the basic properties and methods for any Gsa Model
  /// </summary>
  [Serializable]
  public class GsaModel
  {
    #region properties
    public Model Model { get; set; } = new Model();
    public string FileNameAndPath { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public LengthUnit ModelUnit { get; set; } = LengthUnit.Undefined;
    public BoundingBox BoundingBox 
    { 
      get
      {
        if (!_boundingBox.IsValid)
          _boundingBox = this.GetBoundingBox();
        return _boundingBox;
      }
    }
    private BoundingBox _boundingBox = Rhino.Geometry.BoundingBox.Empty;

    internal GsaAPI.Titles Titles
    {
      get
      {
        return Model.Titles();
      }
    }
    #endregion

    #region constructors
    public GsaModel()
    {
    }
    #endregion

    #region methods
    /// <summary>
    /// Clones this model so we can make changes safely
    /// </summary>
    /// <returns>Returns a clone of this model with a new GUID</returns>
    public GsaModel Clone()
    {
      GsaModel clone = new GsaModel();
      clone.Model = this.Model.Clone();
      clone.FileNameAndPath = this.FileNameAndPath;
      clone.ModelUnit = this.ModelUnit;
      clone.Guid = Guid.NewGuid();
      clone._boundingBox = this._boundingBox;
      return clone;
    }

    public GsaModel Duplicate(bool copy = false)
    {
      if (copy)
        return this.Clone();

      // create shallow copy
      GsaModel dup = new GsaModel();
      dup.Model = this.Model;
      if (this.FileNameAndPath != null)
        dup.FileNameAndPath = this.FileNameAndPath.ToString();
      dup.Guid = new Guid(this.Guid.ToString());
      dup.ModelUnit = this.ModelUnit;
      dup._boundingBox = this._boundingBox;
      return dup;
    }

    public override string ToString()
    {
      string s = "New GsaGH Model";
      if (this.Model != null && this.Titles != null)
      {
        if (this.FileNameAndPath != null && this.FileNameAndPath != "")
          s = Path.GetFileName(this.FileNameAndPath).Replace(".gwb", string.Empty);
        
        if (Titles != null && Titles.Title != null && Titles.Title != "")
        {
          if (s == "" || s == "Invalid")
            s = Titles.Title;
          else
            s += " {" + Titles.Title + "}";
        }
      }
      if (this.ModelUnit != LengthUnit.Undefined)
        s += " [" + Length.GetAbbreviation(this.ModelUnit) + "]";
      return s;
    }

    private BoundingBox GetBoundingBox()
    {
      ConcurrentDictionary<int, Node> outNodes = new ConcurrentDictionary<int, Node>(this.Model.Nodes());
      ConcurrentBag<Point3d> pts = new ConcurrentBag<Point3d>();
      Parallel.ForEach(outNodes, node =>
      {
        pts.Add(Helpers.Import.Nodes.Point3dFromNode(node.Value, LengthUnit.Meter));
      });

      if (this.ModelUnit != LengthUnit.Undefined && this.ModelUnit != LengthUnit.Meter)
      {
        double factor = 1 / new Length(1, this.ModelUnit).Meters;
        Transform scale = Transform.Scale(new Point3d(0,0,0), factor);
        return new BoundingBox(pts, scale);
      }

      return new BoundingBox(pts);
    }
    #endregion
  }
}
