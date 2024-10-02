using System.Collections.Generic;

using Grasshopper.Kernel;

namespace GsaGH.Data {

  public class InputAttributes {
    public string NickName { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public GH_ParamAccess Access { get; set; }
    public bool Optional { get; set; }
  }

  public interface IInputManager {
    List<InputAttributes> GetInputs();
  }

  public class FootfallInputManager : IInputManager {
    private readonly bool _selfExcitation;

    private readonly InputAttributes _modalAnalysisTaskAttributes = new InputAttributes() {
      NickName = "T",
      Name = "Modal analysis task",
      Description = "Modal or Ritz analysis task",
      Access = GH_ParamAccess.item,
      Optional = false
    };

    private readonly InputAttributes _responseNodesAttributes = new InputAttributes() {
      NickName = "RN",
      Name = "Response nodes",
      Description = "Node list to define the nodes that the responses will be calculated in the analysis",
      Access = GH_ParamAccess.item,
      Optional = true
    };
    public static readonly InputAttributes _excitationNodesAttributes = new InputAttributes() {
      NickName = "EN",
      Name = "Excitation nodes",
      Description
        = "Node list to define the nodes that will be excited for evaluation of the response of the response nodes",
      Access = GH_ParamAccess.item,
      Optional = true
    };
    private readonly InputAttributes _numberOfFootfallsAttributes = new InputAttributes() {
      NickName = "NF",
      Name = "Number of footfalls",
      Description = "Number of footfalls to be considered in the analysis",
      Access = GH_ParamAccess.item,
      Optional = true
    };

    private readonly InputAttributes _walkerAttributes = new InputAttributes() {
      NickName = "W",
      Name = "Walker",
      Description = "The mass representing a sample walker",
      Access = GH_ParamAccess.item,
      Optional = true
    };

    private readonly InputAttributes _responseDirectionAttributes = new InputAttributes() {
      NickName = "D",
      Name = "Direction of responses",
      Description = "The direction of response in the GSA global axis direction."
        + "\nInput either text string or a integer:" + "\n 1 : Z (vertical)" + "\n 2 : X" + "\n 3 : Y"
        + "\n4 : XY (horizontal)",
      Access = GH_ParamAccess.item,
      Optional = true
    };

    private readonly InputAttributes _frequencyWeightingCurveAttributes = new InputAttributes() {
      NickName = "F",
      Name = "Frequency Weighting Curve",
      Description = "The Frequency Weighting Curve (FWC) is used in calculating the response factors."
        + "\nInput the corresponding integer:" + "\n1 : (Freq. Weighting) Wb (BS6472-1:2008)"
        + "\n2 : (Freq. Weighting) Wd (BS6472-1:2008)" + "\n3 : (Freq. Weighting) Wg (BS6472:1992)",
      Access = GH_ParamAccess.item,
      Optional = true
    };

    private readonly InputAttributes _excitationForcesAttributes = new InputAttributes() {
      NickName = "EF",
      Name = "Excitation forces (DLFs)",
      Description = "This defines the way of the structure to be excited (the dynamic Load Factor to be used)"
        + "\nInput the corresponding integer:" + "\n1 : Walking on floor (AISC SDGS11)"
        + "\n2 : Walking on floor (AISC SDGS11 2nd ed)" + "\n3 : Walking on floor (CCIP-016)"
        + "\n4 : Walking on floor (SCI P354)" + "\n5 : Walking on stair (AISC SDGS11 2nd ed)"
        + "\n6 : Walking on stair (Arup)" + "\n7 : Walking on stair (SCI P354)"
        + "\n8 : Running on floor (AISC SDGS11 2nd)",
      Access = GH_ParamAccess.item,
      Optional = true
    };

    private readonly InputAttributes _dampingAttributes = new InputAttributes() {
      NickName = "DC",
      Name = "Constant Damping",
      Description = "Constant damping in percent",
      Access = GH_ParamAccess.item,
      Optional = true
    };
    public FootfallInputManager(bool selfExcitation) { _selfExcitation = selfExcitation; }

    public List<InputAttributes> GetInputs() {
      return FootfallAttributes(_selfExcitation);
    }

    private List<InputAttributes> FootfallAttributes(bool selfExcitation) {
      var attributes = new List<InputAttributes>() {
        _modalAnalysisTaskAttributes,
        _responseNodesAttributes,
      };

      if (selfExcitation) {
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
