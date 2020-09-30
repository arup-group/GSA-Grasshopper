using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;

using Grasshopper.Kernel.Parameters;
using GsaAPI;
using GhSA.Parameters;
using System.Resources;
using System.Linq;
using GhSA.Util.Gsa;

namespace GhSA.Components
{
    /// <summary>
    /// Component to create a profile text-string
    /// </summary>
    public class CreateProfile : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("ea9741e5-905e-4ecb-8270-a584e3f99aa3");
        public CreateProfile()
          : base("Create Profile", "Profile", "Create Profile text-string for GSA Section",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                SetSelected(loadlistid, loadselectid, isTapered, isHollow, isElliptical, isGeneral, isB2B);
                first = false;
            }

            m_attributes = new UI.ProfileComponentUI(this, SetSelected, dropdowncontents, selections, dropdownspacer, null, isTapered, isHollow, isElliptical, isGeneral, isB2B);
        }

        public void SetSelected(int dropdownlistidd, int selectedidd, bool taper, bool hollow, bool elliptical, bool general, bool b2b)
        {
            loadlistid = dropdownlistidd; 
            loadselectid = selectedidd;

            if (dropdownlistidd > 0)
            {
                if (selections[dropdownlistidd] == null || selections.Count <= dropdownlistidd)
                    selections.Add(dropdowncontents[dropdownlistidd][selectedidd]);
                else
                    selections[dropdownlistidd] = dropdowncontents[dropdownlistidd][selectedidd];
            }
            
            isTapered = taper;
            isHollow = hollow;
            isElliptical = elliptical;
            isGeneral = general;
            isB2B = b2b;

            // update dropdown and spacer lists according to selection in first list
            if (selections == null || selections.Count == 0)
            {
                if (selections == null)
                    selections = new List<string>();
                selections.Add(mainlist[0]);
            }

            if (_mode == FoldMode.Catalogue)
                dropdownspacer = new List<string>(cataloguespacer);
            else if (_mode == FoldMode.Geometric)
                dropdownspacer = new List<string>(geometricspacer);
            else 
                dropdownspacer = new List<string>(standardspacer);

            switch (selections[0])
            {
                case ("Catalogue"):
                    // update inputs
                    Mode1Clicked();

                    // update dropdown lists
                    if (dropdowncontents != null)
                        dropdowncontents.Clear();
                    if (dropdowncontents == null || dropdowncontents.Count == 0)
                    {
                        if (dropdowncontents == null)
                            dropdowncontents = new List<List<string>>();
                        dropdowncontents.Add(mainlist);
                        dropdowncontents.Add(cataloguelist);
                        dropdowncontents.Add(typelist);
                        dropdowncontents.Add(sectionlist);
                    }
                    
                    if (selections.Count < 2)
                    {
                        selections.Add(cataloguelist[0]);
                        selections.Add(typelist[0]);
                        selections.Add(sectionlist[0]);
                    }
                    if (dropdownlistidd > 0)
                    {
                        if (dropdownlistidd == 1)
                            catalogueIndex = selectedidd;
                        if (dropdownlistidd == 2)
                            catalogueTypeIndex = selectedidd;
                        if (dropdownlistidd == 3)
                            catalogueProfileIndex = selectedidd;
                    }
                        
                    break;
                case ("Standard"):
                    if (selections.Count == 1)
                        selections.Add(standardlist[0]);

                    // update inputs
                    switch (selections[1])
                    {
                        case "Rectangle":
                            Mode2Clicked();

                            break;
                        case "Circle":
                            Mode3Clicked();
                            break;
                        case "I section":
                            Mode4Clicked();

                            break;
                        case "Tee":
                            Mode5Clicked();
                            break;
                        case "Channel":
                            Mode6Clicked();
                            break;
                        case "Angle":
                            Mode7Clicked();
                            break;
                        default:
                            selections[1] = standardlist[0];
                            Mode2Clicked();
                            break;
                    }
                    // update dropdown lists
                    if (dropdowncontents != null)
                        dropdowncontents.Clear();
                    if (dropdowncontents == null || dropdowncontents.Count == 0)
                    {
                        dropdowncontents.Add(mainlist);
                        dropdowncontents.Add(standardlist);
                    }
                    
                    break;
                case ("Geometric"):
                    // update inputs
                    Mode8Clicked();

                    // update dropdown lists
                    if (dropdowncontents != null)
                        dropdowncontents.Clear();
                    if (dropdowncontents == null || dropdowncontents.Count == 0)
                    {
                        dropdowncontents.Add(mainlist);
                    }
                    
                    break;
            }
        }

        #endregion

        #region Input and output
        #region dropdown lists
        List<List<string>> dropdowncontents; // list that holds all dropdown contents
        List<string> dropdownspacer; // list that holds all dropdown spacer 
        List<string> selections;
        int loadlistid = -1; // for when component is saved and re-laoded
        int loadselectid = -1; // for when component is saved and re-laoded

        // first dropdown list
        readonly List<string> mainlist = new List<string>(new string[]
        {
            "Catalogue", "Standard", "Geometric"
        });

        // second dropdown list - we set initial value here (or leave blank) and change it  
        // later depending on selection from first dropdown list to one for the sub-lists
        readonly List<string> standardlist = new List<string>(new string[]
        {
            "Rectangle", "Circle", "I section", "Tee", "Channel", "Angle"
        });

        // first sublist for second dropdown list
        readonly List<string> cataloguelist = new List<string>(new string[]
        {
            "Europrofile", "To", "be", "implemented"
        });

        // second sublist for second dropdown list
        readonly List<string> typelist = new List<string>(new string[]
        {
            "EP IPE Beams", "EP HE Beams"
        });
        readonly List<string> sectionlist = new List<string>(new string[]
        {
            "HE100.AA", "HE100.A", "HE100.B", "HE100.M"
        });

        // list of spacers to inform user the content of dropdown
        readonly List<string> cataloguespacer = new List<string>(new string[]
        {
            "Profile type", "Catalogue", "Type", "Profile"
        });
        readonly List<string> standardspacer = new List<string>(new string[]
        {
            "Profile type", "Shape"
        });
        readonly List<string> geometricspacer = new List<string>(new string[]
        {
            "Profile type"
        });
        #endregion

        #region other user selection
        int catalogueIndex;
        int catalogueTypeIndex;
        int catalogueProfileIndex;

        bool isTapered;
        bool isHollow;
        bool isElliptical;
        bool isGeneral;
        bool isB2B;

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Profile", "Prfl", "Profile for GSA Section", GH_ParamAccess.item);
        }
        #endregion
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaProfile profile = new GsaProfile();
            #region catalogue
            if (_mode == FoldMode.Catalogue)
            {
                profile.profileType = GsaProfile.ProfileTypes.Catalogue;

                // need to implement the lists of cross sections
                profile.catalogueIndex = catalogueIndex;
                profile.catalogueProfileIndex = catalogueProfileIndex;
                profile.catalogueTypeIndex = catalogueTypeIndex;
                
            }
            #endregion
            #region geometric
            if (_mode == FoldMode.Geometric)
            {
                profile.profileType = GsaProfile.ProfileTypes.Geometric;
                GH_Brep gh_Brep = new GH_Brep();
                if (DA.GetData(0, ref gh_Brep))
                {
                    Brep brep = new Brep();
                    if(GH_Convert.ToBrep(gh_Brep, ref brep, GH_Conversion.Both))
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
                            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Cannot convert edge to Polyline");
                        }    
                        Plane.FitPlaneToPoints(ctrl_pts, out Plane plane);
                        Rhino.Geometry.Transform xform = Rhino.Geometry.Transform.ChangeBasis(Plane.WorldXY, plane);

                        profile.geoType = GsaProfile.GeoTypes.Perim;

                        List<Point2d> pts = new List<Point2d>();
                        foreach (Point3d pt3d in ctrl_pts)
                        {
                            pt3d.Transform(xform);
                            Point2d pt2d = new Point2d(pt3d);
                            pts.Add(pt2d);
                        }
                        profile.perimeterPoints = pts;

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
                                        pt3d.Transform(xform);
                                        Point2d pt2d = new Point2d(pt3d);
                                        pts.Add(pt2d);
                                    }
                                    voidPoints.Add(pts);
                                }
                                else
                                {
                                    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Cannot convert internal edge  to Polyline");
                                }
                            }
                            profile.voidPoints = voidPoints;
                        }
                    }
                }
                
            }
            #endregion
            #region standard section
            if (_mode != FoldMode.Geometric & _mode != FoldMode.Catalogue)
            {
                profile.profileType = GsaProfile.ProfileTypes.Standard;
                GH_Number gh_d = new GH_Number();
                GH_Number gh_b1 = new GH_Number();
                GH_Number gh_b2 = new GH_Number();
                GH_Number gh_tw1 = new GH_Number();
                GH_Number gh_tw2 = new GH_Number();
                GH_Number gh_tf1 = new GH_Number();
                GH_Number gh_tf2 = new GH_Number();
                switch (selections[1])
                {
                    
                    case "Rectangle":
                        profile.stdShape = GsaProfile.StdShapeOptions.Rectangle;

                        // 0 d
                        DA.GetData(0, ref gh_d);

                        // 1 b1
                        DA.GetData(1, ref gh_b1);

                        if (isTapered)
                        {
                            // 2 b2
                            DA.GetData(2, ref gh_b2);
                        }
                        else
                        {
                            if (isHollow)
                            {
                                // 2 tw
                                DA.GetData(2, ref gh_tw1);

                                // 3 tw
                                DA.GetData(3, ref gh_tf1);
                            }
                        }
                        break;

                    case "Circle":
                        profile.stdShape = GsaProfile.StdShapeOptions.Circle;

                        // 0 d
                        DA.GetData(0, ref gh_d);
                        
                        if (isHollow)
                        {
                            if (isElliptical)
                            {
                                // 1 b1
                                DA.GetData(1, ref gh_b1);

                                // 2 tw
                                DA.GetData(2, ref gh_tw1);
                            }
                            else
                            {
                                // 1 tw
                                DA.GetData(1, ref gh_tw1);
                            }
                        }
                        else
                        {
                            if (isElliptical)
                            {
                                // 1 b1
                                DA.GetData(1, ref gh_b1);
                            }
                        }

                        break;
                    case "I section":
                        profile.stdShape = GsaProfile.StdShapeOptions.I_section;

                        // 0 d
                        DA.GetData(0, ref gh_d);

                        // 1 b1
                        DA.GetData(1, ref gh_b1);

                        // 2 tw1
                        DA.GetData(2, ref gh_tw1);

                        // 3 tf1
                        DA.GetData(3, ref gh_tf1);
                                                
                        if (isGeneral)
                        {
                            if (isTapered)
                            {
                                // 4 b2
                                DA.GetData(4, ref gh_b2);

                                // 5 tw2
                                DA.GetData(5, ref gh_tw2);

                                // 6 tf2
                                DA.GetData(6, ref gh_tf2);
                            }
                            else
                            {
                                // 4 b2
                                DA.GetData(4, ref gh_b2);

                                // 5 tf2
                                DA.GetData(5, ref gh_tf2);
                            }
                        }
                        break;
                    case "Tee":
                        profile.stdShape = GsaProfile.StdShapeOptions.Tee;
                        // 0 d
                        DA.GetData(0, ref gh_d);

                        // 1 b1
                        DA.GetData(1, ref gh_b1);

                        // 2 tf1
                        DA.GetData(2, ref gh_tf1);

                        // 3 tw1
                        DA.GetData(3, ref gh_tw1);

                        if (isTapered)
                        {
                            // 4 tw2
                            DA.GetData(4, ref gh_tw2);
                        }
                        break;
                    case "Channel":
                        profile.stdShape = GsaProfile.StdShapeOptions.Channel;
                        // 0 d
                        DA.GetData(0, ref gh_d);

                        // 1 b1
                        DA.GetData(1, ref gh_b1);

                        // 2 tf1
                        DA.GetData(2, ref gh_tf1);

                        // 3 tw1
                        DA.GetData(3, ref gh_tw1);
                        break;
                    case "Angle":
                        profile.stdShape = GsaProfile.StdShapeOptions.Angle;
                        // 0 d
                        DA.GetData(0, ref gh_d);

                        // 1 b1
                        DA.GetData(1, ref gh_b1);

                        // 2 tf1
                        DA.GetData(2, ref gh_tf1);

                        // 3 tw1
                        DA.GetData(3, ref gh_tw1);
                        break;
                }

                if (gh_d != null)
                {
                    if (GH_Convert.ToDouble(gh_d, out double d, GH_Conversion.Both))
                        profile.d = d;
                }
                if (gh_b1 != null)
                {
                    if (GH_Convert.ToDouble(gh_b1, out double b1, GH_Conversion.Both))
                        profile.b1 = b1;
                }
                if (gh_b2 != null)
                {
                    if (GH_Convert.ToDouble(gh_b2, out double b2, GH_Conversion.Both))
                        profile.b2 = b2;
                }
                if (gh_tw1 != null)
                {
                    if (GH_Convert.ToDouble(gh_tw1, out double tw1, GH_Conversion.Both))
                        profile.tw1 = tw1;
                }
                if (gh_tw2 != null)
                {
                    if (GH_Convert.ToDouble(gh_tw2, out double tw2, GH_Conversion.Both))
                        profile.tw2 = tw2;
                }
                if (gh_tf1 != null)
                {
                    if (GH_Convert.ToDouble(gh_tf1, out double tf1, GH_Conversion.Both))
                        profile.tf1 = tf1;
                }
                if (gh_tf2 != null)
                {
                    if (GH_Convert.ToDouble(gh_tf2, out double tf2, GH_Conversion.Both))
                        profile.tf2 = tf2;
                }
                profile.isB2B = isB2B;
                profile.isElliptical = isElliptical;
                profile.isGeneral = isGeneral;
                profile.isHollow = isHollow;
                profile.isTapered = isTapered;
            }
            #endregion
            #region units
            switch (Util.Unit.LengthSection)
            {
                case "mm":
                    profile.sectUnit = GsaProfile.SectUnitOptions.u_mm;
                    break;
                case "cm":
                    profile.sectUnit = GsaProfile.SectUnitOptions.u_cm;
                    break;
                case "m":
                    profile.sectUnit = GsaProfile.SectUnitOptions.u_m;
                    break;
                case "in":
                    profile.sectUnit = GsaProfile.SectUnitOptions.u_in;
                    break;
                case "ft":
                    profile.sectUnit = GsaProfile.SectUnitOptions.u_ft;
                    break;
            }
            #endregion

            // build string and output
            DA.SetData(0, ConvertSection.ProfileConversion(profile));
        }
        #region menu override
        private enum FoldMode
        {
            Catalogue,
            Rectangle, Circle, I_section, Tee, Channel, Angle,
            Geometric
        }
        private bool first = true;
        private FoldMode _mode = FoldMode.Catalogue;


        private void Mode1Clicked()
        {
            FoldMode myMode = FoldMode.Catalogue;
            if (_mode == myMode)
                return;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param = 0;
            
            //remove input parameters
            while (Params.Input.Count > param)
                Params.UnregisterInputParameter(Params.Input[param], true);

            //register input parameter
            while (Params.Input.Count < param)
                Params.RegisterInputParam(new Param_Number());

            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            FoldMode myMode = FoldMode.Rectangle;
            //if (_mode == myMode)
            //    return;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param;
            if (isTapered)
                param = 3;
            else
            {
                if (isHollow)
                    param = 4;
                else
                    param = 2;
            }
            //handle exception when we come from Geometric mode where 
            //first input paraemter is of curve type and must be deleted
            int par2;
            if (_mode == FoldMode.Geometric)
                par2 = 0;
            else
                par2 = param;
            //remove input parameters
            while (Params.Input.Count > par2)
                Params.UnregisterInputParameter(Params.Input[par2], true);

            //register input parameter
            while (Params.Input.Count < param)
                Params.RegisterInputParam(new Param_Number());

            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode3Clicked()
        {
            FoldMode myMode = FoldMode.Circle;
            //if (_mode == myMode)
            //    return;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param;
            if (isHollow)
            {
                if (isElliptical)
                    param = 3;
                else
                    param = 2;
            }
            else
            {
                if (isElliptical)
                    param = 2;
                else
                    param = 1;
            }

            //remove input parameters
            while (Params.Input.Count > param)
                Params.UnregisterInputParameter(Params.Input[param], true);

            //register input parameter
            while (Params.Input.Count < param)
                Params.RegisterInputParam(new Param_Number());

            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode4Clicked()
        {
            FoldMode myMode = FoldMode.I_section;
            //if (_mode == myMode)
            //    return;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param;
            if (isGeneral)
            {
                if (isTapered)
                    param = 7;
                else
                    param = 6;
            }
            else
                param = 4;

            //remove input parameters
            while (Params.Input.Count > param)
                Params.UnregisterInputParameter(Params.Input[param], true);

            //register input parameter
            while (Params.Input.Count < param)
                Params.RegisterInputParam(new Param_Number());

            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode5Clicked()
        {
            FoldMode myMode = FoldMode.Tee;
            //if (_mode == myMode)
            //    return;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param;
            if (isTapered)
                param = 5;
            else
                param = 4;

            //remove input parameters
            while (Params.Input.Count > param)
                Params.UnregisterInputParameter(Params.Input[param], true);

            //register input parameter
            while (Params.Input.Count < param)
                Params.RegisterInputParam(new Param_Number());

            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode6Clicked()
        {
            FoldMode myMode = FoldMode.Channel;
            //if (_mode == myMode)
            //    return;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param = 4;

            //remove input parameters
            while (Params.Input.Count > param)
                Params.UnregisterInputParameter(Params.Input[param], true);

            //register input parameter
            while (Params.Input.Count < param)
                Params.RegisterInputParam(new Param_Number());

            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode7Clicked()
        {
            FoldMode myMode = FoldMode.Angle;
            //if (_mode == myMode)
            //    return;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            // set number of input parameters
            int param = 4;

            //remove input parameters
            while (Params.Input.Count > param)
                Params.UnregisterInputParameter(Params.Input[param], true);

            //register input parameter
            while (Params.Input.Count < param)
                Params.RegisterInputParam(new Param_Number());

            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        private void Mode8Clicked()
        {
            FoldMode myMode = FoldMode.Geometric;
            if (_mode == myMode)
                return;

            RecordUndoEvent(myMode.ToString() + " Parameter");

            //remove input parameters
            while (Params.Input.Count > 0)
                Params.UnregisterInputParameter(Params.Input[0], true);

            //register input parameter
            Params.RegisterInputParam(new Param_Brep());

            _mode = myMode;

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        #endregion
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            // we need to save all the items that we want to reappear when a GH file is saved and re-opened
            writer.SetInt32("Mode", (int)_mode);
            writer.SetInt32("loadselectid", loadselectid);
            writer.SetInt32("loadlistid", loadlistid);
            writer.SetInt32("catalogueIndex", catalogueIndex);
            writer.SetInt32("catalogueTypeIndex", catalogueTypeIndex);
            writer.SetInt32("catalogueProfileIndex", catalogueProfileIndex);
            writer.SetBoolean("isTapered", isTapered);
            writer.SetBoolean("isHollow", isHollow);
            writer.SetBoolean("isElliptical", isElliptical);
            writer.SetBoolean("isGeneral", isGeneral);
            writer.SetBoolean("isB2B", isB2B);

            // to save the dropdownlist content, spacer list and selection list 
            // loop through the lists and save number of lists as well
            writer.SetInt32("dropdownCount", dropdowncontents.Count);
            for (int i = 0; i< dropdowncontents.Count; i++)
            {
                writer.SetInt32("dropdowncontentsCount" + i, dropdowncontents[i].Count);
                for (int j = 0; j < dropdowncontents[i].Count; j++)
                    writer.SetString("dropdowncontents" + i + j, dropdowncontents[i][j]);
            }
            // spacer list
            writer.SetInt32("spacerCount", dropdownspacer.Count);
            for (int i = 0; i < dropdownspacer.Count; i++)
                writer.SetString("spacercontents" + i, dropdownspacer[i]);
            // selection list
            writer.SetInt32("selectionCount", selections.Count);
            for (int i = 0; i < selections.Count; i++)
                writer.SetString("selectioncontents" + i, selections[i]);



            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            // when a GH file is opened we need to read in the data that was previously set by user

            _mode = (FoldMode)reader.GetInt32("Mode");

            // dropdown content list
            int dropdownCount = reader.GetInt32("dropdownCount");
            dropdowncontents = new List<List<string>>();
            for (int i = 0; i < dropdownCount; i++)
            {
                int dropdowncontentsCount = reader.GetInt32("dropdowncontentsCount" + i);
                List<string> tempcontent = new List<string>();
                for (int j = 0; j < dropdowncontentsCount; j++)
                    tempcontent.Add(reader.GetString("dropdowncontents" + i + j));
                dropdowncontents.Add(tempcontent);
            }
            // spacer list
            int dropdownspacerCount = reader.GetInt32("spacerCount");
            dropdownspacer = new List<string>();
            for (int i = 0; i < dropdownspacerCount; i++)
                dropdownspacer.Add(reader.GetString("spacercontents" + i));
            // selection list
            int selectionsCount = reader.GetInt32("selectionCount");
            selections = new List<string>();
            for (int i = 0; i < selectionsCount; i++)
                selections.Add(reader.GetString("selectioncontents" + i));

            loadlistid = reader.GetInt32("loadlistid");
            loadselectid = reader.GetInt32("loadselectid");

            catalogueIndex = reader.GetInt32("catalogueIndex");
            catalogueTypeIndex = reader.GetInt32("catalogueTypeIndex");
            catalogueProfileIndex = reader.GetInt32("catalogueProfileIndex");

            isTapered = reader.GetBoolean("isTapered");
            isHollow = reader.GetBoolean("isHollow");
            isElliptical = reader.GetBoolean("isElliptical");
            isGeneral = reader.GetBoolean("isGeneral");
            isB2B = reader.GetBoolean("isB2B");

            // we need to recreate the custom UI again as this is created before this read IO is called
            // otherwise the component will not display the selected items on the canvas
            this.CreateAttributes();
            return base.Read(reader);
        }

        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
        {
            return false;
        }
        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return false;
        }
        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
        {
            return null;
        }
        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
        {
            return false;
        }
        #endregion
        #region IGH_VariableParameterComponent null implementation
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
            if (_mode == FoldMode.Rectangle)
            {
                int i = 0;
                Params.Input[i].NickName = "d";
                Params.Input[i].Name = "Depth (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Depth";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "b";
                Params.Input[i].Name = "Width (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Width";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                
                if (isTapered)
                {
                    i++;
                    Params.Input[i-1].NickName = "b1";
                    Params.Input[i-1].Name = "Start Width (" + Util.Unit.LengthSection + ")";
                    Params.Input[i-1].Description = "Section Width at Start";
                    Params.Input[i].NickName = "b2";
                    Params.Input[i].Name = "End Width (" + Util.Unit.LengthSection + ")";
                    Params.Input[i].Description = "Section Width at End";
                    Params.Input[i].Access = GH_ParamAccess.item;
                    Params.Input[i].Optional = false;
                }
                else
                {
                    if (isHollow)
                    {
                        i++;
                        Params.Input[i].NickName = "tw";
                        Params.Input[i].Name = "Web THK  (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Section Web Thickness";
                        Params.Input[i].Access = GH_ParamAccess.item;
                        Params.Input[i].Optional = false;
                        i++;
                        Params.Input[i].NickName = "tf";
                        Params.Input[i].Name = "Flange THK  (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Section Flange Thickness";
                        Params.Input[i].Access = GH_ParamAccess.item;
                        Params.Input[i].Optional = false;
                    }
                }
            }
            if (_mode == FoldMode.Circle)
            {
                int i = 0;
                Params.Input[i].NickName = "d";
                Params.Input[i].Name = "Depth (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Depth";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                if (isHollow)
                {
                    if (isElliptical)
                    {
                        i++;
                        Params.Input[i].NickName = "b";
                        Params.Input[i].Name = "Width (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Section Width";
                        Params.Input[i].Access = GH_ParamAccess.item;
                        Params.Input[i].Optional = false;
                        i++;
                        Params.Input[i].NickName = "tw";
                        Params.Input[i].Name = "Thickness (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Section Thickness";
                        Params.Input[i].Access = GH_ParamAccess.item;
                        Params.Input[i].Optional = false;
                    }
                    else
                    {
                        i++;
                        Params.Input[i].NickName = "tw";
                        Params.Input[i].Name = "Thickness (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Section Thickness";
                        Params.Input[i].Access = GH_ParamAccess.item;
                        Params.Input[i].Optional = false;
                    }
                }
                else
                {
                    if (isElliptical)
                    {
                        i++;
                        Params.Input[i].NickName = "b";
                        Params.Input[i].Name = "Width (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Section Width";
                        Params.Input[i].Access = GH_ParamAccess.item;
                        Params.Input[i].Optional = false;
                    }
                }
                
            }
            if (_mode == FoldMode.I_section)
            {
                int i = 0;
                Params.Input[i].NickName = "d";
                Params.Input[i].Name = "Depth (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Depth";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "b";
                Params.Input[i].Name = "Width (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Width";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "tw";
                Params.Input[i].Name = "Web THK (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Web Thickness";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "tf";
                Params.Input[i].Name = "Flange THK (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Flange Thickness";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;

                if (isGeneral)
                {
                    if (isTapered)
                    {
                        i = 1;
                        Params.Input[i].NickName = "bt";
                        Params.Input[i].Name = "Top Width (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Top Flange Width";
                        i++;
                        Params.Input[i].NickName = "twt";
                        Params.Input[i].Name = "Top Web THK (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Web Thickness at Top of Web";
                        i++;
                        Params.Input[i].NickName = "tft";
                        Params.Input[i].Name = "Top Flange THK (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Top Flange Thickness";
                        i++;
                        Params.Input[i].NickName = "bb";
                        Params.Input[i].Name = "Bottom Width (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Bottom Flange Width";
                        Params.Input[i].Access = GH_ParamAccess.item;
                        Params.Input[i].Optional = false;
                        i++;
                        Params.Input[i].NickName = "twb";
                        Params.Input[i].Name = "Bottom Web THK (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Web Thickness at Bottom of Web";
                        Params.Input[i].Access = GH_ParamAccess.item;
                        Params.Input[i].Optional = false;
                        i++;
                        Params.Input[i].NickName = "tfb";
                        Params.Input[i].Name = "Bottom Flange THK (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Bottom Flange Thickness";
                        Params.Input[i].Access = GH_ParamAccess.item;
                        Params.Input[i].Optional = false;
                    }
                    else
                    {
                        i = 1;
                        Params.Input[i].NickName = "bt";
                        Params.Input[i].Name = "Top Width (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Top Flange Width";
                        i++;
                        Params.Input[i].NickName = "tw";
                        Params.Input[i].Name = "Web Thickness (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Web Thickness";
                        i++;
                        Params.Input[i].NickName = "tft";
                        Params.Input[i].Name = "Top Flange THK (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Top Flange Thickness";
                        i++;
                        Params.Input[i].NickName = "bb";
                        Params.Input[i].Name = "Bottom Width (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Bottom Flange Width";
                        Params.Input[i].Access = GH_ParamAccess.item;
                        Params.Input[i].Optional = false;
                        i++;
                        Params.Input[i].NickName = "tfb";
                        Params.Input[i].Name = "Bottom Flange THK (" + Util.Unit.LengthSection + ")";
                        Params.Input[i].Description = "Bottom Flange Thickness";
                        Params.Input[i].Access = GH_ParamAccess.item;
                        Params.Input[i].Optional = false;
                    }
                }
            }

            if (_mode == FoldMode.Tee)
            {
                int i = 0;
                Params.Input[i].NickName = "d";
                Params.Input[i].Name = "Depth (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Depth";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "b";
                Params.Input[i].Name = "Width (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Width";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "tf";
                Params.Input[i].Name = "Flange Thickness (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Web Thickness";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "tw";
                Params.Input[i].Name = "Web Thickness (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Flange Thickness";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;

                if (isTapered)
                {
                    i = 3;
                    Params.Input[i].NickName = "twt";
                    Params.Input[i].Name = "Top Web THK (" + Util.Unit.LengthSection + ")";
                    Params.Input[i].Description = "Top Flange Thickness";
                    i++;
                    Params.Input[i].NickName = "twb";
                    Params.Input[i].Name = "Bottom Web THK (" + Util.Unit.LengthSection + ")";
                    Params.Input[i].Description = "Bottom Web Thickness";
                    Params.Input[i].Access = GH_ParamAccess.item;
                    Params.Input[i].Optional = false;
                }
            }

            if (_mode == FoldMode.Angle || _mode == FoldMode.Channel)
            {
                int i = 0;
                Params.Input[i].NickName = "d";
                Params.Input[i].Name = "Depth (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Depth";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "b";
                Params.Input[i].Name = "Width (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Flange Width";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "tf";
                Params.Input[i].Name = "Flange Thickness (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Web Thickness";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
                i++;
                Params.Input[i].NickName = "tw";
                Params.Input[i].Name = "Web Thickness (" + Util.Unit.LengthSection + ")";
                Params.Input[i].Description = "Section Flange Thickness";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
            }

            if (_mode == FoldMode.Geometric)
            {
                int i = 0;
                Params.Input[i].NickName = "B";
                Params.Input[i].Name = "Boundary";
                Params.Input[i].Description = "Planar Brep or closed planar curve. ";
                Params.Input[i].Access = GH_ParamAccess.item;
                Params.Input[i].Optional = false;
            }
        }
        #endregion  
    }
}