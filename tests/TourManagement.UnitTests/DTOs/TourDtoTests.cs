using TourManagement.Domain.DTOs;
using Xunit;

namespace TourManagement.UnitTests.DTOs;

public class TourDtoTests
{
    [Fact]
    public void TourDto_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var dto = new TourDto();

        // Assert
        Assert.Equal(0, dto.Id);
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PicturePath);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void TourDto_Properties_ShouldBeSettable()
    {
        // Arrange
        var dto = new TourDto();
        var now = DateTime.UtcNow;

        // Act
        dto.Id = 1;
        dto.TourName = "Rome Tour";
        dto.Place = "Rome";
        dto.Days = 5;
        dto.Price = 1200.00m;
        dto.Locations = "Colosseum, Vatican";
        dto.TourInfo = "Explore ancient Rome";
        dto.PicturePath = "/images/rome.jpg";
        dto.CreatedDate = now;
        dto.IsActive = true;

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Rome Tour", dto.TourName);
        Assert.Equal("Rome", dto.Place);
        Assert.Equal(5, dto.Days);
        Assert.Equal(1200.00m, dto.Price);
        Assert.Equal("Colosseum, Vatican", dto.Locations);
        Assert.Equal("Explore ancient Rome", dto.TourInfo);
        Assert.Equal("/images/rome.jpg", dto.PicturePath);
        Assert.Equal(now, dto.CreatedDate);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void TourCreateDto_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var dto = new TourCreateDto();

        // Assert
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PicturePath);
    }

    [Fact]
    public void TourCreateDto_Properties_ShouldBeSettable()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.TourName = "Tokyo Tour";
        dto.Place = "Tokyo";
        dto.Days = 10;
        dto.Price = 3000.00m;
        dto.Locations = "Shibuya, Shinjuku";
        dto.TourInfo = "Experience modern Japan";
        dto.PicturePath = "/images/tokyo.jpg";

        // Assert
        Assert.Equal("Tokyo Tour", dto.TourName);
        Assert.Equal("Tokyo", dto.Place);
        Assert.Equal(10, dto.Days);
        Assert.Equal(3000.00m, dto.Price);
        Assert.Equal("Shibuya, Shinjuku", dto.Locations);
        Assert.Equal("Experience modern Japan", dto.TourInfo);
        Assert.Equal("/images/tokyo.jpg", dto.PicturePath);
    }

    [Fact]
    public void TourUpdateDto_Constructor_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var dto = new TourUpdateDto();

        // Assert
        Assert.Equal(string.Empty, dto.TourName);
        Assert.Equal(string.Empty, dto.Place);
        Assert.Equal(0, dto.Days);
        Assert.Equal(0, dto.Price);
        Assert.Equal(string.Empty, dto.Locations);
        Assert.Equal(string.Empty, dto.TourInfo);
        Assert.Null(dto.PicturePath);
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void TourUpdateDto_Properties_ShouldBeSettable()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.TourName = "London Tour";
        dto.Place = "London";
        dto.Days = 6;
        dto.Price = 1800.00m;
        dto.Locations = "Big Ben, Tower Bridge";
        dto.TourInfo = "Discover historic London";
        dto.PicturePath = "/images/london.jpg";
        dto.IsActive = true;

        // Assert
        Assert.Equal("London Tour", dto.TourName);
        Assert.Equal("London", dto.Place);
        Assert.Equal(6, dto.Days);
        Assert.Equal(1800.00m, dto.Price);
        Assert.Equal("Big Ben, Tower Bridge", dto.Locations);
        Assert.Equal("Discover historic London", dto.TourInfo);
        Assert.Equal("/images/london.jpg", dto.PicturePath);
        Assert.True(dto.IsActive);
    }
}
