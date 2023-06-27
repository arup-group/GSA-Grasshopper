using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using GsaGH;
using GsaGH.Components;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using Xunit;

namespace GsaGHTests.Components.Results {
  [Collection("GrasshopperFixture collection")]
  public class ReactionForceDiagramsTests {

    [Fact]
    public void WhenCreated_ThenComponentGuid_ShouldBeValid() {
      var obj = new ReactionForceDiagrams();
      var expectedGuid = new Guid("3f359541-342e-4323-be43-d12c4708f2e5");
      Assert.Equal(expectedGuid, obj.ComponentGuid);
    }

    [Fact]
    public void WhenCreated_ThenExposureValue_ShouldBeValid() {
      var obj = new ReactionForceDiagrams();
      Assert.Equal(GH_Exposure.secondary, obj.Exposure);
    }

    [Fact]
    public void WhenCreated_ThenObject_ShouldNotBeEmpty() {
      var obj = new ReactionForceDiagrams();
      Assert.NotNull(obj);
    }

    [Fact]
    public void WhenCreated_ThenPluginInfoValue_ShouldBeValid() {
      var obj = new ReactionForceDiagrams();
      Assert.Equal(PluginInfo.Instance, obj.PluginInfo);
    }

    [Fact]
    public void WhenCreated_ThenVariablesSetByConstructor_ShouldBeValid() {
      var obj = new ReactionForceDiagrams();
      string expectedName = "Reaction Force Diagrams";
      string expectedNickName = "ReactionForce";
      string expectedDesc = "Diplays GSA Node Reaction Force Results as Vector Diagrams";
      string expectedCategory = CategoryName.Name();
      string expectedSubCategory = SubCategoryName.Cat5();

      Assert.Equal(expectedName, obj.Name);
      Assert.Equal(expectedNickName, obj.NickName);
      Assert.Equal(expectedDesc, obj.Description);
      Assert.Equal(expectedCategory, obj.Category);
      Assert.Equal(expectedSubCategory, obj.SubCategory);
    }

    [Fact]
    public void InputParamsAreValid() {
      var obj = new ReactionForceDiagrams();
      var expectedParam = new GsaResultParameter() {
        Name = "Result",
        NickName = "Res",
        Description = "GSA Result",
        Access = GH_ParamAccess.item,
        Optional = false,
      };
      var expectedStringParam = new Param_String() {
        Name = "Node filter list",
        NickName = "No",
        Description = 
        "Filter results by list (by default 'all')" + Environment.NewLine
        + "Input a GSA List or a text string taking the form:" + Environment.NewLine
        + " 1 11 to 72 step 2 not (XY3 31 to 45)" + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        Access = GH_ParamAccess.item,
        Optional = true,
      };
      var expectedColorParam = new Param_Colour() {
        Name = "Colour",
        NickName = "Co",
        Description
          = "[Optional] Colour to override default colour",
        Access = GH_ParamAccess.item,
        Optional = true,
      };
      var expectedNumberParam = new Param_Number() {
        Name = "Scalar",
        NickName = "x:X",
        Description
          = "Scale the result vectors to a specific size. If left empty, automatic scaling based on model size and maximum result by load cases will be computed.",
        Access = GH_ParamAccess.item,
        Optional = true,
      };

      var expectedInputs = new List<IGH_Param>() {
        expectedParam,
        expectedStringParam,
        expectedColorParam,
        expectedNumberParam,
      };

      List<IGH_Param> actualInputs = obj.Params.Input;

      for (int i = 0; i < actualInputs.Count; i++) {
        Assert.Equal(actualInputs[i].Name, expectedInputs[i].Name);
        Assert.Equal(actualInputs[i].NickName, expectedInputs[i].NickName);
        Assert.Equal(actualInputs[i].Description, expectedInputs[i].Description);
        Assert.Equal(actualInputs[i].Access, expectedInputs[i].Access);
        Assert.Equal(actualInputs[i].Optional, expectedInputs[i].Optional);
      }

      Assert.Equal(actualInputs.Count, expectedInputs.Count);
    }

    [Fact]
    public void OutputParamsAreValid() {
      var obj = new ReactionForceDiagrams();
      var expectedPointParam = new Param_Point() {
        Name = "Anchor Point",
        NickName = "A",
        Description = "Support Node Location",
        Access = GH_ParamAccess.list,
      };
      var expectedVectorParam = new Param_Vector() {
        Name = "Vector",
        NickName = "V",
        Description = "Reaction Force Vector",
        Access = GH_ParamAccess.list,
      };
      var expectedGenericParam = new Param_GenericObject() {
        Name = "Value",
        NickName = "Val",
        Description = "Reaction Force Value",
        Access = GH_ParamAccess.list,
      };

      var expectedOutputs = new List<IGH_Param>() {
        expectedPointParam,
        expectedVectorParam,
        expectedGenericParam,
      };

      List<IGH_Param> actualOutputs = obj.Params.Output;

      for (int i = 0; i < actualOutputs.Count; i++) {
        Assert.Equal(actualOutputs[i].Name, expectedOutputs[i].Name);
        Assert.Equal(actualOutputs[i].NickName, expectedOutputs[i].NickName);
        Assert.Equal(actualOutputs[i].Description, expectedOutputs[i].Description);
        Assert.Equal(actualOutputs[i].Access, expectedOutputs[i].Access);
      }

      Assert.Equal(actualOutputs.Count, expectedOutputs.Count);
    }

    [Fact]
    private void WhenCreateAttributes_ThenAttributes_ShouldBeSet() {
      var obj = new ReactionForceDiagrams();
      obj.CreateAttributes();

      Assert.NotNull(obj.Attributes);
    }

    [Fact]
    private void WhenCreateAttributesCalledBeforeInitialised_ThenInitialising_ShouldBeCalled() {
      var obj = new ReactionForceDiagrams();
      obj.CreateAttributes();

      Assert.NotNull(obj._spacerDescriptions);
      Assert.NotNull(obj._dropDownItems);
      Assert.NotNull(obj._selectedItems);
      Assert.True(obj._isInitialised);
    }

    [Fact]
    private void WhenInitialiseDropdowns_ThenDropDownItems_ShouldBeValid() {
      var obj = new ReactionForceDiagrams();
      obj.InitialiseDropdowns();

      var expectedValues = new List<List<string>> {
        new List<string> {
          "Reaction Fx",
          "Reaction Fy",
          "Reaction Fz",
          "Resolved |F|",
          "Reaction Mxx",
          "Reaction Myy",
          "Reaction Mzz",
          "Resolved |M|",
        },
      };
      Assert.Equal(expectedValues, obj._dropDownItems);
    }

    [Fact]
    private void WhenInitialiseDropdowns_ThenIsInitialisedValue_ShouldBeTrue() {
      var obj = new ReactionForceDiagrams();
      obj.InitialiseDropdowns();

      Assert.True(obj._isInitialised);
    }

    [Fact]
    private void WhenInitialiseDropdowns_ThenSelectedItems_ShouldBeSetTo() {
      var obj = new ReactionForceDiagrams();
      obj.InitialiseDropdowns();

      var expectedValues = new List<string> {
        "Resolved |F|",
      };
      Assert.Equal(expectedValues, obj._selectedItems);
    }

    [Fact]
    private void WhenInitialiseDropdowns_ThenSpacerDescription_ShouldBeSet() {
      var obj = new ReactionForceDiagrams();
      obj.InitialiseDropdowns();

      Assert.Equal(obj._spacerDescriptions, new List<string>() {
        "Component",
      });
    }

    [Fact]
    private void WhenSetSelected_ThenSelectedItems_ShouldBeValid() {
      var obj = new ReactionForceDiagrams();
      string defaultValue = obj._dropDownItems[0][3];
      string expectedValue = obj._dropDownItems[0][0];

      Assert.Equal(obj._selectedItems[0], defaultValue);

      obj.SetSelected(0, 0);

      Assert.Equal(obj._selectedItems[0], expectedValue);
    }
  }
}
