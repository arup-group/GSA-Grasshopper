using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml;

using GH_IO.Serialization;
using GH_IO.Types;

namespace GsaGHTests.Helpers.GH.Legend {
  // Mock classes for GH_IReader and GH_IWriter interfaces.
  public class MockWriter : GH_IWriter {
    public Dictionary<string, object> Data { get; private set; } = new Dictionary<string, object>();

    public void SetSingle(string item_name, int item_index, float item_value) { throw new NotImplementedException(); }

    public void SetDouble(string key, double value) {
      Data[key] = value;
    }

    public void SetDouble(string item_name, int item_index, double item_value) { throw new NotImplementedException(); }
    public void SetDecimal(string item_name, decimal item_value) { throw new NotImplementedException(); }

    public void SetDecimal(string item_name, int item_index, decimal item_value) {
      throw new NotImplementedException();
    }

    public void SetDate(string item_name, DateTime item_value) { throw new NotImplementedException(); }
    public void SetDate(string item_name, int item_index, DateTime item_value) { throw new NotImplementedException(); }
    public void SetGuid(string item_name, Guid item_value) { throw new NotImplementedException(); }
    public void SetGuid(string item_name, int item_index, Guid item_value) { throw new NotImplementedException(); }
    public void SetString(string item_name, string item_value) { throw new NotImplementedException(); }
    public void SetString(string item_name, int item_index, string item_value) { throw new NotImplementedException(); }
    public void SetPath(string item_name, string absolutePath, string basePath) { throw new NotImplementedException(); }

    public void SetPath(string item_name, int item_index, string absolutePath, string basePath) {
      throw new NotImplementedException();
    }

    public void SetDrawingPoint(string item_name, Point item_value) { throw new NotImplementedException(); }

    public void SetDrawingPoint(string item_name, int item_index, Point item_value) {
      throw new NotImplementedException();
    }

    public void SetDrawingPointF(string item_name, PointF item_value) { throw new NotImplementedException(); }

    public void SetDrawingPointF(string item_name, int item_index, PointF item_value) {
      throw new NotImplementedException();
    }

    public void SetDrawingSize(string item_name, Size item_value) { throw new NotImplementedException(); }

    public void SetDrawingSize(string item_name, int item_index, Size item_value) {
      throw new NotImplementedException();
    }

    public void SetDrawingSizeF(string item_name, SizeF item_value) { throw new NotImplementedException(); }

    public void SetDrawingSizeF(string item_name, int item_index, SizeF item_value) {
      throw new NotImplementedException();
    }

    public void SetDrawingRectangle(string item_name, Rectangle item_value) { throw new NotImplementedException(); }

    public void SetDrawingRectangle(string item_name, int item_index, Rectangle item_value) {
      throw new NotImplementedException();
    }

    public void SetDrawingRectangleF(string item_name, RectangleF item_value) { throw new NotImplementedException(); }

    public void SetDrawingRectangleF(string item_name, int item_index, RectangleF item_value) {
      throw new NotImplementedException();
    }

    public void SetDrawingColor(string item_name, Color item_value) { throw new NotImplementedException(); }

    public void SetDrawingColor(string item_name, int item_index, Color item_value) {
      throw new NotImplementedException();
    }

    public void SetDrawingBitmap(string item_name, Bitmap item_value) { throw new NotImplementedException(); }

    public void SetDrawingBitmap(string item_name, int item_index, Bitmap item_value) {
      throw new NotImplementedException();
    }

    public void SetByteArray(string item_name, byte[] item_value) { throw new NotImplementedException(); }

    public void SetByteArray(string item_name, int item_index, byte[] item_value) {
      throw new NotImplementedException();
    }

    public void SetDoubleArray(string item_name, double[] item_value) { throw new NotImplementedException(); }

    public void SetDoubleArray(string item_name, int item_index, double[] item_value) {
      throw new NotImplementedException();
    }

    public void SetPoint2D(string item_name, GH_Point2D item_value) { throw new NotImplementedException(); }

    public void SetPoint2D(string item_name, int item_index, GH_Point2D item_value) {
      throw new NotImplementedException();
    }

    public void SetPoint3D(string item_name, GH_Point3D item_value) { throw new NotImplementedException(); }

    public void SetPoint3D(string item_name, int item_index, GH_Point3D item_value) {
      throw new NotImplementedException();
    }

    public void SetPoint4D(string item_name, GH_Point4D item_value) { throw new NotImplementedException(); }

    public void SetPoint4D(string item_name, int item_index, GH_Point4D item_value) {
      throw new NotImplementedException();
    }

    public void SetInterval1D(string item_name, GH_Interval1D item_value) { throw new NotImplementedException(); }

    public void SetInterval1D(string item_name, int item_index, GH_Interval1D item_value) {
      throw new NotImplementedException();
    }

    public void SetInterval2D(string item_name, GH_Interval2D item_value) { throw new NotImplementedException(); }

    public void SetInterval2D(string item_name, int item_index, GH_Interval2D item_value) {
      throw new NotImplementedException();
    }

    public void SetLine(string item_name, GH_Line item_value) { throw new NotImplementedException(); }
    public void SetLine(string item_name, int item_index, GH_Line item_value) { throw new NotImplementedException(); }
    public void SetBoundingBox(string item_name, GH_BoundingBox item_value) { throw new NotImplementedException(); }

    public void SetBoundingBox(string item_name, int item_index, GH_BoundingBox item_value) {
      throw new NotImplementedException();
    }

    public void SetPlane(string item_name, GH_Plane item_value) { throw new NotImplementedException(); }
    public void SetPlane(string item_name, int item_index, GH_Plane item_value) { throw new NotImplementedException(); }
    public void SetVersion(string item_name, GH_Version item_value) { throw new NotImplementedException(); }

    public void SetVersion(string item_name, int item_index, GH_Version item_value) {
      throw new NotImplementedException();
    }

    public void SetVersion(string item_name, int major, int minor, int revision) {
      throw new NotImplementedException();
    }

    public void SetVersion(string item_name, int item_index, int major, int minor, int revision) {
      throw new NotImplementedException();
    }

    public void AddComment(string comment_text) { throw new NotImplementedException(); }
    public GH_IWriter CreateChunk(string chunk_name) { throw new NotImplementedException(); }
    public GH_IWriter CreateChunk(string chunk_name, int chunk_index) { throw new NotImplementedException(); }
    public bool RemoveChunk(string chunk_name) { throw new NotImplementedException(); }
    public bool RemoveChunk(string chunk_name, int chunk_index) { throw new NotImplementedException(); }
    public bool RemoveChunk(GH_IChunk chunk) { throw new NotImplementedException(); }
    public bool RemoveItem(string itemName) { throw new NotImplementedException(); }
    public bool RemoveItem(string itemName, int itemIndex) { throw new NotImplementedException(); }

    public void SetBoolean(string key, bool value) {
      Data[key] = value;
    }

    public void SetBoolean(string item_name, int item_index, bool item_value) { throw new NotImplementedException(); }
    public void SetByte(string item_name, byte item_value) { throw new NotImplementedException(); }
    public void SetByte(string item_name, int item_index, byte item_value) { throw new NotImplementedException(); }
    public void SetInt32(string item_name, int item_value) { throw new NotImplementedException(); }
    public void SetInt32(string item_name, int item_index, int item_value) { throw new NotImplementedException(); }
    public void SetInt64(string item_name, long item_value) { throw new NotImplementedException(); }
    public void SetInt64(string item_name, int item_index, long item_value) { throw new NotImplementedException(); }
    public void SetSingle(string item_name, float item_value) { throw new NotImplementedException(); }

    public void Write(BinaryWriter writer) { throw new NotImplementedException(); }
    public void Read(BinaryReader reader) { throw new NotImplementedException(); }
    public void Write(XmlWriter writer) { throw new NotImplementedException(); }
    public void Read(XmlNode node) { throw new NotImplementedException(); }
    public void AddMessage(string m, GH_Message_Type t) { throw new NotImplementedException(); }
    public GH_Archive Archive { get; }
    public string ArchiveLocation { get; }
    public string Name { get; }
    public int Index { get; }
    public int ItemCount { get; }
    public List<GH_Item> Items { get; }
    public int ChunkCount { get; }
    public List<GH_IChunk> Chunks { get; }
  }

  public class MockReader : GH_IReader {
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

    public float GetSingle(string item_name, int item_index) { throw new NotImplementedException(); }

    public double GetDouble(string key) {
      return (double)Data[key];
    }

    public double GetDouble(string item_name, int item_index) { throw new NotImplementedException(); }
    public decimal GetDecimal(string item_name) { throw new NotImplementedException(); }
    public decimal GetDecimal(string item_name, int item_index) { throw new NotImplementedException(); }
    public DateTime GetDate(string item_name) { throw new NotImplementedException(); }
    public DateTime GetDate(string item_name, int item_index) { throw new NotImplementedException(); }
    public Guid GetGuid(string item_name) { throw new NotImplementedException(); }
    public Guid GetGuid(string item_name, int item_index) { throw new NotImplementedException(); }
    public string GetString(string item_name) { throw new NotImplementedException(); }
    public string GetString(string item_name, int item_index) { throw new NotImplementedException(); }
    public string[] GetPath(string item_name, string basePath) { throw new NotImplementedException(); }
    public string[] GetPath(string item_name, int item_index, string basePath) { throw new NotImplementedException(); }
    public Point GetDrawingPoint(string item_name) { throw new NotImplementedException(); }
    public Point GetDrawingPoint(string item_name, int item_index) { throw new NotImplementedException(); }
    public PointF GetDrawingPointF(string item_name) { throw new NotImplementedException(); }
    public PointF GetDrawingPointF(string item_name, int item_index) { throw new NotImplementedException(); }
    public Size GetDrawingSize(string item_name) { throw new NotImplementedException(); }
    public Size GetDrawingSize(string item_name, int item_index) { throw new NotImplementedException(); }
    public SizeF GetDrawingSizeF(string item_name) { throw new NotImplementedException(); }
    public SizeF GetDrawingSizeF(string item_name, int item_index) { throw new NotImplementedException(); }
    public Rectangle GetDrawingRectangle(string item_name) { throw new NotImplementedException(); }
    public Rectangle GetDrawingRectangle(string item_name, int item_index) { throw new NotImplementedException(); }
    public RectangleF GetDrawingRectangleF(string item_name) { throw new NotImplementedException(); }
    public RectangleF GetDrawingRectangleF(string item_name, int item_index) { throw new NotImplementedException(); }
    public Color GetDrawingColor(string item_name) { throw new NotImplementedException(); }
    public Color GetDrawingColor(string item_name, int item_index) { throw new NotImplementedException(); }
    public Bitmap GetDrawingBitmap(string item_name) { throw new NotImplementedException(); }
    public Bitmap GetDrawingBitmap(string item_name, int item_index) { throw new NotImplementedException(); }
    public byte[] GetByteArray(string item_name) { throw new NotImplementedException(); }
    public byte[] GetByteArray(string item_name, int item_index) { throw new NotImplementedException(); }
    public double[] GetDoubleArray(string item_name) { throw new NotImplementedException(); }
    public double[] GetDoubleArray(string item_name, int item_index) { throw new NotImplementedException(); }
    public GH_Point2D GetPoint2D(string item_name) { throw new NotImplementedException(); }
    public GH_Point2D GetPoint2D(string item_name, int item_index) { throw new NotImplementedException(); }
    public GH_Point3D GetPoint3D(string item_name) { throw new NotImplementedException(); }
    public GH_Point3D GetPoint3D(string item_name, int item_index) { throw new NotImplementedException(); }
    public GH_Point4D GetPoint4D(string item_name) { throw new NotImplementedException(); }
    public GH_Point4D GetPoint4D(string item_name, int item_index) { throw new NotImplementedException(); }
    public GH_Interval1D GetInterval1D(string item_name) { throw new NotImplementedException(); }
    public GH_Interval1D GetInterval1D(string item_name, int item_index) { throw new NotImplementedException(); }
    public GH_Interval2D GetInterval2D(string item_name) { throw new NotImplementedException(); }
    public GH_Interval2D GetInterval2D(string item_name, int item_index) { throw new NotImplementedException(); }
    public GH_Line GetLine(string item_name) { throw new NotImplementedException(); }
    public GH_Line GetLine(string item_name, int item_index) { throw new NotImplementedException(); }
    public GH_BoundingBox GetBoundingBox(string item_name) { throw new NotImplementedException(); }
    public GH_BoundingBox GetBoundingBox(string item_name, int item_index) { throw new NotImplementedException(); }
    public GH_Plane GetPlane(string item_name) { throw new NotImplementedException(); }
    public GH_Plane GetPlane(string item_name, int item_index) { throw new NotImplementedException(); }
    public GH_Version GetVersion(string item_name) { throw new NotImplementedException(); }
    public GH_Version GetVersion(string item_name, int item_index) { throw new NotImplementedException(); }
    public bool TryGetBoolean(string item_name, ref bool value) { throw new NotImplementedException(); }
    public bool TryGetBoolean(string item_name, int item_index, ref bool value) { throw new NotImplementedException(); }
    public bool TryGetByte(string item_name, ref byte value) { throw new NotImplementedException(); }
    public bool TryGetByte(string item_name, int item_index, ref byte value) { throw new NotImplementedException(); }
    public bool TryGetInt32(string item_name, ref int value) { throw new NotImplementedException(); }
    public bool TryGetInt32(string item_name, int item_index, ref int value) { throw new NotImplementedException(); }
    public bool TryGetInt64(string item_name, ref long value) { throw new NotImplementedException(); }
    public bool TryGetInt64(string item_name, int item_index, ref long value) { throw new NotImplementedException(); }
    public bool TryGetSingle(string item_name, ref float value) { throw new NotImplementedException(); }
    public bool TryGetSingle(string item_name, int item_index, ref float value) { throw new NotImplementedException(); }
    public bool TryGetDouble(string item_name, ref double value) { throw new NotImplementedException(); }

    public bool TryGetDouble(string item_name, int item_index, ref double value) {
      throw new NotImplementedException();
    }

    public bool TryGetDecimal(string item_name, ref decimal value) { throw new NotImplementedException(); }

    public bool TryGetDecimal(string item_name, int item_index, ref decimal value) {
      throw new NotImplementedException();
    }

    public bool TryGetDate(string item_name, ref DateTime value) { throw new NotImplementedException(); }

    public bool TryGetDate(string item_name, int item_index, ref DateTime value) {
      throw new NotImplementedException();
    }

    public bool TryGetGuid(string item_name, ref Guid value) { throw new NotImplementedException(); }
    public bool TryGetGuid(string item_name, int item_index, ref Guid value) { throw new NotImplementedException(); }
    public bool TryGetString(string item_name, ref string value) { throw new NotImplementedException(); }

    public bool TryGetString(string item_name, int item_index, ref string value) {
      throw new NotImplementedException();
    }

    public bool TryGetDrawingPoint(string item_name, ref Point value) { throw new NotImplementedException(); }

    public bool TryGetDrawingPoint(string item_name, int item_index, ref Point value) {
      throw new NotImplementedException();
    }

    public bool TryGetDrawingPointF(string item_name, ref PointF value) { throw new NotImplementedException(); }

    public bool TryGetDrawingPointF(string item_name, int item_index, ref PointF value) {
      throw new NotImplementedException();
    }

    public bool TryGetDrawingSize(string item_name, ref Size value) { throw new NotImplementedException(); }

    public bool TryGetDrawingSize(string item_name, int item_index, ref Size value) {
      throw new NotImplementedException();
    }

    public bool TryGetDrawingSizeF(string item_name, ref SizeF value) { throw new NotImplementedException(); }

    public bool TryGetDrawingSizeF(string item_name, int item_index, ref SizeF value) {
      throw new NotImplementedException();
    }

    public bool TryGetDrawingRectangle(string item_name, ref Rectangle value) { throw new NotImplementedException(); }

    public bool TryGetDrawingRectangle(string item_name, int item_index, ref Rectangle value) {
      throw new NotImplementedException();
    }

    public bool TryGetDrawingRectangleF(string item_name, ref RectangleF value) { throw new NotImplementedException(); }

    public bool TryGetDrawingRectangleF(string item_name, int item_index, ref RectangleF value) {
      throw new NotImplementedException();
    }

    public bool TryGetDrawingColor(string item_name, ref Color value) { throw new NotImplementedException(); }

    public bool TryGetDrawingColor(string item_name, int item_index, ref Color value) {
      throw new NotImplementedException();
    }

    public bool TryGetPoint2D(string item_name, ref GH_Point2D value) { throw new NotImplementedException(); }

    public bool TryGetPoint2D(string item_name, int item_index, ref GH_Point2D value) {
      throw new NotImplementedException();
    }

    public bool TryGetPoint3D(string item_name, ref GH_Point3D value) { throw new NotImplementedException(); }

    public bool TryGetPoint3D(string item_name, int item_index, ref GH_Point3D value) {
      throw new NotImplementedException();
    }

    public bool TryGetPoint4D(string item_name, ref GH_Point4D value) { throw new NotImplementedException(); }

    public bool TryGetPoint4D(string item_name, int item_index, ref GH_Point4D value) {
      throw new NotImplementedException();
    }

    public bool TryGetInterval1D(string item_name, ref GH_Interval1D value) { throw new NotImplementedException(); }

    public bool TryGetInterval1D(string item_name, int item_index, ref GH_Interval1D value) {
      throw new NotImplementedException();
    }

    public bool TryGetInterval2D(string item_name, ref GH_Interval2D value) { throw new NotImplementedException(); }

    public bool TryGetInterval2D(string item_name, int item_index, ref GH_Interval2D value) {
      throw new NotImplementedException();
    }

    public bool TryGetLine(string item_name, ref GH_Line value) { throw new NotImplementedException(); }
    public bool TryGetLine(string item_name, int item_index, ref GH_Line value) { throw new NotImplementedException(); }
    public bool TryGetBoundingBox(string item_name, ref GH_BoundingBox value) { throw new NotImplementedException(); }

    public bool TryGetBoundingBox(string item_name, int item_index, ref GH_BoundingBox value) {
      throw new NotImplementedException();
    }

    public bool TryGetPlane(string item_name, ref GH_Plane value) { throw new NotImplementedException(); }

    public bool TryGetPlane(string item_name, int item_index, ref GH_Plane value) {
      throw new NotImplementedException();
    }

    public bool TryGetVersion(string item_name, ref GH_Version value) { throw new NotImplementedException(); }

    public bool TryGetVersion(string item_name, int item_index, ref GH_Version value) {
      throw new NotImplementedException();
    }

    public bool ItemExists(string name, int index) { throw new NotImplementedException(); }

    public bool GetBoolean(string key) {
      return (bool)Data[key];
    }

    public bool GetBoolean(string item_name, int item_index) { throw new NotImplementedException(); }
    public byte GetByte(string item_name) { throw new NotImplementedException(); }
    public byte GetByte(string item_name, int item_index) { throw new NotImplementedException(); }
    public int GetInt32(string item_name) { throw new NotImplementedException(); }
    public int GetInt32(string item_name, int item_index) { throw new NotImplementedException(); }
    public long GetInt64(string item_name) { throw new NotImplementedException(); }
    public long GetInt64(string item_name, int item_index) { throw new NotImplementedException(); }
    public float GetSingle(string item_name) { throw new NotImplementedException(); }
    public GH_IReader FindChunk(string name) { throw new NotImplementedException(); }
    public GH_IReader FindChunk(string name, int index) { throw new NotImplementedException(); }
    public bool ChunkExists(string name) { throw new NotImplementedException(); }
    public bool ChunkExists(string name, int index) { throw new NotImplementedException(); }
    public GH_Item FindItem(string name) { throw new NotImplementedException(); }
    public GH_Item FindItem(string name, int index) { throw new NotImplementedException(); }

    public bool ItemExists(string key) {
      return Data.ContainsKey(key);
    }

    // Add other required methods as needed.
    public void Write(BinaryWriter writer) { throw new NotImplementedException(); }
    public void Read(BinaryReader reader) { throw new NotImplementedException(); }
    public void Write(XmlWriter writer) { throw new NotImplementedException(); }
    public void Read(XmlNode node) { throw new NotImplementedException(); }
    public void AddMessage(string m, GH_Message_Type t) { throw new NotImplementedException(); }
    public GH_Archive Archive { get; }
    public string ArchiveLocation { get; }
    public string Name { get; }
    public int Index { get; }
    public int ItemCount { get; }
    public List<GH_Item> Items { get; }
    public int ChunkCount { get; }
    public List<GH_IChunk> Chunks { get; }
  }
}
