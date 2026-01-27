using Xunit;
using System;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.DTOs.Tests;

public class TourDtoTests
{
    [Fact]
    public void TourDto_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var dto = new TourDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0m, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PicturePath);
        Assert.Equal(default(DateTime), dto.CreatedDate);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void TourDto_SetAllProperties_SetsCorrectly()
    {
        // Arrange
        var dto = new TourDto
        {
            Id = 1,
            TourName = "Paris Tour",
            Place = "Paris",
            Days = 5,
            Price = 1200m,
            Locations = "Paris, Lyon, Nice",
            TourInfo = "Wonderful tour",
            PicturePath = "/images/paris.jpg",
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Paris Tour", dto.TourName);
        Assert.Equal("Paris", dto.Place);
        Assert.Equal(5, dto.Days);
        Assert.Equal(1200m, dto.Price);
        Assert.Equal("Paris, Lyon, Nice", dto.Locations);
        Assert.Equal("Wonderful tour", dto.TourInfo);
        Assert.Equal("/images/paris.jpg", dto.PicturePath);
        Assert.NotNull(dto.CreatedDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void TourDto_SetPrice_WithZeroValue_SetsCorrectly()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.Price = 0m;

        // Assert
        Assert.Equal(0m, dto.Price);
    }

    [Fact]
    public void TourDto_SetDays_WithMaxValue_SetsCorrectly()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.Days = int.MaxValue;

        // Assert
        Assert.Equal(int.MaxValue, dto.Days);
    }

    [Fact]
    public void TourDto_PicturePath_CanBeNull()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.PicturePath = null;

        // Assert
        Assert.Null(dto.PicturePath);
    }
}

public class TourCreateDtoTests
{
    [Fact]
    public void TourCreateDto_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var dto = new TourCreateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0m, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PicturePath);
    }

    [Fact]
    public void TourCreateDto_SetAllProperties_SetsCorrectly()
    {
        // Arrange
        var dto = new TourCreateDto
        {
            TourName = "Berlin Adventure",
            Place = "Berlin",
            Days = 7,
            Price = 1500m,
            Locations = "Berlin, Munich",
            TourInfo = "Amazing tour",
            PicturePath = "/images/berlin.jpg"
        };

        // Assert
        Assert.Equal("Berlin Adventure", dto.TourName);
        Assert.Equal("Berlin", dto.Place);
        Assert.Equal(7, dto.Days);
        Assert.Equal(1500m, dto.Price);
        Assert.Equal("Berlin, Munich", dto.Locations);
        Assert.Equal("Amazing tour", dto.TourInfo);
        Assert.Equal("/images/berlin.jpg", dto.PicturePath);
    }

    [Fact]
    public void TourCreateDto_TourName_CanBeEmpty()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.TourName = string.Empty;

        // Assert
        Assert.Equal(string.Empty, dto.TourName);
    }

    [Fact]
    public void TourCreateDto_PicturePath_CanBeNull()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.PicturePath = null;

        // Assert
        Assert.Null(dto.PicturePath);
    }

    [Fact]
    public void TourCreateDto_SetPrice_WithNegativeValue_SetsValue()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.Price = -100m;

        // Assert
        Assert.Equal(-100m, dto.Price);
    }
}

public class TourUpdateDtoTests
{
    [Fact]
    public void TourUpdateDto_Constructor_InitializesProperties()
    {
        // Arrange & Act
        var dto = new TourUpdateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0m, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PicturePath);
    }

    [Fact]
    public void TourUpdateDto_SetAllProperties_SetsCorrectly()
    {
        // Arrange
        var dto = new TourUpdateDto
        {
            TourName = "Rome Experience",
            Place = "Rome",
            Days = 6,
            Price = 1800m,
            Locations = "Rome, Florence",
            TourInfo = "Incredible tour",
            PicturePath = "/images/rome.jpg"
        };

        // Assert
        Assert.Equal("Rome Experience", dto.TourName);
        Assert.Equal("Rome", dto.Place);
        Assert.Equal(6, dto.Days);
        Assert.Equal(1800m, dto.Price);
        Assert.Equal("Rome, Florence", dto.Locations);
        Assert.Equal("Incredible tour", dto.TourInfo);
        Assert.Equal("/images/rome.jpg", dto.PicturePath);
    }

    [Fact]
    public void TourUpdateDto_Place_CanBeEmpty()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.Place = string.Empty;

        // Assert
        Assert.Equal(string.Empty, dto.Place);
    }

    [Fact]
    public void TourUpdateDto_PicturePath_CanBeNull()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.PicturePath = null;

        // Assert
        Assert.Null(dto.PicturePath);
    }

    [Fact]
    public void TourUpdateDto_SetDays_WithNegativeValue_SetsValue()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.Days = -5;

        // Assert
        Assert.Equal(-5, dto.Days);
    }
}
