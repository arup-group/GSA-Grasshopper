using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using UnitsNet.Units;
using UnitsNet;
using System.Linq;
using Oasys.Units;
using GsaGH.Util.GH;
using GsaGH.Util.Gsa;
using UnitsNet.GH;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class SelectResult : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("c803bba4-a026-4f95-b588-9d76455a53fa");
        public SelectResult()
          : base("Select Results", "SelRes", "Select AnalysisCase or Combination Result from an analysed GSA model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat5())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.SelectResult;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                dropdownitems.Add(resultTypes);
                dropdownitems.Add(new List<string>() { "   " });

                selecteditems = new List<string>();
                selecteditems.Add(dropdownitems[0][0]);
                selecteditems.Add("   ");

                ResultType = GsaResult.ResultType.AnalysisCase;

                first = false;
            }

            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            if (i == 0) //change is made to first dropdown list
            {
                if (selecteditems[i] == resultTypes[0])
                {
                    if (ResultType == GsaResult.ResultType.AnalysisCase)
                        return;
                    ResultType = GsaResult.ResultType.AnalysisCase;
                    selecteditems[1] = "A" + CaseID.ToString();
                    if (dropdownitems.Count > 2)
                        dropdownitems.RemoveAt(2);
                    updateCases = true;
                }
                else if (selecteditems[i] == resultTypes[1])
                {
                    if (ResultType == GsaResult.ResultType.Combination)
                        return;
                    ResultType = GsaResult.ResultType.Combination;
                    if (dropdownitems.Count < 3)
                    {
                        dropdownitems.Add(new List<string>() { "All" });
                        if (selecteditems.Count < 3)
                        {
                            selecteditems.Add("All");
                        }    
                    }
                    selecteditems[1] = "C" + CaseID.ToString();
                    updateCases = true;
                }
            }

            if (i == 1)
            {
                int newID = int.Parse(string.Join("", selecteditems[i].ToCharArray().Where(Char.IsDigit)));
                if (newID != CaseID)
                {
                    CaseID = newID;
                    updatePermutations = true;
                }
            }

            if (i == 2)
            {
                if (selecteditems[i].ToLower() == "all")
                    Permutation = -1;
                else
                    Permutation = int.Parse(string.Join("", selecteditems[i].ToCharArray().Where(Char.IsDigit)));
                updatePermutations = true;
            }

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        private void UpdateUIFromSelectedItems()
        {
            if (selecteditems[0] == resultTypes[0])
                ResultType = GsaResult.ResultType.AnalysisCase;
            else if (selecteditems[0] == resultTypes[1])
                ResultType = GsaResult.ResultType.Combination;

            CreateAttributes();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }

        // list of lists with all dropdown lists conctent
        List<List<string>> dropdownitems;
        List<string> resultTypes = new List<string>(new string[]
        {
            "AnalysisCase",
            "Combination"
        });
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Result Type",
            "Case ID",
            "Permutation"
        });
        bool first = true;
        #region Input and output
        GsaResult.ResultType ResultType;
        int CaseID = 1;
        int Permutation = -1;
        bool updatePermutations;
        bool updateCases;
        GsaModel gsaModel;
        ReadOnlyDictionary<int, AnalysisCaseResult> analysisCaseResults;
        ReadOnlyDictionary<int, CombinationCaseResult> combinationCaseResults;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
            pManager.AddTextParameter("Result Type", "rT", "Result type. Accepted inputs are: 'AnalysisCase' and 'Combination'", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Case", "C", "Case ID(s)", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Permutation", "P", "Permutation(s) (only applicable for combination cases). " +
                System.Environment.NewLine + "Use -1 for 'all'", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Result", "Res", "GSA Result", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Model to work on
            GsaModel in_Model = new GsaModel();

            // Get Model
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                if (gh_typ.Value is GsaModelGoo)
                {
                    gh_typ.CastTo(ref in_Model);
                    if (gsaModel != null)
                    {
                        if (in_Model.GUID != gsaModel.GUID) // only get results if GUID is not similar
                        {
                            gsaModel = in_Model;
                            updateCases = true;
                        }
                    }
                    else
                    {
                        gsaModel = in_Model;
                        updateCases = true;
                    }
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
                    return;
                }

                // Get analysis case 
                GH_String gh_Type = new GH_String();
                if (DA.GetData(1, ref gh_Type))
                {
                    string type = "";
                    if (GH_Convert.ToString(gh_Type, out type, GH_Conversion.Both))
                    {
                        if (type.ToUpper().StartsWith("A"))
                            ResultType = GsaResult.ResultType.AnalysisCase;
                        else if (type.ToUpper().StartsWith("C"))
                            ResultType = GsaResult.ResultType.Combination;
                    }
                }

                // Get analysis case 
                GH_Integer gh_aCase = new GH_Integer();
                if (DA.GetData(2, ref gh_aCase))
                {
                    int analCase = 1;
                    if (GH_Convert.ToInt32(gh_aCase, out analCase, GH_Conversion.Both))
                        CaseID = analCase;
                }

                // Get permutation case 
                GH_Integer gh_perm = new GH_Integer();
                if (DA.GetData(3, ref gh_perm))
                {
                    int permutation = -1;
                    if (GH_Convert.ToInt32(gh_perm, out permutation, GH_Conversion.Both))
                        Permutation = permutation;
                }

                AnalysisCaseResult analysisCaseResult;
                CombinationCaseResult combinationCaseResult;
                IReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> tempNodeResult;
                // get results
                if (updateCases)
                {
                    switch (ResultType)
                    {
                        case GsaResult.ResultType.AnalysisCase:
                            analysisCaseResults = gsaModel.Model.Results();
                            dropdownitems[1] = new List<string>();
                            // add analysis cases to dropdown menu
                            List<int> caseIDs = analysisCaseResults.Keys.OrderBy(x => x).ToList();
                            foreach (int key in caseIDs)
                                dropdownitems[1].Add("A" + key.ToString());
                            // trim excess dropdown lists
                            if (dropdownitems.Count > 2)
                                dropdownitems.RemoveAt(2);
                            // set selected item to first case
                            selecteditems[1] = dropdownitems[1][0];
                            // remove excess selected items
                            if (selecteditems.Count > 2)
                                selecteditems.RemoveAt(2);
                            updateCases = false;
                            ExpireSolution(true);
                            return;

                        case GsaResult.ResultType.Combination:
                            combinationCaseResults = gsaModel.Model.CombinationCaseResults();
                            dropdownitems[1] = new List<string>();
                            // add analysis cases to dropdown menu
                            List<int> comboIDs = combinationCaseResults.Keys.OrderBy(x => x).ToList();
                            foreach (int key in comboIDs)
                                dropdownitems[1].Add("C" + key.ToString());

                            // update permutations
                            int combo = CaseID;
                            if (!comboIDs.Contains(combo))
                                combo = combinationCaseResults.Keys.First();
                            combinationCaseResult = combinationCaseResults[combo];
                            tempNodeResult = combinationCaseResult.NodeResults("all");
                            int nP = tempNodeResult[CaseID].Count;
                            dropdownitems[2] = new List<string>();
                            dropdownitems[2].Add("All");
                            if (nP > 1)
                            {
                                for (int i = 0; i < nP; i++)
                                {
                                    dropdownitems[2].Add("p" + i.ToString());
                                }
                            }
                            updateCases = false;
                            updatePermutations = false;
                            ExpireSolution(true);
                            return;
                    }
                }
                if (ResultType == GsaResult.ResultType.Combination & updatePermutations)
                {
                    // update permutations
                    int combo = CaseID;
                    if (!combinationCaseResults.ContainsKey(combo))
                        combo = combinationCaseResults.Keys.OrderBy(x => x).ToList().First();
                    combinationCaseResult = combinationCaseResults[combo];
                    tempNodeResult = combinationCaseResult.NodeResults("all");
                    int nP = tempNodeResult[tempNodeResult.Keys.First()].Count;
                    dropdownitems[2] = new List<string>();
                    dropdownitems[2].Add("All");
                    if (nP > 1)
                    {
                        for (int i = 0; i < nP; i++)
                        {
                            dropdownitems[2].Add("p" + i.ToString());
                        }
                    }
                    updatePermutations = false;
                    ExpireSolution(true);
                    return;
                }

                GsaResult result = new GsaResult();
                switch (ResultType)
                {
                    case GsaResult.ResultType.AnalysisCase:
                        if (!analysisCaseResults.ContainsKey(CaseID))
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Analysis case does not exist in model");
                            return;
                        }    
                        result = new GsaResult(gsaModel.Model, analysisCaseResults[CaseID], CaseID);
                        break;

                    case GsaResult.ResultType.Combination:
                        if (!combinationCaseResults.ContainsKey(CaseID))
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Combination case does not exist in model");
                            return;
                        }
                        result = new GsaResult(gsaModel.Model, combinationCaseResults[CaseID], CaseID, Permutation);
                        break;
                }

                DA.SetData(0, new GsaResultGoo(result));
            }
        }
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
            
            first = false;
            UpdateUIFromSelectedItems();
            return base.Read(reader);
        }
        #endregion
        #region IGH_VariableParameterComponent null implementation

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
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
            
        }
        #endregion  
    }
}

