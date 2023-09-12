using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaGH.Graveyard;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create AdSec profile
  /// </summary>
  public class CreateProfile_OBSOLETE : GH_OasysDropDownComponent {
    private enum FoldMode {
      Catalogue,
      Other,
    }

    public override Guid ComponentGuid => new Guid("ea1741e5-905e-4ecb-8270-a584e3f99aa3");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateProfile;
    private static readonly List<string> easterCat = new List<string>() {
      "▌─────────────────────────▐█─────▐" + Environment.NewLine
      + "▌────▄──────────────────▄█▓█▌────▐" + Environment.NewLine
      + "▌───▐██▄───────────────▄▓░░▓▓────▐" + Environment.NewLine
      + "▌───▐█░██▓────────────▓▓░░░▓▌────▐" + Environment.NewLine
      + "▌───▐█▌░▓██──────────█▓░░░░▓─────▐" + Environment.NewLine
      + "▌────▓█▌░░▓█▄███████▄███▓░▓█─────▐" + Environment.NewLine
      + "▌────▓██▌░▓██░░░░░░░░░░▓█░▓▌─────▐" + Environment.NewLine
      + "▌─────▓█████░░░░░░░░░░░░▓██──────▐" + Environment.NewLine
      + "▌─────▓██▓░░░░░░░░░░░░░░░▓█──────▐" + Environment.NewLine
      + "▌─────▐█▓░░░░░░█▓░░▓█░░░░▓█▌─────▐" + Environment.NewLine
      + "▌─────▓█▌░▓█▓▓██▓░█▓▓▓▓▓░▓█▌─────▐" + Environment.NewLine
      + "▌─────▓▓░▓██████▓░▓███▓▓▌░█▓─────▐" + Environment.NewLine
      + "▌────▐▓▓░█▄▐▓▌█▓░░▓█▐▓▌▄▓░██─────▐" + Environment.NewLine
      + "▌────▓█▓░▓█▄▄▄█▓░░▓█▄▄▄█▓░██▌────▐" + Environment.NewLine
      + "▌────▓█▌░▓█████▓░░░▓███▓▀░▓█▓────▐" + Environment.NewLine
      + "▌───▐▓█░░░▀▓██▀░░░░░─▀▓▀░░▓█▓────▐" + Environment.NewLine
      + "▌───▓██░░░░░░░░▀▄▄▄▄▀░░░░░░▓▓────▐" + Environment.NewLine
      + "▌───▓█▌░░░░░░░░░░▐▌░░░░░░░░▓▓▌───▐" + Environment.NewLine
      + "▌───▓█░░░░░░░░░▄▀▀▀▀▄░░░░░░░█▓───▐" + Environment.NewLine
      + "▌──▐█▌░░░░░░░░▀░░░░░░▀░░░░░░█▓▌──▐" + Environment.NewLine
      + "▌──▓█░░░░░░░░░░░░░░░░░░░░░░░██▓──▐" + Environment.NewLine
      + "▌──▓█░░░░░░░░░░░░░░░░░░░░░░░▓█▓──▐" + Environment.NewLine
      + "▌──██░░░░░░░░░░░░░░░░░░░░░░░░█▓──▐" + Environment.NewLine
      + "▌──█▌░░░░░░░░░░░░░░░░░░░░░░░░▐▓▌─▐" + Environment.NewLine
      + "▌─▐▓░░░░░░░░░░░░░░░░░░░░░░░░░░█▓─▐" + Environment.NewLine
      + "▌─█▓░░░░░░░░░░░░░░░░░░░░░░░░░░▓▓─▐" + Environment.NewLine
      + "▌─█▓░░░░░░░░░░░░░░░░░░░░░░░░░░▓▓▌▐" + Environment.NewLine
      + "▌▐█▓░░░░░░░░░░░░░░░░░░░░░░░░░░░██▐" + Environment.NewLine
      + "▌█▓▌░░░░░░░░░░░░░░░░░░░░░░░░░░░▓█▐", };
    private static readonly Dictionary<string, string> profileTypes
      = new Dictionary<string, string> {
        {
          "Angle", "IAngleProfile"
        }, {
          "Catalogue", "ICatalogueProfile"
        }, {
          "Channel", "IChannelProfile"
        }, {
          "Circle Hollow", "ICircleHollowProfile"
        }, {
          "Circle", "ICircleProfile"
        }, {
          "Cruciform Symmetrical", "ICruciformSymmetricalProfile"
        }, {
          "Ellipse Hollow", "IEllipseHollowProfile"
        }, {
          "Ellipse", "IEllipseProfile"
        }, {
          "General C", "IGeneralCProfile"
        }, {
          "General Z", "IGeneralZProfile"
        }, {
          "I Beam Asymmetrical", "IIBeamAsymmetricalProfile"
        }, {
          "I Beam Cellular", "IIBeamCellularProfile"
        }, {
          "I Beam Symmetrical", "IIBeamSymmetricalProfile"
        }, {
          "Perimeter", "IPerimeterProfile"
        }, {
          "Rectangle Hollow", "IRectangleHollowProfile"
        }, {
          "Rectangle", "IRectangleProfile"
        }, {
          "Recto Ellipse", "IRectoEllipseProfile"
        }, {
          "Recto Circle", "IStadiumProfile"
        }, {
          "Secant Pile", "ISecantPileProfile"
        }, {
          "Sheet Pile", "ISheetPileProfile"
        }, {
          "Trapezoid", "ITrapezoidProfile"
        }, {
          "T Section", "ITSectionProfile"
        },
      };
    private readonly Tuple<List<string>, List<int>> _cataloguedata;
    private int _catalogueIndex = -1;
    private List<string> _catalogueNames = new List<string>();
    private List<int> _catalogueNumbers = new List<int>();
    private bool _inclSs;
    private bool _lastInputWasSecant;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;
    private FoldMode _mode = FoldMode.Other;
    private int _numberOfInputs;
    private List<string> _profileString = new List<string>() {
      "CAT HE HE200.B",
    };
    private string _search = string.Empty;
    private List<string> _sectionList;
    private string _type = "IRectangleProfile";
    private Tuple<List<string>, List<int>> _typedata;
    private int _typeIndex = -1;
    private List<string> _typeNames = new List<string>();
    private List<int> _typeNumbers = new List<int>();

    public CreateProfile_OBSOLETE() : base("Create Profile", "Profile",
      "Create Profile text-string for a GSA Section", CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
      _cataloguedata
        = MicrosoftSQLiteReader.Instance.GetCataloguesDataFromSQLite(
          Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
      _typedata = MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(-1,
        Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
      _sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(new List<int> {
        -1,
      }, Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
    }

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)Enum.Parse(typeof(FoldMode), reader.GetString("mode"));
      _lengthUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("lengthUnit"));
      _inclSs = reader.GetBoolean("inclSS");
      _numberOfInputs = reader.GetInt32("NumberOfInputs");
      _catalogueIndex = reader.GetInt32("catalogueIndex");
      _typeIndex = reader.GetInt32("typeIndex");
      _search = reader.GetString("search");

      bool flag = base.Read(reader);
      Params.Output[0].Access = GH_ParamAccess.tree;
      return flag;
    }

    public override void SetSelected(int i, int j) {
      bool updateCat = false;
      if (i == -1) {
        _selectedItems[0] = "Catalogue";
        updateCat = true;
        i = 0;
      } else {
        _selectedItems[i] = _dropDownItems[i][j];
      }

      if (_selectedItems[0] == "Catalogue") {
        _spacerDescriptions[1] = "Catalogue";

        if ((_mode != FoldMode.Catalogue) | updateCat) {
          while (_selectedItems.Count > 1) {
            _selectedItems.RemoveAt(1);
          }

          _catalogueIndex = -1;

          _catalogueNames = _cataloguedata.Item1;
          _catalogueNumbers = _cataloguedata.Item2;

          _typeIndex = -1;
          UpdateTypeData();

          _sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(_typeNumbers,
            Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);

          _selectedItems.Add(_catalogueNames[0]);
          _selectedItems.Add(_typeNames[0]);
          _selectedItems.Add(_sectionList[0]);

          Mode1Clicked();
        }

        while (_dropDownItems.Count > 1) {
          _dropDownItems.RemoveAt(1);
        }

        _dropDownItems.Add(_catalogueNames);

        if (i == 1) {
          _catalogueIndex = _catalogueNumbers[j];
          _selectedItems[1] = _catalogueNames[j];

          _typedata = MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(_catalogueIndex,
            Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);
          _typeNames = _typedata.Item1;
          _typeNumbers = _typedata.Item2;

          var types = _typeNumbers.ToList();
          types.RemoveAt(0);
          _sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(types,
            Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);

          _selectedItems[2] = _typeNames[0];
          _selectedItems[3] = _sectionList[0];
        }

        _dropDownItems.Add(_typeNames);

        if (i == 2) {
          _typeIndex = _typeNumbers[j];
          _selectedItems[2] = _typeNames[j];

          List<int> types;
          if (_typeIndex == -1) {
            types = _typeNumbers.ToList();
            types.RemoveAt(0);
          } else {
            types = new List<int> {
              _typeIndex,
            };
          }

          _sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(types,
            Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);

          _selectedItems[3] = _sectionList[0];
        }

        _dropDownItems.Add(_sectionList);

        if (i == 3) {
          _selectedItems[3] = _sectionList[j];
        }

        if (_search == string.Empty) {
          UpdateProfileString();
        }

        base.UpdateUI();
      } else {
        _spacerDescriptions[1] = "Measure";

        if (_mode != FoldMode.Other) {
          while (_dropDownItems.Count > 1) {
            _dropDownItems.RemoveAt(1);
          }

          _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));

          _selectedItems[1] = _lengthUnit.ToString();
        }

        if (i == 0) {
          _type = profileTypes[_selectedItems[0]];
          Mode2Clicked();
        } else {
          _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);

          base.UpdateUI();
        }
      }
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
      } else {
        string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

        int i = 0;
        switch (_type) {
          case "IAngleProfile":
            Params.Input[i].NickName = "D";
            Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
            Params.Input[i].Description
              = "The depth of the angle profile (leg in the local z axis).";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;

            i++;
            Params.Input[i].NickName = "W";
            Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
            Params.Input[i].Description
              = "The width of the angle profile (leg in the local y axis).";
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
            break;

          case "IChannelProfile":
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
            break;

          case "ICircleHollowProfile":
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
            break;

          case "ICircleProfile":
            Params.Input[i].NickName = "Ø";
            Params.Input[i].Name = "Diameter [" + unitAbbreviation + "]";
            Params.Input[i].Description = "The diameter of the circle.";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;
            break;

          case "ICruciformSymmetricalProfile":
            Params.Input[i].NickName = "D";
            Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
            Params.Input[i].Description = "The depth (local z axis leg) of the profile.";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;

            i++;
            Params.Input[i].NickName = "B";
            Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
            Params.Input[i].Description
              = "The width of the flange (local y axis leg) of the cruciform.";
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
            break;

          case "IEllipseHollowProfile":
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
            break;

          case "IEllipseProfile":
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
            break;

          case "IGeneralCProfile":
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
            break;

          case "IGeneralZProfile":
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
            Params.Input[i].Description
              = "The bottom flange width of the generic z section profile.";
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
            break;

          case "IIBeamAsymmetricalProfile":
            Params.Input[i].NickName = "D";
            Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
            Params.Input[i].Description = "The depth of the profile.";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;

            i++;
            Params.Input[i].NickName = "Bt";
            Params.Input[i].Name = "TopFlangeWidth [" + unitAbbreviation + "]";
            Params.Input[i].Description
              = "The width of the top flange of the beam. Top is relative to the beam local access.";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;

            i++;
            Params.Input[i].NickName = "Bb";
            Params.Input[i].Name = "BottomFlangeWidth [" + unitAbbreviation + "]";
            Params.Input[i].Description
              = "The width of the bottom flange of the beam. Bottom is relative to the beam local access.";
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
            break;

          case "IIBeamCellularProfile":
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
            break;

          case "IIBeamSymmetricalProfile":
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
            break;

          case "IRectangleHollowProfile":
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
            break;

          case "IRectangleProfile":
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
            break;

          case "IRectoEllipseProfile":
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
            break;

          case "ISecantPileProfile":
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
            Params.Input[i].Description
              = "Converts the profile into a wall secant pile profile if true -- Converts the profile into a section secant pile profile if false.";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;
            break;

          case "ISheetPileProfile":
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
            Params.Input[i].Description
              = "The bottom flange width of the sheet pile section profile.";
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
            break;

          case "IStadiumProfile":
            Params.Input[i].NickName = "D";
            Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
            Params.Input[i].Description
              = "The profile's overall depth considering the side length of the rectangle and the radii of the semicircles on the two ends.";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;

            i++;
            Params.Input[i].NickName = "B";
            Params.Input[i].Name = "Width [" + unitAbbreviation + "]";
            Params.Input[i].Description = "The profile's width (diameter of the semicircles).";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;
            break;

          case "ITrapezoidProfile":
            Params.Input[i].NickName = "D";
            Params.Input[i].Name = "Depth [" + unitAbbreviation + "]";
            Params.Input[i].Description = "The depth in z-axis direction of trapezoidal profile.";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;

            i++;
            Params.Input[i].NickName = "Bt";
            Params.Input[i].Name = "TopWidth [" + unitAbbreviation + "]";
            Params.Input[i].Description
              = "The top width of trapezoidal profile. Top is relative to the local z-axis.";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;

            i++;
            Params.Input[i].NickName = "Bb";
            Params.Input[i].Name = "BottomWidth [" + unitAbbreviation + "]";
            Params.Input[i].Description
              = "The bottom width of trapezoidal profile. Bottom is relative to the local z-axis.";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;
            break;

          case "ITSectionProfile":
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
            break;

          case "IPerimeterProfile":
            Params.Input[i].NickName = "B";
            Params.Input[i].Name = "Boundary";
            Params.Input[i].Description = "Planar Brep or closed planar curve.";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = false;

            if (Params.Input.Count == 1) {
              Params.RegisterInputParam(new Param_Plane());
            }

            i++;
            Params.Input[i].NickName = "P";
            Params.Input[i].Name = "Plane";
            Params.Input[i].Description
              = "Optional plane in which to project boundary onto. Profile will get coordinates in this plane.";
            Params.Input[i].Access = GH_ParamAccess.item;
            Params.Input[i].Optional = true;
            break;
        }
      }
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("mode", _mode.ToString());
      writer.SetString("lengthUnit", _lengthUnit.ToString());
      writer.SetBoolean("inclSS", _inclSs);
      writer.SetInt32("NumberOfInputs", _numberOfInputs);
      writer.SetInt32("catalogueIndex", _catalogueIndex);
      writer.SetInt32("typeIndex", _typeIndex);
      writer.SetString("search", _search);
      return base.Write(writer);
    }

    protected override string HtmlHelp_Source() {
      string help
        = "GOTO:https://arup-group.github.io/oasys-combined/adsec-api/api/Oasys.Profiles.html";
      return help;
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Profile type",
        "Measure",
        "Type",
        "Profile",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(profileTypes.Keys.ToList());
      _selectedItems.Add("Rectangle");

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      pManager.AddGenericParameter("Width [" + unitAbbreviation + "]", "B", "Profile width",
        GH_ParamAccess.item);
      pManager.AddGenericParameter("Depth [" + unitAbbreviation + "]", "H", "Profile depth",
        GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddTextParameter("Profile", "Pf", "Profile for a GSA Section", GH_ParamAccess.tree);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      ClearRuntimeMessages();
      foreach (IGH_Param input in Params.Input) {
        input.ClearRuntimeMessages();
      }

      #region catalogue

      ClearRuntimeMessages();
      if (_mode == FoldMode.Catalogue) {
        bool incl = false;
        if (da.GetData(1, ref incl)) {
          if (_inclSs != incl) {
            _inclSs = incl;
            UpdateTypeData();
            _sectionList = MicrosoftSQLiteReader.Instance.GetSectionsDataFromSQLite(_typeNumbers,
              Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);

            _selectedItems[2] = _typeNames[0];
            _dropDownItems[2] = _typeNames;

            _selectedItems[3] = _sectionList[0];
            _dropDownItems[3] = _sectionList;

            base.UpdateUI();
          }
        }

        _search = null;
        string inSearch = string.Empty;
        if (da.GetData(0, ref inSearch)) {
          _search = inSearch.Trim().ToLower().Replace(".", string.Empty).Replace("*", ".*")
           .Replace(" ", ".*");
          if (_search == "cat") {
            string eventName = "EasterCat";
            var properties = new Dictionary<string, object>();
            _ = PostHog.SendToPostHog(GsaGH.PluginInfo.Instance, eventName, properties);
            da.SetDataList(0, easterCat);
            return;
          }

          if (_search.Contains("cat")) {
            string[] s = _search.Split(new[] {
              "cat",
            }, StringSplitOptions.None);
            _search = s[s.Length - 1];
          }

          bool tryHard = Regex.Match(_search, "he[abcm]", RegexOptions.Singleline).Success;

          var filteredlist = new List<string>();
          if (_selectedItems[3] != "All") {
            if (!MatchAndAdd(_selectedItems[3], _search, ref filteredlist, tryHard)) {
              _profileString = new List<string>();
              this.AddRuntimeWarning("No profile found that matches selected profile and search!");
            }
          } else if (_search != string.Empty) {
            foreach (string section in _sectionList) {
              if (MatchAndAdd(section, _search, ref filteredlist, tryHard)) { } else if (
                !_search.Any(char.IsDigit)) {
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
            foreach (string profile in filteredlist) {
              _profileString.Add("CAT " + profile);
            }
          } else {
            this.AddRuntimeWarning("No profile found that matches selection and search!");
          }
        }

        if (_search == null) {
          UpdateProfileString();
        }

        var tree = new DataTree<string>();

        int pathCount = 0;
        if (Params.Output[0].VolatileDataCount > 0) {
          pathCount = Params.Output[0].VolatileData.PathCount;
        }

        var path = new GH_Path(new[] {
          pathCount,
        });
        if (_profileString.Count > 0) {
          tree.AddRange(_profileString, path);
        } else {
          tree.Add(null, path);
        }

        da.SetDataTree(0, tree);
      }

      #endregion

      #region std

      if (_mode != FoldMode.Other) {
        return;
      } else {
        string unit = "(" + Length.GetAbbreviation(_lengthUnit, new CultureInfo("en")) + ") ";
        string profile = "STD ";
        switch (_type) {
          case "IAngleProfile":
            profile += "A" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit);
            break;

          case "IChannelProfile":
            profile += "CH" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit);
            break;

          case "ICircleHollowProfile":
            profile += "CHS" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit);
            break;

          case "ICircleProfile":
            profile += "C" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit);
            break;

          case "ICruciformSymmetricalProfile":
            profile += "X" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit);
            break;

          case "IEllipseHollowProfile":
            profile += "OVAL" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit);
            break;

          case "IEllipseProfile":
            profile += "E" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " 2";
            break;

          case "IGeneralCProfile":
            profile += "GC" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit);
            break;

          case "IGeneralZProfile":
            profile += "GZ" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 4, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 5, _lengthUnit).As(_lengthUnit);
            break;

          case "IIBeamAsymmetricalProfile":
            profile += "GI" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 4, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 5, _lengthUnit).As(_lengthUnit);
            break;

          case "IIBeamCellularProfile":
            profile += "CB" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 4, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 5, _lengthUnit).As(_lengthUnit);
            break;

          case "IIBeamSymmetricalProfile":
            profile += "I" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit);
            break;

          case "IRectangleHollowProfile":
            profile += "RHS" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit);
            break;

          case "IRectangleProfile":
            profile += "R" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit);
            break;

          case "IRectoEllipseProfile":
            profile += "RE" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit) + " 2";
            break;

          case "ISecantPileProfile":
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

            profile += (isWallNotSection ? "SP" : "SPW") + unit
              + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " " + pileCount;
            break;

          case "ISheetPileProfile":
            profile += "SHT" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 4, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 5, _lengthUnit).As(_lengthUnit);
            break;

          case "IStadiumProfile":
            profile += "RC" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit);
            break;

          case "ITrapezoidProfile":
            profile += "TR" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit);
            break;

          case "ITSectionProfile":
            profile += "T" + unit + Input.LengthOrRatio(this, da, 0, _lengthUnit).As(_lengthUnit)
              + " " + Input.LengthOrRatio(this, da, 1, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 2, _lengthUnit).As(_lengthUnit) + " "
              + Input.LengthOrRatio(this, da, 3, _lengthUnit).As(_lengthUnit);
            break;

          case "IPerimeterProfile":
            var perimeter = new ProfileHelper() {
              ProfileType = ProfileHelper.ProfileTypes.Geometric,
            };
            var ghBrep = new GH_Brep();

            if (da.GetData(0, ref ghBrep)) {
              var brep = new Brep();
              if (GH_Convert.ToBrep(ghBrep, ref brep, GH_Conversion.Both)) {
                Curve[] edgeSegments = brep.DuplicateEdgeCurves();
                Curve[] edges = Curve.JoinCurves(edgeSegments);

                var ctrlPts = new Point3dList();
                if (edges[0].TryGetPolyline(out Polyline tempCrv)) {
                  ctrlPts = new Point3dList(tempCrv);
                } else {
                  this.AddRuntimeError("Cannot convert edge to Polyline");
                  return;
                }

                bool localPlaneNotSet = true;
                Plane plane = Plane.Unset;
                if (da.GetData(1, ref plane)) {
                  localPlaneNotSet = false;
                }

                var origin = new Point3d();
                if (localPlaneNotSet) {
                  foreach (Point3d p in ctrlPts) {
                    origin.X += p.X;
                    origin.Y += p.Y;
                    origin.Z += p.Z;
                  }

                  origin.X /= origin.X / ctrlPts.Count;
                  origin.Y /= ctrlPts.Count;
                  origin.Z /= ctrlPts.Count;

                  Plane.FitPlaneToPoints(ctrlPts, out plane);

                  var xDirection = new Vector3d(Math.Abs(plane.XAxis.X), Math.Abs(plane.XAxis.Y),
                    Math.Abs(plane.XAxis.Z));
                  xDirection.Unitize();
                  var yDirection = new Vector3d(Math.Abs(plane.YAxis.X), Math.Abs(plane.YAxis.Y),
                    Math.Abs(plane.YAxis.Z));
                  xDirection.Unitize();

                  Vector3d normal = plane.Normal;
                  normal.Unitize();
                  plane = normal.X == 1 ?
                    Plane.WorldYZ :
                    normal.Y == 1 ?
                      Plane.WorldZX :
                      normal.Z == 1 ?
                        Plane.WorldXY :
                        new Plane(Point3d.Origin, xDirection, yDirection);

                  plane.Origin = origin;
                } else {
                  origin = plane.Origin;
                }

                var translation = Transform.Translation(-origin.X, -origin.Y, -origin.Z);
                var rotation = Transform.ChangeBasis(Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis,
                  plane.XAxis, plane.YAxis, plane.ZAxis);
                if (localPlaneNotSet) {
                  rotation = Transform.ChangeBasis(Vector3d.XAxis, Vector3d.YAxis, Vector3d.ZAxis,
                    plane.YAxis, plane.XAxis, plane.ZAxis);
                }

                perimeter.GeoType = ProfileHelper.GeoTypes.Perim;

                var pts = new List<Point2d>();
                foreach (Point3d pt3d in ctrlPts) {
                  pt3d.Transform(translation);
                  pt3d.Transform(rotation);
                  var pt2d = new Point2d(pt3d);
                  pts.Add(pt2d);
                }

                perimeter.PerimeterPoints = pts;

                if (edges.Length > 1) {
                  var voidPoints = new List<List<Point2d>>();
                  for (int i = 1; i < edges.Length; i++) {
                    ctrlPts.Clear();
                    if (!edges[i].IsPlanar()) {
                      for (int j = 0; j < edges.Length; j++) {
                        edges[j] = Curve.ProjectToPlane(edges[j], plane);
                      }
                    }

                    if (edges[i].TryGetPolyline(out tempCrv)) {
                      ctrlPts = new Point3dList(tempCrv);
                      pts = new List<Point2d>();
                      foreach (Point3d pt3d in ctrlPts) {
                        pt3d.Transform(translation);
                        pt3d.Transform(rotation);
                        var pt2d = new Point2d(pt3d);
                        pts.Add(pt2d);
                      }

                      voidPoints.Add(pts);
                    } else {
                      this.AddRuntimeError("Cannot convert internal edge to Polyline");
                      return;
                    }
                  }

                  perimeter.VoidPoints = voidPoints;
                }
              }
            }

            switch (_lengthUnit) {
              case LengthUnit.Millimeter:
                perimeter.SectUnit = ProfileHelper.SectUnitOptions.UMm;
                break;

              case LengthUnit.Centimeter:
                perimeter.SectUnit = ProfileHelper.SectUnitOptions.UCm;
                break;

              case LengthUnit.Meter:
                perimeter.SectUnit = ProfileHelper.SectUnitOptions.Um;
                break;

              case LengthUnit.Foot:
                perimeter.SectUnit = ProfileHelper.SectUnitOptions.UFt;
                break;

              case LengthUnit.Inch:
                perimeter.SectUnit = ProfileHelper.SectUnitOptions.UIn;
                break;
            }

            da.SetData(0, ConvertSection.ProfileConversion(perimeter));
            return;
          default:
            this.AddRuntimeError("Unable to create profile");
            return;
        }

        da.SetData(0, profile);
      }

      #endregion
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems[0] == "Catalogue") {
        _spacerDescriptions = new List<string>(new[] {
          "Profile type",
          "Catalogue",
          "Type",
          "Profile",
        });

        _catalogueNames = _cataloguedata.Item1;
        _catalogueNumbers = _cataloguedata.Item2;
        _typedata = MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(_catalogueIndex,
          Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);
        _typeNames = _typedata.Item1;
        _typeNumbers = _typedata.Item2;

        Mode1Clicked();

        _profileString = new List<string>() {
          "CAT " + _selectedItems[3],
        };
      } else {
        _spacerDescriptions = new List<string>(new[] {
          "Profile type",
          "Measure",
          "Type",
          "Profile",
        });

        _type = profileTypes[_selectedItems[0]];
        Mode2Clicked();
      }

      base.UpdateUIFromSelectedItems();
    }

    private static Tuple<List<string>, List<int>> GetTypesDataFromSqLite(
      int catalogueIndex, string filePath, bool inclSuperseeded) {
      return MicrosoftSQLiteReader.Instance.GetTypesDataFromSQLite(catalogueIndex, filePath,
        inclSuperseeded);
    }

    private static bool MatchAndAdd(
      string item, string pattern, ref List<string> list, bool tryHard = false) {
      string input = item.ToLower().Replace(".", string.Empty);
      if (Regex.Match(input, pattern, RegexOptions.Singleline).Success) {
        list.Add(item);
        return true;
      }

      if (!tryHard || !Regex.Match(pattern, "he[abcm]", RegexOptions.Singleline).Success) {
        return false;
      }

      string[] substring = pattern.Split(new[] {
        "he",
      }, StringSplitOptions.None);
      int count = 1;
      if (substring[substring.Length - 1].Length > 1
        && !char.IsNumber(substring[substring.Length - 1][1])) {
        count = 2;
      }

      pattern = "he" + substring[substring.Length - 1].Remove(0, count)
        + substring[substring.Length - 1].Substring(0, count);
      if (!Regex.Match(input, pattern, RegexOptions.Singleline).Success) {
        return false;
      }

      list.Add(item);
      return true;
    }

    private void Mode1Clicked() {
      while (Params.Input.Count > 0) {
        Params.UnregisterInputParameter(Params.Input[0], true);
      }

      Params.RegisterInputParam(new Param_String());
      Params.RegisterInputParam(new Param_Boolean());

      _mode = FoldMode.Catalogue;

      base.UpdateUI();
    }

    private void Mode2Clicked() {
      if (_mode != FoldMode.Other) {
        while (Params.Input.Count > 0) {
          Params.UnregisterInputParameter(Params.Input[0], true);
        }

        _mode = FoldMode.Other;
      }

      switch (_type) {
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

    private void SetNumberOfGenericInputs(
      int inputs, bool isSecantPile = false, bool isPerimeter = false) {
      _numberOfInputs = inputs;
      if (_lastInputWasSecant || isSecantPile || isPerimeter) {
        if (Params.Input.Count > 0) {
          Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
          Params.UnregisterInputParameter(Params.Input[Params.Input.Count - 1], true);
        }
      }

      while (Params.Input.Count > inputs) {
        Params.UnregisterInputParameter(Params.Input[inputs], true);
      }

      if (isSecantPile) {
        while (Params.Input.Count > inputs + 2) {
          Params.UnregisterInputParameter(Params.Input[inputs + 2], true);
        }

        inputs -= 2;
      }

      while (Params.Input.Count < inputs) {
        Params.RegisterInputParam(new Param_GenericObject());
      }

      if (isSecantPile) {
        Params.RegisterInputParam(new Param_Integer());
        Params.RegisterInputParam(new Param_Boolean());
        _lastInputWasSecant = true;
      } else {
        _lastInputWasSecant = false;
      }

      if (isPerimeter) {
        Params.RegisterInputParam(new Param_Plane());
      }
    }

    private void UpdateProfileString() {
      if (_selectedItems[3] == "All") {
        _profileString = new List<string>();
        foreach (string profile in _sectionList.Where(profile => profile != "All")) {
          _profileString.Add("CAT " + profile);
        }
      } else {
        _profileString = new List<string>() {
          "CAT " + _selectedItems[3],
        };
      }
    }

    private void UpdateTypeData() {
      _typedata = GetTypesDataFromSqLite(_catalogueIndex,
        Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"), _inclSs);
      _typeNames = _typedata.Item1;
      _typeNumbers = _typedata.Item2;
    }
  }
}
