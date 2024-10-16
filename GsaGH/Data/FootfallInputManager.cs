using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;

using GsaGH.Helpers.GH;

namespace GsaGH.Data {

  public interface IInputManager {
    List<InputAttributes> GetInputsForSelfExcitation(bool selfExcitation = true);
  }

  public class FootfallInputManager : IInputManager {
    private bool _selfExcitation;
    private List<InputAttributes> _attributes;

    public static readonly InputAttributes _modalAnalysisTaskAttributes = new InputAttributes() {
      ParamType = new Param_Integer(),
      NickName = "T",
      Name = "Modal analysis task",
      Description = "Modal or Ritz analysis task",
      Access = GH_ParamAccess.item,
      Optional = false,
    };

    public static readonly InputAttributes _responseNodesAttributes = new InputAttributes() {
      ParamType = new Param_String(),
      NickName = "RN",
      Name = "Response nodes",
      Description = "Node list to define the nodes that the responses will be calculated in the analysis",
      Access = GH_ParamAccess.item,
      Optional = true,
    };
    public static readonly InputAttributes _excitationNodesAttributes = new InputAttributes() {
      ParamType = new Param_String(),
      NickName = "EN",
      Name = "Excitation nodes",
      Description
        = "Node list to define the nodes that will be excited for evaluation of the response of the response nodes",
      Access = GH_ParamAccess.item,
      Optional = true,
    };
    public static readonly InputAttributes _numberOfFootfallsAttributes = new InputAttributes() {
      ParamType = new Param_Integer(),
      NickName = "NF",
      Name = "Number of footfalls",
      Description = "Number of footfalls to be considered in the analysis",
      Access = GH_ParamAccess.item,
      Optional = true,
    };

    public static readonly InputAttributes _walkerAttributes = new InputAttributes() {
      ParamType = new Param_Number(),
      NickName = "W",
      Name = "Walker",
      Description = "The mass representing a sample walker",
      Access = GH_ParamAccess.item,
      Optional = true,
    };

    public static readonly InputAttributes _responseDirectionAttributes = new InputAttributes() {
      ParamType = new Param_GenericObject(),
      NickName = "D",
      Name = "Direction of responses",
      Description = "The direction of response in the GSA global axis direction."
        + "\nInput either text string or a integer:" + "\n 1 : Z (vertical)" + "\n 2 : X" + "\n 3 : Y"
        + "\n4 : XY (horizontal)",
      Access = GH_ParamAccess.item,
      Optional = true,
    };

    public static readonly InputAttributes _frequencyWeightingCurveAttributes = new InputAttributes() {
      ParamType = new Param_Integer(),
      NickName = "F",
      Name = "Frequency Weighting Curve",
      Description = "The Frequency Weighting Curve (FWC) is used in calculating the response factors."
        + "\nInput the corresponding integer:" + "\n1 : (Freq. Weighting) Wb (BS6472-1:2008)"
        + "\n2 : (Freq. Weighting) Wd (BS6472-1:2008)" + "\n3 : (Freq. Weighting) Wg (BS6472:1992)",
      Access = GH_ParamAccess.item,
      Optional = true,
    };

    public static readonly InputAttributes _excitationForcesAttributes = new InputAttributes() {
      ParamType = new Param_Integer(),
      NickName = "EF",
      Name = "Excitation forces (DLFs)",
      Description = "This defines the way of the structure to be excited (the dynamic Load Factor to be used)"
        + "\nInput the corresponding integer:" + "\n1 : Walking on floor (AISC SDGS11)"
        + "\n2 : Walking on floor (AISC SDGS11 2nd ed)" + "\n3 : Walking on floor (CCIP-016)"
        + "\n4 : Walking on floor (SCI P354)" + "\n5 : Walking on stair (AISC SDGS11 2nd ed)"
        + "\n6 : Walking on stair (Arup)" + "\n7 : Walking on stair (SCI P354)"
        + "\n8 : Running on floor (AISC SDGS11 2nd)",
      Access = GH_ParamAccess.item,
      Optional = true,
    };

    public static readonly InputAttributes _dampingAttributes = new InputAttributes() {
      ParamType = new Param_Number(),
      NickName = "DC",
      Name = "Constant Damping",
      Description = "Constant damping in percent",
      Access = GH_ParamAccess.item,
      Optional = true,
    };

    public FootfallInputManager(bool selfExcitation) {
      _selfExcitation = selfExcitation;
      _attributes = CreateInputAttributesList();
    }

    public List<InputAttributes> GetInputsForSelfExcitation(bool selfExcitation) {
      return FootfallAttributes(selfExcitation);
    }

    private List<InputAttributes> FootfallAttributes(bool selfExcitation) {
      if (selfExcitation == _selfExcitation) {
        return _attributes;
      }

      _selfExcitation = selfExcitation;
      _attributes = CreateInputAttributesList();

      return _attributes;
    }

    private List<InputAttributes> CreateInputAttributesList() {
      var attributes = new List<InputAttributes>() {
        _modalAnalysisTaskAttributes,
        // _responseNodesAttributes,
      };

      if (_selfExcitation) {
        attributes.Add(_excitationNodesAttributes);
      }

      InputAttributes[] further = {
        _numberOfFootfallsAttributes,
        _walkerAttributes,
        _responseDirectionAttributes,
        _frequencyWeightingCurveAttributes,
        _excitationForcesAttributes,
        _dampingAttributes
      };
      attributes.AddRange(further);
      return attributes;
    }
  }
}
