using Xunit;
using TourManagement.Application.DTOs;
using System;

namespace TourManagement.Application.DTOs.Tests;

public class TourDtoTests
{
    [Fact]
    public void TourDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new TourDto();
        var date = DateTime.Now;

        // Act
        dto.Id = 1;
        dto.TourName = "Paris Tour";
        dto.Place = "Paris";
        dto.Days = 5;
        dto.Price = 1000;
        dto.Locations = "Eiffel Tower, Louvre";
        dto.TourInfo = "Best of Paris";
        dto.PictureFileName = "paris.jpg";
        dto.CreatedDate = date;
        dto.IsActive = true;

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Paris Tour", dto.TourName);
        Assert.Equal("Paris", dto.Place);
        Assert.Equal(5, dto.Days);
        Assert.Equal(1000, dto.Price);
        Assert.Equal("Eiffel Tower, Louvre", dto.Locations);
        Assert.Equal("Best of Paris", dto.TourInfo);
        Assert.Equal("paris.jpg", dto.PictureFileName);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public void TourCreateDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new TourCreateDto();

        // Act
        dto.TourName = "London Tour";
        dto.Place = "London";
        dto.Days = 3;
        dto.Price = 800;
        dto.Locations = "Big Ben, Tower Bridge";
        dto.TourInfo = "London Highlights";
        dto.PictureFileName = "london.jpg";

        // Assert
        Assert.Equal("London Tour", dto.TourName);
        Assert.Equal("London", dto.Place);
        Assert.Equal(3, dto.Days);
        Assert.Equal(800, dto.Price);
        Assert.Equal("Big Ben, Tower Bridge", dto.Locations);
        Assert.Equal("London Highlights", dto.TourInfo);
        Assert.Equal("london.jpg", dto.PictureFileName);
    }

    [Fact]
    public void TourUpdateDto_PropertiesCanBeSet()
    {
        // Arrange
        var dto = new TourUpdateDto();

        // Act
        dto.TourName = "Updated Tour";
        dto.Place = "Rome";
        dto.Days = 7;
        dto.Price = 1500;
        dto.Locations = "Colosseum";
        dto.TourInfo = "Updated Info";
        dto.PictureFileName = "rome.jpg";

        // Assert
        Assert.Equal("Updated Tour", dto.TourName);
        Assert.Equal("Rome", dto.Place);
        Assert.Equal(7, dto.Days);
        Assert.Equal(1500, dto.Price);
        Assert.Equal("Colosseum", dto.Locations);
        Assert.Equal("Updated Info", dto.TourInfo);
        Assert.Equal("rome.jpg", dto.PictureFileName);
    }
}
