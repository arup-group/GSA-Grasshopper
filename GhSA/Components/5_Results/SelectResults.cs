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
                    if (selecteditems[1] != "   ")
                    {
                        if (selecteditems[1] != "All")
                        {
                            selecteditems[1] = "A" + CaseID.ToString();
                        }
                        if (dropdownitems.Count > 2)
                            dropdownitems.RemoveAt(2);
                        updateCases = true;
                    }
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
                    if (selecteditems[1] != "   ")
                    {
                        if (selecteditems[1] != "All")
                        {
                            selecteditems[1] = "C" + CaseID.ToString();
                        }
                        updateCases = true;
                    }
                }
            }

            if (i == 1)
            {
                if (selecteditems[i].ToLower() == "all")
                    CaseID = -1;
                else
                {
                    int newID = int.Parse(string.Join("", selecteditems[i].ToCharArray().Where(Char.IsDigit)));
                    if (newID != CaseID)
                    {
                        CaseID = newID;
                        updatePermutations = true;
                    }
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
            "Type",
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
        int tempNodeID = 0;
        Dictionary<Tuple<GsaResult.ResultType, int, int>, GsaResult> Result; // this is the cache object!
        GsaModel gsaModel;
        ReadOnlyDictionary<int, AnalysisCaseResult> analysisCaseResults;
        ReadOnlyDictionary<int, CombinationCaseResult> combinationCaseResults;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
            pManager.AddTextParameter("Result Type", "rT", "Result type. " +
                System.Environment.NewLine + "Accepted inputs are: " +
                System.Environment.NewLine + "'AnalysisCase' or 'Combination'", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Case", "C", "Case ID(s)" +
                System.Environment.NewLine + "Use -1 for 'all'", GH_ParamAccess.item);
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
                            ClearData();
                            Result = new Dictionary<Tuple<GsaResult.ResultType, int, int>, GsaResult>();
                            tempNodeID = gsaModel.Model.Nodes().Keys.First();
                        }
                    }
                    else
                    {
                        // first time
                        gsaModel = in_Model;
                        updateCases = true;
                        Result = new Dictionary<Tuple<GsaResult.ResultType, int, int>, GsaResult>();
                        tempNodeID = gsaModel.Model.Nodes().Keys.First();
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
                        {
                            ResultType = GsaResult.ResultType.AnalysisCase;
                            selecteditems[0] = dropdownitems[0][0];
                        }
                        else if (type.ToUpper().StartsWith("C"))
                        {
                            selecteditems[0] = dropdownitems[0][1];
                            if (ResultType != GsaResult.ResultType.Combination)
                            {
                                ResultType = GsaResult.ResultType.Combination;
                                if (dropdownitems.Count < 3)
                                {
                                    dropdownitems.Add(new List<string>() { "All" });
                                    if (selecteditems.Count < 3)
                                    {
                                        selecteditems.Add("All");
                                    }
                                }
                                if (selecteditems[1] != "   ")
                                {
                                    if (selecteditems[1] != "All")
                                    {
                                        selecteditems[1] = "C" + CaseID.ToString();
                                    }
                                    updateCases = true;
                                }
                            }
                        }
                    }
                }

                // Get analysis case 
                GH_Integer gh_aCase = new GH_Integer();
                if (DA.GetData(2, ref gh_aCase))
                {
                    int analCase = 1;
                    if (GH_Convert.ToInt32(gh_aCase, out analCase, GH_Conversion.Both))
                    {
                        if (ResultType == GsaResult.ResultType.Combination && CaseID != analCase)
                            updatePermutations = true;
                        CaseID = analCase;
                        if (analCase < 1)
                            selecteditems[1] = "All";
                        else
                            selecteditems[1] = (ResultType == GsaResult.ResultType.AnalysisCase) ? "A" : "C" + analCase;
                        
                        updateCases = false;
                    }
                }

                // Get permutation case 
                GH_Integer gh_perm = new GH_Integer();
                if (DA.GetData(3, ref gh_perm))
                {
                    int permutation = -1;
                    if (GH_Convert.ToInt32(gh_perm, out permutation, GH_Conversion.Both))
                    {
                        Permutation = permutation;
                        if (permutation < 1)
                            selecteditems[2] = "All";
                        else
                            selecteditems[2] = "P" + permutation;
                        updatePermutations = false;
                    }
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
                            if (analysisCaseResults == null || analysisCaseResults.Count == 0 )
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The GSA Model contains no results. Please analyse the model first.");
                                return;
                            }
                            dropdownitems[1] = new List<string>();
                            dropdownitems[1].Add("All");
                            // add analysis cases to dropdown menu
                            List<int> caseIDs = analysisCaseResults.Keys.OrderBy(x => x).ToList();
                            foreach (int key in caseIDs)
                                dropdownitems[1].Add("A" + key.ToString());
                            // trim excess dropdown lists
                            if (dropdownitems.Count > 2)
                                dropdownitems.RemoveAt(2);
                            // set selected item to first case
                            selecteditems[1] = dropdownitems[1][1];
                            // remove excess selected items
                            if (selecteditems.Count > 2)
                                selecteditems.RemoveAt(2);
                            if (!caseIDs.Contains(CaseID))
                            {
                                CaseID = caseIDs.First();
                                selecteditems[1] = "A" + CaseID;
                            }
                            updateCases = false;
                            ExpireSolution(true);
                            return;

                        case GsaResult.ResultType.Combination:
                            combinationCaseResults = gsaModel.Model.CombinationCaseResults();
                            if (combinationCaseResults == null || combinationCaseResults.Count == 0)
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The GSA Model contains no Combination Cases.");
                                return;
                            }
                            dropdownitems[1] = new List<string>();
                            dropdownitems[1].Add("All");
                            // add analysis cases to dropdown menu
                            List<int> comboIDs = combinationCaseResults.Keys.OrderBy(x => x).ToList();
                            foreach (int key in comboIDs)
                                dropdownitems[1].Add("C" + key.ToString());
                            // set selected item to first case
                            selecteditems[1] = dropdownitems[1][1];
                            // update permutations
                            if (!comboIDs.Contains(CaseID)) // if we are coming from analysis cases then test if case exist
                            {
                                CaseID = comboIDs.First(); //otherwise revert to first in list
                                selecteditems[1] = "C" + CaseID;
                            }
                            combinationCaseResult = combinationCaseResults[CaseID];
                            tempNodeResult = combinationCaseResult.NodeResults(tempNodeID.ToString());
                            int nP = tempNodeResult[tempNodeResult.Keys.First()].Count;
                            if (dropdownitems.Count < 3)
                                dropdownitems.Add(new List<string>());
                            dropdownitems[2] = new List<string>();
                            dropdownitems[2].Add("All");
                            if (nP > 1)
                                for (int i = 1; i < nP + 1; i++)
                                    dropdownitems[2].Add("P" + i.ToString());
                            updateCases = false;
                            updatePermutations = false;
                            ExpireSolution(true);
                            return;
                    }
                }
                if (ResultType == GsaResult.ResultType.Combination & updatePermutations | Permutation > 0)
                {
                    // calc permutations
                    if (combinationCaseResults == null)
                        combinationCaseResults = gsaModel.Model.CombinationCaseResults();
                    if (!combinationCaseResults.ContainsKey(CaseID))
                        CaseID = combinationCaseResults.Keys.OrderBy(x => x).ToList().First();
                    combinationCaseResult = combinationCaseResults[CaseID];
                    tempNodeResult = combinationCaseResult.NodeResults(tempNodeID.ToString());
                    int nP = tempNodeResult[tempNodeResult.Keys.First()].Count;
                    
                    if (ResultType == GsaResult.ResultType.Combination & updatePermutations)
                    {
                        if (dropdownitems.Count < 3)
                            dropdownitems.Add(new List<string>());
                        dropdownitems[2] = new List<string>();
                        dropdownitems[2].Add("All");
                        if (nP > 1)
                            for (int i = 1; i < nP + 1; i++)
                                dropdownitems[2].Add("P" + i.ToString());
                        updatePermutations = false;
                        ExpireSolution(true);
                        return;
                    }
                    if (Permutation > nP)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Combination Case C" + CaseID + " does not contain permutation " + Permutation);
                        return;
                    }
                }

                // Get results from model and create result object
                switch (ResultType)
                {
                    case GsaResult.ResultType.AnalysisCase:
                        if (CaseID > 0)
                        {
                            if (analysisCaseResults == null)
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Analysis Case Results exist in Model");
                                return;
                            }
                            if (!analysisCaseResults.ContainsKey(CaseID))
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Analysis Case does not exist in model");
                                return;
                            }
                            if (!Result.ContainsKey(new Tuple<GsaResult.ResultType, int, int>(GsaResult.ResultType.AnalysisCase, CaseID, -1)))
                            {
                                Result.Add(new Tuple<GsaResult.ResultType, int, int>(GsaResult.ResultType.AnalysisCase, CaseID, -1),
                                    new GsaResult(gsaModel.Model, analysisCaseResults[CaseID], CaseID));
                            }
                        }
                        else
                        {
                            if (analysisCaseResults == null)
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Analysis Case Results exist in Model");
                                return;
                            }
                            foreach (int key in analysisCaseResults.Keys)
                            {
                                if (!Result.ContainsKey(new Tuple<GsaResult.ResultType, int, int>(GsaResult.ResultType.AnalysisCase, key, -1)))
                                {
                                    Result.Add(new Tuple<GsaResult.ResultType, int, int>(GsaResult.ResultType.AnalysisCase, key, -1),
                                        new GsaResult(gsaModel.Model, analysisCaseResults[key], key));
                                }
                            }
                        }
                        break;

                    case GsaResult.ResultType.Combination:
                        if (CaseID > 0)
                        {
                            if (combinationCaseResults == null)
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Combination Case Results exist in Model");
                                return;
                            }
                            if (!combinationCaseResults.ContainsKey(CaseID))
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Combination Case does not exist in model");
                                return;
                            }
                            if (!Result.ContainsKey(new Tuple<GsaResult.ResultType, int, int>(GsaResult.ResultType.Combination, CaseID, Permutation)))
                            {
                                Result.Add(new Tuple<GsaResult.ResultType, int, int>(GsaResult.ResultType.Combination, CaseID, Permutation),
                                    new GsaResult(gsaModel.Model, combinationCaseResults[CaseID], CaseID, Permutation, tempNodeID));
                            }
                        }
                        else
                        {
                            if (combinationCaseResults == null)
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Combination Case Results exist in Model");
                                return;
                            }
                            foreach (int key in combinationCaseResults.Keys)
                            {
                                if (!Result.ContainsKey(new Tuple<GsaResult.ResultType, int, int>(GsaResult.ResultType.Combination, key, Permutation)))
                                {
                                    Result.Add(new Tuple<GsaResult.ResultType, int, int>(GsaResult.ResultType.Combination, key, Permutation),
                                        new GsaResult(gsaModel.Model, combinationCaseResults[key], key, Permutation, tempNodeID));
                                }
                            }
                        }
                        break;
                }

                if (CaseID > 0)
                {
                    if (ResultType == GsaResult.ResultType.AnalysisCase)
                        DA.SetData(0, new GsaResultGoo(Result[new Tuple<GsaResult.ResultType, int, int>(ResultType, CaseID, -1)]));
                    else
                        DA.SetData(0, new GsaResultGoo(Result[new Tuple<GsaResult.ResultType, int, int>(ResultType, CaseID, Permutation)]));
                }
                else
                {
                    // in case all results are selected
                    List<GsaResultGoo> results = new List<GsaResultGoo>();
                    List<Tuple<GsaResult.ResultType, int, int>> combinations = Result.Keys.ToList();
                    List<int> caseIds = combinations.Where(x => x.Item1 == ResultType).Select(x => x.Item2).ToList();
                    foreach (int id in caseIds)
                        results.Add(new GsaResultGoo(Result[new Tuple<GsaResult.ResultType, int, int>(ResultType, id, -1)]));
                    DA.SetDataList(0, results);
                }
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

