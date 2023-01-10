using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaAPI;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create AdSec profile
  /// </summary>
  public class CreateProfile : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("ea1741e5-905e-4ecb-8270-a584e3f99aa3");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateProfile;


    public CreateProfile()
      : base("Create Profile", "Profile", "Create Profile text-string for a GSA Section",
            CategoryName.Name(),
            SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden

    protected override string HtmlHelp_Source()
    {
      string help = "GOTO:https://arup-group.github.io/oasys-combined/adsec-api/api/Oasys.Profiles.html";
      return help;
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter("Width [" + unitAbbreviation + "]", "B", "Profile width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth [" + unitAbbreviation + "]", "H", "Profile depth", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("Profile", "Pf", "Profile for a GSA Section", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      this.ClearRuntimeMessages();
      for (int i = 0; i < this.Params.Input.Count; i++)
        this.Params.Input[i].ClearRuntimeMessages();

      #region catalogue
      this.ClearRuntimeMessages();
      if (_mode == FoldMode.Catalogue)
      {
        // get user input filter search string
        bool incl = false;
        if (DA.GetData(1, ref incl))
        {
          if (_inclSS != incl)
          {
            SetSelected(-1, 0);
            this.ExpireSolution(true);
          }
        }

        // get user input filter search string
        string inSearch = "";
        if (DA.GetData(0, ref inSearch))
        {
          inSearch = inSearch.ToLower();

        }
        if (!inSearch.Equals(_search))
        {
          _search = inSearch.ToString();
          SetSelected(-1, 0);
          this.ExpireSolution(true);
        }

        DA.SetData(0, "CAT " + profileString);

        return;
      }
      #endregion

      #region std
      if (_mode == FoldMode.Other)
      {
        string unit = "(" + Length.GetAbbreviation(this.LengthUnit, new CultureInfo("en")) + ") ";
        string profile = "STD ";
        // angle
        if (typ == "IAngleProfile") //(typ.Name.Equals(typeof(IAngleProfile).Name))
        {
          profile += "A" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString();

          //profile = IAngleProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    Input.Web(this, DA, 2));
        }

        // channel
        else if (typ == "IChannelProfile") //(typ.Name.Equals(typeof(IChannelProfile).Name))
        {
          profile += "CH" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString();

          //profile = IChannelProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    (IWebConstant)Input.Web(this, DA, 2));
        }

        // circle hollow
        else if (typ == "ICircleHollowProfile") //(typ.Name.Equals(typeof(ICircleHollowProfile).Name))
        {
          profile += "CHS" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString();

          //profile = ICircleHollowProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit));
        }

        // circle
        else if (typ == "ICircleProfile") //(typ.Name.Equals(typeof(ICircleProfile).Name))
        {
          profile += "C" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString();

          //profile = ICircleProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit));
        }

        // ICruciformSymmetricalProfile
        else if (typ == "ICruciformSymmetricalProfile") //(typ.Name.Equals(typeof(ICruciformSymmetricalProfile).Name))
        {
          profile += "X" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString();

          //profile = ICruciformSymmetricalProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    (IWebConstant)Input.Web(this, DA, 2));
        }

        // IEllipseHollowProfile
        else if (typ == "IEllipseHollowProfile") //(typ.Name.Equals(typeof(IEllipseHollowProfile).Name))
        {
          profile += "OVAL" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString();

          //profile = IEllipseHollowProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit));
        }

        // IEllipseProfile
        else if (typ == "IEllipseProfile") //(typ.Name.Equals(typeof(IEllipseProfile).Name))
        {
          profile += "E" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " 2";

          //profile = IEllipseProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit));
        }

        // IGeneralCProfile
        else if (typ == "IGeneralCProfile") //(typ.Name.Equals(typeof(IGeneralCProfile).Name))
        {
          profile += "GC" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString();

          //profile = IGeneralCProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 3, lengthUnit));
        }

        // IGeneralZProfile
        else if (typ == "IGeneralZProfile") //(typ.Name.Equals(typeof(IGeneralZProfile).Name))
        {
          profile += "GZ" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 4, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 5, LengthUnit).As(LengthUnit).ToString();

          //profile = IGeneralZProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 3, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 4, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 5, lengthUnit));
        }

        // IIBeamAsymmetricalProfile
        else if (typ == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
        {
          profile += "GI" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 4, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 5, LengthUnit).As(LengthUnit).ToString();

          //profile = IIBeamAsymmetricalProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    Input.Flange(this, DA, 2),
          //    Input.Web(this, DA, 3));
        }

        // IIBeamCellularProfile
        else if (typ == "IIBeamCellularProfile") //(typ.Name.Equals(typeof(IIBeamCellularProfile).Name))
        {
          profile += "CB" + unit +
             Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 4, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 5, LengthUnit).As(LengthUnit).ToString();

          //profile = IIBeamCellularProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    (IWebConstant)Input.Web(this, DA, 2),
          //    Input.LengthOrRatio(this, DA, 3, lengthUnit));
        }

        // IIBeamSymmetricalProfile
        else if (typ == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
        {
          profile += "I" + unit +
             Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString();

          //profile = IIBeamSymmetricalProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    (IWebConstant)Input.Web(this, DA, 2));
        }

        // IRectangleHollowProfile
        else if (typ == "IRectangleHollowProfile") //(typ.Name.Equals(typeof(IRectangleHollowProfile).Name))
        {
          profile += "RHS" + unit +
             Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString();

          //profile = IRectangleHollowProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    (IWebConstant)Input.Web(this, DA, 2));
        }

        // IRectangleProfile
        else if (typ == "IRectangleProfile") //(typ.Name.Equals(typeof(IRectangleProfile).Name))
        {
          profile += "R" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString();

          //profile = IRectangleProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit));
        }

        // IRectoEllipseProfile
        else if (typ == "IRectoEllipseProfile") //(typ.Name.Equals(typeof(IRectoEllipseProfile).Name))
        {
          profile += "RE" + unit +
             Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString() + " 2";

          //profile = IRectoEllipseProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 3, lengthUnit));
        }

        // ISecantPileProfile
        else if (typ == "ISecantPileProfile") //(typ.Name.Equals(typeof(ISecantPileProfile).Name))
        {
          int pileCount = 0;
          if (!DA.GetData(2, ref pileCount))
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert input PileCount to integer.");
            return;
          }

          bool isWallNotSection = false;
          if (!DA.GetData(3, ref isWallNotSection))
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert input isWall to boolean.");
            return;
          }

          profile += (isWallNotSection ? "SP" : "SPW") + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
              pileCount.ToString();

          //profile = ISecantPileProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    pileCount, isWallNotSection);
        }

        // ISheetPileProfile
        else if (typ == "ISheetPileProfile") //(typ.Name.Equals(typeof(ISheetPileProfile).Name))
        {
          profile += "SHT" + unit +
             Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 4, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 5, LengthUnit).As(LengthUnit).ToString();

          //profile = ISheetPileProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 3, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 4, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 5, lengthUnit));
        }

        // IStadiumProfile
        else if (typ == "IStadiumProfile") //(typ.Name.Equals(typeof(IStadiumProfile).Name))
        {
          profile += "RC" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString();

          //profile = IStadiumProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit));
        }

        // ITrapezoidProfile
        else if (typ == "ITrapezoidProfile") //(typ.Name.Equals(typeof(ITrapezoidProfile).Name))
        {
          profile += "TR" + unit +
              Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString();

          //profile = ITrapezoidProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit));
        }

        // ITSectionProfile
        else if (typ == "ITSectionProfile") //(typ.Name.Equals(typeof(ITSectionProfile).Name))
        {
          profile += "T" + unit +
             Input.LengthOrRatio(this, DA, 0, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 1, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 2, LengthUnit).As(LengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 3, LengthUnit).As(LengthUnit).ToString();

          //profile = ITSectionProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    Input.Web(this, DA, 2));
        }

        // IPerimeterProfile (last chance...)
        else if (typ == "IPerimeterProfile") //(typ.Name.Equals(typeof(IPerimeterProfile).Name))
        {
          ProfileHelper perimeter = new ProfileHelper();
          perimeter.profileType = ProfileHelper.ProfileTypes.Geometric;
          GH_Brep gh_Brep = new GH_Brep();
          if (DA.GetData(0, ref gh_Brep))
          {
            Brep brep = new Brep();
            if (GH_Convert.ToBrep(gh_Brep, ref brep, GH_Conversion.Both))
            {
              // get edge curves from Brep
              Curve[] edgeSegments = brep.DuplicateEdgeCurves();
              Curve[] edges = Curve.JoinCurves(edgeSegments);

              // find the best fit plane
              List<Point3d> ctrl_pts = new List<Point3d>();
              if (edges[0].TryGetPolyline(out Polyline tempCrv))
                ctrl_pts = tempCrv.ToList();
              else
              {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert edge to Polyline");
                return;
              }

              bool localPlaneNotSet = true;
              Plane plane = Plane.Unset;
              if (DA.GetData(1, ref plane))
                localPlaneNotSet = false;

              Point3d origin = new Point3d();
              if (localPlaneNotSet)
              {
                foreach (Point3d p in ctrl_pts)
                {
                  origin.X += p.X;
                  origin.Y += p.Y;
                  origin.Z += p.Z;
                }
                origin.X = origin.X / ctrl_pts.Count;
                origin.Y = origin.Y / ctrl_pts.Count;
                origin.Z = origin.Z / ctrl_pts.Count;

                Plane.FitPlaneToPoints(ctrl_pts, out plane);

                Vector3d xDirection = new Vector3d(
                  Math.Abs(plane.XAxis.X),
                  Math.Abs(plane.XAxis.Y),
                  Math.Abs(plane.XAxis.Z));
                xDirection.Unitize();
                Vector3d yDirection = new Vector3d(
                  Math.Abs(plane.YAxis.X),
                  Math.Abs(plane.YAxis.Y),
                  Math.Abs(plane.YAxis.Z));
                xDirection.Unitize();

                Vector3d normal = plane.Normal;
                normal.Unitize();
                if (normal.X == 1)
                  plane = Plane.WorldYZ;
                else if (normal.Y == 1)
                  plane = Plane.WorldZX;
                else if (normal.Z == 1)
                  plane = Plane.WorldXY;
                else
                  plane = new Plane(Point3d.Origin, xDirection, yDirection);
                plane.Origin = origin;

                //double x1 = ctrl_pts[ctrl_pts.Count - 2].X - origin.X;
                //double y1 = ctrl_pts[ctrl_pts.Count - 2].Y - origin.Y;
                //double z1 = ctrl_pts[ctrl_pts.Count - 2].Z - origin.Z;
                //Vector3d xDirection = new Vector3d(x1, y1, z1);
                //xDirection.Unitize();

                //double x2 = ctrl_pts[1].X - origin.X;
                //double y2 = ctrl_pts[1].Y - origin.Y;
                //double z2 = ctrl_pts[1].Z - origin.Z;
                //Vector3d yDirection = new Vector3d(x2, y2, z2);
                //yDirection.Unitize();

                //plane = new Plane(Point3d.Origin, xDirection, yDirection);
              }
              else
              {
                origin = plane.Origin;
              }

              Transform translation = Transform.Translation(-origin.X, -origin.Y, -origin.Z);
              Transform rotation = Transform.ChangeBasis(Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, plane.XAxis, plane.YAxis, plane.ZAxis);
              if (localPlaneNotSet)
                rotation = Transform.ChangeBasis(Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, plane.YAxis, plane.XAxis, plane.ZAxis);

              perimeter.geoType = ProfileHelper.GeoTypes.Perim;

              List<Point2d> pts = new List<Point2d>();
              foreach (Point3d pt3d in ctrl_pts)
              {
                pt3d.Transform(translation);
                pt3d.Transform(rotation);
                Point2d pt2d = new Point2d(pt3d);
                pts.Add(pt2d);
              }
              perimeter.perimeterPoints = pts;

              if (edges.Length > 1)
              {
                List<List<Point2d>> voidPoints = new List<List<Point2d>>();
                for (int i = 1; i < edges.Length; i++)
                {
                  ctrl_pts.Clear();
                  if (!edges[i].IsPlanar())
                  {
                    for (int j = 0; j < edges.Length; j++)
                      edges[j] = Curve.ProjectToPlane(edges[j], plane);
                  }
                  if (edges[i].TryGetPolyline(out tempCrv))
                  {
                    ctrl_pts = tempCrv.ToList();
                    pts = new List<Point2d>();
                    foreach (Point3d pt3d in ctrl_pts)
                    {
                      pt3d.Transform(translation);
                      pt3d.Transform(rotation);
                      Point2d pt2d = new Point2d(pt3d);
                      pts.Add(pt2d);
                    }
                    voidPoints.Add(pts);
                  }
                  else
                  {
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert internal edge  to Polyline");
                    return;
                  }
                }
                perimeter.voidPoints = voidPoints;
              }
            }
          }
          switch (LengthUnit)
          {
            case LengthUnit.Millimeter:
              perimeter.sectUnit = ProfileHelper.SectUnitOptions.u_mm;
              break;
            case LengthUnit.Centimeter:
              perimeter.sectUnit = ProfileHelper.SectUnitOptions.u_cm;
              break;
            case LengthUnit.Meter:
              perimeter.sectUnit = ProfileHelper.SectUnitOptions.u_m;
              break;
            case LengthUnit.Foot:
              perimeter.sectUnit = ProfileHelper.SectUnitOptions.u_ft;
              break;
            case LengthUnit.Inch:
              perimeter.sectUnit = ProfileHelper.SectUnitOptions.u_in;
              break;
          }

          DA.SetData(0, ConvertSection.ProfileConversion(perimeter));
          //profile = Input.Boundaries(this, DA, 0, 1, lengthUnit);
          //DA.SetData(0, Input.Boundaries(this, DA, 0, 1, lengthUnit));
          return;
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to create profile");
          return;
        }

        //try
        //{
        //    Oasys.Collections.IList<Oasys.AdSec.IWarning> warn = profile.Validate();
        //    foreach (Oasys.AdSec.IWarning warning in warn)
        //        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, warning.Description);
        //}
        //catch (Exception e)
        //{
        //    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, e.Message);
        //    return;
        //}

        DA.SetData(0, profile);
        return;
      }

      #endregion
    }

    #region menu override
    private enum FoldMode
    {
      Catalogue,
      Other
    }
    private FoldMode _mode = FoldMode.Other;

    private void Mode1Clicked()
    {
      //remove input parameters
      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);

      //register input parameter
      Params.RegisterInputParam(new Param_String());
      Params.RegisterInputParam(new Param_Boolean());

      _mode = FoldMode.Catalogue;

      base.UpdateUI();
    }
    private void SetNumberOfGenericInputs(int inputs, bool isSecantPile = false, bool isPerimeter = false)
    {
      this._numberOfInputs = inputs;

      // if last input previously was a bool and we no longer need that
      if (lastInputWasSecant || isSecantPile || isPerimeter)
      {
        if (Params.Input.Count > 0)
        {
          // make sure to remove last param
          Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
          Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
        }
      }

      // remove any additional inputs
      while (Params.Input.Count > inputs)
        Params.UnregisterInputParameter(Params.Input[inputs], true);

      if (isSecantPile) // add two less generic than input says
      {
        while (Params.Input.Count > inputs + 2)
          Params.UnregisterInputParameter(Params.Input[inputs + 2], true);
        inputs -= 2;
      }

      // add inputs parameter
      while (Params.Input.Count < inputs)
        Params.RegisterInputParam(new Param_GenericObject());

      if (isSecantPile) // finally add int and bool param if secant
      {
        Params.RegisterInputParam(new Param_Integer());
        Params.RegisterInputParam(new Param_Boolean());
        lastInputWasSecant = true;
      }
      else
        lastInputWasSecant = false;

      if (isPerimeter)
        Params.RegisterInputParam(new Param_Plane());
    }
    private bool lastInputWasSecant;
    private int _numberOfInputs;
    // temporary 
    //private Type typ = typeof(IRectangleProfile);
    private string typ = "IRectangleProfile";
    private void Mode2Clicked()
    {
      // check if mode is correct
      if (_mode != FoldMode.Other)
      {
        // if we come from catalogue mode remove all input parameters
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        // set mode to other
        _mode = FoldMode.Other;
      }

      // angle
      if (typ == "IAngleProfile") //(typ.Name.Equals(typeof(IAngleProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IAngleProfile.Create(angle.Depth, angle.Flange, angle.Web);
      }

      // channel
      else if (typ == "IChannelProfile") //(typ.Name.Equals(typeof(IChannelProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IChannelProfile.Create(channel.Depth, channel.Flanges, channel.Web);
      }

      // circle hollow
      else if (typ == "ICircleHollowProfile") //(typ.Name.Equals(typeof(ICircleHollowProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = ICircleHollowProfile.Create(circleHollow.Diameter, circleHollow.WallThickness);
      }

      // circle
      else if (typ == "ICircleProfile") //(typ.Name.Equals(typeof(ICircleProfile).Name))
      {
        SetNumberOfGenericInputs(1);
        //dup = ICircleProfile.Create(circle.Diameter);
      }

      // ICruciformSymmetricalProfile
      else if (typ == "ICruciformSymmetricalProfile") //(typ.Name.Equals(typeof(ICruciformSymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = ICruciformSymmetricalProfile.Create(cruciformSymmetrical.Depth, cruciformSymmetrical.Flange, cruciformSymmetrical.Web);
      }

      // IEllipseHollowProfile
      else if (typ == "IEllipseHollowProfile") //(typ.Name.Equals(typeof(IEllipseHollowProfile).Name))
      {
        SetNumberOfGenericInputs(3);
        //dup = IEllipseHollowProfile.Create(ellipseHollow.Depth, ellipseHollow.Width, ellipseHollow.WallThickness);
      }

      // IEllipseProfile
      else if (typ == "IEllipseProfile") //(typ.Name.Equals(typeof(IEllipseProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IEllipseProfile.Create(ellipse.Depth, ellipse.Width);
      }

      // IGeneralCProfile
      else if (typ == "IGeneralCProfile") //(typ.Name.Equals(typeof(IGeneralCProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IGeneralCProfile.Create(generalC.Depth, generalC.FlangeWidth, generalC.Lip, generalC.Thickness);
      }

      // IGeneralZProfile
      else if (typ == "IGeneralZProfile") //(typ.Name.Equals(typeof(IGeneralZProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = IGeneralZProfile.Create(generalZ.Depth, generalZ.TopFlangeWidth, generalZ.BottomFlangeWidth, generalZ.TopLip, generalZ.BottomLip, generalZ.Thickness);
      }

      // IIBeamAsymmetricalProfile
      else if (typ == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = IIBeamAsymmetricalProfile.Create(iBeamAsymmetrical.Depth, iBeamAsymmetrical.TopFlange, iBeamAsymmetrical.BottomFlange, iBeamAsymmetrical.Web);
      }

      // IIBeamCellularProfile
      else if (typ == "IIBeamCellularProfile") //(typ.Name.Equals(typeof(IIBeamCellularProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = IIBeamCellularProfile.Create(iBeamCellular.Depth, iBeamCellular.Flanges, iBeamCellular.Web, iBeamCellular.WebOpening);
      }

      // IIBeamSymmetricalProfile
      else if (typ == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IIBeamSymmetricalProfile.Create(iBeamSymmetrical.Depth, iBeamSymmetrical.Flanges, iBeamSymmetrical.Web);
      }

      // IRectangleHollowProfile
      else if (typ == "IRectangleHollowProfile") //(typ.Name.Equals(typeof(IRectangleHollowProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IRectangleHollowProfile.Create(rectangleHollow.Depth, rectangleHollow.Flanges, rectangleHollow.Webs);
      }

      // IRectangleProfile
      else if (typ == "IRectangleProfile") //(typ.Name.Equals(typeof(IRectangleProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IRectangleProfile.Create(rectangle.Depth, rectangle.Width);
      }

      // IRectoEllipseProfile
      else if (typ == "IRectoEllipseProfile") //(typ.Name.Equals(typeof(IRectoEllipseProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IRectoEllipseProfile.Create(rectoEllipse.Depth, rectoEllipse.DepthFlat, rectoEllipse.Width, rectoEllipse.WidthFlat);
      }

      // ISecantPileProfile
      else if (typ == "ISecantPileProfile") //(typ.Name.Equals(typeof(ISecantPileProfile).Name))
      {
        SetNumberOfGenericInputs(4, true);
        //dup = ISecantPileProfile.Create(secantPile.Diameter, secantPile.PileCentres, secantPile.PileCount, secantPile.IsWallNotSection);
      }

      // ISheetPileProfile
      else if (typ == "ISheetPileProfile") //(typ.Name.Equals(typeof(ISheetPileProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = ISheetPileProfile.Create(sheetPile.Depth, sheetPile.Width, sheetPile.TopFlangeWidth, sheetPile.BottomFlangeWidth, sheetPile.FlangeThickness, sheetPile.WebThickness);
      }

      // IStadiumProfile
      else if (typ == "IStadiumProfile") //(typ.Name.Equals(typeof(IStadiumProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IStadiumProfile.Create(stadium.Depth, stadium.Width);
      }

      // ITrapezoidProfile
      else if (typ == "ITrapezoidProfile") //(typ.Name.Equals(typeof(ITrapezoidProfile).Name))
      {
        SetNumberOfGenericInputs(3);
        //dup = ITrapezoidProfile.Create(trapezoid.Depth, trapezoid.TopWidth, trapezoid.BottomWidth);
      }

      // ITSectionProfile
      else if (typ == "ITSectionProfile") //(typ.Name.Equals(typeof(ITSectionProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = ITSectionProfile.Create(tSection.Depth, tSection.Flange, tSection.Web);
      }
      // IPerimeterProfile
      else if (typ == "IPerimeterProfile") //(typ.Name.Equals(typeof(IPerimeterProfile).Name))
      {
        SetNumberOfGenericInputs(1, false, true);

        //dup = IPerimeterProfile.Create();
        //solidPolygon;
        //voidPolygons;
      }

        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    #endregion

    #region Custom UI

    //List<string> excludedInterfaces = new List<string>(new string[]
    //{
    //    "IProfile", "IPoint", "IPolygon", "IFlange", "IWeb", "IWebConstant", "IWebTapered", "ITrapezoidProfileAbstractInterface", "IIBeamProfile"
    //});

    // temporary manual implementation of profile types (to be replaced by reflection of Oasys.Profiles)
    //Dictionary<string, Type> profileTypes;
    private static Dictionary<string, string> _profileTypes = new Dictionary<string, string>
        {
            { "Angle", "IAngleProfile" },
            { "Catalogue", "ICatalogueProfile" },
            { "Channel", "IChannelProfile" },
            { "Circle Hollow", "ICircleHollowProfile" },
            { "Circle", "ICircleProfile" },
            { "Cruciform Symmetrical", "ICruciformSymmetricalProfile" },
            { "Ellipse Hollow", "IEllipseHollowProfile" },
            { "Ellipse", "IEllipseProfile" },
            { "General C", "IGeneralCProfile" },
            { "General Z", "IGeneralZProfile" },
            { "I Beam Asymmetrical", "IIBeamAsymmetricalProfile" },
            { "I Beam Cellular", "IIBeamCellularProfile" },
            { "I Beam Symmetrical", "IIBeamSymmetricalProfile" },
            { "Perimeter", "IPerimeterProfile" },
            { "Rectangle Hollow", "IRectangleHollowProfile" },
            { "Rectangle", "IRectangleProfile" },
            { "Recto Ellipse", "IRectoEllipseProfile" },
            { "Recto Circle", "IStadiumProfile" },
            { "Secant Pile", "ISecantPileProfile" },
            { "Sheet Pile", "ISheetPileProfile" },
            { "Trapezoid", "ITrapezoidProfile" },
            { "T Section", "ITSectionProfile" },
        };

    private LengthUnit LengthUnit = DefaultUnits.LengthUnitSection;

    // for catalogue selection
    // Catalogues
    private readonly Tuple<List<string>, List<int>> _cataloguedata = SqlReader.Instance.GetCataloguesDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
    private List<int> _catalogueNumbers = new List<int>(); // internal db catalogue numbers
    private List<string> _catalogueNames = new List<string>(); // list of displayed catalogues
    bool _inclSS;

    // Types
    private Tuple<List<string>, List<int>> _typedata = SqlReader.Instance.GetTypesDataFromSQLite(-1, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), false);
    private List<int> _typeNumbers = new List<int>(); //  internal db type numbers
    private List<string> _typeNames = new List<string>(); // list of displayed types

    // Sections
    // list of displayed sections
    private List<string> _sectionList = SqlReader.Instance.GetSectionsDataFromSQLite(new List<int> { -1 }, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), false);
    private List<string> _filteredlist = new List<string>();
    int _catalogueIndex = -1; //-1 is all
    int _typeIndex = -1;
    // displayed selections
    // list of sections as outcome from selections
    string profileString = "HE HE200.B";
    string _search = "";

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Profile type", "Measure", "Type", "Profile"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Profile type
      this.DropDownItems.Add(_profileTypes.Keys.ToList());
      this.SelectedItems.Add("Rectangle");

      // Length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // input -1 to force update of catalogue sections to include/exclude superseeded
      bool updateCat = false;
      if (i == -1)
      {
        SelectedItems[0] = "Catalogue";
        updateCat = true;
        i = 0;
      }
      else
      {
        // change selected item
        SelectedItems[i] = DropDownItems[i][j];
      }

      if (SelectedItems[0] == "Catalogue")
      {
        // update spacer description to match catalogue dropdowns
        SpacerDescriptions[1] = "Catalogue";

        // if FoldMode is not currently catalogue state, then we update all lists
        if (_mode != FoldMode.Catalogue | updateCat)
        {
          // remove any existing selections
          while (SelectedItems.Count > 1)
            SelectedItems.RemoveAt(1);

          // set catalogue selection to all
          _catalogueIndex = -1;

          _catalogueNames = _cataloguedata.Item1;
          _catalogueNumbers = _cataloguedata.Item2;

          // set types to all
          _typeIndex = -1;
          // update typelist with all catalogues
          _typedata = this.GetTypesDataFromSQLite(_catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSS);
          _typeNames = _typedata.Item1;
          _typeNumbers = _typedata.Item2;

          // update section list to all types
          _sectionList = SqlReader.Instance.GetSectionsDataFromSQLite(_typeNumbers, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSS);

          // filter by search pattern
          _filteredlist = new List<string>();
          if (_search == "")
          {
            _filteredlist = _sectionList;
          }
          else
          {
            for (int k = 0; k < _sectionList.Count; k++)
            {
              if (_sectionList[k].ToLower().Contains(_search))
              {
                _filteredlist.Add(_sectionList[k]);
              }
              if (!_search.Any(char.IsDigit))
              {
                string test = _sectionList[k].ToString();
                test = Regex.Replace(test, "[0-9]", string.Empty);
                test = test.Replace(".", string.Empty);
                test = test.Replace("-", string.Empty);
                test = test.ToLower();
                if (test.Contains(_search))
                {
                  _filteredlist.Add(_sectionList[k]);
                }
              }
            }
          }

          // update displayed selections to all
          SelectedItems.Add(_catalogueNames[0]);
          SelectedItems.Add(_typeNames[0]);
          SelectedItems.Add(_filteredlist[0]);

          // call graphics update
          Mode1Clicked();
        }

        // update dropdown lists
        while (DropDownItems.Count > 1)
          DropDownItems.RemoveAt(1);

        // add catalogues (they will always be the same so no need to rerun sql call)
        DropDownItems.Add(_catalogueNames);

        // type list
        // if second list (i.e. catalogue list) is changed, update types list to account for that catalogue
        if (i == 1)
        {
          // update catalogue index with the selected catalogue
          _catalogueIndex = _catalogueNumbers[j];
          SelectedItems[1] = _catalogueNames[j];

          // update typelist with selected input catalogue
          _typedata = SqlReader.Instance.GetTypesDataFromSQLite(_catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSS);
          _typeNames = _typedata.Item1;
          _typeNumbers = _typedata.Item2;

          // update section list from new types (all new types in catalogue)
          List<int> types = _typeNumbers.ToList();
          types.RemoveAt(0); // remove -1 from beginning of list
          _sectionList = SqlReader.Instance.GetSectionsDataFromSQLite(types, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSS);

          // filter by search pattern
          _filteredlist = new List<string>();
          if (_search == "")
          {
            _filteredlist = _sectionList;
          }
          else
          {
            for (int k = 0; k < _sectionList.Count; k++)
            {
              if (_sectionList[k].ToLower().Contains(_search))
              {
                _filteredlist.Add(_sectionList[k]);
              }
              if (!_search.Any(char.IsDigit))
              {
                string test = _sectionList[k].ToString();
                test = Regex.Replace(test, "[0-9]", string.Empty);
                test = test.Replace(".", string.Empty);
                test = test.Replace("-", string.Empty);
                test = test.ToLower();
                if (test.Contains(_search))
                {
                  _filteredlist.Add(_sectionList[k]);
                }
              }
            }
          }

          // update selections to display first item in new list
          SelectedItems[2] = _typeNames[0];
          SelectedItems[3] = _filteredlist[0];
        }
        DropDownItems.Add(_typeNames);

        // section list
        // if third list (i.e. types list) is changed, update sections list to account for these section types

        if (i == 2)
        {
          // update catalogue index with the selected catalogue
          _typeIndex = _typeNumbers[j];
          SelectedItems[2] = _typeNames[j];

          // create type list
          List<int> types = new List<int>();
          if (_typeIndex == -1) // if all
          {
            types = _typeNumbers.ToList(); // use current selected list of type numbers
            types.RemoveAt(0); // remove -1 from beginning of list
          }
          else
            types = new List<int> { _typeIndex }; // create empty list and add the single selected type 


          // section list with selected types (only types in selected type)
          _sectionList = SqlReader.Instance.GetSectionsDataFromSQLite(types, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSS);

          // filter by search pattern
          _filteredlist = new List<string>();
          if (_search == "")
          {
            _filteredlist = _sectionList;
          }
          else
          {
            for (int k = 0; k < _sectionList.Count; k++)
            {
              if (_sectionList[k].ToLower().Contains(_search))
              {
                _filteredlist.Add(_sectionList[k]);
              }
              if (!_search.Any(char.IsDigit))
              {
                string test = _sectionList[k].ToString();
                test = Regex.Replace(test, "[0-9]", string.Empty);
                test = test.Replace(".", string.Empty);
                test = test.Replace("-", string.Empty);
                test = test.ToLower();
                if (test.Contains(_search))
                {
                  _filteredlist.Add(_sectionList[k]);
                }
              }
            }
          }

          // update selected section to be all
          SelectedItems[3] = _filteredlist[0];
        }
        DropDownItems.Add(_filteredlist);

        // selected profile
        // if fourth list (i.e. section list) is changed, updated the sections list to only be that single profile
        if (i == 3)
        {
          // update displayed selected
          SelectedItems[3] = _filteredlist[j];
        }
        profileString = SelectedItems[3];

        base.UpdateUI();
      }
      else
      {
        // update spacer description to match none-catalogue dropdowns
        SpacerDescriptions[1] = "Measure";// = new List<string>(new string[]

        if (_mode != FoldMode.Other)
        {
          // remove all catalogue dropdowns
          while (DropDownItems.Count > 1)
            DropDownItems.RemoveAt(1);

          // add length measure dropdown list
          this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));

          // set selected length
          SelectedItems[1] = LengthUnit.ToString();
        }

        if (i == 0)
        {
          // update profile type if change is made to first dropdown menu
          typ = _profileTypes[SelectedItems[0]];
          Mode2Clicked();
        }
        else
        {
          // change unit
          this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);

          base.UpdateUI();
        }
      }
    }

    private Tuple<List<string>, List<int>> GetTypesDataFromSQLite(int catalogueIndex, string filePath, bool inclSuperseeded)
    {
      return SqlReader.Instance.GetTypesDataFromSQLite2(catalogueIndex, filePath, inclSuperseeded);
    }

    public override void UpdateUIFromSelectedItems()
    {
      if (SelectedItems[0] == "Catalogue")
      {
        // update spacer description to match catalogue dropdowns
        SpacerDescriptions = new List<string>(new string[]
        {
          "Profile type", "Catalogue", "Type", "Profile"
        });

        _catalogueNames = _cataloguedata.Item1;
        _catalogueNumbers = _cataloguedata.Item2;
        _typedata = SqlReader.Instance.GetTypesDataFromSQLite(_catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSS);
        _typeNames = _typedata.Item1;
        _typeNumbers = _typedata.Item2;

        Mode1Clicked();

        profileString = SelectedItems[3];
      }
      else
      {
        // update spacer description to match none-catalogue dropdowns
        SpacerDescriptions = new List<string>(new string[]
        {
          "Profile type", "Measure", "Type", "Profile"
        });

        typ = _profileTypes[SelectedItems[0]];
        Mode2Clicked();
      }

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {

      if (_mode == FoldMode.Catalogue)
      {
        int i = 0;
        Params.Input[i].NickName = "S";
        Params.Input[i].Name = "Search";
        Params.Input[i].Description = "Text to search from";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;

        i++;
        Params.Input[i].NickName = "iSS";
        Params.Input[i].Name = "InclSuperseeded";
        Params.Input[i].Description = "Input true to include superseeded catalogue sections";
        Params.Input[i].Access = GH_ParamAccess.item;
        Params.Input[i].Optional = true;
      }
      else
      {
        string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

        int i = 0;
        // angle
        if (typ == "IAngleProfile") //(typ.Name.Equals(typeof(IAngleProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the angle profile (leg in the local z axis).";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "W";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the angle profile (leg in the local y axis).";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //dup = IAngleProfile.Create(angle.Depth, angle.Flange, angle.Web);
        }

        // channel
        else if (typ == "IChannelProfile") //(typ.Name.Equals(typeof(IChannelProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the channel profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the flange of the channel profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the channel profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the channel profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IChannelProfile.Create(channel.Depth, channel.Flanges, channel.Web);
        }

        // circle hollow
        else if (typ == "ICircleHollowProfile") //(typ.Name.Equals(typeof(ICircleHollowProfile).Name))
        {
          Params.Input[i].NickName = "Ø";
          Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The diameter of the hollow circle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "t";
          Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The wall thickness of the hollow circle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = ICircleHollowProfile.Create(circleHollow.Diameter, circleHollow.WallThickness);
        }

        // circle
        else if (typ == "ICircleProfile") //(typ.Name.Equals(typeof(ICircleProfile).Name))
        {
          Params.Input[i].NickName = "Ø";
          Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The diameter of the circle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //dup = ICircleProfile.Create(circle.Diameter);
        }

        // ICruciformSymmetricalProfile
        else if (typ == "ICruciformSymmetricalProfile") //(typ.Name.Equals(typeof(ICruciformSymmetricalProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth (local z axis leg) of the profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the flange (local y axis leg) of the cruciform.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the cruciform.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the cruciform.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = ICruciformSymmetricalProfile.Create(cruciformSymmetrical.Depth, cruciformSymmetrical.Flange, cruciformSymmetrical.Web);
        }

        // IEllipseHollowProfile
        else if (typ == "IEllipseHollowProfile") //(typ.Name.Equals(typeof(IEllipseHollowProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the hollow ellipse.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the hollow ellipse.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "t";
          Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The wall thickness of the hollow ellipse.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IEllipseHollowProfile.Create(ellipseHollow.Depth, ellipseHollow.Width, ellipseHollow.WallThickness);
        }

        // IEllipseProfile
        else if (typ == "IEllipseProfile") //(typ.Name.Equals(typeof(IEllipseProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the ellipse.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the ellipse.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IEllipseProfile.Create(ellipse.Depth, ellipse.Width);
        }

        // IGeneralCProfile
        else if (typ == "IGeneralCProfile") //(typ.Name.Equals(typeof(IGeneralCProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the generic c section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange width of the generic c section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "L";
          Params.Input[i].Name = "Lip [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The lip of the generic c section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "t";
          Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The thickness of the generic c section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IGeneralCProfile.Create(generalC.Depth, generalC.FlangeWidth, generalC.Lip, generalC.Thickness);
        }

        // IGeneralZProfile
        else if (typ == "IGeneralZProfile") //(typ.Name.Equals(typeof(IGeneralZProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bt";
          Params.Input[i].Name = "TopWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top flange width of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bb";
          Params.Input[i].Name = "BottomWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The bottom flange width of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Lt";
          Params.Input[i].Name = "Top Lip [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top lip of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Lb";
          Params.Input[i].Name = "Lip [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top lip of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "t";
          Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The thickness of the generic z section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IGeneralZProfile.Create(generalZ.Depth, generalZ.TopFlangeWidth, generalZ.BottomFlangeWidth, generalZ.TopLip, generalZ.BottomLip, generalZ.Thickness);
        }

        // IIBeamAsymmetricalProfile
        else if (typ == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bt";
          Params.Input[i].Name = "TopFlangeWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the top flange of the beam. Top is relative to the beam local access.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bb";
          Params.Input[i].Name = "BottomFlangeWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the bottom flange of the beam. Bottom is relative to the beam local access.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Web";
          Params.Input[i].Name = "Web Thickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the beam.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tt";
          Params.Input[i].Name = "TopFlangeThk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top flange thickness.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tb";
          Params.Input[i].Name = "BottomFlangeThk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The bpttom flange thickness.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //dup = IIBeamAsymmetricalProfile.Create(iBeamAsymmetrical.Depth, iBeamAsymmetrical.TopFlange, iBeamAsymmetrical.BottomFlange, iBeamAsymmetrical.Web);
        }

        // IIBeamCellularProfile
        else if (typ == "IIBeamCellularProfile") //(typ.Name.Equals(typeof(IIBeamCellularProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the flanges of the beam.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "O";
          Params.Input[i].Name = "WebOpening [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The size of the web opening.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "P";
          Params.Input[i].Name = "Pitch [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The pitch (spacing) between the web openings.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IIBeamCellularProfile.Create(iBeamCellular.Depth, iBeamCellular.Flanges, iBeamCellular.Web, iBeamCellular.WebOpening);
        }

        // IIBeamSymmetricalProfile
        else if (typ == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the flanges of the beam.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the angle profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //dup = IIBeamSymmetricalProfile.Create(iBeamSymmetrical.Depth, iBeamSymmetrical.Flanges, iBeamSymmetrical.Web);
        }

        // IRectangleHollowProfile
        else if (typ == "IRectangleHollowProfile") //(typ.Name.Equals(typeof(IRectangleHollowProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the hollow rectangle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the hollow rectangle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The side thickness of the hollow rectangle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top/bottom thickness of the hollow rectangle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IRectangleHollowProfile.Create(rectangleHollow.Depth, rectangleHollow.Flanges, rectangleHollow.Webs);
        }

        // IRectangleProfile
        else if (typ == "IRectangleProfile") //(typ.Name.Equals(typeof(IRectangleProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "Depth of the rectangle, in local z-axis direction.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "Width of the rectangle, in local y-axis direction.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IRectangleProfile.Create(rectangle.Depth, rectangle.Width);
        }

        // IRectoEllipseProfile
        else if (typ == "IRectoEllipseProfile") //(typ.Name.Equals(typeof(IRectoEllipseProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The overall depth of the recto-ellipse profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Df";
          Params.Input[i].Name = "DepthFlat [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flat length of the profile's overall depth.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The overall width of the recto-ellipse profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bf";
          Params.Input[i].Name = "WidthFlat [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flat length of the profile's overall width.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IRectoEllipseProfile.Create(rectoEllipse.Depth, rectoEllipse.DepthFlat, rectoEllipse.Width, rectoEllipse.WidthFlat);
        }

        // ISecantPileProfile
        else if (typ == "ISecantPileProfile") //(typ.Name.Equals(typeof(ISecantPileProfile).Name))
        {
          Params.Input[i].NickName = "Ø";
          Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The diameter of the piles.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "c/c";
          Params.Input[i].Name = "PileCentres [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The centre to centre distance between adjacent piles.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "No";
          Params.Input[i].Name = "PileCount";
          Params.Input[i].Description = "The number of piles in the profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "W/S";
          Params.Input[i].Name = "isWall";
          Params.Input[i].Description = "Converts the profile into a wall secant pile profile if true -- Converts the profile into a section secant pile profile if false.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = ISecantPileProfile.Create(secantPile.Diameter, secantPile.PileCentres, secantPile.PileCount, secantPile.IsWallNotSection);
        }

        // ISheetPileProfile
        else if (typ == "ISheetPileProfile") //(typ.Name.Equals(typeof(ISheetPileProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The overall width of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bt";
          Params.Input[i].Name = "TopFlangeWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top flange width of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bb";
          Params.Input[i].Name = "BottomFlangeWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The bottom flange width of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Ft";
          Params.Input[i].Name = "FlangeThickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Wt";
          Params.Input[i].Name = "WebThickness [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the sheet pile section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = ISheetPileProfile.Create(sheetPile.Depth, sheetPile.Width, sheetPile.TopFlangeWidth, sheetPile.BottomFlangeWidth, sheetPile.FlangeThickness, sheetPile.WebThickness);
        }

        // IStadiumProfile
        else if (typ == "IStadiumProfile") //(typ.Name.Equals(typeof(IStadiumProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The profile's overall depth considering the side length of the rectangle and the radii of the semicircles on the two ends.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The profile's width (diameter of the semicircles).";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = IStadiumProfile.Create(stadium.Depth, stadium.Width);
        }

        // ITrapezoidProfile
        else if (typ == "ITrapezoidProfile") //(typ.Name.Equals(typeof(ITrapezoidProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth in z-axis direction of trapezoidal profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bt";
          Params.Input[i].Name = "TopWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The top width of trapezoidal profile. Top is relative to the local z-axis.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Bb";
          Params.Input[i].Name = "BottomWidth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The bottom width of trapezoidal profile. Bottom is relative to the local z-axis.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
          //dup = ITrapezoidProfile.Create(trapezoid.Depth, trapezoid.TopWidth, trapezoid.BottomWidth);
        }

        // ITSectionProfile
        else if (typ == "ITSectionProfile") //(typ.Name.Equals(typeof(ITSectionProfile).Name))
        {
          Params.Input[i].NickName = "D";
          Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The depth of the T section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The width of the T section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tw";
          Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The web thickness of the T section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          i++;
          Params.Input[i].NickName = "Tf";
          Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The flange thickness of the T section profile.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          //dup = ITSectionProfile.Create(tSection.Depth, tSection.Flange, tSection.Web);
        }
        // IPerimeterProfile
        else if (typ == "IPerimeterProfile") //(typ.Name.Equals(typeof(IPerimeterProfile).Name))
        {
          Params.Input[i].NickName = "B";
          Params.Input[i].Name = "Boundary";
          Params.Input[i].Description = "Planar Brep or closed planar curve.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;

          if (Params.Input.Count == 1) // handle backwards compatability
            Params.RegisterInputParam(new Param_Plane());

          i++;
          Params.Input[i].NickName = "P";
          Params.Input[i].Name = "Plane";
          Params.Input[i].Description = "Optional plane in which to project boundary onto. Profile will get coordinates in this plane.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = true;

          //i++;
          //Params.Input[i].NickName = "V";
          //Params.Input[i].Name = "[Optional] VoidPolylines";
          //Params.Input[i].Description = "The void polygons within the solid polygon of the perimeter profile. If first input is a BRep this input will be ignored.";
          //Params.Input[i].Access = GH_ParamAccess.list;
          //Params.Input[i].Optional = true;
        }
      }
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetString("mode", _mode.ToString());
      writer.SetString("lengthUnit", LengthUnit.ToString());
      writer.SetBoolean("inclSS", _inclSS);
      writer.SetInt32("NumberOfInputs", _numberOfInputs);
      writer.SetInt32("catalogueIndex", _catalogueIndex);
      writer.SetInt32("typeIndex", _typeIndex);
      writer.SetString("search", _search);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this._mode = (FoldMode)Enum.Parse(typeof(FoldMode), reader.GetString("mode"));
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("lengthUnit"));

      this._inclSS = reader.GetBoolean("inclSS");
      this._numberOfInputs = reader.GetInt32("NumberOfInputs");
      this._catalogueIndex = reader.GetInt32("catalogueIndex");
      this._typeIndex = reader.GetInt32("typeIndex");
      this._search = reader.GetString("search");

      return base.Read(reader);
    }
    #endregion
  }
}