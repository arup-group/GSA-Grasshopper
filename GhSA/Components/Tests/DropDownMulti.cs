using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace GhSA.Components
{
    /// <summary>
    /// Dummy component with multi-dropdown functionality, showing how custom UI class UI.MultiDropDownComponentUI shall be called
    /// </summary>
    public class DropDownTest1 : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("460a2412-ce15-49a6-b8da-e512ba92eeec");
        public DropDownTest1()
          : base("DropDownMulti", "DropDown2", "Create GSA Node Support",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat8())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (dropdowncontents == null)
            {
                List<List<string>> dropdownlists = new List<List<string>>();
                dropdownlists.Add(dropdownlist);
                dropdownlists.Add(dropdownitems);
                dropdowncontents = dropdownlists;
            }
            
            m_attributes = new UI.MultiDropDownComponentUI(this, setSelected, dropdowncontents, selections, dropdownspacer, dropdowndescription);
        }

        public void setSelected(int dropdownlistidd, int selectedidd)
        {
            if (dropdownlistidd == 0) // if change is made to first list
            {
                if (selections == null || selections[0] != dropdowncontents[0][selectedidd]) // skip if new selection is same as current selection
                {
                    dropdowncontents[1] = (selectedidd == 0) ? dropdownitems1 : dropdownitems2; // change which list is being displayed in second dropdown
                    if (selections == null)
                    {
                        List<string> tempsel = new List<string>();
                        tempsel.Add(dropdownlist[selectedidd]);
                        selections = tempsel;
                    }
                    else
                    {
                        selections[0] = dropdownlist[selectedidd];
                        selections[1] = dropdowndescription[1];
                    }
                }
            }
            else // change is made to second list
            {
                if (selections != null)
                {
                    if (selections.Count < 2) 
                        selections.Add(dropdowncontents[1][selectedidd]);
                    else
                        selections[1] = dropdowncontents[1][selectedidd];
                }
            }
        }

        #endregion

        #region Input and output
        #region dropdown lists
        List<List<string>> dropdowncontents; // list that holds all dropdown contents
        List<string> selections;

        // first dropdown list
        readonly List<string> dropdownlist = new List<string>(new string[]
        {
            "Concrete", "Steel"
        });

        // second dropdown list - we set initial value here (or leave blank) and change it  
        // later depending on selection from first dropdown list to one for the sub-lists
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "Select material first"
        });

        // first sublist for second dropdown list
        readonly List<string> dropdownitems1 = new List<string>(new string[]
        {
            "C12/15", "C16/20", "C20/25", "C25/30", "C30/37", "C35/45", "C40/50", 
            "C45/55", "C50/60", "C55/67", "C60/75", "C70/85", "C80/95", "C90/105"
        });

        // second sublist for second dropdown list
        readonly List<string> dropdownitems2 = new List<string>(new string[]
        {
            "S235", "S275", "S355", "S420", "S460"
        });

        // list of spacers to inform user the content of dropdown
        readonly List<string> dropdownspacer = new List<string>(new string[]
        {
            "Material", "Grade",
        });

        // initial description to be displayed before user selection
        readonly List<string> dropdowndescription = new List<string>(new string[]
        {
            "Select material", "Select grade",
        });
        #endregion
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            // we need to save all the items that we want to reappear when a GH file is saved and re-opened
            if (selections != null)
            {
                if (selections.Count > 0)
                    writer.SetString("list", selections[0]);
                if (selections.Count > 1)
                    writer.SetString("item", selections[1]);
            }
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            // when a GH file is opened we need to read in the data that was previously set by user
            if (selections == null)
            {
                List<string> tempsel = new List<string>();
                tempsel.Add((string)reader.GetString("list"));
                tempsel.Add((string)reader.GetString("item"));
                selections = tempsel;
            }
            else
            {
                selections[0] = (string)reader.GetString("list");
                selections[1] = (string)reader.GetString("item");
            }
            // we need to recreate the custom UI again as this is created before this read IO is called
            // otherwise the component will not display the selected items on the canvas
            this.CreateAttributes();
            return base.Read(reader);
        }
        #endregion

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Pt", "Point (x, y, z) location of support", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pl", "(Optional) Plane for local axis", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddGenericParameter("Bool6", "Re", "(Optional) Restraint in Bool6 form", GH_ParamAccess.item);
            pManager.AddGenericParameter("Spring", "Sp", "(Optional) GSA Spring", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager.HideParameter(1);
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Node", "Node", "GSA Node with Restraint", GH_ParamAccess.list);
            
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
        }

        #region UI override

        #endregion
    }
}

