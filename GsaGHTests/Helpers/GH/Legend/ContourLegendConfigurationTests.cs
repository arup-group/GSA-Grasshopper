using System;
using System.Collections.Generic;

using GH_IO.Serialization;

using GsaGH.Helpers.GH;

using Moq;

using Xunit;

namespace GsaGHTests.Helpers {
  [Collection("GrasshopperFixture collection")]
  public class ContourLegendConfigurationTests {
    private readonly ContourLegendConfiguration legendConfiguration;

    public ContourLegendConfigurationTests() {
      legendConfiguration = ContourLegendConfiguration.GetDefault();
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
      Assert.Throws<ArgumentOutOfRangeException>(() => legendConfiguration.Scale = -1);
    }

    [Fact]
    public void SetScaleShouldThrowErrorWhenScaleIsEqual0() {
      Assert.Throws<ArgumentOutOfRangeException>(() => legendConfiguration.Scale = 0);
    }

    [Fact]
    public void SetScaleShouldSetScaleCorrectly() {
      const double expectedScale = 1.5;
      legendConfiguration.Scale = expectedScale;
      Assert.Equal(expectedScale, legendConfiguration.Scale);
    }

    [Fact]
    public void SetWidthShouldThrowErrorWhenScaleIsLessThan0() {
      Assert.Throws<ArgumentOutOfRangeException>(() => legendConfiguration.Width = -1);
    }

    [Fact]
    public void SetWidthShouldThrowErrorWhenScaleIsEqual0() {
      Assert.Throws<ArgumentOutOfRangeException>(() => legendConfiguration.Width = 0);
    }

    [Fact]
    public void SetWidthShouldSetScaleCorrectly() {
      const int expectedWidth = 1;
      legendConfiguration.Width = expectedWidth;
      Assert.Equal(expectedWidth, legendConfiguration.Width);
    }

    [Fact]
    public void SetHeightShouldThrowErrorWhenScaleIsLessThan0() {
      Assert.Throws<ArgumentOutOfRangeException>(() => legendConfiguration.Height = -1);
    }

    [Fact]
    public void SetHeightShouldThrowErrorWhenScaleIsEqual0() {
      Assert.Throws<ArgumentOutOfRangeException>(() => legendConfiguration.Height = 0);
    }

    [Fact]
    public void SetHeightShouldSetScaleCorrectly() {
      const int expectedHeight = 1;
      legendConfiguration.Height = expectedHeight;
      Assert.Equal(expectedHeight, legendConfiguration.Height);
    }

    [Fact]
    public void ActualWidthShouldReturnCorrectValue() {
      legendConfiguration.Scale = 2;
      legendConfiguration.Width = 2;
      Assert.Equal(4, legendConfiguration.ActualWidth);
    }

    [Fact]
    public void ActualHeightShouldReturnCorrectValue() {
      legendConfiguration.Scale = 2;
      legendConfiguration.Height = 2;
      Assert.Equal(4, legendConfiguration.ActualHeight);
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
    public void SerializeAndDeserializeLegendStateWorksCorrectly() {
      var mockWriter = new Mock<GH_IWriter>();
      var mockReader = new Mock<GH_IReader>();

      double scale = 2.0;
      bool visibility = false;
      int width = 2;
      int height = 3;
      string scaleKey = ContourLegendConfiguration.ScaleKey;
      string visibilityKey = ContourLegendConfiguration.VisibilityKey;
      string widthKey = ContourLegendConfiguration.WidthKey;
      string heightKey = ContourLegendConfiguration.HeightKey;

      legendConfiguration.ToggleLegendVisibility();
      legendConfiguration.Scale = scale;
      legendConfiguration.Height = height;
      legendConfiguration.Width = width;
      legendConfiguration.SerializeLegendState(mockWriter.Object);

      // Verify that serialization methods were called with correct arguments
      mockWriter.Verify(writer => writer.SetDouble(scaleKey, scale), Times.Once);
      mockWriter.Verify(writer => writer.SetBoolean(visibilityKey, visibility), Times.Once);
      mockWriter.Verify(writer => writer.SetInt32(widthKey, width), Times.Once);
      mockWriter.Verify(writer => writer.SetInt32(heightKey, height), Times.Once);

      // Mock the reader to return serialized values
      mockReader.Setup(reader => reader.ItemExists(scaleKey)).Returns(true);
      mockReader.Setup(reader => reader.ItemExists(visibilityKey)).Returns(true);
      mockReader.Setup(reader => reader.ItemExists(widthKey)).Returns(true);
      mockReader.Setup(reader => reader.ItemExists(heightKey)).Returns(true);
      mockReader.Setup(reader => reader.GetDouble(scaleKey)).Returns(scale);
      mockReader.Setup(reader => reader.GetBoolean(visibilityKey)).Returns(visibility);
      mockReader.Setup(reader => reader.GetInt32(widthKey)).Returns(width);
      mockReader.Setup(reader => reader.GetInt32(heightKey)).Returns(height);

      var deserializedConfig = ContourLegendConfiguration.GetDefault();
      deserializedConfig.DeserializeLegendState(mockReader.Object);

      // Assert
      Assert.Equal(legendConfiguration.Scale, deserializedConfig.Scale);
      Assert.Equal(legendConfiguration.IsVisible, deserializedConfig.IsVisible);
      Assert.Equal(legendConfiguration.Width, deserializedConfig.Width);
      Assert.Equal(legendConfiguration.Height, deserializedConfig.Height);

      // Verify that deserialization methods were called
      mockReader.Verify(reader => reader.ItemExists(scaleKey), Times.Once);
      mockReader.Verify(reader => reader.ItemExists(visibilityKey), Times.Once);
      mockReader.Verify(reader => reader.ItemExists(widthKey), Times.Once);
      mockReader.Verify(reader => reader.ItemExists(heightKey), Times.Once);
      mockReader.Verify(reader => reader.GetDouble(scaleKey), Times.Once);
      mockReader.Verify(reader => reader.GetBoolean(visibilityKey), Times.Once);
      mockReader.Verify(reader => reader.GetInt32(widthKey), Times.Once);
      mockReader.Verify(reader => reader.GetInt32(heightKey), Times.Once);
    }

    [Fact]
    public void VariablesShouldBeSetProperlyWhenCreatingNewConfiguration() {
      var config = new ContourLegendConfiguration(1, 2, 3);
      Assert.Equal(1, config.Width);
      Assert.Equal(2, config.Height);
      Assert.Equal(3, config.Scale);
      Assert.True(config.IsVisible);
      Assert.Empty(config.Values);
      Assert.Empty(config.ValuePositionsY);
    }
  }

}
