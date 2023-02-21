using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
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
    private enum FoldMode
    {
      Catalogue,
      Other
    }

    // temporary manual implementation of profile types (to be replaced by reflection of Oasys.Profiles)
    private static readonly Dictionary<string, string> _profileTypes = new Dictionary<string, string> {
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

    private static readonly List<string> EasterCat = new List<string>() {
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
    List<string> ProfileString = new List<string>() { "CAT HE HE200.B" };
    string _search = "";

    //List<string> excludedInterfaces = new List<string>(new string[]
    //{
    //    "IProfile", "IPoint", "IPolygon", "IFlange", "IWeb", "IWebConstant", "IWebTapered", "ITrapezoidProfileAbstractInterface", "IIBeamProfile"
    //});

    // Catalogues
    private readonly Tuple<List<string>, List<int>> _cataloguedata;
    private List<int> _catalogueNumbers = new List<int>(); // internal db catalogue numbers
    private List<string> _catalogueNames = new List<string>(); // list of displayed catalogues
    private bool _inclSS;

    // Types
    private Tuple<List<string>, List<int>> _typedata;
    private List<int> _typeNumbers = new List<int>(); //  internal db type numbers
    private List<string> _typeNames = new List<string>(); // list of displayed types

    // Sections
    private List<string> _sectionList;

    private int _catalogueIndex = -1; //-1 is all
    private int _typeIndex = -1;

    private bool lastInputWasSecant;
    private int _numberOfInputs;
    // temporary (???)
    private string type = "IRectangleProfile";

    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private FoldMode _mode = FoldMode.Other;

    public CreateProfile() : base(
      "Create Profile",
      "Profile",
      "Create Profile text-string for a GSA Section",
      CategoryName.Name(),
      SubCategoryName.Cat1())
    {
      this.Hidden = true; // sets the initial state of the component to hidden
      this._cataloguedata = MicrosoftSQLiteReader.Instance.GetCataloguesDataFromSQLite(Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
      this._typedata = MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(-1, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), false);
      this._sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(new List<int> { -1 }, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), false);
    }

    private static bool MatchAndAdd(string item, string pattern, ref List<string> list, bool tryHard = false)
    {
      string input = item.ToLower().Replace(".", String.Empty);
      if (Regex.Match(input, pattern, RegexOptions.Singleline).Success)
      {
        list.Add(item);
        return true;
      }
      else if (tryHard && Regex.Match(pattern, "he[abcm]", RegexOptions.Singleline).Success)
      {
        string[] substring = pattern.Split(new string[] { "he" }, StringSplitOptions.None);
        int count = 1;
        if (substring[substring.Length - 1].Length > 1 && !Char.IsNumber(substring[substring.Length - 1][1]))
          count = 2;

        pattern = "he" + substring[substring.Length - 1].Remove(0, count) + substring[substring.Length - 1].Substring(0, count);
        if (Regex.Match(input, pattern, RegexOptions.Singleline).Success)
        {
          list.Add(item);
          return true;
        }
      }
      return false;
    }

    private static Tuple<List<string>, List<int>> GetTypesDataFromSQLite(int catalogueIndex, string filePath, bool inclSuperseeded)
    {
      return MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(catalogueIndex, filePath, inclSuperseeded);
    }

    protected override string HtmlHelp_Source()
    {
      string help = "GOTO:https://arup-group.github.io/oasys-combined/adsec-api/api/Oasys.Profiles.html";
      return help;
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this._lengthUnit);
      pManager.AddGenericParameter("Width [" + unitAbbreviation + "]", "B", "Profile width", GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth [" + unitAbbreviation + "]", "H", "Profile depth", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("Profile", "Pf", "Profile for a GSA Section", GH_ParamAccess.tree);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      this.ClearRuntimeMessages();
      for (int i = 0; i < this.Params.Input.Count; i++)
        this.Params.Input[i].ClearRuntimeMessages();

      #region catalogue
      this.ClearRuntimeMessages();
      if (this._mode == FoldMode.Catalogue)
      {
        // get user input filter search string
        bool incl = false;
        if (DA.GetData(1, ref incl))
        {
          if (this._inclSS != incl)
          {
            this._inclSS = incl;
            this.UpdateTypeData();
            this._sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(_typeNumbers, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), this._inclSS);

            this.SelectedItems[2] = this._typeNames[0];
            this.DropDownItems[2] = this._typeNames;

            this.SelectedItems[3] = this._sectionList[0];
            this.DropDownItems[3] = this._sectionList;

            base.UpdateUI();
          }
        }

        // get user input filter search string
        this._search = null;
        string inSearch = "";
        if (DA.GetData(0, ref inSearch))
        {
          this._search = inSearch.Trim().ToLower().Replace(".", string.Empty).Replace("*", ".*").Replace(" ", ".*");
          if (this._search == "cat")
          {
            string eventName = "EasterCat";
            Dictionary<string, object> properties = new Dictionary<string, object>();
            _ = PostHog.SendToPostHog(GsaGH.PluginInfo.Instance, eventName, properties);
            DA.SetDataList(0, EasterCat);
            return;
          }
          if (this._search.Contains("cat"))
          {
            string[] s = _search.Split(new string[] { "cat" }, StringSplitOptions.None);
            this._search = s[s.Length - 1];
          }

          // boolean to save state of search string being 'problematic' to avoid checking this every single time
          bool tryHard = Regex.Match(this._search, "he[abcm]", RegexOptions.Singleline).Success;

          // filter by search pattern
          List<string> filteredlist = new List<string>();
          if (this.SelectedItems[3] != "All")
          {
            if (!MatchAndAdd(this.SelectedItems[3], this._search, ref filteredlist, tryHard))
            {
              this.ProfileString = new List<string>();
              this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No profile found that matches selected profile and search!");
            }
          }
          else if (this._search != "")
          {
            for (int k = 0; k < this._sectionList.Count; k++)
            {
              if (MatchAndAdd(this._sectionList[k], this._search, ref filteredlist, tryHard))
              {

              }
              else if (!this._search.Any(char.IsDigit))
              {
                string test = this._sectionList[k].ToString();
                test = Regex.Replace(test, "[0-9]", string.Empty);
                test = test.Replace(".", string.Empty);
                test = test.Replace("-", string.Empty);
                test = test.ToLower();
                if (test.Contains(this._search))
                {
                  filteredlist.Add(this._sectionList[k]);
                }
              }
            }
          }
          this.ProfileString = new List<string>();
          if (filteredlist.Count > 0)
          {
            foreach (string profile in filteredlist)
              this.ProfileString.Add("CAT " + profile);
          }
          else
          {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No profile found that matches selection and search!");
            return;
          }
        }

        if (this._search == null)
          this.UpdateProfileString();

        DataTree<string> tree = new DataTree<string>();

        int pathCount = 0;
        if (this.Params.Output[0].VolatileDataCount > 0)
          pathCount = this.Params.Output[0].VolatileData.PathCount;

        GH_Path path = new Grasshopper.Kernel.Data.GH_Path(new int[] { pathCount });
        tree.AddRange(this.ProfileString, path);

        DA.SetDataTree(0, tree);
      }
      #endregion

      #region std
      if (_mode == FoldMode.Other)
      {
        string unit = "(" + Length.GetAbbreviation(this._lengthUnit, new CultureInfo("en")) + ") ";
        string profile = "STD ";
        // angle
        if (this.type == "IAngleProfile") //(typ.Name.Equals(typeof(IAngleProfile).Name))
        {
          profile += "A" + unit +
              Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = IAngleProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    Input.Web(this, DA, 2));
        }

        // channel
        else if (this.type == "IChannelProfile") //(typ.Name.Equals(typeof(IChannelProfile).Name))
        {
          profile += "CH" + unit +
              Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = IChannelProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    (IWebConstant)Input.Web(this, DA, 2));
        }

        // circle hollow
        else if (this.type == "ICircleHollowProfile") //(typ.Name.Equals(typeof(ICircleHollowProfile).Name))
        {
          profile += "CHS" + unit +
              Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = ICircleHollowProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit));
        }

        // circle
        else if (this.type == "ICircleProfile") //(typ.Name.Equals(typeof(ICircleProfile).Name))
        {
          profile += "C" + unit +
              Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = ICircleProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit));
        }

        // ICruciformSymmetricalProfile
        else if (this.type == "ICruciformSymmetricalProfile") //(typ.Name.Equals(typeof(ICruciformSymmetricalProfile).Name))
        {
          profile += "X" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = ICruciformSymmetricalProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    (IWebConstant)Input.Web(this, DA, 2));
        }

        // IEllipseHollowProfile
        else if (this.type == "IEllipseHollowProfile") //(typ.Name.Equals(typeof(IEllipseHollowProfile).Name))
        {
          profile += "OVAL" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = IEllipseHollowProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit));
        }

        // IEllipseProfile
        else if (this.type == "IEllipseProfile") //(typ.Name.Equals(typeof(IEllipseProfile).Name))
        {
          profile += "E" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " 2";

          //profile = IEllipseProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit));
        }

        // IGeneralCProfile
        else if (this.type == "IGeneralCProfile") //(typ.Name.Equals(typeof(IGeneralCProfile).Name))
        {
          profile += "GC" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = IGeneralCProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 3, lengthUnit));
        }

        // IGeneralZProfile
        else if (this.type == "IGeneralZProfile") //(typ.Name.Equals(typeof(IGeneralZProfile).Name))
        {
          profile += "GZ" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 4, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 5, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = IGeneralZProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 3, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 4, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 5, lengthUnit));
        }

        // IIBeamAsymmetricalProfile
        else if (this.type == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
        {
          profile += "GI" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 4, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 5, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = IIBeamAsymmetricalProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    Input.Flange(this, DA, 2),
          //    Input.Web(this, DA, 3));
        }

        // IIBeamCellularProfile
        else if (this.type == "IIBeamCellularProfile") //(typ.Name.Equals(typeof(IIBeamCellularProfile).Name))
        {
          profile += "CB" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 4, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 5, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = IIBeamCellularProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    (IWebConstant)Input.Web(this, DA, 2),
          //    Input.LengthOrRatio(this, DA, 3, lengthUnit));
        }

        // IIBeamSymmetricalProfile
        else if (this.type == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
        {
          profile += "I" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = IIBeamSymmetricalProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    (IWebConstant)Input.Web(this, DA, 2));
        }

        // IRectangleHollowProfile
        else if (this.type == "IRectangleHollowProfile") //(typ.Name.Equals(typeof(IRectangleHollowProfile).Name))
        {
          profile += "RHS" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = IRectangleHollowProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    (IWebConstant)Input.Web(this, DA, 2));
        }

        // IRectangleProfile
        else if (this.type == "IRectangleProfile") //(typ.Name.Equals(typeof(IRectangleProfile).Name))
        {
          profile += "R" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = IRectangleProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit));
        }

        // IRectoEllipseProfile
        else if (this.type == "IRectoEllipseProfile") //(typ.Name.Equals(typeof(IRectoEllipseProfile).Name))
        {
          profile += "RE" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString() + " 2";

          //profile = IRectoEllipseProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 3, lengthUnit));
        }

        // ISecantPileProfile
        else if (this.type == "ISecantPileProfile") //(typ.Name.Equals(typeof(ISecantPileProfile).Name))
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
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            pileCount.ToString();

          //profile = ISecantPileProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    pileCount, isWallNotSection);
        }

        // ISheetPileProfile
        else if (this.type == "ISheetPileProfile") //(typ.Name.Equals(typeof(ISheetPileProfile).Name))
        {
          profile += "SHT" + unit +
            Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 4, this._lengthUnit).As(_lengthUnit).ToString() + " " +
            Input.LengthOrRatio(this, DA, 5, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = ISheetPileProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 3, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 4, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 5, lengthUnit));
        }

        // IStadiumProfile
        else if (this.type == "IStadiumProfile") //(typ.Name.Equals(typeof(IStadiumProfile).Name))
        {
          profile += "RC" + unit +
              Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = IStadiumProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit));
        }

        // ITrapezoidProfile
        else if (this.type == "ITrapezoidProfile") //(typ.Name.Equals(typeof(ITrapezoidProfile).Name))
        {
          profile += "TR" + unit +
              Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
              Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = ITrapezoidProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 1, lengthUnit),
          //    Input.LengthOrRatio(this, DA, 2, lengthUnit));
        }

        // ITSectionProfile
        else if (this.type == "ITSectionProfile") //(typ.Name.Equals(typeof(ITSectionProfile).Name))
        {
          profile += "T" + unit +
             Input.LengthOrRatio(this, DA, 0, this._lengthUnit).As(_lengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 1, this._lengthUnit).As(_lengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 2, this._lengthUnit).As(_lengthUnit).ToString() + " " +
             Input.LengthOrRatio(this, DA, 3, this._lengthUnit).As(_lengthUnit).ToString();

          //profile = ITSectionProfile.Create(
          //    Input.LengthOrRatio(this, DA, 0, lengthUnit),
          //    Input.Flange(this, DA, 1),
          //    Input.Web(this, DA, 2));
        }

        // IPerimeterProfile (last chance...)
        else if (this.type == "IPerimeterProfile") //(typ.Name.Equals(typeof(IPerimeterProfile).Name))
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
                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Cannot convert internal edge to Polyline");
                    return;
                  }
                }
                perimeter.voidPoints = voidPoints;
              }
            }
          }
          switch (_lengthUnit)
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
    private void Mode1Clicked()
    {
      // remove input parameters
      while (this.Params.Input.Count > 0)
        this.Params.UnregisterInputParameter(this.Params.Input[0], true);

      // register input parameter
      this.Params.RegisterInputParam(new Param_String());
      this.Params.RegisterInputParam(new Param_Boolean());

      this._mode = FoldMode.Catalogue;

      base.UpdateUI();
    }

    private void SetNumberOfGenericInputs(int inputs, bool isSecantPile = false, bool isPerimeter = false)
    {
      this._numberOfInputs = inputs;

      // if last input previously was a bool and we no longer need that
      if (this.lastInputWasSecant || isSecantPile || isPerimeter)
      {
        if (this.Params.Input.Count > 0)
        {
          // make sure to remove last param
          this.Params.UnregisterInputParameter(this.Params.Input[this.Params.Input.Count - 1], true);
          this.Params.UnregisterInputParameter(this.Params.Input[this.Params.Input.Count - 1], true);
        }
      }

      // remove any additional inputs
      while (this.Params.Input.Count > inputs)
        this.Params.UnregisterInputParameter(this.Params.Input[inputs], true);

      if (isSecantPile) // add two less generic than input says
      {
        while (this.Params.Input.Count > inputs + 2)
          this.Params.UnregisterInputParameter(this.Params.Input[inputs + 2], true);
        inputs -= 2;
      }

      // add inputs parameter
      while (this.Params.Input.Count < inputs)
        this.Params.RegisterInputParam(new Param_GenericObject());

      if (isSecantPile) // finally add int and bool param if secant
      {
        this.Params.RegisterInputParam(new Param_Integer());
        this.Params.RegisterInputParam(new Param_Boolean());
        this.lastInputWasSecant = true;
      }
      else
        this.lastInputWasSecant = false;

      if (isPerimeter)
        this.Params.RegisterInputParam(new Param_Plane());
    }

    private void Mode2Clicked()
    {
      // check if mode is correct
      if (this._mode != FoldMode.Other)
      {
        // if we come from catalogue mode remove all input parameters
        while (this.Params.Input.Count > 0)
          this.Params.UnregisterInputParameter(this.Params.Input[0], true);

        // set mode to other
        this._mode = FoldMode.Other;
      }

      // angle
      if (this.type == "IAngleProfile") //(typ.Name.Equals(typeof(IAngleProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IAngleProfile.Create(angle.Depth, angle.Flange, angle.Web);
      }

      // channel
      else if (this.type == "IChannelProfile") //(typ.Name.Equals(typeof(IChannelProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IChannelProfile.Create(channel.Depth, channel.Flanges, channel.Web);
      }

      // circle hollow
      else if (this.type == "ICircleHollowProfile") //(typ.Name.Equals(typeof(ICircleHollowProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = ICircleHollowProfile.Create(circleHollow.Diameter, circleHollow.WallThickness);
      }

      // circle
      else if (this.type == "ICircleProfile") //(typ.Name.Equals(typeof(ICircleProfile).Name))
      {
        SetNumberOfGenericInputs(1);
        //dup = ICircleProfile.Create(circle.Diameter);
      }

      // ICruciformSymmetricalProfile
      else if (this.type == "ICruciformSymmetricalProfile") //(typ.Name.Equals(typeof(ICruciformSymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = ICruciformSymmetricalProfile.Create(cruciformSymmetrical.Depth, cruciformSymmetrical.Flange, cruciformSymmetrical.Web);
      }

      // IEllipseHollowProfile
      else if (this.type == "IEllipseHollowProfile") //(typ.Name.Equals(typeof(IEllipseHollowProfile).Name))
      {
        SetNumberOfGenericInputs(3);
        //dup = IEllipseHollowProfile.Create(ellipseHollow.Depth, ellipseHollow.Width, ellipseHollow.WallThickness);
      }

      // IEllipseProfile
      else if (this.type == "IEllipseProfile") //(typ.Name.Equals(typeof(IEllipseProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IEllipseProfile.Create(ellipse.Depth, ellipse.Width);
      }

      // IGeneralCProfile
      else if (this.type == "IGeneralCProfile") //(typ.Name.Equals(typeof(IGeneralCProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IGeneralCProfile.Create(generalC.Depth, generalC.FlangeWidth, generalC.Lip, generalC.Thickness);
      }

      // IGeneralZProfile
      else if (this.type == "IGeneralZProfile") //(typ.Name.Equals(typeof(IGeneralZProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = IGeneralZProfile.Create(generalZ.Depth, generalZ.TopFlangeWidth, generalZ.BottomFlangeWidth, generalZ.TopLip, generalZ.BottomLip, generalZ.Thickness);
      }

      // IIBeamAsymmetricalProfile
      else if (this.type == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = IIBeamAsymmetricalProfile.Create(iBeamAsymmetrical.Depth, iBeamAsymmetrical.TopFlange, iBeamAsymmetrical.BottomFlange, iBeamAsymmetrical.Web);
      }

      // IIBeamCellularProfile
      else if (this.type == "IIBeamCellularProfile") //(typ.Name.Equals(typeof(IIBeamCellularProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = IIBeamCellularProfile.Create(iBeamCellular.Depth, iBeamCellular.Flanges, iBeamCellular.Web, iBeamCellular.WebOpening);
      }

      // IIBeamSymmetricalProfile
      else if (this.type == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IIBeamSymmetricalProfile.Create(iBeamSymmetrical.Depth, iBeamSymmetrical.Flanges, iBeamSymmetrical.Web);
      }

      // IRectangleHollowProfile
      else if (this.type == "IRectangleHollowProfile") //(typ.Name.Equals(typeof(IRectangleHollowProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IRectangleHollowProfile.Create(rectangleHollow.Depth, rectangleHollow.Flanges, rectangleHollow.Webs);
      }

      // IRectangleProfile
      else if (this.type == "IRectangleProfile") //(typ.Name.Equals(typeof(IRectangleProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IRectangleProfile.Create(rectangle.Depth, rectangle.Width);
      }

      // IRectoEllipseProfile
      else if (this.type == "IRectoEllipseProfile") //(typ.Name.Equals(typeof(IRectoEllipseProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = IRectoEllipseProfile.Create(rectoEllipse.Depth, rectoEllipse.DepthFlat, rectoEllipse.Width, rectoEllipse.WidthFlat);
      }

      // ISecantPileProfile
      else if (this.type == "ISecantPileProfile") //(typ.Name.Equals(typeof(ISecantPileProfile).Name))
      {
        SetNumberOfGenericInputs(4, true);
        //dup = ISecantPileProfile.Create(secantPile.Diameter, secantPile.PileCentres, secantPile.PileCount, secantPile.IsWallNotSection);
      }

      // ISheetPileProfile
      else if (this.type == "ISheetPileProfile") //(typ.Name.Equals(typeof(ISheetPileProfile).Name))
      {
        SetNumberOfGenericInputs(6);
        //dup = ISheetPileProfile.Create(sheetPile.Depth, sheetPile.Width, sheetPile.TopFlangeWidth, sheetPile.BottomFlangeWidth, sheetPile.FlangeThickness, sheetPile.WebThickness);
      }

      // IStadiumProfile
      else if (this.type == "IStadiumProfile") //(typ.Name.Equals(typeof(IStadiumProfile).Name))
      {
        SetNumberOfGenericInputs(2);
        //dup = IStadiumProfile.Create(stadium.Depth, stadium.Width);
      }

      // ITrapezoidProfile
      else if (this.type == "ITrapezoidProfile") //(typ.Name.Equals(typeof(ITrapezoidProfile).Name))
      {
        SetNumberOfGenericInputs(3);
        //dup = ITrapezoidProfile.Create(trapezoid.Depth, trapezoid.TopWidth, trapezoid.BottomWidth);
      }

      // ITSectionProfile
      else if (this.type == "ITSectionProfile") //(typ.Name.Equals(typeof(ITSectionProfile).Name))
      {
        SetNumberOfGenericInputs(4);
        //dup = ITSectionProfile.Create(tSection.Depth, tSection.Flange, tSection.Web);
      }
      // IPerimeterProfile
      else if (this.type == "IPerimeterProfile") //(typ.Name.Equals(typeof(IPerimeterProfile).Name))
      {
        SetNumberOfGenericInputs(1, false, true);

        //dup = IPerimeterProfile.Create();
        //solidPolygon;
        //voidPolygons;
      }

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      this.Params.OnParametersChanged();
      this.ExpireSolution(true);
    }
    #endregion

    #region Custom UI
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[] {
        "Profile type",
        "Measure",
        "Type",
        "Profile"
      });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Profile type
      this.DropDownItems.Add(_profileTypes.Keys.ToList());
      this.SelectedItems.Add("Rectangle");

      // Length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this._lengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      // input -1 to force update of catalogue sections to include/exclude superseeded
      bool updateCat = false;
      if (i == -1)
      {
        this.SelectedItems[0] = "Catalogue";
        updateCat = true;
        i = 0;
      }
      else
      {
        // change selected item
        this.SelectedItems[i] = this.DropDownItems[i][j];
      }

      if (this.SelectedItems[0] == "Catalogue")
      {
        // update spacer description to match catalogue dropdowns
        this.SpacerDescriptions[1] = "Catalogue";

        // if FoldMode is not currently catalogue state, then we update all lists
        if (this._mode != FoldMode.Catalogue | updateCat)
        {
          // remove any existing selections
          while (this.SelectedItems.Count > 1)
            this.SelectedItems.RemoveAt(1);

          // set catalogue selection to all
          this._catalogueIndex = -1;

          this._catalogueNames = this._cataloguedata.Item1;
          this._catalogueNumbers = this._cataloguedata.Item2;

          // set types to all
          this._typeIndex = -1;
          // update typelist with all catalogues
          this.UpdateTypeData();

          // update section list to all types
          this._sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(_typeNumbers, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), this._inclSS);

          // update displayed selections to all
          this.SelectedItems.Add(this._catalogueNames[0]);
          this.SelectedItems.Add(this._typeNames[0]);
          this.SelectedItems.Add(this._sectionList[0]);

          // call graphics update
          this.Mode1Clicked();
        }

        // update dropdown lists
        while (this.DropDownItems.Count > 1)
          this.DropDownItems.RemoveAt(1);

        // add catalogues (they will always be the same so no need to rerun sql call)
        this.DropDownItems.Add(this._catalogueNames);

        // type list
        // if second list (i.e. catalogue list) is changed, update types list to account for that catalogue
        if (i == 1)
        {
          // update catalogue index with the selected catalogue
          this._catalogueIndex = this._catalogueNumbers[j];
          this.SelectedItems[1] = this._catalogueNames[j];

          // update typelist with selected input catalogue
          this._typedata = MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(_catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), this._inclSS);
          this._typeNames = this._typedata.Item1;
          this._typeNumbers = this._typedata.Item2;

          // update section list from new types (all new types in catalogue)
          List<int> types = this._typeNumbers.ToList();
          types.RemoveAt(0); // remove -1 from beginning of list
          this._sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(types, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), this._inclSS);

          // update selections to display first item in new list
          this.SelectedItems[2] = this._typeNames[0];
          this.SelectedItems[3] = this._sectionList[0];
        }
        this.DropDownItems.Add(this._typeNames);

        // section list
        // if third list (i.e. types list) is changed, update sections list to account for these section types
        if (i == 2)
        {
          // update catalogue index with the selected catalogue
          this._typeIndex = this._typeNumbers[j];
          this.SelectedItems[2] = this._typeNames[j];

          // create type list
          List<int> types = new List<int>();
          if (this._typeIndex == -1) // if all
          {
            types = this._typeNumbers.ToList(); // use current selected list of type numbers
            types.RemoveAt(0); // remove -1 from beginning of list
          }
          else
            types = new List<int> { this._typeIndex }; // create empty list and add the single selected type 


          // section list with selected types (only types in selected type)
          this._sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(types, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), this._inclSS);

          // update selected section to be all
          this.SelectedItems[3] = this._sectionList[0];
        }
        this.DropDownItems.Add(this._sectionList);

        // selected profile
        // if fourth list (i.e. section list) is changed, updated the sections list to only be that single profile
        if (i == 3)
        {
          // update displayed selected
          this.SelectedItems[3] = this._sectionList[j];
        }

        if (this._search == "")
          this.UpdateProfileString();

        base.UpdateUI();
      }
      else
      {
        // update spacer description to match none-catalogue dropdowns
        this.SpacerDescriptions[1] = "Measure";// = new List<string>(new string[]

        if (this._mode != FoldMode.Other)
        {
          // remove all catalogue dropdowns
          while (this.DropDownItems.Count > 1)
            this.DropDownItems.RemoveAt(1);

          // add length measure dropdown list
          this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));

          // set selected length
          this.SelectedItems[1] = this._lengthUnit.ToString();
        }

        if (i == 0)
        {
          // update profile type if change is made to first dropdown menu
          this.type = _profileTypes[this.SelectedItems[0]];
          Mode2Clicked();
        }
        else
        {
          // change unit
          this._lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);

          base.UpdateUI();
        }
      }
    }

    private void UpdateTypeData()
    {
      this._typedata = GetTypesDataFromSQLite(_catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), this._inclSS);
      this._typeNames = this._typedata.Item1;
      this._typeNumbers = this._typedata.Item2;
    }

    private void UpdateProfileString()
    {
      if (this.SelectedItems[3] == "All")
      {
        this.ProfileString = new List<string>();
        foreach (string profile in _sectionList)
        {
          if (profile == "All")
            continue;
          this.ProfileString.Add("CAT " + profile);
        }
      }
      else
        this.ProfileString = new List<string>() { "CAT " + this.SelectedItems[3] };
    }

    public override void UpdateUIFromSelectedItems()
    {
      if (this.SelectedItems[0] == "Catalogue")
      {
        // update spacer description to match catalogue dropdowns
        this.SpacerDescriptions = new List<string>(new string[]
        {
          "Profile type", "Catalogue", "Type", "Profile"
        });

        this._catalogueNames = this._cataloguedata.Item1;
        this._catalogueNumbers = this._cataloguedata.Item2;
        this._typedata = MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(this._catalogueIndex, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), this._inclSS);
        this._typeNames = this._typedata.Item1;
        this._typeNumbers = this._typedata.Item2;

        this.Mode1Clicked();

        this.ProfileString = new List<string>() { "CAT " + this.SelectedItems[3] };
      }
      else
      {
        // update spacer description to match none-catalogue dropdowns
        this.SpacerDescriptions = new List<string>(new string[]
        {
          "Profile type", "Measure", "Type", "Profile"
        });

        this.type = _profileTypes[SelectedItems[0]];
        this.Mode2Clicked();
      }

      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      if (this._mode == FoldMode.Catalogue)
      {
        int i = 0;
        this.Params.Input[i].NickName = "S";
        this.Params.Input[i].Name = "Search";
        this.Params.Input[i].Description = "Text to search from";
        this.Params.Input[i].Access = GH_ParamAccess.item;
        this.Params.Input[i].Optional = true;

        i++;
        this.Params.Input[i].NickName = "iSS";
        this.Params.Input[i].Name = "InclSuperseeded";
        this.Params.Input[i].Description = "Input true to include superseeded catalogue sections";
        this.Params.Input[i].Access = GH_ParamAccess.item;
        this.Params.Input[i].Optional = true;
      }
      else
      {
        string unitAbbreviation = Length.GetAbbreviation(this._lengthUnit);

        int i = 0;
        // angle
        if (this.type == "IAngleProfile") //(typ.Name.Equals(typeof(IAngleProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the angle profile (leg in the local z axis).";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "W";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The width of the angle profile (leg in the local y axis).";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tw";
          this.Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The web thickness of the angle profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tf";
          this.Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The flange thickness of the angle profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          //dup = IAngleProfile.Create(angle.Depth, angle.Flange, angle.Web);
        }

        // channel
        else if (this.type == "IChannelProfile") //(typ.Name.Equals(typeof(IChannelProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the channel profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The width of the flange of the channel profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tw";
          this.Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The web thickness of the channel profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tf";
          this.Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The flange thickness of the channel profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          this.Params.Input[i].Optional = false;
          //dup = IChannelProfile.Create(channel.Depth, channel.Flanges, channel.Web);
        }

        // circle hollow
        else if (this.type == "ICircleHollowProfile") //(typ.Name.Equals(typeof(ICircleHollowProfile).Name))
        {
          this.Params.Input[i].NickName = "Ø";
          this.Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The diameter of the hollow circle.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "t";
          this.Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The wall thickness of the hollow circle.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = ICircleHollowProfile.Create(circleHollow.Diameter, circleHollow.WallThickness);
        }

        // circle
        else if (this.type == "ICircleProfile") //(typ.Name.Equals(typeof(ICircleProfile).Name))
        {
          this.Params.Input[i].NickName = "Ø";
          this.Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The diameter of the circle.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          //dup = ICircleProfile.Create(circle.Diameter);
        }

        // ICruciformSymmetricalProfile
        else if (this.type == "ICruciformSymmetricalProfile") //(typ.Name.Equals(typeof(ICruciformSymmetricalProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth (local z axis leg) of the profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The width of the flange (local y axis leg) of the cruciform.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tw";
          this.Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The web thickness of the cruciform.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tf";
          this.Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The flange thickness of the cruciform.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = ICruciformSymmetricalProfile.Create(cruciformSymmetrical.Depth, cruciformSymmetrical.Flange, cruciformSymmetrical.Web);
        }

        // IEllipseHollowProfile
        else if (this.type == "IEllipseHollowProfile") //(typ.Name.Equals(typeof(IEllipseHollowProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the hollow ellipse.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The width of the hollow ellipse.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "t";
          this.Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The wall thickness of the hollow ellipse.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = IEllipseHollowProfile.Create(ellipseHollow.Depth, ellipseHollow.Width, ellipseHollow.WallThickness);
        }

        // IEllipseProfile
        else if (this.type == "IEllipseProfile") //(typ.Name.Equals(typeof(IEllipseProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the ellipse.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The width of the ellipse.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = IEllipseProfile.Create(ellipse.Depth, ellipse.Width);
        }

        // IGeneralCProfile
        else if (this.type == "IGeneralCProfile") //(typ.Name.Equals(typeof(IGeneralCProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the generic c section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The flange width of the generic c section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "L";
          this.Params.Input[i].Name = "Lip [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The lip of the generic c section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "t";
          this.Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The thickness of the generic c section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = IGeneralCProfile.Create(generalC.Depth, generalC.FlangeWidth, generalC.Lip, generalC.Thickness);
        }

        // IGeneralZProfile
        else if (this.type == "IGeneralZProfile") //(typ.Name.Equals(typeof(IGeneralZProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the generic z section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Bt";
          this.Params.Input[i].Name = "TopWidth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The top flange width of the generic z section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Bb";
          this.Params.Input[i].Name = "BottomWidth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The bottom flange width of the generic z section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Lt";
          this.Params.Input[i].Name = "Top Lip [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The top lip of the generic z section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Lb";
          this.Params.Input[i].Name = "Lip [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The top lip of the generic z section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "t";
          this.Params.Input[i].Name = "Thickness [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The thickness of the generic z section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = IGeneralZProfile.Create(generalZ.Depth, generalZ.TopFlangeWidth, generalZ.BottomFlangeWidth, generalZ.TopLip, generalZ.BottomLip, generalZ.Thickness);
        }

        // IIBeamAsymmetricalProfile
        else if (this.type == "IIBeamAsymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamAsymmetricalProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Bt";
          this.Params.Input[i].Name = "TopFlangeWidth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The width of the top flange of the beam. Top is relative to the beam local access.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Bb";
          this.Params.Input[i].Name = "BottomFlangeWidth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The width of the bottom flange of the beam. Bottom is relative to the beam local access.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Web";
          this.Params.Input[i].Name = "Web Thickness [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The web thickness of the beam.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tt";
          this.Params.Input[i].Name = "TopFlangeThk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The top flange thickness.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tb";
          this.Params.Input[i].Name = "BottomFlangeThk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The bpttom flange thickness.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          //dup = IIBeamAsymmetricalProfile.Create(iBeamAsymmetrical.Depth, iBeamAsymmetrical.TopFlange, iBeamAsymmetrical.BottomFlange, iBeamAsymmetrical.Web);
        }

        // IIBeamCellularProfile
        else if (this.type == "IIBeamCellularProfile") //(typ.Name.Equals(typeof(IIBeamCellularProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The width of the flanges of the beam.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tw";
          this.Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The web thickness of the angle profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tf";
          this.Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The flange thickness of the angle profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "O";
          this.Params.Input[i].Name = "WebOpening [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The size of the web opening.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "P";
          this.Params.Input[i].Name = "Pitch [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The pitch (spacing) between the web openings.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = IIBeamCellularProfile.Create(iBeamCellular.Depth, iBeamCellular.Flanges, iBeamCellular.Web, iBeamCellular.WebOpening);
        }

        // IIBeamSymmetricalProfile
        else if (this.type == "IIBeamSymmetricalProfile") //(typ.Name.Equals(typeof(IIBeamSymmetricalProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The width of the flanges of the beam.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tw";
          this.Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The web thickness of the angle profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tf";
          this.Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The flange thickness of the angle profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          //dup = IIBeamSymmetricalProfile.Create(iBeamSymmetrical.Depth, iBeamSymmetrical.Flanges, iBeamSymmetrical.Web);
        }

        // IRectangleHollowProfile
        else if (this.type == "IRectangleHollowProfile") //(typ.Name.Equals(typeof(IRectangleHollowProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the hollow rectangle.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The width of the hollow rectangle.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tw";
          this.Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The side thickness of the hollow rectangle.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tf";
          this.Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The top/bottom thickness of the hollow rectangle.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = IRectangleHollowProfile.Create(rectangleHollow.Depth, rectangleHollow.Flanges, rectangleHollow.Webs);
        }

        // IRectangleProfile
        else if (this.type == "IRectangleProfile") //(typ.Name.Equals(typeof(IRectangleProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "Depth of the rectangle, in local z-axis direction.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "Width of the rectangle, in local y-axis direction.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = IRectangleProfile.Create(rectangle.Depth, rectangle.Width);
        }

        // IRectoEllipseProfile
        else if (this.type == "IRectoEllipseProfile") //(typ.Name.Equals(typeof(IRectoEllipseProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The overall depth of the recto-ellipse profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Df";
          this.Params.Input[i].Name = "DepthFlat [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The flat length of the profile's overall depth.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The overall width of the recto-ellipse profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Bf";
          this.Params.Input[i].Name = "WidthFlat [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The flat length of the profile's overall width.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = IRectoEllipseProfile.Create(rectoEllipse.Depth, rectoEllipse.DepthFlat, rectoEllipse.Width, rectoEllipse.WidthFlat);
        }

        // ISecantPileProfile
        else if (this.type == "ISecantPileProfile") //(typ.Name.Equals(typeof(ISecantPileProfile).Name))
        {
          this.Params.Input[i].NickName = "Ø";
          this.Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The diameter of the piles.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "c/c";
          this.Params.Input[i].Name = "PileCentres [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The centre to centre distance between adjacent piles.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "No";
          this.Params.Input[i].Name = "PileCount";
          this.Params.Input[i].Description = "The number of piles in the profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "W/S";
          this.Params.Input[i].Name = "isWall";
          this.Params.Input[i].Description = "Converts the profile into a wall secant pile profile if true -- Converts the profile into a section secant pile profile if false.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = ISecantPileProfile.Create(secantPile.Diameter, secantPile.PileCentres, secantPile.PileCount, secantPile.IsWallNotSection);
        }

        // ISheetPileProfile
        else if (this.type == "ISheetPileProfile") //(typ.Name.Equals(typeof(ISheetPileProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the sheet pile section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The overall width of the sheet pile section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Bt";
          this.Params.Input[i].Name = "TopFlangeWidth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The top flange width of the sheet pile section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Bb";
          this.Params.Input[i].Name = "BottomFlangeWidth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The bottom flange width of the sheet pile section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Ft";
          this.Params.Input[i].Name = "FlangeThickness [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The flange thickness of the sheet pile section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Wt";
          this.Params.Input[i].Name = "WebThickness [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The web thickness of the sheet pile section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = ISheetPileProfile.Create(sheetPile.Depth, sheetPile.Width, sheetPile.TopFlangeWidth, sheetPile.BottomFlangeWidth, sheetPile.FlangeThickness, sheetPile.WebThickness);
        }

        // IStadiumProfile
        else if (this.type == "IStadiumProfile") //(typ.Name.Equals(typeof(IStadiumProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The profile's overall depth considering the side length of the rectangle and the radii of the semicircles on the two ends.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The profile's width (diameter of the semicircles).";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = IStadiumProfile.Create(stadium.Depth, stadium.Width);
        }

        // ITrapezoidProfile
        else if (this.type == "ITrapezoidProfile") //(typ.Name.Equals(typeof(ITrapezoidProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth in z-axis direction of trapezoidal profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Bt";
          this.Params.Input[i].Name = "TopWidth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The top width of trapezoidal profile. Top is relative to the local z-axis.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Bb";
          this.Params.Input[i].Name = "BottomWidth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The bottom width of trapezoidal profile. Bottom is relative to the local z-axis.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;
          //dup = ITrapezoidProfile.Create(trapezoid.Depth, trapezoid.TopWidth, trapezoid.BottomWidth);
        }

        // ITSectionProfile
        else if (this.type == "ITSectionProfile") //(typ.Name.Equals(typeof(ITSectionProfile).Name))
        {
          this.Params.Input[i].NickName = "D";
          this.Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The depth of the T section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The width of the T section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tw";
          this.Params.Input[i].Name = "Web Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The web thickness of the T section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          i++;
          this.Params.Input[i].NickName = "Tf";
          this.Params.Input[i].Name = "Flange Thk [" + unitAbbreviation + "]";
          this.Params.Input[i].Description = "The flange thickness of the T section profile.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          //dup = ITSectionProfile.Create(tSection.Depth, tSection.Flange, tSection.Web);
        }
        // IPerimeterProfile
        else if (this.type == "IPerimeterProfile") //(typ.Name.Equals(typeof(IPerimeterProfile).Name))
        {
          this.Params.Input[i].NickName = "B";
          this.Params.Input[i].Name = "Boundary";
          this.Params.Input[i].Description = "Planar Brep or closed planar curve.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = false;

          if (Params.Input.Count == 1) // handle backwards compatability
            Params.RegisterInputParam(new Param_Plane());

          i++;
          this.Params.Input[i].NickName = "P";
          this.Params.Input[i].Name = "Plane";
          this.Params.Input[i].Description = "Optional plane in which to project boundary onto. Profile will get coordinates in this plane.";
          this.Params.Input[i].Access = GH_ParamAccess.item;
          this.Params.Input[i].Optional = true;

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
      writer.SetString("mode", this._mode.ToString());
      writer.SetString("lengthUnit", this._lengthUnit.ToString());
      writer.SetBoolean("inclSS", this._inclSS);
      writer.SetInt32("NumberOfInputs", this._numberOfInputs);
      writer.SetInt32("catalogueIndex", this._catalogueIndex);
      writer.SetInt32("typeIndex", this._typeIndex);
      writer.SetString("search", this._search);
      return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this._mode = (FoldMode)Enum.Parse(typeof(FoldMode), reader.GetString("mode"));
      this._lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("lengthUnit"));
      this._inclSS = reader.GetBoolean("inclSS");
      this._numberOfInputs = reader.GetInt32("NumberOfInputs");
      this._catalogueIndex = reader.GetInt32("catalogueIndex");
      this._typeIndex = reader.GetInt32("typeIndex");
      this._search = reader.GetString("search");

      bool flag = base.Read(reader);
      this.Params.Output[0].Access = GH_ParamAccess.tree;
      return flag;
    }
    #endregion
  }
}
