using System;
using System.Collections.Generic;

using GH_IO.Serialization;

using GsaGH.Helpers.GH;

using Moq;

using Xunit;

namespace GsaGHTests.Helpers.GH.Legend {
  [Collection("GrasshopperFixture collection")]
  public class ContourLegendConfigurationTests {
    private readonly ContourLegendConfiguration legendConfiguration;

    public ContourLegendConfigurationTests() {
      legendConfiguration = new ContourLegendConfiguration();
    }

    [Fact]
    public void WhenInitialisedShouldContainEmptyValues() {
      Assert.NotNull(legendConfiguration.Values);
      Assert.Empty(legendConfiguration.Values);
    }

    [Fact]
    public void WhenInitialisedShouldContainEmptyPositionYValues() {
      Assert.NotNull(legendConfiguration.ValuePositionsY);
      Assert.Empty(legendConfiguration.ValuePositionsY);
    }

    [Fact]
    public void WhenInitialisedIsVisibleShouldBeTrue() {
      Assert.True(legendConfiguration.IsVisible);
    }

    [Fact]
    public void WhenInitialisedScaleShouldBeSet() {
      double defaultScale = 1.0d;
      Assert.Equal(defaultScale, legendConfiguration.Scale);
    }

    [Fact]
    public void WhenInitialisedBitmapShouldBeSet() {
      Assert.NotNull(legendConfiguration.Bitmap);
      Assert.Equal(legendConfiguration.DefaultWidth, legendConfiguration.Bitmap.Width);
      Assert.Equal(legendConfiguration.DefaultHeight, legendConfiguration.Bitmap.Height);
    }

    [Fact]
    public void WhenInitialisedIsLegendDisplayableShouldReturnFalse() {
      Assert.False(legendConfiguration.IsLegendDisplayable());
    }

    [Fact]
    public void SetTextValuesShouldThrowErrorWhenValuesAreNull() {
      Assert.Throws<ArgumentNullException>(() => legendConfiguration.SetTextValues(null));
    }

    [Fact]
    public void SetTextValuesShouldSetValuesCorrectly() {
      var expectedValues = new List<string>() {
        "1",
        "2",
      };
      legendConfiguration.SetTextValues(expectedValues);
      Assert.Equal(expectedValues.Count, legendConfiguration.Values.Count);
      Assert.Same(expectedValues, legendConfiguration.Values);
    }

    [Fact]
    public void SetTextValuesShouldSetEmptyList() {
      legendConfiguration.SetTextValues(new List<string>());
      Assert.NotNull(legendConfiguration.Values);
      Assert.Empty(legendConfiguration.Values);
    }

    [Fact]
    public void SetValuePositionsYShouldThrowErrorWhenValuesAreNull() {
      Assert.Throws<ArgumentNullException>(() => legendConfiguration.SetValuePositionsY(null));
    }

    [Fact]
    public void SetValuePositionsYShouldSetValuesCorrectly() {
      var expectedValues = new List<int>() {
        1,
        2,
      };
      legendConfiguration.SetValuePositionsY(expectedValues);
      Assert.Equal(expectedValues.Count, legendConfiguration.ValuePositionsY.Count);
      Assert.Same(expectedValues, legendConfiguration.ValuePositionsY);
    }

    [Fact]
    public void SetValuePositionsYShouldSetEmptyList() {
      legendConfiguration.SetValuePositionsY(new List<int>());
      Assert.NotNull(legendConfiguration.ValuePositionsY);
      Assert.Empty(legendConfiguration.ValuePositionsY);
    }

    [Fact]
    public void SetScaleShouldThrowErrorWhenScaleIsLessThan0() {
      Assert.Throws<ArgumentOutOfRangeException>(() => legendConfiguration.SetLegendScale(-1));
    }

    [Fact]
    public void SetScaleShouldThrowErrorWhenScaleIsEqual0() {
      Assert.Throws<ArgumentOutOfRangeException>(() => legendConfiguration.SetLegendScale(0));
    }

    [Fact]
    public void SetScaleShouldSetScaleCorrectly() {
      const double expectedScale = 1.5;
      legendConfiguration.SetLegendScale(expectedScale);
      Assert.Equal(expectedScale, legendConfiguration.Scale);
    }

    [Fact]
    public void SetScaleShouldResizeBitmapCorrectly() {
      const double expectedScale = 1.5;
      int expectedWidth = (int)(expectedScale * legendConfiguration.DefaultWidth);
      int expectedHeight = (int)(expectedScale * legendConfiguration.DefaultHeight);
      legendConfiguration.SetLegendScale(expectedScale);
      Assert.Equal(expectedHeight, legendConfiguration.Bitmap.Height);
      Assert.Equal(expectedWidth, legendConfiguration.Bitmap.Width);
    }

    [Fact]
    public void ToggleLegendVisibilityWorksCorrectly() {
      Assert.False(legendConfiguration.ToggleLegendVisibility());
      Assert.True(legendConfiguration.ToggleLegendVisibility());
    }

    [Fact]
    public void IsLegendDisplayableWillReturnTrueIfLegendIsVisibleAndHasValues() {
      Assert.False(legendConfiguration.IsLegendDisplayable());
      legendConfiguration.SetTextValues(new List<string>() {
        "1",
      });
      legendConfiguration.SetValuePositionsY(new List<int>() {
        1,
      });
      Assert.True(legendConfiguration.IsLegendDisplayable());
    }

    [Fact]
    public void IsLegendDisplayableWillReturnFalseIfLegendIsNotVisibleButHasValues() {
      Assert.False(legendConfiguration.IsLegendDisplayable());
      legendConfiguration.ToggleLegendVisibility();
      legendConfiguration.SetTextValues(new List<string>() {
        "1",
      });
      legendConfiguration.SetValuePositionsY(new List<int>() {
        1,
      });
      Assert.False(legendConfiguration.IsLegendDisplayable());
    }

    [Fact]
    public void IsLegendDisplayableWillReturnFalseIfLegendIsVisibleButHasNotValues() {
      legendConfiguration.SetValuePositionsY(new List<int>() {
        1,
      });
      Assert.False(legendConfiguration.IsLegendDisplayable());
    }

    [Fact]
    public void SerialiseWillThrowErrorWhenObjectIsNotSet() {
      Assert.Throws<ArgumentNullException>(() => legendConfiguration.SerializeLegendState(null));
    }

    [Fact]
    public void DeserialiseWillThrowErrorWhenObjectIsNotSet() {
      Assert.Throws<ArgumentNullException>(() => legendConfiguration.DeserializeLegendState(null));
    }

    [Fact]
    public void SerializeAndDeserializeLegendStateWorksCorrectly() {
      var mockWriter = new Mock<GH_IWriter>();
      var mockReader = new Mock<GH_IReader>();

      double scale = 2.0;
      bool visibility = false;
      string scaleKey = ContourLegendConfiguration.ScaleKey;
      string visibilityKey = ContourLegendConfiguration.VisibilityKey;

      legendConfiguration.SetLegendScale(scale);
      legendConfiguration.ToggleLegendVisibility();
      legendConfiguration.SerializeLegendState(mockWriter.Object);

      // Verify that serialization methods were called with correct arguments
      mockWriter.Verify(writer => writer.SetDouble(scaleKey, scale), Times.Once);
      mockWriter.Verify(writer => writer.SetBoolean(visibilityKey, visibility), Times.Once);

      // Mock the reader to return serialized values
      mockReader.Setup(reader => reader.ItemExists(scaleKey)).Returns(true);
      mockReader.Setup(reader => reader.ItemExists(visibilityKey)).Returns(true);
      mockReader.Setup(reader => reader.GetDouble(scaleKey)).Returns(scale);
      mockReader.Setup(reader => reader.GetBoolean(visibilityKey)).Returns(visibility);

      var deserializedConfig = new ContourLegendConfiguration();
      deserializedConfig.DeserializeLegendState(mockReader.Object);

      // Assert
      Assert.Equal(legendConfiguration.Scale, deserializedConfig.Scale);
      Assert.Equal(legendConfiguration.IsVisible, deserializedConfig.IsVisible);

      // Verify that deserialization methods were called
      mockReader.Verify(reader => reader.ItemExists(scaleKey), Times.Once);
      mockReader.Verify(reader => reader.ItemExists(visibilityKey), Times.Once);
      mockReader.Verify(reader => reader.GetDouble(scaleKey), Times.Once);
      mockReader.Verify(reader => reader.GetBoolean(visibilityKey), Times.Once);
    }
  }

}
