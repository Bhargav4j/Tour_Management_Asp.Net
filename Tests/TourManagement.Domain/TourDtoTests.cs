using Xunit;
using TourManagement.Domain.DTOs;
using System;

namespace TourManagement.Domain.Tests;

/// <summary>
/// Test class for TourDto, TourCreateDto, and TourUpdateDto
/// </summary>
public class TourDtoTests
{
    #region TourDto Tests

    [Fact]
    public void TourDto_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var dto = new TourDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PictureUrl);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void TourDto_AllProperties_CanBeSetAndGet()
    {
        // Arrange
        var dto = new TourDto
        {
            Id = 1,
            TourName = "Grand Canyon Adventure",
            Place = "Arizona",
            Days = 5,
            Price = 599.99m,
            Locations = "Grand Canyon, Las Vegas",
            TourInfo = "Explore the breathtaking Grand Canyon",
            PictureUrl = "https://example.com/tour.jpg",
            CreatedDate = new DateTime(2024, 1, 15),
            IsActive = true
        };

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Grand Canyon Adventure", dto.TourName);
        Assert.Equal("Arizona", dto.Place);
        Assert.Equal(5, dto.Days);
        Assert.Equal(599.99m, dto.Price);
        Assert.Equal("Grand Canyon, Las Vegas", dto.Locations);
        Assert.Equal("Explore the breathtaking Grand Canyon", dto.TourInfo);
        Assert.Equal("https://example.com/tour.jpg", dto.PictureUrl);
        Assert.Equal(new DateTime(2024, 1, 15), dto.CreatedDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void TourDto_PictureUrl_CanBeNull()
    {
        // Arrange
        var dto = new TourDto { PictureUrl = null };

        // Assert
        Assert.Null(dto.PictureUrl);
    }

    [Fact]
    public void TourDto_Price_CanBeZero()
    {
        // Arrange
        var dto = new TourDto { Price = 0 };

        // Assert
        Assert.Equal(0, dto.Price);
    }

    [Fact]
    public void TourDto_Days_CanBeZero()
    {
        // Arrange
        var dto = new TourDto { Days = 0 };

        // Assert
        Assert.Equal(0, dto.Days);
    }

    [Fact]
    public void TourDto_Days_CanBeNegative()
    {
        // Arrange
        var dto = new TourDto { Days = -1 };

        // Assert
        Assert.Equal(-1, dto.Days);
    }

    [Fact]
    public void TourDto_IsActive_CanBeTrue()
    {
        // Arrange
        var dto = new TourDto { IsActive = true };

        // Assert
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void TourDto_IsActive_CanBeFalse()
    {
        // Arrange
        var dto = new TourDto { IsActive = false };

        // Assert
        Assert.False(dto.IsActive);
    }

    #endregion

    #region TourCreateDto Tests

    [Fact]
    public void TourCreateDto_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var dto = new TourCreateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PictureUrl);
        Assert.Equal("System", dto.CreatedBy);
    }

    [Fact]
    public void TourCreateDto_AllProperties_CanBeSetAndGet()
    {
        // Arrange
        var dto = new TourCreateDto
        {
            TourName = "European Adventure",
            Place = "Europe",
            Days = 14,
            Price = 2500.00m,
            Locations = "Paris, Rome, London",
            TourInfo = "Comprehensive European tour",
            PictureUrl = "https://example.com/europe.jpg",
            CreatedBy = "Admin"
        };

        // Assert
        Assert.Equal("European Adventure", dto.TourName);
        Assert.Equal("Europe", dto.Place);
        Assert.Equal(14, dto.Days);
        Assert.Equal(2500.00m, dto.Price);
        Assert.Equal("Paris, Rome, London", dto.Locations);
        Assert.Equal("Comprehensive European tour", dto.TourInfo);
        Assert.Equal("https://example.com/europe.jpg", dto.PictureUrl);
        Assert.Equal("Admin", dto.CreatedBy);
    }

    [Fact]
    public void TourCreateDto_CreatedBy_DefaultsToSystem()
    {
        // Arrange & Act
        var dto = new TourCreateDto();

        // Assert
        Assert.Equal("System", dto.CreatedBy);
    }

    [Fact]
    public void TourCreateDto_CreatedBy_CanBeChanged()
    {
        // Arrange
        var dto = new TourCreateDto { CreatedBy = "AdminUser" };

        // Assert
        Assert.Equal("AdminUser", dto.CreatedBy);
    }

    [Fact]
    public void TourCreateDto_PictureUrl_CanBeNull()
    {
        // Arrange
        var dto = new TourCreateDto { PictureUrl = null };

        // Assert
        Assert.Null(dto.PictureUrl);
    }

    [Fact]
    public void TourCreateDto_Price_CanBeDecimal()
    {
        // Arrange
        var dto = new TourCreateDto { Price = 1234.56m };

        // Assert
        Assert.Equal(1234.56m, dto.Price);
    }

    [Fact]
    public void TourCreateDto_Days_CanBePositive()
    {
        // Arrange
        var dto = new TourCreateDto { Days = 10 };

        // Assert
        Assert.Equal(10, dto.Days);
    }

    [Fact]
    public void TourCreateDto_EmptyStrings_AreValid()
    {
        // Arrange
        var dto = new TourCreateDto
        {
            TourName = string.Empty,
            Place = string.Empty,
            Locations = string.Empty,
            TourInfo = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
    }

    #endregion

    #region TourUpdateDto Tests

    [Fact]
    public void TourUpdateDto_Constructor_InitializesWithDefaults()
    {
        // Arrange & Act
        var dto = new TourUpdateDto();

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PictureUrl);
        Assert.Equal("System", dto.ModifiedBy);
    }

    [Fact]
    public void TourUpdateDto_AllProperties_CanBeSetAndGet()
    {
        // Arrange
        var dto = new TourUpdateDto
        {
            TourName = "Updated Tour",
            Place = "Updated Place",
            Days = 7,
            Price = 799.99m,
            Locations = "Location1, Location2",
            TourInfo = "Updated tour information",
            PictureUrl = "https://example.com/updated.jpg",
            ModifiedBy = "Editor"
        };

        // Assert
        Assert.Equal("Updated Tour", dto.TourName);
        Assert.Equal("Updated Place", dto.Place);
        Assert.Equal(7, dto.Days);
        Assert.Equal(799.99m, dto.Price);
        Assert.Equal("Location1, Location2", dto.Locations);
        Assert.Equal("Updated tour information", dto.TourInfo);
        Assert.Equal("https://example.com/updated.jpg", dto.PictureUrl);
        Assert.Equal("Editor", dto.ModifiedBy);
    }

    [Fact]
    public void TourUpdateDto_ModifiedBy_DefaultsToSystem()
    {
        // Arrange & Act
        var dto = new TourUpdateDto();

        // Assert
        Assert.Equal("System", dto.ModifiedBy);
    }

    [Fact]
    public void TourUpdateDto_ModifiedBy_CanBeChanged()
    {
        // Arrange
        var dto = new TourUpdateDto { ModifiedBy = "EditorUser" };

        // Assert
        Assert.Equal("EditorUser", dto.ModifiedBy);
    }

    [Fact]
    public void TourUpdateDto_PictureUrl_CanBeNull()
    {
        // Arrange
        var dto = new TourUpdateDto { PictureUrl = null };

        // Assert
        Assert.Null(dto.PictureUrl);
    }

    [Fact]
    public void TourUpdateDto_Price_CanBeModified()
    {
        // Arrange
        var dto = new TourUpdateDto { Price = 999.99m };

        // Assert
        Assert.Equal(999.99m, dto.Price);
    }

    [Fact]
    public void TourUpdateDto_Days_CanBeModified()
    {
        // Arrange
        var dto = new TourUpdateDto { Days = 21 };

        // Assert
        Assert.Equal(21, dto.Days);
    }

    [Fact]
    public void TourUpdateDto_EmptyStrings_AreValid()
    {
        // Arrange
        var dto = new TourUpdateDto
        {
            TourName = string.Empty,
            Place = string.Empty,
            Locations = string.Empty,
            TourInfo = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
    }

    #endregion

    #region Comparison Tests

    [Fact]
    public void TourCreateDto_And_TourUpdateDto_HaveSimilarProperties()
    {
        // Arrange
        var createDto = new TourCreateDto
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 500m,
            Locations = "Loc1, Loc2",
            TourInfo = "Info",
            PictureUrl = "url"
        };

        var updateDto = new TourUpdateDto
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 500m,
            Locations = "Loc1, Loc2",
            TourInfo = "Info",
            PictureUrl = "url"
        };

        // Assert
        Assert.Equal(createDto.TourName, updateDto.TourName);
        Assert.Equal(createDto.Place, updateDto.Place);
        Assert.Equal(createDto.Days, updateDto.Days);
        Assert.Equal(createDto.Price, updateDto.Price);
        Assert.Equal(createDto.Locations, updateDto.Locations);
        Assert.Equal(createDto.TourInfo, updateDto.TourInfo);
        Assert.Equal(createDto.PictureUrl, updateDto.PictureUrl);
    }

    #endregion
}
