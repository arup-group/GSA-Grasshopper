using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Grasshopper;
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

namespace GsaGH.Components {
  /// <summary>
  /// Component to create AdSec profile
  /// </summary>
  public class CreateProfile : GH_OasysDropDownComponent {
    private enum FoldMode {
      Catalogue,
      Other,
    }

    // temporary manual implementation of profile types (to be replaced by reflection of Oasys.Profiles)
    private static readonly Dictionary<string, string> s_profileTypes = new Dictionary<string, string> {
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

    private static readonly List<string> s_easterCat = new List<string>() {
      "▌─────────────────────────▐█─────▐" + Environment.NewLine +
      "▌────▄──────────────────▄█▓█▌────▐" + Environment.NewLine +
      "▌───▐██▄───────────────▄▓░░▓▓────▐" + Environment.NewLine +
      "▌───▐█░██▓────────────▓▓░░░▓▌────▐" + Environment.NewLine +
      "▌───▐█▌░▓██──────────█▓░░░░▓─────▐" + Environment.NewLine +
      "▌────▓█▌░░▓█▄███████▄███▓░▓█─────▐" + Environment.NewLine +
      "▌────▓██▌░▓██░░░░░░░░░░▓█░▓▌─────▐" + Environment.NewLine +
      "▌─────▓█████░░░░░░░░░░░░▓██──────▐" + Environment.NewLine +
      "▌─────▓██▓░░░░░░░░░░░░░░░▓█──────▐" + Environment.NewLine +
      "▌─────▐█▓░░░░░░█▓░░▓█░░░░▓█▌─────▐" + Environment.NewLine +
      "▌─────▓█▌░▓█▓▓██▓░█▓▓▓▓▓░▓█▌─────▐" + Environment.NewLine +
      "▌─────▓▓░▓██████▓░▓███▓▓▌░█▓─────▐" + Environment.NewLine +
      "▌────▐▓▓░█▄▐▓▌█▓░░▓█▐▓▌▄▓░██─────▐" + Environment.NewLine +
      "▌────▓█▓░▓█▄▄▄█▓░░▓█▄▄▄█▓░██▌────▐" + Environment.NewLine +
      "▌────▓█▌░▓█████▓░░░▓███▓▀░▓█▓────▐" + Environment.NewLine +
      "▌───▐▓█░░░▀▓██▀░░░░░─▀▓▀░░▓█▓────▐" + Environment.NewLine +
      "▌───▓██░░░░░░░░▀▄▄▄▄▀░░░░░░▓▓────▐" + Environment.NewLine +
      "▌───▓█▌░░░░░░░░░░▐▌░░░░░░░░▓▓▌───▐" + Environment.NewLine +
      "▌───▓█░░░░░░░░░▄▀▀▀▀▄░░░░░░░█▓───▐" + Environment.NewLine +
      "▌──▐█▌░░░░░░░░▀░░░░░░▀░░░░░░█▓▌──▐" + Environment.NewLine +
      "▌──▓█░░░░░░░░░░░░░░░░░░░░░░░██▓──▐" + Environment.NewLine +
      "▌──▓█░░░░░░░░░░░░░░░░░░░░░░░▓█▓──▐" + Environment.NewLine +
      "▌──██░░░░░░░░░░░░░░░░░░░░░░░░█▓──▐" + Environment.NewLine +
      "▌──█▌░░░░░░░░░░░░░░░░░░░░░░░░▐▓▌─▐" + Environment.NewLine +
      "▌─▐▓░░░░░░░░░░░░░░░░░░░░░░░░░░█▓─▐" + Environment.NewLine +
      "▌─█▓░░░░░░░░░░░░░░░░░░░░░░░░░░▓▓─▐" + Environment.NewLine +
      "▌─█▓░░░░░░░░░░░░░░░░░░░░░░░░░░▓▓▌▐" + Environment.NewLine +
      "▌▐█▓░░░░░░░░░░░░░░░░░░░░░░░░░░░██▐" + Environment.NewLine +
      "▌█▓▌░░░░░░░░░░░░░░░░░░░░░░░░░░░▓█▐" };

    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("ea1741e5-905e-4ecb-8270-a584e3f99aa3");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateProfile;

    // list of sections as outcome from selections
    private List<string> _profileString = new List<string>() { "CAT HE HE200.B" };
    private string _search = "";

    //List<string> excludedInterfaces = new List<string>(new string[]
    //{
    //    "IProfile", "IPoint", "IPolygon", "IFlange", "IWeb", "IWebConstant", "IWebTapered", "ITrapezoidProfileAbstractInterface", "IIBeamProfile"
    //});

    // Catalogues
    private readonly Tuple<List<string>, List<int>> _cataloguedata;
    private List<int> _catalogueNumbers = new List<int>(); // internal db catalogue numbers
    private List<string> _catalogueNames = new List<string>(); // list of displayed catalogues
    private bool _inclSs;

    // Types
    private Tuple<List<string>, List<int>> _typedata;
    private List<int> _typeNumbers = new List<int>(); //  internal db type numbers
    private List<string> _typeNames = new List<string>(); // list of displayed types

    // Sections
    private List<string> _sectionList;

    private int _catalogueIndex = -1; //-1 is all
    private int _typeIndex = -1;

    private bool _lastInputWasSecant;
    private int _numberOfInputs;
    // temporary (???)
    private string _type = "IRectangleProfile";

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private FoldMode _mode = FoldMode.Other;

    public CreateProfile() : base(
      "Create Profile",
      "Profile",
      "Create Profile text-string for a GSA Section",
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
        Hidden = true; // sets the initial state of the component to hidden
        _cataloguedata = MicrosoftSQLiteReader.Instance.GetCataloguesDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
        _typedata = MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(-1, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
        _sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(new List<int> { -1 }, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
    }

    private static bool MatchAndAdd(string item, string pattern, ref List<string> list, bool tryHard = false) {
      string input = item.ToLower().Replace(".", string.Empty);
      if (Regex.Match(input, pattern, RegexOptions.Singleline).Success) {
        list.Add(item);
        return true;
      }
      else if (tryHard && Regex.Match(pattern, "he[abcm]", RegexOptions.Singleline).Success) {
        string[] substring = pattern.Split(new [] { "he" }, StringSplitOptions.None);
        int count = 1;
        if (substring[substring.Length - 1].Length > 1 && !char.IsNumber(substring[substring.Length - 1][1]))
          count = 2;

        pattern = "he" + substring[substring.Length - 1].Remove(0, count) + substring[substring.Length - 1].Substring(0, count);
        if (!Regex.Match(input, pattern, RegexOptions.Singleline).Success) {
          return false;
        }

        list.Add(item);
        return true;
      }

      return false;
    }

    private static Tuple<List<string>, List<int>> GetTypesDataFromSqLite(int catalogueIndex, string filePath, bool inclSuperseeded) {
      return MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(catalogueIndex, filePath, inclSuperseeded);
    }

    protected override string HtmlHelp_Source() {
      string help = "GOTO:https://arup-group.github.io/oasys-combined/adsec-api/api/Oasys.Profiles.html";
      return help;
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      pManager.AddGenericParameter("Width [" + unitAbbreviation + "]", "B", "Profile width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth [" + unitAbbreviation + "]", "H", "Profile depth", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Profile", "Pf", "Profile for a GSA Section", GH_ParamAccess.tree);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      ClearRuntimeMessages();
      foreach (IGH_Param input in Params.Input)
        input.ClearRuntimeMessages();

      #region catalogue
      ClearRuntimeMessages();
      if (_mode == FoldMode.Catalogue) {
        // get user input filter search string
        bool incl = false;
        if (da.GetData(1, ref incl)) {
          if (_inclSs != incl) {
            _inclSs = incl;
            UpdateTypeData();
            _sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(_typeNumbers, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);

            SelectedItems[2] = _typeNames[0];
            DropDownItems[2] = _typeNames;

            SelectedItems[3] = _sectionList[0];
            DropDownItems[3] = _sectionList;

            base.UpdateUI();
          }
        }

        // get user input filter search string
        _search = null;
        string inSearch = "";
        if (da.GetData(0, ref inSearch)) {
          _search = inSearch
            .Trim()
            .ToLower()
            .Replace(".", string.Empty)
            .Replace("*", ".*")
            .Replace(" ", ".*");
          if (_search == "cat") {
            string eventName = "EasterCat";
            var properties = new Dictionary<string, object>();
            _ = PostHog.SendToPostHog(GsaGH.PluginInfo.Instance, eventName, properties);
            da.SetDataList(0, s_easterCat);
            return;
          }

          if (_search.Contains("cat")) {
            string[] s = _search.Split(new[] { "cat" }, StringSplitOptions.None);
            _search = s[s.Length - 1];
          }

          // boolean to save state of search string being 'problematic' to avoid checking this every single time
          bool tryHard = Regex.Match(_search, "he[abcm]", RegexOptions.Singleline).Success;

          // filter by search pattern
          var filteredlist = new List<string>();
          if (SelectedItems[3] != "All") {
            if (!MatchAndAdd(SelectedItems[3], _search, ref filteredlist, tryHard)) {
              _profileString = new List<string>();
              this.AddRuntimeWarning("No profile found that matches selected profile and search!");
            }
          }
          else if (_search != "") {
            foreach (string section in _sectionList)
            {
              if (MatchAndAdd(section, _search, ref filteredlist, tryHard)) {

              }
              else if (!_search.Any(char.IsDigit)) {
                string test = section;
                test = Regex.Replace(test, "[0-9]", string.Empty);
                test = test.Replace(".", string.Empty);
                test = test.Replace("-", string.Empty);
                test = test.ToLower();
                if (test.Contains(_search)) {
                  filteredlist.Add(section);
                }
              }
            }
          }

          _profileString = new List<string>();
          if (filteredlist.Count > 0) {
            foreach (string profile in filteredlist)
              _profileString.Add("CAT " + profile);
          }
          else {
            this.AddRuntimeWarning("No profile found that matches selection and search!");
            return;
          }
        }

        if (_search == null)
          UpdateProfileString();

        var tree = new DataTree<string>();

        int pathCount = 0;
        if (Params.Output[0].VolatileDataCount > 0)
          pathCount = Params.Output[0].VolatileData.PathCount;

        var path = new Grasshopper.Kernel.Data.GH_Path(new [] { pathCount });
        tree.AddRange(_profileString, path);

        da.SetDataTree(0, tree);
      }
      #endregion

      #region std

      if (_mode != FoldMode.Other) {
        return;
      }

      {
        string unit = "(" + Length.GetAbbreviation(_lengthUnit, new CultureInfo("en")) + ") ";
        string profile = "STD ";
        switch (_type)
        {
          case "IAngleProfile":
            profile += "A" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IChannelProfile":
            profile += "CH" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "ICircleHollowProfile":
            profile += "CHS" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "ICircleProfile":
            profile += "C" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "ICruciformSymmetricalProfile":
            profile += "X" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IEllipseHollowProfile":
            profile += "OVAL" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IEllipseProfile":
            profile += "E" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " 2";
            break;
          case "IGeneralCProfile":
            profile += "GC" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IGeneralZProfile":
            profile += "GZ" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 4, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 5, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IIBeamAsymmetricalProfile":
            profile += "GI" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 4, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 5, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IIBeamCellularProfile":
            profile += "CB" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 4, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 5, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IIBeamSymmetricalProfile":
            profile += "I" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IRectangleHollowProfile":
            profile += "RHS" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IRectangleProfile":
            profile += "R" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IRectoEllipseProfile":
            profile += "RE" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString() + " 2";
            break;
          case "ISecantPileProfile":
          {
            int pileCount = 0;
            if (!da.GetData(2, ref pileCount)) {
              this.AddRuntimeError("Unable to convert input PileCount to integer.");
              return;
            }

            bool isWallNotSection = false;
            if (!da.GetData(3, ref isWallNotSection)) {
              this.AddRuntimeError("Unable to convert input isWall to boolean.");
              return;
            }

            profile += (isWallNotSection ? "SP" : "SPW") + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       pileCount.ToString();
            break;
          }
          case "ISheetPileProfile":
            profile += "SHT" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 4, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 5, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IStadiumProfile":
            profile += "RC" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "ITrapezoidProfile":
            profile += "TR" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "ITSectionProfile":
            profile += "T" + unit +
                       Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit).ToString() + " " +
                       Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit).ToString();
            break;
          case "IPerimeterProfile": {
            var perimeter = new ProfileHelper() { profileType = ProfileHelper.ProfileTypes.Geometric };
            var ghBrep = new GH_Brep();

            if (da.GetData(0, ref ghBrep)) {
              var brep = new Brep();
              if (GH_Convert.ToBrep(ghBrep, ref brep, GH_Conversion.Both)) {
                // get edge curves from Brep
                Curve[] edgeSegments = brep.DuplicateEdgeCurves();
                Curve[] edges = Curve.JoinCurves(edgeSegments);

                // find the best fit plane
                var ctrlPts = new List<Point3d>();
                if (edges[0].TryGetPolyline(out Polyline tempCrv))
                  ctrlPts = tempCrv.ToList();
                else {
                  this.AddRuntimeError("Cannot convert edge to Polyline");
                  return;
                }

                bool localPlaneNotSet = true;
                Plane plane = Plane.Unset;
                if (da.GetData(1, ref plane))
                  localPlaneNotSet = false;

                var origin = new Point3d();
                if (localPlaneNotSet) {
                  foreach (Point3d p in ctrlPts) {
                    origin.X += p.X;
                    origin.Y += p.Y;
                    origin.Z += p.Z;
                  }

                  origin.X /= ctrlPts.Count;
                  origin.Y /= ctrlPts.Count;
                  origin.Z /= ctrlPts.Count;

                  Plane.FitPlaneToPoints(ctrlPts, out plane);

                  var xDirection = new Vector3d(
                    Math.Abs(plane.XAxis.X),
                    Math.Abs(plane.XAxis.Y),
                    Math.Abs(plane.XAxis.Z));
                  xDirection.Unitize();
                  var yDirection = new Vector3d(
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
                }
                else {
                  origin = plane.Origin;
                }

                var translation = Transform.Translation(-origin.X, -origin.Y, -origin.Z);
                var rotation = Transform.ChangeBasis(Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, plane.XAxis, plane.YAxis, plane.ZAxis);
                if (localPlaneNotSet)
                  rotation = Transform.ChangeBasis(Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis, plane.YAxis, plane.XAxis, plane.ZAxis);

                perimeter.geoType = ProfileHelper.GeoTypes.Perim;

                var pts = new List<Point2d>();
                foreach (Point3d pt3d in ctrlPts) {
                  pt3d.Transform(translation);
                  pt3d.Transform(rotation);
                  var pt2d = new Point2d(pt3d);
                  pts.Add(pt2d);
                }

                perimeter.perimeterPoints = pts;

                if (edges.Length > 1) {
                  var voidPoints = new List<List<Point2d>>();
                  for (int i = 1; i < edges.Length; i++) {
                    ctrlPts.Clear();
                    if (!edges[i].IsPlanar()) {
                      for (int j = 0; j < edges.Length; j++)
                        edges[j] = Curve.ProjectToPlane(edges[j], plane);
                    }

                    if (edges[i].TryGetPolyline(out tempCrv)) {
                      ctrlPts = tempCrv.ToList();
                      pts = new List<Point2d>();
                      foreach (Point3d pt3d in ctrlPts) {
                        pt3d.Transform(translation);
                        pt3d.Transform(rotation);
                        var pt2d = new Point2d(pt3d);
                        pts.Add(pt2d);
                      }

                      voidPoints.Add(pts);
                    }
                    else {
                      this.AddRuntimeError("Cannot convert internal edge to Polyline");
                      return;
                    }
                  }

                  perimeter.voidPoints = voidPoints;
                }
              }
            }

            switch (_lengthUnit) {
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

            da.SetData(0, ConvertSection.ProfileConversion(perimeter));
            return;
          }
          default:
            this.AddRuntimeError("Unable to create profile");
            return;
        }

        da.SetData(0, profile);
      }
      #endregion
    }

    #region menu override
    private void Mode1Clicked() {
      // remove input parameters
      while (Params.Input.Count > 0)
        Params.UnregisterInputParameter(Params.Input[0], true);

      Params.RegisterInputParam(new Param_String());
      Params.RegisterInputParam(new Param_Boolean());

      _mode = FoldMode.Catalogue;

      base.UpdateUI();
    }

    private void SetNumberOfGenericInputs(int inputs, bool isSecantPile = false, bool isPerimeter = false) {
      _numberOfInputs = inputs;

      // if last input previously was a bool and we no longer need that
      if (_lastInputWasSecant || isSecantPile || isPerimeter) {
        if (Params.Input.Count > 0) {
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
        _lastInputWasSecant = true;
      }
      else
        _lastInputWasSecant = false;

      if (isPerimeter)
        Params.RegisterInputParam(new Param_Plane());
    }

    private void Mode2Clicked() {
      // check if mode is correct
      if (_mode != FoldMode.Other) {
        // if we come from catalogue mode remove all input parameters
        while (Params.Input.Count > 0)
          Params.UnregisterInputParameter(Params.Input[0], true);

        // set mode to other
        _mode = FoldMode.Other;
      }

      switch (_type)
      {
        case "IAngleProfile":
        case "IChannelProfile":
          SetNumberOfGenericInputs(4);
          break;
        case "ICircleHollowProfile":
          SetNumberOfGenericInputs(2);
          break;
        case "ICircleProfile":
          SetNumberOfGenericInputs(1);
          break;
        case "ICruciformSymmetricalProfile":
          SetNumberOfGenericInputs(4);
          break;
        case "IEllipseHollowProfile":
          SetNumberOfGenericInputs(3);
          break;
        case "IEllipseProfile":
          SetNumberOfGenericInputs(2);
          break;
        case "IGeneralCProfile":
          SetNumberOfGenericInputs(4);
          break;
        case "IGeneralZProfile":
        case "IIBeamAsymmetricalProfile":
        case "IIBeamCellularProfile":
          SetNumberOfGenericInputs(6);
          break;
        case "IIBeamSymmetricalProfile":
        case "IRectangleHollowProfile":
          SetNumberOfGenericInputs(4);
          break;
        case "IRectangleProfile":
          SetNumberOfGenericInputs(2);
          break;
        case "IRectoEllipseProfile":
          SetNumberOfGenericInputs(4);
          break;
        case "ISecantPileProfile":
          SetNumberOfGenericInputs(4, true);
          break;
        case "ISheetPileProfile":
          SetNumberOfGenericInputs(6);
          break;
        case "IStadiumProfile":
          SetNumberOfGenericInputs(2);
          break;
        case "ITrapezoidProfile":
          SetNumberOfGenericInputs(3);
          break;
        case "ITSectionProfile":
          SetNumberOfGenericInputs(4);
          break;
        case "IPerimeterProfile":
          SetNumberOfGenericInputs(1, false, true);
          break;
      }

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
    #endregion

    #region Custom UI
    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new [] {
        "Profile type",
        "Measure",
        "Type",
        "Profile",
      });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      // Profile type
      DropDownItems.Add(s_profileTypes.Keys.ToList());
      SelectedItems.Add("Rectangle");

      // Length
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      // input -1 to force update of catalogue sections to include/exclude superseeded
      bool updateCat = false;
      if (i == -1) {
        SelectedItems[0] = "Catalogue";
        updateCat = true;
        i = 0;
      }
      else {
        // change selected item
        SelectedItems[i] = DropDownItems[i][j];
      }

      if (SelectedItems[0] == "Catalogue") {
        // update spacer description to match catalogue dropdowns
        SpacerDescriptions[1] = "Catalogue";

        // if FoldMode is not currently catalogue state, then we update all lists
        if (_mode != FoldMode.Catalogue | updateCat) {
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
          UpdateTypeData();

          // update section list to all types
          _sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(_typeNumbers, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);

          // update displayed selections to all
          SelectedItems.Add(_catalogueNames[0]);
          SelectedItems.Add(_typeNames[0]);
          SelectedItems.Add(_sectionList[0]);

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
        if (i == 1) {
          // update catalogue index with the selected catalogue
          _catalogueIndex = _catalogueNumbers[j];
          SelectedItems[1] = _catalogueNames[j];

          // update typelist with selected input catalogue
          _typedata = MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(_catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);
          _typeNames = _typedata.Item1;
          _typeNumbers = _typedata.Item2;

          // update section list from new types (all new types in catalogue)
          var types = _typeNumbers.ToList();
          types.RemoveAt(0); // remove -1 from beginning of list
          _sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(types, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);

          // update selections to display first item in new list
          SelectedItems[2] = _typeNames[0];
          SelectedItems[3] = _sectionList[0];
        }

        DropDownItems.Add(_typeNames);

        // section list
        // if third list (i.e. types list) is changed, update sections list to account for these section types
        if (i == 2) {
          // update catalogue index with the selected catalogue
          _typeIndex = _typeNumbers[j];
          SelectedItems[2] = _typeNames[j];

          // create type list
          List<int> types;
          if (_typeIndex == -1) // if all
          {
            types = _typeNumbers.ToList(); // use current selected list of type numbers
            types.RemoveAt(0); // remove -1 from beginning of list
          }
          else
            types = new List<int> { _typeIndex }; // create empty list and add the single selected type 

          // section list with selected types (only types in selected type)
          _sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(types, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);

          // update selected section to be all
          SelectedItems[3] = _sectionList[0];
        }

        DropDownItems.Add(_sectionList);

        // selected profile
        // if fourth list (i.e. section list) is changed, updated the sections list to only be that single profile
        if (i == 3) {
          // update displayed selected
          SelectedItems[3] = _sectionList[j];
        }

        if (_search == "")
          UpdateProfileString();

        base.UpdateUI();
      }
      else {
        // update spacer description to match none-catalogue dropdowns
        SpacerDescriptions[1] = "Measure";// = new List<string>(new string[]

        if (_mode != FoldMode.Other) {
          // remove all catalogue dropdowns
          while (DropDownItems.Count > 1)
            DropDownItems.RemoveAt(1);

          // add length measure dropdown list
          DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));

          // set selected length
          SelectedItems[1] = _lengthUnit.ToString();
        }

        if (i == 0) {
          // update profile type if change is made to first dropdown menu
          _type = s_profileTypes[SelectedItems[0]];
          Mode2Clicked();
        }
        else {
          // change unit
          _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);

          base.UpdateUI();
        }
      }
    }

    private void UpdateTypeData() {
      _typedata = GetTypesDataFromSqLite(_catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);
      _typeNames = _typedata.Item1;
      _typeNumbers = _typedata.Item2;
    }

    private void UpdateProfileString() {
      if (SelectedItems[3] == "All") {
        _profileString = new List<string>();
        foreach (string profile in _sectionList.Where(profile => profile != "All"))
        {
          _profileString.Add("CAT " + profile);
        }
      }
      else
        _profileString = new List<string>() { "CAT " + SelectedItems[3] };
    }

    public override void UpdateUIFromSelectedItems() {
      if (SelectedItems[0] == "Catalogue") {
        // update spacer description to match catalogue dropdowns
        SpacerDescriptions = new List<string>(new []
        {
          "Profile type", "Catalogue", "Type", "Profile"
        });

        _catalogueNames = _cataloguedata.Item1;
        _catalogueNumbers = _cataloguedata.Item2;
        _typedata = MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(_catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);
        _typeNames = _typedata.Item1;
        _typeNumbers = _typedata.Item2;

        Mode1Clicked();

        _profileString = new List<string>() { "CAT " + SelectedItems[3] };
      }
      else {
        // update spacer description to match none-catalogue dropdowns
        SpacerDescriptions = new List<string>(new[] { "Profile type", "Measure", "Type", "Profile" });

        _type = s_profileTypes[SelectedItems[0]];
        Mode2Clicked();
      }

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      if (_mode == FoldMode.Catalogue) {
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
      else {
        string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

        int i = 0;
        if (_type == "IAngleProfile") //(typ.Name.Equals(typeof(IAngleProfile).Name))
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
        }

        else if (_type == "IChannelProfile") //(typ.Name.Equals(typeof(IChannelProfile).Name))
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
          Params.Input[i].Optional = false;
        }
        else if (_type == "ICircleHollowProfile") //(typ.Name.Equals(typeof(ICircleHollowProfile).Name))
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
        }

        else if (_type == "ICircleProfile") //(typ.Name.Equals(typeof(ICircleProfile).Name))
        {
          Params.Input[i].NickName = "Ø";
          Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
          Params.Input[i].Description = "The diameter of the circle.";
          Params.Input[i].Access = GH_ParamAccess.item;
          Params.Input[i].Optional = false;
        }
        else if (_type == "ICruciformSymmetricalProfile") //(typ.Name.Equals(typeof(ICruciformSymmetricalProfile).Name))
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
        }
        else if (_type == "IEllipseHollowProfile") //(typ.Name.Equals(typeof(IEllipseHollowProfile).Name))
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
        }
        else if (_type == "IEllipseProfile") //(typ.Name.Equals(typeof(IEllipseProfile).Name))
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
        }
        else if (_type == "IGeneralCProfile") //(typ.Name.Equals(typeof(IGeneralCProfile).Name))
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
        }
        else if (_type == "IGeneralZProfile") //(typ.Name.Equals(typeof(IGeneralZProfile).Name))
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
        }
        else if (_type == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
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
        }
        else if (_type == "IIBeamCellularProfile") //(typ.Name.Equals(typeof(IIBeamCellularProfile).Name))
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
        }
        else if (_type == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
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
        }
        else if (_type == "IRectangleHollowProfile") //(typ.Name.Equals(typeof(IRectangleHollowProfile).Name))
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
        }
        else if (_type == "IRectangleProfile") //(typ.Name.Equals(typeof(IRectangleProfile).Name))
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
        }
        else if (_type == "IRectoEllipseProfile") //(typ.Name.Equals(typeof(IRectoEllipseProfile).Name))
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
        }
        else if (_type == "ISecantPileProfile") //(typ.Name.Equals(typeof(ISecantPileProfile).Name))
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
        }
        else if (_type == "ISheetPileProfile") //(typ.Name.Equals(typeof(ISheetPileProfile).Name))
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
        }
        else if (_type == "IStadiumProfile") //(typ.Name.Equals(typeof(IStadiumProfile).Name))
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
        }
        else if (_type == "ITrapezoidProfile") //(typ.Name.Equals(typeof(ITrapezoidProfile).Name))
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
        }
        else if (_type == "ITSectionProfile") //(typ.Name.Equals(typeof(ITSectionProfile).Name))
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
        }
        else if (_type == "IPerimeterProfile") //(typ.Name.Equals(typeof(IPerimeterProfile).Name))
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
        }
      }
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetString("mode", _mode.ToString());
      writer.SetString("lengthUnit", _lengthUnit.ToString());
      writer.SetBoolean("inclSS", _inclSs);
      writer.SetInt32("NumberOfInputs", _numberOfInputs);
      writer.SetInt32("catalogueIndex", _catalogueIndex);
      writer.SetInt32("typeIndex", _typeIndex);
      writer.SetString("search", _search);
      return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), reader.GetString("mode"));
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("lengthUnit"));
      _inclSs = reader.GetBoolean("inclSS");
      _numberOfInputs = reader.GetInt32("NumberOfInputs");
      _catalogueIndex = reader.GetInt32("catalogueIndex");
      _typeIndex = reader.GetInt32("typeIndex");
      _search = reader.GetString("search");

      bool flag = base.Read(reader);
      Params.Output[0].Access = GH_ParamAccess.tree;
      return flag;
    }
    #endregion
  }
}
