using Xunit;
using TourManagement.Domain.DTOs;

namespace TourManagement.Domain.DTOs.Tests;

public class TourDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var dto = new TourDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void TourId_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.TourId = 1;

        // Assert
        Assert.Equal(1, dto.TourId);
    }

    [Fact]
    public void TourName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.TourName = "European Adventure";

        // Assert
        Assert.Equal("European Adventure", dto.TourName);
    }

    [Fact]
    public void TourName_ShouldHaveDefaultValue()
    {
        // Arrange & Act
        var dto = new TourDto();

        // Assert
        Assert.Equal(string.Empty, dto.TourName);
    }

    [Fact]
    public void Place_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.Place = "Paris";

        // Assert
        Assert.Equal("Paris", dto.Place);
    }

    [Fact]
    public void Days_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.Days = 7;

        // Assert
        Assert.Equal(7, dto.Days);
    }

    [Fact]
    public void Price_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.Price = 1500.99m;

        // Assert
        Assert.Equal(1500.99m, dto.Price);
    }

    [Fact]
    public void Locations_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.Locations = "Paris, Rome, Barcelona";

        // Assert
        Assert.Equal("Paris, Rome, Barcelona", dto.Locations);
    }

    [Fact]
    public void TourInfo_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.TourInfo = "An amazing tour";

        // Assert
        Assert.Equal("An amazing tour", dto.TourInfo);
    }

    [Fact]
    public void PictureFileName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.PictureFileName = "tour.jpg";

        // Assert
        Assert.Equal("tour.jpg", dto.PictureFileName);
    }

    [Fact]
    public void PictureFileName_ShouldBeNullable()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.PictureFileName = null;

        // Assert
        Assert.Null(dto.PictureFileName);
    }

    [Fact]
    public void IsActive_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourDto();

        // Act
        dto.IsActive = true;

        // Assert
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void TourDto_ShouldSetAllProperties()
    {
        // Arrange & Act
        var dto = new TourDto
        {
            TourId = 1,
            TourName = "European Adventure",
            Place = "Paris",
            Days = 7,
            Price = 1500.99m,
            Locations = "Paris, Rome, Barcelona",
            TourInfo = "An amazing tour",
            PictureFileName = "tour.jpg",
            IsActive = true
        };

        // Assert
        Assert.Equal(1, dto.TourId);
        Assert.Equal("European Adventure", dto.TourName);
        Assert.Equal("Paris", dto.Place);
        Assert.Equal(7, dto.Days);
        Assert.Equal(1500.99m, dto.Price);
        Assert.Equal("Paris, Rome, Barcelona", dto.Locations);
        Assert.Equal("An amazing tour", dto.TourInfo);
        Assert.Equal("tour.jpg", dto.PictureFileName);
        Assert.True(dto.IsActive);
    }
}

public class TourCreateDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var dto = new TourCreateDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void TourName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.TourName = "European Adventure";

        // Assert
        Assert.Equal("European Adventure", dto.TourName);
    }

    [Fact]
    public void Place_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.Place = "Paris";

        // Assert
        Assert.Equal("Paris", dto.Place);
    }

    [Fact]
    public void Days_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.Days = 7;

        // Assert
        Assert.Equal(7, dto.Days);
    }

    [Fact]
    public void Price_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.Price = 1500.99m;

        // Assert
        Assert.Equal(1500.99m, dto.Price);
    }

    [Fact]
    public void Locations_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.Locations = "Paris, Rome";

        // Assert
        Assert.Equal("Paris, Rome", dto.Locations);
    }

    [Fact]
    public void TourInfo_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.TourInfo = "Tour info";

        // Assert
        Assert.Equal("Tour info", dto.TourInfo);
    }

    [Fact]
    public void PictureFileName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.PictureFileName = "pic.jpg";

        // Assert
        Assert.Equal("pic.jpg", dto.PictureFileName);
    }

    [Fact]
    public void TourCreateDto_ShouldSetAllProperties()
    {
        // Arrange & Act
        var dto = new TourCreateDto
        {
            TourName = "European Adventure",
            Place = "Paris",
            Days = 7,
            Price = 1500.99m,
            Locations = "Paris, Rome",
            TourInfo = "Tour info",
            PictureFileName = "pic.jpg"
        };

        // Assert
        Assert.Equal("European Adventure", dto.TourName);
        Assert.Equal("Paris", dto.Place);
        Assert.Equal(7, dto.Days);
        Assert.Equal(1500.99m, dto.Price);
        Assert.Equal("Paris, Rome", dto.Locations);
        Assert.Equal("Tour info", dto.TourInfo);
        Assert.Equal("pic.jpg", dto.PictureFileName);
    }
}

public class TourUpdateDtoTests
{
    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act
        var dto = new TourUpdateDto();

        // Assert
        Assert.NotNull(dto);
    }

    [Fact]
    public void TourName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.TourName = "Updated Tour";

        // Assert
        Assert.Equal("Updated Tour", dto.TourName);
    }

    [Fact]
    public void Place_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.Place = "Rome";

        // Assert
        Assert.Equal("Rome", dto.Place);
    }

    [Fact]
    public void Days_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.Days = 10;

        // Assert
        Assert.Equal(10, dto.Days);
    }

    [Fact]
    public void Price_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.Price = 2000.00m;

        // Assert
        Assert.Equal(2000.00m, dto.Price);
    }

    [Fact]
    public void Locations_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.Locations = "Rome, Venice";

        // Assert
        Assert.Equal("Rome, Venice", dto.Locations);
    }

    [Fact]
    public void TourInfo_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.TourInfo = "Updated info";

        // Assert
        Assert.Equal("Updated info", dto.TourInfo);
    }

    [Fact]
    public void PictureFileName_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.PictureFileName = "updated.jpg";

        // Assert
        Assert.Equal("updated.jpg", dto.PictureFileName);
    }

    [Fact]
    public void IsActive_ShouldSetAndGetValue()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.IsActive = false;

        // Assert
        Assert.False(dto.IsActive);
    }

    [Fact]
    public void TourUpdateDto_ShouldSetAllProperties()
    {
        // Arrange & Act
        var dto = new TourUpdateDto
        {
            TourName = "Updated Tour",
            Place = "Rome",
            Days = 10,
            Price = 2000.00m,
            Locations = "Rome, Venice",
            TourInfo = "Updated info",
            PictureFileName = "updated.jpg",
            IsActive = false
        };

        // Assert
        Assert.Equal("Updated Tour", dto.TourName);
        Assert.Equal("Rome", dto.Place);
        Assert.Equal(10, dto.Days);
        Assert.Equal(2000.00m, dto.Price);
        Assert.Equal("Rome, Venice", dto.Locations);
        Assert.Equal("Updated info", dto.TourInfo);
        Assert.Equal("updated.jpg", dto.PictureFileName);
        Assert.False(dto.IsActive);
    }
}
